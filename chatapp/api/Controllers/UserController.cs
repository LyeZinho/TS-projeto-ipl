using Microsoft.AspNetCore.Mvc;
using api.Models;
using chatlib;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("User endpoint is working.");
        }


        /*
        public class UserModel
        public string Username { get; set; }
        private string passwordHash;
        private string PrivateKey = string.Empty; // Inicializado para evitar CS8618
        private string PublicKey = string.Empty; // Inicializado para evitar CS8618
        private string PublicKeyEncrypted = string.Empty; // Inicializado para evitar CS8618
        private string PrivateKeyEncrypted = string.Empty; // Inicializado para evitar CS8618
        private string Salt = string.Empty; // Inicializado para evitar CS8618
        public string UniqueId;
         */
        // Registra um novo usuário
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserModel user)
        {
            if (user == null)
            {
                return BadRequest("User data is required.");
            }
            // Verifica se o usuário já existe
            var existingUser = UserModel.GetUserByUsername(user.Username);
            if (existingUser != null)
            {
                return Conflict("User already exists.");
            }
            // Cria o usuário
            var newUser = new UserModel(user.Username, user.GetPasswordHash(), user.GetPublicKey(), user.GetPrivateKey(), user.GetSalt(), user.UniqueId);

            // Salva o usuário no banco de dados
            UserModel.CreateUser(newUser);

            // Cria uma sessão de validação
            var session = ValidationSessionModel.Create(newUser.Username, newUser.UniqueId, Guid.NewGuid().ToString(), newUser.GetPublicKey());

            if (session == null)
            {
                return StatusCode(500, "Failed to create validation session.");
            }

            // Retorna a sessão de validação
            return Ok(new
            {
                sessionId = session.SessionId,
                username = session.Username,
                uniqueId = session.UniqueId,
                publicKey = session.PublicKey
            });
        }

        /*
        public int Id { get; set; } // ID da sessão, necessário para Entity Framework
        public string Username { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string UniqueId { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string SessionId { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string PublicKey { get; set; } = string.Empty; // Inicializado para evitar CS8618
         */
        // Faz login do usuário
        [HttpPost("login")]
        public IActionResult Login([FromBody] ValidationSessionModel validationSessionModel)
        {
            if (validationSessionModel == null)
            {
                return BadRequest("Validation session data is required.");
            }
            // Obtém o usuário pelo nome de usuário
            var user = UserModel.GetUserByUsername(validationSessionModel.AttemptUsername);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Verifica se a senha está correta
            string attemptPasswordHash = Encript.HashPassword(validationSessionModel.AttemptPassword, user.Salt);

            if (user.GetPasswordHash() != attemptPasswordHash)
            {
                return Unauthorized("Invalid password.");
            }

            var session = ValidationSessionModel.Create(user.Username, user.UniqueId, validationSessionModel.SessionId, user.GetPublicKey());
            if (session == null)
            {
                return StatusCode(500, "Failed to create validation session.");
            }
            // Retorna a sessão de validação
            return Ok(new
            {
                sessionId = session.SessionId,
                username = session.Username,
                uniqueId = session.UniqueId,
                publicKey = session.PublicKey
            });
        }

        // Busca as informaçõs do usuario e retorna o usuário
        // Deve ser chamado com o sessionid do usuário
        /*
        public class UserModel
        public string Username { get; set; }
        private string passwordHash;
        private string PrivateKey = string.Empty; // Inicializado para evitar CS8618
        private string PublicKey = string.Empty; // Inicializado para evitar CS8618
        private string PublicKeyEncrypted = string.Empty; // Inicializado para evitar CS8618
        private string PrivateKeyEncrypted = string.Empty; // Inicializado para evitar CS8618
        private string Salt = string.Empty; // Inicializado para evitar CS8618
        public string UniqueId;
        */
        [HttpGet("getuser")]
        public IActionResult GetUser([FromQuery] string username, string sessionid)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(sessionid))
            {
                return BadRequest("Username and session ID are required.");
            }
            // Obtém a sessão de validação
            var session = ValidationSessionModel.GetBySessionId(sessionid);
            if (session == null || !session.IsValid())
            {
                return Unauthorized("Invalid session.");
            }
            // Obtém o usuário pelo nome de usuário
            var user = UserModel.GetUserByUsername(username);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(new
            {
                user.Username,
                user.PublicKey,
                user.UniqueId,
                PrivateKey = user.GetPrivateKey(),
                PublicKeyEncrypted = user.GetPublicKeyEncrypted(),
                PrivateKeyEncrypted = user.GetPrivateKeyEncrypted(),
                Salt = user.GetSalt(),
                PasswordHash = user.GetPasswordHash()
            });
        }

        // Valida se a sessão do user é valida
        [HttpGet("validatesession")]
        public IActionResult ValidateSession([FromQuery] string sessionid)
        {
            if (string.IsNullOrEmpty(sessionid))
            {
                return BadRequest("Session ID is required.");
            }
            // Obtém a sessão de validação
            var session = ValidationSessionModel.GetBySessionId(sessionid);
            if (session == null || !session.IsValid())
            {
                return Unauthorized("Invalid session.");
            }
            // Retorna a sessão se for válida
            return Ok(new
            {
                session.SessionId,
                session.Username,
                session.UniqueId,
                session.PublicKey
            });
        }


        // Busca a public key de um usuario por nome
        [HttpGet("getpublickey")]
        public IActionResult GetPublicKey([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username is required.");
            }
            // Obtém o usuário pelo nome de usuário
            var user = UserModel.GetUserByUsername(username);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            // Retorna a chave pública do usuário
            return Ok(new
            {
                user.PublicKey
            });
        }
    }
}
