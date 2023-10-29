using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timeorganizer.DatabaseModels
{
    [Table("UserSession")]
    public class UserSession
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public string DateCreated { get; set; }
        public string ExpirationDate { get; set; }
    }
}
