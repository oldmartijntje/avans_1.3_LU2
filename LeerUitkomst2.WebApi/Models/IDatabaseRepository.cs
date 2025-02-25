namespace LeerUitkomst2.WebApi.Models
{
    public interface IDatabaseRepository<T>
    {
        Task<T> GetSingleByUser(string userId, int requestedId);
    }
}
