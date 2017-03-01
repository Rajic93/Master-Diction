using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
    public class SubscriptionManager
    {
        private static SubscriptionManager _manager;
        private object _lock = new object();

        public List<Subscription> ActiveSubscriptions { get; set; }
        public List<Subscription> InactiveSubscriptions { get; set; }

        private SubscriptionManager()
        {
            
        }

        public SubscriptionManager CreateInstance()
        {
            if (_manager == null)
                lock (_lock)
                    return _manager ?? new SubscriptionManager();
            return _manager;
        }


    }
}
