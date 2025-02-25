namespace LeerUitkomst2.WebApi.Models
{
    public class DatabaseBundle<T>
    {
        public string UserId { get; set; }
        public IDatabaseRepository<T> DatabaseRepository { get; set; }
        public int RequestedId { get; set; }
    }
}
