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
        private ClientManager clientManagerAudio;
        private ClientManager clientManagerTeachers;
        private ClientManager clientManagerDiction;

        public MainWindow()
        {
            SetupServer();
            InitializeComponent();
        }

        private void buttonDiction_Click(object sender, RoutedEventArgs e)
        {
            contentManager.SetAppType(ApplicationType.Diction);
            Content manager = new Content(contentManager);
            manager.Closing += delegate(object o, CancelEventArgs args)
            {
                contentManager.Notify(ApplicationType.Diction);
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
            clientManagerAudio = new ClientManager(ApplicationType.Audio);
            clientManagerAudio.Port = 30011;
            clientManagerAudio.Start();
            clientManagerDiction = new ClientManager(ApplicationType.Diction);
            clientManagerDiction.Port = 30012;
            clientManagerDiction.Start();
            clientManagerTeachers = new ClientManager(ApplicationType.Teachers);
            clientManagerTeachers.Port = 30013;
            clientManagerTeachers.Start();
            contentManager = Diction_Master___Library.ContentManager.CreateInstance();
            contentManager.Attach(clientManagerAudio);
            contentManager.Attach(clientManagerDiction);
            contentManager.Attach(clientManagerTeachers);
            contentManager.Start();
        }

        private void buttonAudio_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private void buttonTeachers_Click(object sender, RoutedEventArgs e)
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
                contentManager.SetAppType(ApplicationType.Teachers);
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
                contentManager.Notify(ApplicationType.Teachers);
            };
            window.ShowDialog();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

            contentManager.Detach(clientManagerAudio);
            contentManager.Detach(clientManagerDiction);
            contentManager.Detach(clientManagerTeachers);

            clientManagerAudio.Stop();
            clientManagerDiction.Stop();
            clientManagerTeachers.Stop();
            contentManager.Stop();
        }

        private void Updates_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Subscriptions_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
