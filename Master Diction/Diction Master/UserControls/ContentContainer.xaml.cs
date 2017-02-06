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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Diction_Master___Library.UserControls
{
    /// <summary>
    /// Interaction logic for ContentContainer.xaml
    /// </summary>
    public partial class ContentContainer : UserControl
    {
        private bool _menuMinimized = false;
        private double _previusWidth;
        public ContentContainer()
        {
            InitializeComponent();
        }

        private void Menu_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_menuMinimized)
            {
                Navigation.Width = new GridLength(0.04, GridUnitType.Star);
                Border.Visibility = Visibility.Collapsed;
            }
            else
            {
                Navigation.Width = new GridLength(0.25, GridUnitType.Star);
                Border.Visibility = Visibility.Visible;
            }
            _menuMinimized = !_menuMinimized;
        }
    }
}
