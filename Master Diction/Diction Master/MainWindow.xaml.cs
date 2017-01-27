using System;
using System.Collections.Generic;
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

        public MainWindow()
        {
            InitializeComponent();
            if (_firstStart)
            {
                Register();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Authentication<Client> auth = new Authentication<Client>(SocketType.Stream, AddressFamily.InterNetwork, ProtocolType.Tcp);
            auth.Connect("192.168.1.142", 30000);
            auth.RegisterClientSide("Aleks", "Rajic");
        }

        private void NextControlAnimation(UserControl control, UserControl nextControl)
        {
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
            animation.Completed += delegate(object sender, EventArgs args)
            {
                Content = nextControl;
                DoubleAnimation animation2 = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));
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
            regKey.button.Click += delegate (object sender, RoutedEventArgs args)
            {
                NextControlAnimation(regKey, reg);
            };
            reg.button.Click += delegate (object sender, RoutedEventArgs args)
            {
                NextControlAnimation(reg, selection);
            };
            selection.button.Click += delegate (object sender, RoutedEventArgs args)
            {
                NextControlAnimation(selection, level);
            };
            level.button.Click += delegate (object sender, RoutedEventArgs args)
            {
                NextControlAnimation(level, grade);
            };
            grade.button.Click += delegate (object sender, RoutedEventArgs args)
            {
                NextControlAnimation(grade, term);
            };
            Content = regKey;

            Width = selection.Width + 20;
            Height = selection.Height + 40;
        }
    }
}
