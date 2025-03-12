using System.Data;
using Dapper;
using MijnWebApi.WebApi.Classes.Interfaces;
using MijnWebApi.WebApi.Classes.Models;

public class Environment2DRepository : IEnvironment2DRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly ILogger<Environment2DRepository> _logger;

    public Environment2DRepository(IDbConnection dbConnection, ILogger<Environment2DRepository> logger)
    {
        _dbConnection = dbConnection;
        _logger = logger;
    }

    public async Task<IEnumerable<Environment2D>> GetAllEnvironment2DsAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Fetching all Environment2D records...");

            string sql = "SELECT id, Name, MaxHeight, MaxWidth, OwnerUserID FROM Environment2D";
            if (_dbConnection.State != ConnectionState.Open) _dbConnection.Open();
            var result = await _dbConnection.QueryAsync<Environment2D>(sql);

            _logger.LogInformation($"✅ Retrieved {result.AsList().Count} Environment2D records.");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ ERROR in GetAllEnvironment2DsAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<Environment2D>> GetWorldsByUserIdAsync(Guid userId)
    {
        var sql = "SELECT * FROM Environment2D WHERE OwnerUserID = @UserId";
        return await _dbConnection.QueryAsync<Environment2D>(sql, new { UserId = userId });
    }



    public async Task<Environment2D?> GetWorldByIdAsync(Guid id)
    {
        try
        {
            string sql = @"SELECT 
                  ID AS Id, 
                  Name, 
                  MaxHeight, 
                  MaxWidth, 
                  OwnerUserID AS OwnerUserID 
               FROM Environment2D WHERE Id = @Id";
            if (_dbConnection.State != ConnectionState.Open) _dbConnection.Open();
            var world = await _dbConnection.QueryFirstOrDefaultAsync<Environment2D>(sql, new { Id = id });

            if (world != null)
                _logger.LogInformation("✅ World retrieved successfully.");
            else
                _logger.LogWarning($"⚠️ No world found with ID: {id}");

            return world;
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ ERROR in GetWorldByIdAsync: {ex.Message}");
            throw;
        }
    }

    public async Task AddWorldAsync(Environment2D environment2D)
    {
        try
        {
            if (environment2D.Id == Guid.Empty)
                environment2D.Id = Guid.NewGuid();

            _logger.LogInformation($"📝 Inserting new world with ID: {environment2D.Id}");

            string sql = "INSERT INTO Environment2D (id, Name, MaxHeight, MaxWidth, OwnerUserID) VALUES (@Id, @Name, @MaxHeight, @MaxWidth, @OwnerUserID)";

            if (_dbConnection.State != ConnectionState.Open) _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                int rowsAffected = await _dbConnection.ExecuteAsync(sql, new
                {
                    Id = environment2D.Id,
                    Name = environment2D.Name,
                    MaxHeight = environment2D.MaxHeight,
                    MaxWidth = environment2D.MaxWidth,
                    OwnerUserID = environment2D.OwnerUserID
                }, transaction);

                if (rowsAffected > 0)
                {
                    _logger.LogInformation("✅ World inserted successfully.");
                    transaction.Commit();
                }
                else
                {
                    _logger.LogWarning("⚠️ INSERT executed, but no rows affected.");
                    transaction.Rollback();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ ERROR in AddWorldAsync: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateWorldAsync(Environment2D environment2D)
    {
        try
        {
            _logger.LogInformation($"🔄 Updating world with ID: {environment2D.Id}");

            string sql = @"UPDATE Environment2D 
                               SET Name = @Name, MaxHeight = @MaxHeight, MaxWidth = @MaxWidth, OwnerUserID = @OwnerUserID
                               WHERE id = @Id";

            if (_dbConnection.State != ConnectionState.Open) _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                int rowsAffected = await _dbConnection.ExecuteAsync(sql, new
                {
                    Id = environment2D.Id,
                    Name = environment2D.Name,
                    MaxHeight = environment2D.MaxHeight,
                    MaxWidth = environment2D.MaxWidth,
                    OwnerUserID = environment2D.OwnerUserID,
                }, transaction);

                if (rowsAffected > 0)
                {
                    _logger.LogInformation("✅ World updated successfully.");
                    transaction.Commit();
                }
                else
                {
                    _logger.LogWarning("⚠️ UPDATE executed, but no rows affected.");
                    transaction.Rollback();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ ERROR in UpdateWorldAsync: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteWorldAsync(Guid id)
    {
        try
        {
            _logger.LogInformation($"🗑 Deleting world with ID: {id}");

            string sql = "DELETE FROM Environment2D WHERE id = @Id";

            if (_dbConnection.State != ConnectionState.Open) _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                int rowsAffected = await _dbConnection.ExecuteAsync(sql, new { Id = id }, transaction);

                if (rowsAffected > 0)
                {
                    _logger.LogInformation("✅ World deleted successfully.");
                    transaction.Commit();
                }
                else
                {
                    _logger.LogWarning($"⚠️ DELETE executed, but no rows affected.");
                    transaction.Rollback();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ ERROR in DeleteWorldAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<Object2D>> GetObjectsForWorld(Guid worldId)
    {
        try
        {
            _logger.LogInformation($"🔍 Fetching objects for World ID: {worldId}");

            string sql = "SELECT * FROM Object2D WHERE Environment2DID = @WorldId";

            if (_dbConnection.State != ConnectionState.Open) _dbConnection.Open();
            var objects = await _dbConnection.QueryAsync<Object2D>(sql, new { WorldId = worldId });

            _logger.LogInformation($"✅ Retrieved {objects.AsList().Count} objects.");
            return objects;
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ ERROR in GetObjectsForWorld: {ex.Message}");
            throw;
        }
    }
}
