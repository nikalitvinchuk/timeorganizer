using SQLite;
using System.Linq.Expressions;

namespace timeorganizer.DatabaseModels
{
    public class DatabaseLogin : IAsyncDisposable // przykładowy plik jak korzystać z połączenia do bazy danych stworzony dla rejestracji RegisterPageviewModel - dane podane z palca
    {
        private const string DbName = "Timeorgranizer.db3";
        private static string DbPath => Path.Combine(FileSystem.AppDataDirectory, DbName);

        private SQLiteAsyncConnection _connection;
        private SQLiteAsyncConnection Database =>
             (_connection ??= new SQLiteAsyncConnection(DbPath,
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache)); // polączenie do bazy

        private async Task CreateTableIfNotExists<TTable>() where TTable : class, new() //tworzenie tabel z klas
        {
            await Database.CreateTableAsync<TTable>();
        }

        private async Task<AsyncTableQuery<TTable>> GetTableAsync<TTable>() where TTable : class, new()  //odczytanie tabeli
        {
            await CreateTableIfNotExists<TTable>();
            return Database.Table<TTable>();
        }

        public async Task<List<TTable>> GetByQuery<TTable>(string query, params object[] args) where TTable : class, new()
        {
            return await Database.QueryAsync<TTable>(query, args);
        } //Dodałem żebym móc wykonywać operacje SQL-a na bazie/ potrzebuję do dynamicznego filtrowania
          //Przykąłd użycie:
          //var query = $"SELECT * FROM Tasks WHERE UserId = {_userId}";
          //var tasks = await _context.GetByQuery<Tasks>(query);
        public async Task<IEnumerable<TTable>> GetAllAsync<TTable>() where TTable : class, new() //odczyt z tabeli 
        {
            var table = await GetTableAsync<TTable>();
            return await table.ToListAsync();
        }

        public async Task<IEnumerable<TTable>> GetFileteredAsync<TTable>(Expression<Func<TTable, bool>> predicate) where TTable : class, new() //wyszukiwanie w tabeli - przekazujemy funckje
        {
            var table = await GetTableAsync<TTable>();
            var tt = await table.Where(predicate).ToListAsync();
            return tt;
        }

        private async Task<TResult> Execute<TTable, TResult>(Func<Task<TResult>> action) where TTable : class, new()
        {
            await CreateTableIfNotExists<TTable>();
            return await action();
        }

        public async Task<TTable> GetItemByKeyAsync<TTable>(object primaryKey) where TTable : class, new() // odczyt po kluczu glowym z tabeli np ID. 
        {
            return await Execute<TTable, TTable>(async () => await Database.GetAsync<TTable>(primaryKey));
        }

        public async Task<bool> AddItemAsync<TTable>(TTable item) where TTable : class, new() //dodawanie do tabeli
        {
            return await Execute<TTable, bool>(async () => await Database.InsertAsync(item) > 0);
        }

        public async Task<bool> UpdateItemAsync<TTable>(TTable item) where TTable : class, new() //aktualziacja w tabeli np zmiana statusu. Przekazujemy caly wiersz zgodny z struktura np Users
        {
            await CreateTableIfNotExists<TTable>();
            return await Database.UpdateAsync(item) > 0;
        }

        public async Task<bool> DeleteItemAsync<TTable>(TTable item) where TTable : class, new()  //usuwanie po obiekcie
        {
            await CreateTableIfNotExists<TTable>();
            return await Database.DeleteAsync(item) > 0;
        }

        public async Task<bool> DeleteItemByKeyAsync<TTable>(object primaryKey) where TTable : class, new() //usuwanie z tabeli
        {
            await CreateTableIfNotExists<TTable>();
            return await Database.DeleteAsync<TTable>(primaryKey) > 0;
        }

        public async ValueTask DisposeAsync() => await _connection?.CloseAsync(); // zamkniecie polaczenia z baza

        public Expression<Func<TTable, bool>> CreatePredicateToFiltred<TTable>(IDictionary<string, object> parameters,IDictionary<object, string> temp)
        {
            string operation = "Equal";
            var param = Expression.Parameter(typeof(TTable), "p");
            Expression? body = null;
            foreach (var pair in parameters)
            {
                foreach (var tmp in temp)
                {
                    if(tmp.Key == pair.Value)
                    {
                        operation = tmp.Value; break; 
                    }
                }
                var member = Expression.Property(param, pair.Key);
                var constant = Expression.Constant(pair.Value);
                var expression = Expression.Equal(member, constant);
                if (operation == "NotEqual")
                {
                    expression = Expression.NotEqual(member, constant);
                   
                }
                
                
                body = body == null ? expression : Expression.AndAlso(body, expression);
            }
            return Expression.Lambda<Func<TTable, bool>>(body, param);
        }
    }
}

