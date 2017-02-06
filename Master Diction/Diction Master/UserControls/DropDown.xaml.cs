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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Diction_Master___Library.UserControls
{
    /// <summary>
    /// Interaction logic for DropDown.xaml
    /// </summary>
    public partial class DropDown : UserControl
    {
        public double Height { get; set; }
        private bool _down = true;
        private double _maxHeight;
        public DropDown()
        {
            InitializeComponent();
        }

        public void AddContent(UserControl content)
        {
            Content.Children.Add(content);
        }

        private void Image_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Content.Visibility = _down ? Visibility.Visible : Visibility.Collapsed;
            _down = !_down;
        }
    }
}
