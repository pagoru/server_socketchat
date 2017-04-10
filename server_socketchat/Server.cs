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

                    AddClient(new Client(listener.AcceptTcpClient()));
                }
            }
            catch (Exception e)
            {

            }

        }

        public int ClientCount()
        {
            return _clientList.Count;
        }

        public void AddClient(Client client)
        {
            _clientList.Add(client);
        }

        public void RemoveClient(string username)
        {
            _clientList = _clientList.FindAll(c => c.Username != username);
        }

        public void JoinRoom(string username, string roomname)
        {
            SendToRoom($"{username} ha entrat a la sala.", roomname, username);
        }

        public void LeaveRoom(string username, string roomname)
        {
            SendToRoom($"{username} s'ha anat de la sala.", roomname, username);
        }

        public void SendRoomMessage(string username, string roomname, string message)
        {
            SendToRoom($"{username}: {message}", roomname, null);
        }

        private void SendToRoom(string message, string roomname, string noUsername)
        {
            _clientList.FindAll(c => c.Roomname == roomname && c.Username != noUsername).ForEach(c => c.SendMessage(
                    new SocketAction(SocketActions.ServerSendMessage, message, true)
                )
            );
        }

    }
}