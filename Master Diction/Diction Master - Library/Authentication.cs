using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
    public class Authentication<T>
    {
        private Socket _socket;
        private AddressFamily _addressFamily;
        private SocketType _socketType;
        private ProtocolType _protocolType;
        private IPAddress _ipAddress;
        private IPEndPoint _ipEndPoint;

        private bool _running = true;

        public delegate void CreateUser(string username, byte[] password, byte[] salt);

        public delegate bool CheckForUser(string username);

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

        public int Listen(string ipAdd, int port, CheckForUser checkForUser, CreateUser createUser)
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
                    new Thread(() =>
                    {
                        ConnectionAccepted(socket, checkForUser, createUser);
                    }).Start();
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

        private void ConnectionAccepted(Socket socket, CheckForUser checkForUser, CreateUser createUser)
        {
            byte[] buffer = new byte[8];
            socket.Receive(buffer);
            if (Encoding.Unicode.GetString(buffer) == "LOGIN")
                LoginServerSide(socket, checkForUser);
            else
                RegisterServerSide(socket, checkForUser, createUser);
        }

        public bool LoginClientSide(string username, string password)
        {
            if (_socket == null)
                return false;
            //send action
            byte[] login = Encoding.Unicode.GetBytes("LOGIN");
            _socket.Send(login);
            byte[] ok = new byte[4];
            _socket.Receive(ok);
            if (Encoding.Unicode.GetString(ok) == "OK")
            {
                //send username
                byte[] usernameBytes = Encoding.Unicode.GetBytes(username);
                _socket.Send(usernameBytes);
                ok = new byte[40];
                _socket.Receive(ok); //OK: salt_value
                if (Encoding.Unicode.GetString(ok).Split(':').First() == "OK")
                {
                    //get salt
                    byte[] saltBytes = Encoding.Unicode.GetBytes(Encoding.Unicode.GetString(ok).Split(':').Last());
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

        private static byte[] CreateHashedPassword(byte[] passwordBytes, byte[] saltBytes)
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

        public byte[] RegisterClientSide(string username, string password)
        {
            if (_socket == null)
                throw new Exception("Socket not initialized");
            byte[] register = Encoding.Unicode.GetBytes("REGISTER");
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
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] salt = new byte[16];
                    rng.GetNonZeroBytes(salt);
                    _socket.Send(salt);
                    _socket.Receive(ok);
                    if (Encoding.Unicode.GetString(ok) == "OK")
                    {
                        //send hashed password
                        byte[] passwordBytes = CreateHashedPassword(Encoding.Unicode.GetBytes(password), salt);
                        _socket.Send(passwordBytes);
                        _socket.Receive(ok);
                        if (Encoding.Unicode.GetString(ok) == "OK")
                        {
                            //created new user
                            return salt;
                        }
                    }
                }
                //username exists
            }
            return null;
        }

        private void LoginServerSide(Socket socket, CheckForUser checkForUser)
        {
            byte[] ok = Encoding.Unicode.GetBytes("OK");
            byte[] err = Encoding.Unicode.GetBytes("ER");
            //response to action
            _socket.Send(ok);
            //receive username
            byte[] buffer = new byte[50];
            socket.Receive(buffer);
            string username = Encoding.Unicode.GetString(buffer);
            //check for existing user
            if (checkForUser(username))
            {
                //user exists send OK: salt_value
                string salt = "1234567891234567";
                string password = "Aleks@12";
                socket.Send(Encoding.Unicode.GetBytes("OK: " + salt));
                //receive hashed password
                buffer = new byte[64];
                socket.Receive(buffer);
                byte[] hashedPassword = CreateHashedPassword(Encoding.Unicode.GetBytes(password),
                    Encoding.Unicode.GetBytes(salt));
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

        private void RegisterServerSide(Socket socket, CheckForUser checkForUser, CreateUser createUser)
        {
            byte[] ok = Encoding.Unicode.GetBytes("OK");
            byte[] err = Encoding.Unicode.GetBytes("ER");
            //response to action
            socket.Send(ok);
            //receive username
            byte[] buffer = new byte[50];
            socket.Receive(buffer);
            string username = Encoding.Unicode.GetString(buffer);
            //check for existing user
            if (!checkForUser(username))
            {
                //user does not exist send
                socket.Send(ok);
                //receive salt
                buffer = new byte[16];
                socket.Receive(buffer);
                //save salt
                byte[] salt = new byte[16];
                buffer.CopyTo(salt,0);
                //send ok
                socket.Send(ok);
                //receive hashed password
                buffer = new byte[64];
                socket.Receive(buffer);
                //save hashed password
                byte[] pass = new byte[64];
                buffer.CopyTo(pass, 0);
                //send ok
                createUser(username, pass, salt);
                socket.Send(ok);
            }
            socket.Send(err);
        }
    }
}
