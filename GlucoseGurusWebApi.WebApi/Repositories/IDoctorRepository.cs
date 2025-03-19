using GlucoseGurusWebApi.WebApi.Models;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public interface IDoctorRepository
    {
        Task<Doctor> InsertAsync(Doctor docter);
        Task<Doctor?> ReadAsync(Guid id);
        Task<IEnumerable<Doctor>> ReadAllAsync();
        Task UpdateAsync(Doctor docter);
        Task DeleteAsync(Guid id);
    }
}
