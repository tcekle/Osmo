using Dapper;
using Npgsql;
using System.Data;

namespace Osmo.ConneX;

internal class PostgresDatabaseService
{
    private string _databaseName;
    
    public PostgresDatabaseService(string databaseName)
    {
        _databaseName = databaseName;
    }
    
    private IDbConnection CreateConnection() => new NpgsqlConnection($"Host=localhost;Port=5432;Database={_databaseName};Username=dataio;Password=dataio");

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<T>(sql, parameters);
    }

    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync(sql, parameters);
    }
}