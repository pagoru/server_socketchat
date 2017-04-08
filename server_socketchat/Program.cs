using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server_socketchat
{
    public class Program
    {
        public static Server Server { get; set; } = new Server();

        public static void Main(string[] args)
        {
            Server.Start();
        }
    }
}
