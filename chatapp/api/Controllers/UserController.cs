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
            // Aqui voc� pode implementar a l�gica para obter informa��es do usu�rio
            // Por exemplo, retornar uma lista de usu�rios ou detalhes de um usu�rio espec�fico
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
        // Registra um novo usu�rio
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserModel user)
        {
            if (user == null)
            {
                return BadRequest("User data is required.");
            }
            // Verifica se o usu�rio j� existe
            var existingUser = UserModel.GetUserByUsername(user.Username);
            if (existingUser != null)
            {
                return Conflict("User already exists.");
            }
            // Cria o usu�rio
            var newUser = new UserModel(user.Username, user.GetPasswordHash(), user.GetPublicKey(), user.GetPrivateKey(), user.GetSalt(), user.UniqueId);

            // Salva o usu�rio no banco de dados
            UserModel.CreateUser(newUser);

            // Cria uma sess�o de valida��o
            var session = ValidationSessionModel.Create(newUser.Username, newUser.UniqueId, Guid.NewGuid().ToString(), newUser.GetPublicKey());

            if (session == null)
            {
                return StatusCode(500, "Failed to create validation session.");
            }

            // Retorna a sess�o de valida��o
            return Ok(new
            {
                sessionId = session.SessionId,
                username = session.Username,
                uniqueId = session.UniqueId,
                publicKey = session.PublicKey
            });
        }

        /*
        public int Id { get; set; } // ID da sess�o, necess�rio para Entity Framework
        public string Username { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string UniqueId { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string SessionId { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string PublicKey { get; set; } = string.Empty; // Inicializado para evitar CS8618
         */
        // Faz login do usu�rio
        [HttpPost("login")]
        public IActionResult Login([FromBody] ValidationSessionModel validationSessionModel)
        {
            if (validationSessionModel == null)
            {
                return BadRequest("Validation session data is required.");
            }
            // Obt�m o usu�rio pelo nome de usu�rio
            var user = UserModel.GetUserByUsername(validationSessionModel.AttemptUsername);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Verifica se a senha est� correta
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
            // Retorna a sess�o de valida��o
            return Ok(new
            {
                sessionId = session.SessionId,
                username = session.Username,
                uniqueId = session.UniqueId,
                publicKey = session.PublicKey
            });
        }

        // Busca as informa��s do usuario e retorna o usu�rio
        // Deve ser chamado com o sessionid do usu�rio
        // E informa��es pessoais do usu�rio devem ser suprimidas por <privateinfo>
        // s�o elas: passwordHash, PrivateKey, PrivateKeyEncrypted, Salt, 
        // PublicKeyEncrypted, PublicKey ��o deve ser encriptada ja que n�o � uma informa��o sens�vel 
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
            // Obt�m a sess�o de valida��o
            var session = ValidationSessionModel.GetBySessionId(sessionid);
            if (session == null || !session.IsValid())
            {
                return Unauthorized("Invalid session.");
            }
            // Obt�m o usu�rio pelo nome de usu�rio
            var user = UserModel.GetUserByUsername(username);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            // Retorna o usu�rio com informa��es sens�veis suprimidas
            return Ok(new
            {
                user.Username,
                user.PublicKey,
                user.UniqueId,
                PrivateKey = "<privateinfo>",
                PublicKeyEncrypted = "<privateinfo>",
                PrivateKeyEncrypted = "<privateinfo>",
                Salt = "<privateinfo>",
                PasswordHash = "<privateinfo>"
            });
        }

        // Valida se a sess�o do user � valida
        [HttpGet("validatesession")]
        public IActionResult ValidateSession([FromQuery] string sessionid)
        {
            if (string.IsNullOrEmpty(sessionid))
            {
                return BadRequest("Session ID is required.");
            }
            // Obt�m a sess�o de valida��o
            var session = ValidationSessionModel.GetBySessionId(sessionid);
            if (session == null || !session.IsValid())
            {
                return Unauthorized("Invalid session.");
            }
            // Retorna a sess�o se for v�lida
            return Ok(new
            {
                session.SessionId,
                session.Username,
                session.UniqueId,
                session.PublicKey
            });
        }
    }
}
