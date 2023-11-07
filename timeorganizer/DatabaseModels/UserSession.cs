using SQLite;

namespace timeorganizer.DatabaseModels
{
    [Table("UserSessions")]
    public class UserSessions
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public string DateCreated { get; set; }
        public string ExpirationDate { get; set; }
    }
}
