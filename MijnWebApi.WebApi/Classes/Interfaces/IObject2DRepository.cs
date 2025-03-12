
using MijnWebApi.WebApi.Classes.Models;

namespace MijnWebApi.WebApi.Classes.Interfaces
{
    public interface IObject2DRepository
    {
        Task<IEnumerable<Object2D>> GetAllObject2DsAsync();
        Task<Object2D?> GetObject2DByIdAsync(Guid id);
        Task AddObject2DAsync(Object2D Object2D);
        Task UpdateObject2DAsync(Object2D Object2D);
        Task DeleteObject2DAsync(Guid id);

        Task<IEnumerable<Object2D>> GetObjectsForUserWorld(Guid userId, Guid worldId);

    }
}
