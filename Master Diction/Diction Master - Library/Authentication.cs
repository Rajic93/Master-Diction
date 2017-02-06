using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
    public class Authentication
    {
        private Socket _socket;
        private AddressFamily _addressFamily;
        private SocketType _socketType;
        private ProtocolType _protocolType;
        private IPAddress _ipAddress;
        private IPEndPoint _ipEndPoint;

        private bool _running = true;

        public delegate bool Subscribe(Socket socket, string username);

        public Authentication(SocketType socketType, AddressFamily family, ProtocolType protocolType)
        {
            _addressFamily = family;
            _socketType = socketType;
            _protocolType = protocolType;
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
                    else if (command.Contains("SUBSCRIBE"))
                        RenewSubsServerSide(socket, clientManager);
                    else if (command.Contains("CHKUPDATE"))
                        RenewSubsServerSide(socket, clientManager); 
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
                socket.Send(err);
            }
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
                long id = clientManager.CreateNewUser(username, pass, salt);
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
                        //send clientID, courseID, eduLevelID, gradeID, termID
                        byte[] ids = new byte[40];
                        BitConverter.GetBytes(clientID).CopyTo(ids, 0);
                        BitConverter.GetBytes(courseID).CopyTo(ids, 8);
                        BitConverter.GetBytes(eduLevelID).CopyTo(ids, 16);
                        BitConverter.GetBytes(gradeID).CopyTo(ids, 24);
                        BitConverter.GetBytes(termID).CopyTo(ids, 32);
                        _socket.Send(ids);
                        _socket.Receive(ok);
                        if (Encoding.Unicode.GetString(ok) == "OK")
                        {
                            //send key
                            _socket.Send(Encoding.Unicode.GetBytes(key));
                            _socket.Receive(ok);
                            if (Encoding.Unicode.GetString(ok) == "OK")
                            {
                                byte[] buffer = new byte[20];
                                _socket.Receive(buffer);
                                long id;
                                byte[] idBytes = new byte[8];
                                buffer.CopyTo(idBytes, 0);
                                id = BitConverter.ToInt64(idBytes, 0);
                                byte[] intBytes = new byte[4];
                                buffer.CopyTo(intBytes, 8);
                                int day = BitConverter.ToInt32(intBytes, 0);
                                buffer.CopyTo(intBytes, 12);
                                int month = BitConverter.ToInt32(intBytes, 0);
                                buffer.CopyTo(intBytes, 16);
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
                buffer = new byte[512];
                socket.Receive(buffer);
                if (buffer.SequenceEqual(clientManager.GetHashedPassword(username)))
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
                    socket.Receive(buffer);
                    string key = Encoding.Unicode.GetString(buffer);
                    KeyValuePair<long, DateTime> info = clientManager.CreateNewSubscription(clientID, courseID, eduLevelID, gradeID, termID, key);
                    if (info.Key != 0)
                    {
                        //subscription created
                        socket.Send(ok);
                        buffer = new byte[20];
                        BitConverter.GetBytes(info.Key).CopyTo(buffer, 0);
                        BitConverter.GetBytes(info.Value.Day).CopyTo(buffer, 8);
                        BitConverter.GetBytes(info.Value.Month).CopyTo(buffer, 12); ;
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
