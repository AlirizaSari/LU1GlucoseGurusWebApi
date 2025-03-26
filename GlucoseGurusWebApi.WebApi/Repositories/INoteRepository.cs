using GlucoseGurusWebApi.WebApi.Models;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public interface INoteRepository
    {
        Task<Note> InsertAsync(Note note);
        Task<Note?> ReadAsync(Guid id);
        Task<IEnumerable<Note>> ReadByPatientAsync(Guid patientId);
        Task<IEnumerable<Note>> ReadByParentGuardianAsync(Guid parentGuardianId);
        Task<IEnumerable<Note>> ReadAllAsync();
        Task UpdateAsync(Note note);
        Task DeleteAsync(Guid id);
    }
}
