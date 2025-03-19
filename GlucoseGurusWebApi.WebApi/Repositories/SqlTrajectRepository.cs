using Dapper;
using GlucoseGurusWebApi.WebApi.Models;
using Microsoft.Data.SqlClient;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public class SqlTrajectRepository : ITrajectRepository
    {
        private readonly string sqlConnectionString;

        public SqlTrajectRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<Traject> InsertAsync(Traject traject)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var trajectId = await sqlConnection.ExecuteAsync("INSERT INTO [Traject] (Id, Name) VALUES (@Id, @Name)", traject);
                return traject;
            }
        }

        public async Task<Traject?> ReadAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<Traject>("SELECT * FROM [Traject] WHERE Id = @Id", new { id });
            }
        }

        public async Task<IEnumerable<Traject>> ReadAllAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Traject>("SELECT * FROM [Traject]");
            }
        }

        public async Task UpdateAsync(Traject traject)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [Traject] SET Name = @Name WHERE Id = @Id", traject);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [Traject] WHERE Id = @Id", new { id });
            }
        }
    }
}
