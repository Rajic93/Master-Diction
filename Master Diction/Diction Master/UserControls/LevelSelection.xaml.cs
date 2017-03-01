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
using Diction_Master___Library;

namespace Diction_Master___Library.UserControls
{
    /// <summary>
    /// Interaction logic for LevelSelection.xaml
    /// </summary>
    public partial class LevelSelection : UserControl
    {
        private Diction_Master___Library.EducationalLevelType SelectedEducationalLevel;
        private string Icon;
        private bool levelSelected;

        private Component _selectedGrade;
        private List<Component> _availableLevels;
        


        public LevelSelection()
        {
            _availableLevels = new List<Component>();
            InitializeComponent();
        }

        internal void SetAvailableLevels(List<Component> components)
        {
            _availableLevels.Clear();
            foreach (EducationalLevel item in components)
            {
                _availableLevels.Add(item);
                if (item.Level == EducationalLevelType.Nursery)
                {
                    Nursery.Visibility = Visibility.Visible;
                }
                if (item.Level == EducationalLevelType.Primary)
                {
                    Primary.Visibility = Visibility.Visible;
                }
                if (item.Level == EducationalLevelType.Secondary)
                {
                    Secondary.Visibility = Visibility.Visible;
                }
            }
        }

        private void Nursery_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedEducationalLevel = EducationalLevelType.Nursery;
            Nursery.Opacity = 1;
            Primary.Opacity = 0.6;
            Secondary.Opacity = 0.6;
            levelSelected = true;
            button.IsEnabled = true;
            Icon = ImageNursery.Source.ToString();
            _selectedGrade = _availableLevels.Find(x => (x as EducationalLevel).Level == EducationalLevelType.Nursery);
        }

        private void Primary_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedEducationalLevel = EducationalLevelType.Primary;
            Nursery.Opacity = 0.6;
            Primary.Opacity = 1;
            Secondary.Opacity = 0.6;
            levelSelected = true;
            button.IsEnabled = true;
            Icon = ImagePrimary.Source.ToString();
            _selectedGrade = _availableLevels.Find(x => (x as EducationalLevel).Level == EducationalLevelType.Primary);
        }

        private void Secondary_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedEducationalLevel = EducationalLevelType.Secondary;
            Nursery.Opacity = 0.6;
            Primary.Opacity = 0.6;
            Secondary.Opacity = 1;
            levelSelected = true;
            button.IsEnabled = true;
            Icon = ImageSecondary.Source.ToString();
            _selectedGrade = _availableLevels.Find(x => (x as EducationalLevel).Level == EducationalLevelType.Secondary);
        }

        public Component GetSelectedComponent()
        {
            return _selectedGrade;
        }

        public EducationalLevelType GetSelectedEducationalLevel()
        {
            return SelectedEducationalLevel;
        }
    }
}
