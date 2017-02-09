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
        private readonly LanguagesHashTable _languages;
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

        public HomePage(List<Component> courses)
        {
            _availableCourses = courses;
            InitializeComponent();
            LoadCourses();
        }


        public void LoadCourses()
        {
            foreach (Course course in _availableCourses)
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

        private void ImageCourse_Click(Image image)
        {
            if (_selectedLanguage != null)
            {
                _selectedLanguage.Opacity = 0.5;
            }
            image.Opacity = 1;
            _selectedLanguage = image;
            long id = _courseImagesCache[image.Name];
            WrapPanelEducationaLevel.Children.Clear();
            LoadEducationalLevels(id);
        }

        private void LoadEducationalLevels(long id)
        {
            WrapPanelEducationaLevel.Children.Clear();
            List<EducationalLevel> children = GetChildrenEduLevels();
            if (children != null)
            {
                foreach (EducationalLevel child in children)
                {
                    Image image = new Image()
                    {
                        Source = new BitmapImage(new Uri(child.Icon)),
                        RenderSize = new Size(100, 100),
                        MaxHeight = 100,
                        MaxWidth = 100,
                        Margin = new Thickness(15),
                        Opacity = 0.5,
                        Visibility = Visibility.Visible,
                        Name = "Icon_" + child.ID
                    };
                    image.MouseUp += delegate (object sender, MouseButtonEventArgs args)
                    {
                        ImageEducationaLevel_Click(sender as Image);
                    };
                    _educationalLevelDictionary[image.Name] = child.ID;
                    WrapPanelEducationaLevel.Children.Add(image);
                    _selectedEducationalLevel = image;
                }
            }
            WrapPanelGrades.Children.Clear();
        }

        private List<EducationalLevel> GetChildrenEduLevels()
        {
            List<EducationalLevel> children = new List<EducationalLevel>();
            foreach (EducationalLevel level in (_availableCourses.Find(x => x.ID == _courseImagesCache[_selectedLanguage.Name]) as CompositeComponent).Components)
            {
                children.Add(level);
            }
            return children;
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
                long id = _educationalLevelDictionary[sender.Name];
                WrapPanelGrades.Children.Clear();
                LoadGrades(id);
            }
        }

        private void LoadGrades(long id)
        {
            WrapPanelGrades.Children.Clear();
            List<Grade> children = GetChildrenGrades();
            if (children != null)
            {
                foreach (Grade child in children)
                {
                    Image image = new Image()
                    {
                        Source = new BitmapImage(new Uri(child.Icon)),
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
                    _gradesDictionary[image.Name] = child.ID;
                    WrapPanelGrades.Children.Add(image);
                    _selectedGrade = image;
                }
            }
        }

        private List<Grade> GetChildrenGrades()
        {
            List<Grade> grades = new List<Grade>();
            EducationalLevel level = (_availableCourses.Find(x => x.ID == _courseImagesCache[_selectedLanguage.Name]) as CompositeComponent)
                .Components.Find(g => g.ID == _educationalLevelDictionary[_selectedEducationalLevel.Name]) as EducationalLevel;
            foreach (Grade grade in level.Components)
            {
                grades.Add(grade);
            }
            return grades;
        }

        private void ImageGrade_Click(Image image)
        {
            if (_selectedGrade != null)
            {
                _selectedGrade.Opacity = 0.5;
            }
            image.Opacity = 1;
            _selectedGrade = image;
            long id = _gradesDictionary[image.Name];
        }

        public long GetGradeID()
        {
            return _gradesDictionary[_selectedGrade.Name];
        }

    }
}
