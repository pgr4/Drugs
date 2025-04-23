namespace Drugs.Library.Repository
{
    public abstract class BaseSqliteRepository
    {
        protected readonly string ConnectionString;

        public BaseSqliteRepository(string connectionString)
        {
            ConnectionString = connectionString;

            InitializeDatabase();
        }

        public abstract void InitializeDatabase();
    }
}