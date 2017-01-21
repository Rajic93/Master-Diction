using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;
using Diction_Master___Library;
using Diction_Master___Server.Custom_Controls;
using Component = Diction_Master___Library.Component;

namespace Diction_Master___Server
{
    /// <summary>
    /// Interaction logic for Content.xaml
    /// </summary>
    public partial class Content : Window
    {
        private Diction_Master___Library.ContentManager _contentManager;
        //Course
        private LanguagesHashTable _languages;
        private Dictionary<string, int> _courseImagesCache;
        private Image _selectedLanguage;
        //Educational Level
        private Dictionary<string, int> _educationalLevelDictionary;
        private Image _selectedEducationalLevel;
        //Grades
        private Dictionary<string, int> _gradesDictionary;
        private Image _selectedGrade;
        //Weeks

        //Lessons

        //
        //private Dictionary<string, Component> _componentsCache;
        //private Dictionary<Image, Component> courses;
        //private ObservableCollection<Image> _courses;
        //private int _chosenCourse;
        //private ObservableCollection<Image> _levels;
        //private int _chosenLevel;
        //private ObservableCollection<Image> _grades;
        //private int _chosenGrade;

        public Content()
        {
            //_courses = new ObservableCollection<Image>();
            //_levels = new ObservableCollection<Image>();
            //_grades = new ObservableCollection<Image>();
            //courses = new Dictionary<Image, Component>();
            _contentManager = Diction_Master___Library.ContentManager.CreateInstance();
            //------------------------------------------------------------------------
            //TMP();
            //------------------------------------------------------------------------
            _languages = new LanguagesHashTable();
            _courseImagesCache = new Dictionary<string, int>();
            _educationalLevelDictionary = new Dictionary<string, int>();
            _gradesDictionary = new Dictionary<string, int>();
            InitializeComponent();
        }

        private void TMP()
        {
            _contentManager.AddCourse("Serbian", "./Resources/Flag of Serbia.png");
            //_contentManager.EditCourse(1, null, "./Resources/Flag of Serbia.png");

            //_contentManager.AddEducationalLevel(Icons.Nursery, 2);
            _contentManager.AddEducationalLevel(1, "pack://application:,,,/Resources/nursery.jpg", EducationalLevelType.Nursery);
            //_contentManager.EditEducationalLevel(2, "./Resources/nursery.png", 0);
            //_contentManager.DeleteEducationalLevel(2, 1);

            _contentManager.AddGrade(2, "pack://application:,,,/Resources/nursery.jpg", GradeType.NurseryI);
            //_contentManager.EditGrade(3, "", GradeType.NurseryI);
            //_contentManager.DeleteGrade(3, 2);

            _contentManager.AddWeek(3, "Week 1", 1, 1);
            //_contentManager.EditWeek(4, "Week 1", 1, 1);
            //_contentManager.DeleteWeek(4, 3);

            _contentManager.AddLesson(4, "Lesson 1", 1);
           // _contentManager.EditLesson(5, "Lesson 1", 1);
            //_contentManager.DeleteLesson(5, 4);

            _contentManager.AddQuiz(5, "Quiz 1");
            //_contentManager.EditQuiz(6, "Quiz 1");
            //_contentManager.DeleteQuiz(6, 5);

            _contentManager.AddQuestion(6, "How old are you?", "I am 24 years old.", QuestionType.Puzzle);
            //_contentManager.EditQuestion(7, "How old are you", "I am 24 years old.", QuestionType.Puzzle);
            //_contentManager.DeleteQuestion(7, 6);

            _contentManager.AddContentFile(5, ComponentType.Audio, "", "", "", 2, "HEHEHE");
            //_contentManager.EditContentFile(8, ComponentType.Audio, "", "", "", 2, "HEHEHE");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        #region Course

        /// <summary>
        /// Add new Course
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddCourse_OnClick(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Create course",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate(object o, RoutedEventArgs args)
            {
                LanguageSelection language = new LanguageSelection()
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                window.Width = language.Width + 15;
                window.Height = language.Height + 40;
                window.Content = language;
                language.button.Click += delegate (object obj, RoutedEventArgs Args)
                {
                    LanguageSelection control = (LanguageSelection)window.Content;
                    string nation = control.GetSelectedNation();
                    string selectedLanguage = _languages.GetLanguageName(nation);
                    if (selectedLanguage != null)
                    {
                        int id = _contentManager.AddCourse(selectedLanguage, control.GetSelectedLanguageIcon());
                        if (id > 0) //new course created
                        {
                            string uri = control.GetSelectedLanguageIcon();
                            Image image = new Image()
                            {
                                Source = new BitmapImage(new Uri(uri)),
                                RenderSize = new Size(100, 100),
                                MaxHeight = 100,
                                MaxWidth = 100,
                                Margin = new Thickness(10),
                                Opacity = 0.5
                            };
                            image.MouseUp += delegate (object senderImage, MouseButtonEventArgs eventArgs)
                            {
                                ImageCourse_Click(senderImage as Image);
                            };
                            if (!_courseImagesCache.ContainsKey(image.Source.ToString()))
                            {
                                _courseImagesCache[image.Source.ToString()] = id;
                                Courses.Children.Add(image);
                                ImageCourse_Click(image);
                                AddEducationalLevel.IsEnabled = true;
                            }
                            else
                                image = null;
                        }// -1 - error; 0 - already exists
                        window.Close();
                        
                    }
                };
            };
            window.ShowDialog();
        }
        /// <summary>
        /// Edit existing Course
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditCourse_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Create course",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                LanguageSelection language = new LanguageSelection()
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                window.Width = language.Width + 15;
                window.Height = language.Height + 40;
                window.Content = language;
                language.button.Click += delegate (object obj, RoutedEventArgs Args)
                {
                    LanguageSelection control = (LanguageSelection)window.Content;
                    int id = _courseImagesCache[_selectedLanguage.Source.ToString()];
                    string nation = control.GetSelectedNation();
                    string selectedLanguage = _languages.GetLanguageName(nation);
                    if (selectedLanguage != null)
                    {
                        _courseImagesCache.Remove(_selectedLanguage.Source.ToString());
                        _courseImagesCache[control.GetSelectedLanguageIcon()] = id;
                        _contentManager.EditCourse(id, selectedLanguage, control.GetSelectedLanguageIcon());
                        _selectedLanguage.Source = new BitmapImage(new Uri(control.GetSelectedLanguageIcon()));
                        window.Close();
                    }
                };
            };
            window.ShowDialog();
        }
        /// <summary>
        /// Delete existing course
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCourse_Click(object sender, RoutedEventArgs e)
        {
            int id = _courseImagesCache[_selectedLanguage.Source.ToString()];
            _contentManager.DeleteCourse(id);
            _courseImagesCache.Remove(_selectedLanguage.Source.ToString());
            Courses.Children.Remove(_selectedLanguage);
            _selectedLanguage = null;
            WrapPanelEducationaLevel.Children.Clear();
            WrapPanelGrades.Children.Clear();
            NoOfFiles.Text = "";
            NoOfLessons.Text = "";
            NoOfWeeks.Text = "";
            EditCourse.IsEnabled = false;
            DeleteCourse.IsEnabled = false;
            AddEducationalLevel.IsEnabled = false;
            EditEducationalLevel.IsEnabled = false;
            DeleteEducationalLevel.IsEnabled = false;
            AddGrade.IsEnabled = false;
            EditGrade.IsEnabled = false;
            DeleteGrade.IsEnabled = false;
            EditWeeks.IsEnabled = false;
            EditLessons.IsEnabled = false;
            EditFiles.IsEnabled = false;
        }

        private void ImageCourse_Click(Image image)
        {
            if (_selectedLanguage != null)
            {
                _selectedLanguage.Opacity = 0.5;
            }
            image.Opacity = 1;
            _selectedLanguage = image;
            int id = _courseImagesCache[image.Source.ToString()];
            EditCourse.IsEnabled = true;
            DeleteCourse.IsEnabled = true;
            AddEducationalLevel.IsEnabled = true;
            LoadEducationalLevels(id);
        }

        #endregion

        #region EducationalLevel
        
        private void LoadEducationalLevels(int id)
        {
            WrapPanelEducationaLevel.Children.Clear();
            List<int> childrenIDs = _contentManager.GetChildrenIDs(id);
            if (childrenIDs != null)
            {
                foreach (int childID in childrenIDs)
                {
                    EducationalLevel level = _contentManager.GetComponent(childID) as EducationalLevel;
                    Image image = new Image()
                    {
                        Source = new BitmapImage(new Uri(level.Icon)),
                        RenderSize = new Size(100, 100),
                        MaxHeight = 100,
                        MaxWidth = 100,
                        Margin = new Thickness(15),
                        Opacity = 0.5,
                        Visibility = Visibility.Visible
                    };
                    image.MouseUp += delegate(object sender, MouseButtonEventArgs args)
                    {
                        ImageEducationaLevel_Click(sender as Image);
                    };
                    _educationalLevelDictionary[image.Source.ToString()] = childID;
                    WrapPanelEducationaLevel.Children.Add(image);
                    _selectedEducationalLevel = image;
                }
            }
            WrapPanelGrades.Children.Clear();
            NoOfLessons.Text = "0";
            NoOfFiles.Text = "0";
            NoOfWeeks.Text = "0";
        }

        private void ImageEducationaLevel_Click(Image sender)
        {
            if (_selectedEducationalLevel != null)
            {
                _selectedEducationalLevel.Opacity = 0.5;
            }
            sender.Opacity = 1;
            _selectedEducationalLevel = sender;
            int id = _educationalLevelDictionary[sender.Source.ToString()];
            EditEducationalLevel.IsEnabled = true;
            DeleteEducationalLevel.IsEnabled = true;
            AddGrade.IsEnabled = true;
            LoadGrades(id);
        }
        /// <summary>
        /// Add new Educational Level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddEducationalLevel_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Create educatinal level",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                LevelSelection level = new LevelSelection()
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                window.Width = level.Width + 15;
                window.Height = level.Height + 40;
                window.Content = level;
                level.button.Click += delegate (object obj, RoutedEventArgs Args)
                {
                    LevelSelection control = window.Content as LevelSelection;
                    string icon = control.GetSelectedIcon();
                    EducationalLevelType type = control.GetSelectedEducationalLevel();
                    if (icon != "")
                    {
                        if (_selectedLanguage != null)
                        {
                            int id = _contentManager.AddEducationalLevel(_courseImagesCache[_selectedLanguage.Source.ToString()], icon, type);
                            if (id > 0) //new edu level created
                            {
                                Image image = new Image()
                                {
                                    Source = new BitmapImage(new Uri(icon)),
                                    RenderSize = new Size(100, 100),
                                    MaxHeight = 100,
                                    MaxWidth = 100,
                                    Margin = new Thickness(15),
                                    Opacity = 0.5
                                };
                                image.MouseUp += delegate (object senderImage, MouseButtonEventArgs eventArgs)
                                {
                                    ImageEducationaLevel_Click(senderImage as Image);
                                };
                                if (!_educationalLevelDictionary.ContainsKey(icon))
                                {
                                    _educationalLevelDictionary[icon] = id;
                                    WrapPanelEducationaLevel.Children.Add(image);
                                    ImageEducationaLevel_Click(image);
                                    AddGrade.IsEnabled = true;
                                }
                                else
                                    image = null;
                            }// -1 - error; 0 - already exists
                        }
                    }
                    window.Close();
                };

            };
            window.ShowDialog();
        }

        private void EditEducationalLevel_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Create educatinal level",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                LevelSelection level = new LevelSelection()
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                window.Width = level.Width + 15;
                window.Height = level.Height + 40;
                window.Content = level;
                level.button.Click += delegate (object obj, RoutedEventArgs Args)
                {
                    LevelSelection control = window.Content as LevelSelection;
                    int id = _educationalLevelDictionary[_selectedEducationalLevel.Source.ToString()];
                    string icon = control.GetSelectedIcon();
                    EducationalLevelType type = control.GetSelectedEducationalLevel();
                    if (icon != "")
                    {
                        if (_selectedLanguage != null)
                        {
                            //_contentManager.AddEducationalLevel(_courseImagesCache[_selectedLanguage.Source.ToString()], icon, type);
                            _contentManager.EditEducationalLevel(id, icon, type);
                            _educationalLevelDictionary.Remove(_selectedEducationalLevel.Source.ToString());
                            _educationalLevelDictionary[icon] = id;
                            _selectedEducationalLevel.Source = new BitmapImage(new Uri(icon));
                        }
                    }
                    window.Close();
                };

            };
            window.ShowDialog();
        }

        private void DeleteEducationalLevel_Click(object sender, RoutedEventArgs e)
        {
            _contentManager.DeleteEducationalLevel(_educationalLevelDictionary[_selectedEducationalLevel.Source.ToString()],
                _courseImagesCache[_selectedLanguage.Source.ToString()]);
            _educationalLevelDictionary.Remove(_selectedEducationalLevel.Source.ToString());
            WrapPanelEducationaLevel.Children.Remove(_selectedEducationalLevel);
            _selectedEducationalLevel = null;
            WrapPanelGrades.Children.Clear();
            NoOfFiles.Text = "";
            NoOfLessons.Text = "";
            NoOfWeeks.Text = "";
            EditEducationalLevel.IsEnabled = false;
            DeleteEducationalLevel.IsEnabled = false;
            AddGrade.IsEnabled = false;
            EditGrade.IsEnabled = false;
            DeleteGrade.IsEnabled = false;
            EditWeeks.IsEnabled = false;
            EditLessons.IsEnabled = false;
            EditFiles.IsEnabled = false;
        }

        #endregion

        #region Grades
        
        private void LoadGrades(int id)
        {
            WrapPanelGrades.Children.Clear();
            List<int> childrenIDs = _contentManager.GetChildrenIDs(id);
            if (childrenIDs != null)
            {
                foreach (int childID in childrenIDs)
                {
                    Grade grade = _contentManager.GetComponent(childID) as Grade;
                    Image image = new Image()
                    {
                        Source = new BitmapImage(new Uri(grade.Icon)),
                        RenderSize = new Size(100, 100),
                        MaxHeight = 100,
                        MaxWidth = 100,
                        Margin = new Thickness(15),
                        Opacity = 0.5,
                        Visibility = Visibility.Visible
                    };
                    image.MouseUp += delegate (object sender, MouseButtonEventArgs args)
                    {
                        ImageGrade_Click(sender as Image);
                    };
                    _gradesDictionary[image.Source.ToString()] = childID;
                    WrapPanelGrades.Children.Add(image);
                    _selectedGrade = image;
                }
            }
            NoOfLessons.Text = "0";
            NoOfFiles.Text = "0";
            NoOfWeeks.Text = "0";
        }

        private void ImageGrade_Click(Image image)
        {
            if (_selectedGrade != null)
            {
                _selectedGrade.Opacity = 0.5;
            }
            image.Opacity = 1;
            _selectedGrade = image;
            int id = _gradesDictionary[image.Source.ToString()];
            int num = _contentManager.GetNoOfWeeks(id);
            NoOfWeeks.Text = num.ToString();
            num = _contentManager.GetNoOfLessons(id);
            NoOfLessons.Text = num.ToString();
            if (num > 0)
                EditLessons.IsEnabled = true;
            num = _contentManager.GetNoOfFiles(id);
            NoOfFiles.Text = num.ToString();
            if (num > 0)
                EditFiles.IsEnabled = true;
            EditGrade.IsEnabled = true;
            DeleteGrade.IsEnabled = true;
            EditWeeks.IsEnabled = true;
        }
        /// <summary>
        /// Add new Grade
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddGrade_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Create educatinal level",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                if (_selectedEducationalLevel != null)
                {
                    EducationalLevelType type =
                    (_contentManager.GetComponent(_educationalLevelDictionary[_selectedEducationalLevel.Source.ToString()])
                        as EducationalLevel).Level;
                    GradeSelection grade = new GradeSelection(type)
                    {
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };
                    window.Width = grade.Width + 35;
                    window.Height = grade.Height + 40;
                    window.Content = grade;
                    grade.button.Click += delegate (object obj, RoutedEventArgs Args)
                    {
                        GradeSelection control = window.Content as GradeSelection;
                        string icon = control.GetSelectedIcon();
                        GradeType gradeType = control.GetSelectedGrade();
                        if (icon != "")
                        {
                            if (_selectedEducationalLevel != null)
                            {
                                int id = _contentManager.AddGrade(_educationalLevelDictionary[_selectedEducationalLevel.Source.ToString()], icon, gradeType);
                                if (id > 0) //new edu level created
                                {
                                    Image image = new Image()
                                    {
                                        Source = new BitmapImage(new Uri(icon)),
                                        RenderSize = new Size(100, 100),
                                        MaxHeight = 100,
                                        MaxWidth = 100,
                                        Margin = new Thickness(15),
                                        Opacity = 0.5
                                    };
                                    image.MouseUp += delegate (object senderImage, MouseButtonEventArgs eventArgs)
                                    {
                                        ImageGrade_Click(senderImage as Image);
                                    };
                                    if (!_gradesDictionary.ContainsKey(icon))
                                    {
                                        _gradesDictionary[icon] = id;
                                        WrapPanelGrades.Children.Add(image);
                                        ImageGrade_Click(image);
                                    }
                                    else
                                        image = null;
                                }// -1 - error; 0 - already exists
                            }
                        }
                        window.Close();
                    };
                }
                else
                    window.Close();
            };
            window.ShowDialog();
        }

        private void EditGrade_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Create educatinal level",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                if (_selectedEducationalLevel != null)
                {
                    EducationalLevelType type =
                    (_contentManager.GetComponent(_educationalLevelDictionary[_selectedEducationalLevel.Source.ToString()])
                        as EducationalLevel).Level;
                    GradeSelection grade = new GradeSelection(type)
                    {
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };
                    window.Width = grade.Width + 35;
                    window.Height = grade.Height + 40;
                    window.Content = grade;
                    grade.button.Click += delegate (object obj, RoutedEventArgs Args)
                    {
                        GradeSelection control = window.Content as GradeSelection;
                        int id = _gradesDictionary[_selectedGrade.Source.ToString()];
                        string icon = control.GetSelectedIcon();
                        GradeType gradeType = control.GetSelectedGrade();
                        if (icon != "")
                        {
                            if (_selectedEducationalLevel != null)
                            {
                                _contentManager.EditGrade(id, icon, gradeType);
                                _gradesDictionary.Remove(_selectedGrade.Source.ToString());
                                _gradesDictionary[icon] = id;
                                _selectedGrade.Source = new BitmapImage(new Uri(icon));
                            }
                        }
                        window.Close();
                    };
                }
                else
                    window.Close();
            };
            window.ShowDialog();
        }
        /// <summary>
        /// Delete existing grade
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteGrade_Click(object sender, RoutedEventArgs e)
        {
            int id = _gradesDictionary[_selectedGrade.Source.ToString()];
            _contentManager.DeleteGrade(id, _educationalLevelDictionary[_selectedEducationalLevel.Source.ToString()]);
            _gradesDictionary.Remove(_selectedGrade.Source.ToString());
            WrapPanelGrades.Children.Remove(_selectedGrade);
            _selectedGrade = null;
            EditGrade.IsEnabled = false;
            DeleteGrade.IsEnabled = false;
            NoOfFiles.Text = "";
            NoOfLessons.Text = "";
            NoOfWeeks.Text = "";
            EditWeeks.IsEnabled = false;
            EditLessons.IsEnabled = false;
            EditFiles.IsEnabled = false;
        }

        #endregion

        private void EditWeeks_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Create educatinal level",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                if (_selectedEducationalLevel != null)
                {
                    EducationalLevelType type =
                    (_contentManager.GetComponent(_educationalLevelDictionary[_selectedEducationalLevel.Source.ToString()])
                        as EducationalLevel).Level;
                    WeeksCreation weeksCreation = new WeeksCreation(_gradesDictionary[_selectedGrade.Source.ToString()])
                    {
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };
                    window.Width = weeksCreation.Width + 35;
                    window.Height = weeksCreation.Height + 40;
                    window.Content = weeksCreation;
                    weeksCreation.Button.Click += delegate (object obj, RoutedEventArgs Args)
                    {
                        WeeksCreation control = window.Content as WeeksCreation;
                        //string icon = control.GetSelectedIcon();
                        //GradeType gradeType = control.GetSelectedGrade();
                        //if (icon != "")
                        //{
                        //    if (_selectedEducationalLevel != null)
                        //    {
                        //        int id = _contentManager.AddGrade(_educationalLevelDictionary[_selectedEducationalLevel.Source.ToString()], icon, gradeType);
                        //        if (id > 0) //new edu level created
                        //        {
                        //            Image image = new Image()
                        //            {
                        //                Source = new BitmapImage(new Uri(icon)),
                        //                RenderSize = new Size(100, 100),
                        //                MaxHeight = 100,
                        //                MaxWidth = 100,
                        //                Margin = new Thickness(15),
                        //                Opacity = 0.5
                        //            };
                        //            image.MouseUp += delegate (object senderImage, MouseButtonEventArgs eventArgs)
                        //            {
                        //                ImageGrade_Click(senderImage as Image);
                        //            };
                        //            if (!_gradesDictionary.ContainsKey(icon))
                        //            {
                        //                _gradesDictionary[icon] = id;
                        //                WrapPanelGrades.Children.Add(image);
                        //                ImageGrade_Click(image);
                        //            }
                        //            else
                        //                image = null;
                        //        }// -1 - error; 0 - already exists
                        //    }
                        //}
                        window.Close();
                    };
                }
                else
                    window.Close();
            };
            window.Closing += delegate(object o, CancelEventArgs args)
            {
                //not good
                if (!(window.Content as WeeksCreation).IsSaved())
                {
                    args.Cancel = true;
                    MessageBox.Show("Progress is not saved!!!");
                }
            };
            window.ShowDialog();
        }

        private void EditLessons_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditFiles_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
