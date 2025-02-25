namespace LeerUitkomst2.WebApi.Models
{
    public class Validator
    {
        static readonly int MAX_ENVIRONMENTS_PER_USER = 5;



        public static bool AllowedToCreateEnvironment(int? amountOfEnvironments)
        {
            if (amountOfEnvironments == null)
            {
                return false;
            }
            return amountOfEnvironments >= Validator.MAX_ENVIRONMENTS_PER_USER;
        }
    }
}
