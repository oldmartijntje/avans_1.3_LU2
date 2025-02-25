namespace LeerUitkomst2.WebApi.Models
{
    public class Validator
    {
        static readonly int MAX_ENVIRONMENTS_PER_USER = 5;
        static readonly int MAX_ENVIRONMENT_HEIGHT = 99;
        static readonly int MIN_ENVIRONMENT_HEIGHT = 11;
        static readonly int MAX_ENVIRONMENT_LENGTH = 199;
        static readonly int MIN_ENVIRONMENT_LENGTH = 21;
        static readonly int MAX_ENVIRONMENT_NAME_LENGTH = 25;
        static readonly int MIN_ENVIRONMENT_NAME_LENGTH = 1;

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
            if (environment.Name.Length > Validator.MAX_ENVIRONMENT_NAME_LENGTH || environment.Name.Length < Validator.MIN_ENVIRONMENT_NAME_LENGTH)
            {
                return new DataBoolean(false, $"Name should be {Validator.MIN_ENVIRONMENT_NAME_LENGTH}-{Validator.MAX_ENVIRONMENT_NAME_LENGTH} characters.", "Invalid name length");
            }
            if (environment.MaxLength > Validator.MAX_ENVIRONMENT_LENGTH || environment.MaxLength < Validator.MIN_ENVIRONMENT_LENGTH)
            {
                return new DataBoolean(false, $"MaxLength should be {Validator.MIN_ENVIRONMENT_LENGTH}-{Validator.MAX_ENVIRONMENT_LENGTH}.", "Invalid MaxLength");
            }
            if (environment.MaxHeight > Validator.MAX_ENVIRONMENT_HEIGHT || environment.MaxHeight < Validator.MIN_ENVIRONMENT_HEIGHT)
            {
                return new DataBoolean(false, $"MaxHeight should be {Validator.MIN_ENVIRONMENT_HEIGHT}-{Validator.MAX_ENVIRONMENT_HEIGHT}.", "Invalid MaxHeight");
            }
            if (environmentsNameList.Count > 0)
            {
                return new DataBoolean(false, "Only 1 environment allowed with the same name.", "Name already taken");
            }
            if (amountOfEnvironments >= Validator.MAX_ENVIRONMENTS_PER_USER)
            {
                return new DataBoolean(false, $"You have {amountOfEnvironments} Environments, which is {amountOfEnvironments - Validator.MAX_ENVIRONMENTS_PER_USER + 1} too many to be able to create a new one.", "User reached max Environments");
            }
            return new DataBoolean(true, "Allowed");
        }
    }
}
