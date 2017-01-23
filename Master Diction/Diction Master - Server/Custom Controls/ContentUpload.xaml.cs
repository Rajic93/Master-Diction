using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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
using Component = Diction_Master___Library.Component;
using Image = System.Windows.Controls.Image;
using Size = System.Windows.Size;

namespace Diction_Master___Server.Custom_Controls
{

    /// <summary>
    /// Interaction logic for ContentUpload.xaml
    /// </summary>
    public partial class ContentUpload : UserControl
    {
        public bool savedChanges = true;
        public ObservableCollection<Lesson> lessons;
        private ObservableCollection<Component> lessonContent;
        private Diction_Master___Library.ContentManager _contentManager;
        private Dictionary<string, int> _imagesCache;

        private int _selectedGrade;
        private Image _selectedFile;
        private bool _saved;

        public ContentUpload(int parent, Diction_Master___Library.ContentManager manager)
        {
            _contentManager = manager;
            _selectedGrade = parent;
            lessonContent = new ObservableCollection<Component>();
            _imagesCache = new Dictionary<string, int>();
            //---------------------
            lessons = new ObservableCollection<Lesson>();
            //---------------------
            savedChanges = true;
            InitializeComponent();
            LoadLessons();
        }

        private void LoadLessons()
        {
            lessons = _contentManager.GetAllLessons(_contentManager.GetComponent(_selectedGrade));
            comboBox.ItemsSource = lessons;
            comboBox.DisplayMemberPath = "Title";
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (savedChanges)
            {
                lessonContent.Clear();
                _imagesCache.Clear();
                Files.Children.Clear();
                if (comboBox.SelectedItem != null)
                {
                    Lesson lesson = ((Lesson)comboBox.SelectedItem);
                    foreach (Component leafComponent in lesson.Components)
                    {
                        lessonContent.Add(leafComponent);
                        Image image = new Image()
                        {
                            RenderSize = new Size(65, 65),
                            MaxHeight = 65,
                            MaxWidth = 65,
                            Margin = new Thickness(10),
                            Opacity = 0.5,
                            Visibility = Visibility.Visible
                        };
                        if (leafComponent.GetType().Name == "Quiz")
                        {
                            LoadImage(image, Properties.Resources.quiz);
                        }
                        else
                        {
                            switch ((leafComponent as ContentFile).ComponentType)
                            {
                                case ComponentType.Audio:
                                    LoadImage(image, Properties.Resources.audio);
                                    break;
                                case ComponentType.Video:
                                    LoadImage(image, Properties.Resources.video);
                                    break;
                                case ComponentType.Document:
                                    LoadImage(image, Properties.Resources.document);
                                    break;
                            }
                        }
                        image.Name = "Content_" + leafComponent.ID;
                        image.MouseUp += delegate(object o, MouseButtonEventArgs args)
                        {
                            if (_selectedFile != null)
                                _selectedFile.Opacity = 0.5;
                            _selectedFile = o as Image;
                            _selectedFile.Opacity = 1;
                            LoadContentInfo(_imagesCache[(o as Image).Name]);
                        };
                        _imagesCache[image.Name] = leafComponent.ID;
                        Files.Children.Add(image);
                    }
                    Add.IsEnabled = true;
                    AddQuiz.IsEnabled = true;
                }
            }
        }

        private static void LoadImage(Image image, Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                //bitmapimage.BaseUri = new Uri(image.Source.ToString());

                image.Source = bitmapimage;
            }
        }

        private void LoadContentInfo(int id)
        {
            Component content = _contentManager.GetComponent(id);
            if (content.GetType().Name == "ContentFile")
            {
                textBoxTitle.Text = ((ContentFile)content).Title;
                TextBoxURI.Text = ((ContentFile)content).URI;
                textBoxDescription.Text = ((ContentFile)content).Description;
                Size.Text = ((ContentFile)content).Size.ToString();
                switch (((ContentFile)content).ComponentType)
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
                Edit.IsEnabled = true;
                Delete.IsEnabled = true;
                DisableQuizInfo();
            }
            else if (content.GetType().Name == "Quiz")
            {
                textBoxQuizTitle.Text = (content as Quiz).Title;
                textBoxTitle.IsEnabled = true;
                EditQuiz.IsEnabled = true;
                DeleteQuiz.IsEnabled = true;
                DisableContentFileInfo();
            }
        }

        private void DisableQuizInfo()
        {
            textBoxQuizTitle.Text = "";
            textBoxQuizTitle.IsEnabled = false;
            EditQuiz.IsEnabled = false;
            DeleteQuiz.IsEnabled = false;
        }

        private void DisableContentFileInfo()
        {
            radioButtonAudio.IsChecked = false;
            radioButtonVideo.IsChecked = false;
            radioButtonDocument.IsChecked = false;
            textBoxTitle.Text = "";
            TextBoxURI.Text = "";
            textBoxDescription.Text = "";
            Size.Text = "";
            textBoxTitle.IsEnabled = false;
            TextBoxURI.IsEnabled = false;
            textBoxDescription.IsEnabled = false;
            Edit.IsEnabled = false;
            Delete.IsEnabled = false;
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedItem != null)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                var res = dialog.ShowDialog();
                if (res != null && res == true)
                {
                    string selectedFile = dialog.FileName;
                    byte[] bytes = File.ReadAllBytes(selectedFile);
                    string name = selectedFile.Split('\\').Last();
                    long size = new FileInfo(dialog.FileName).Length;
                    //change path
                    int id = _contentManager.AddContentFile(((Lesson)comboBox.SelectedItem).ID, CheckType(selectedFile), name, name, selectedFile, size, "");
                    if (id > 0)
                    {
                        string icon = "";
                        switch (CheckType(selectedFile))
                        {
                            case ComponentType.Audio:
                                radioButtonAudio.IsChecked = true;
                                icon = "pack://application:,,,/Resources/audio.png";
                                break;
                            case ComponentType.Video:
                                icon = "pack://application:,,,/Resources/video.png";
                                radioButtonVideo.IsChecked = true;
                                break;
                            case ComponentType.Document:
                                icon = "pack://application:,,,/Resources/document.png";
                                radioButtonDocument.IsChecked = true;
                                break;
                        }
                        //change path
                        _contentManager.EditContentFile(id, CheckType(selectedFile), icon, name, name, size, "");
                        Image image = new Image()
                        {
                            RenderSize = new Size(65, 65),
                            MaxHeight = 65,
                            MaxWidth = 65,
                            Margin = new Thickness(10),
                            Opacity = 1,
                            Source = new BitmapImage(new Uri(icon)),
                            Name = "Content_" + id
                        };
                        image.MouseUp += delegate (object o, MouseButtonEventArgs args)
                        {
                            if (_selectedFile != null)
                                _selectedFile.Opacity = 0.5;
                            _selectedFile = o as Image;
                            _selectedFile.Opacity = 1;
                            LoadContentInfo(_imagesCache[(o as Image).Name]);
                        };
                        _imagesCache[image.Name] = id;
                        Files.Children.Add(image);
                        if (_selectedFile != null)
                        {
                            _selectedFile.Opacity = 0.5;
                        }
                        _selectedFile = image;
                        Edit.IsEnabled = true;
                        Delete.IsEnabled = true;
                        //change URI
                        TextBoxURI.Text = name;
                        textBoxTitle.Text = name;
                        textBoxDescription.Text = "";

                        _saved = false;
                        Save.IsEnabled = true;
                    }
                }
            }
            else
            {
                MessageBox.Show("Lesson is not selected!!!");
            }
        }

        private Diction_Master___Library.ComponentType CheckType(string selectedFile)
        {
            string extension = selectedFile.Split('.').Last();
            if (extension == "mp3")
                return ComponentType.Audio;
            if (extension == "mpc")
                return ComponentType.Audio;
            if (extension == "wma")
                return ComponentType.Audio;
            if (extension == "wav")
                return ComponentType.Audio;
            if (extension == "avi")
                return ComponentType.Video;
            if (extension == "wmv")
                return ComponentType.Video;
            if (extension == "mp4")
                return ComponentType.Video;
            if (extension == "mpeg")
                return ComponentType.Video;
            if (extension == "doc")
                return ComponentType.Document;
            if (extension == "docx")
                return ComponentType.Document;
            if (extension == "txt")
                return ComponentType.Document;
            if (extension == "log")
                return ComponentType.Document;
            if (extension == "pdf")
                return ComponentType.Document;
            return ComponentType.Uknown;
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            savedChanges = false;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            savedChanges = false;
        }

        private void Edit_OnClick(object sender, RoutedEventArgs e)
        {
            if (_selectedFile != null)
            {
                ComponentType type;
                if (radioButtonDocument.IsChecked.Value)
                    type = ComponentType.Document;
                else if (radioButtonAudio.IsChecked.Value)
                    type = ComponentType.Audio;
                else
                    type = ComponentType.Video;
                _contentManager.EditContentFile(_imagesCache[_selectedFile.Name], type, "",
                    textBoxTitle.Text, TextBoxURI.Text, Size.Text!= "" ? Convert.ToInt64(Size.Text) : 0, textBoxDescription.Text);
                _saved = false;
                Save.IsEnabled = true;
            }
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            if (_selectedFile != null)
            {
                _contentManager.DeleteContentFile(_imagesCache[_selectedFile.Name], (comboBox.SelectedItem as Lesson).ID);
                Files.Children.Remove(_selectedFile);
                _imagesCache.Remove(_selectedFile.Name);
                _selectedFile = null;
                _saved = false;
                Save.IsEnabled = true;
                DisableContentFileInfo();
                DisableQuizInfo();
            }
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            _saved = true;
            Save.IsEnabled = false;
        }

        private void AddQuiz_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Add Quiz",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                QuizCreation quiz = new QuizCreation((comboBox.SelectedItem as Lesson).ID, _contentManager, false, 0);
                window.Height = quiz.Height + 45;
                window.Width = quiz.Width + 45;
                quiz.Button.Click += delegate (object sender1, RoutedEventArgs eventArgs)
                {
                    if (quiz.IsSaved())
                    {
                        if (quiz.NewQuizCreated())
                        {
                            int id = quiz.NewQuizID();
                            Quiz newQuiz = _contentManager.GetComponent(id) as Quiz;
                            newQuiz.Title = quiz.GetTitle();
                            Image image = new Image()
                            {
                                RenderSize = new Size(65, 65),
                                MaxHeight = 65,
                                MaxWidth = 65,
                                Margin = new Thickness(10),
                                Opacity = 1,
                                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/quiz.png")),
                                Name = "Content_" + id
                            };
                            image.MouseUp += delegate (object obj, MouseButtonEventArgs Args)
                            {
                                if (_selectedFile != null)
                                    _selectedFile.Opacity = 0.5;
                                _selectedFile = obj as Image;
                                _selectedFile.Opacity = 1;
                                LoadContentInfo(_imagesCache[(obj as Image).Name]);
                            };
                            textBoxQuizTitle.Text = newQuiz.Title;
                            _imagesCache[image.Name] = id;
                            Files.Children.Add(image);
                            _selectedFile = image;
                            EditQuiz.IsEnabled = true;
                            DeleteQuiz.IsEnabled = true;
                        }
                        window.Close();
                    }
                };
                window.Content = quiz;
            };
            window.Closing += delegate (object o, CancelEventArgs args)
            {
                if (!(window.Content as QuizCreation).IsSaved())
                {
                    args.Cancel = true;
                    MessageBox.Show("Progress is not saved!");
                }
                _saved = false;
                Save.IsEnabled = true;
            };
            window.ShowDialog();
        }

        private void EditQuiz_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Title = "Edit Quiz",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Loaded += delegate (object o, RoutedEventArgs args)
            {
                QuizCreation quiz = new QuizCreation((comboBox.SelectedItem as Lesson).ID, _contentManager, true, _imagesCache[_selectedFile.Name]);
                window.Height = quiz.Height + 45;
                window.Width = quiz.Width + 45;
                quiz.Button.Click += delegate(object sender1, RoutedEventArgs eventArgs)
                {
                    if (quiz.IsSaved())
                    {
                        window.Close();
                    }
                };
                window.Content = quiz;
            };
            window.Closing += delegate (object o, CancelEventArgs args)
            {
                if (!(window.Content as QuizCreation).IsSaved())
                {
                    args.Cancel = true;
                    MessageBox.Show("Progress is not saved!");
                }
                _saved = false;
                Save.IsEnabled = true;
            };
            window.ShowDialog();
        }

        private void DeleteQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFile != null)
            {
                if (_contentManager.GetComponent(_imagesCache[_selectedFile.Name]).GetType().Name == "Quiz")
                {
                    _contentManager.DeleteQuiz(_imagesCache[_selectedFile.Name], (comboBox.SelectedItem as Lesson).ID);
                    _imagesCache.Remove(_selectedFile.Name);
                    Files.Children.Remove(_selectedFile);
                    _selectedFile = null;
                    _saved = false;
                    Save.IsEnabled = true;
                }
            }
        }

        public bool IsSaved()
        {
            return _saved;
        }

        public string GetNoOfFiles()
        {
            return Files.Children.Count.ToString();
        }
    }
}
