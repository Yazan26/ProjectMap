using MijnWebApi.WebApi.Classes;

namespace MijnWebApi.WebApi.Classes.Interfaces
{

    public interface IEnvironment2DRepository
    {
        /// <summary>
        /// Retrieve all Environment2D objects.
        /// </summary>
        Task<IEnumerable<Environment2D>> GetAllEnvironment2DsAsync();

        /// <summary>
        /// Retrieve a specific Environment2D by ID.
        /// </summary>
        Task<Environment2D?> GetWorldByIdAsync(Guid id);

        /// <summary>
        /// Retrieve all Environment2D objects belonging to a specific user.
        /// </summary>

        /// <summary>
        /// Add a new Environment2D.
        /// </summary>
        Task AddWorldAsync(Environment2D environment2D);

        /// <summary>
        /// Update an existing Environment2D.
        /// </summary>
        Task UpdateWorldAsync(Environment2D environment2D);

        /// <summary>
        /// Delete an Environment2D by ID.
        /// </summary>
        Task DeleteWorldAsync(Guid id);

        Task<IEnumerable<Object2D>> GetObjectsForWorld(Guid WorldID);

    }
}