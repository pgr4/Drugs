using System.Data.SQLite;
using Dapper;
using Drugs.Library.Models;

namespace Drugs.Library.Repository
{
    public class DrugSideEffectLinkSqliteRepository() : BaseSqliteRepository("Data Source=DrugSideEffectLinks.db;Version=3;"), IRepository<DrugSideEffectLink>
    {
        public override void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var sql = @"
                    CREATE TABLE IF NOT EXISTS DrugSideEffectLinks (
                        DrugSideEffectLinkId INTEGER PRIMARY KEY AUTOINCREMENT,
                        DrugId INTEGER NOT NULL,
                        SideEffectId INTEGER NOT NULL,
                        FOREIGN KEY (DrugId) REFERENCES Drugs(DrugId),
                        FOREIGN KEY (SideEffectId) REFERENCES SideEffects(SideEffectId)
                    );";

                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task AddAsync(DrugSideEffectLink drugSideEffectLink)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = "INSERT INTO DrugSideEffectLinks (DrugId, SideEffectId) VALUES (@DrugId, @SideEffectId)";
                await connection.ExecuteAsync(sql, drugSideEffectLink);
            }
        }

        public async Task<DrugSideEffectLink?> GetSideEffectByIdAsync(int drugSideEffectLink)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<DrugSideEffectLink?>
                (
                    "SELECT * FROM DrugSideEffectLinks WHERE DrugSideEffectLinkId = @DrugSideEffectLinkId", 
                    new { DrugSideEffectLinkId = drugSideEffectLink }
                );
            }
        }

        public async Task<IEnumerable<DrugSideEffectLink>> GetDrugSideEffectLinksByDrugIdAsync(int drugId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<DrugSideEffectLink>
                (
                    "SELECT * FROM DrugSideEffectLinks WHERE DrugId = @DrugId",
                    new { DrugId = drugId } 
                );
            }
        }

        public async Task<IEnumerable<DrugSideEffectLink>> GetDrugSideEffectLinksBySideEffectIdAsync(int sideEffectId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<DrugSideEffectLink>
                (
                    "SELECT * FROM DrugSideEffectLinks WHERE SideEffectId = @SideEffectId", 
                    new { SideEffectId = sideEffectId }
                );
            }
        }
    }
}