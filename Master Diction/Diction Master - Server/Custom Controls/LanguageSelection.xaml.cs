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

namespace Diction_Master___Server
{
    /// <summary>
    /// Interaction logic for LanguageSelection.xaml
    /// </summary>
    public partial class LanguageSelection : UserControl
    {
        private Image selectedLanguage;

        public LanguageSelection()
        {
            InitializeComponent();
        }

        private void image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedLanguage != null)
            {
                selectedLanguage.Opacity = 0.5;
            }
            selectedLanguage = (Image)sender;
            selectedLanguage.Opacity = 1;
        }

        public Image GetSelectedLanguage()
        {
            return selectedLanguage;
        }

        public bool IsSelected()
        {
            return selectedLanguage == null ? false : true;
        }
    }
}
