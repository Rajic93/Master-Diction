using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Diction_Master___Library
{

    public class DictionMasterServer
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private readonly ClientManager clientManager;
        private static int port;
        private const int BUFFER_SIZE = 2048;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        private static IPAddress serverAddress;

        public DictionMasterServer(IPAddress localIP, int localPort, ClientManager manager)
        {
            clientManager = manager;
            serverAddress = localIP;
            port = localPort;
            SetupServer();
        }

        private static void SetupServer()
        {
            serverSocket.Bind(new IPEndPoint(serverAddress, port));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException)
            // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }
            //add newly created one and continue listening
            clientSockets.Add(socket);
            new Thread(() =>
            {
                socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            }).Start();
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                current.Close();
                clientSockets.Remove(current);
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string command = Encoding.ASCII.GetString(recBuf);
            //check for operation
            if (command.ToLower().Contains("login")) // Client requested time
            {
                current.Send(Encoding.ASCII.GetBytes("OK"));
                byte[] buff = new byte[1024];
                //current.Receive(buff);

            }
            else if (command.ToLower().Contains("register")) // Client requested time
            {

            }
            else if (command.ToLower().Contains("exit")) // Client wants to exit gracefully
            {
                // Always Shutdown before closing
                current.Shutdown(SocketShutdown.Both);
                current.Close();
                clientSockets.Remove(current);
                return;
            }
            else
            {
                byte[] data = Encoding.ASCII.GetBytes("Invalid request");
                current.Send(data);
            }

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }









        private static void Login(byte[] data)
        {
            string username = Encoding.ASCII.GetString(data);
        }

        /// <summary>
        /// Close all connected client (we do not need to shutdown the server socket as its connections
        /// are already closed with the clients).
        /// </summary>
        public void CloseAllSockets()
        {
            foreach (Socket socket in clientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            serverSocket.Close();
        }
    }
}
