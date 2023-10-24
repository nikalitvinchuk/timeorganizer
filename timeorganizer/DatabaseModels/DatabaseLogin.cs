using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timeorganizer.DatabaseModels //logowanie do bazy danych
{
    public class DatabaseLogin
    {
        readonly SQLiteConnection database;
        string _dbpath;

        public DatabaseLogin(string dbPath)
        {
            database = new SQLiteConnection(dbPath);
            this._dbpath = dbPath;
            database.Cre<Users>();
            database.CreateTableAsync<TaskComponents>().Wait();
            database.CreateTableAsync<Task>().Wait();
        }

        public Task<Users> GetLoginDataAsync(string userName)
        {
            database.open();
            return database.Table<Users>()
                            .Where(i => i.Email == userName)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveLoginDataAsync(Users loginData)
        {
            return database.InsertAsync(loginData);
        }

    }
}
