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
            var server = new ChatServer("127.0.0.1", 3000, false);
            server.Start();

            Console.WriteLine("Servidor iniciado. insira shutdown para parar o servidor.");
            string? command = Console.ReadLine();
            while (command != "shutdown")
            {
                Console.WriteLine(">");
                command = Console.ReadLine();
            }
            server.Stop();
            Console.WriteLine("Servidor parado.");
        }
    }
}
