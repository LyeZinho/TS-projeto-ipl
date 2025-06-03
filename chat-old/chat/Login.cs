namespace chat
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btn_ConfirmUsername_Click(object sender, EventArgs e)
        {
            // Inicia o form Chat com o username fornecido
            Chat chat = new Chat(tb_UsernameImput.Text);
            chat.Show();
            this.Hide();
        }
    }
}
