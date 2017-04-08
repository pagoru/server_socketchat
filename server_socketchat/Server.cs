using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace server_socketchat
{
    public class Server
    {
        public int Port { get; set; } = 55555;

        private List<Client> _clientList = new List<Client>();

        public void Start()
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, Port);
                listener.Start();
                Console.WriteLine($"Server started @ {IPAddress.Any}:{Port}");

                while (true)
                {
                    Thread.Sleep(100);
                    Console.WriteLine("Waiting for new clients...");

                    _clientList.Add(new Client(listener.AcceptTcpClient()));
                }
            }
            catch (Exception e)
            {

            }

        }

        public void RemoveClient(string username)
        {
            _clientList = _clientList.FindAll(c => c.Username != username);
        }

        public void SendRoomMessage(string username, string roomname, string message)
        {
            //.FindAll(c => c.Username != username)
            _clientList.FindAll(c => c.Roomname == roomname).ForEach(c => c.SendMessage(
                    new SocketAction(SocketActions.ServerSendMessage, $"{username}: {message}", true)
                )
            );
        }

    }
}