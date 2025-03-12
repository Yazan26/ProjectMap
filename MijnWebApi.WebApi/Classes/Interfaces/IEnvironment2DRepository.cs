using MijnWebApi.WebApi.Classes.Models;

namespace MijnWebApi.WebApi.Classes.Interfaces
{

    public interface IEnvironment2DRepository
    {
       
        Task<IEnumerable<Environment2D>> GetAllEnvironment2DsAsync();

       
        Task<Environment2D?> GetWorldByIdAsync(Guid id);

        

      
        Task AddWorldAsync(Environment2D environment2D);

      
        Task UpdateWorldAsync(Environment2D environment2D);

       
        Task DeleteWorldAsync(Guid id);

        Task<IEnumerable<Object2D>> GetObjectsForWorld(Guid WorldID);

        Task<IEnumerable<Environment2D>> GetWorldsByUserIdAsync(Guid userId);

    }
}