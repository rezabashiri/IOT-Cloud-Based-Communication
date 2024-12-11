namespace Shared.Core.Settings;

public class PersistenceSettings
{
    public bool UseMsSql { get; set; }

    public bool UsePostgres { get; set; }

    public bool UseInMemory { get; set; }

    public bool UseSqlite { get; set; }

    public PersistenceConnectionStrings ConnectionStrings { get; set; }

    public class PersistenceConnectionStrings
    {
        public string MSSQL { get; set; }

        public string Postgres { get; set; }

        public string Sqlite { get; set; }
    }
}