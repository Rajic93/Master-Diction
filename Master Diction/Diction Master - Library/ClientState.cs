using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diction_Master___Library
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
        public List<Component> _availableCourses;
        public List<Component> _enabledCourses;
        public List<Component> _enabledEduLevels;
        public List<Component> _enabledGrades;
        public List<KeyValuePair<long, int>> _enabledTerms;
        public List<PendingNotification> _notifications;
        public bool _pendingNotification;

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
            _notifications = new List<PendingNotification>();
            _availableCourses = new List<Component>();
            _enabledCourses = new List<Component>();
            _enabledEduLevels = new List<Component>();
            _enabledGrades = new List<Component>();
            _enabledTerms = new List<KeyValuePair<long, int>>();
        }

        public void SetNotification(PendingNotification notification)
        {
            _pendingNotification = true;
            _notifications.Add(notification);
        }
    }
}
