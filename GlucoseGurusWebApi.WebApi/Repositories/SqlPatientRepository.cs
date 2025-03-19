using Dapper;
using GlucoseGurusWebApi.WebApi.Models;
using Microsoft.Data.SqlClient;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public class SqlPatientRepository : IPatientRepository
    {
        private readonly string sqlConnectionString;

        public SqlPatientRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<Patient> InsertAsync(Patient patient)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var patientId = await sqlConnection.ExecuteAsync("INSERT INTO [Patient] (Id, FirstName, LastName, Avatar, ParentGuardianId, TracjectId, DocterId) VALUES (@Id, @FirstName, @LastName, @Avatar, @ParentGuardianId, @TracjectId, @DocterId)", patient);
                return patient;
            }
        }

        public async Task<Patient?> ReadAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<Patient>("SELECT * FROM [Patient] WHERE Id = @Id", new { Id = id });
            }
        }

        public async Task<IEnumerable<Patient>> ReadAllAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Patient>("SELECT * FROM [Patient]");
            }
        }

        public async Task UpdateAsync(Patient patient)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [Patient] SET FirstName = @FirstName, LastName = @LastName, Avatar = @Avatar, ParentGuardianId = @ParentGuardianId, TracjectId = @TracjectId, DocterId = @DocterId WHERE Id = @Id", patient);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [Patient] WHERE Id = @Id", new { id });
            }
        }

    }
}
