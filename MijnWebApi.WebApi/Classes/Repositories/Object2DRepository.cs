using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MijnWebApi.WebApi.Classes.Interfaces;
using MijnWebApi.WebApi.Classes.Models;


namespace MijnWebApi.WebApi.Classes.Repository
{
    public class Object2DRepository : IObject2DRepository
    {
        private readonly string _connectionString;
        //private readonly ILogger<Environment2DRepository> _logger;

        public Object2DRepository(string sqlConnectionString)//, ILogger<Environment2DRepository> logger)
        {
            _connectionString = sqlConnectionString;
          //  _logger = logger;
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<Object2D> PostObjectAsync(Object2D object2d)
        {
            using (var connection = CreateConnection())
            {
                var sql = await connection.ExecuteAsync("INSERT INTO [Object2D] (Id, EnvironmentId, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer) VALUES (@Id, @EnvironmentId, @PrefabId, @PositionX, @PositionY. @ScaleX, @ScaleY, @RotationZ, @SortingLayer)", object2d);
                return object2d;
            }
        }

        public async Task<IEnumerable<Object2D>> GetObjectAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Object2D>("SELECT * FROM [Object2D] WHERE EnvironmentId = @Id", new { id });
            }
        }

        public async Task<IEnumerable<Object2D>> GetAllObjectsAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Object2D>("SELECT * FROM [Object2D]");
            }
        }

        public async Task UpdateObjectAsync(Object2D object2D)
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync("UPDATE [Object2D] SET PositionX = @PositionX, PositionY = @PositionY WHERE Id = @Id", object2D);
            }
        }

        public async Task DeleteObjectAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync("UDELETE FROM [Object2D] WHERE Id = @Id", new { id });
            }
        }

    }
}