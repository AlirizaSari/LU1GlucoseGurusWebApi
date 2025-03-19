using Dapper;
using GlucoseGurusWebApi.WebApi.Models;
using Microsoft.Data.SqlClient;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public class SqlDoctorRepository : IDoctorRepository
    {
        private readonly string sqlConnectionString;

        public SqlDoctorRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<Doctor> InsertAsync(Doctor doctor)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var docterId = await sqlConnection.ExecuteAsync("INSERT INTO [Doctor] (Id, Name, Specialization) VALUES (@Id, @Name, @Specialization)", docter);
                return doctor;
            }
        }

        public async Task<Doctor?> ReadAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<Doctor>("SELECT * FROM [Doctor] WHERE Id = @Id", new { id });
            }
        }

        public async Task<IEnumerable<Doctor>> ReadAllAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Doctor>("SELECT * FROM [Doctor]");
            }
        }

        public async Task UpdateAsync(Doctor docter)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [Doctor] SET Name = @Name, Specialization = @Specialization WHERE Id = @Id", docter);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [Doctor] WHERE Id = @Id", new { id });
            }
        }
    }
}
