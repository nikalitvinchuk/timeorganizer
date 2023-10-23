using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timeorganizer.DatabaseModels
{
    public class LoginModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool isAdmin { get; set; }
        public bool RememberMe { get; set; } = false;
        public DateTime DataCreated { get; set; }

    }
}
