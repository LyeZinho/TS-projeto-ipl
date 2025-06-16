using chatapp.data;

namespace chatapp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DbSetup dbSetup = new DbSetup(DbSetup.CreateConnection()); // Cria a conexão com o banco de dados e inicializa as tabelas
            // Initialize the database connection and setup

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Login());
        }
    }
}