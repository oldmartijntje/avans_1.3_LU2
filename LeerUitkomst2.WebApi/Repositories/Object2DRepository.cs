using Dapper;
using LeerUitkomst2.WebApi.Models;
using Microsoft.Data.SqlClient;

namespace ProjectMap.WebApi.Repositories
{
    public class Object2DRepository : IDatabaseRepository<Object2D>
    {
        private readonly string sqlConnectionString;

        public Object2DRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<IEnumerable<Object2D>> GetAllByEnvironmentId(int environmentId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var object2D = await sqlConnection.QueryAsync<Object2D>("SELECT * FROM [Object2D] WHERE EnvironmentId = @environmentId", new { environmentId });
                return object2D;
            }
        }

        Task<Object2D> IDatabaseRepository<Object2D>.GetSingleByUser(string userId, int requestedId)
        {
            throw new NotImplementedException();
        }
    }
}