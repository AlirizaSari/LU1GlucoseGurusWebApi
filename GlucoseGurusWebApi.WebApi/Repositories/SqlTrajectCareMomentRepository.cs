using Dapper;
using GlucoseGurusWebApi.WebApi.Models;
using Microsoft.Data.SqlClient;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public class SqlTrajectCareMomentRepository
    {
        private readonly string sqlConnectionString;

        public SqlTrajectCareMomentRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<TracjetCareMoment> InsertAsync(TracjetCareMoment tracjetCareMoment)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var tracjetCareMomentId = await sqlConnection.ExecuteAsync("INSERT INTO [TracjetCareMoment] (TrajectId, CareMomentId, Name, Step) VALUES (@TrajectId, @CareMomentId, @Name, @Step)", tracjetCareMoment);
                return tracjetCareMoment;
            }
        }

        public async Task<TracjetCareMoment?> ReadAsync(Guid trajectId, Guid careMomentId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<TracjetCareMoment>("SELECT * FROM [TracjetCareMoment] WHERE TrajectId = @TrajectId AND CareMomentId = @CareMomentId", new { TrajectId = trajectId, CareMomentId = careMomentId });
            }
        }

        public async Task<IEnumerable<TracjetCareMoment>> ReadByTrajectAsync(Guid trajectId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<TracjetCareMoment>("SELECT * FROM [TracjetCareMoment] WHERE TrajectId = @TrajectId", new { TrajectId = trajectId });
            }
        }

        public async Task<IEnumerable<TracjetCareMoment>> ReadAllAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<TracjetCareMoment>("SELECT * FROM [TracjetCareMoment]");
            }
        }

        public async Task UpdateAsync(TracjetCareMoment tracjetCareMoment)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [TracjetCareMoment] SET Name = @Name, Step = @Step WHERE TrajectId = @TrajectId AND CareMomentId = @CareMomentId", tracjetCareMoment);
            }
        }

        public async Task DeleteAsync(Guid trajectId, Guid careMomentId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [TracjetCareMoment] WHERE TrajectId = @TrajectId AND CareMomentId = @CareMomentId", new { TrajectId = trajectId, CareMomentId = careMomentId });
            }
        }
    }
}
