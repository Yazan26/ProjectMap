using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Data.SqlClient;
using MijnWebApi.WebApi.Classes.Interfaces;
using MijnWebApi.WebApi.Classes.Models;

public class Environment2DRepository : IEnvironment2DRepository
{
    private readonly string _connectionString;
    private readonly ILogger<Environment2DRepository> _logger;

    public Environment2DRepository(string sqlConnectionString, ILogger<Environment2DRepository> logger)
    {
        _connectionString = sqlConnectionString;
        _logger = logger;
    }

    private SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public async Task<Environment2D> PostWorldAsync(Environment2D environment2D)
    {
        using (var connection = CreateConnection())
        {
            var sql = await connection.ExecuteAsync("INSERT INTO [Environment2D] (Id, Name, MaxHeight, MaxWidth, OwnerUserID) VALUES (@Id, @Name, @MaxHeight, @MaxWidth, @OwnerUserID)", environment2D);
            return environment2D;
        }
    }

    public async Task<Environment2D> GetWorldAsync(Guid Id)
    {
        using (var connection = CreateConnection())
        {
            return await connection.QuerySingleOrDefaultAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE Id = @Id", new { Id });
        }
    }

    public async Task<IEnumerable<Environment2D>> GetWorldsForUserAsync(Guid id)
    {
        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE OwnerUserID = @id", new { id });
        }
    }

    public async Task<IEnumerable<Environment2D>> GetworldsAsync()
    {
        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D]");
        }
    }

    public async Task UpdateWorldAsync(Environment2D environment)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync("UPDATE [Environment2D] SET Name = @Name, MaxHeight = @MaxHeight, MaxWidth = @MaxWidth WHERE Id = @Id", environment);
        }
    }

    public async Task DeleteWorldAsync(Guid id)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync("DELETE FROM [Environment2D] WHERE Id = @Id", new { Id = id });
            await connection.ExecuteAsync("DELETE FROM [Object2D] WHERE Environment2DID = @Id", new { Id = id });
        }
    }
}