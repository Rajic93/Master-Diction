using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
    [Serializable]
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

        private IPAddress _ipAddress;
        public IPAddress IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }
        public int Port { get; set; }
        public ContentStatus ContentStatus { get; set; }
        public NetworkAvailability NetworkAvailability { get; set; }
        public ContentVersionInfo ContentVersionInfo { get; set; }

        private DateTime _lastUpdate;
        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value; }
        }

        private DateTime _registrationDate;
        public DateTime RegistrationDate
        {
            get { return _registrationDate; }
            set { _registrationDate = value; }
        }

        private List<Subscription> _activeSubscriptions;
        public List<Subscription> ActiveSubscriptions
        {
            get { return _activeSubscriptions; }
            set { _activeSubscriptions = value; }
        }

        private List<Subscription> _expiredSubscription;
        public List<Subscription> ExpiredSubscription
        {
            get { return _expiredSubscription; }
            set { _expiredSubscription = value; }
        }

        public Client()
        {
            _expiredSubscription = new List<Subscription>();
            _activeSubscriptions = new List<Subscription>();
        }
    }
}
