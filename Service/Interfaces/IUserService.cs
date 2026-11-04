using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriviaPruebaTecnica.Models;

namespace TriviaPruebaTecnica.Service.Interfaces
{

    public interface IUserService
    {
        Task<(bool Success, string Message)> RegisterAsync(string username, string password);
        Task<(bool Success, string Message, User? User)> AuthenticateAsync(string username, string password);
    }
}