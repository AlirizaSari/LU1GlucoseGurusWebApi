using Dapper;
using GlucoseGurusWebApi.WebApi.Models;
using Microsoft.Data.SqlClient;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public class SqlParentGuardianRepository : IParentGuardianRepository
    {
        private readonly string sqlConnectionString;

        public SqlParentGuardianRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<ParentGuardian> InsertAsync(ParentGuardian parentGuardian)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var parentGuardianId = await sqlConnection.ExecuteAsync("INSERT INTO [ParentGuardian] (Id, FirstName, LastName, UserId) VALUES (@Id, @FirstName, @LastName, @UserId)", parentGuardian);
                return parentGuardian;
            }
        }

        public async Task<ParentGuardian?> ReadAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<ParentGuardian>("SELECT * FROM [ParentGuardian] WHERE Id = @Id", new { id });
            }
        }

        public async Task<IEnumerable<ParentGuardian>> ReadAllAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<ParentGuardian>("SELECT * FROM [ParentGuardian]");
            }
        }

        public async Task<IEnumerable<ParentGuardian>> ReadAllByUserIdAsync(string userId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<ParentGuardian>("SELECT * FROM [ParentGuardian] WHERE UserId = @UserId", new { UserId = userId });
            }
        }

        public async Task UpdateAsync(ParentGuardian parentGuardian)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [ParentGuardian] SET FirstName = @FirstName, LastName = @LastName, UserId = @UserId WHERE Id = @Id", parentGuardian);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [ParentGuardian] WHERE Id = @Id", new { id });
            }
        }
    }
}
