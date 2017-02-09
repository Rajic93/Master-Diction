using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Diction_Master___Library.UserControls;
using Diction_Master___Library;
using Action = Diction_Master___Library.Action;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Diction_Master;
using System.Threading;
using Diction_Master.UserControls;

namespace Diction_Master___Library
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int Description, int ReservedValue);

        private ClientState _state { get; set; }

        private KeyValidation _keyType;
        private SubscriptionType _subType;
        private long _courseID;
        private long _eduLevelID;
        private long _gradeID;
        private long _termID;
        private DateTime _expirationDateTime;
        private string _key;
        private Authentication _auth;

        private Subscription _currentSubscription;
        private readonly List<Subscription> _subscriptions;
        private UserControl _currentControl;
        private readonly Stack<UserControl> _previousControls;
        private readonly Stack<UserControl> _nextControls;

        private Thread _backgroundWorker;
        private Thread _listeningThread;
        

        public MainWindow()
        {
            _previousControls = new Stack<UserControl>();
            _nextControls = new Stack<UserControl>();
            _subscriptions = new List<Subscription>();
            _auth = new Authentication(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);

            LoadConfig();
            InitializeComponent();
            if (_state._firstStart)
                Register();
            else
                if (_state._registered)
                    Login();
                else
                    Register();
            _backgroundWorker = new Thread(() =>
            {
                BackgroundWork();
            });
            _backgroundWorker.Start();
        }

        #region Config

        private void LoadConfig()
        {
            if (File.Exists("ClMan.dat"))
            {
                FileStream fs = new FileStream("ClMan.dat", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    _state = (ClientState)formatter.Deserialize(fs);
                }
                catch (SerializationException e)
                {
                    _state = new ClientState();
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                } 
            }
            else
                _state = new ClientState();
        }

        private void SaveConfig()
        {
            _state._loggedIn = false;
            FileStream fs = new FileStream("ClMan.dat", FileMode.Create, FileAccess.Write);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, _state);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
            }
            finally
            {
                fs.Close();
            }
        }

        #endregion

        #region Background Work

        private void BackgroundWork()
        {
            _listeningThread = new Thread(() => ListenForNotifications());
            _listeningThread.Start();
            while (true)
            {
                if (_state._registered && _state.LocalRegistration)
                {
                    TryRegister();
                }
                if (_state._registered && !_state.LocalRegistration && _state._pendingSubscriptions.Count != 0)
                {
                    TrySubscribe(_state._pendingSubscriptions.First());
                }
                if (_state._pendingNotification)
                {
                    //show notifications
                    int num = _state._notifications.Count;
                    string notif = num + " Total Notification";
                    if (num > 1)
                        notif += "s";
                    Notification.Text = notif;
                }
            }
        }

        private void ListenForNotifications()
        {
            Authentication auth = new Authentication(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
            auth.SetNotificationHandler(_state);
            auth.Listen(_state._serverIPAdd, 50000, null);
        }

        private void TryRegister()
        {
            try
            {
                Authentication auth = new Authentication(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
                auth.Connect(_state._serverIPAdd, _state._port);
                KeyValuePair<byte[], long> ret = auth.RegisterClientSide(_state.clientProfile.Username, _state._pass, _state.clientProfile.Salt);
                _state.clientProfile.Password = ret.Key;
                _state.clientProfile.ID = ret.Value;
                _state.LocalRegistration = false;
            }
            catch (Exception)
            {
                Thread.Sleep(10000);
                //Thread.Sleep(3600000);
            }
        }

        private void TrySubscribe(Subscription subscription)
        {
            try
            {
                Authentication auth = new Authentication(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
                auth.Connect(_state._serverIPAdd, _state._port);
                KeyValuePair<long, DateTime> ret = auth.SubscribeClientSide(_state.clientProfile.Username, _state._pass, _state.clientProfile.Salt, subscription.Key,
                    _state.clientProfile.ID, subscription.CourseID, subscription.EduLevelID, subscription.GradeID, subscription.TermID);
                if (ret.Key != 0)
                {
                    subscription.ID = ret.Key;
                    subscription.ExpirationDateTime = ret.Value;
                    _state.clientProfile.ActiveSubscriptions.Add(subscription);
                    _state._pendingSubscriptions.Remove(subscription); 
                }
                else
                    Thread.Sleep(10000);
            }
            catch (Exception)
            {
                Thread.Sleep(10000);
                //Thread.Sleep(3600000);
            }
        }

        #endregion

        #region Events

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _backgroundWorker.Abort();
            _listeningThread.Abort();
            SaveConfig();
        }

        private void Notification_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Notification.Text != "")
            {

            }
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            if (_previousControls.Count != 0)
            {
                if (_previousControls.Peek().GetType().Name == "Login" && _state._loggedIn)
                {
                    if (MessageBox.Show("Do you want to logout?", "Logout", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        _nextControls.Clear();// Push(_currentControl);
                        UserControl prev = _currentControl;
                        _currentControl = _previousControls.Pop();
                        NextControlAnimation(prev, _currentControl);
                        Next.IsEnabled = false;
                        if (_previousControls.Count == 0)
                            Back.IsEnabled = false;
                        _state._loggedIn = false;
                        Loggout.Visibility = Visibility.Collapsed;
                        Loggin.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    _nextControls.Push(_currentControl);
                    UserControl prev = _currentControl;
                    _currentControl = _previousControls.Pop();
                    NextControlAnimation(prev, _currentControl);
                    Next.IsEnabled = true;
                    if (_previousControls.Count == 0)
                        Back.IsEnabled = false;
                }
            }
        }

        private void Next_OnClick(object sender, RoutedEventArgs e)
        {
            if (_nextControls.Count != 0)
            {
                _previousControls.Push(_currentControl);
                UserControl prev = _currentControl;
                _currentControl = _nextControls.Pop();
                NextControlAnimation(prev, _currentControl);
                Back.IsEnabled = true;
                if (_nextControls.Count == 0)
                    Next.IsEnabled = false;
            }
        }

        private void Loggin_OnClick(object sender, RoutedEventArgs e)
        {
            Login login = new Login
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 0
            };
            login.button.Click += delegate (object o, RoutedEventArgs args)
            {

            };
            login.skip.MouseUp += delegate (object o, MouseButtonEventArgs args)
            {

            };
            if (_currentControl != null)
            {
                _previousControls.Push(_currentControl);
                UserControl temp = _currentControl;
                _currentControl = login;
                Back.IsEnabled = true;
                NextControlAnimation(temp, _currentControl);
            }
            else
            {
                NextControlAnimation(new UserControl()
                {
                    Opacity = 1
                }, login);
            }
        }

        private void RegisterUser_OnClick(object sender, RoutedEventArgs e)
        {
            Register register = new Register
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 0
            };
            register.button.Click += delegate (object o, RoutedEventArgs args)
            {

            };
            if (_currentControl != null)
            {
                _previousControls.Push(_currentControl);
                UserControl temp = _currentControl;
                _currentControl = register;
                Back.IsEnabled = true;
                NextControlAnimation(temp, _currentControl);
            }
            else
            {
                NextControlAnimation(new UserControl()
                {
                    Opacity = 1
                }, register);
            }
        }

        private void Loggout_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void Properties_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void CheckForUpdates_OnClick(object sender, RoutedEventArgs e)
        {
            Authentication auth = new Authentication(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
            auth.Connect(_state._serverIPAdd, _state._port);
            auth.
        }

        #endregion

        #region Animation

        private void NextControlAnimation(UserControl control, UserControl nextControl)
        {
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.3));
            animation.Completed += delegate (object sender, EventArgs args)
            {
                Panel.Children.Clear();
                Panel.Children.Add(nextControl);
                DoubleAnimation animation2 = new DoubleAnimation(1, TimeSpan.FromSeconds(0.3));
                nextControl.BeginAnimation(OpacityProperty, animation2);
            };
            control.BeginAnimation(OpacityProperty, animation);
        }

        #endregion

        #region Register

        public void Register()
        {
            RegistrationKey regKey = new RegistrationKey();
            regKey.HorizontalAlignment = HorizontalAlignment.Center;
            regKey.VerticalAlignment = VerticalAlignment.Center;

            regKey.Opacity = 1;
            regKey.button.Click += delegate (object sender, RoutedEventArgs args)
            {
                string key = regKey.textBox.Text + regKey.textBox1.Text + regKey.textBox2.Text + regKey.textBox3.Text +
                             regKey.textBox4.Text + regKey.textBox5.Text;
                try
                {
                    _keyType = new Keys().CheckKey("FX8hMdt5JixcCokAwdik2B4A"); //new Keys().CheckKey(key);
                }
                catch (Exception)
                {
                    // ignored
                }
                if (_keyType != KeyValidation.Invalid) //valid key FX8hMdt5JixcCokAwdik2B4A
                {
                    //save key and create profile
                    _key = "FX8hMdt5JixcCokAwdik2B4A";// key;
                    if (_keyType == KeyValidation.ValidOneTerm)
                    {
                        _subType = SubscriptionType.Term;
                    }
                    if (_keyType == KeyValidation.ValidFullYear)
                    {
                        _subType = SubscriptionType.Year;
                    }
                    Register reg = new Register()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Opacity = 0
                    };
                    reg.button.Click += delegate (object obj, RoutedEventArgs arg)
                    {
                        //register
                        if (reg.Username.Text != "" && reg.Password.Text != "" && reg.Password.Text == reg.RepeatPassword.Text)
                        {
                            int desc;
                            if (InternetGetConnectedState(out desc, 0)) //online
                            {
                                try
                                {
                                    _auth.Connect(_state._serverIPAdd, _state._port);
                                    KeyValuePair<byte[], long> info = _auth.RegisterClientSide(reg.Username.Text, reg.Password.Text, null);
                                    if (info.Key != null && info.Value != 0)
                                    {
                                        //success/createClientProfile
                                        _state.clientProfile = new Client
                                        {
                                            ID = info.Value,
                                            Username = reg.Username.Text,
                                            Salt = info.Key
                                        };
                                        _state.clientProfile.Password = Authentication.CreateHashedPassword(Encoding.Unicode.GetBytes(reg.Password.Text), _state.clientProfile.Salt);
                                        _state._registered = true;
                                        _state.LocalRegistration = false;
                                        _state._firstStart = false;
                                        _state._pass = reg.Password.Text;
                                    }
                                    else
                                    {
                                        _state.clientProfile = new Client
                                        {
                                            Username = reg.Username.Text,
                                            Salt = Authentication.GenerateSalt()
                                        };
                                        _state.clientProfile.Password = Authentication.CreateHashedPassword(Encoding.Unicode.GetBytes(reg.Password.Text), _state.clientProfile.Salt);
                                        _state._registered = true;
                                        _state.LocalRegistration = true;
                                        _state._firstStart = false;
                                        _state._pass = reg.Password.Text;
                                        MessageBox.Show("Failed to register.");
                                    }
                                }
                                catch (Exception e)
                                {
                                    _state.clientProfile = new Client
                                    {
                                        Username = reg.Username.Text,
                                        Salt = Authentication.GenerateSalt()
                                    };
                                    _state.clientProfile.Password = Authentication.CreateHashedPassword(Encoding.Unicode.GetBytes(reg.Password.Text), _state.clientProfile.Salt);
                                    _state._registered = true;
                                    _state.LocalRegistration = true;
                                    _state._firstStart = false;
                                    _state._pass = reg.Password.Text;
                                    MessageBox.Show("Failed to connect to server: " + e.Message);
                                }
                            }
                            else
                            {
                                _state.clientProfile = new Client
                                {
                                    Username = reg.Username.Text,
                                    Salt = Authentication.GenerateSalt()
                                };
                                _state.clientProfile.Password = Authentication.CreateHashedPassword(Encoding.Unicode.GetBytes(reg.Password.Text), _state.clientProfile.Salt);
                                _state._registered = true;
                                _state.LocalRegistration = true;
                                _state._firstStart = false;
                                _state._pass = reg.Password.Text;
                            }
                            Login();
                            
                        }
                        else
                        {
                            MessageBox.Show("Username or password not valid");
                        }
                    };
                    _previousControls.Push(regKey);
                    _currentControl = reg;
                    Back.IsEnabled = true;
                    NextControlAnimation(regKey, reg);
                }
                else //invalid key
                {
                    MessageBox.Show("Invalid key!");
                }
            };
            _currentControl = regKey;
            Panel.Children.Add(regKey);
        }

        #endregion

        #region Login

        private void Login()
        {
            Login login = new Login
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            login.button.Click += delegate (object o, RoutedEventArgs eventArgs)
            {
                int desc;
                if (InternetGetConnectedState(out desc, 0))
                {
                    try
                    {
                        _auth.Connect(_state._serverIPAdd, _state._port);
                        if (_auth.LoginClientSide(login.textBox.Text, login.textBox1.Text))
                        {
                            ShowContent();
                            _state._loggedIn = true;
                        }
                        else
                        {
                            MessageBox.Show("Login failed: bad credetials");
                            if (login.textBox.Text == _state.clientProfile.Username &&
                            _state.clientProfile.Password.SequenceEqual(Authentication.CreateHashedPassword(Encoding.Unicode.GetBytes(login.textBox1.Text), _state.clientProfile.Salt)))
                            {
                                _state._loggedIn = true;
                                ShowContent();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //offline mode
                        MessageBox.Show("Login failed: server error.");
                        if (login.textBox.Text == _state.clientProfile.Username &&
                        _state.clientProfile.Password.SequenceEqual(Authentication.CreateHashedPassword(Encoding.Unicode.GetBytes(login.textBox1.Text), _state.clientProfile.Salt)))
                        {
                            _state._loggedIn = true;
                            ShowContent();
                        }
                    } 
                }
                else
                {
                    //offline mode
                    if (login.textBox.Text == _state.clientProfile.Username &&
                    _state.clientProfile.Password.SequenceEqual(Authentication.CreateHashedPassword(Encoding.Unicode.GetBytes(login.textBox1.Text), _state.clientProfile.Salt)))
                    {
                        _state._loggedIn = true;
                        ShowContent();
                    }
                }
            };
            if (_currentControl == null)
            {
                _currentControl = login;
                Panel.Children.Add(login); 
            }
            else
            {
                _previousControls.Push(_currentControl);
                NextControlAnimation(_currentControl, login);
                _currentControl = login;
                Back.IsEnabled = true;
            }
        }

        #endregion

        #region Subscribe

        public void Subscribe()
        {
            foreach (Subscription pendingSubscription in _state._pendingSubscriptions)
            {
                _currentSubscription = pendingSubscription;
                if (_state._subscriptionCreated)
                {
                    _subscriptions.Add(_currentSubscription);
                    _state._pendingSubscriptions.Remove(_currentSubscription);
                }
            }
        }

        #endregion

        #region Other Methods

        private void ShowContent()
        {
            if (_state._contentEnabled)
            {
                //list all available content
                _previousControls.Push(Panel.Children[0] as UserControl);
                Back.IsEnabled = true;
                HomePage home = new HomePage(_state._availableCourses)
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                NextControlAnimation(_currentControl, home);
                home.ShowContent.Click += delegate (object sender, RoutedEventArgs args)
                {
                    _previousControls.Push(Panel.Children[0] as UserControl);
                    Back.IsEnabled = true;
                    ContentContainer container = new ContentContainer()
                    {
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };
                    NextControlAnimation(_currentControl, container);
                    _currentControl = container;
                };
                _currentControl = home;
            }
            else
            {
                ChooseContent();
            }
        }

        private void ChooseContent()
        {
            LanguageSelection selection = new LanguageSelection()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 0
            };
            selection.button.Click += delegate (object sender1, RoutedEventArgs args1)
            {
                //set CourseID
                LevelSelection level = new LevelSelection()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Opacity = 0
                };
                level.button.Click += delegate (object sender2, RoutedEventArgs args2)
                {
                    //set EduLevelID
                    GradeSelection grade = new GradeSelection(level.GetSelectedEducationalLevel())
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Opacity = 0
                    };
                    grade.button.Click += delegate (object sender3, RoutedEventArgs args3)
                    {
                        //set GradeID
                        if (_keyType == KeyValidation.ValidOneTerm)
                        {
                            TermSelection term = new TermSelection()
                            {
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Opacity = 0
                            };
                            term.button.Click += delegate (object ob, RoutedEventArgs eventAr)
                            {
                                //set TermID and ExpDate
                                Confirm(true);
                            };
                            _previousControls.Push(grade);
                            _currentControl = term;
                            NextControlAnimation(grade, term);
                        }
                        else
                        {
                            //set ExpDate
                            Confirm(false);
                        }
                    };
                    _previousControls.Push(level);
                    _currentControl = grade;
                    NextControlAnimation(level, grade);
                };
                _previousControls.Push(selection);
                _currentControl = level;
                NextControlAnimation(selection, level);
            };
            _previousControls.Push(_currentControl);
            NextControlAnimation(_currentControl, selection);
            _currentControl = selection;
        }

        private void Confirm(bool term)
        {
            Subscription subscription = new Subscription();
            subscription.SubscriptionType = _subType;
            subscription.CourseID = _courseID;
            subscription.EduLevelID = _eduLevelID;
            subscription.GradeID = _gradeID;
            subscription.TermID = _termID;
            subscription.ExpirationDateTime = _expirationDateTime; //calculate it
            subscription.Key = _key;
            //save config
            int desc;
            if (InternetGetConnectedState(out desc, 0)) //online
            {
                
                try
                {
                    //ids and key are hardcoded
                    long clID, crsID, eduID, grID, tmID;
                    clID = crsID = eduID = grID = tmID = 0;
                    KeyValuePair<long, DateTime> ret = _auth.SubscribeClientSide(_state.clientProfile.Username, _state._pass, _state.clientProfile.Salt,
                        _key, _state.clientProfile.ID, crsID, eduID, grID, tmID);

                    if (ret.Key != 0)
                    {
                        subscription.ID = ret.Key;
                        subscription.ExpirationDateTime = ret.Value; //not good date - it should be this plus something
                        _state._contentEnabled = true;
                        _state.clientProfile.ActiveSubscriptions.Add(subscription); 
                    }
                    else
                    {
                        _state._contentEnabled = true;
                        _state._pendingSubscriptions.Add(subscription);
                    }
                }
                catch (Exception)
                {
                    //failed to create subsc on server
                    _state._contentEnabled = true;
                    _state._pendingSubscriptions.Add(subscription);
                }
            }
            else
            {
                _state._contentEnabled = true;
                _state._pendingSubscriptions.Add(subscription);
            }
            ShowContent();
        }

        #endregion

        #region Potential Delete

        private void GetValue()
        {
            RegistrationKey regKey = new RegistrationKey();
            regKey.HorizontalAlignment = HorizontalAlignment.Center;
            regKey.VerticalAlignment = VerticalAlignment.Center;
            regKey.Opacity = 1;

            LanguageSelection selection = new LanguageSelection()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 0
            };
            Register reg = new Register()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 0
            };
            LevelSelection level = new LevelSelection()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 0
            };
            GradeSelection grade = new GradeSelection(EducationalLevelType.Primary)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 0
            };
            TermSelection term = new TermSelection()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 0
            };
            selection.button.Click += delegate (object sender, RoutedEventArgs args) { NextControlAnimation(selection, level); };
            level.button.Click += delegate (object sender, RoutedEventArgs args) { NextControlAnimation(level, grade); };
            grade.button.Click += delegate (object sender, RoutedEventArgs args) { NextControlAnimation(grade, term); };
            Content = regKey;

            Width = selection.Width + 20;
            Height = selection.Height + 40;
        }

        private bool CreateSubsc(Socket socket, string username)
        {
            try
            {
                //Subscription sub = _currentSubscription;
                Subscription sub = new Subscription
                {
                    CourseID = 1,
                    EduLevelID = 1,
                    GradeID = 1,
                    TermID = 1,
                    ClientID = 1,
                    SubscriptionType = SubscriptionType.Term,
                    Key = "FX8hMdt5JixcCokAwdik2B4AlMWV4K",
                    ExpirationDateTime = DateTime.Today
                };
                //5*long + 3*int + byte + string = 40 + 12 + 1 + 60 = 113
                //byte[] buffer = Encoding.Unicode.GetBytes(_key);
                byte[] subsBytes = new byte[112];
                //write IDs
                BitConverter.GetBytes(sub.CourseID).CopyTo(subsBytes, 0);
                BitConverter.GetBytes(sub.EduLevelID).CopyTo(subsBytes, 8);
                BitConverter.GetBytes(sub.GradeID).CopyTo(subsBytes, 16);
                BitConverter.GetBytes(sub.TermID).CopyTo(subsBytes, 24);
                BitConverter.GetBytes(sub.ClientID).CopyTo(subsBytes, 32);
                //write date
                BitConverter.GetBytes(sub.ExpirationDateTime.Day).CopyTo(subsBytes, 40);
                BitConverter.GetBytes(sub.ExpirationDateTime.Month).CopyTo(subsBytes, 44);
                BitConverter.GetBytes(sub.ExpirationDateTime.Year).CopyTo(subsBytes, 48);
                //write key
                Encoding.Unicode.GetBytes(sub.Key).CopyTo(subsBytes, 52);
                socket.Send(subsBytes);
                byte[] id = new byte[8];
                socket.Receive(id);
                sub.ID = BitConverter.ToInt64(id, 0);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void TMP(Socket socket)
        {
            byte[] subsBytes = new byte[113];
            socket.Receive(subsBytes);
            Subscription sub = new Subscription();
            //read IDs
            byte[] longBytes = new byte[8];
            Array.Copy(subsBytes, 0, longBytes, 0, 8);
            sub.CourseID = BitConverter.ToInt64(longBytes, 0);
            Array.Copy(subsBytes, 8, longBytes, 0, 8);
            sub.EduLevelID = BitConverter.ToInt64(longBytes, 0);
            Array.Copy(subsBytes, 16, longBytes, 0, 8);
            sub.GradeID = BitConverter.ToInt64(longBytes, 0);
            Array.Copy(subsBytes, 24, longBytes, 0, 8);
            sub.TermID = BitConverter.ToInt64(longBytes, 0);
            Array.Copy(subsBytes, 32, longBytes, 0, 8);
            sub.ClientID = BitConverter.ToInt64(longBytes, 0);
            //read type
            byte[] type = new byte[1];
            Array.Copy(subsBytes, 40, type, 0, 1);
            sub.SubscriptionType = BitConverter.ToBoolean(type, 0) ? SubscriptionType.Term : SubscriptionType.Year;
            //read date
            byte[] intBytes = new byte[4];
            Array.Copy(subsBytes, 41, intBytes, 0, 4);
            int day = BitConverter.ToInt32(intBytes, 0);
            Array.Copy(subsBytes, 45, intBytes, 0, 4);
            int month = BitConverter.ToInt32(intBytes, 0);
            Array.Copy(subsBytes, 49, intBytes, 0, 4);
            int year = BitConverter.ToInt32(intBytes, 0);
            sub.ExpirationDateTime = new DateTime(year, month, day);
            //read key
            byte[] keyBytes = new byte[60];
            Array.Copy(subsBytes, 63, keyBytes, 0, 60);
            sub.Key = Encoding.Unicode.GetString(keyBytes);
        }

        #endregion

    }
}
