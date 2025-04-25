// Program.cs
using System;
using chatlib.server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph.Models.Security;

namespace chatserver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Cria e inicia o servidor
            var chatServer = new ChatServer(5000);
            Console.WriteLine("Servidor iniciado na porta 5000.");
            chatServer.Start();
            Console.ReadKey();
            chatServer.Stop();
        }
    }
}
