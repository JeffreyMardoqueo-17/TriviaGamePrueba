using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using TriviaPruebaTecnica.Models;
using TriviaPruebaTecnica.Repository.Interfaces;
using System.Data;

namespace TriviaPruebaTecnica.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> RegisterAsync(string username, string passwordHash)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Username", username);
            parameters.Add("@PasswordHash", passwordHash);

            // Ejecuta sp_RegisterUser y retorna el ID generado (SCOPE_IDENTITY)
            var userId = await _dbConnection.ExecuteScalarAsync<int>(
                "sp_RegisterUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return userId;
        }

        public async Task<User?> Login(string username)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Username", username);

            // Ejecuta sp_LoginUser que valida y retorna el registro del usuario por su Username
            var user = await _dbConnection.QuerySingleOrDefaultAsync<User>(
                "sp_LoginUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return user;
        }

        //validacion si existe 
        public async Task<bool> ExistsAsync(string username)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Username", username);

            var exists = await _dbConnection.ExecuteScalarAsync<bool>(
                "sp_ExistUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return exists;
        }
        public async Task<int> GetUserTotalPointsAsync(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var result = await _dbConnection.ExecuteScalarAsync<int?>(
                "sp_GetUserTotalPoints",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? 0;
        }
    }
}