using System.Data.SQLite;
using Dapper;
using Drugs.Library.Models;

namespace Drugs.Library.Repository
{
    public class DrugSqliteRepository() : BaseSqliteRepository("Data Source=Drugs.db;Version=3;"), IRepository<Drug>
    {
        public override void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var sql = @"
                    CREATE TABLE IF NOT EXISTS Drugs (
                        DrugId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        IsFavorite BOOLEAN
                );";

                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task AddAsync(Drug drug)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = "INSERT INTO Drugs (Name, IsFavorite) VALUES (@Name, @IsFavorite)";
                await connection.ExecuteAsync(sql, drug);
            }
        }

        public async Task<IEnumerable<Drug>> GetAllAsync()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<Drug>("SELECT * FROM Drugs");
            }
        }

        public async Task<Drug?> GetDrugByIdAsync(int drugId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<Drug?>
                (
                    "SELECT * FROM Drugs WHERE DrugId = @DrugId", 
                    new { DrugId = drugId }
                );
            }
        }

        public async Task<Drug?> GetDrugByNameAsync(string name)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<Drug?>
                (
                    "SELECT * FROM Drugs WHERE Name = @Name", 
                    new { Name = name }
                );
            }
        }

        public async Task<IEnumerable<Drug>> GetDrugsBySimilarNameAsync(string name)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<Drug>
                (
                    "SELECT * FROM Drugs WHERE LOWER(Name) LIKE LOWER(@Name)", 
                    new { Name = $"%{name}%" }
                );
            }
        }
    }
}