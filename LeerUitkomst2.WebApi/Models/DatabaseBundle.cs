namespace LeerUitkomst2.WebApi.Models
{
    public class DatabaseBundle
    {
        public string? UserId { get; set; }
        public IDatabaseRepository DatabaseRepository { get; set; }
        public int RequestedId { get; set; }
    }
}
