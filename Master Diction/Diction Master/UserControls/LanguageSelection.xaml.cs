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
    /// Interaction logic for LanguageSelection.xaml
    /// </summary>
    public partial class LanguageSelection : UserControl
    {
        private Image _selectedLanguage;
        private string _imageURI;
        private string _nation;
        private LanguagesDictionary _dictionary;
        private List<Component> _availableLanguages;
        private Component _selectedComponent;

        public LanguageSelection()
        {
            _dictionary = new LanguagesDictionary();
            InitializeComponent();
        }

        public void SetAvailableLanguages(List<Component> languages)
        {
            _availableLanguages = languages;
            WrapPanelLanguages.Children.Clear();
            foreach (Component language in _availableLanguages)
            {
                string country = (language as Course).Icon.Split('/').Last().Split('.').First().Substring("Flag of ".Length);
                Image img = new Image
                {
                    Height = 100,
                    Width = 100,
                    Margin = new Thickness(20),
                    Opacity = 0.5,
                    Visibility = Visibility.Visible,
                    Source = new BitmapImage(new Uri("../Resources/Flag of " + country + ".png", UriKind.Relative)),
                    Name = "image_" + (language as Course).Name.Replace('(', '_').Replace(")", ""),
                    ToolTip = _dictionary.GetCountryAbreviation(country)
                };
                img.MouseUp += image_MouseUp;
                WrapPanelLanguages.Children.Add(img);
            }
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
            string name = _selectedLanguage.Name.Substring("image_".Length);
            if (_availableLanguages.Exists(x => (x as Course).Name.Contains(name.Replace('_', '('))))
                _selectedComponent = _availableLanguages.Find(x => (x as Course).Name.Contains(name.Replace('_', '(')));
            else
                _selectedComponent = null;
            button.IsEnabled = true;
        }

        public Component GetSelectedLanguage()
        {
            return _selectedComponent;
        }

        public bool IsSelected()
        {
            return _selectedLanguage != null;
        }

        public string GetSelectedNation() => _nation;
    }
}
