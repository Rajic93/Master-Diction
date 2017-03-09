using System;
using System.Collections.Generic;
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

namespace Diction_Master
{
    /// <summary>
    /// Interaction logic for Notifications.xaml
    /// </summary>
    public partial class Notifications : Window
    {
        private ClientState _state;

        public Notifications(ClientState state)
        {
            _state = state;
            InitializeComponent();
        }
    }
}
