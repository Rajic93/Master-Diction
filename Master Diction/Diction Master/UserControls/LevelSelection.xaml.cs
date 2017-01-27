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

namespace Diction_Master.UserControls
{
    /// <summary>
    /// Interaction logic for LevelSelection.xaml
    /// </summary>
    public partial class LevelSelection : UserControl
    {
        private Diction_Master___Library.GradeType SelectedGrade;
        private Diction_Master___Library.EducationalLevelType SelectedEducationalLevel;
        private string Icon;
        private bool gradeSelected;
        private bool levelSelected;

        private Button previousSelected;

        public LevelSelection()
        {
            InitializeComponent();
        }

        public bool LevelSelected()
        {
            return levelSelected;
        }

        public bool GradeSelected()
        {
            return gradeSelected;
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
        }

        public GradeType GetSelectedGrade()
        {
            return SelectedGrade;
        }

        public EducationalLevelType GetSelectedEducationalLevel()
        {
            return SelectedEducationalLevel;
        }

        public string GetSelectedIcon()
        {
            return Icon;
        }
    }
}
