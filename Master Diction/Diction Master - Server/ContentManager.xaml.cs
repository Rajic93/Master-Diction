using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Diction_Master___Library;
using Diction_Master___Server.Custom_Controls;

namespace Diction_Master___Server
{
    /// <summary>
    /// Interaction logic for ContentManager.xaml
    /// </summary>
    public partial class ContentManager : Window
    {
        private int active_section = 0;
        private Diction_Master___Library.ContentManager manager;
        private Component buildingCourse;
        private MainWindow main;
        private Image selectedLanguage;
        private GradeType selectedGrade;
        private EducationalLevelType selectedEducationalLevel;
        private ObservableCollection<Component> termI;
        private ObservableCollection<Component> termII;
        private ObservableCollection<Component> termIII;

        public ContentManager(MainWindow main)
        {
            this.main = main;
            manager = Diction_Master___Library.ContentManager.CreateInstance();
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\tmp"))
            {
                DirectoryInfo dir = Directory.CreateDirectory("tmp");
                dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            content.Children.Add(new LanguageSelection()
            {
                RenderSize = content.RenderSize,
                Visibility = Visibility.Visible
            });
            content.Children.Add(new LevelSelection()
            {
                RenderSize = content.RenderSize,
                Visibility = Visibility.Collapsed
            });
            content.Children.Add(new WeeksCreation()
            {
                RenderSize = content.RenderSize,
                Visibility = Visibility.Collapsed
            });
            content.Children.Add(new LessonsCreation()
            {
                RenderSize = content.RenderSize,
                Visibility = Visibility.Collapsed
            });
            content.Children.Add(new ContentUpload()
            {
                RenderSize = content.RenderSize,
                Visibility = Visibility.Collapsed
            });
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Labela.Text = "Choose language";
        }

        private void TextBlock_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            //Labela.Text = "Define Weeks";
        }

        private void TextBlock_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            //Labela.Text = "Define Lessons";
        }

        private void TextBlock_MouseUp_3(object sender, MouseButtonEventArgs e)
        {
            //Labela.Text = "Define Content";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            main.Show();
        }

        private void image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedLanguage != null)
            {
                selectedLanguage.Opacity = 0.5;
            }
            selectedLanguage = (Image) sender;
            selectedLanguage.Opacity = 1;
        }

        private void Next_OnClick(object sender, RoutedEventArgs e)
        {
            bool next = true;
            switch (active_section)
            {
                case 0:
                    if (((LanguageSelection)content.Children[0]).IsSelected())
                    {
                        selectedLanguage = ((LanguageSelection)content.Children[0]).GetSelectedLanguage();
                        Previous.Visibility = Visibility.Visible;
                        string course = selectedLanguage.Source.ToString().Split('/').Last().Split(' ').Last();
                        buildingCourse = manager.GetCourse(course.Split('.').First());
                    }
                    else
                    {
                        MessageBox.Show("Language is not selected!");
                        next = false;
                    }
                    break;
                case 1:
                    if (((LevelSelection)content.Children[1]).LevelSelected())
                    {
                        if (((LevelSelection) content.Children[1]).GradeSelected())
                        {
                            selectedGrade = ((LevelSelection) content.Children[1]).GetSelectedGrade();
                            selectedEducationalLevel =
                                ((LevelSelection) content.Children[1]).GetSelectedEducationalLevel();
                            //buildingCourse = manager.GetChildComponent(buildingCourse, ComponentType.EducationalLevel,
                            //    selectedEducationalLevel, selectedGrade);
                        }
                        else
                        {
                            MessageBox.Show("Grade is not selected!");
                            next = false;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Level is not selected!");
                        next = false;
                    }
                    break;
                case 2:
                    if (!((WeeksCreation)content.Children[2]).IsEmpty())
                    {
                        if (((WeeksCreation)content.Children[2]).IsSaved())
                        {
                            termI = ((WeeksCreation) content.Children[2]).GetTerm(1);
                            termII = ((WeeksCreation)content.Children[2]).GetTerm(2);
                            termIII = ((WeeksCreation) content.Children[2]).GetTerm(3);
                            manager.CreateWeekComponents(buildingCourse, ComponentType.Week, termI, termII, termIII);
                            ((LessonsCreation) content.Children[3]).LoadWeeks(buildingCourse);
                        }
                        else
                        {
                            MessageBox.Show("You did not save progress");
                            next = false;
                            //goto end;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Content is empty");
                        next = false;
                        //goto end;
                    }
                    break;
                case 3:
                    if (!((LessonsCreation) content.Children[3]).IsEmpty())
                    {
                        if (((LessonsCreation) content.Children[3]).IsSaved())
                        {
                            ((ContentUpload) content.Children[4]).LoadLessons(buildingCourse);
                            Next.Visibility = Visibility.Collapsed;
                            SaveCourse.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageBox.Show("Progress is not saved!");
                            next = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Lessons not defined!");
                        next = false;
                    }
                    break;
            }
            if (next && active_section != 4)
            {
                content.Children[active_section].Visibility = Visibility.Collapsed;
                content.Children[++active_section].Visibility = Visibility.Visible;
            }
        
        }

        private void SaveCourse_OnClick(object sender, RoutedEventArgs e)
        {
            manager.SaveCourse();
        }

        private void Previous_OnClick(object sender, RoutedEventArgs e)
        {
            switch (active_section)
            {
                case 1:
                    selectedLanguage = ((LanguageSelection)content.Children[0]).GetSelectedLanguage();
                    Previous.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    SaveCourse.Visibility = Visibility.Collapsed;
                    Next.Visibility = Visibility.Visible;
                    break;
            }
            if (active_section != 0)
            {
                content.Children[active_section].Visibility = Visibility.Collapsed;
                content.Children[--active_section].Visibility = Visibility.Visible;
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
