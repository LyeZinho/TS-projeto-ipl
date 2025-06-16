
using chatlib;
using System;
using System.Windows.Forms;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace chatapp
{
    public partial class Login : Form
    {


        /*
         Login endpoint: https://localhost:7016/User/login
         {
          "username": "",
          "attemptUsername": "username_tentativa",
          "attemptPassword": "hash_da_senha_aqui",
          "sessionId": ""
         }
         */

        // Metodo para fazer o login do usuário
        public ValidationSession MakeLogin(string attemptUsername, string attemptPassword)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7016/");
                var loginData = new
                {
                    username = attemptUsername,
                    attemptUsername = attemptUsername,
                    attemptPassword = attemptPassword,
                    sessionId = Guid.NewGuid().ToString() // Gera um novo SessionId
                };
                var jsonContent = JsonSerializer.Serialize(loginData);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = client.PostAsync("User/login", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    using JsonDocument doc = JsonDocument.Parse(responseData);
                    JsonElement root = doc.RootElement;

                    ValidationSession session = new ValidationSession
                    {
                        SessionId = root.GetProperty("sessionId").GetString(),
                        Username = root.GetProperty("username").GetString(),
                        UniqueId = root.GetProperty("uniqueId").GetString(),
                        PublicKey = root.GetProperty("publicKey").GetString(),
                        AttemptUsername = attemptUsername,
                        AttemptPassword = attemptPassword
                    };

                    return session;
                }
                else
                {
                    MessageBox.Show("Erro ao fazer login: " + response.ReasonPhrase, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }

        public User GetSessionUser(ValidationSession session)
        {
            // https://localhost:7016/User/GetUser?username=exemplo_usuario&sessionid=42ea3aae-9fbf-42d2-88ba-b616afbf51bc
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7016/");
                var response = client.GetAsync($"User/GetUser?username={session.Username}&sessionid={session.SessionId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    /*
                     {
                    "username":"exemplo_usuario",
                    "publicKey":"chave_publica_em_texto_base64",
                    "uniqueId":"7881fb52-e7d7-4566-907a-789893f99915",
                    "privateKey":"<privateinfo>",
                    "publicKeyEncrypted":"<privateinfo>",
                    "privateKeyEncrypted":"<privateinfo>",
                    "salt":"<privateinfo>",
                    "passwordHash":"<privateinfo>"}
                     */

                    using JsonDocument doc = JsonDocument.Parse(responseData);
                    JsonElement root = doc.RootElement;

                    User user = new User()
                        .SetUsername(root.GetProperty("username").GetString())
                        .SetPublicKey(root.GetProperty("publicKey").GetString())
                        .SetUniqueId(root.GetProperty("uniqueId").GetString())
                        .SetPrivateKey(root.GetProperty("privateKey").GetString()) // Pode ser <privateinfo> se não for necessário
                        .SetPublicKeyEncrypted(root.GetProperty("publicKeyEncrypted").GetString()) // Pode ser <privateinfo> se não for necessário
                        .SetPrivateKeyEncrypted(root.GetProperty("privateKeyEncrypted").GetString()) // Pode ser <privateinfo> se não for necessário
                        .SetSalt(root.GetProperty("salt").GetString()) // Pode ser <privateinfo> se não for necessário
                        .SetPasswordHash(root.GetProperty("passwordHash").GetString()); // Pode ser <privateinfo> se não for necessário
                    return user;
                }
                else
                {
                    MessageBox.Show("Erro ao obter usuário: " + response.ReasonPhrase, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }


        public Login()
        {
            InitializeComponent();
        }

        //private void Connect_Click(object sender, EventArgs e)
        //{
        //    // Abre o form Chat
        //    string username = tb_Connect.Text.Trim();
        //    if (!string.IsNullOrEmpty(username))
        //    {
        //        Chat chatForm = new Chat();
        //        chatForm.Show();
        //        this.Hide(); // Esconde o formulário atual
        //    }
        //    else
        //    {
        //        MessageBox.Show("Por favor, insira um nome de usuário válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}


        private void btn_Register_Click(object sender, EventArgs e)
        {
            // Abre o form Register
            Register registerForm = new Register();
            registerForm.Show();
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = tb_Connect.Text.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Faz o login do usuário
            var session = MakeLogin(username, password);
            if (session != null)
            {
                // Busca o usuário na sessão
                var user = GetSessionUser(session);
                if (user != null)
                {
                    // Abre o form Chat com o usuário
                    Chat chatForm = new Chat(user);
                    chatForm.Show();
                    this.Hide(); // Esconde o formulário atual
                }
                else
                {
                    MessageBox.Show("Usuário não encontrado na sessão.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Login falhou. Verifique suas credenciais.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
