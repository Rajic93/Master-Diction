using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
    public class NetworkModuleFactory
    {
        public static DictionMasterClient CreateTcpClient(IPAddress remoteIP, int port)
        {
            return new DictionMasterClient(remoteIP, port);
        }

        public static DictionMasterServer CreateTcpServer(IPAddress localIP, int port, ClientManager manager)
        {
            return new DictionMasterServer(localIP, port, manager);
        }
    }
}
