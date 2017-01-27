using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
    public class Client
    {
        public long ID { get; set; }
        public string Username { get; set; }

        private byte[] _password = new byte[64];
        public byte[] Password
        {
            get { return _password; }
            set { value.CopyTo(_password, 0);}
        }

        private byte[] _salt = new byte[16];
        public byte[] Salt
        {
            get { return _salt; }
            set { value.CopyTo(_salt, 0); }
        }

        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
        public ContentStatus ContentStatus { get; set; }
        public NetworkAvailability NetworkAvailability { get; set; }
        public ContentVersionInfo ContentVersionInfo { get; set; }
    }
}
