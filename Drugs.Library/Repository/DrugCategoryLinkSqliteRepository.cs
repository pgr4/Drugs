using System.Data.SQLite;
using Dapper;
using Drugs.Library.Models;

namespace Drugs.Library.Repository
{
    public class DrugCategoryLinkSqliteRepository() : BaseSqliteRepository("Data Source=DrugCategoryLinks.db;Version=3;"), IRepository<DrugCategoryLink>
    {
        public override void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var sql = @"
                    CREATE TABLE IF NOT EXISTS DrugCategoryLinks (
                        DrugCategoryLinkId INTEGER PRIMARY KEY AUTOINCREMENT,
                        DrugId INTEGER NOT NULL,
                        CategoryId INTEGER NOT NULL,
                        FOREIGN KEY (DrugId) REFERENCES Drugs(DrugId),
                        FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
                    );";

                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task<IEnumerable<DrugCategoryLink>> GetAllAsync()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<DrugCategoryLink>("SELECT * FROM DrugCategoryLinks");
            }
        }

        public async Task AddAsync(DrugCategoryLink drugCategoryLink)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = "INSERT INTO DrugCategoryLinks (DrugId, CategoryId) VALUES (@DrugId, @CategoryId)";
                await connection.ExecuteAsync(sql, drugCategoryLink);
            }
        }

        public async Task<IEnumerable<DrugCategoryLink>> GetDrugCategoryLinksByDrugIdAsync(int drugId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<DrugCategoryLink>
                (
                    "SELECT * FROM DrugCategoryLinks WHERE DrugId = @DrugId",
                    new { DrugId = drugId } 
                );
            }
        }

        public async Task<IEnumerable<DrugCategoryLink>> GetDrugCategoryLinksByCategoryIdAsync(int categoryId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<DrugCategoryLink>
                (
                    "SELECT * FROM DrugCategoryLinks WHERE CategoryId = @CategoryId", 
                    new { CategoryId = categoryId }
                );
            }
        }
    }
}