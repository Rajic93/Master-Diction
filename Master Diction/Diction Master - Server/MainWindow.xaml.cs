using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Diction_Master___Library;

namespace Diction_Master___Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Diction_Master___Library.ContentManager contentManager;
        private Diction_Master___Library.ClientManager clientManager;

        public MainWindow()
        {
            SetupServer();
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ContentManager manager = new ContentManager(this);
            manager.Show();
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void SetupServer()
        {
            new Thread(() =>
            {
                clientManager = ClientManager.CreateInstance();
                clientManager.Start();
            }).Start();
            
            new Thread(() =>
            {
                contentManager = Diction_Master___Library.ContentManager.CreateInstance(clientManager);
                contentManager.Start();
            }).Start();
        }
    }
}
