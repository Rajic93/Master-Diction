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
        private Diction_Master___Library.ContentManager _contentManager;
        private bool savedLessons = true;
        private bool empty = true;
        private Component loadedWeek;
        private ObservableCollection<Week> weeks;
        private ObservableCollection<Component> lessons;
        private int numOfLessons = 0;

        private int _selectedGrade;

        public LessonsCreation(int parentID, Diction_Master___Library.ContentManager manager)
        {
            _contentManager = manager;
            _selectedGrade = parentID;
            lessons = new ObservableCollection<Component>();
            InitializeComponent();
            LoadWeeks();
        }

        public bool IsEmpty()
        {
            return numOfLessons == 0;
        }

        public bool IsSaved()
        {
            return savedLessons;
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                if (savedLessons || empty)
                {
                    lessons.Clear();
                    foreach (Lesson lesson in ((Week) listBox.SelectedItem).Components)
                    {
                        lessons.Add(lesson);
                    }
                    loadedWeek = (Week) listBox.SelectedItem;
                    listBox1.Items.Refresh();
                    Add.IsEnabled = true;
                    empty = true;
                }
                else
                {
                    MessageBox.Show("Progress is not saved!");
                }
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            listBox1.ItemsSource = lessons;
            listBox1.DisplayMemberPath = "Title";
        }

        public void LoadWeeks()
        {
            weeks = _contentManager.GetAllWeeks(_contentManager.GetComponent(_selectedGrade));
            numOfLessons = _contentManager.GetNoOfLessons(_selectedGrade);
            listBox.ItemsSource = weeks;
            listBox.DisplayMemberPath = "Title";
            listBox.Items.Refresh();
            for (int i = 0; i < 100; i++)
            {
                comboBox.Items.Add((i + 1).ToString());
            }
            comboBox.Text = 1.ToString();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textBox1.Text = ((Week) listBox.SelectedItem).ID.ToString();
            textBox2.Text = ((Week) listBox.SelectedItem).Num.ToString();
            textBox3.Text = ((Week) listBox.SelectedItem).Term.ToString();
            textBox4.Text = ((Week) listBox.SelectedItem).Title;
            Load.IsEnabled = true;
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                textBox.Text = ((Lesson)listBox1.SelectedItem).Title;
                comboBox.Text = ((Lesson)listBox1.SelectedItem).Num.ToString();
                Edit.IsEnabled = true;
                Delete.IsEnabled = true;
            }
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            int id = _contentManager.AddLesson(loadedWeek.ID, textBox.Text, Convert.ToInt16(comboBox.Text));
            if (id > 0)
            {
                lessons.Add(_contentManager.GetComponent(id));
                Confirm.IsEnabled = true;
                savedLessons = false;
                empty = false;
                numOfLessons++;
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                ((Lesson)listBox1.SelectedItem).Title = textBox.Text;
                ((Lesson)listBox1.SelectedItem).Num = Convert.ToInt16(comboBox.Text);
                listBox1.Items.Refresh();
                Confirm.IsEnabled = true;
                savedLessons = false;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                int id = (listBox1.SelectedItem as Lesson).ID;
                lessons.Remove((Lesson) listBox1.SelectedItem);
                _contentManager.DeleteLesson(id, loadedWeek.ID);
                listBox1.Items.Refresh();
                Confirm.IsEnabled = true;
                savedLessons = false;
                numOfLessons--;
                if (listBox1.Items.Count == 0)
                    empty = true;
                else
                    empty = false;
                Edit.IsEnabled = false;
                Delete.IsEnabled = false;
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            ((Week)loadedWeek).Components.Clear();
            foreach (Lesson lesson in lessons)
            {
                ((Week)loadedWeek).Components.Add(lesson);
            }
            savedLessons = true;
            Confirm.IsEnabled = false;
        }
    }
}
