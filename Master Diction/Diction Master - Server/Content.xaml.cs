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
using System.Windows.Shapes;

namespace Diction_Master___Server
{
    /// <summary>
    /// Interaction logic for Content.xaml
    /// </summary>
    public partial class Content : Window
    {
        private Diction_Master___Library.ContentManager contentManager;
        private Dictionary<string, int> coursesCache;

        public Content()
        {
            contentManager = Diction_Master___Library.ContentManager.CreateInstance();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            coursesCache = contentManager.GetCourses();
            foreach (KeyValuePair<string, int> course in coursesCache)
            {
                Button btn = new Button();
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("./Resources/Flag of " + course.Key));
                btn.Content = img;
                btn.Width = 50;
                btn.Height = 50;
                Courses.Children.Add(btn);
            }
        }

        private void Button6_OnClick(object sender, RoutedEventArgs e)
        {
            ContentManager createCourse = new ContentManager(new MainWindow());
            createCourse.Show();
            Hide();
        }
    }
}
