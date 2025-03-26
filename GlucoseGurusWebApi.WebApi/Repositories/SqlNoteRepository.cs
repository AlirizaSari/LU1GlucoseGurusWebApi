using Dapper;
using GlucoseGurusWebApi.WebApi.Models;
using Microsoft.Data.SqlClient;

namespace GlucoseGurusWebApi.WebApi.Repositories
{
    public class SqlNoteRepository : INoteRepository
    {
        private readonly string sqlConnectionString;

        public SqlNoteRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<Note> InsertAsync(Note note)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var noteId = await sqlConnection.ExecuteAsync("INSERT INTO [Note] (Id, Date, Text, UserMood, ParentGuardianId, PatientId ) VALUES (@Id, @Date, @Text, @UserMood, @ParentGuardianId, @PatientId)", note);
                return note;
            }
        }

        public async Task<Note?> ReadAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<Note>("SELECT * FROM [Note] WHERE Id = @Id", new { id });
            }
        }

        public async Task<IEnumerable<Note>> ReadAllAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Note>("SELECT * FROM [Note]");
            }
        }

        public async Task<IEnumerable<Note>> ReadByPatientAsync(Guid patientId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Note>("SELECT * FROM [Note] WHERE PatientId = @PatientId", new { patientId });
            }
        }

        public async Task<IEnumerable<Note>> ReadByParentGuardianAsync(Guid parentGuardianId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Note>("SELECT * FROM [Note] WHERE ParentGuardianId = @ParentGuardianId", new { parentGuardianId });
            }
        }

        public async Task UpdateAsync(Note note)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [Note] SET Name = @Name, Date = @Date, Text = @Text, UserMood = @UserMood, ParentGuardianId = @ParentGuardianId, PatientId = @PatientId WHERE Id = @Id", note);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [Note] WHERE Id = @Id", new { id });
            }
        }

    }
}
