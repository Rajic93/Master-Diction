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
using Component = Diction_Master___Library.Component;

namespace Diction_Master___Server
{
    /// <summary>
    /// Interaction logic for Content.xaml
    /// </summary>
    public partial class Content : Window
    {
        private Diction_Master___Library.ContentManager _contentManager;
        private Dictionary<string, int> _componentsCache;
        private Dictionary<Image, Component> courses;
        public ObservableCollection<Image> _courses;
        private int _chosenCourse;
        private ObservableCollection<Image> _levels;
        private int _chosenLevel;
        private ObservableCollection<Image> _grades;
        private int _chosenGrade;

        public Content()
        {
            _courses = new ObservableCollection<Image>();
            _levels = new ObservableCollection<Image>();
            _grades = new ObservableCollection<Image>();
            courses = new Dictionary<Image, Component>();
            _contentManager = Diction_Master___Library.ContentManager.CreateInstance();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _componentsCache = _contentManager.GetCourses();
            foreach (KeyValuePair<string, int> course in _componentsCache)
            {
                Button btn = new Button();
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("./Resources/Flag of " + course.Key));
                btn.Content = img;
                btn.Width = 50;
                btn.Height = 50;
            }
            listView.ItemsSource = _courses;
        }

        private void Button6_OnClick(object sender, RoutedEventArgs e)
        {
            string nation = "";
            string icon = "";
            LanguageSelection language = new LanguageSelection();
            Window window = new Window()
            {
                Title = "Create course",
                Content = language,
                
            };
            window.SizeChanged += delegate(object o, SizeChangedEventArgs args)
            {
            };
            language.button.Click += delegate (object o, RoutedEventArgs args)
            {
                Course newCourse = (Course)_contentManager.CreateComponent(ComponentType.Course);
                LanguageSelection control = (LanguageSelection)window.Content;
                nation = control.GetSelectedNation();
                newCourse.Name = nation;
                Image image = new Image()
                {
                    Source = new BitmapImage(new Uri("./Resources/Flag of " + nation + ".png", UriKind.Relative)),
                    RenderSize = new Size(100, 100)
                };
                newCourse.Icon = control.GetSelectedLanguage().Source.ToString();
                courses.Add(image, newCourse);
                _courses.Add(image);
                listView.Items.Refresh();
            };
            window.ShowDialog();
        }
    }
}
