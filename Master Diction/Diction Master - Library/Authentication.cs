using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
    public class Authentication
    {
        public delegate bool Subscribe(Socket socket, string username);

        public delegate void HandleNotification(DateTime currentVersion);


        private Socket _socket;
        private AddressFamily _addressFamily;
        private SocketType _socketType;
        private ProtocolType _protocolType;
        private IPAddress _ipAddress;
        private IPEndPoint _ipEndPoint;

        private bool _running = true;

        private ClientState _state;

        public Authentication(SocketType socketType, AddressFamily family, ProtocolType protocolType)
        {
            _addressFamily = family;
            _socketType = socketType;
            _protocolType = protocolType;
        }

        public void SetNotificationHandler(ClientState state)
        {
            _state = state;
        }

        public int Connect(string ipAdd, int port)
        {
            try
            {
                _addressFamily = AddressFamily.InterNetwork;
                _ipAddress = IPAddress.Parse(ipAdd);
                _ipEndPoint = new IPEndPoint(_ipAddress, port);
                _socket = new Socket(_addressFamily, _socketType, _protocolType);
                _socket.Connect(_ipEndPoint);
                return 0;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public int Listen(string ipAdd, int port, ClientManager clientManager)
        {
            try
            {
                _ipAddress = IPAddress.Parse(ipAdd);
                _ipEndPoint = new IPEndPoint(_ipAddress, port);
                _socket = new Socket(_addressFamily, _socketType, _protocolType);
                _socket.Bind(_ipEndPoint);
                while (_running)
                {
                    _socket.Listen(30000);
                    Socket socket = _socket.Accept();
                    
                    Thread thread = new Thread(() =>
                    {
                        ConnectionAccepted(ref socket, clientManager);
                    });
                    thread.Start();
                }
                return 0;
            }
            catch (SocketException se)
            {
                return -1;
            }
            catch (ObjectDisposedException ode)
            {
                return -1;
            }
            catch (InvalidOperationException ioe)
            {
                return -1;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        private void ConnectionAccepted(ref Socket socket, ClientManager clientManager)
        {
            try
            {
                string command = "";
                do
                {
                    byte[] len = new byte[4];
                    socket.Receive(len);
                    byte[] buffer = new byte[BitConverter.ToInt32(len, 0)];
                    socket.Receive(buffer);
                    command = Encoding.Unicode.GetString(buffer);
                    if (command.Contains("LOGIN"))
                        LoginServerSide(ref socket, clientManager);
                    else if (command.Contains("REGISTER"))
                        RegisterServerSide(socket, clientManager);
                    else if (command.Contains("SUBSCRIBE"))
                        SubscribeServerSide(socket, clientManager);
                    else if (command.Contains("NOTIFICATION"))
                        NotifyClientSide(socket);
                    else if (command.Contains("CHKUPDATES"))
                        CheckUpdatesServerSide(socket, clientManager);
                    else if (command.Contains("UPDATE"))
                        UpdateServerSide(socket, clientManager); 
                } while (!command.Contains("TERMINATE"));
                Close(socket);
            }
            catch (ArgumentNullException argNullEx)
            {

            }
            catch (SocketException sockEx)
            {
                
            }
            catch (ObjectDisposedException objDispEx)
            {
                
            }
            catch (SecurityException secEx)
            {
                
            }
            catch (Exception ex)
            {
                
            }
        }

        private void Close(Socket socket)
        {
            socket.Close();
        }

        public void Terminate()
        {
            _running = false;
            _socket.Close();
        }

        #region Login

        public bool LoginClientSide(string username, string password)
        {
            if (_socket == null)
                return false;
            //send action
            byte[] login = Encoding.Unicode.GetBytes("LOGIN");
            _socket.Send(BitConverter.GetBytes(login.Length));
            _socket.Send(login);
            byte[] ok = new byte[4];
            _socket.Receive(ok);
            if (Encoding.Unicode.GetString(ok) == "OK")
            {
                //send username
                byte[] usernameBytes = Encoding.Unicode.GetBytes(username);
                _socket.Send(usernameBytes);
                _socket.Receive(ok); //OK: salt_value
                if (Encoding.Unicode.GetString(ok) == "OK")
                {
                    //get salt
                    byte[] saltBytes = new byte[16];
                    _socket.Receive(saltBytes);
                    //send hashed password
                    byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
                    byte[] hashedPassword = CreateHashedPassword(passwordBytes, saltBytes);
                    _socket.Send(hashedPassword);
                    _socket.Receive(ok);
                    if (Encoding.Unicode.GetString(ok) == "OK")
                        return true;
                }
            }
            return false;
        }

        private void LoginServerSide(ref Socket socket, ClientManager clientManager)
        {
            byte[] ok = Encoding.Unicode.GetBytes("OK");
            byte[] err = Encoding.Unicode.GetBytes("ER");
            //response to action
            socket.Send(ok);
            //receive username
            byte[] buffer = new byte[50];
            int received = socket.Receive(buffer);
            string username = Encoding.Unicode.GetString(buffer).Substring(0, received / 2);
            //check for existing user
            if (clientManager.CheckForUser(username))
            {
                //user exists send OK: salt_value
                byte[] salt = clientManager.GetSalt(username);
                socket.Send(Encoding.Unicode.GetBytes("OK"));
                socket.Send(salt);
                //receive hashed password
                buffer = new byte[64];
                socket.Receive(buffer);
                byte[] hashedPassword = clientManager.GetHashedPassword(username);
                if (buffer.SequenceEqual(hashedPassword))
                {
                    //password is ok
                    socket.Send(ok);
                }
                else
                    socket.Send(err);
            }
            else
                //user does not exist
                socket.Send(err);
        }

        #endregion

        #region Register

        public KeyValuePair<byte[], long> RegisterClientSide(string username, string password, byte[] saltBytes)
        {
            if (_socket == null)
                throw new Exception("Socket not initialized");
            byte[] register = Encoding.Unicode.GetBytes("REGISTER");
            _socket.Send(BitConverter.GetBytes(register.Length));
            _socket.Send(register);
            byte[] ok = new byte[4];
            _socket.Receive(ok);
            string tmp = Encoding.Unicode.GetString(ok);
            if (tmp == "OK")
            {
                //send username
                byte[] usernameBytes = Encoding.Unicode.GetBytes(username);
                _socket.Send(usernameBytes);
                ok = new byte[4];
                _socket.Receive(ok);
                if (Encoding.Unicode.GetString(ok) == "OK")
                {
                    //generate and send salt
                    byte[] salt;
                    if (saltBytes == null)
                        salt = GenerateSalt();
                    else
                        salt = saltBytes;
                    _socket.Send(salt);
                    _socket.Receive(ok);
                    if (Encoding.Unicode.GetString(ok) == "OK")
                    {
                        //send hashed password
                        byte[] passwordBytes = CreateHashedPassword(Encoding.Unicode.GetBytes(password), salt);
                        _socket.Send(passwordBytes);
                        _socket.Receive(ok);
                        if (Encoding.Unicode.GetString(ok).Split(':').First() == "OK")
                        {
                            byte[] idBytes = new byte[8];
                            _socket.Receive(idBytes);
                            long id = BitConverter.ToInt64(idBytes, 0);
                            //created new user
                            return new KeyValuePair<byte[], long>(salt, id);
                        }
                        //server error
                    }
                    //server error
                }
                //username exists
            }
            return new KeyValuePair<byte[], long>();
        }

        private void RegisterServerSide(Socket socket, ClientManager clientManager)
        {
            byte[] ok = Encoding.Unicode.GetBytes("OK");
            byte[] err = Encoding.Unicode.GetBytes("ER");
            //response to action
            socket.Send(ok);
            //receive username
            byte[] buffer = new byte[50];
            socket.Receive(buffer);
            string username = Encoding.Unicode.GetString(buffer).TrimEnd('\0');
            //check for existing user
            if (!clientManager.CheckForUser(username))
            {
                //user does not exist send ok
                socket.Send(ok);
                //receive salt
                buffer = new byte[16];
                socket.Receive(buffer);
                //save salt
                byte[] salt = new byte[16];
                buffer.CopyTo(salt, 0);
                //send ok
                socket.Send(ok);
                //receive hashed password
                buffer = new byte[64];
                socket.Receive(buffer);
                //save hashed password
                byte[] pass = new byte[64];
                buffer.CopyTo(pass, 0);
                //send ok: id
                IPEndPoint endPoint = socket.RemoteEndPoint as IPEndPoint;
                long id = clientManager.CreateNewUser(username, pass, salt, endPoint.Address, endPoint.Port);
                socket.Send(Encoding.Unicode.GetBytes("OK"));
                socket.Send(BitConverter.GetBytes(id));
            }
            else
                socket.Send(err);
        }

        #endregion

        #region Subscribe

        public KeyValuePair<long, DateTime> SubscribeClientSide(string username, string password, byte[] salt, string key,
            long clientID, long courseID, long eduLevelID, long gradeID, long termID)
        {
            if (_socket == null)
                throw new Exception("Socket not initialized");
            byte[] subs = Encoding.Unicode.GetBytes("SUBSCRIBE");
            _socket.Send(BitConverter.GetBytes(subs.Length));
            _socket.Send(subs);
            byte[] ok = new byte[4];
            _socket.Receive(ok);
            string tmp = Encoding.Unicode.GetString(ok);
            if (tmp == "OK")
            {
                //send username
                byte[] usernameBytes = Encoding.Unicode.GetBytes(username);
                _socket.Send(usernameBytes);
                ok = new byte[4];
                _socket.Receive(ok);
                tmp = Encoding.Unicode.GetString(ok);
                if (Encoding.Unicode.GetString(ok) == "OK")
                {
                    //send hashed password
                    byte[] hash = CreateHashedPassword(Encoding.Unicode.GetBytes(password), salt);
                    int len = hash.Length;
                    _socket.Send(hash);
                    _socket.Receive(ok);
                    tmp = Encoding.Unicode.GetString(ok);
                    if (Encoding.Unicode.GetString(ok) == "OK")
                    {
                        //send details and get new subscription
                        //send clientID, courseID, eduLevelID, gradeID, termID
                        byte[] ids = new byte[40];
                        BitConverter.GetBytes(clientID).CopyTo(ids, 0);
                        BitConverter.GetBytes(courseID).CopyTo(ids, 8);
                        BitConverter.GetBytes(eduLevelID).CopyTo(ids, 16);
                        BitConverter.GetBytes(gradeID).CopyTo(ids, 24);
                        BitConverter.GetBytes(termID).CopyTo(ids, 32);
                        _socket.Send(ids);
                        _socket.Receive(ok);
                        tmp = Encoding.Unicode.GetString(ok);
                        if (Encoding.Unicode.GetString(ok) == "OK")
                        {
                            //send key
                            _socket.Send(Encoding.Unicode.GetBytes(key));
                            _socket.Receive(ok);
                            if (Encoding.Unicode.GetString(ok) == "OK")
                            {
                                byte[] buffer = new byte[80];
                                _socket.Receive(buffer);
                                long id;
                                byte[] idBytes = new byte[8];
                                Array.Copy(buffer, idBytes, 8);


                                id = BitConverter.ToInt64(idBytes, 0);
                                byte[] intBytes = new byte[4];
                                Array.Copy(buffer, 8, intBytes, 0, 4);
                                int day = BitConverter.ToInt32(intBytes, 0);
                                Array.Copy(buffer, 12, intBytes, 0, 4);
                                int month = BitConverter.ToInt32(intBytes, 0);
                                Array.Copy(buffer, 16, intBytes, 0, 4);
                                int year = BitConverter.ToInt32(intBytes, 0);
                                return new KeyValuePair<long, DateTime>(id, new DateTime(year, month, day));
                            }
                        }
                    }
                }
            }
            return new KeyValuePair<long, DateTime>();
        }

        public void SubscribeServerSide(Socket socket, ClientManager clientManager)
        {
            byte[] ok = Encoding.Unicode.GetBytes("OK");
            byte[] err = Encoding.Unicode.GetBytes("ER");
            //response to action
            socket.Send(ok);
            //receive username
            byte[] buffer = new byte[50];
            socket.Receive(buffer);
            string username = Encoding.Unicode.GetString(buffer).Trim('\0');
            //check for existing user
            if (clientManager.CheckForUser(username))
            {
                //user exists send
                socket.Send(ok);
                //subscribe(socket, username);
                buffer = new byte[64];
                socket.Receive(buffer);
                byte[] hash = clientManager.GetHashedPassword(username);
                if (buffer.SequenceEqual(hash))
                {
                    //valid password send ok
                    socket.Send(ok);
                    buffer = new byte[40];
                    socket.Receive(buffer);
                    //ids received
                    byte[] longBytes = new byte[8];
                    Array.Copy(buffer, 0, longBytes, 0, 8);
                    long clientID = BitConverter.ToInt64(longBytes, 0);
                    Array.Copy(buffer, 8, longBytes, 0, 8);
                    long courseID = BitConverter.ToInt64(longBytes, 0);
                    Array.Copy(buffer, 16, longBytes, 0, 8);
                    long eduLevelID = BitConverter.ToInt64(longBytes, 0);
                    Array.Copy(buffer, 24, longBytes, 0, 8);
                    long gradeID = BitConverter.ToInt64(longBytes, 0);
                    Array.Copy(buffer, 32, longBytes, 0, 8);
                    long termID = BitConverter.ToInt64(longBytes, 0);
                    //send ok
                    socket.Send(ok);
                    //receive key
                    buffer = new byte[48];
                    socket.Receive(buffer); // here
                    string key = Encoding.Unicode.GetString(buffer).Trim('\0');
                    KeyValuePair<long, DateTime> info = clientManager.CreateNewSubscription(clientID, courseID, eduLevelID, gradeID, termID, key);
                    if (info.Key != 0)
                    {
                        //subscription created
                        socket.Send(ok);
                        buffer = new byte[20];

                        long id = info.Key;
                        BitConverter.GetBytes(id).CopyTo(buffer, 0);
                        BitConverter.GetBytes(info.Value.Day).CopyTo(buffer, 8);
                        BitConverter.GetBytes(info.Value.Month).CopyTo(buffer, 12);
                        BitConverter.GetBytes(info.Value.Year).CopyTo(buffer, 16);
                        //send subscription key and date
                        socket.Send(buffer);
                    }
                    else
                        socket.Send(err);
                }
                else
                    socket.Send(err);
            }
            else
                socket.Send(err);
        }

        #endregion

        #region Renew Subscription

        public object RenewSubsClientSide(string username, string password, byte[] salt, string key, ClientManager clientManager)
        {
            if (_socket == null)
                throw new Exception("Socket not initialized");
            byte[] subs = Encoding.Unicode.GetBytes("RENEWSUBS");
            _socket.Send(subs);
            byte[] ok = new byte[4];
            _socket.Receive(ok);
            string tmp = Encoding.Unicode.GetString(ok);
            if (tmp == "OK")
            {
                //send username
                byte[] usernameBytes = Encoding.Unicode.GetBytes(username);
                _socket.Send(usernameBytes);
                ok = new byte[4];
                _socket.Receive(ok);
                if (Encoding.Unicode.GetString(ok) == "OK")
                {
                    //send hashed password
                    byte[] hash = CreateHashedPassword(Encoding.Unicode.GetBytes(password), salt);
                    int len = hash.Length;
                    _socket.Send(BitConverter.GetBytes(len));
                    _socket.Send(hash);
                    _socket.Receive(ok);
                    if (Encoding.Unicode.GetString(ok) == "OK")
                    {
                        //send details and get new subscription
                        return null; //subscribe(_socket, username);
                    }
                }
            }
            return null;
        }

        private void RenewSubsServerSide(Socket socket, ClientManager clientManager)
        {

        }

        #endregion

        #region Notify

        public void NotifyServerSide(PendingNotification notification, string ipAdd, int port)
        {
            _addressFamily = AddressFamily.InterNetwork;
            _ipAddress = IPAddress.Parse(ipAdd);
            _ipEndPoint = new IPEndPoint(_ipAddress, port);
            _socket = new Socket(_addressFamily, _socketType, _protocolType);
            _socket.Connect(_ipEndPoint);
            if (_socket == null)
                throw new Exception("Socket is null");
            //send command
            string command = "NOTIFICATION";
            _socket.Send(BitConverter.GetBytes(command.Length * 2));
            _socket.Send(Encoding.Unicode.GetBytes(command));
            byte[] status = new byte[4];
            _socket.Receive(status);
            if (Encoding.Unicode.GetString(status) == "OK")
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, notification);
                byte[] buffer = ms.GetBuffer();
                _socket.Send(BitConverter.GetBytes(buffer.Length));
                _socket.Send(buffer);
                _socket.Receive(status);
                if (Encoding.Unicode.GetString(status) == "OK")
                {
                    command = "TERMINATE";
                    _socket.Send(BitConverter.GetBytes(command.Length * 2));
                    _socket.Send(Encoding.Unicode.GetBytes(command));
                }
            }
        }

        public void NotifyClientSide(Socket socket)
        {
            byte[] ok = Encoding.Unicode.GetBytes("OK");
            byte[] er = Encoding.Unicode.GetBytes("ER");
            socket.Send(ok);
            byte[] buffer = new byte[4];
            socket.Receive(buffer);
            buffer = new byte[BitConverter.ToInt32(buffer, 0)];
            socket.Receive(buffer);
            BinaryFormatter formatter = new BinaryFormatter();
            PendingNotification notification = (PendingNotification)formatter.Deserialize(new MemoryStream(buffer));
            socket.Send(ok);
            _state.SetNotification(notification);
        }

        #endregion

        #region Check For Updates

        public void CheckUpdatesServerSide(Socket socket, ClientManager manager)
        {
            byte[] ok = Encoding.Unicode.GetBytes("OK");
            socket.Send(ok);
            byte[] buffer = new byte[4];
            socket.Receive(buffer);
            buffer = new byte[BitConverter.ToInt32(buffer, 0)];
            socket.Receive(buffer);
            BinaryFormatter formatter = new BinaryFormatter();
            DateTime lastUpdate = (DateTime)formatter.Deserialize(new MemoryStream(buffer));
            SendChanges(socket, manager, lastUpdate);
            socket.Send(ok);
        }

        public List<ContentVersionInfo> CheckUpdatesClientSide(DateTime lastUpdate)
        {
            try
            {
                if (_socket == null)
                    throw new Exception("Socket is null");
                //send command
                string command = "CHKUPDATES";
                _socket.Send(BitConverter.GetBytes(command.Length * 2));
                _socket.Send(Encoding.Unicode.GetBytes(command));
                byte[] status = new byte[4];
                _socket.Receive(status);
                if (Encoding.Unicode.GetString(status) == "OK")
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(ms, lastUpdate);
                    byte[] buffer = ms.GetBuffer();
                    _socket.Send(BitConverter.GetBytes(buffer.Length));
                    _socket.Send(buffer);
                    _socket.Receive(status);
                    if (Encoding.Unicode.GetString(status) == "OK")
                    {
                        List<ContentVersionInfo> changes = ReceiveChanges();
                        return changes;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private void SendChanges(Socket socket, ClientManager manager, DateTime lastUpdate)
        {
            List<ContentVersionInfo> changes = manager.GetChanges();
            List<ContentVersionInfo> validChanges = changes.Where(x => x.Date > lastUpdate) as List<ContentVersionInfo>;
            socket.Send(BitConverter.GetBytes(validChanges.Count)); //number of changes
            foreach (ContentVersionInfo change in validChanges)
            {
                byte[] changeBytes = null;
                //if (change.Date > lastUpdate)
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryFormatter formatter = new BinaryFormatter();
                    try
                    {
                        formatter.Serialize(ms, change);
                        changeBytes = ms.GetBuffer();
                    }
                    catch { }
                    if (changeBytes != null)
                    {
                        int numOfBlocks;
                        if (changeBytes.Length > 8000)
                        {
                            float len = changeBytes.Length / 8000;
                            numOfBlocks = (int)len;
                       
                            if (len - (int)len > 0)
                                numOfBlocks++;
                            for (int i = 0; i < numOfBlocks; ++i)
                            {
                                byte[] block = new byte[8000];
                                Array.Copy(changeBytes, 8000 * i, block, 0, 8000);
                                socket.Send(block);
                                byte[] echo = new byte[8000];
                                socket.Receive(echo);
                                if (!echo.SequenceEqual(block))
                                    --i;
                            }
                        }
                        else
                        {
                        label:
                            socket.Send(changeBytes);
                            byte[] echo = new byte[changeBytes.Length];
                            socket.Receive(echo);
                            if (!echo.SequenceEqual(changeBytes))
                                goto label;
                        }
                    }
                }
            }
        }

        public List<ContentVersionInfo> ReceiveChanges()
        {
            byte[] buffer = new byte[4];
            _socket.Receive(buffer);
            int num = BitConverter.ToInt32(buffer, 0);

            return null;
        }

        #endregion

        #region Update

        public void UpdateServerSide(Socket socket, ClientManager manager)
        {
            byte[] ok = Encoding.Unicode.GetBytes("OK");
            byte[] err = Encoding.Unicode.GetBytes("ER");
            //response to action
            _socket.Send(ok);
            //receive username
            byte[] buffer = new byte[50];
            int received = _socket.Receive(buffer);
            string username = Encoding.Unicode.GetString(buffer).Substring(0, received / 2);
            //check for existing user
            if (manager.CheckForUser(username))
            {
                //user exists send OK: salt_value
                byte[] salt = manager.GetSalt(username);
                _socket.Send(Encoding.Unicode.GetBytes("OK"));
                _socket.Send(salt);
                //receive hashed password
                buffer = new byte[64];
                _socket.Receive(buffer);
                byte[] hashedPassword = manager.GetHashedPassword(username);
                if (buffer.SequenceEqual(hashedPassword))
                {
                    //password is ok
                    _socket.Send(ok);
                    //receive last update datetime
                    byte[] dateTime = new byte[24];
                    socket.Receive(dateTime);
                    int year, month, day, hour, minute, second;
                    byte[] intBytes = new byte[4];
                    Array.Copy(dateTime, 0, intBytes, 0, 4);
                    year = BitConverter.ToInt32(intBytes, 0);
                    Array.Copy(dateTime, 4, intBytes, 0, 4);
                    month = BitConverter.ToInt32(intBytes, 0);
                    Array.Copy(dateTime, 8, intBytes, 0, 4);
                    day = BitConverter.ToInt32(intBytes, 0);
                    Array.Copy(dateTime, 12, intBytes, 0, 4);
                    hour = BitConverter.ToInt32(intBytes, 0);
                    Array.Copy(dateTime, 16, intBytes, 0, 4);
                    minute = BitConverter.ToInt32(intBytes, 0);
                    Array.Copy(dateTime, 20, intBytes, 0, 4);
                    second = BitConverter.ToInt32(intBytes, 0);
                    socket.Send(ok);
                    SendChanges(socket, manager, new DateTime(year, month, day, hour, minute, second));
                }
                else
                    _socket.Send(err);
            }
            else
                //user does not exist
                _socket.Send(err);
        }

        public void UpdateClientSide(string username, string password, DateTime lastUpdate)
        {
            //if (_socket == null)
            //    throw new Exception("Socket is null");
            ////send action
            //byte[] login = Encoding.Unicode.GetBytes("UPDATE");
            //_socket.Send(BitConverter.GetBytes(login.Length));
            //_socket.Send(login);
            //byte[] ok = new byte[4];
            //_socket.Receive(ok);
            //if (Encoding.Unicode.GetString(ok) == "OK")
            //{
            //    //send username
            //    byte[] usernameBytes = Encoding.Unicode.GetBytes(username);
            //    _socket.Send(usernameBytes);
            //    _socket.Receive(ok); //OK: salt_value
            //    if (Encoding.Unicode.GetString(ok) == "OK")
            //    {
            //        //get salt
            //        byte[] saltBytes = new byte[16];
            //        _socket.Receive(saltBytes);
            //        //send hashed password
            //        byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            //        byte[] hashedPassword = CreateHashedPassword(passwordBytes, saltBytes);
            //        _socket.Send(hashedPassword);
            //        _socket.Receive(ok);
            //        if (Encoding.Unicode.GetString(ok) == "OK") ;
            //            //receive changes
            //    }
            //}
        }

        #endregion

        #region Other Methods

        public static byte[] CreateHashedPassword(byte[] passwordBytes, byte[] saltBytes)
        {
            HashAlgorithm algoritm = new SHA512Managed();
            byte[] hashedPassword = new byte[passwordBytes.Length + saltBytes.Length];
            for (int i = 0; i < passwordBytes.Length; i++)
            {
                hashedPassword[i] = passwordBytes[i];
            }
            for (int i = 0; i < saltBytes.Length; i++)
            {
                hashedPassword[passwordBytes.Length + i] = saltBytes[i];
            }
            hashedPassword = algoritm.ComputeHash(hashedPassword);
            return hashedPassword;
        }

        public static byte[] GenerateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[16];
            rng.GetNonZeroBytes(salt);
            return salt;
        }

        #endregion
    }
}
