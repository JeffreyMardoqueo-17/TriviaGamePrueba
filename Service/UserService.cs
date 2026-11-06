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
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "El nombre de usuario y la contraseña no pueden estar vacíos.");

            var cleanUsername = username.Trim();

            if (cleanUsername.Length < 3 || cleanUsername.Length > 50)
                return (false, "El nombre de usuario debe tener entre 3 y 50 caracteres.");

            if (password.Length < 6)
                return (false, "La contraseña debe tener al menos 6 caracteres.");

            try
            {
                bool userExists = await _userRepository.ExistsAsync(cleanUsername);
                if (userExists)
                {
                    _logger.LogWarning("Intento de registro fallido: El usuario '{Username}' ya existe.", cleanUsername);
                    return (false, "El nombre de usuario ya está registrado. Por favor, elige otro.");
                }

                string passwordHash = _passwordHasher.HashPassword(password);
                await _userRepository.RegisterAsync(cleanUsername, passwordHash);

                _logger.LogInformation("Usuario '{Username}' registrado exitosamente.", cleanUsername);
                return (true, "Usuario registrado exitosamente.");
            }
            catch (Exception ex)
            {
                // Capturamos si la base de datos arroja un error de llave duplicada (SQL Server Error 2627 o 2601)
                if (ex.Message.Contains("Violation of PRIMARY KEY") ||
                    ex.Message.Contains("duplicate key") ||
                    ex.Message.Contains("IX_") ||
                    (ex is Microsoft.Data.SqlClient.SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601)))
                {
                    _logger.LogWarning("Intento de registro fallido por restricción de BD: El usuario '{Username}' ya existe.", cleanUsername);
                    return (false, "El nombre de usuario ya está registrado. Por favor, elige otro.");
                }

                // Si es cualquier otro error real de servidor o conexión
                _logger.LogError(ex, "Error crítico al registrar al usuario '{Username}'.", cleanUsername);
                return (false, "Ocurrió un error interno al procesar el registro. Inténtalo más tarde.");
            }
        }

        // Actualizamos la tupla para retornar también los puntos totales (TotalPoints)
        public async Task<(bool Success, string Message, User? User, int TotalPoints)> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return (false, "Debe ingresar usuario y contraseña.", null, 0);
            }

            var cleanUsername = username.Trim();

            try
            {
                var user = await _userRepository.Login(cleanUsername);

                if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                {
                    _logger.LogWarning("Intento de autenticación fallido: Usuario no encontrado '{Username}'.", cleanUsername);
                    return (false, "Credenciales inválidas.", null, 0);
                }

                bool isValidPassword = _passwordHasher.VerifyPassword(password, user.PasswordHash);

                if (!isValidPassword)
                {
                    _logger.LogWarning("Intento de autenticación fallido: Contraseña incorrecta para el usuario '{Username}'.", cleanUsername);
                    return (false, "Credenciales inválidas.", null, 0);
                }

                // Consultamos los puntos acumulados del usuario
                int totalPoints = await _userRepository.GetUserTotalPointsAsync(user.Id);

                _logger.LogInformation("Usuario '{Username}' autenticado exitosamente con {Points} puntos.", cleanUsername, totalPoints);
                return (true, "Autenticación exitosa.", user, totalPoints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico durante la autenticación del usuario '{Username}'.", cleanUsername);
                return (false, "Ocurrió un error interno al procesar la autenticación.", null, 0);
            }
        }
    }
}