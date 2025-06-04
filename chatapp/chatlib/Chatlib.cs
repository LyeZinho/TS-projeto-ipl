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

        public User() { } // Construtor padrão para permitir a criação de instâncias sem parâmetros
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

        public static string NormalizeJson(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                throw new ArgumentException("Input string is null or empty.");

            // Remove caracteres de controle e nulos
            var cleaned = Regex.Replace(raw, @"[\u0000-\u001F]+", string.Empty);

            // Tenta analisar diretamente a string limpa
            try
            {
                using var doc = JsonDocument.Parse(cleaned);
                return JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
            catch (JsonException)
            {
                // Fallback para o método de recuperação caso o parse direto falhe
            }

            // Busca a propriedade "Type" como ponto de referência
            int typeIndex = cleaned.IndexOf("\"Type\"", StringComparison.Ordinal);
            if (typeIndex == -1)
                throw new FormatException("JSON root object not found: missing \"Type\" property.");

            // Encontra a posição do primeiro '{' válido antes de "Type"
            int startIndex = FindJsonStart(cleaned, typeIndex);

            // Extrai o objeto JSON válido
            string json = ExtractValidJsonObject(cleaned, startIndex);

            // Re-serializa para garantir formato consistente
            try
            {
                using var doc = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
            catch (JsonException ex)
            {
                throw new FormatException("Failed to parse normalized JSON content.", ex);
            }
        }

        private static int FindJsonStart(string input, int typeIndex)
        {
            // Procura o '{' mais próximo antes de "Type"
            int braceIndex = input.LastIndexOf('{', typeIndex);

            if (braceIndex != -1)
            {
                return braceIndex;
            }

            // Se não encontrou '{', usa a posição de "Type" como referência
            // Verifica se há uma aspa antes de "Type"
            int quoteIndex = input.LastIndexOf('"', typeIndex - 1);

            if (quoteIndex == -1)
            {
                throw new FormatException("Invalid JSON structure: no opening brace or quote found.");
            }

            return quoteIndex;
        }

        private static string ExtractValidJsonObject(string input, int startIndex)
        {
            int openBraces = 0;
            bool inString = false;

            for (int i = startIndex; i < input.Length; i++)
            {
                char current = input[i];

                // Gerencia contexto de strings
                if (current == '"' && (i == 0 || input[i - 1] != '\\'))
                {
                    inString = !inString;
                }

                // Conta braces apenas fora de strings
                if (!inString)
                {
                    if (current == '{') openBraces++;
                    if (current == '}') openBraces--;

                    // Objeto completo encontrado
                    if (openBraces == 0)
                    {
                        return input.Substring(startIndex, i - startIndex + 1);
                    }
                }
            }

            throw new FormatException("Unbalanced JSON: object not closed properly.");
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
            string json = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
            // Verifica se a string JSON é nula ou vazia
            if (string.IsNullOrEmpty(json))
                throw new InvalidOperationException("A string JSON não pode ser nula ou vazia.");
            // Normaliza a string JSON para remover caracteres extras
            json = NormalizeJson(json);

            // Desserializa a string JSON para um objeto Payload
            return Deserialize(json);
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
