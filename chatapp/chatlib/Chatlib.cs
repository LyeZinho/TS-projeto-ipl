using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace chatlib
{
    public class User
    {
        public string Username { get; set; }
        private string passwordHash;
        private string PrivateKey = string.Empty; // Inicializado para evitar CS8618
        private string PublicKey = string.Empty; // Inicializado para evitar CS8618
        private string PublicKeyEncrypted = string.Empty; // Inicializado para evitar CS8618
        private string PrivateKeyEncrypted = string.Empty; // Inicializado para evitar CS8618
        private string Salt = string.Empty; // Inicializado para evitar CS8618
        public string UniqueId;
        TcpClient Client { get; set; } = new TcpClient(); // Cliente TCP para conexão com o servidor

        // Builders para definir os valores 

        #region Builders
        public User SetPrivateKey(string privateKey)
        {
            if (string.IsNullOrEmpty(privateKey))
                throw new ArgumentException("A chave privada não pode ser nula ou vazia.");
            PrivateKey = privateKey;
            return this;
        }

        public User SetPublicKey(string publicKey)
        {
            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException("A chave pública não pode ser nula ou vazia.");
            PublicKey = publicKey;
            return this;
        }

        public User SetPublicKeyEncrypted(string publicKeyEncrypted)
        {
            if (string.IsNullOrEmpty(publicKeyEncrypted))
                throw new ArgumentException("A chave pública criptografada não pode ser nula ou vazia.");
            PublicKeyEncrypted = publicKeyEncrypted;
            return this;
        }

        public User SetPrivateKeyEncrypted(string privateKeyEncrypted)
        {
            if (string.IsNullOrEmpty(privateKeyEncrypted))
                throw new ArgumentException("A chave privada criptografada não pode ser nula ou vazia.");
            PrivateKeyEncrypted = privateKeyEncrypted;
            return this;
        }

        public User SetSalt(string salt)
        {
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentException("O salt não pode ser nulo ou vazio.");
            Salt = salt;
            return this;
        }

        public User SetUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("O nome do usuário não pode ser nulo ou vazio.");
            Username = username;
            return this;
        }

        public User SetUniqueId(string uniqueId)
        {
            if (string.IsNullOrEmpty(uniqueId))
                throw new ArgumentException("O ID único não pode ser nulo ou vazio.");
            UniqueId = uniqueId;
            return this;
        }

        public User SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
                throw new ArgumentException("A senha não pode ser nula ou vazia.");
            this.passwordHash = passwordHash;
            return this;
        }

        public User SetClient(TcpClient client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client), "O cliente não pode ser nulo.");
            Client = client;
            return this;
        }

        public User SetClient(string ip, int port)
        {
            // Método para definir o cliente TCP com IP e porta
            if (string.IsNullOrEmpty(ip))
                throw new ArgumentException("O IP não pode ser nulo ou vazio.");
            if (port <= 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "A porta deve estar entre 1 e 65535.");
            Client = new TcpClient(ip, port);
            return this;
        }

        public TcpClient GetClient()
        {
            // Método para obter o cliente TCP
            if (Client == null)
                throw new InvalidOperationException("O cliente não foi definido.");
            return Client;
        }

        public User Build()
        {
            // Método para construir o objeto User após definir todos os valores necessários

            // Por enquanto ignora as validações, pois o construtor já garante que os valores sejam definidos
            //if (string.IsNullOrEmpty(Username))
            //    throw new InvalidOperationException("O nome do usuário deve ser definido antes de construir o objeto User.");
            //if (string.IsNullOrEmpty(passwordHash))
            //    throw new InvalidOperationException("A senha deve ser definida antes de construir o objeto User.");
            //if (string.IsNullOrEmpty(PrivateKey) || string.IsNullOrEmpty(PublicKey) ||
            //    string.IsNullOrEmpty(PublicKeyEncrypted) || string.IsNullOrEmpty(PrivateKeyEncrypted) ||
            //    string.IsNullOrEmpty(Salt))
            //    throw new InvalidOperationException("Todas as chaves e salt devem ser definidos antes de construir o objeto User.");
            return this;
        }
        #endregion

        public string GetPasswordHash()
        {
            // Método para obter o hash da senha
            if (string.IsNullOrEmpty(passwordHash))
                throw new InvalidOperationException("A senha não foi definida.");
            return passwordHash;
        }

        public string GetPublicKey()
        {
            // Método para obter a chave pública
            if (string.IsNullOrEmpty(PublicKey))
                throw new InvalidOperationException("A chave pública não foi definida.");
            return PublicKey;
        }

        public string GetPrivateKey()
        {
            // Método para obter a chave privada
            if (string.IsNullOrEmpty(PrivateKey))
                throw new InvalidOperationException("A chave privada não foi definida.");
            return PrivateKey;
        }

        public string GetPublicKeyEncrypted()
        {
            // Método para obter a chave pública criptografada
            if (string.IsNullOrEmpty(PublicKeyEncrypted))
                throw new InvalidOperationException("A chave pública criptografada não foi definida.");
            return PublicKeyEncrypted;
        }

        public string GetPrivateKeyEncrypted()
        {
            // Método para obter a chave privada criptografada
            if (string.IsNullOrEmpty(PrivateKeyEncrypted))
                throw new InvalidOperationException("A chave privada criptografada não foi definida.");
            return PrivateKeyEncrypted;
        }

        public string GetSalt()
        {
            // Método para obter o salt
            if (string.IsNullOrEmpty(Salt))
                throw new InvalidOperationException("O salt não foi definido.");
            return Salt;
        }

        public User() { } // Construtor padrão para permitir a criação de instâncias sem parâmetros
    }

    public class ValidationSession
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
    }

    public class MessageDataClient
    {
        public int Id { get; set; } // ID da mensagem, necessário para Entity Framework
        public string From { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string To { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public string MessageText { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Data e hora do envio da mensagem, inicializada para evitar CS8618
    }

    public class Friend
    {
        public string Username { get; set; } = string.Empty; // Nome de usuário do amigo, inicializado para evitar CS8618
        public string SelfUsername { get; set; } = string.Empty; // Nome de usuário do próprio usuário, inicializado para evitar CS8618
        public string UniqueId { get; set; } = string.Empty; // ID único do amigo, inicializado para evitar CS8618
        public string PublicKey { get; set; } = string.Empty; // Chave pública do amigo, inicializado para evitar CS8618
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data de criação do amigo, inicializada para evitar CS8618
    }


    public class Friends
    {
        public string OriginUser { get; private set; }
        public string DestinationUser { get; private set; }

        public Friends(string originUser, string destinationUser)
        {
            // Construtor para criar uma instância de Friends com usuários de origem e destino
            if (string.IsNullOrEmpty(originUser) || string.IsNullOrEmpty(destinationUser))
                throw new ArgumentException("Usuários não podem ser nulos ou vazios.");
            OriginUser = originUser;
            DestinationUser = destinationUser;
        }
    }


    public class Connections
    {
        Dictionary<string, User> Users { get; set; } = new Dictionary<string, User>();
        Dictionary<string, List<Friends>> FriendsList { get; set; } = new Dictionary<string, List<Friends>>();

        public void AddUser(User user)
        {
            // Adiciona um usuário ao sistema
            if (user == null || string.IsNullOrEmpty(user.Username))
                throw new ArgumentException("Usuário inválido.");
            // Verifica se o usuário já existe no dicionário de usuários
            Users[user.Username] = user;
        }

        public void AddFriend(string originUser, string destinationUser)
        {
            // Adiciona um amigo à lista de amigos do usuário de origem
            if (string.IsNullOrEmpty(originUser) || string.IsNullOrEmpty(destinationUser))
                throw new ArgumentException("Usuários não podem ser nulos ou vazios.");
            // Verifica se ambos os usuários existem no dicionário de usuários
            if (!Users.ContainsKey(originUser) || !Users.ContainsKey(destinationUser))
                throw new KeyNotFoundException("Um ou ambos os usuários não existem.");
            // Verifica se a lista de amigos do usuário de origem já existe, caso contrário, cria uma nova lista
            if (!FriendsList.ContainsKey(originUser))
                FriendsList[originUser] = new List<Friends>();
            // Verifica se o amigo já está na lista para evitar duplicatas
            FriendsList[originUser].Add(new Friends(originUser, destinationUser));
        }

        public List<Friends> GetFriends(string userName)
        {
            // Obtém a lista de amigos de um usuário
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("O nome do usuário não pode ser nulo ou vazio.");
            // Verifica se o usuário existe no dicionário de amigos
            if (!FriendsList.ContainsKey(userName))
                return new List<Friends>();
            // Retorna a lista de amigos do usuário
            return FriendsList[userName];
        }

        public User GetUser(string userName)
        {
            // Obtém um usuário pelo nome de usuário
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("O nome do usuário não pode ser nulo ou vazio.");
            // Verifica se o usuário existe no dicionário de usuários
            if (!Users.ContainsKey(userName))
                throw new KeyNotFoundException("Usuário não encontrado.");
            // Retorna o usuário correspondente ao nome de usuário
            return Users[userName];
        }

        public User GetUserByAdress(string ip, int port)
        {
            // Obtém um usuário pelo endereço IP e porta do cliente
            if (string.IsNullOrEmpty(ip) || port <= 0 || port > 65535)
                throw new ArgumentException("IP ou porta inválidos.");
            // Percorre o dicionário de usuários para encontrar o usuário com o cliente correspondente
            foreach (var user in Users.Values)
            {
                if (user.GetClient().Client.RemoteEndPoint is IPEndPoint remoteEndPoint &&
                    remoteEndPoint.Address.ToString() == ip && remoteEndPoint.Port == port)
                {
                    return user;
                }
            }
            throw new KeyNotFoundException("Usuário não encontrado pelo endereço IP e porta.");
        }

        // Retorna uma conexão TCP para o usuário especificado
        public TcpClient GetTcpClient(string userName)
        {
            // Obtém o cliente TCP de um usuário pelo nome de usuário
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("O nome do usuário não pode ser nulo ou vazio.");
            // Verifica se o usuário existe no dicionário de usuários
            if (!Users.ContainsKey(userName))
                throw new KeyNotFoundException("Usuário não encontrado.");
            // Retorna o cliente TCP do usuário
            return Users[userName].GetClient();
        }

        public User GetUserByUniqueId(string uniqueId)
        {
            // Obtém um usuário pelo ID único
            if (string.IsNullOrEmpty(uniqueId))
                throw new ArgumentException("O ID único não pode ser nulo ou vazio.");
            // Percorre o dicionário de usuários para encontrar o usuário com o ID único correspondente
            foreach (var user in Users.Values)
            {
                if (user.UniqueId == uniqueId)
                {
                    return user;
                }
            }
            throw new KeyNotFoundException("Usuário não encontrado pelo ID único.");
        }

        public void RemoveUser(string userName)
        {
            // Remove um usuário do sistema, incluindo sua lista de amigos
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("O nome do usuário não pode ser nulo ou vazio.");
            // Verifica se o usuário existe
            if (!Users.ContainsKey(userName))
                throw new KeyNotFoundException("Usuário não encontrado.");
            // Remove o usuário do dicionário de usuários e da lista de amigos
            Users.Remove(userName);
            FriendsList.Remove(userName);

            // Remove o usuário de todas as listas de amigos dos outros usuários
            foreach (var friends in FriendsList.Values)
            {
                // Remove todas as instâncias de amigos onde o usuário é o usuário de origem ou destino
                friends.RemoveAll(f => f.OriginUser == userName || f.DestinationUser == userName);
            }
        }

        public void RemoveFriend(string originUser, string destinationUser)
        {
            // Remove um amigo da lista de amigos do usuário de origem
            if (string.IsNullOrEmpty(originUser) || string.IsNullOrEmpty(destinationUser))
                throw new ArgumentException("Usuários não podem ser nulos ou vazios.");
            // Verifica se o usuário de origem existe
            if (!FriendsList.ContainsKey(originUser))
                throw new KeyNotFoundException("Usuário de origem não encontrado.");
            // Verifica se o usuário de destino existe
            FriendsList[originUser].RemoveAll(f => f.DestinationUser == destinationUser);
        }

        public User GetUserByUsername(string userna)
        {
            // Obtém um usuário pelo nome de usuário
            if (string.IsNullOrEmpty(userna))
                throw new ArgumentException("O nome do usuário não pode ser nulo ou vazio.");
            // Verifica se o usuário existe no dicionário de usuários
            if (!Users.ContainsKey(userna))
                throw new KeyNotFoundException("Usuário não encontrado.");
            // Retorna o usuário correspondente ao nome de usuário
            return Users[userna];
        }

        public List<User> GetAllUsers()
        {
            // Retorna uma lista de todos os usuários no sistema
            return new List<User>(Users.Values);
        }

        public void Clear()
        {
            Users.Clear();
            FriendsList.Clear();
        }


        public Connections() { }
    }

    /*

    Default package payload:

    {
        type: "", -> Tipo de payload recebido isto age como um filtro para a forma que o payload sera tratado.
        data: {} -> Campo generico para enviar dados específicos de cada tipo de comando.
        timestamp: "" -> Timestamp do comando, usado para sincronização e controle de tempo.
    }

    */

    public class Payload
    {
        public TypePayload Type { get; set; }
        public object Data { get; set; }
        public string Timestamp { get; set; }
        public Payload(TypePayload type, object data, string timestamp)
        {
            Type = type;
            Data = data;
            Timestamp = timestamp;
        }
        public Payload() { }
    }

    public class MessageData
    {
        public string Message { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }

    public class FriendRequestData
    {
        public string From { get; set; }
        public string To { get; set; }
        public string UniqueId { get; set; } // ID único do usuário que está enviando o pedido de amizade
        public string SelfPublicKey { get; set; }
    }

    public class FriendReplyData
    {
        public string From { get; set; }
        public string To { get; set; }
        public bool Accepted { get; set; }
        public string SelfPublicKey { get; set; } // Caso não seja aceito, self public key e vazio
        public string UniqueId { get; set; } // ID único do usuário que está respondendo ao pedido de amizade
    }

    public class ConnectionData
    {
        public string Username { get; set; }
        public string UniqueId { get; set; }
    }

    public class clientConnection
    {
        public string Username { get; set; }
        public string UniqueId { get; set; }
        public string address { get; set; } // Endereço IP do cliente
        public int Port { get; set; } // Porta do cliente
        public TcpClient TcpClient { get; set; } // Cliente TCP associado
    }


    public class SerializationHelper
    {
        /*
         Elimina o problema de serialização de JSONs com caracteres de controle e nulos.
         */
        public static byte[] CompressJson(string json)
        {
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(jsonBytes, 0, jsonBytes.Length);
            }

            return output.ToArray();
        }

        public static string DecompressJson(byte[] compressed)
        {
            using var input = new MemoryStream(compressed);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var reader = new StreamReader(gzip, Encoding.UTF8);

            return reader.ReadToEnd();
        }


        public static string Serialize(Payload payload)
        {
            // Serializa um objeto Payload para uma string JSON
            return System.Text.Json.JsonSerializer.Serialize(payload);
        }

        public static Payload Deserialize(string json)
        {
            // Deserializa um objeto Payload para uma string JSON
            return System.Text.Json.JsonSerializer.Deserialize<Payload>(json) ?? throw new InvalidOperationException("Falha ao desserializar o JSON para Payload.");
        }

        public Payload ByteToPayload(byte[] data)
        {
            // Converte um array de bytes em um objeto Payload
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data), "Os dados não podem ser nulos ou vazios.");
            // Decomprime o array de bytes para obter a string JSON
            string json = DecompressJson(data);
            // Verifica se a string JSON é nula ou vazia
            using var output = new MemoryStream();
            if (string.IsNullOrEmpty(json))
                throw new InvalidOperationException("A string JSON não pode ser nula ou vazia.");
            // Desserializa a string JSON para um objeto Payload
            return Deserialize(json) ?? throw new InvalidOperationException("Falha ao desserializar o JSON para Payload.");
        }

        public byte[] PayloadToByte(Payload payload)
        {
            // Converte um objeto Payload em um array de bytes
            if (payload == null)
                throw new ArgumentNullException(nameof(payload), "O payload não pode ser nulo.");
            // Serializa o objeto Payload para uma string JSON e converte para um array de bytes
            string json = Serialize(payload);
            // Comprime a string JSON para reduzir o tamanho do array de bytes e não conter caracteres de controle
            return CompressJson(json);
        }


        public Payload ReplyBufferToPayload(byte[] buffer)
        {
            // Algumas vezes o buffer pode conter bytes extras, isso causa erros no processo de conversão
            // Exemplo de string que o buffer retorna: "\u0001\0\u00010\"Type\":6,\"Data\":\"Conex\\u00E3o estabelecida com sucesso.\",\"Timestamp\":\"2025-06-03T22:20:34.3736327Z\"}"
            // A string acima foi convertida para string usando: return System.Text.Encoding.UTF8.GetString(input).TrimEnd('\0');
            // Por isso o processo correto seria obter a string do buffer normalizar e depois desserializar para Payload
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentNullException(nameof(buffer), "O buffer não pode ser nulo ou vazio.");
            // Converte o array de bytes em uma string JSON
            // Descomprime o array de bytes para obter a string JSON
            string json = DecompressJson(buffer);
            // Verifica se a string JSON é nula ou vazia
            if (string.IsNullOrEmpty(json))
                throw new InvalidOperationException("A string JSON não pode ser nula ou vazia.");
            // Desserializa a string JSON para um objeto Payload
            return Deserialize(json);
        }

        public object Deserialize<T>(string? v)
        {
            throw new NotImplementedException();
        }

        public T DeserializeData<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json)
                ?? throw new InvalidOperationException("Falha na desserialização.");
        }
    }

    public enum TypePayload
    {
        CONNECT,
        MESSAGE,
        FRIENDADD,
        FRIENDREMOVE,
        FRIENDREPLY,
        FRIENDREQUEST,
        ACK,
        EOT // End of Transmission
    }
}
