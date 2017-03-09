using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using FontAwesome.WPF;

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
        private int _termID;
        private DateTime _expirationDateTime;
        private string _key;
        private NetworkOperations _auth;
        private NetworkOperations _authNotifications;
        private bool _running = true;

        private Subscription _currentSubscription;
        private readonly List<Subscription> _subscriptions;
        private UserControl _currentControl;
        private readonly Stack<UserControl> _previousControls;
        private readonly Stack<UserControl> _nextControls;

        private Thread _backgroundWorker;
        private Thread _listeningThread;
        private bool _loginEnable;


        public MainWindow()
        {
            _previousControls = new Stack<UserControl>();
            _nextControls = new Stack<UserControl>();
            _subscriptions = new List<Subscription>();
            _auth = new NetworkOperations(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);

            LoadConfig();
            InitializeComponent();
            if (_state._firstStart)
                Register();
            else
                if (_state._registered)
                    Login();
                else
                    Register();
            _backgroundWorker = new Thread(BackgroundWork);
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
            //CreateContent();
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
            while (!_state._loggedIn)
            {}
            _listeningThread = new Thread(ListenForNotifications);
            _listeningThread.Start();
            while (_running)
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
                    string notif = "Total " + num + " Notification";
                    if (num > 1)
                        notif += "s";
                    if (_running)
                    {
                        Dispatcher.Invoke(() =>
                                    {
                                        Notification.Text = notif;
                                    }); 
                    }
                }
                if (_state._lastSubscriptionCheck.AddDays(1) <= DateTime.Now)
                {
                    CheckSubscriptions(_state.clientProfile.ActiveSubscriptions);
                    CheckSubscriptions(_state._pendingSubscriptions);
                    _state._lastSubscriptionCheck = DateTime.Now;
                }
            }
        }

        private void CheckSubscriptions(List<Subscription> subscriptions)
        {
            foreach (Subscription subscription in new List<Subscription>(subscriptions))
            {
                if (subscription.ExpirationDateTime < DateTime.Now)
                {
                    if (_state._enabledTerms.Exists(x => x.Key == subscription.GradeID && x.Value == subscription.TermID))
                    {
                        _state._enabledTerms.RemoveAll(x => x.Key == subscription.GradeID && x.Value == subscription.TermID);
                    }
                    foreach (var component in new List<Component>(_state._enabledGrades))
                    {
                        var grade = (Grade) component;
                        if (!_state._enabledTerms.Exists(x => x.Key == grade.ID))
                            _state._enabledGrades.Remove(grade);
                    }
                    foreach (var component in new List<Component>(_state._enabledEduLevels))
                    {
                        var level = (EducationalLevel)component;
                        if (!_state._enabledGrades.Exists(x => x.ParentID == level.ID))
                            _state._enabledEduLevels.Remove(level);
                    }
                    foreach (var component in new List<Component>(_state._enabledCourses))
                    {
                        var course = (Course)component;
                        if (!_state._enabledGrades.Exists(x => x.ParentID == course.ID))
                            _state._enabledCourses.Remove(course);
                    }
                    _state.clientProfile.ExpiredSubscription.Add(subscription);
                    subscriptions.Remove(subscription);
                    _state._notifications.Add(new PendingNotification
                    {
                        ClientID = _state.clientProfile.ID,
                        NotificationType = NotificationType.SubscriptionExpired,
                        SubscriptionID = subscription.ID
                    });
                    _state._pendingNotification = true;
                }
            }
        }

        private void ListenForNotifications()
        {
            _authNotifications = new NetworkOperations(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
            _authNotifications.SetNotificationHandler(_state);
            _authNotifications.Listen(_state._serverIPAdd, 50000, null);
        }

        private void TryRegister()
        {
            try
            {
                NetworkOperations auth = new NetworkOperations(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
                auth.Connect(_state._serverIPAdd, _state._port);
                NetworkOperationResult ret = auth.RegisterClientSide(_state.clientProfile.Username, _state._pass, _state.clientProfile.Salt);
                if (ret.Status == NetworkOperationStatus.Success)
                {
                    _state.clientProfile.Password = ret.Credetials.Key;
                    _state.clientProfile.ID = ret.Credetials.Value;
                    _state.LocalRegistration = false; 
                }
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
                NetworkOperations auth = new NetworkOperations(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
                auth.Connect(_state._serverIPAdd, _state._port);
                NetworkOperationResult ret = auth.SubscribeClientSide(_state.clientProfile.Username, _state._pass, _state.clientProfile.Salt, subscription.Key,
                    _state.clientProfile.ID, subscription.CourseID, subscription.EduLevelID, subscription.GradeID, subscription.TermID);
                if (ret.Status == NetworkOperationStatus.Success)
                {
                    subscription.ID = ret.ValidUntil.Key;
                    subscription.ExpirationDateTime = ret.ValidUntil.Value;
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

        private void MainWindow_OnContentRendered(object sender, EventArgs e)
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += delegate
            {
                Awesome.Visibility = Visibility.Collapsed;
                Awesome.Spin = false;
                GridContent.Visibility = Visibility.Visible;
                GridContent.Opacity = 0;
                DoubleAnimation animation2 = new DoubleAnimation(1, TimeSpan.FromSeconds(0.3));
                GridContent.BeginAnimation(OpacityProperty, animation2);
            };
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 3, 0);
            dispatcherTimer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _running = false;
            //_backgroundWorker.Abort();
            //_listeningThread.Abort(); //does not pass
            _authNotifications?.Terminate();
            SaveConfig();
        }

        private void Notification_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                Notifications notifications = new Notifications(_state);
                notifications.ShowDialog();
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
                Loggin.Visibility = Visibility.Collapsed;
                Loggout.Visibility = Visibility.Visible;
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
                RegisterUser.Visibility = Visibility.Visible;
            }
        }

        private void Properties_OnClick(object sender, RoutedEventArgs e)
        {
            Options options = new Options(_state);
            options.ShowDialog();
        }

        private void CheckForUpdates_OnClick(object sender, RoutedEventArgs e)
        {
            NetworkOperations auth = new NetworkOperations(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
            auth.Connect(_state._serverIPAdd, _state._port);
            //auth.CheckUpdatesClientSide()
        }

        public void AddSubscription_OnClick(object sender, RoutedEventArgs e)
        {
            ChooseContent();
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

        private void Register()
        {
            RegistrationKey regKey = new RegistrationKey
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 1
            };

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
                                    NetworkOperationResult info = _auth.RegisterClientSide(reg.Username.Text, reg.Password.Text, null);
                                    if (info.Status == NetworkOperationStatus.Success)
                                    {
                                        //success/createClientProfile
                                        _state.clientProfile = new Client
                                        {
                                            ID = info.Credetials.Value,
                                            Username = reg.Username.Text,
                                            Salt = info.Credetials.Key
                                        };
                                        _state.clientProfile.Password = NetworkOperations.CreateHashedPassword(Encoding.Unicode.GetBytes(reg.Password.Text), _state.clientProfile.Salt);
                                        _state._registered = true;
                                        _state.LocalRegistration = false;
                                        _state._firstStart = false;
                                        _state._pass = reg.Password.Text;
                                        reg.StatusBox.Text = "";
                                    }
                                    else if (info.Status == NetworkOperationStatus.UnavailableUsername)
                                    {
                                        reg.StatusBox.Text = "Username is not available. Try:";
                                        //reg.Suggestions.Text
                                        _loginEnable = false;
                                    }
                                    else if (info.Status == NetworkOperationStatus.UnableToConnectToServer || info.Status == NetworkOperationStatus.Failed)
                                    {
                                        _state.clientProfile = new Client
                                        {
                                            Username = reg.Username.Text,
                                            Salt = NetworkOperations.GenerateSalt()
                                        };
                                        _state.clientProfile.Password = NetworkOperations.CreateHashedPassword(Encoding.Unicode.GetBytes(reg.Password.Text), _state.clientProfile.Salt);
                                        _state._registered = true;
                                        _state.LocalRegistration = true;
                                        _state._firstStart = false;
                                        _state._pass = reg.Password.Text;
                                        _loginEnable = true;
                                        MessageBox.Show("Failed to register.");
                                    }
                                }
                                catch (Exception e)
                                {
                                    //failed to connect to server
                                    _state.clientProfile = new Client
                                    {
                                        Username = reg.Username.Text,
                                        Salt = NetworkOperations.GenerateSalt()
                                    };
                                    _state.clientProfile.Password = NetworkOperations.CreateHashedPassword(Encoding.Unicode.GetBytes(reg.Password.Text), _state.clientProfile.Salt);
                                    _state._registered = true;
                                    _state.LocalRegistration = true;
                                    _state._firstStart = false;
                                    _state._pass = reg.Password.Text;
                                    _loginEnable = true;
                                    //MessageBox.Show("Failed to connect to server: " + e.Message);
                                }
                            }
                            else
                            {
                                //offline
                                _state.clientProfile = new Client
                                {
                                    Username = reg.Username.Text,
                                    Salt = NetworkOperations.GenerateSalt()
                                };
                                _state.clientProfile.Password = NetworkOperations.CreateHashedPassword(Encoding.Unicode.GetBytes(reg.Password.Text), _state.clientProfile.Salt);
                                _state._registered = true;
                                _state.LocalRegistration = true;
                                _state._firstStart = false;
                                _state._pass = reg.Password.Text;
                                _loginEnable = true;
                            }
                            if (_loginEnable)
                                Login();
                        }
                        else
                        {
                            reg.StatusBox.Text = "Passwords do not match.";
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
                        NetworkOperationResult result = _auth.LoginClientSide(login.textBox.Text, login.textBox1.Text);
                        if (result.Status == NetworkOperationStatus.Success)
                        {
                            ShowContent();
                            _state._loggedIn = true;
                            RegisterUser.Visibility = Visibility.Collapsed;
                            Loggin.Visibility = Visibility.Collapsed;
                            Loggout.Visibility = Visibility.Visible; 
                        }
                        else if (result.Status == NetworkOperationStatus.InvalidCredetials)
                        {
                            MessageBox.Show("Login failed: bad credetials");
                            if (login.textBox.Text == _state.clientProfile.Username &&
                            _state.clientProfile.Password.SequenceEqual(NetworkOperations.CreateHashedPassword(Encoding.Unicode.GetBytes(login.textBox1.Text), _state.clientProfile.Salt)))
                            {
                                _state._loggedIn = true;
                                ShowContent();
                                RegisterUser.Visibility = Visibility.Collapsed;
                                Loggin.Visibility = Visibility.Collapsed;
                                Loggout.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //failed to connect to server
                        //MessageBox.Show("Login failed: server error.");
                        if (login.textBox.Text == _state.clientProfile.Username &&
                        _state.clientProfile.Password.SequenceEqual(NetworkOperations.CreateHashedPassword(Encoding.Unicode.GetBytes(login.textBox1.Text), _state.clientProfile.Salt)))
                        {
                            _state._loggedIn = true;
                            ShowContent();
                            RegisterUser.Visibility = Visibility.Collapsed;
                            Loggin.Visibility = Visibility.Collapsed;
                            Loggout.Visibility = Visibility.Visible;
                        }
                    } 
                }
                else
                {
                    //offline mode
                    if (login.textBox.Text == _state.clientProfile.Username &&
                    _state.clientProfile.Password.SequenceEqual(NetworkOperations.CreateHashedPassword(Encoding.Unicode.GetBytes(login.textBox1.Text), _state.clientProfile.Salt)))
                    {
                        _state._loggedIn = true;
                        RegisterUser.Visibility = Visibility.Collapsed;
                        Loggin.Visibility = Visibility.Collapsed;
                        Loggout.Visibility = Visibility.Visible;
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
                Back.IsEnabled = true;
                HomePage home = new HomePage(_state)
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                _previousControls.Push(_currentControl);
                NextControlAnimation(_currentControl, home);
                _currentControl = home;
                home.ShowContent.Click += delegate (object sender, RoutedEventArgs args)
                {
                    _previousControls.Push(Panel.Children[0] as UserControl);
                    Back.IsEnabled = true;
                    ContentContainer container = new ContentContainer(_state)
                    {
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Margin = new Thickness(10)
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
            selection.SetAvailableLanguages(_state._availableCourses);
            //selection.SetAvailableLanguages(Differention(_state._availableCourses, _state._enabledCourses));
            selection.button.Click += delegate (object sender1, RoutedEventArgs args1)
            {
                //set CourseID
                Component language = selection.GetSelectedLanguage();
                _courseID = language.ID;
                LevelSelection level = new LevelSelection()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Opacity = 0
                };
                var compositeComponent = language as CompositeComponent;
                if (compositeComponent != null && compositeComponent.Components.Count != 0)
                {
                    level.SetAvailableLevels(compositeComponent.Components);
                    //level.SetAvailableLevels(Differention((_state._enabledCourses.Find(x => x.ID == _courseID) as CompositeComponent).Components, _state._enabledEduLevels));
                    level.button.Click += delegate(object sender2, RoutedEventArgs args2)
                    {
                        //set EduLevelID
                        Component selectedLevel = level.GetSelectedComponent();
                        _eduLevelID = selectedLevel.ID;
                        GradeSelection grade = new GradeSelection(level.GetSelectedEducationalLevel())
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Opacity = 0
                        };
                        var component = selectedLevel as CompositeComponent;
                        if (component != null && component.Components.Count != 0)
                        {
                            grade.SetAvailableGrades(component.Components);
                            //grade.SetAvailableGrades(Differention((_state._enabledEduLevels.Find(x => x.ID == _eduLevelID) as CompositeComponent).Components, _state._enabledGrades));
                            grade.button.Click += delegate(object sender3, RoutedEventArgs args3)
                            {
                                //set GradeID
                                Component selectedGrade = grade.GetSelectedGrade();
                                _gradeID = selectedGrade.ID;
                                if (_keyType == KeyValidation.ValidOneTerm)
                                {
                                    TermSelection term = new TermSelection
                                    {
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Opacity = 0
                                    };
                                    var component1 = selectedGrade as CompositeComponent;
                                    if (component1 != null)
                                    {
                                        var o = component1.Components.Find(x => x.ID == _gradeID) as Grade;
                                        if (o != null)
                                        {
                                            term.SetAvailableTerms(o.Components);
                                            //term.SetAvailableTerms(Differention((_state._enabledGrades.Find(x => x.ID == _gradeID) as Grade).Components, _state._enabledTerms));
                                            term.button.Click += delegate(object ob, RoutedEventArgs eventAr)
                                            {
                                                if (component1.Components.Count != 0)
                                                {
                                                    //--------------------------------------------------------
                                                    if (!_state._enabledCourses.Exists(x => x.ID == _courseID))
                                                        _state._enabledCourses.Add(language);
                                                    //--------------------------------------------------------
                                                    //--------------------------------------------------------
                                                    if (!_state._enabledEduLevels.Exists(x => x.ID == _eduLevelID))
                                                        _state._enabledEduLevels.Add(selectedLevel);
                                                    //--------------------------------------------------------
                                                    //--------------------------------------------------------
                                                    if (!_state._enabledGrades.Exists(x => x.ID == _gradeID))
                                                        _state._enabledGrades.Add(selectedGrade);
                                                    //--------------------------------------------------------

                                                    //set TermID and ExpDate
                                                    int selectedTerm = term.GetSelectedTerm();
                                                    _termID = selectedTerm;
                                                    //--------------------------------------------------------
                                                    if (!_state._enabledTerms.Exists(x => x.Key == _gradeID && x.Value == _termID))
                                                        _state._enabledTerms.Add(new KeyValuePair<long, int>(_gradeID, _termID));
                                                    //ExpDate
                                                    //--------------------------------------------------------
                                                    Confirm(true);
                                                }
                                                else
                                                    MessageBox.Show("There is no available content for this grade.");
                                            };
                                            _previousControls.Push(grade);
                                            _currentControl = term;
                                            NextControlAnimation(grade, term);
                                        }
                                        else
                                            MessageBox.Show("There is no available  content for this grade.");
                                    }
                                }
                                else
                                {
                                    var o = selectedGrade as CompositeComponent;
                                    if (o != null && o.Components.Count != 0)
                                    {
                                        //--------------------------------------------------------
                                        if (!_state._enabledCourses.Exists(x => x.ID == _courseID))
                                            _state._enabledCourses.Add(language);
                                        //--------------------------------------------------------
                                        //--------------------------------------------------------
                                        if (!_state._enabledEduLevels.Exists(x => x.ID == _eduLevelID))
                                            _state._enabledEduLevels.Add(selectedLevel);
                                        //--------------------------------------------------------
                                        //--------------------------------------------------------
                                        if (!_state._enabledGrades.Exists(x => x.ID == _gradeID))
                                            _state._enabledGrades.Add(selectedGrade);
                                        //--------------------------------------------------------
                                        //set ExpDate
                                        Confirm(false);
                                    }
                                    else
                                        MessageBox.Show("There is no available content for this grade.");
                                }
                            };
                            _previousControls.Push(level);
                            _currentControl = grade;
                            NextControlAnimation(level, grade);
                        }
                        else
                            MessageBox.Show("There is no available content for this educational level.");
                    };
                    _previousControls.Push(selection);
                    _currentControl = level;
                    NextControlAnimation(selection, level);
                }
                else
                    MessageBox.Show("There is no available content for this course.");
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
                    long crsID, eduID, grID, tmID;
                    crsID = eduID = grID = tmID = 0;
                    NetworkOperationResult ret = _auth.SubscribeClientSide(_state.clientProfile.Username, _state._pass, _state.clientProfile.Salt,
                        _key, _state.clientProfile.ID, crsID, eduID, grID, tmID);

                    if (ret.Status == NetworkOperationStatus.Success)
                    {
                        subscription.ID = ret.ValidUntil.Key;
                        subscription.ExpirationDateTime = ret.ValidUntil.Value; //not good date - it should be this plus something
                        _state._contentEnabled = true;
                        _state.clientProfile.ActiveSubscriptions.Add(subscription); 
                    }
                    else
                    {
                        //failed to create sub
                        _state._contentEnabled = true;
                        _state._pendingSubscriptions.Add(subscription);
                    }
                }
                catch (Exception)
                {
                    //failed to connect to server
                    _state._contentEnabled = true;
                    _state._pendingSubscriptions.Add(subscription);
                }
            }
            else
            {
                //offline
                _state._contentEnabled = true;
                _state._pendingSubscriptions.Add(subscription);
            }
            _previousControls.Push(Panel.Children[0] as UserControl);
            int num = term ? 4 : 3;
            for (int i = 0; i < num; i++)
            {
                _previousControls.Pop();
            }
            ShowContent();
        }

        #endregion

        #region Potential Delete

        public void CreateContent()
        {

            _state._availableCourses.Add(new Course
            {
                Icon = "../Resources/Flag of Serbia.png",
                ID = 1,
                Name = "Serbian",
                ParentID = 0,
            });
                (_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Add(new EducationalLevel
                {
                    ID = 5,
                    ParentID = 1,
                    Level = EducationalLevelType.Nursery,
                    Icon = "../Resources/nursery.jpg"
                });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 17,
                        ParentID = 5,
                        GradeNum = GradeType.NurseryI,
                        Icon = "../Resources/nursery1.png"
                    });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Add(new Week
                        {
                            ID = 73,
                            ParentID = 17,
                            Term = 1,
                            Num = 1,
                            Title = "Week 1"
                        });
            ((((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Find(k => k.ID == 73) as Week).Components.Add(new Lesson
            {
                ID = 83,
                ParentID = 17,
                Num = 1,
                Title = "Lesson 1"
            });
            (((((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Find(k => k.ID == 73) as Week).Components.Find(l => l.ID == 83) as Lesson)
                        .Components.Add(new ContentFile
                {
                    ID = 88,
                    ParentID = 83,
                    URI = "Artist - Black - Wonderful life HD.mp3",
                    icon = "../Resources/audio.png",
                    Title = "Audio 1",
                    Description = "Wonderful life song from Black.",
                    ComponentType = ComponentType.Audio
                });
            (((((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Find(k => k.ID == 73) as Week).Components.Find(l => l.ID == 83) as Lesson)
                        .Components.Add(new ContentFile
                {
                    ID = 89,
                    ParentID = 83,
                    URI = "Content definition.avi",
                    icon = "../Resources/video.png",
                    Title = "Video 1",
                    Description = "Video that explains how to define content on server",
                    ComponentType = ComponentType.Video
                });
            (((((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Find(k => k.ID == 73) as Week).Components.Find(l => l.ID == 83) as Lesson)
                        .Components.Add(new ContentFile
                {
                    ID = 90,
                    ParentID = 83,
                    URI = "hgignore_global.txt",
                    icon = "../Resources/document.png",
                    Title = "Document 1",
                    Description = "Git local ignore file",
                    ComponentType = ComponentType.Document
                });
            (((((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Find(k => k.ID == 73) as Week).Components.Find(l => l.ID == 83) as Lesson)
                        .Components.Add(new Quiz
                {
                    ID = 91,
                    ParentID = 83,
                    Title = "Quiz 1"
                });
            ((((((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Find(k => k.ID == 73) as Week).Components.Find(l => l.ID == 83) as Lesson)
                        .Components.Find(m => m.ID == 91) as Quiz).Components.Add(new Question
                {
                    ID = 92,
                    ParentID = 91,
                    Type = QuestionType.Text,
                    Text = "Hello! What is your name, how old are you and where are you from?",
                    Answer = "Hello! My name is Aleksandar Rajic. I am 23-year old and i am coming from Serbia."
                });
            ((((((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                       .Components.Find(z => z.ID == 17) as Grade).Components.Find(k => k.ID == 73) as Week).Components.Find(l => l.ID == 83) as Lesson)
                       .Components.Find(m => m.ID == 91) as Quiz).Components.Add(new Question
                {
                    ID = 93,
                    ParentID = 91,
                    Type = QuestionType.Choice,
                    Text = "Hello! What is your name, how old are you and where are you from?",
                    Answer = "Hello! My name is Aleksandar Rajic. I am 23-year old and i am coming from Serbia.",
                    WrongAnswers = new ObservableCollection<string>(new[]
                    {
                        "Hello! My name is Aleksandar Rajic and am coming from Serbia.",
                        "Hello! I am 23-year old and i am coming from Serbia.",
                        "Hello! My name is Aleksandar Rajic."
                    })
                });
            ((((((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                       .Components.Find(z => z.ID == 17) as Grade).Components.Find(k => k.ID == 73) as Week).Components.Find(l => l.ID == 83) as Lesson)
                       .Components.Find(m => m.ID == 91) as Quiz).Components.Add(new Question
                {
                    ID = 94,
                    ParentID = 91,
                    Type = QuestionType.Puzzle,
                    Text = "Hello! What is your name, how old are you and where are you from?",
                    Answer = "Hello! My name is Aleksandar Rajic. I am 23-year old and i am coming from Serbia.",
                    Pieces = new ObservableCollection<string>(new[]
                    {
                        "Hello!",
                        "My name is Aleksandar Rajic.",
                        "I am 23-year old",
                        "and i am coming from Serbia."
                    })
                });
            (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Add(new Week
                        {
                            ID = 74,
                            ParentID = 17,
                            Term = 1,
                            Num = 2,
                            Title = "Week 2"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Add(new Week
                        {
                            ID = 75,
                            ParentID = 17,
                            Term = 1,
                            Num = 3,
                            Title = "Week 3"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Add(new Week
                        {
                            ID = 76,
                            ParentID = 17,
                            Term = 2,
                            Num = 4,
                            Title = "Week 4"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Add(new Week
                        {
                            ID = 77,
                            ParentID = 17,
                            Term = 2,
                            Num = 5,
                            Title = "Week 5"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Add(new Week
                        {
                            ID = 78,
                            ParentID = 17,
                            Term = 2,
                            Num = 6,
                            Title = "Week 6"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Add(new Week
                        {
                            ID = 79,
                            ParentID = 17,
                            Term = 3,
                            Num = 7,
                            Title = "Week 7"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Add(new Week
                        {
                            ID = 80,
                            ParentID = 17,
                            Term = 3,
                            Num = 8,
                            Title = "Week 8"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Add(new Week
                        {
                            ID = 81,
                            ParentID = 17,
                            Term = 3,
                            Num = 9,
                            Title = "Week 9"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 17) as Grade).Components.Add(new Week
                        {
                            ID = 82,
                            ParentID = 17,
                            Term = 3,
                            Num = 10,
                            Title = "Week 10"
                        });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 18,
                        ParentID = 5,
                        GradeNum = GradeType.NurseryII,
                        Icon = "../Resources/nursery2.png"
                    });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 18) as Grade).Components.Add(new Week
                        {
                            ID = 83,
                            ParentID = 18,
                            Term = 1,
                            Num = 1,
                            Title = "Week 1"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 18) as Grade).Components.Add(new Week
                        {
                            ID = 84,
                            ParentID = 18,
                            Term = 1,
                            Num = 2,
                            Title = "Week 2"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 18) as Grade).Components.Add(new Week
                        {
                            ID = 85,
                            ParentID = 18,
                            Term = 1,
                            Num = 3,
                            Title = "Week 3"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 18) as Grade).Components.Add(new Week
                        {
                            ID = 86,
                            ParentID = 18,
                            Term = 2,
                            Num = 4,
                            Title = "Week 4"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 18) as Grade).Components.Add(new Week
                        {
                            ID = 87,
                            ParentID = 18,
                            Term = 2,
                            Num = 5,
                            Title = "Week 5"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 18) as Grade).Components.Add(new Week
                        {
                            ID = 88,
                            ParentID = 18,
                            Term = 2,
                            Num = 6,
                            Title = "Week 6"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 18) as Grade).Components.Add(new Week
                        {
                            ID = 89,
                            ParentID = 18,
                            Term = 3,
                            Num = 7,
                            Title = "Week 7"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 18) as Grade).Components.Add(new Week
                        {
                            ID = 90,
                            ParentID = 18,
                            Term = 3,
                            Num = 8,
                            Title = "Week 8"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 18) as Grade).Components.Add(new Week
                        {
                            ID = 91,
                            ParentID = 18,
                            Term = 3,
                            Num = 9,
                            Title = "Week 9"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 5) as EducationalLevel)
                        .Components.Find(z => z.ID == 18) as Grade).Components.Add(new Week
                        {
                            ID = 92,
                            ParentID = 18,
                            Term = 3,
                            Num = 10,
                            Title = "Week 10"
                        });
                (_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Add(new EducationalLevel
                {
                    ID = 6,
                    ParentID = 1,
                    Level = EducationalLevelType.Primary,
                    Icon = "../Resources/primary.jpg"
                });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 25,
                        ParentID = 6,
                        GradeNum = GradeType.PrimaryI,
                        Icon = "../Resources/1st Grade.png"
                    });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 25) as Grade).Components.Add(new Week
                        {
                            ID = 93,
                            ParentID = 25,
                            Term = 1,
                            Num = 1,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 25) as Grade).Components.Add(new Week
                        {
                            ID = 94,
                            ParentID = 25,
                            Term = 1,
                            Num = 2,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 25) as Grade).Components.Add(new Week
                        {
                            ID = 95,
                            ParentID = 25,
                            Term = 1,
                            Num = 3,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 25) as Grade).Components.Add(new Week
                        {
                            ID = 96,
                            ParentID = 25,
                            Term = 2,
                            Num = 4,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 25) as Grade).Components.Add(new Week
                        {
                            ID = 97,
                            ParentID = 25,
                            Term = 2,
                            Num = 5,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 25) as Grade).Components.Add(new Week
                        {
                            ID = 98,
                            ParentID = 25,
                            Term = 2,
                            Num = 6,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 25) as Grade).Components.Add(new Week
                        {
                            ID = 99,
                            ParentID = 25,
                            Term = 3,
                            Num = 7,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 25) as Grade).Components.Add(new Week
                        {
                            ID = 100,
                            ParentID = 25,
                            Term = 3,
                            Num = 8,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 25) as Grade).Components.Add(new Week
                        {
                            ID = 101,
                            ParentID = 25,
                            Term = 3,
                            Num = 9,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 25) as Grade).Components.Add(new Week
                        {
                            ID = 102,
                            ParentID = 25,
                            Term = 3,
                            Num = 10,
                            Title = "Week 10"
                        });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 26,
                        ParentID = 6,
                        GradeNum = GradeType.PrimaryII,
                        Icon = "../Resources/2nd Grade.png"
                    });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 26) as Grade).Components.Add(new Week
                        {
                            ID = 103,
                            ParentID = 26,
                            Term = 1,
                            Num = 1,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 26) as Grade).Components.Add(new Week
                        {
                            ID = 104,
                            ParentID = 26,
                            Term = 1,
                            Num = 2,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 26) as Grade).Components.Add(new Week
                        {
                            ID = 105,
                            ParentID = 26,
                            Term = 1,
                            Num = 3,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 26) as Grade).Components.Add(new Week
                        {
                            ID = 106,
                            ParentID = 26,
                            Term = 2,
                            Num = 4,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 26) as Grade).Components.Add(new Week
                        {
                            ID = 107,
                            ParentID = 26,
                            Term = 2,
                            Num = 5,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 26) as Grade).Components.Add(new Week
                        {
                            ID = 108,
                            ParentID = 26,
                            Term = 2,
                            Num = 6,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 26) as Grade).Components.Add(new Week
                        {
                            ID = 109,
                            ParentID = 26,
                            Term = 3,
                            Num = 7,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 26) as Grade).Components.Add(new Week
                        {
                            ID = 110,
                            ParentID = 26,
                            Term = 3,
                            Num = 8,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 26) as Grade).Components.Add(new Week
                        {
                            ID = 111,
                            ParentID = 26,
                            Term = 3,
                            Num = 9,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 26) as Grade).Components.Add(new Week
                        {
                            ID = 112,
                            ParentID = 26,
                            Term = 3,
                            Num = 10,
                            Title = "Week 10"
                        });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 27,
                        ParentID = 6,
                        GradeNum = GradeType.PrimaryIII,
                        Icon = "../Resources/3rd Grade.png"
                    });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 27) as Grade).Components.Add(new Week
                        {
                            ID = 113,
                            ParentID = 27,
                            Term = 1,
                            Num = 1,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 27) as Grade).Components.Add(new Week
                        {
                            ID = 114,
                            ParentID = 27,
                            Term = 1,
                            Num = 2,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 27) as Grade).Components.Add(new Week
                        {
                            ID = 115,
                            ParentID = 27,
                            Term = 1,
                            Num = 3,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 27) as Grade).Components.Add(new Week
                        {
                            ID = 116,
                            ParentID = 27,
                            Term = 2,
                            Num = 4,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 27) as Grade).Components.Add(new Week
                        {
                            ID = 117,
                            ParentID = 27,
                            Term = 2,
                            Num = 5,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 27) as Grade).Components.Add(new Week
                        {
                            ID = 118,
                            ParentID = 27,
                            Term = 2,
                            Num = 6,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 27) as Grade).Components.Add(new Week
                        {
                            ID = 119,
                            ParentID = 27,
                            Term = 3,
                            Num = 7,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 27) as Grade).Components.Add(new Week
                        {
                            ID = 120,
                            ParentID = 27,
                            Term = 3,
                            Num = 8,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 27) as Grade).Components.Add(new Week
                        {
                            ID = 121,
                            ParentID = 27,
                            Term = 3,
                            Num = 9,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 27) as Grade).Components.Add(new Week
                        {
                            ID = 122,
                            ParentID = 27,
                            Term = 3,
                            Num = 10,
                            Title = "Week 10"
                        });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 28,
                        ParentID = 6,
                        GradeNum = GradeType.PrimaryIV,
                        Icon = "../Resources/4th Grade.png"
                    });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 28) as Grade).Components.Add(new Week
                        {
                            ID = 123,
                            ParentID = 28,
                            Term = 1,
                            Num = 1,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 28) as Grade).Components.Add(new Week
                        {
                            ID = 124,
                            ParentID = 28,
                            Term = 1,
                            Num = 2,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 28) as Grade).Components.Add(new Week
                        {
                            ID = 125,
                            ParentID = 28,
                            Term = 1,
                            Num = 3,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 28) as Grade).Components.Add(new Week
                        {
                            ID = 126,
                            ParentID = 28,
                            Term = 2,
                            Num = 4,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 28) as Grade).Components.Add(new Week
                        {
                            ID = 127,
                            ParentID = 28,
                            Term = 2,
                            Num = 5,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 28) as Grade).Components.Add(new Week
                        {
                            ID = 128,
                            ParentID = 28,
                            Term = 2,
                            Num = 6,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 28) as Grade).Components.Add(new Week
                        {
                            ID = 129,
                            ParentID = 28,
                            Term = 3,
                            Num = 7,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 28) as Grade).Components.Add(new Week
                        {
                            ID = 130,
                            ParentID = 28,
                            Term = 3,
                            Num = 8,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 28) as Grade).Components.Add(new Week
                        {
                            ID = 131,
                            ParentID = 28,
                            Term = 3,
                            Num = 9,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 28) as Grade).Components.Add(new Week
                        {
                            ID = 132,
                            ParentID = 28,
                            Term = 3,
                            Num = 10,
                            Title = "Week 10"
                        });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 29,
                        ParentID = 6,
                        GradeNum = GradeType.PrimaryV,
                        Icon = "../Resources/5th Grade.png"
                    });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 29) as Grade).Components.Add(new Week
                        {
                            ID = 133,
                            ParentID = 29,
                            Term = 1,
                            Num = 1,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 29) as Grade).Components.Add(new Week
                        {
                            ID = 134,
                            ParentID = 29,
                            Term = 1,
                            Num = 2,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 29) as Grade).Components.Add(new Week
                        {
                            ID = 135,
                            ParentID = 29,
                            Term = 1,
                            Num = 3,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 29) as Grade).Components.Add(new Week
                        {
                            ID = 136,
                            ParentID = 29,
                            Term = 2,
                            Num = 4,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 29) as Grade).Components.Add(new Week
                        {
                            ID = 137,
                            ParentID = 29,
                            Term = 2,
                            Num = 5,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 29) as Grade).Components.Add(new Week
                        {
                            ID = 139,
                            ParentID = 29,
                            Term = 2,
                            Num = 6,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 29) as Grade).Components.Add(new Week
                        {
                            ID = 139,
                            ParentID = 29,
                            Term = 3,
                            Num = 7,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 29) as Grade).Components.Add(new Week
                        {
                            ID = 140,
                            ParentID = 29,
                            Term = 3,
                            Num = 8,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 29) as Grade).Components.Add(new Week
                        {
                            ID = 141,
                            ParentID = 29,
                            Term = 3,
                            Num = 9,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 29) as Grade).Components.Add(new Week
                        {
                            ID = 142,
                            ParentID = 29,
                            Term = 3,
                            Num = 10,
                            Title = "Week 10"
                        });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 30,
                        ParentID = 6,
                        GradeNum = GradeType.PrimaryVI,
                        Icon = "../Resources/6th Grade.png"
                    });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 30) as Grade).Components.Add(new Week
                        {
                            ID = 143,
                            ParentID = 30,
                            Term = 1,
                            Num = 1,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 30) as Grade).Components.Add(new Week
                        {
                            ID = 144,
                            ParentID = 30,
                            Term = 1,
                            Num = 2,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 30) as Grade).Components.Add(new Week
                        {
                            ID = 145,
                            ParentID = 30,
                            Term = 1,
                            Num = 3,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 30) as Grade).Components.Add(new Week
                        {
                            ID = 146,
                            ParentID = 30,
                            Term = 2,
                            Num = 4,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 30) as Grade).Components.Add(new Week
                        {
                            ID = 147,
                            ParentID = 30,
                            Term = 2,
                            Num = 5,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 30) as Grade).Components.Add(new Week
                        {
                            ID = 149,
                            ParentID = 30,
                            Term = 2,
                            Num = 6,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 30) as Grade).Components.Add(new Week
                        {
                            ID = 149,
                            ParentID = 30,
                            Term = 3,
                            Num = 7,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 30) as Grade).Components.Add(new Week
                        {
                            ID = 150,
                            ParentID = 30,
                            Term = 3,
                            Num = 8,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 30) as Grade).Components.Add(new Week
                        {
                            ID = 151,
                            ParentID = 30,
                            Term = 3,
                            Num = 9,
                            Title = "Week 10"
                        });
                        (((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 6) as EducationalLevel)
                        .Components.Find(z => z.ID == 30) as Grade).Components.Add(new Week
                        {
                            ID = 152,
                            ParentID = 30,
                            Term = 3,
                            Num = 10,
                            Title = "Week 10"
                        });
                (_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Add(new EducationalLevel
                {
                    ID = 7,
                    ParentID = 1,
                    Level = EducationalLevelType.Secondary,
                    Icon = "../Resources/secondary.jpg"
                });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 7) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 49,
                        ParentID = 7,
                        GradeNum = GradeType.SecondaryJuniorI,
                        Icon = "../Resources/1st Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 7) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 50,
                        ParentID = 7,
                        GradeNum = GradeType.SecondaryJuniorII,
                        Icon = "../Resources/2nd Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 7) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 51,
                        ParentID = 7,
                        GradeNum = GradeType.SecondaryJuniorIII,
                        Icon = "../Resources/3rd Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 7) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 52,
                        ParentID = 7,
                        GradeNum = GradeType.SecondarySeniorI,
                        Icon = "../Resources/4th Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 7) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 53,
                        ParentID = 7,
                        GradeNum = GradeType.SecondarySeniorII,
                        Icon = "../Resources/5th Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 1) as Course).Components.Find(y => (y as EducationalLevel).ID == 7) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 54,
                        ParentID = 7,
                        GradeNum = GradeType.SecondarySeniorIII,
                        Icon = "../Resources/6th Grade sec.png"
                    });
            _state._availableCourses.Add(new Course
            {
                Icon = "../Resources/Flag of Spain.png",
                ID = 2,
                Name = "Spanish",
                ParentID = 0
            });
                (_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Add(new EducationalLevel
                {
                    ID = 8,
                    ParentID = 2,
                    Level = EducationalLevelType.Nursery,
                    Icon = "../Resources/nursery.jpg"
                });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 8) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 19,
                        ParentID = 8,
                        GradeNum = GradeType.NurseryI,
                        Icon = "../Resources/nursery1.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 8) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 20,
                        ParentID = 8,
                        GradeNum = GradeType.NurseryII,
                        Icon = "../Resources/nursery2.png"
                    });
                (_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Add(new EducationalLevel
                {
                    ID = 9,
                    ParentID = 2,
                    Level = EducationalLevelType.Primary,
                    Icon = "../Resources/primary.jpg"
                });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 9) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 31,
                        ParentID = 9,
                        GradeNum = GradeType.PrimaryI,
                        Icon = "../Resources/1st Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 9) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 32,
                        ParentID = 9,
                        GradeNum = GradeType.PrimaryII,
                        Icon = "../Resources/2nd Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 9) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 33,
                        ParentID = 9,
                        GradeNum = GradeType.PrimaryIII,
                        Icon = "../Resources/3rd Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 9) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 34,
                        ParentID = 9,
                        GradeNum = GradeType.PrimaryIV,
                        Icon = "../Resources/4th Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 9) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 35,
                        ParentID = 9,
                        GradeNum = GradeType.PrimaryV,
                        Icon = "../Resources/5th Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 9) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 36,
                        ParentID = 9,
                        GradeNum = GradeType.PrimaryVI,
                        Icon = "../Resources/6th Grade.png"
                    });
                (_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Add(new EducationalLevel
                {
                    ID = 10,
                    ParentID = 2,
                    Level = EducationalLevelType.Secondary,
                    Icon = "../Resources/secondary.jpg"
                });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 10) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 55,
                        ParentID = 10,
                        GradeNum = GradeType.SecondaryJuniorI,
                        Icon = "../Resources/1st Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 10) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 56,
                        ParentID = 10,
                        GradeNum = GradeType.SecondaryJuniorII,
                        Icon = "../Resources/2nd Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 10) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 57,
                        ParentID = 10,
                        GradeNum = GradeType.SecondaryJuniorIII,
                        Icon = "../Resources/3rd Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 10) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 58,
                        ParentID = 10,
                        GradeNum = GradeType.SecondarySeniorI,
                        Icon = "../Resources/4th Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 10) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 59,
                        ParentID = 10,
                        GradeNum = GradeType.SecondarySeniorII,
                        Icon = "../Resources/5th Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 2) as Course).Components.Find(y => (y as EducationalLevel).ID == 10) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 60,
                        ParentID = 10,
                        GradeNum = GradeType.SecondarySeniorIII,
                        Icon = "../Resources/6th Grade sec.png"
                    });
            _state._availableCourses.Add(new Course
            {
                Icon = "../Resources/Flag of United Kingdom.png",
                ID = 3,
                Name = "English",
                ParentID = 0
            });
                (_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Add(new EducationalLevel
                {
                    ID = 11,
                    ParentID = 3,
                    Level = EducationalLevelType.Nursery,
                    Icon = "../Resources/nursery.jpg"
                });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 11) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 21,
                        ParentID = 11,
                        GradeNum = GradeType.NurseryI,
                        Icon = "../Resources/nursery1.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 11) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 22,
                        ParentID = 11,
                        GradeNum = GradeType.NurseryII,
                        Icon = "../Resources/nursery2.png"
                    });
                (_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Add(new EducationalLevel
                {
                    ID = 12,
                    ParentID = 3,
                    Level = EducationalLevelType.Primary,
                    Icon = "../Resources/primary.jpg"
                });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 12) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 37,
                        ParentID = 12,
                        GradeNum = GradeType.PrimaryI,
                        Icon = "../Resources/1st Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 12) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 38,
                        ParentID = 12,
                        GradeNum = GradeType.PrimaryII,
                        Icon = "../Resources/2nd Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 12) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 39,
                        ParentID = 12,
                        GradeNum = GradeType.PrimaryIII,
                        Icon = "../Resources/3rd Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 12) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 40,
                        ParentID = 12,
                        GradeNum = GradeType.PrimaryIV,
                        Icon = "../Resources/4th Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 12) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 41,
                        ParentID = 12,
                        GradeNum = GradeType.PrimaryV,
                        Icon = "../Resources/5th Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 12) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 42,
                        ParentID = 12,
                        GradeNum = GradeType.PrimaryVI,
                        Icon = "../Resources/6th Grade.png"
                    });
                (_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Add(new EducationalLevel
                {
                    ID = 13,
                    ParentID = 3,
                    Level = EducationalLevelType.Secondary,
                    Icon = "../Resources/secondary.jpg"
                });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 13) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 61,
                        ParentID = 13,
                        GradeNum = GradeType.SecondaryJuniorI,
                        Icon = "../Resources/1st Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 13) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 62,
                        ParentID = 13,
                        GradeNum = GradeType.SecondaryJuniorII,
                        Icon = "../Resources/2nd Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 13) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 63,
                        ParentID = 13,
                        GradeNum = GradeType.SecondaryJuniorIII,
                        Icon = "../Resources/3rd Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 13) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 64,
                        ParentID = 13,
                        GradeNum = GradeType.SecondarySeniorI,
                        Icon = "../Resources/4th Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 13) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 65,
                        ParentID = 13,
                        GradeNum = GradeType.SecondarySeniorII,
                        Icon = "../Resources/5th Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 3) as Course).Components.Find(y => (y as EducationalLevel).ID == 13) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 66,
                        ParentID = 13,
                        GradeNum = GradeType.SecondarySeniorIII,
                        Icon = "../Resources/6th Grade sec.png"
                    });
            _state._availableCourses.Add(new Course
            {
                Icon = "../Resources/Flag of United States.png",
                ID = 4,
                Name = "English(American)",
                ParentID = 0
            });
                (_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Add(new EducationalLevel
                {
                    ID = 14,
                    ParentID = 4,
                    Level = EducationalLevelType.Nursery,
                    Icon = "../Resources/nursery.jpg"
                });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 14) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 23,
                        ParentID = 14,
                        GradeNum = GradeType.NurseryI,
                        Icon = "../Resources/nursery1.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 14) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 24,
                        ParentID = 14,
                        GradeNum = GradeType.NurseryII,
                        Icon = "../Resources/nursery2.png"
                    });
                (_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Add(new EducationalLevel
                {
                    ID = 15,
                    ParentID = 4,
                    Level = EducationalLevelType.Primary,
                    Icon = "../Resources/primary.jpg"
                });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 15) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 43,
                        ParentID = 15,
                        GradeNum = GradeType.PrimaryI,
                        Icon = "../Resources/1st Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 15) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 44,
                        ParentID = 15,
                        GradeNum = GradeType.PrimaryII,
                        Icon = "../Resources/2nd Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 15) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 45,
                        ParentID = 15,
                        GradeNum = GradeType.PrimaryIII,
                        Icon = "../Resources/3rd Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 15) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 46,
                        ParentID = 15,
                        GradeNum = GradeType.PrimaryIV,
                        Icon = "../Resources/4th Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 15) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 47,
                        ParentID = 15,
                        GradeNum = GradeType.PrimaryV,
                        Icon = "../Resources/5th Grade.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 15) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 48,
                        ParentID = 15,
                        GradeNum = GradeType.PrimaryVI,
                        Icon = "../Resources/6th Grade.png"
                    });
                (_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Add(new EducationalLevel
                {
                    ID = 16,
                    ParentID = 4,
                    Level = EducationalLevelType.Secondary,
                    Icon = "../Resources/secondary.jpg"
                });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 16) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 67,
                        ParentID = 16,
                        GradeNum = GradeType.SecondaryJuniorI,
                        Icon = "../Resources/1st Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 16) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 68,
                        ParentID = 16,
                        GradeNum = GradeType.SecondaryJuniorII,
                        Icon = "../Resources/2nd Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 16) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 69,
                        ParentID = 16,
                        GradeNum = GradeType.SecondaryJuniorIII,
                        Icon = "../Resources/3rd Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 16) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 70,
                        ParentID = 16,
                        GradeNum = GradeType.SecondarySeniorI,
                        Icon = "../Resources/4th Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 16) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 71,
                        ParentID = 16,
                        GradeNum = GradeType.SecondarySeniorII,
                        Icon = "../Resources/5th Grade sec.png"
                    });
                    ((_state._availableCourses.Find(x => x.ID == 4) as Course).Components.Find(y => (y as EducationalLevel).ID == 16) as EducationalLevel)
                    .Components.Add(new Grade
                    {
                        ID = 72,
                        ParentID = 16,
                        GradeNum = GradeType.SecondarySeniorIII,
                        Icon = "../Resources/6th Grade sec.png"
                    });
        }

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

        //private List<Component> Differention(List<Component> listA, List<Component> listB)
        //{
        //    List<Component> listC = new List<Component>();
        //    foreach (Component itemA in listA)
        //    {
        //        if (!listB.Exists(x => x.ID == itemA.ID))
        //        {
        //            listC.Add(itemA);
        //        }
        //    }
        //    return listC;
        //}

        //private List<Component> Differention(List<Component> listA, List<KeyValuePair<long, int>> listB)
        //{
        //    List<Component> listC = new List<Component>();
        //    foreach (Component itemA in listA)
        //    {
        //        if (!listB.Exists(x => (x as KeyValuePair<long, int>).Key == itemA.ID))
        //        {
        //            listC.Add(itemA);
        //        }
        //    }
        //    return listC;
        //}

        #endregion
    }
}
