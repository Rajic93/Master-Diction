using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Diction_Master___Library;

namespace Diction_Master___Server.Custom_Controls
{
    /// <summary>
    /// Interaction logic for LessonsCreation.xaml
    /// </summary>
    public partial class LessonsCreation : UserControl
    {
        private Diction_Master___Library.ContentManager manager;
        private bool savedLessons;
        private bool firstStart = true;
        private Component loadedWeek;
        private ObservableCollection<Week> weeks;
        private ObservableCollection<Lesson> lessons;

        public LessonsCreation()
        {
            manager = Diction_Master___Library.ContentManager.CreateInstance();
            lessons = new ObservableCollection<Lesson>();
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                if (savedLessons || firstStart)
                {
                    lessons.Clear();
                    foreach (Lesson lesson in ((Week)listBox.SelectedItem).Components)
                    {
                        lessons.Add(lesson);
                    }
                }
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            listBox1.ItemsSource = lessons;
            listBox1.DisplayMemberPath = "Title";
        }

        public void LoadWeeks(Component component)
        {
            weeks = manager.GetAllWeeks(component);
            listBox.ItemsSource = weeks;
            listBox.DisplayMemberPath = "Title";
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textBox1.Text = ((Week) listBox.SelectedItem).ID.ToString();
            textBox2.Text = ((Week) listBox.SelectedItem).Num.ToString();
            textBox3.Text = ((Week) listBox.SelectedItem).Term.ToString();
            textBox4.Text = ((Week) listBox.SelectedItem).Title;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            savedLessons = true;
        }
    }
}
