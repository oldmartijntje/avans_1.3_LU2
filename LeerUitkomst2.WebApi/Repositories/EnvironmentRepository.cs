using Dapper;
using LeerUitkomst2.WebApi.Models;
using Microsoft.Data.SqlClient;

namespace ProjectMap.WebApi.Repositories
{
    public class EnvironmentRepository : IDatabaseRepository<Environment2D>
    {
        private readonly string sqlConnectionString;

        public EnvironmentRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<int> GetAmountByUser(string userId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var amount = await sqlConnection.QuerySingleOrDefaultAsync<int>("SELECT COUNT(*) FROM [Environment2D] WHERE UserId = @userId", new { userId });
                return amount;
            }
        }

        public async Task<IEnumerable<Environment2D>> FindEnvironmentByName(string userId, string name)
        {
            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var environments = await sqlConnection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE UserId = @userId AND Name = @name", new { userId, name });
                return environments;
            }
        }

        public async Task<Environment2D> CreateEnvironmentByUser(Environment2DTemplate environment, string userId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var parameters = new
                {
                    environment.Name,
                    environment.MaxHeight,
                    environment.MaxLength,
                    UserId = userId
                };

                var query = @"
            INSERT INTO [Environment2D] (Name, MaxHeight, MaxLength, UserId)
            VALUES (@Name, @MaxHeight, @MaxLength, @UserId);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var environmentId = await sqlConnection.QuerySingleAsync<int>(query, parameters);

                return new Environment2D
                {
                    Id = environmentId,
                    Name = environment.Name,
                    MaxHeight = environment.MaxHeight,
                    MaxLength = environment.MaxLength,
                    UserId = userId
                };
            }
        }

        public async Task<Environment2D> GetSingleByUser(string userId, int requestedId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE Id = @requestedId", new { requestedId });
            }
        }

        public async Task<IEnumerable<Environment2D>> GetEnvironmentByUser(string userId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE UserId = @userId", new { userId });
            }
        }


        public async Task<Environment2D?> ReadAsync(int id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE Id = @Id", new { id });
            }
        }

        public async Task<IEnumerable<Environment2D>> ReadAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D]");
            }
        }

        public async Task UpdateAsync(Environment2D environment)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [Environment2D] SET " +
                                                 "Name = @Name, " +
                                                 "MaxHeight = @MaxHeight, " +
                                                 "MaxLength = @MaxLength " +
                                                 "WHERE Id = @Id"
                                                 , environment);

            }
        }

        public async Task<DataBoolean> DeleteAsync(int id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.OpenAsync();
                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        var query = @"
                    DELETE FROM [Object2D] WHERE EnvironmentId = @Id;
                    DELETE FROM [Environment2D] WHERE Id = @Id;
                ";
                        await sqlConnection.ExecuteAsync(query, new { id }, transaction);

                        transaction.Commit();
                        return new DataBoolean(true, "Success!");
                    }
                    catch (Exception ex)
                    {
                        // als 1 query faalt, wordt het gerollbacked.
                        transaction.Rollback();
                        return new DataBoolean(false, "No", ex.ToString());
                    }
                }
            }

        }
    }
}