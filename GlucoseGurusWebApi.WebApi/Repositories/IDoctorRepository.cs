using GlucoseGurusWebApi.WebApi.Models;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public interface IDoctorRepository
    {
        Task<Doctor> InsertAsync(Doctor doctor);
        Task<Doctor?> ReadAsync(Guid id);
        Task<IEnumerable<Doctor>> ReadAllAsync();
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(Guid id);
    }
}
