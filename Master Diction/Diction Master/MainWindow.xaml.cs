using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using Diction_Master.UserControls;
using Diction_Master___Library;
using Action = Diction_Master___Library.Action;

namespace Diction_Master
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool _firstStart = true;
        private bool _contentEnabled = false;
        private bool _registered = false;
        private bool _subscriptionCreated = false;
        private KeyValidation _keyType;
        private SubscriptionType _subType;
        private long _courseID;
        private long _eduLevelID;
        private long _gradeID;
        private long _termID;
        private DateTime _expirationDateTime;
        private string _key;

        private List<Subscription> _subscriptions;
        private UserControl _currentControl;
        private Stack<UserControl> _previousControls;
        private Stack<UserControl> _nextControls;



        public MainWindow()
        {
            _previousControls = new Stack<UserControl>();
            _nextControls = new Stack<UserControl>();
            _subscriptions = new List<Subscription>();
            LoadConfig();
            InitializeComponent();
            if (_firstStart)
            {
                ShowContent();
                //Register();
            }
            else
            {
                Login();
            }
        }

        private void LoadConfig()
        {
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Authentication<Client> auth = new Authentication<Client>(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
            auth.Connect("192.168.1.142", 30000);
            auth.RegisterClientSide("Aleks", "Rajic");
        }

        private void NextControlAnimation(UserControl control, UserControl nextControl)
        {
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.3));
            animation.Completed += delegate(object sender, EventArgs args)
            {
                Panel.Children.Clear();
                Panel.Children.Add(nextControl);
                DoubleAnimation animation2 = new DoubleAnimation(1, TimeSpan.FromSeconds(0.3));
                nextControl.BeginAnimation(Control.OpacityProperty, animation2);
            };
            control.BeginAnimation(Control.OpacityProperty, animation);
        }

        public void Register()
        {
            RegistrationKey regKey = new RegistrationKey();
            regKey.HorizontalAlignment = HorizontalAlignment.Center;
            regKey.VerticalAlignment = VerticalAlignment.Center;
            
            regKey.Opacity = 1;
            regKey.button.Click += delegate(object sender, RoutedEventArgs args)
            {
                if (true) //valid key
                {
                    //save key and create profile
                    Register reg = new Register()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Opacity = 0
                    };
                    reg.button.Click += delegate(object obj, RoutedEventArgs arg)
                    {

                        
                    };
                    reg.skip.MouseUp += delegate(object o, MouseButtonEventArgs eventArgs)
                    {
                        LanguageSelection selection = new LanguageSelection()
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Opacity = 0
                        };
                        selection.button.Click += delegate (object sender1, RoutedEventArgs args1)
                        {
                            LevelSelection level = new LevelSelection()
                            {
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Opacity = 0
                            };
                            level.button.Click += delegate (object sender2, RoutedEventArgs args2)
                            {
                                GradeSelection grade = new GradeSelection(level.GetSelectedEducationalLevel())
                                {
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Opacity = 0
                                };
                                grade.button.Click += delegate (object sender3, RoutedEventArgs args3)
                                {
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
                                            Confirm(true);
                                        };
                                        _previousControls.Push(grade);
                                        _currentControl = term;
                                        NextControlAnimation(grade, term);
                                    }
                                    else
                                    {
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
                        _previousControls.Push(reg);
                        _currentControl = selection;
                        NextControlAnimation(reg, selection);
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

        private void Confirm(bool term)
        {

            _contentEnabled = true;
            _firstStart = false;
            Subscription sub = new Subscription
            {
                SubscriptionType = _subType,
                CourseID = _courseID,
                EduLevelID = _eduLevelID,
                GradeID = _gradeID,
                TermID = _termID,
                ExpirationDateTime = _expirationDateTime,
                Key = _key
            };
            //save config
            if (!_registered && true) //online
            {
                if (MessageBox.Show("You have not registered. Do you want to register now?", "Register", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Authentication<Client> auth = new Authentication<Client>(SocketType.Stream, AddressFamily.InterNetwork,
                        ProtocolType.Tcp);
                    auth.Connect("192.168.1.142", 30000);
                    //take cred from profile
                    byte[] salt = auth.RegisterClientSide("Aleks", "Rajic");
                    if (salt != null)
                    {
                        //save salt
                        //
                        //--------------------------
                        auth.SubscribeClientSide("Aleks", "Rajic", salt, CreatedSubsc);

                        _registered = true;
                        _subscriptionCreated = true;
                    }
                }
                else
                {
                    _registered = false;
                    _subscriptionCreated = false;
                }
            }
            _subscriptions.Add(sub);
            ShowContent();
        }

        private void ShowContent()
        {
            Panel.Children.Clear();
            Panel.Children.Add(new ContentContainer());
        }

        private object CreatedSubsc(byte[] subscriptionBytes)
        {
            return null;
        }

        private void Login()
        {
            Login login = new Login
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 0
            };
            login.button.Click += delegate (object o, RoutedEventArgs eventArgs)
            {
                
            };
            NextControlAnimation(Content as UserControl, login);
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
            selection.button.Click += delegate(object sender, RoutedEventArgs args) { NextControlAnimation(selection, level); };
            level.button.Click += delegate(object sender, RoutedEventArgs args) { NextControlAnimation(level, grade); };
            grade.button.Click += delegate(object sender, RoutedEventArgs args) { NextControlAnimation(grade, term); };
            Content = regKey;

            Width = selection.Width + 20;
            Height = selection.Height + 40;
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            if (_previousControls.Count != 0)
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
            register.button.Click += delegate(object o, RoutedEventArgs args)
            {
                
            };
            register.skip.MouseUp += delegate(object o, MouseButtonEventArgs args)
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
    }
}
