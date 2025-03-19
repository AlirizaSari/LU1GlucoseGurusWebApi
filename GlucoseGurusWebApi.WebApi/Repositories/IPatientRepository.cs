using GlucoseGurusWebApi.WebApi.Models;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient> InsertAsync(Patient patient);
        Task<Patient?> ReadAsync(Guid id);
        Task<IEnumerable<Patient>> ReadAllAsync();
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(Guid id);
    }
}
