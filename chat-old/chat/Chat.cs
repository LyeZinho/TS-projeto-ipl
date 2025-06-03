using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using chatlib.client;

/*
 exemplo de uso:
// Cria uma instância do cliente
ChatClient client = new ChatClient("username", 5000, "publicKey", "privateKey");
client.Start();
client.MessageReceived += (sender, message) =>
{
    Console.WriteLine($"Mensagem recebida de {sender}: {message}");
};
client.FriendAdded += (sender, message) =>
{
    Console.WriteLine($"Amigo adicionado: {sender}");
};
// Envia uma mensagem
client.SendMessage("targetUsername", "Hello, world!", "targetPublicKey");
// Adiciona um amigo
client.AddFriend("friendUsername", "friendPublicKey");
// Aguarda a resposta do servidor
string response = await client.MessageGet("message");
// Exibe a resposta
Console.WriteLine($"Resposta do servidor: {response}");
// Encerra a conexão
client.Stop();
 */

namespace chat
{
    public partial class Chat : Form
    {
        string username;
        string publicKey;
        string privateKey;
        ChatClient client;

        public Chat(string username)
        {
            // Gera as chaves públicas e privadas
            var (chavePublicaB, chavePrivadaB) = RsaChatCrypto.GerarParDeChaves();
            publicKey = Convert.ToBase64String(chavePublicaB);
            privateKey = Convert.ToBase64String(chavePrivadaB);
            this.username = username;
            InitializeComponent();

            // Cria uma instância do cliente
            // public ChatClient(string username, string publicKey, string privateKey, string serverip, int serverport)
            client = new ChatClient(username, publicKey, privateKey, "127.0.0.1", 5000);
            client.ConnectServer();
            client.MessageReceived += (sender, message) =>
            {
                // Exibe a mensagem recebida
                rtb_Messages.AppendText($"[{sender}]: {message}\n");
            };
        }


        private void btn_Send_Click(object sender, EventArgs e)
        {

            // Envia uma mensagem
            string targetUsername = tb_MessageInput.Text;
            string message = tb_MessageInput.Text;
            string targetPublicKey = publicKey; // Aqui você deve obter a chave pública do destinatário
            client.SendMessage(targetUsername, message, targetPublicKey);
            //tb_MessageInput.Clear();
            //tb_MessageInput.Focus();
        }

        private void btn_AddFriend_Click(object sender, EventArgs e)
        {

        }
    }
}
