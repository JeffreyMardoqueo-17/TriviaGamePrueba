using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriviaPruebaTecnica.Models;

namespace TriviaPruebaTecnica.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<int> RegisterAsync(string username, string passwordHash);
        Task<User?> Login(string username);
        Task<bool> ExistsAsync(string username);
        Task<int> GetUserTotalPointsAsync(int userId);
    }
}