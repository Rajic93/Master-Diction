﻿using Diction_Master___Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diction_Master
{
    [Serializable]
    public class ClientState
    {
        public bool _firstStart { get; set; }
        public bool _contentEnabled { get; set; }
        public bool LocalRegistration { get; set; }
        public bool _registered { get; set; }
        public bool _subscriptionCreated { get; set; }
        public bool _loggedIn { get; set; }
        public string _pass { get; set; }
        public Client clientProfile { get; set; }
        public List<Subscription> _pendingSubscriptions { get; set; }

        public string _serverIPAdd = "192.168.56.1";
        public int _port = 30012;

        public ClientState()
        {
            LocalRegistration = false;
            _firstStart = true;
            _contentEnabled = false;
            _registered = false;
            _subscriptionCreated = false;
            _loggedIn = false;
            clientProfile = new Client();
            _pendingSubscriptions = new List<Subscription>();
        }
    }
}
