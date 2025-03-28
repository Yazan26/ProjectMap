using MijnWebApi.WebApi.Classes.Models;

namespace MijnWebApi.WebApi.Classes.Interfaces
{

    public interface IEnvironment2DRepository
    {

        public Task<Environment2D> PostWorldAsync(Environment2D Environment2D);
        public Task<Environment2D?> GetWorldAsync(Guid id);
        public Task<IEnumerable<Environment2D>> GetWorldsForUserAsync(Guid id);
        public Task<IEnumerable<Environment2D>> GetworldsAsync();
        public Task UpdateWorldAsync(Environment2D environment);
        public Task DeleteWorldAsync(Guid id);

    }
}