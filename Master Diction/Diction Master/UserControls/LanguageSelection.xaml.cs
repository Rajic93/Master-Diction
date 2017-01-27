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

namespace Diction_Master.UserControls
{
    /// <summary>
    /// Interaction logic for LanguageSelection.xaml
    /// </summary>
    public partial class LanguageSelection : UserControl
    {
        private Image _selectedLanguage;
        private string _imageURI;
        private string _nation;

        public LanguageSelection()
        {
            InitializeComponent();
        }

        private void image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_selectedLanguage != null)
            {
                _selectedLanguage.Opacity = 0.5;
            }
            _selectedLanguage = (Image)sender;
            _selectedLanguage.Opacity = 1;
            _imageURI = _selectedLanguage.Source.ToString();
            _nation = _selectedLanguage.Source.ToString().Split('/').Last().Split('.').First().Substring(("Flag of ").Length);
            button.IsEnabled = true;
        }

        public string GetSelectedLanguageIcon()
        {
            return _imageURI;
        }

        public bool IsSelected()
        {
            return _selectedLanguage != null;
        }

        public string GetSelectedNation() => _nation;
    }
}
