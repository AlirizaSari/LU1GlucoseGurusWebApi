using GlucoseGurusWebApi.WebApi.Models;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public interface ITrajectCareMomentRepository
    {
        Task<TrajectCareMoment> InsertAsync(TrajectCareMoment tracjetCareMoment);
        Task<TrajectCareMoment?> ReadAsync(Guid trajectId, Guid careMomentId);
        Task<IEnumerable<TrajectCareMoment>> ReadAllAsync();
        Task UpdateAsync(TrajectCareMoment tracjetCareMoment);
        Task DeleteAsync(Guid trajectId, Guid careMomentId);
    }
}
