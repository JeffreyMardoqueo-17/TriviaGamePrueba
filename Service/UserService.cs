using System.Threading.Tasks;
using TriviaPruebaTecnica.Models;
using TriviaPruebaTecnica.Repository.Interfaces;
using TriviaPruebaTecnica.Service;
using TriviaPruebaTecnica.Service.Interfaces;

namespace TriviaPruebaTecnica.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "El nombre de usuario y la contraseña no pueden estar vacíos.");

            var existingUser = await _userRepository.Login(username.Trim());
            if (existingUser != null)
            {
                return (false, "El nombre de usuario ya está registrado. Por favor, elige otro.");
            }
            string passwordHash = _passwordHasher.HashPassword(password);

            // llamao al repositorio para guardar en la BD 
            await _userRepository.RegisterAsync(username.Trim(), passwordHash);

            return (true, "Usuario registrado exitosamente.");
        }

        public async Task<(bool Success, string Message, User? User)> AuthenticateAsync(string username, string password)
        {
            // 1. Validaciones de negocio
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return (false, "Debe ingresar usuario y contraseña.", null);
            }

            // 2. Obtener el usuario desde la base de datos usando el SP sp_LoginUser
            var user = await _userRepository.Login(username.Trim());
            if (user == null)
            {
                return (false, "Credenciales inválidas.", null);
            }

            // 3. Verificar si la contraseña ingresada coincide con el Hash almacenado (BCrypt)
            bool isValidPassword = _passwordHasher.VerifyPassword(password, user.PasswordHash);
            if (!isValidPassword)
            {
                return (false, "Credenciales inválidas.", null);
            }

            return (true, "Autenticación exitosa.", user);
        }
    }
}