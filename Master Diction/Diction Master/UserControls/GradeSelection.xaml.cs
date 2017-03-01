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
    /// Interaction logic for GradeSelection.xaml
    /// </summary>
    public partial class GradeSelection : UserControl
    {
        private Diction_Master___Library.GradeType SelectedGradeType;

        private Component _selectedGrade;
        private List<Component> _availableGrades;

        private Button previousSelected;

        public GradeSelection(EducationalLevelType type)
        {
            _availableGrades = new List<Component>();
            InitializeComponent();
        }

        internal void SetAvailableGrades(List<Component> components)
        {
            foreach (Grade item in components)
            {
                _availableGrades.Add(item);
                switch (item.GradeNum)
                {
                    case GradeType.NurseryI:
                        NurseryI.Visibility = Visibility.Visible;
                        break;
                    case GradeType.NurseryII:
                        NurseryII.Visibility = Visibility.Visible;
                        break;
                    case GradeType.PrimaryI:
                        PrimaryI.Visibility = Visibility.Visible;
                        break;
                    case GradeType.PrimaryII:
                        PrimaryII.Visibility = Visibility.Visible;
                        break;
                    case GradeType.PrimaryIII:
                        PrimaryIII.Visibility = Visibility.Visible;
                        break;
                    case GradeType.PrimaryIV:
                        PrimaryIV.Visibility = Visibility.Visible;
                        break;
                    case GradeType.PrimaryV:
                        PrimaryV.Visibility = Visibility.Visible;
                        break;
                    case GradeType.PrimaryVI:
                        PrimaryVI.Visibility = Visibility.Visible;
                        break;
                    case GradeType.SecondaryJuniorI:
                        SecondaryJuniorI.Visibility = Visibility.Visible;
                        break;
                    case GradeType.SecondaryJuniorII:
                        SecondaryJuniorII.Visibility = Visibility.Visible;
                        break;
                    case GradeType.SecondaryJuniorIII:
                        SecondaryJuniorIII.Visibility = Visibility.Visible;
                        break;
                    case GradeType.SecondarySeniorI:
                        SecondarySeniorI.Visibility = Visibility.Visible;
                        break;
                    case GradeType.SecondarySeniorII:
                        SecondarySeniorII.Visibility = Visibility.Visible;
                        break;
                    case GradeType.SecondarySeniorIII:
                        SecondarySeniorIII.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
        }

        //private void EnableSecondary()
        //{
        //    NurseryI.Visibility = Visibility.Collapsed;
        //    NurseryII.Visibility = Visibility.Collapsed;
        //    PrimaryI.Visibility = Visibility.Collapsed;
        //    PrimaryII.Visibility = Visibility.Collapsed;
        //    PrimaryIII.Visibility = Visibility.Collapsed;
        //    PrimaryIV.Visibility = Visibility.Collapsed;
        //    PrimaryV.Visibility = Visibility.Collapsed;
        //    PrimaryVI.Visibility = Visibility.Collapsed;
        //    SecondaryJuniorI.Visibility = Visibility.Visible;
        //    SecondaryJuniorII.Visibility = Visibility.Visible;
        //    SecondaryJuniorIII.Visibility = Visibility.Visible;
        //    SecondarySeniorI.Visibility = Visibility.Visible;
        //    SecondarySeniorII.Visibility = Visibility.Visible;
        //    SecondarySeniorIII.Visibility = Visibility.Visible;
        //}

        //private void EnablePrimary()
        //{
        //    NurseryI.Visibility = Visibility.Collapsed;
        //    NurseryII.Visibility = Visibility.Collapsed;
        //    PrimaryI.Visibility = Visibility.Visible;
        //    PrimaryII.Visibility = Visibility.Visible;
        //    PrimaryIII.Visibility = Visibility.Visible;
        //    PrimaryIV.Visibility = Visibility.Visible;
        //    PrimaryV.Visibility = Visibility.Visible;
        //    PrimaryVI.Visibility = Visibility.Visible;
        //    SecondaryJuniorI.Visibility = Visibility.Collapsed;
        //    SecondaryJuniorII.Visibility = Visibility.Collapsed;
        //    SecondaryJuniorIII.Visibility = Visibility.Collapsed;
        //    SecondarySeniorI.Visibility = Visibility.Collapsed;
        //    SecondarySeniorII.Visibility = Visibility.Collapsed;
        //    SecondarySeniorIII.Visibility = Visibility.Collapsed;
        //}

        //private void EnableNursery()
        //{
        //    NurseryI.Visibility = Visibility.Visible;
        //    NurseryII.Visibility = Visibility.Visible;
        //    PrimaryI.Visibility = Visibility.Collapsed;
        //    PrimaryII.Visibility = Visibility.Collapsed;
        //    PrimaryIII.Visibility = Visibility.Collapsed;
        //    PrimaryIV.Visibility = Visibility.Collapsed;
        //    PrimaryV.Visibility = Visibility.Collapsed;
        //    PrimaryVI.Visibility = Visibility.Collapsed;
        //    SecondaryJuniorI.Visibility = Visibility.Collapsed;
        //    SecondaryJuniorII.Visibility = Visibility.Collapsed;
        //    SecondaryJuniorIII.Visibility = Visibility.Collapsed;
        //    SecondarySeniorI.Visibility = Visibility.Collapsed;
        //    SecondarySeniorII.Visibility = Visibility.Collapsed;
        //    SecondarySeniorIII.Visibility = Visibility.Collapsed;
        //}

        private void Grade_OnClick(object sender, RoutedEventArgs e)
        {
            if (previousSelected == null)
            {
                previousSelected = (Button)sender;
                previousSelected.Opacity = 1;
            }
            else
            {
                previousSelected.Opacity = 0.6;
                previousSelected = previousSelected = (Button)sender;
                ((Button)sender).Opacity = 1;
            }
            switch (((Button)sender).Name)
            {
                case "NurseryI":
                    //Icon = "pack://application:,,,/Resources/nusery1.png";
                    SelectedGradeType = GradeType.NurseryI;
                    break;
                case "NurseryII":
                    //Icon = "pack://application:,,,/Resources/nusery2.png";
                    SelectedGradeType = GradeType.NurseryII;
                    break;
                case "PrimaryI":
                    //Icon = "pack://application:,,,/Resources/1st Grade.png";
                    SelectedGradeType = GradeType.PrimaryI;
                    break;
                case "PrimaryII":
                    //Icon = "pack://application:,,,/Resources/2nd Grade.png";
                    SelectedGradeType = GradeType.PrimaryII;
                    break;
                case "PrimaryIII":
                    //Icon = "pack://application:,,,/Resources/3rd Grade.png";
                    SelectedGradeType = GradeType.PrimaryIII;
                    break;
                case "PrimaryIV":
                    //Icon = "pack://application:,,,/Resources/4th Grade.png";
                    SelectedGradeType = GradeType.PrimaryIV;
                    break;
                case "PrimaryV":
                   // Icon = "pack://application:,,,/Resources/5th Grade.png";
                    SelectedGradeType = GradeType.PrimaryV;
                    break;
                case "PrimaryVI":
                    //Icon = "pack://application:,,,/Resources/6th Grade.png";
                    SelectedGradeType = GradeType.PrimaryVI;
                    break;
                case "SecondaryJuniorI":
                    //Icon = "pack://application:,,,/Resources/1st Grade sec.png";
                    SelectedGradeType = GradeType.SecondaryJuniorI;
                    break;
                case "SecondaryJuniorII":
                    //Icon = "pack://application:,,,/Resources/2nd Grade sec.png";
                    SelectedGradeType = GradeType.SecondaryJuniorII;
                    break;
                case "SecondaryJuniorIII":
                    //Icon = "pack://application:,,,/Resources/3rd Grade sec.png";
                    SelectedGradeType = GradeType.SecondaryJuniorIII;
                    break;
                case "SecondarySeniorI":
                    //Icon = "pack://application:,,,/Resources/4th Grade sec.png";
                    SelectedGradeType = GradeType.SecondarySeniorI;
                    break;
                case "SecondarySeniorII":
                    //Icon = "pack://application:,,,/Resources/5th Grade sec.png";
                    SelectedGradeType = GradeType.SecondarySeniorII;
                    break;
                case "SecondarySeniorIII":
                    //Icon = "pack://application:,,,/Resources/6th Grade sec.png";
                    SelectedGradeType = GradeType.SecondarySeniorIII;
                    break;
            }
            _selectedGrade = _availableGrades.Find(x => (x as Grade).GradeNum == SelectedGradeType);
            button.IsEnabled = true;
        }

        public Component GetSelectedGrade()
        {
            return _selectedGrade;
        }
    }
}
