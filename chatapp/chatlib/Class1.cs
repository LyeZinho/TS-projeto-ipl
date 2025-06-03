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
        private string UniqueId;

        // Builders para definir os valores 

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

        public User(string username, string passwordHash)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("O nome do usuário não pode ser nulo ou vazio.");
            Username = username;
            if (string.IsNullOrEmpty(passwordHash))
                throw new ArgumentException("A senha não pode ser nula ou vazia.");
            this.passwordHash = passwordHash;
            UniqueId = Guid.NewGuid().ToString(); // Gera um ID único para o usuário
        }

        public User(string name, string passwordHash, string privateKey, string publicKey, string publicKeyEncrypted, string privateKeyEncrypted, string salt)
            : this(name, passwordHash)
        {
            SetPrivateKey(privateKey);
            SetPublicKey(publicKey);
            SetPublicKeyEncrypted(publicKeyEncrypted);
            SetPrivateKeyEncrypted(privateKeyEncrypted);
            SetSalt(salt);
        }
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
        type: "", -> 
        CONNECT: O tipo de conexão envia informações do usuario como username, senha etc, isso é usado para autenticação e criação de sessão.
        DATA: O tipo de dado envia mensagens entre usuários, isso é usado para comunicação.
        ACK: O tipo de confirmação é usado para confirmar o recebimento de mensagens ou comandos.
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

    public class SerializationHelper
    {
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
            // Converte o array de bytes em uma string JSON e desserializa para um objeto Payload
            string json = System.Text.Encoding.UTF8.GetString(data);
            // Verifica se a string JSON é nula ou vazia
            return Deserialize(json);
        }

        public byte[] PayloadToByte(Payload payload)
        {
            // Converte um objeto Payload em um array de bytes
            if (payload == null)
                throw new ArgumentNullException(nameof(payload), "O payload não pode ser nulo.");
            // Serializa o objeto Payload para uma string JSON e converte para um array de bytes
            string json = Serialize(payload);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }
    }

    public enum TypePayload
    {
        CONNECT,
        MESSAGE,
        FRIENDADD,
        FRIENDREMOVE,
        LOGIN,
        REGISTER,
        ACK,
        EOT // End of Transmission
    }
}
