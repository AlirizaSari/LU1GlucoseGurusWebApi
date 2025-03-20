using GlucoseGurusWebApi.WebApi.Models;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public interface ICareMomentRepository
    {
        Task<CareMoment> InsertAsync(CareMoment careMoment);
        Task<CareMoment?> ReadAsync(Guid careMomentId);
        Task<IEnumerable<CareMoment>> ReadAllAsync();
        Task UpdateAsync(CareMoment careMoment);
        Task DeleteAsync(Guid careMomentId);
    }
}
