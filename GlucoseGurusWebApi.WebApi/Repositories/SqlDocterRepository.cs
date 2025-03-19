using Dapper;
using GlucoseGurusWebApi.WebApi.Models;
using Microsoft.Data.SqlClient;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public class SqlDocterRepository : IDocterRepository
    {
        private readonly string sqlConnectionString;

        public SqlDocterRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<Docter> InsertAsync(Docter docter)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var docterId = await sqlConnection.ExecuteAsync("INSERT INTO [Docter] (Id, Name, Specialization) VALUES (@Id, @Name, @Specialization)", docter);
                return docter;
            }
        }

        public async Task<Docter?> ReadAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<Docter>("SELECT * FROM [Docter] WHERE Id = @Id", new { id });
            }
        }

        public async Task<IEnumerable<Docter>> ReadAllAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Docter>("SELECT * FROM [Docter]");
            }
        }

        public async Task UpdateAsync(Docter docter)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [Docter] SET Name = @Name, Specialization = @Specialization WHERE Id = @Id", docter);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [Docter] WHERE Id = @Id", new { id });
            }
        }
    }
}
