using SQLite;

namespace timeorganizer.DatabaseModels //model uzytkownika 
{
    [Table("Users")]
    public class Users
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; } = false;
        public string DataCreated { get; set; }
        public string DataModified { get; set; }

    }
}
