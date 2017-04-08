using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace server_socketchat
{
    public class Client
    {
        private TcpClient TcpClient { get; }

        private StreamWriter StreamWriter { get; }
        private StreamReader StreamReader { get; }

        private Thread ReadThread { get; set; }

        public string Username { get; set; } = "Default";
        public string Roomname { get; set; } = "Default";

        public Client(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            NetworkStream networkStream = TcpClient.GetStream();

            StreamWriter = new StreamWriter(networkStream);
            StreamReader = new StreamReader(networkStream);

            ReadThread = new Thread(ReadFrom);
            ReadThread.Start();
        }

        public async Task<int> SendMessage(SocketAction message)
        {
            await StreamWriter.WriteLineAsync(message.Serialize());
            await StreamWriter.FlushAsync();
            return 1;
        }

        public void ReadCallback(string message)
        {
            SocketAction socketAction = SocketAction.Deserialize(message);

            switch (socketAction.SocketActions)
            {
                case SocketActions.ClientLogin:
                    Username = socketAction.Message;
                    Console.WriteLine($"Welcome {Username}!");
                    break;
                case SocketActions.ClientChat:
                    break;
                case SocketActions.ClientGoodbye:
                    ReadThread.Abort();
                    Program.Server.RemoveClient(Username);
                    break;
                case SocketActions.ClientSendMessage:
                    Program.Server.SendRoomMessage(Username, Roomname, socketAction.Message);
                    break;
            }
        }

        private void ReadFrom()
        {
            try
            {
                while (true)
                {
                    ReadCallback(StreamReader.ReadLine());
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}