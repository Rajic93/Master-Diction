using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Diction_Master___Library;
using Diction_Master___Server.Custom_Controls;

namespace Diction_Master___Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Diction_Master___Library.ContentManager contentManager;
        private Diction_Master___Library.ClientManager clientManager;
        private Thread clientManagerThread;
        private Thread contentManagerThread;
        public MainWindow()
        {
            SetupServer();
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Content manager = new Content(contentManager);
            manager.Closing += delegate(object o, CancelEventArgs args)
            {
                Show();
            };
            manager.Show();
            Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (mcChart.Series[0] as PieSeries).ItemsSource = new KeyValuePair<string, int>[]
            {
                new KeyValuePair<string, int>("Up-to-date Clients", 66),
                new KeyValuePair<string, int>("Out-dated Clients", 34),
                new KeyValuePair<string, int>("Expired subscriptions", 16)
            };
            NoOfCoursesDictionApp.Text = contentManager.GetNoOfCourses().ToString();
        }

        private void SetupServer()
        {
            new Thread(() =>
            {
                clientManager = ClientManager.CreateInstance();
                clientManagerThread = clientManager.Start();
            }).Start();
            new Thread(() =>
            {
                contentManager = Diction_Master___Library.ContentManager.CreateInstance();
                contentManagerThread = contentManager.Start();
            }).Start();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //topics creation
            Window window = new Window()
            {
                Title = "Edit Teachers app content",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                TopicsCreation topicsCreation = new TopicsCreation(contentManager)
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                window.Width = topicsCreation.Width + 35;
                window.Height = topicsCreation.Height + 40;
                window.Content = topicsCreation;
            };
            window.Closing += delegate (object o, CancelEventArgs args)
            {
                if (window.Content != null && !(window.Content as TopicsCreation).IsSaved())
                {
                    args.Cancel = true;
                    MessageBox.Show("Progress is not saved!");
                }
            };
            window.ShowDialog();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            clientManagerThread.Abort();
            contentManagerThread.Abort();
        }
    }
}
