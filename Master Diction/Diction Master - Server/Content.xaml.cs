using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
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
        /// <summary>
        /// 
        /// </summary>
        private readonly Diction_Master___Library.ContentManager _contentManager;
        //Course
        /// <summary>
        /// 
        /// </summary>
        private readonly LanguagesHashTable _languages;
        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<string, int> _courseImagesCache;
        /// <summary>
        /// 
        /// </summary>
        private Image _selectedLanguage;
        //Educational Level
        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<string, int> _educationalLevelDictionary;
        /// <summary>
        /// 
        /// </summary>
        private Image _selectedEducationalLevel;
        //Grades
        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<string, int> _gradesDictionary;
        /// <summary>
        /// 
        /// </summary>
        private Image _selectedGrade;

        public Content(Diction_Master___Library.ContentManager manager)
        {
            _contentManager = manager;
            _languages = new LanguagesHashTable();
            _courseImagesCache = new Dictionary<string, int>();
            _educationalLevelDictionary = new Dictionary<string, int>();
            _gradesDictionary = new Dictionary<string, int>();
            InitializeComponent();
            LoadCourses();
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

            //_contentManager.AddQuestion(6, "How old are you?", "I am 24 years old.", QuestionType.Puzzle);
            //_contentManager.EditQuestion(7, "How old are you", "I am 24 years old.", QuestionType.Puzzle);
            //_contentManager.DeleteQuestion(7, 6);

            _contentManager.AddContentFile(5, ComponentType.Audio, "", "", "", 2, "HEHEHE");
            //_contentManager.EditContentFile(8, ComponentType.Audio, "", "", "", 2, "HEHEHE");
        }
        

        #region Course

        /// <summary>
        /// 
        /// </summary>
        public void LoadCourses()
        {
            foreach (Course course in _contentManager.GetAllCourses())
            {
                Image image = new Image()
                {
                    Source = new BitmapImage(new Uri(course.Icon)),
                    RenderSize = new Size(100, 100),
                    MaxHeight = 100,
                    MaxWidth = 100,
                    Margin = new Thickness(10),
                    Opacity = 0.5,
                    Name = "Image_" + course.ID
                };
                image.MouseUp += delegate (object senderImage, MouseButtonEventArgs eventArgs)
                {
                    ImageCourse_Click(senderImage as Image);
                };
                if (!_courseImagesCache.ContainsKey(image.Name))
                {
                    _courseImagesCache[image.Name] = course.ID;
                    Courses.Children.Add(image);
                    
                    
                }
            }
        }
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
                                Opacity = 0.5,
                                Name = "Image_" + id
                            };
                            image.MouseUp += delegate (object senderImage, MouseButtonEventArgs eventArgs)
                            {
                                ImageCourse_Click(senderImage as Image);
                            };
                            if (!_courseImagesCache.ContainsKey(image.Name))
                            {
                                _courseImagesCache[image.Name] = id;
                                Courses.Children.Add(image);
                                ImageCourse_Click(image);
                                AddEducationalLevel.IsEnabled = true;
                                EditCourse.IsEnabled = true;
                                DeleteCourse.IsEnabled = true;
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
            bool exitCancel = false;
            bool normalExit = true;
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
                    int id = _courseImagesCache[_selectedLanguage.Name];
                    string nation = control.GetSelectedNation();
                    string selectedLanguage = _languages.GetLanguageName(nation);
                    if (selectedLanguage != null)
                    {
                        if (!ExistingCourse(control.GetSelectedLanguageIcon()))
                        {
                            _courseImagesCache.Remove(_selectedLanguage.Name);
                            _courseImagesCache[control.GetSelectedLanguageIcon()] = id;
                            _contentManager.EditCourse(id, selectedLanguage, control.GetSelectedLanguageIcon());
                            _selectedLanguage.Source = new BitmapImage(new Uri(control.GetSelectedLanguageIcon()));
                            exitCancel = false;
                            normalExit = false;
                            window.Close();
                        }
                        else
                        {
                            MessageBox.Show("Course already exists!");
                            exitCancel = true;
                            normalExit = true;
                        }
                    }
                };
            };
            window.Closing += delegate(object o, CancelEventArgs args)
            {
                if (!normalExit)
                {
                    if (exitCancel)
                    {
                        args.Cancel = true;
                    }
                }
            };
            window.ShowDialog();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        private bool ExistingCourse(string icon)
        {
            foreach (Image image in Courses.Children)
                if (image.Source.ToString() == icon)
                    return true;
            return false;
        }
        /// <summary>
        /// Delete existing course
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCourse_Click(object sender, RoutedEventArgs e)
        {
            int id = _courseImagesCache[_selectedLanguage.Name];
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        private void ImageCourse_Click(Image image)
        {
            if (_selectedLanguage != null)
            {
                _selectedLanguage.Opacity = 0.5;
            }
            image.Opacity = 1;
            _selectedLanguage = image;
            int id = _courseImagesCache[image.Name];
            EditCourse.IsEnabled = true;
            DeleteCourse.IsEnabled = true;
            AddEducationalLevel.IsEnabled = true;
            WrapPanelEducationaLevel.Children.Clear();
            LoadEducationalLevels(id);
        }

        #endregion

        #region EducationalLevel
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
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
                        Visibility = Visibility.Visible,
                        Name = "Icon_" + level.ID
                    };
                    image.MouseUp += delegate(object sender, MouseButtonEventArgs args)
                    {
                        ImageEducationaLevel_Click(sender as Image);
                    };
                    _educationalLevelDictionary[image.Name] = childID;
                    WrapPanelEducationaLevel.Children.Add(image);
                    _selectedEducationalLevel = image;
                }
            }
            WrapPanelGrades.Children.Clear();
            NoOfLessons.Text = "0";
            NoOfFiles.Text = "0";
            NoOfWeeks.Text = "0";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        private void ImageEducationaLevel_Click(Image sender)
        {
            if (_selectedEducationalLevel != null)
            {
                _selectedEducationalLevel.Opacity = 0.5;
            }
            sender.Opacity = 1;
            _selectedEducationalLevel = sender;
            if (_educationalLevelDictionary.ContainsKey(sender.Name))
            {
                int id = _educationalLevelDictionary[sender.Name];
                EditEducationalLevel.IsEnabled = true;
                DeleteEducationalLevel.IsEnabled = true;
                AddGrade.IsEnabled = true;
                WrapPanelGrades.Children.Clear();
                LoadGrades(id);
            }
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
                            int id = _contentManager.AddEducationalLevel(_courseImagesCache[_selectedLanguage.Name], icon, type);
                            if (id > 0) //new edu level created
                            {
                                Image image = new Image()
                                {
                                    Source = new BitmapImage(new Uri(icon)),
                                    RenderSize = new Size(100, 100),
                                    MaxHeight = 100,
                                    MaxWidth = 100,
                                    Margin = new Thickness(15),
                                    Opacity = 0.5,
                                    Name = "Icon_" + id
                                };
                                image.MouseUp += delegate (object senderImage, MouseButtonEventArgs eventArgs)
                                {
                                    ImageEducationaLevel_Click(senderImage as Image);
                                };
                                if (!_educationalLevelDictionary.ContainsKey(image.Name))
                                {
                                    _educationalLevelDictionary[image.Name] = id;
                                    WrapPanelEducationaLevel.Children.Add(image);
                                    ImageEducationaLevel_Click(image);
                                    AddGrade.IsEnabled = true;
                                    EditEducationalLevel.IsEnabled = true;
                                    DeleteEducationalLevel.IsEnabled = true;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditEducationalLevel_Click(object sender, RoutedEventArgs e)
        {
            bool exitCancel = false;
            bool normalExit = true;
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
                    int id = _educationalLevelDictionary[_selectedEducationalLevel.Name];
                    string icon = control.GetSelectedIcon();
                    EducationalLevelType type = control.GetSelectedEducationalLevel();
                    if (icon != "")
                    {
                        if (_selectedLanguage != null)
                        {
                            if (!ExistingEducationalLevel(_selectedEducationalLevel.Name))
                            {
                                _contentManager.EditEducationalLevel(id, icon, type);
                                _selectedEducationalLevel.Source = new BitmapImage(new Uri(icon));
                                exitCancel = false;
                                normalExit = false;
                                window.Close();
                            }
                            else
                            {
                                MessageBox.Show("Educational level already exists!");
                                exitCancel = true;
                                normalExit = true;
                            }
                        }
                    }
                };

            };
            window.Closing += delegate(object o, CancelEventArgs args)
            {
                if (!normalExit)
                {
                    if (exitCancel)
                    {
                        args.Cancel = true;
                    }
                }
            };
            window.ShowDialog();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        private bool ExistingEducationalLevel(string icon)
        {
            foreach (Image image in WrapPanelEducationaLevel.Children)
                if (image.Name == icon)
                    return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteEducationalLevel_Click(object sender, RoutedEventArgs e)
        {
            _contentManager.DeleteEducationalLevel(_educationalLevelDictionary[_selectedEducationalLevel.Name],
                _courseImagesCache[_selectedLanguage.Name]);
            _educationalLevelDictionary.Remove(_selectedEducationalLevel.Name);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
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
                    _gradesDictionary[image.Name] = childID;
                    WrapPanelGrades.Children.Add(image);
                    _selectedGrade = image;
                }
            }
            NoOfLessons.Text = "0";
            NoOfFiles.Text = "0";
            NoOfWeeks.Text = "0";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        private void ImageGrade_Click(Image image)
        {
            if (_selectedGrade != null)
            {
                _selectedGrade.Opacity = 0.5;
            }
            image.Opacity = 1;
            _selectedGrade = image;
            int id = _gradesDictionary[image.Name];
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
                Title = "Create grade",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                if (_selectedEducationalLevel != null)
                {
                    EducationalLevelType type =
                    (_contentManager.GetComponent(_educationalLevelDictionary[_selectedEducationalLevel.Name])
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
                                int id = _contentManager.AddGrade(_educationalLevelDictionary[_selectedEducationalLevel.Name], icon, gradeType);
                                if (id > 0) //new edu level created
                                {
                                    Image image = new Image()
                                    {
                                        Source = new BitmapImage(new Uri(icon)),
                                        RenderSize = new Size(100, 100),
                                        MaxHeight = 100,
                                        MaxWidth = 100,
                                        Margin = new Thickness(15),
                                        Opacity = 0.5,
                                        Name = "Image_" + id
                                    };
                                    image.MouseUp += delegate (object senderImage, MouseButtonEventArgs eventArgs)
                                    {
                                        ImageGrade_Click(senderImage as Image);
                                    };
                                    if (!_gradesDictionary.ContainsKey(image.Name))
                                    {
                                        _gradesDictionary[image.Name] = id;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditGrade_Click(object sender, RoutedEventArgs e)
        {
            bool exitCancel = false;
            bool normalExit = true;
            Window window = new Window()
            {
                Title = "Create grade",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                if (_selectedEducationalLevel != null)
                {
                    EducationalLevelType type =
                    (_contentManager.GetComponent(_educationalLevelDictionary[_selectedEducationalLevel.Name])
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
                        int id = _gradesDictionary[_selectedGrade.Name];
                        string icon = control.GetSelectedIcon();
                        GradeType gradeType = control.GetSelectedGrade();
                        if (icon != "")
                        {
                            if (_selectedEducationalLevel != null)
                            {
                                if (!ExistingGrade(_selectedGrade.Name))
                                {
                                    _contentManager.EditGrade(id, icon, gradeType);
                                    _selectedGrade.Source = new BitmapImage(new Uri(icon));
                                    exitCancel = false;
                                    normalExit = false;
                                    window.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Grade already exists!");
                                    exitCancel = true;
                                    normalExit = true;
                                }
                            }
                        }
                    };
                }
                else
                    window.Close();
            };
            window.Closing += delegate(object o, CancelEventArgs args)
            {
                if (!normalExit)
                {
                    if (exitCancel)
                    {
                        args.Cancel = true;
                    }
                }
            };
            window.ShowDialog();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        private bool ExistingGrade(string icon)
        {
            foreach (Image image in WrapPanelGrades.Children)
                if (image.Name == icon)
                    return true;
            return false;
        }
        /// <summary>
        /// Delete existing grade
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteGrade_Click(object sender, RoutedEventArgs e)
        {
            int id = _gradesDictionary[_selectedGrade.Name];
            _contentManager.DeleteGrade(id, _educationalLevelDictionary[_selectedEducationalLevel.Name]);
            _gradesDictionary.Remove(_selectedGrade.Name);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditWeeks_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Edit weeks",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                if (_selectedEducationalLevel != null)
                {
                    WeeksCreation weeksCreation = new WeeksCreation(_gradesDictionary[_selectedGrade.Name], _contentManager)
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
                        if (!control.IsEmpty() && control.IsSaved())
                        {
                            int num = _contentManager.GetNoOfWeeks(_gradesDictionary[_selectedGrade.Name]);
                            NoOfWeeks.Text = num.ToString();
                            if (num > 0)
                                EditLessons.IsEnabled = true;
                        }
                        window.Close();
                    };
                }
                else
                    window.Close();
            };
            window.Closing += delegate(object o, CancelEventArgs args)
            {
                //not good
                bool empty = (window.Content as WeeksCreation).IsEmpty();
                bool ctrl = (window.Content as WeeksCreation).IsSaved();
                if (!empty && !ctrl)
                {
                    args.Cancel = true;
                    MessageBox.Show("Progress is not saved!!!");
                }
            };
            window.ShowDialog();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditLessons_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Edit lessons",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                if (_selectedEducationalLevel != null)
                {
                    LessonsCreation lessonsCreation = new LessonsCreation(_gradesDictionary[_selectedGrade.Name], _contentManager, false)
                    {
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };
                    window.Width = lessonsCreation.Width + 35;
                    window.Height = lessonsCreation.Height + 40;
                    window.Content = lessonsCreation;
                    lessonsCreation.Button.Click += delegate (object obj, RoutedEventArgs Args)
                    {
                        LessonsCreation control = window.Content as LessonsCreation;
                        if (!control.IsEmpty() && control.IsSaved())
                        {
                            int num = _contentManager.GetNoOfLessons(_gradesDictionary[_selectedGrade.Name]);
                            NoOfLessons.Text = num.ToString();
                            if (num > 0)
                                EditFiles.IsEnabled = true;
                        }
                        window.Close();
                    };
                }
                else
                    window.Close();
            };
            window.Closing += delegate (object o, CancelEventArgs args)
            {
                //not good
                bool empty = (window.Content as LessonsCreation).IsEmpty();
                bool ctrl = (window.Content as LessonsCreation).IsSaved();
                if (!empty && !ctrl)
                {
                    args.Cancel = true;
                    MessageBox.Show("Progress is not saved!!!");
                }
            };
            window.ShowDialog();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditFiles_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Edit content",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                if (_selectedEducationalLevel != null)
                {
                    ContentUpload contentUpload= new ContentUpload(_gradesDictionary[_selectedGrade.Name], _contentManager, false)
                    {
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };
                    window.Width = contentUpload.Width + 35;
                    window.Height = contentUpload.Height + 40;
                    window.Content = contentUpload;
                    contentUpload.Button.Click += delegate (object obj, RoutedEventArgs Args)
                    {
                        if (contentUpload.IsSaved())
                        {
                            NoOfFiles.Text = contentUpload.GetNoOfFiles();
                            window.Close();
                        }
                    };
                }
                else
                    window.Close();
            };
            window.Closing += delegate (object o, CancelEventArgs args)
            {
                if (!(window.Content as ContentUpload).IsSaved())
                {
                    args.Cancel = true;
                    MessageBox.Show("Progress is not saved!");
                }
            };
            window.ShowDialog();
        }
    }
}
