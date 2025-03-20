using Dapper;
using GlucoseGurusWebApi.WebApi.Models;
using Microsoft.Data.SqlClient;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public class SqlTrajectCareMomentRepository : ITrajectCareMomentRepository
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
                var trajectCareMomentId = await sqlConnection.ExecuteAsync("INSERT INTO [TrajectCareMoment] (TrajectId, CareMomentId, Name, Step) VALUES (@TrajectId, @CareMomentId, @Name, @Step)", tracjetCareMoment);
                return tracjetCareMoment;
            }
        }

        public async Task<TracjetCareMoment?> ReadAsync(Guid trajectId, Guid careMomentId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<TracjetCareMoment>("SELECT * FROM [TrajectCareMoment] WHERE TrajectId = @TrajectId AND CareMomentId = @CareMomentId", new { TrajectId = trajectId, CareMomentId = careMomentId });
            }
        }

        public async Task<IEnumerable<TracjetCareMoment>> ReadAllAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<TracjetCareMoment>("SELECT * FROM [TrajectCareMoment]");
            }
        }

        public async Task UpdateAsync(TracjetCareMoment tracjetCareMoment)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [TrajectCareMoment] SET Name = @Name, Step = @Step WHERE TrajectId = @TrajectId AND CareMomentId = @CareMomentId", tracjetCareMoment);
            }
        }

        public async Task DeleteAsync(Guid trajectId, Guid careMomentId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [TrajectCareMoment] WHERE TrajectId = @TrajectId AND CareMomentId = @CareMomentId", new { TrajectId = trajectId, CareMomentId = careMomentId });
            }
        }
    }
}
