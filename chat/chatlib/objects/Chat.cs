using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatlib.objects
{
    /*
     Esta classe é a base para a classe Chat.
    como ele vai funcionar:

    o chat funciona de forma bidirecional de ponto a ponto,
    ou seja um usuario pode enviar mensagens para outro usuario
    essas mensagens passam pelo servidor, que seleciona o usuario para 
    quem sera enviada a mensagem, e o usuario que recebera a mensagem

    o que e nessesario para a classe chat:

    - o usuario que enviou a mensagem
    - o usuario que recebera a mensagem
    - a mensagem (encriptada)
    - chave publica do usuario que recebera a mensagem

    processo de envio:
    
    0. Usuario entra na aplicação e o programa gera um par de chaves (apenas do lado do usuario assim nem o servidor sabe as chaves)
    1. Usuario envia a chave publica para o servidor (o servidor armazena a chave publica do usuario)
    2. Usuario entra na sala de chat (clica no user que quer conversar)
    3. Sevidor envia a chave publica do usuario que recebera a mensagem (o usuario que o usuario quer conversar)
    4. Usuario envia a mensagem (encriptada com a chave publica do usuario que recebera a mensagem)
    5. Servidor envia a mensagem para o usuario que recebera a mensagem (o usuario que o usuario quer conversar)
    6. Usuario que recebera a mensagem desencripta a mensagem com sua chave privada
    7. Usuario que recebera a mensagem recebe a mensagem desencriptada
    8. Fim


     O gestor de envio e responsavel por armazenar os dados dos chats que não 
     estão diretamente conectados com as mensagens de envio, ou seja, ele armazena
     dados e cruza informações para o servidor.

     Dados como:
     - usuario que enviou a mensagem (username)
     - Porta ligada ao usuario
     - Ip ligado ao usuario

    funções:
    - Adicionar usuario a lista
    - Encontrar usuario na lista
    - Fornecer dados do usuario
     */

    public class UserManager
    {
        /*
         Essa classe e responsavel por armazenar os dados dos usuarios
         */
        public List<User> Users { get; set; } = new List<User>();
        public List<Chat> Chats { get; set; } = new List<Chat>();

        public void AddUser(User user)
        {
            Users.Add(user);
        }

        public void RemoveUser(string username)
        {
            // u == usernanme -> u.Username == username
            var user = Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                Users.Remove(user);
            }
        }

        public User? GetUser(string username)
        {
            // u = usernanme -> u.Username == username
            return Users.FirstOrDefault(u => u.Username == username);
        }

        public User? GetUser(int id)
        {
            // u = id -> u.Id == id
            return Users.FirstOrDefault(u => u.Id == id);
        }

        public List<User> GetAllUsers()
        {
            return Users;
        }
    }

    public class Chat
    {
        public string? Type { get; set; } // "Chat", "FriendRequest", "FriendResponse", null = primeira mensagem de registro
        public string? Sender { get; set; }
        public string? Receiver { get; set; }
        public string? Message { get; set; }
        public string? PublicKey { get; set; }
    }
}
