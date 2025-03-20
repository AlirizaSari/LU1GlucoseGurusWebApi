using GlucoseGurusWebApi.WebApi.Models;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public interface INoteRepository
    {
        Task<Note> InsertAsync(Note note);
        Task<Note?> ReadAsync(Guid id);
        Task<IEnumerable<Note>> ReadAllAsync();
        Task UpdateAsync(Note note);
        Task DeleteAsync(Guid id);
    }
}
