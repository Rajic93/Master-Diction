using Diction_Master___Library;
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
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly LanguagesDictionary _languages;
        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<string, long> _courseImagesCache;
        /// <summary>
        /// 
        /// </summary>
        private Image _selectedLanguage;
        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<string, long> _educationalLevelDictionary;
        /// <summary>
        /// 
        /// </summary>
        private Image _selectedEducationalLevel;
        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<string, long> _gradesDictionary;
        /// <summary>
        /// 
        /// </summary>
        private Image _selectedGrade;
        /// <summary>
        /// 
        /// </summary>
        private List<Component> _availableCourses;
        private List<Component> _availableEducationalLevels;
        private List<Component> _availableGrades;
        private List<KeyValuePair<long, int>> _availableTerms;

        public HomePage(ClientState state)
        {
            _courseImagesCache = new Dictionary<string, long>();
            _educationalLevelDictionary = new Dictionary<string, long>();
            _gradesDictionary = new Dictionary<string, long>();
            _availableCourses = state._enabledCourses;
            _availableEducationalLevels = state._enabledEduLevels;
            _availableGrades = state._enabledGrades;
            _availableTerms = state._enabledTerms;
            InitializeComponent();
            LoadCourses();
        }


        public void LoadCourses()
        {
            foreach (Course course in _availableCourses)
            {
                Image image = new Image()
                {
                    Source = new BitmapImage(new Uri(course.Icon, UriKind.Relative)),
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

        private void ImageCourse_Click(Image image)
        {
            if (_selectedLanguage != null)
            {
                _selectedLanguage.Opacity = 0.5;
            }
            image.Opacity = 1;
            _selectedLanguage = image;
            LoadEducationalLevels();
        }

        private void LoadEducationalLevels()
        {
            WrapPanelEducationaLevel.Children.Clear();
            if (_availableEducationalLevels != null)
            {
                foreach (EducationalLevel child in _availableEducationalLevels)
                {
                    if (child.ParentID == _courseImagesCache[_selectedLanguage.Name])
                    {
                        Image image = new Image()
                        {
                            Source = new BitmapImage(new Uri(child.Icon, UriKind.Relative)),
                            RenderSize = new Size(100, 100),
                            MaxHeight = 100,
                            MaxWidth = 100,
                            Margin = new Thickness(15, 45, 15, 15),
                            Opacity = 0.5,
                            Name = "Icon_" + child.ID
                        };
                        image.MouseUp += delegate (object sender, MouseButtonEventArgs args)
                        {
                            ImageEducationaLevel_Click(sender as Image);
                        };
                        if (!_educationalLevelDictionary.ContainsKey(image.Name))
                        {
                            _educationalLevelDictionary[image.Name] = child.ID;
                        } 
                        WrapPanelEducationaLevel.Children.Add(image);
                    }
                    //_selectedEducationalLevel = image;
                }
            }
            WrapPanelGrades.Children.Clear();
        }

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
                LoadGrades();
            }
        }

        private void LoadGrades()
        {
            WrapPanelGrades.Children.Clear();
            if (_availableGrades != null)
            {
                foreach (Grade child in _availableGrades)
                {
                    if (child.ParentID == _educationalLevelDictionary[_selectedEducationalLevel.Name])
                    {
                        Image image = new Image()
                        {
                            Source = new BitmapImage(new Uri(child.Icon, UriKind.Relative)),
                            RenderSize = new Size(100, 100),
                            MaxHeight = 100,
                            MaxWidth = 100,
                            Margin = new Thickness(15),
                            Opacity = 0.5,
                            Name = "Icon_" + child.ID
                        };
                        image.MouseUp += delegate (object sender, MouseButtonEventArgs args)
                        {
                            ImageGrade_Click(sender as Image);
                        };
                        if (!_gradesDictionary.ContainsKey(image.Name))
                        {
                            _gradesDictionary[image.Name] = child.ID;
                        }
                        WrapPanelGrades.Children.Add(image);  
                    }
                }
            }
        }

        private void ImageGrade_Click(Image image)
        {
            if (_selectedGrade != null)
            {
                _selectedGrade.Opacity = 0.5;
            }
            image.Opacity = 1;
            _selectedGrade = image;
        }

        public long GetGradeID()
        {
            return _gradesDictionary[_selectedGrade.Name];
        }

    }
}
