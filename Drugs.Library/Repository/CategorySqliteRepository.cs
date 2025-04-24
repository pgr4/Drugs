using System.Data.SQLite;
using Dapper;
using Drugs.Library.Models;

namespace Drugs.Library.Repository
{
    public class CategorySqliteRepository() : BaseSqliteRepository("Data Source=Categories.db;Version=3;"), IRepository<Category>
    {
        public override void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var sql = @"
                    CREATE TABLE IF NOT EXISTS Categories (
                        CategoryId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        ColorCode INTEGER NULL
                    );";

                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<Category>("SELECT * FROM Categories");
            }
        }

        public async Task AddAsync(Category category)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = "INSERT INTO Categories (Name, ColorCode) VALUES (@Name, @ColorCode)";
                await connection.ExecuteAsync(sql, category);
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<Category?>
                (
                    "SELECT * FROM Categories WHERE CategoryId = @CategoryId", 
                    new { CategoryId = categoryId }
                );
            }
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<Category?>
                (
                    "SELECT * FROM Categories WHERE Name = @Name", 
                    new { Name = name }
                );
            }
        }
    }
}