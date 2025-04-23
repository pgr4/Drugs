using System.Data.SQLite;
using Dapper;
using Drugs.Library.Models;

namespace Drugs.Library.Repository
{
    public class SideEffectSqliteRepository() : BaseSqliteRepository("Data Source=SideEffects.db;Version=3;"), IRepository<SideEffect>
    {
        public override void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var sql = @"
                    CREATE TABLE IF NOT EXISTS SideEffects (
                        SideEffectId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Description TEXT
                    );";

                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task AddAsync(SideEffect sideEffect)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = "INSERT INTO SideEffects (Name) VALUES (@Name)";
                await connection.ExecuteAsync(sql, sideEffect);
            }
        }

        public async Task<SideEffect?> GetSideEffectByIdAsync(int sideEffectId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<SideEffect?>
                (
                    "SELECT * FROM SideEffects WHERE SideEffectId = @SideEffectId", 
                    new { SideEffectId = sideEffectId }
                );
            }
        }

        public async Task<SideEffect?> GetSideEffectByNameAsync(string name)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<SideEffect?>
                (
                    "SELECT * FROM SideEffects WHERE Name = @Name", 
                    new { Name = name }
                );
            }
        }
    }
}