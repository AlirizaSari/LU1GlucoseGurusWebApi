using GlucoseGurusWebApi.WebApi.Models;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public interface IParentGuardianRepository
    {
        Task<ParentGuardian> InsertAsync(ParentGuardian parentGuardian);
        Task<ParentGuardian?> ReadAsync(Guid id);
        Task<IEnumerable<ParentGuardian>> ReadAllAsync();
        Task<IEnumerable<ParentGuardian>> ReadAllByUserIdAsync(string userId);
        Task UpdateAsync(ParentGuardian parentGuardian);
        Task DeleteAsync(Guid id);
    }
}
