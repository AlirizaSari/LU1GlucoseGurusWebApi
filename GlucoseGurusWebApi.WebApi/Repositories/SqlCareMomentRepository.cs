using Dapper;
using GlucoseGurusWebApi.WebApi.Models;
using Microsoft.Data.SqlClient;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public class SqlCareMomentRepository : ICareMomentRepository
    {
        private readonly string sqlConnectionString;

        public SqlCareMomentRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<CareMoment> InsertAsync(CareMoment careMoment)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var careMomentId = await sqlConnection.ExecuteAsync("INSERT INTO [CareMoment] (Id, Name, Url, Picture, TimeDurationInMin) VALUES (@Id, @Name, @Url, @Picture, @TimeDurationInMin)", careMoment);
                return careMoment;
            }
        }

        public async Task<CareMoment?> ReadAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<CareMoment>("SELECT * FROM [CareMoment] WHERE Id = @Id", new { id });
            }
        }

        public async Task<IEnumerable<CareMoment>> ReadAllAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<CareMoment>("SELECT * FROM [CareMoment]");
            }
        }

        public async Task UpdateAsync(CareMoment careMoment)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [CareMoment] SET Name = @Name, Url = @Url, Picture = @Picture, TimeDurationInMin = @TimeDurationInMin WHERE Id = @Id", careMoment);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [CareMoment] WHERE Id = @Id", new { id });
            }
        }

    }
}
