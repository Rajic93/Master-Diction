using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Diction_Master___Library
{

    public class DictionMasterClient
    {
        private static readonly Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static IPAddress remoteIPAdd;
        private static int port;

        public DictionMasterClient(IPAddress remoteIP, int remotePort)
        {
            remoteIPAdd = remoteIP;
            port = remotePort;
            ConnectToServer();
            RequestLoop();
            Exit();
        }

        private static void ConnectToServer()
        {
            int attempts = 0;

            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    // Change IPAddress.Loopback to a remote IP to connect to a remote host.
                    ClientSocket.Connect(remoteIPAdd, port);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }
        }

        private static void RequestLoop()
        {
            while (true)
            {
                SendRequest();
                ReceiveResponse();
            }
        }

        /// <summary>
        /// Close socket and exit program.
        /// </summary>
        private static void Exit()
        {
            SendString("exit"); // Tell the server we are exiting
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            //Environment.Exit(0);
        }

        //public bool 


        private static void SendRequest()
        {
            SendString("Login");

            if ("Login".ToLower() == "exit")
            {
                Exit();
            }
        }

        /// <summary>
        /// Sends a string to the server with ASCII encoding.
        /// </summary>
        private static void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        private static void ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            //Console.WriteLine(text);
        }
    }
}
