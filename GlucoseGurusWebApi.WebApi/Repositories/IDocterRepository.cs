using GlucoseGurusWebApi.WebApi.Models;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public interface IDocterRepository
    {
        Task<Docter> InsertAsync(Docter docter);
        Task<Docter?> ReadAsync(Guid id);
        Task<IEnumerable<Docter>> ReadAllAsync();
        Task UpdateAsync(Docter docter);
        Task DeleteAsync(Guid id);
    }
}
