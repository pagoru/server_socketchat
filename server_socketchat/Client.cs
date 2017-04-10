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
        private Thread TimeoutThread { get; set; }
        private int SecondsLastMessage { get; set; } = 0;

        public string Username { get; set; } = "Default";
        public string Roomname { get; set; } = "Global";

        public Client(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            NetworkStream networkStream = TcpClient.GetStream();

            StreamWriter = new StreamWriter(networkStream);
            StreamReader = new StreamReader(networkStream);

            ReadThread = new Thread(ReadFrom);
            ReadThread.Start();

            TimeoutThread = new Thread(TimeoutControl);
            TimeoutThread.Start();
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
                    Program.Server.JoinRoom(Username, Roomname);
                    break;
                case SocketActions.ClientChat:
                    if (Roomname != socketAction.Message)
                    {
                        Program.Server.LeaveRoom(Username, Roomname);
                        Roomname = socketAction.Message;
                        Program.Server.JoinRoom(Username, Roomname);
                    }
                    break;
                case SocketActions.ClientGoodbye:
                    Close();
                    break;
                case SocketActions.ClientSendMessage:
                    MessageControl(socketAction.Message);
                    break;
            }
        }

        private async void MessageControl(string message)
        {
            if (message.StartsWith("!"))
            {
                switch (message)
                {
                    case "!quit":
                        Close();
                        break;
                    case "!usuaris":
                        await SendMessage(new SocketAction(SocketActions.CommandUsuaris, $"{Program.Server.ClientCount()}", true));
                        break;
                }
                return;
            }
            Program.Server.SendRoomMessage(Username, Roomname, message);
        }

        public void Close()
        {
            try
            {
                Console.WriteLine($"Goodbye {Username}!");

                TcpClient.Client.Close();
                TcpClient.Close();
                Program.Server.LeaveRoom(Username, Roomname);
                Program.Server.RemoveClient(Username);
                ReadThread.Abort();
            }
            catch (Exception e){}
        }

        private void ReadFrom()
        {
            try
            {
                while (true)
                {
                    ReadCallback(StreamReader.ReadLine());
                    SecondsLastMessage = 0;
                }
            }
            catch (Exception e)
            {

            }
            Close();
        }

        private void TimeoutControl()
        {
            while (true)
            {
                Thread.Sleep(1000);
                SecondsLastMessage++;
                if (SecondsLastMessage == 20)
                {
                    Close();
                    break;
                }
            }
        }
    }
}