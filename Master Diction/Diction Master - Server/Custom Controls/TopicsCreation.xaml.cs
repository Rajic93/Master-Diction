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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Diction_Master___Library;
using Component = Diction_Master___Library.Component;

namespace Diction_Master___Server.Custom_Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TopicsCreation : UserControl
    {
        private readonly Diction_Master___Library.ContentManager _contentManager;
        private readonly ObservableCollection<Component> _topics;
        private bool _saved = true;
        private bool _empty = true;
        
        public TopicsCreation(Diction_Master___Library.ContentManager manager)
        {
            _contentManager = manager;
            _topics = new ObservableCollection<Component>();
            InitializeComponent();
            _topics = _contentManager.GetAllTopics();
            listBox.ItemsSource = _topics;
            listBox.DisplayMemberPath = "Title";
            for (int i = 0; i < 100; i++)
            {
                comboBox.Items.Add(i);
            }
            comboBox.Text = 1.ToString();
        }

        private void listBoxTerm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                textBox.Text = ((Topic)listBox.SelectedItem).Title;
                comboBox.Text = ((Topic)listBox.SelectedItem).Num.ToString();
                Edit.IsEnabled = true;
                Delete.IsEnabled = true;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text != "")
            {
                int id = _contentManager.AddTopic(textBox.Text, Convert.ToInt16(comboBox.SelectedValue));
                if (id > 0)
                {
                    _topics.Add(_contentManager.GetComponent(id) as Topic);
                    listBox.Items.Refresh();
                    _saved = false;
                    Confirm.IsEnabled = true;
                    _empty = false;
                    EditLessons.IsEnabled = true;
                }
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                ((Topic)listBox.SelectedItem).Title = textBox.Text;
                ((Topic)listBox.SelectedItem).Num = Convert.ToInt16(comboBox.SelectedValue);
                listBox.Items.Refresh();
                _saved = false;
                Confirm.IsEnabled = true;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                _contentManager.DeleteTopic((listBox.SelectedItem as Topic).ID);
                _topics.Remove(listBox.SelectedItem as Topic);
                textBox.Text = "";
                _saved = false;
                Edit.IsEnabled = false;
                Delete.IsEnabled = false;
                Confirm.IsEnabled = true;
                if (_topics.Count == 0)
                {
                    _empty = true;
                    EditFiles.IsEnabled = false;
                    EditLessons.IsEnabled = false;
                }
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            _saved = true;
            Confirm.IsEnabled = false;
        }

        public bool IsEmpty()
        {
            return _empty;
        }

        public bool IsSaved()
        {
            return _saved;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Edit lessons",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                    LessonsCreation lessonsCreation = new LessonsCreation(0, _contentManager, true)
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
                            int num = _contentManager.GetNoOfTopicsLessons();
                            NoOfLessons.Text = num.ToString();
                            if (num > 0)
                                EditFiles.IsEnabled = true;
                        }
                        window.Close();
                    };
            };
            window.Closing += delegate (object o, CancelEventArgs args)
            {
                if (window.Content != null)
                {
                    bool empty = (window.Content as LessonsCreation).IsEmpty();
                    bool ctrl = (window.Content as LessonsCreation).IsSaved();
                    if (!empty && !ctrl)
                    {
                        args.Cancel = true;
                        MessageBox.Show("Progress is not saved!!!");
                    }
                }
            };
            window.ShowDialog();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Edit content",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                ContentUpload contentUpload = new ContentUpload(0, _contentManager, true)
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                window.Width = contentUpload.Width + 35;
                window.Height = contentUpload.Height + 40;
                window.Content = contentUpload;
                contentUpload.Button.Click += delegate(object obj, RoutedEventArgs Args)
                {
                    if (contentUpload.IsSaved())
                    {
                        NoOfFiles.Text = contentUpload.GetNoOfFiles();
                        window.Close();
                    }
                };
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
