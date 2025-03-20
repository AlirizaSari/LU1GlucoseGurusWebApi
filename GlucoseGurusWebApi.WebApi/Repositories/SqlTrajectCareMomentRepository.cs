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

        public async Task<TrajectCareMoment> InsertAsync(TrajectCareMoment tracjetCareMoment)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var trajectCareMomentId = await sqlConnection.ExecuteAsync("INSERT INTO [Traject_CareMoment] (TrajectId, CareMomentId, Name, Step) VALUES (@TrajectId, @CareMomentId, @Name, @Step)", tracjetCareMoment);
                return tracjetCareMoment;
            }
        }

        public async Task<TrajectCareMoment?> ReadAsync(Guid trajectId, Guid careMomentId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<TrajectCareMoment>("SELECT * FROM [Traject_CareMoment] WHERE TrajectId = @TrajectId AND CareMomentId = @CareMomentId", new { TrajectId = trajectId, CareMomentId = careMomentId });
            }
        }

        public async Task<IEnumerable<TrajectCareMoment>> ReadAllAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<TrajectCareMoment>("SELECT * FROM [Traject_CareMoment]");
            }
        }

        public async Task UpdateAsync(TrajectCareMoment tracjetCareMoment)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [Traject_CareMoment] SET Name = @Name, Step = @Step WHERE TrajectId = @TrajectId AND CareMomentId = @CareMomentId", tracjetCareMoment);
            }
        }

        public async Task DeleteAsync(Guid trajectId, Guid careMomentId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [Traject_CareMoment] WHERE TrajectId = @TrajectId AND CareMomentId = @CareMomentId", new { TrajectId = trajectId, CareMomentId = careMomentId });
            }
        }
    }
}
