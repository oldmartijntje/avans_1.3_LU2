namespace LeerUitkomst2.WebApi.Models
{
    public class Validator
    {
        static readonly string STATUS_SUCCESS = "Success!";

        static readonly int MAX_ENVIRONMENTS_PER_USER = 5;
        static readonly int MAX_ENVIRONMENT_HEIGHT = 99;
        static readonly int MIN_ENVIRONMENT_HEIGHT = 11;
        static readonly int MAX_ENVIRONMENT_LENGTH = 199;
        static readonly int MIN_ENVIRONMENT_LENGTH = 21;
        static readonly int MAX_ENVIRONMENT_NAME_LENGTH = 25;
        static readonly int MIN_ENVIRONMENT_NAME_LENGTH = 1;

        static readonly float MIN_OBJECT_SCALE = 0.5f;
        static readonly float MAX_OBJECT_SCALE = 5f;

        /// <summary>
        /// Validates wether the user is allowed to create this environment.
        /// It checks wether the user has reached their limit, environment already exists, and if it is legal.
        /// </summary>
        /// <param name="environment">The environment the user wants to create.</param>
        /// <param name="amountOfEnvironments">The amount of environments the user has.</param>
        /// <param name="environmentsNameList">The list of environments with the same name as the user wants to create.</param>
        /// <returns>Wether it is allowed or not.</returns>
        public static DataBoolean AllowedToCreateEnvironment(Environment2DTemplate environment, int? amountOfEnvironments, List<Environment2D> environmentsNameList)
        {
            if (amountOfEnvironments == null)
            {
                return new DataBoolean(false, "Database Error");
            }
            else if (environment.Name.Length > Validator.MAX_ENVIRONMENT_NAME_LENGTH || environment.Name.Length < Validator.MIN_ENVIRONMENT_NAME_LENGTH)
            {
                return new DataBoolean(false, $"Name should be {Validator.MIN_ENVIRONMENT_NAME_LENGTH}-{Validator.MAX_ENVIRONMENT_NAME_LENGTH} characters.", "Invalid name length");
            }
            else if (environment.MaxLength > Validator.MAX_ENVIRONMENT_LENGTH || environment.MaxLength < Validator.MIN_ENVIRONMENT_LENGTH)
            {
                return new DataBoolean(false, $"MaxLength should be {Validator.MIN_ENVIRONMENT_LENGTH}-{Validator.MAX_ENVIRONMENT_LENGTH}.", "Invalid MaxLength");
            }
            else if(environment.MaxHeight > Validator.MAX_ENVIRONMENT_HEIGHT || environment.MaxHeight < Validator.MIN_ENVIRONMENT_HEIGHT)
            {
                return new DataBoolean(false, $"MaxHeight should be {Validator.MIN_ENVIRONMENT_HEIGHT}-{Validator.MAX_ENVIRONMENT_HEIGHT}.", "Invalid MaxHeight");
            }
            else if(environmentsNameList.Count > 0)
            {
                return new DataBoolean(false, "Only 1 environment allowed with the same name.", "Name already taken");
            }
            else if(amountOfEnvironments >= Validator.MAX_ENVIRONMENTS_PER_USER)
            {
                return new DataBoolean(false, $"You have {amountOfEnvironments} Environments, which is {amountOfEnvironments - Validator.MAX_ENVIRONMENTS_PER_USER + 1} too many to be able to create a new one.", "User reached max Environments");
            }
            return new DataBoolean(true, Validator.STATUS_SUCCESS);
        }
    
        /// <summary>
        /// Checks wether or not the user logged in has access to the environment. And wehter it even exists in the first place.
        /// </summary>
        /// <param name="dataBundle">This knows who the user is, what they want to access, and what DB to use</param>
        /// <returns></returns>
        public static async Task<DataBoolean> Environment2DAccessCheck(DatabaseBundle dataBundle)
        {
            if (dataBundle.UserId == null)
            {
                return new DataBoolean(false, "unauthorized", "no user id");
            }
            var environment = await dataBundle.DatabaseRepository.GetSingleEnvironmentByUser(dataBundle.UserId, dataBundle.RequestedId);
            if (environment == null)
            {
                return new DataBoolean(false, "This environment does not exist in this context.", "404");
            }
            if (environment.UserId != dataBundle.UserId)
            {
                return new DataBoolean(false, "This environment does not exist in this context.", "403");
            }
            return new DataBoolean(true, Validator.STATUS_SUCCESS).SetData(environment);

        }

        /// <summary>
        /// Checks wether it is a valid Object2D type
        /// </summary>
        /// <param name="object2D">The Object2D</param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static DataBoolean IsValidObject2D(Object2D object2D, Environment2D? environment)
        {
            if (object2D == null)
            {
                return new DataBoolean(false, "Object2D is null");
            } 
            else if (environment == null)
            {
                return new DataBoolean(false, "Environment2D is null");
            }
            else if (object2D.EnvironmentId != environment.Id)
            {
                return new DataBoolean(false, "Object2D does not belong to this environment", "Object2D does not belong to this environment");
            }
            return IsValidObject2D(object2D as Object2DTemplate, environment);
        }

        /// <summary>
        /// Checks wether it is a valid Object2D type
        /// </summary>
        /// <param name="object2D">The Object2D</param>
        /// <param name="environment">The env it is in.</param>
        /// <returns></returns>
        public static DataBoolean IsValidObject2D(Object2DTemplate object2D, Environment2D? environment)
        {
            if (object2D == null)
            {
                return new DataBoolean(false, "Object2DTemplate is null");
            }
            else if (environment == null)
            {
                return new DataBoolean(false, "Environment2D is null");
            }
            else if (object2D.PositionX < 0 || object2D.PositionX > environment.MaxLength)
            {
                return new DataBoolean(false, "Invalid Position", "PositionX is out of bounds");
            }
            else if (object2D.PositionY < 0 || object2D.PositionY > environment.MaxHeight)
            {
                return new DataBoolean(false, "Invalid Position", "PositionY is out of bounds");
            }
            else if (object2D.ScaleX < Validator.MIN_OBJECT_SCALE || object2D.ScaleY < Validator.MIN_OBJECT_SCALE ||
                object2D.ScaleX > Validator.MAX_OBJECT_SCALE || object2D.ScaleY > Validator.MAX_OBJECT_SCALE)
            {
                return new DataBoolean(false, $"Invalid Scale, Min={Validator.MIN_OBJECT_SCALE}&Max={Validator.MAX_OBJECT_SCALE}");
            }
            else if (object2D.RotationZ < 0 || object2D.RotationZ > 360)
            {
                return new DataBoolean(false, "Invalid Rotation (Degrees°) value, Are you missing a cast?", "Degrees < 0 || Degrees > 360");
            } 
            else if (object2D.PrefabId < 0)
            {
                return new DataBoolean(false, "Invalid Id, can not me smaller than 0.", "PrefabId < 0");
            }
            return new DataBoolean(true, Validator.STATUS_SUCCESS);
        }
    }
}
