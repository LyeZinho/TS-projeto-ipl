using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace chatlib.objects
{
    /*
     Esta classe é a base para a classe User.
     */
    public class User
    {
        public string? Username { get; set; }
        public int Id { get; set; }

        public string? PublicKey { get; set; }
        public string? PrivateKey { get; set; }
    }
}
