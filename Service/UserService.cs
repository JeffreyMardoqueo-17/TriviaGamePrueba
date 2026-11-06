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
                return (false, "El correo y la contraseña no pueden estar vacíos.");

            var cleanUsername = username.Trim().ToLower();

            if (cleanUsername.Length < 3 || cleanUsername.Length > 50)
                return (false, "El correo debe tener entre 3 y 50 caracteres.");

            if (password.Length < 6)
                return (false, "La contraseña debe tener al menos 6 caracteres.");

            try
            {
                bool userExists = await _userRepository.ExistsAsync(cleanUsername);
                if (userExists)
                {
                    return (false, "Este correo ya está registrado. Por favor, elige otro o inicia sesión.");
                }

                string passwordHash = _passwordHasher.HashPassword(password);
                await _userRepository.RegisterAsync(cleanUsername, passwordHash);

                return (true, "¡Cuenta creada con éxito! Por favor, inicia sesión.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario.");
                return (false, "Ocurrió un error al procesar el registro.");
            }
        }

        public async Task<(bool Success, string Message, User? User, int TotalPoints)> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Debe ingresar su correo y contraseña.", null, 0);

            var cleanUsername = username.Trim().ToLower();

            try
            {
                var user = await _userRepository.Login(cleanUsername);

                if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                {
                    _logger.LogWarning("Intento fallido: Correo no encontrado '{Username}'.", cleanUsername);
                    return (false, "El usuario o la contraseña son incorrectos.", null, 0); // Mensaje genérico por seguridad
                }

                bool isValidPassword = _passwordHasher.VerifyPassword(password, user.PasswordHash);

                if (!isValidPassword)
                {
                    _logger.LogWarning("Intento fallido: Contraseña incorrecta para '{Username}'.", cleanUsername);
                    return (false, "El usuario o la contraseña son incorrectos.", null, 0);
                }

                int totalPoints = await _userRepository.GetUserTotalPointsAsync(user.Id);

                _logger.LogInformation("Usuario '{Username}' autenticado exitosamente.", cleanUsername);
                return (true, "Autenticación exitosa.", user, totalPoints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico durante la autenticación de '{Username}'.", cleanUsername);
                return (false, "Ocurrió un error interno al procesar la autenticación.", null, 0);
            }
        }
    }
}