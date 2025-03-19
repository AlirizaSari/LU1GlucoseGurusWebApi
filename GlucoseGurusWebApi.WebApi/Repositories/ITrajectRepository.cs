using GlucoseGurusWebApi.WebApi.Models;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public interface ITrajectRepository
    {
        Task<Traject> InsertAsync(Traject traject);
        Task<Traject?> ReadAsync(Guid id);
        Task<IEnumerable<Traject>> ReadAllAsync();
        Task UpdateAsync(Traject traject);
        Task DeleteAsync(Guid id);
    }
}
