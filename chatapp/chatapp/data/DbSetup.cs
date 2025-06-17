using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatlib;
using Microsoft.Data.Sqlite;

/*
using Microsoft.Data.Sqlite;

string connectionString = "Data Source=meubanco.db";

// Criar a conexão
using var connection = new SqliteConnection(connectionString);
connection.Open();

// Criar tabela se não existir
var createCmd = connection.CreateCommand();
createCmd.CommandText = """
    CREATE TABLE IF NOT EXISTS pessoas (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        nome TEXT NOT NULL,
        idade INTEGER
    );
""";
createCmd.ExecuteNonQuery();

// Inserir dados
var insertCmd = connection.CreateCommand();
insertCmd.CommandText = """
    INSERT INTO pessoas (nome, idade) VALUES ($nome, $idade);
""";
insertCmd.Parameters.AddWithValue("$nome", "Maria");
insertCmd.Parameters.AddWithValue("$idade", 30);
insertCmd.ExecuteNonQuery();

// Ler dados
var selectCmd = connection.CreateCommand();
selectCmd.CommandText = "SELECT id, nome, idade FROM pessoas;";
using var reader = selectCmd.ExecuteReader();

Console.WriteLine("Lista de pessoas:");
while (reader.Read())
{
    var id = reader.GetInt32(0);
    var nome = reader.GetString(1);
    var idade = reader.GetInt32(2);
    Console.WriteLine($"ID: {id}, Nome: {nome}, Idade: {idade}");
}
 */

/*
Tabelas:

        public string Username { get; set; }
        private string passwordHash;
        private string PrivateKey = string.Empty; // Inicializado para evitar CS8618
        private string PublicKey = string.Empty; // Inicializado para evitar CS8618
        private string PublicKeyEncrypted = string.Empty; // Inicializado para evitar CS8618
        private string PrivateKeyEncrypted = string.Empty; // Inicializado para evitar CS8618
        private string Salt = string.Empty; // Inicializado para evitar CS8618
        public string UniqueId;

Mensagens:
    public class MessageData
    {
        public int Id { get; set; } // ID da mensagem, necessário para Entity Framework
        public string From { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string To { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string MessageText { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Data e hora do envio da mensagem, inicializada para evitar CS8618
    }

Amigos
    public class Friend
    {
        public string Username { get; set; } = string.Empty; // Nome de usuário do amigo, inicializado para evitar CS8618
        public string SelfUsername { get; set; } = string.Empty; // Nome de usuário do próprio usuário, inicializado para evitar CS8618
        public string UniqueId { get; set; } = string.Empty; // ID único do amigo, inicializado para evitar CS8618
        public string PublicKey { get; set; } = string.Empty; // Chave pública do amigo, inicializado para evitar CS8618
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data de criação do amigo, inicializada para evitar CS8618
    }

 */


namespace chatapp.data
{
    public class DbSetup
    {
        public DbSetup() { }

        public static SqliteConnection CreateConnection(string dbPath = "Data Source=chatapp.db")
        {
            var connection = new SqliteConnection(dbPath);
            connection.Open();
            return connection;
        }

        public DbSetup(SqliteConnection connection)
        {
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    // Cria a tabela de usuários
                    cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Username TEXT PRIMARY KEY,
                    PasswordHash TEXT NOT NULL,
                    PrivateKey TEXT NOT NULL,
                    PublicKey TEXT NOT NULL,
                    PublicKeyEncrypted TEXT NOT NULL,
                    PrivateKeyEncrypted TEXT NOT NULL,
                    Salt TEXT NOT NULL,
                    UniqueId TEXT NOT NULL
                );";
                    cmd.ExecuteNonQuery();

                    // Cria a tabela de amigos
                    cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Friends (
                    Username TEXT NOT NULL,
                    SelfUsername TEXT NOT NULL,
                    UniqueId TEXT NOT NULL,
                    PublicKey TEXT NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
                    cmd.ExecuteNonQuery();

                    // Cria a tabela de mensagens
                    cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Messages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ""From"" TEXT NOT NULL,
                    ""To"" TEXT NOT NULL,
                    MessageText TEXT NOT NULL,
                    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
                    cmd.ExecuteNonQuery();
                }

                // Fecha a conexão
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao configurar o banco de dados: {ex.Message}");
                // Aqui você pode tratar o erro de forma mais específica, se necessário
            }
        }


        // Crud User (o user é local logo só pode existir um único usuário)
        public void CreateUser(SqliteConnection connection, string username, string passwordHash, string privateKey, string publicKey, string publicKeyEncrypted, string privateKeyEncrypted, string salt, string uniqueId)
        {
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    // Insere o usuário
                    cmd.CommandText = @"
                    INSERT INTO Users (Username, PasswordHash, PrivateKey, PublicKey, PublicKeyEncrypted, PrivateKeyEncrypted, Salt, UniqueId)
                    VALUES ($username, $passwordHash, $privateKey, $publicKey, $publicKeyEncrypted, $privateKeyEncrypted, $salt, $uniqueId);";
                    cmd.Parameters.AddWithValue("$username", username);
                    cmd.Parameters.AddWithValue("$passwordHash", passwordHash);
                    cmd.Parameters.AddWithValue("$privateKey", privateKey);
                    cmd.Parameters.AddWithValue("$publicKey", publicKey);
                    cmd.Parameters.AddWithValue("$publicKeyEncrypted", publicKeyEncrypted);
                    cmd.Parameters.AddWithValue("$privateKeyEncrypted", privateKeyEncrypted);
                    cmd.Parameters.AddWithValue("$salt", salt);
                    cmd.Parameters.AddWithValue("$uniqueId", uniqueId);
                    cmd.ExecuteNonQuery();
                }

                // Fecha a conexão
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar usuário: {ex.Message}");
                // Aqui você pode tratar o erro de forma mais específica, se necessário
            }
        }

        // Busca o usuário 
        public User GetUser(SqliteConnection connection, string username)
        {
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Users WHERE Username = $username;";
                    cmd.Parameters.AddWithValue("$username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            User user = new User()
                                .SetUsername(reader.GetString(0))
                                .SetPasswordHash(reader.GetString(1))
                                .SetPrivateKey(reader.GetString(2))
                                .SetPublicKey(reader.GetString(3))
                                .SetPublicKeyEncrypted(reader.GetString(4))
                                .SetPrivateKeyEncrypted(reader.GetString(5))
                                .SetSalt(reader.GetString(6))
                                .SetUniqueId(reader.GetString(7));

                            // Fecha a conexão
                            connection.Close();

                            return user; // Usuário encontrado e retornado
                        }
                    }
                }
                return null; // Usuário não encontrado
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar usuário: {ex.Message}");
                // Aqui você pode tratar o erro de forma mais específica, se necessário
                return null; // Retorna null em caso de erro
            }
        }

        // Crud Message
        public void CreateMessage(SqliteConnection connection, string from, string to, string messageText)
        {
            try
            {
                using (var validate = connection.CreateCommand())
                {
                    // Verifica se ja existe uma mensagem com o mesmo From, To e MessageText (por motivos de teste os dois estão na mesma maquina logo a db é igual e isso gera duplicidade de mensagens)
                    validate.CommandText = "SELECT COUNT(*) FROM Messages WHERE \"From\" = $from AND \"To\" = $to AND MessageText = $messageText;";
                    validate.Parameters.AddWithValue("$from", from);
                    validate.Parameters.AddWithValue("$to", to);
                    validate.Parameters.AddWithValue("$messageText", messageText);
                    long count = (long)validate.ExecuteScalar();
                    if (count > 0)
                    {
                        // Se já existe, não insere novamente
                        return;
                    }

                }

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Messages (""From"", ""To"", MessageText)
                    VALUES ($from, $to, $messageText);";
                    cmd.Parameters.AddWithValue("$from", from);
                    cmd.Parameters.AddWithValue("$to", to);
                    cmd.Parameters.AddWithValue("$messageText", messageText);
                    cmd.ExecuteNonQuery();
                }

                // Fecha a conexão
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar mensagem: {ex.Message}");
                // Aqui você pode tratar o erro de forma mais específica, se necessário
            }
        }

        // Busca mensagens por To
        public List<MessageDataClient> GetMessages(SqliteConnection connection, string username)
        {
            try
            {
                var messages = new List<MessageDataClient>();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Messages WHERE \"From\" = $username ORDER BY Timestamp;";
                    cmd.Parameters.AddWithValue("$username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new MessageDataClient
                            {
                                Id = reader.GetInt32(0),
                                From = reader.GetString(1),
                                To = reader.GetString(2),
                                MessageText = reader.GetString(3),
                                Timestamp = reader.GetDateTime(4)
                            });
                        }
                    }
                }

                // Fecha a conexão
                connection.Close();
                return messages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar mensagens: {ex.Message}");
                // Aqui você pode tratar o erro de forma mais específica, se necessário
                return new List<MessageDataClient>();
            }
        }

        // Crud Friend
        public void CreateFriend(SqliteConnection connection, string username, string selfUsername, string uniqueId, string publicKey)
        {
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    // se existe um registro com os mesmos dados na mesma ordem, não insere
                    /*
                     Exemplo se username = "amigo1", selfUsername = "eu", uniqueId = "12345" e publicKey = "chavePublica"
                    e já existe um registro com esses mesmos dados, não insere novamente simplesmente ignora a inserção.
                     */

                    // Validar se já existe um amigo com o mesmo username e selfUsername na ordem recebida
                    // Verifica se já existe
                    cmd.CommandText = "SELECT COUNT(*) FROM Friends WHERE Username = $username AND SelfUsername = $selfUsername;";
                    cmd.Parameters.AddWithValue("$username", username);
                    cmd.Parameters.AddWithValue("$selfUsername", selfUsername);
                    long count = (long)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        return;
                    }

                    using (var insertCmd = connection.CreateCommand())
                    {
                        insertCmd.CommandText = @"
                            INSERT INTO Friends (Username, SelfUsername, UniqueId, PublicKey)
                            VALUES ($username, $selfUsername, $uniqueId, $publicKey);";
                        insertCmd.Parameters.AddWithValue("$username", username);
                        insertCmd.Parameters.AddWithValue("$selfUsername", selfUsername);
                        insertCmd.Parameters.AddWithValue("$uniqueId", uniqueId);
                        insertCmd.Parameters.AddWithValue("$publicKey", publicKey);
                        insertCmd.ExecuteNonQuery();
                    }
                }
                // Fecha a conexão
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar amigo: {ex.Message}");
                // Aqui você pode tratar o erro de forma mais específica, se necessário
            }
        }

        public List<Friend> GetFriends(SqliteConnection connection, string selfUsername)
        {
            try
            {
                var friends = new List<Friend>();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Friends WHERE SelfUsername = $selfUsername;";
                    cmd.Parameters.AddWithValue("$selfUsername", selfUsername);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            friends.Add(new Friend
                            {
                                Username = reader.GetString(0),
                                SelfUsername = reader.GetString(1),
                                UniqueId = reader.GetString(2),
                                PublicKey = reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4)
                            });
                        }
                    }
                }
                // Fecha a conexão
                connection.Close();
                return friends;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar amigos: {ex.Message}");
                // Aqui você pode tratar o erro de forma mais específica, se necessário
                return new List<Friend>();
            }
        }

        public bool UpdateFriendPubKey(string publicKey, string username)
        {
            try
            {
                using (var connection = CreateConnection())
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Friends SET PublicKey = $publicKey WHERE Username = $username;";
                    cmd.Parameters.AddWithValue("$publicKey", publicKey);
                    cmd.Parameters.AddWithValue("$username", username);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0; // Retorna true se a atualização foi bem-sucedida
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar chave pública do amigo: {ex.Message}");
                return false; // Retorna false em caso de erro
            }
        }
    }
}
