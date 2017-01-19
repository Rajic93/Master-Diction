using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.IO;
using System.Security.AccessControl;
using Diction_Master___Library;
using Microsoft.Win32;

namespace Diction_Master___Server.Custom_Controls
{

    /// <summary>
    /// Interaction logic for ContentUpload.xaml
    /// </summary>
    public partial class ContentUpload : UserControl
    {
        public bool savedChanges = true;
        public ObservableCollection<Lesson> lessons;
        private ObservableCollection<LeafComponent> lessonContent;
        private Diction_Master___Library.ContentManager manager;

        public ContentUpload()
        {
            manager = Diction_Master___Library.ContentManager.CreateInstance();
            lessonContent = new ObservableCollection<LeafComponent>();
            //---------------------
            lessons = new ObservableCollection<Lesson>();
            //---------------------
            savedChanges = true;
            InitializeComponent();
        }

        private Diction_Master___Library.ComponentType CheckType(string selectedFile)
        {
            //.mp3,.mpc,.wma,.wav
            //.avi,.wmv,.mp4,.mpeg
            //.doc,.docx,.txt,.pdf
            return ComponentType.Document;
        }

        //testing function
        public void LoadWeeks()
        {
            listBox.ItemsSource = lessonContent;
            listBox.DisplayMemberPath = "Title";
            comboBox.ItemsSource = lessons;
            comboBox.DisplayMemberPath = "Title";
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            savedChanges = false;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            savedChanges = false;
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            var res = dialog.ShowDialog();
            if (res != null && res == true)
            {
                string selectedFile = dialog.FileName;
                switch (CheckType(selectedFile))
                {
                    case ComponentType.Audio:
                        radioButtonAudio.IsChecked = true;
                        break;
                    case ComponentType.Video:
                        radioButtonVideo.IsChecked = true;
                        break;
                    case ComponentType.Document:
                        radioButtonDocument.IsChecked = true;
                        break;
                }
                byte[] bytes = File.ReadAllBytes(selectedFile);
                string name = selectedFile.Split('\\').Last();
                TextBoxURI.Text = name;
                textBoxTitle.Text = name;
                textBoxDescription.Text = "";
                Directory.SetCurrentDirectory("tmp\\");
                File.WriteAllBytes(name, bytes);
                //next line is bad
                LeafComponent document = (LeafComponent)ContentFactory.CreateLeafComponent(1, 1, CheckType(selectedFile), name, name, 20, "", "");
                ((Lesson) comboBox.SelectedItem).Components.Add(document);
                lessonContent.Add(document);
                listBox.Items.Refresh();
            }
        }

        public string CreateNewArchive(string userId, string action)
        {
            string archiveName = "";
            try
            {
                string dirPath = ""; //this.GetDirPath(userId, action);
                //long currentTime = (Int32)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                //string timestamp = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
                FileInfo testExist = new FileInfo(System.IO.Path.Combine(dirPath, userId + ".zip"));
                //archiveName = ;
                while (testExist.Exists)
                {
                    //currentTime = (Int32)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    testExist = new FileInfo(System.IO.Path.Combine(dirPath, userId + ".zip"));
                }
                archiveName = System.IO.Path.Combine(dirPath, userId + ".zip");
                using (System.IO.File.Create(archiveName))
                {

                }
            }
            catch (Exception ex)
            {
                return "ERROR";
            }
            return archiveName;
        }

        private void Edit_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        public void LoadLessons(Component buildingCourse)
        {
            lessons = manager.GetAllLessons(buildingCourse);
            listBox.ItemsSource = lessonContent;
            listBox.DisplayMemberPath = "Title";
            comboBox.ItemsSource = lessons;
            comboBox.DisplayMemberPath = "Title";
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (savedChanges)
            {
                if (lessonContent.Count == 0)
                    lessonContent.Clear();
                Lesson lesson = ((Lesson) comboBox.SelectedItem);
                foreach (LeafComponent leafComponent in lesson.Components)
                {
                    lessonContent.Add(leafComponent);
                }
                listBox.Items.Refresh();
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (savedChanges)
            {
                textBoxTitle.Text = ((LeafComponent) listBox.SelectedItem).Title;
                TextBoxURI.Text = ((LeafComponent)listBox.SelectedItem).URI;
                textBoxDescription.Text = ((LeafComponent)listBox.SelectedItem).Description;
                Size.Text = ((LeafComponent)listBox.SelectedItem).Size.ToString();
                switch (((ContentFile)listBox.SelectedItem).ComponentType)
                {
                    case ComponentType.Audio:
                        radioButtonAudio.IsChecked = true;
                        break;
                    case ComponentType.Video:
                        radioButtonVideo.IsChecked = true;
                        break;
                    case ComponentType.Document:
                        radioButtonDocument.IsChecked = true;
                        break;
                }
            }
        }
    }
}
