
using MijnWebApi.WebApi.Classes.Models;

namespace MijnWebApi.WebApi.Classes.Interfaces
{
    public interface IObject2DRepository
    {
        public Task<Object2D> PostObjectAsync(Object2D Object2D);
        public Task<IEnumerable<Object2D>> GetObjectAsync(Guid id);
        public Task<IEnumerable<Object2D>> GetObjectAsync();
        public Task UpdateObjectAsync(Object2D object2D);
        public Task DeleteObjectAsync(Guid id);
    }
}

