﻿using Dapper;
using LeerUitkomst2.WebApi.Models;
using Microsoft.Data.SqlClient;

namespace LeerUitkomst2.WebApi.Repositories
{
    public class Object2DRepository : IDatabaseRepository
    {
        private readonly string _sqlConnectionString;

        public Object2DRepository(string sqlConnectionString)
        {
            this._sqlConnectionString = sqlConnectionString;
        }

        public virtual async Task<IEnumerable<Object2D>> GetAllByEnvironmentId(int environmentId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_sqlConnectionString))
            {
                var object2D = await sqlConnection.QueryAsync<Object2D>("SELECT * FROM [Object2D] WHERE EnvironmentId = @environmentId", new { environmentId });
                return object2D;
            }
        }

        public virtual async Task<Environment2D?> GetSingleEnvironmentByUser(string userId, int requestedId)
        {
            using (var sqlConnection = new SqlConnection(_sqlConnectionString))
            {
                string query = @"
            SELECT e.* 
            FROM [Environment2D] e
            INNER JOIN [Object2D] o ON e.Id = o.EnvironmentId
            WHERE o.Id = @requestedId";

                return await sqlConnection.QuerySingleOrDefaultAsync<Environment2D>(query, new { requestedId });
            }
        }

        public virtual async Task<Object2D> CreateObject(Object2DTemplate objectDesign)
        {
            using (var sqlConnection = new SqlConnection(_sqlConnectionString))
            {
                var parameters = new
                {
                    PrefabId = objectDesign.PrefabId,
                    PositionX = objectDesign.PositionX,
                    PositionY = objectDesign.PositionY,
                    ScaleX = objectDesign.ScaleX,
                    ScaleY = objectDesign.ScaleY,
                    RotationZ = objectDesign.RotationZ,
                    SortingLayer = objectDesign.SortingLayer,
                    EnvironmentId = objectDesign.EnvironmentId,
                };

                var query = @"
            INSERT INTO [Object2D] (PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer, EnvironmentId)
            VALUES (@PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer, @EnvironmentId);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var environmentId = await sqlConnection.QuerySingleAsync<int>(query, parameters);

                return new Object2D
                {
                    Id = environmentId,
                    PrefabId = objectDesign.PrefabId,
                    PositionX = objectDesign.PositionX,
                    PositionY = objectDesign.PositionY,
                    ScaleX = objectDesign.ScaleX,
                    ScaleY = objectDesign.ScaleY,
                    RotationZ = objectDesign.RotationZ,
                    SortingLayer = objectDesign.SortingLayer,
                    EnvironmentId = objectDesign.EnvironmentId
                };
            }
        }

        public virtual async Task UpdateAsync(Object2D environment)
        {
            using (var sqlConnection = new SqlConnection(_sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [Object2D] SET " +
                                                 "PrefabId = @PrefabId, " +
                                                 "PositionX = @PositionX, " +
                                                 "PositionY = @PositionY, " +
                                                 "ScaleX = @ScaleX, " +
                                                 "ScaleY = @ScaleY, " +
                                                 "RotationZ = @RotationZ, " +
                                                 "SortingLayer = @SortingLayer " +
                                                 "WHERE Id = @Id"
                                                 , environment);

            }
        }

        public virtual async Task<Object2D?> ReadAsync(int id)
        {
            using (var sqlConnection = new SqlConnection(_sqlConnectionString))
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<Object2D>("SELECT * FROM [Object2D] WHERE Id = @Id", new { id });
            }
        }

        public virtual async Task DeleteAsync(int id)
        {
            using (var sqlConnection = new SqlConnection(_sqlConnectionString))
            {
                var query = "DELETE FROM [Object2D] WHERE Id = @Id;";
                await sqlConnection.ExecuteAsync(query, new { id });
            }
        }
    }
}