using GlucoseGurusWebApi.WebApi.Models;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public interface ITrajectCareMomentRepository
    {
        Task<TracjetCareMoment> InsertAsync(TracjetCareMoment tracjetCareMoment);
        Task<TracjetCareMoment?> ReadAsync(Guid trajectId, Guid careMomentId);
        Task<IEnumerable<TracjetCareMoment>> ReadAllAsync();
        Task UpdateAsync(TracjetCareMoment tracjetCareMoment);
        Task DeleteAsync(Guid trajectId, Guid careMomentId);
    }
}
