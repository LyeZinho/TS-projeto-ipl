using chatlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace api.Models
{
    public class ValidationSessionModel
    {
        // Essa classe é usada para validar uma sessão de usuário
        public int Id { get; set; } // ID da sessão, necessário para Entity Framework
        public string Username { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string UniqueId { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string SessionId { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string PublicKey { get; set; } = string.Empty; // Inicializado para evitar CS8618s
        public string AttemptUsername { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string AttemptPassword { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public DateTime createdAt { get; set; } = DateTime.UtcNow; // Data de criação da sessão, inicializada para evitar CS8618
        public DateTime expiration { get; set; } = DateTime.UtcNow.AddDays(1); // Data de expiração da sessão, inicializada para evitar CS8618

        // Construtor padrão necessário para Entity Framework
        public ValidationSessionModel(string username, string uniqueId, string sessionId, string publicKey)
        {
            Username = username;
            UniqueId = uniqueId;
            SessionId = sessionId;
            PublicKey = publicKey;
        }

        // Construtor vazio necessário para Entity Framework
        public ValidationSessionModel() { }

        // Método para validar a sessão
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(UniqueId) && !string.IsNullOrEmpty(SessionId);
        }

        // Crud Entity Framework
        public static ValidationSessionModel Create(string username, string uniqueId, string sessionId, string publicKey)
        {
            using (var context = new AplicationDBContext())
            {
                // Verifica se já existe uma sessão com o mesmo SessionId
                var existingSession = context.ValidationSessions.FirstOrDefault(s => s.SessionId == sessionId || s.Username == username && s.UniqueId == uniqueId);
                if (existingSession != null)
                {
                    // Verifica se a sessão é válida (expirada ou não)
                    if (existingSession.expiration > DateTime.UtcNow)
                    {
                        // Se a sessão ainda é válida, retorna a sessão existente
                        return existingSession;
                    }
                    else
                    {
                        // Se a sessão está expirada, remove-a
                        context.ValidationSessions.Remove(existingSession);
                        context.SaveChanges();
                    }
                }

                var session = new ValidationSessionModel(username, uniqueId, sessionId, publicKey);
                context.ValidationSessions.Add(session);
                context.SaveChanges();
                return session;
            }
        }

        public static ValidationSessionModel GetBySessionId(string sessionId)
        {
            using (var context = new AplicationDBContext())
            {
                return context.ValidationSessions.FirstOrDefault(s => s.SessionId == sessionId);
            }
        }

        public static ValidationSessionModel GetByUsername(string username)
        {
            using (var context = new AplicationDBContext())
            {
                return context.ValidationSessions.FirstOrDefault(s => s.Username == username);
            }
        }

        public static List<ValidationSessionModel> GetAll()
        {
            using (var context = new AplicationDBContext())
            {
                return context.ValidationSessions.ToList();
            }
        }

        public static void DeleteBySessionId(string sessionId)
        {
            using (var context = new AplicationDBContext())
            {
                var session = context.ValidationSessions.FirstOrDefault(s => s.SessionId == sessionId);
                if (session != null)
                {
                    context.ValidationSessions.Remove(session);
                    context.SaveChanges();
                }
            }
        }

        public static void DeleteByUsername(string username)
        {
            using (var context = new AplicationDBContext())
            {
                var session = context.ValidationSessions.FirstOrDefault(s => s.Username == username);
                if (session != null)
                {
                    context.ValidationSessions.Remove(session);
                    context.SaveChanges();
                }
            }
        }
    }
}
