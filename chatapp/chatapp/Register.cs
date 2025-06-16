using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Text.Json;
using System.Configuration;
using chatlib;
using chatapp.data;

namespace chatapp
{
    public partial class Register : Form
    {

        DbSetup dbSetup;
        /*
         https://localhost:7016/User/register
        Post data:
        {
          "username": "exemplo_usuario",
          "passwordHash": "hash_da_senha_aqui",
          "privateKey": "chave_privada_em_texto_base64",
          "publicKey": "chave_publica_em_texto_base64",
          "publicKeyEncrypted": "chave_publica_criptografada",
          "privateKeyEncrypted": "chave_privada_criptografada",
          "salt": "sal_criptografico",
          "uniqueId": "id_unico_do_usuario"
        }

        Return:
        {
          "sessionId": "f9356d6f-5d08-41f9-a0c2-41198e873d14",
          "username": "exemplo_usuario",
          "uniqueId": "7881fb52-e7d7-4566-907a-789893f99915",
          "publicKey": "chave_publica_em_texto_base64"
        }
        */
        public ValidationSession MakeRegister()
        {
            DbSetup dbSetup = new DbSetup();


            var (publicKey, privateKey) = Encript.GenerateKeys();
            string salt = Encript.GenerateSalt();
            string passwordHash = Encript.HashPassword(this.tb_Password.Text, salt);
            string uid = Guid.NewGuid().ToString();
            string saltkey = System.Configuration.ConfigurationManager.AppSettings["SaltKey"];
            string publicKeyEncrypted = Encript.EncryptDataFixedIV(publicKey, saltkey);
            string privateKeyEncrypted = Encript.EncryptDataFixedIV(privateKey, saltkey);

            dbSetup.CreateUser(
                DbSetup.CreateConnection(),
                this.tb_Username.Text,
                passwordHash,
                privateKey,
                publicKey,
                publicKeyEncrypted,
                privateKeyEncrypted,
                salt,
                uid
            );

            //saltEncript = Encript.EncryptDataFixedIV(salt, saltkey); // Depois de encriptar a senha, encripta o salt também
            var userData = new
            {
                username = this.tb_Username.Text,
                passwordHash = passwordHash,
                privateKey = privateKey,
                publicKey = publicKey,
                publicKeyEncrypted = publicKeyEncrypted,
                privateKeyEncrypted = privateKeyEncrypted,
                salt,
                uniqueId = uid
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7016/");
                var jsonContent = JsonSerializer.Serialize(userData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = client.PostAsync("User/register", content).Result;
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
                        AttemptUsername = this.lb_Username.Text,
                        AttemptPassword = this.lb_Password.Text
                    };
                    return session;
                }
                else
                {
                    MessageBox.Show("Erro ao registrar usuário: " + response.ReasonPhrase, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }


        public Register()
        {
            InitializeComponent();
        }

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            string username = this.tb_Username.Text.Trim();
            string password = this.tb_Password.Text.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var session = MakeRegister();
            if (session != null)
            {
                MessageBox.Show($"Usuário {session.Username} registrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Fecha o formulário de registro
            }
            else
            {
                MessageBox.Show("Falha ao registrar usuário.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
