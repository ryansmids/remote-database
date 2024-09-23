/*using Serilog;
using Npgsql;

namespace DatabaseCheck;

public class DatabaseTool
{
    private string _connectionStringZonderDatabase;
    private string _ConnectionString;
    private string _DatabaseNaam;

    public DatabaseTool(string host, string gebruiker, string wachtwoord, string databaseNaam)
    {
        this._connectionStringZonderDatabase = $"Host={host};Username={gebruiker};Password={wachtwoord};";
        this._ConnectionString = $"Host={host};Username={gebruiker};Password={wachtwoord};Database={databaseNaam}";
        this._DatabaseNaam = databaseNaam;
    }

    
    public bool DatabaseBestaat()
    {
        using var verbinding = new NpgsqlConnection(this._connectionStringZonderDatabase);
        try
        {
            verbinding.Open();
            string controleerDatabaseQuery = $"SELECT 1 FROM pg_database WHERE datname = '{_DatabaseNaam}'";

            using var opdracht = new NpgsqlCommand(controleerDatabaseQuery, verbinding);
            var resultaat = opdracht.ExecuteScalar();
            return resultaat != null;
        }
        catch (Exception ex)
        {
            Log.Information($"Er is een fout opgetreden bij het controleren van de database: {ex.Message}");
            return false;
        }
    }

    
    public void MaakDatabase()
    {
        using var verbinding = new NpgsqlConnection(this._connectionStringZonderDatabase);
        try
        {
            verbinding.Open();
            string maakDatabaseQuery = $"CREATE DATABASE \"{_DatabaseNaam}\"";
            using var opdracht = new NpgsqlCommand(maakDatabaseQuery, verbinding);
            opdracht.ExecuteNonQuery();
            Log.Information($"Database '{_DatabaseNaam}' succesvol aangemaakt.");
            
            
            this._ConnectionString = $"{_connectionStringZonderDatabase}Database={_DatabaseNaam};";

        }
        catch (Exception ex)
        {
            Log.Information($"Er is een fout opgetreden tijdens het aanmaken van de database: {ex.Message}");
        }
    }

    public bool TabelBestaat(string tabelNaam)
    {
        using var verbinding = new NpgsqlConnection(this._ConnectionString);
        {
            try
            {
                verbinding.Open();
                string controleerTabelQuery = $"SELECT EXISTS (SELECT 1 FROM pg_tables WHERE LOWER(tablename) = LOWER('{tabelNaam}') AND schemaname = 'public')";

                using var opdracht = new NpgsqlCommand(controleerTabelQuery, verbinding);
                bool bestaat = (opdracht.ExecuteScalar() as bool?) ?? false;
                return bestaat;
                

            }
            catch (Exception ex)
            {
                Log.Information($"Er is een fout opgetreden bij het controleren van de tabel '{tabelNaam}': {ex.Message}");
                return false;
            }
        }
    }

    public void MaakTabel(string tabelNaam, string tabelDefinitie)
    {
        using var verbinding = new NpgsqlConnection(this._ConnectionString);
        try
        {
            verbinding.Open();
            string maakTabelQuery = $"CREATE TABLE {tabelNaam} ({tabelDefinitie})";
            using var opdracht = new NpgsqlCommand(maakTabelQuery, verbinding);
            opdracht.ExecuteNonQuery();
            Log.Information($"Tabel '{tabelNaam}' succesvol aangemaakt.");
        }
        catch (Exception ex)
        {
            Log.Information($"Er is een fout opgetreden bij het aanmaken van de tabel '{tabelNaam}': {ex.Message}");
        }
    }
    
}*/
