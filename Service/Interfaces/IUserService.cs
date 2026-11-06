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
        
        // Debe coincidir exactamente con los 4 elementos que devuelve UserService
        Task<(bool Success, string Message, User? User, int TotalPoints)> AuthenticateAsync(string username, string password);
    }
}