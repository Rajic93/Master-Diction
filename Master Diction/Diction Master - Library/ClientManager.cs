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
    public class ClientManager : IObserver
    {
        private static ClientManager clientManager;
        private static int _globalIDCounter = 0;

        private List<Client> _clients { get; set; }

        private bool _running;

        private ClientManager()
        {
            _clients = new List<Client>();
            _running = true;
        }

        ~ClientManager()
        {
            _running = false;
        }

        public static ClientManager CreateInstance()
        {
            if (clientManager == null)
            {
                clientManager = new ClientManager();
            }
            return clientManager;
        }

        public Thread Start()
        {
            Thread thread = new Thread(ListenForIncomingClientConnections);
            thread.Start();
            return thread;
        }

        public void Update(List<ContentVersionInfo> changes)
        {

        }

        public int GetID()
        {
            return ++_globalIDCounter;
        }

        public void ListenForIncomingClientConnections()
        {
            try
            {
                Authentication<Client> auth = new Authentication<Client>(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
                auth.Listen("192.168.1.142", 30000, CheckForUser, CreateNewUser, Subscribe);
            }
            catch (ArgumentNullException ae)
            {
                Console.WriteLine("ArgumentNullException : {0}", ae);
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e);
            }
        }

        private bool CheckForUser(string username)
        {
            return _clients.Exists(x => x.Username == username);
        }

        private void CreateNewUser(string username, byte[] password, byte[] salt)
        {
            int id = GetID();
            _clients.Add(new Client()
            {
                ID = id,
                Username = username,
                Password = password,
                Salt = salt
            });
        }

        private object Subscribe(object obj)
        {
            return obj;
        }
    }
}
