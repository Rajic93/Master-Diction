using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
    public class ClientManager
    {
        private static ClientManager clientManager;

        private ClientManager()
        {
            
        }

        public static ClientManager CreateInstance()
        {
            return clientManager ?? new ClientManager();
        }

        public void Start()
        {
            new Thread(() =>
            {
                while (true)
                {

                }
            }).Start();
        }
    }
}
