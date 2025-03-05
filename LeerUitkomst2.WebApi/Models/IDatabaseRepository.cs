namespace LeerUitkomst2.WebApi.Models
{
    public interface IDatabaseRepository
    {
        Task<Environment2D?> GetSingleEnvironmentByUser(string userId, int requestedId);
    }
}
