using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
    public class ContentManager : ISubject, IMigrate
    {
        private List<Component> courses;
        private ClientManager clientManager;
        private static ContentManager contentManager;

        private ContentManager(ClientManager manager)
        {
            contentManager = new ContentManager();
            contentManager.clientManager = manager;
        }

        private ContentManager()
        {
            courses = new List<Component>();
        }

        public static ContentManager CreateInstance(ClientManager clientManager)
        {
                return contentManager ?? new ContentManager(clientManager);
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

        public void Attach(IObserver observer)
        {
            throw new NotImplementedException();
        }

        public void Detach(IObserver observer)
        {
            throw new NotImplementedException();
        }

        public void Notify()
        {
            throw new NotImplementedException();
        }

        public List<ContentVersionInfo> CheckStatus(List<ContentVersionInfo> contentVersions)
        {
            throw new NotImplementedException();
        }

        public Component GetComponent(int id)
        {
            throw new NotImplementedException();
        }

        public string Export()
        {
            throw new NotImplementedException();
        }

        public void Import(string content)
        {
            throw new NotImplementedException();
        }
    }
}
