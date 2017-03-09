using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Diction_Master___Library
{
    public class ClientManager : IObserver
    {
        private static object _lock = new object();

        private int _globalIDCounter {get; set;}
        private object _IDLock = new object();

        private List<Client> _clients { get; set; }
        private object _clientsLock = new object();
        private Queue<Client> _unableToContact { get; set; }
        private object _unableToContactLock = new object();

        private List<PendingNotification> _pendingNotifications { get; set; }

        private NetworkOperations _auth;
        private string _IPAddress = "192.168.56.1";
        private int _port;
        private ApplicationType _clientsType;
        private bool _running = true;

        private List<ContentVersionInfo> _changes { get; set; }
        private DateTime _currentVersion { get; set; }

        private object _changesLock = new object();

        private bool _updateInProgress = false;

        private Thread _listeningThread;
        private Thread _retryThread;
        private Thread _updateThread;
        private Thread _checkSubscriptionsThread;

        #region Properties

        public ApplicationType ClientsType
        {
            get
            {
                return _clientsType;
            }

            set
            {
                _clientsType = value;
            }
        }
        public string IPAddress
        {
            get
            {
                return _IPAddress;
            }

            set
            {
                _IPAddress = value;
            }
        }
        public int Port
        {
            get
            {
                return _port;
            }

            set
            {
                _port = value;
            }
        }

        #endregion

        public ClientManager(ApplicationType type)
        {
            _clients = new List<Client>();
            _unableToContact = new Queue<Client>();
            _changes = new List<ContentVersionInfo>();
            _pendingNotifications = new List<PendingNotification>();
            _listeningThread = new Thread(ListenForIncomingClientConnections);
            _checkSubscriptionsThread = new Thread(CheckSubscriptions);
            _clientsType = type;
            LoadConfig();
        }

        #region Config

        private void SaveConfig()
        {

            string file = "";
            switch (_clientsType)
            {
                case ApplicationType.Diction:
                    file = "DctDat.dat";
                    break;
                case ApplicationType.Teachers:
                    file = "TchDat.dat";
                    break;
                case ApplicationType.Audio:
                    file = "AuoDat.dat";
                    break;
                default:
                    break;
            }
            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, _clients);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);

            }
            finally
            {
                fs.Close();
            }

            //XmlSerializer serializer = new XmlSerializer(_clients.GetType());
            //using (var writer = new StreamWriter("Clients.xml"))
            //using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
            //{
            //    serializer.Serialize(xmlWriter, _clients);
            //}
            //serializer = new XmlSerializer(Topics.GetType());
            //using (var writer = new StreamWriter("TeachersAppContentManifest.xml"))
            //using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
            //{
            //    serializer.Serialize(xmlWriter, Topics);
            //}
            //serializer = new XmlSerializer(GetType());
            //using (var writer = new StreamWriter("ContentManifest.xml"))
            //using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
            //{
            //    serializer.Serialize(xmlWriter, this);
            //}
        }

        private void LoadConfig()
        {
            string file = "";
            switch (_clientsType)
            {
                case ApplicationType.Diction:
                    file = "DctDat.dat";
                    break;
                case ApplicationType.Teachers:
                    file = "TchDat.dat";
                    break;
                case ApplicationType.Audio:
                    file = "AuoDat.dat";
                    break;
                default:
                    break;
            }
            if (File.Exists(file))
            {
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    // Deserialize the hashtable from the file and 
                    // assign the reference to the local variable.
                    _clients = (List<Client>)formatter.Deserialize(fs);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + e.Message);

                }
                finally
                {
                    fs.Close();
                }
            }

            //XmlSerializer serializer = new XmlSerializer(_clients.GetType());
            //if (File.Exists("Clients.xml"))
            //    _clients = serializer.Deserialize(new XmlTextReader("Clients.xml")) as List<Client>;
            //if (File.Exists("TeachersAppContentManifest.xml"))
            //    Topics = serializer.Deserialize(new XmlTextReader("TeachersAppContentManifest.xml")) as List<Component>;
        } 

        #endregion

        public void Start()
        {
            //start thread to try to update clients that are not updated
            _retryThread = new Thread(() =>
            {
                RetryToContactClients();
            });
            _retryThread.Start();
            _listeningThread.Start();
            _checkSubscriptionsThread.Start();
        }

        public void Stop()
        {
            _running = false;
            _auth.Terminate();
            if (_listeningThread != null)
            {
                _listeningThread.Abort();
            }
            if (_retryThread != null)
            {
                _retryThread.Abort();
            }
            if (_updateThread != null)
            {
                _updateThread.Abort();
            }
            SaveConfig();
        }

        public void Update(List<ContentVersionInfo> changes, DateTime currentVersion)
        {
            _currentVersion = currentVersion;
            _updateThread = new Thread(() =>
            {
                _updateInProgress = true;
                CompareAndMergeChangesLists(changes);
                lock (_unableToContactLock)
                {
                    _unableToContact.Clear();
                }
                foreach (Client client in _clients)
                {
                    NotifyUser(client, _currentVersion);
                }
                _updateInProgress = false;
            });
            _updateThread.Start();
        }

        private void CompareAndMergeChangesLists(List<ContentVersionInfo> changes)
        {
            lock (_changesLock)
            {
                foreach (ContentVersionInfo change in changes)
                {
                    if (_changes.Exists(x => x.ComponentID == change.ComponentID && x.ParentID == change.ParentID))
                    {
                        ContentVersionInfo info = _changes.Find(x => x.ComponentID == change.ComponentID && x.ParentID == change.ParentID);
                        info.ParentID = change.ParentID;
                        info.Status = change.Status;
                        info.Component = change.Component;
                    }
                    else
                        _changes.Add(change);
                }
                _currentVersion = DateTime.Now;
            }
        }

        private void RetryToContactClients()
         {
            int sleep = 30000;
            while (_running)
            {
                if (!_updateInProgress)
                {
                    sleep = 60000;
                    int number;
                    lock (_unableToContactLock)
                    {
                        number = _unableToContact.Count;
                    }
                    if (number != 0)
                    {
                        int loopCounter = 0;
                        while (loopCounter < number && !_updateInProgress)
                        {
                            Client client;
                            bool update = true;
                            lock (_unableToContactLock)
                            {
                                client = _unableToContact.Dequeue();
                                if (client.LastUpdate >= _currentVersion)
                                {
                                    _unableToContact.Enqueue(client);
                                    update = false;
                                }
                            }
                            if (update && !_updateInProgress)
                            {
                                try
                                {
                                    //try to contact and if it is successfull remove it from queue
                                    NotifyUser(client, _currentVersion);
                                }
                                catch (Exception)
                                {
                                    //failed to update or contact
                                    lock (_unableToContactLock)
                                    {
                                        _unableToContact.Enqueue(client);
                                    }
                                } 
                            }
                            loopCounter++;
                        }
                    }
                }
                else
                    sleep *= 2;
                Thread.Sleep(sleep);
            }
        }

        public DateTime GetCurrentVersionDate()
        {
            return _currentVersion;
        }

        public int GetUpToDateClients()
        {
            lock (_clientsLock)
            {
               return _clients.Count;
            }
        }

        public int GetOutDatedClients()
        {
            int total;
            lock (_clientsLock)
            {
                total = _clients.Count;
            }
            int outDated;
            lock (_unableToContactLock)
            {
                outDated = _unableToContact.Count;
            }
            return total - outDated;
        }

        private int GetId()
        {
            lock (_IDLock)
            {
                return ++_globalIDCounter;
            }
        }

        public bool IsRunning()
        {
            return _running;
        }

        private void CheckSubscriptions()
        {
            foreach (Client client in _clients)
            {
                CheckSubscription(client);
            }
        }

        private void ListenForIncomingClientConnections()
        {
            //manage exceptions
            try
            {
                _auth = new NetworkOperations(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
                _auth.Listen(_IPAddress, _port, this);
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

        #region Users

        public bool CheckForUser(string username)
        {
            return _clients.Exists(x => x.Username == username);
        }

        public long CreateNewUser(string username, byte[] password, byte[] salt, IPAddress address, int port)
        {
            long id = GetId();
            lock (_clientsLock)
            {
                _clients.Add(new Client()
                {
                    ID = id,
                    Username = username,
                    Password = password,
                    Salt = salt,
                    RegistrationDate = DateTime.Now,
                    LastUpdate = DateTime.Now,
                    ActiveSubscriptions = new List<Subscription>(),
                    ExpiredSubscription = new List<Subscription>(),
                    IpAddress = address,
                    Port = port
                });
            }
            return id;
        }

        public void EditUser(long id, string username, byte[] password, byte[] salt)
        {
            lock (_clientsLock)
            {
                if (_clients.Exists(x => x.ID == id))
                {
                    Client client = _clients.Find(x => x.ID == id);
                    client.Username = username;
                    client.Password = password;
                    client.Salt = salt;
                }
            }
        }

        public void DeleteUser(long id)
        {
            lock (_clientsLock)
            {
                if (_clients.Exists(x => x.ID == id))
                {
                    _clients.RemoveAll(x => x.ID == id);
                }
            }
        }

        public void NotifyUser(Client client, DateTime currentVersion)
        {
            //connect to client, notify about updates and send if confirmed
            //if not confirmed put it 
            if (client.LastUpdate < currentVersion)
            {
                try
                {
                    //try to connect to client
                    NetworkOperations auth = new NetworkOperations(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
                    auth.NotifyServerSide(new PendingNotification
                    {
                        ClientID = client.ID,
                        IPAddress = client.IpAddress.ToString(),
                        Port = client.Port,
                        NotificationType = NotificationType.UpdateAvailable,
                        UpdateVersion = currentVersion
                    }, client.IpAddress.ToString(), 50000);
                }
                catch (Exception ex)
                {
                    //failed to connect to client add to 
                    lock (_unableToContactLock)
                    {
                        _unableToContact.Enqueue(client);
                    }
                }
            }
        }

        #endregion

        #region Subscription

        public bool Subscribe()
        {
            return true;
        }

        public KeyValuePair<long, DateTime> CreateNewSubscription(long clientID, long courseID, long eduLevelID, long gradeID, long termID, string key)
        {
            lock (_clientsLock)
            {
                long id = GetId();
                DateTime now = DateTime.Now;
                KeyValidation validation = new Keys().CheckKey(key);
                if (validation != KeyValidation.Invalid)
                {
                    SubscriptionType type = (validation == KeyValidation.ValidFullYear) ? SubscriptionType.Year : SubscriptionType.Term;
                    if (_clients.Exists(x => x.ID == clientID))
                    {
                        _clients.Find(x => x.ID == clientID).ActiveSubscriptions.Add(new Subscription
                        {
                            ID = id,
                            ClientID = clientID,
                            CourseID = courseID,
                            EduLevelID = eduLevelID,
                            GradeID = gradeID,
                            TermID = termID,
                            SubscriptionType = type,
                            ExpirationDateTime = now,
                            Key = key
                        });
                        return new KeyValuePair<long, DateTime>(id, now);
                    }
                }
                return new KeyValuePair<long, DateTime>();
            }
        }

        public void EditSubscription(long id, long clientID, long courseID, long eduLevelID, long gradeID, long termID,
            SubscriptionType type, DateTime expDate, string key)
        {
            lock (_clientsLock)
            {
                if (_clients.Exists(x => x.ID == clientID))
                {
                    Client client = _clients.Find(x => x.ID == clientID);
                    if (client.ActiveSubscriptions.Exists(x => x.ID == id))
                    {
                        Subscription sub = client.ActiveSubscriptions.Find(s => s.ID == id);
                        sub.ID = id;
                        sub.ClientID = clientID;
                        sub.CourseID = courseID;
                        sub.EduLevelID = eduLevelID;
                        sub.GradeID = gradeID;
                        sub.TermID = termID;
                        sub.SubscriptionType = type;
                        sub.ExpirationDateTime = expDate;
                        sub.Key = key; 
                    }
                }
            }
        }

        public void DeleteSubscription(long id, long clientID)
        {
            lock (_clientsLock)
            {
                if (_clients.Exists(x => x.ID == clientID))
                {
                    Client client = _clients.Find(x => x.ID == clientID);
                    if (client.ActiveSubscriptions.Exists(x => x.ID == id))
                    {
                        client.ActiveSubscriptions.RemoveAll(x => x.ID == id);
                    }
                }
            }
        }

        public void CheckSubscription(Client client)
        {
            List<Subscription> subs = new List<Subscription>(client.ActiveSubscriptions);
            foreach (Subscription sub in subs)
            {
                if (sub.ExpirationDateTime >= DateTime.Now)
                {
                    client.ActiveSubscriptions.Remove(sub);
                    client.ExpiredSubscription.Add(sub);
                    PendingNotification notification = new PendingNotification()
                    {
                        ClientID = client.ID,
                        IPAddress = client.IpAddress.ToString(),
                        Port = client.Port,
                        NotificationType = NotificationType.SubscriptionExpired,
                        SubscriptionID = sub.ID
                    };
                    //send notification
                    try
                    {
                        NetworkOperations auth = new NetworkOperations(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
                        auth.NotifyServerSide(notification, client.IpAddress.ToString(), 50000);
                    }
                    catch (Exception)
                    {
                        _pendingNotifications.Add(notification);
                    }
                }
            }
        }

        #endregion

        public byte[] GetSalt(string username)
        {
            return _clients.Exists(x => x.Username == username)
                ? _clients.Find(x => x.Username == username).Salt
                : null;
        }

        public byte[] GetHashedPassword(string username)
        {
            return _clients.Exists(x => x.Username == username)
                ? _clients.Find(x => x.Username == username).Password
                : null;
        }

        public List<ContentVersionInfo> GetChanges()
        {
            return new List<ContentVersionInfo>(_changes);
        }

        public string GetUsernameSuggestions(string username)
        {
            return "";
        }
    }
}
