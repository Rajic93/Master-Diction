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

namespace Diction_Master___Server.Custom_Controls
{
    /// <summary>
    /// Interaction logic for LevelSelection.xaml
    /// </summary>
    public partial class LevelSelection : UserControl
    {
        public Diction_Master___Library.GradeType SelectedGrade;
        public Diction_Master___Library.EducationalLevelType SelectEducationalLevel;

        private Button previousSelected;

        public LevelSelection()
        {
            InitializeComponent();
        }

        private void Nursery_OnClick(object sender, RoutedEventArgs e)
        {
            SelectEducationalLevel = EducationalLevelType.Nursery;
            NurseryI.Visibility = Visibility.Visible;
            NurseryII.Visibility = Visibility.Visible;
            PrimaryI.Visibility = Visibility.Collapsed;
            PrimaryII.Visibility = Visibility.Collapsed;
            PrimaryIII.Visibility = Visibility.Collapsed;
            PrimaryIV.Visibility = Visibility.Collapsed;
            PrimaryV.Visibility = Visibility.Collapsed;
            PrimaryVI.Visibility = Visibility.Collapsed;
            SecondaryJuniorI.Visibility = Visibility.Collapsed;
            SecondaryJuniorII.Visibility = Visibility.Collapsed;
            SecondaryJuniorIII.Visibility = Visibility.Collapsed;
            SecondarySeniorI.Visibility = Visibility.Collapsed;
            SecondarySeniorII.Visibility = Visibility.Collapsed;
            SecondarySeniorIII.Visibility = Visibility.Collapsed;
            Nursery.Opacity = 1;
            Primary.Opacity = 0.6;
            Secondary.Opacity = 0.6;
        }

        private void Primary_OnClick(object sender, RoutedEventArgs e)
        {
            SelectEducationalLevel = EducationalLevelType.Primary;
            NurseryI.Visibility = Visibility.Collapsed;
            NurseryII.Visibility = Visibility.Collapsed;
            PrimaryI.Visibility = Visibility.Visible;
            PrimaryII.Visibility = Visibility.Visible;
            PrimaryIII.Visibility = Visibility.Visible;
            PrimaryIV.Visibility = Visibility.Visible;
            PrimaryV.Visibility = Visibility.Visible;
            PrimaryVI.Visibility = Visibility.Visible;
            SecondaryJuniorI.Visibility = Visibility.Collapsed;
            SecondaryJuniorII.Visibility = Visibility.Collapsed;
            SecondaryJuniorIII.Visibility = Visibility.Collapsed;
            SecondarySeniorI.Visibility = Visibility.Collapsed;
            SecondarySeniorII.Visibility = Visibility.Collapsed;
            SecondarySeniorIII.Visibility = Visibility.Collapsed;
            Nursery.Opacity = 0.6;
            Primary.Opacity = 1;
            Secondary.Opacity = 0.6;
        }

        private void Secondary_OnClick(object sender, RoutedEventArgs e)
        {
            SelectEducationalLevel = EducationalLevelType.Secondary;
            NurseryI.Visibility = Visibility.Collapsed;
            NurseryII.Visibility = Visibility.Collapsed;
            PrimaryI.Visibility = Visibility.Collapsed;
            PrimaryII.Visibility = Visibility.Collapsed;
            PrimaryIII.Visibility = Visibility.Collapsed;
            PrimaryIV.Visibility = Visibility.Collapsed;
            PrimaryV.Visibility = Visibility.Collapsed;
            PrimaryVI.Visibility = Visibility.Collapsed;
            SecondaryJuniorI.Visibility = Visibility.Visible;
            SecondaryJuniorII.Visibility = Visibility.Visible;
            SecondaryJuniorIII.Visibility = Visibility.Visible;
            SecondarySeniorI.Visibility = Visibility.Visible;
            SecondarySeniorII.Visibility = Visibility.Visible;
            SecondarySeniorIII.Visibility = Visibility.Visible;
            Nursery.Opacity = 0.6;
            Primary.Opacity = 0.6;
            Secondary.Opacity = 1;
        }

        private void Grade_OnClick(object sender, RoutedEventArgs e)
        {
            if (previousSelected == null)
            {
                previousSelected = (Button) sender;
                previousSelected.Opacity = 1;
            }
            else
            {
                previousSelected.Opacity = 0.6;
                previousSelected = previousSelected = (Button)sender;
                ((Button) sender).Opacity = 1;
            }
            switch (((Button)sender).Name)
            {
                case "NurseryI":
                    SelectedGrade = GradeType.NurseryI;
                    break;
                case "NurseryII":
                    SelectedGrade = GradeType.NurseryII;
                    break;
                case "PrimaryI":
                    SelectedGrade = GradeType.PrimaryI;
                    break;
                case "PrimaryII":
                    SelectedGrade = GradeType.PrimaryII;
                    break;
                case "PrimaryIII":
                    SelectedGrade = GradeType.PrimaryIII;
                    break;
                case "PrimaryIV":
                    SelectedGrade = GradeType.PrimaryIV;
                    break;
                case "PrimaryV":
                    SelectedGrade = GradeType.PrimaryV;
                    break;
                case "PrimaryVI":
                    SelectedGrade = GradeType.PrimaryVI;
                    break;
                case "SecondaryJuniorI":
                    SelectedGrade = GradeType.SecondaryJuniorI;
                    break;
                case "SecondaryJuniorII":
                    SelectedGrade = GradeType.SecondaryJuniorII;
                    break;
                case "SecondaryJuniorIII":
                    SelectedGrade = GradeType.SecondaryJuniorIII;
                    break;
                case "SecondarySeniorI":
                    SelectedGrade = GradeType.SecondarySeniorI;
                    break;
                case "SecondarySeniorII":
                    SelectedGrade = GradeType.SecondarySeniorII;
                    break;
                case "SecondarySeniorIII":
                    SelectedGrade = GradeType.SecondarySeniorIII;
                    break;
            }
        }
    }
}
