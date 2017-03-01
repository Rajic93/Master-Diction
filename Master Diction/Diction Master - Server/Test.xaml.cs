using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using Diction_Master___Library;
using Diction_Master___Server.Custom_Controls;
using System.Net;

namespace Diction_Master___Server
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        private Diction_Master___Library.ContentManager manager;

        public Test()
        {
            manager = Diction_Master___Library.ContentManager.CreateInstance();
            InitializeComponent();
            ButtonBase_OnClick();
            //WrapPanel1.DataContext = manager.GetCourses();

        }

        private void ButtonBase_OnClick()
        {
            //DictionMasterServer server = NetworkModuleFactory.CreateTcpServer(IPAddress.Any, 30000, new ClientManager(ApplicationType.Diction));
            Authentication auth = new Authentication(System.Net.Sockets.SocketType.Stream, System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.ProtocolType.Tcp);
            auth.Listen("127.0.0.1", 30000, new ClientManager(ApplicationType.Diction));
        }
    } 
}
