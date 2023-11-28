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
        public DateTime DateCreated { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string DateCreatedText { get; set; }
        public string ExpirationDateText { get; set; }
    }
}
