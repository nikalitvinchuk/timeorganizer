﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timeorganizer.DatabaseModels
{
    public class DatabaseLogin
    {
        readonly SQLiteAsyncConnection database;

        public DatabaseLogin(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<LoginModel>().Wait();
        }

        public Task<LoginModel> GetLoginDataAsync(string userName)
        {
            return database.Table<LoginModel>()
                            .Where(i => i.Email == userName)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveLoginDataAsync(LoginModel loginData)
        {
            return database.InsertAsync(loginData);
        }

    }
}
