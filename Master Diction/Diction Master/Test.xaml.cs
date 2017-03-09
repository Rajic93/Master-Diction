using Diction_Master___Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Diction_Master
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        public Test()
        {
            InitializeComponent();
            WrapPanel.Children.Add(new Diction_Master___Library.UserControls.ContentContainer(new ClientState())
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            });
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //DictionMasterClient client = NetworkModuleFactory.CreateTcpClient(IPAddress.Parse("127.0.0.1"), 30000);
            NetworkOperations auth = new NetworkOperations(System.Net.Sockets.SocketType.Stream, System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.ProtocolType.Tcp);
            auth.Connect("127.0.0.1", 30000);
        }
    }
}
