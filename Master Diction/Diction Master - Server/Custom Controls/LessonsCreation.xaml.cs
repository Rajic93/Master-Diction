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

namespace Diction_Master___Server.Custom_Controls
{
    /// <summary>
    /// Interaction logic for LessonsCreation.xaml
    /// </summary>
    public partial class LessonsCreation : UserControl
    {
        private bool savedLessons;

        public LessonsCreation()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                if (savedLessons)
                {
                    
                }
            }
        }
    }
}
