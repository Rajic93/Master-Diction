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
    /// Interaction logic for GradeSelection.xaml
    /// </summary>
    public partial class GradeSelection : UserControl
    {
        private Diction_Master___Library.GradeType SelectedGrade;
        private Diction_Master___Library.EducationalLevelType SelectedEducationalLevel;
        private string Icon;

        private Button previousSelected;

        public GradeSelection(EducationalLevelType type)
        {
            InitializeComponent();
            if (type == EducationalLevelType.Nursery)
            {
                EnableNursery();
            }
            else if (type == EducationalLevelType.Primary)
            {
                EnablePrimary();
            }
            else if (type == EducationalLevelType.Secondary)
            {
                EnableSecondary();
            }
        }

        private void EnableSecondary()
        {
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
        }

        private void EnablePrimary()
        {
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
        }

        private void EnableNursery()
        {
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
        }

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
                    Icon = "pack://application:,,,/Resources/nusery1.png";
                    SelectedGrade = GradeType.NurseryI;
                    break;
                case "NurseryII":
                    Icon = "pack://application:,,,/Resources/nusery2.png";
                    SelectedGrade = GradeType.NurseryII;
                    break;
                case "PrimaryI":
                    Icon = "pack://application:,,,/Resources/1st Grade.png";
                    SelectedGrade = GradeType.PrimaryI;
                    break;
                case "PrimaryII":
                    Icon = "pack://application:,,,/Resources/2nd Grade.png";
                    SelectedGrade = GradeType.PrimaryII;
                    break;
                case "PrimaryIII":
                    Icon = "pack://application:,,,/Resources/3rd Grade.png";
                    SelectedGrade = GradeType.PrimaryIII;
                    break;
                case "PrimaryIV":
                    Icon = "pack://application:,,,/Resources/4th Grade.png";
                    SelectedGrade = GradeType.PrimaryIV;
                    break;
                case "PrimaryV":
                    Icon = "pack://application:,,,/Resources/5th Grade.png";
                    SelectedGrade = GradeType.PrimaryV;
                    break;
                case "PrimaryVI":
                    Icon = "pack://application:,,,/Resources/6th Grade.png";
                    SelectedGrade = GradeType.PrimaryVI;
                    break;
                case "SecondaryJuniorI":
                    Icon = "pack://application:,,,/Resources/1st Grade sec.png";
                    SelectedGrade = GradeType.SecondaryJuniorI;
                    break;
                case "SecondaryJuniorII":
                    Icon = "pack://application:,,,/Resources/2nd Grade sec.png";
                    SelectedGrade = GradeType.SecondaryJuniorII;
                    break;
                case "SecondaryJuniorIII":
                    Icon = "pack://application:,,,/Resources/3rd Grade sec.png";
                    SelectedGrade = GradeType.SecondaryJuniorIII;
                    break;
                case "SecondarySeniorI":
                    Icon = "pack://application:,,,/Resources/4th Grade sec.png";
                    SelectedGrade = GradeType.SecondarySeniorI;
                    break;
                case "SecondarySeniorII":
                    Icon = "pack://application:,,,/Resources/5th Grade sec.png";
                    SelectedGrade = GradeType.SecondarySeniorII;
                    break;
                case "SecondarySeniorIII":
                    Icon = "pack://application:,,,/Resources/6th Grade sec.png";
                    SelectedGrade = GradeType.SecondarySeniorIII;
                    break;
            }
            button.IsEnabled = true;
        }

        public GradeType GetSelectedGrade()
        {
            return SelectedGrade;
        }

        public string GetSelectedIcon()
        {
            return Icon;
        }
    }
}
