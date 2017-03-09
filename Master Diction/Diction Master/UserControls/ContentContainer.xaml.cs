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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Diction_Master.UserControls;

namespace Diction_Master___Library.UserControls
{
    /// <summary>
    /// Interaction logic for ContentContainer.xaml
    /// </summary>
    public partial class ContentContainer : UserControl
    {
        private bool _menuMinimized = false;
        private List<Week> _termI;
        private List<Week> _termII;
        private List<Week> _termIII;

        private bool _termIEnabled = true;
        private bool _termIIEnabled = true;
        private bool _termIIIEnabled = true;

        private WrapPanel _previousWeekPanel;
        private WrapPanel _previousLessonPanel;
        private Week _choosenWeek;
        private Lesson _choosenLesson;
        
        private ClientState _state = new ClientState();

        public ContentContainer(ClientState state)
        {
            _state = state;
            _termI = new List<Week>();
            _termII = new List<Week>();
            _termIII = new List<Week>();
            InitializeComponent();
            //CreateContent();
            foreach (Week item in (_state._enabledGrades.First() as CompositeComponent).Components)
            {
                switch (item.Term)
                {
                    case 1:
                        _termI.Add(item);
                        break;
                    case 2:
                        _termII.Add(item);
                        break;
                    case 3:
                        _termIII.Add(item);
                        break;
                    default:
                        break;
                }
            }
            if (_termIEnabled)
                InitializeWeeks(_termI);
            //else
                //show disabled term
            if (_termIIEnabled)
                InitializeWeeks(_termII);
            //else
            //show disabled term
            if (_termIIIEnabled)
                InitializeWeeks(_termIII);
            //else
            //show disabled term
        }

        private void InitializeWeeks(List<Week> term)
        {
            foreach (Week item in term)
            {
                string name = item.Title.Replace(' ', '_') + "_term_" + item.Term + "_" + item.ID;
                WrapPanel panel = new WrapPanel
                {
                    Name = "panel_" + name,
                    Height = 30,
                    MinWidth = Weeks.MinWidth// - Weeks.Margin.Left - Weeks.Margin.Right
                };
                panel.HorizontalAlignment = HorizontalAlignment.Stretch;
                Button button = new Button
                {
                    Name = "button_" + name,
                    Height = 30,
                    MinWidth = panel.MinWidth,
                    Margin = new Thickness(0, 2, 0, 0),
                    Content = item.Title,
                    BorderThickness = new Thickness(0),
                    Background = new SolidColorBrush(Color.FromRgb(93, 177, 246))
                };
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.Click += delegate (object sender, RoutedEventArgs args)
                {
                    WeekClicked(sender as Button);
                };
                panel.Children.Add(button);
                InitializeLesson(item.Components, panel);
                Weeks.Children.Add(panel);
            }
        }

        private void InitializeLesson(List<Component> components, WrapPanel container)
        {
            foreach (Lesson item in components)
            {
                string name = item.Title.Replace(' ', '_') + "_num_" + item.Num + "_" + item.ID;
                BrushConverter bc = new BrushConverter();
                Brush brush = (Brush)bc.ConvertFrom("#F2F2F2F2");
                brush.Freeze();
                WrapPanel panel = new WrapPanel
                {
                    Name = "panel_" + name,
                    Height = 30,
                    MinWidth = Weeks.MinWidth - Weeks.Margin.Left - Weeks.Margin.Right,
                    Margin = new Thickness(Weeks.Margin.Left, 0, 0, 0),
                    Visibility = Visibility.Collapsed,
                    Background = brush
                };
                panel.HorizontalAlignment = HorizontalAlignment.Stretch;
                Button button = new Button
                {
                    Name = "button_" + name,
                    Height = 30,
                    MinWidth = panel.MinWidth,
                    Margin = new Thickness(0, 2, 0, 0),
                    Content = item.Title,
                    BorderThickness = new Thickness(0),
                    Background = new SolidColorBrush(Color.FromRgb(136, 201, 255))
                };
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.Click += delegate (object sender, RoutedEventArgs args)
                {
                    LessonClicked(sender as Button);
                };
                panel.Children.Add(button);
                InitializeContent(item.Components, panel);
                container.Children.Add(panel);
            }
        }

        private void InitializeContent(List<Component> components, WrapPanel container)
        {
            foreach (Component item in components)
            {
                string name = "";
                string icon = "";
                if (item.GetType().Name == "ContentFile")
                {
                    ContentFile file = item as ContentFile;
                    name = file.Title.Replace(' ', '_') + "_" + file.ID;
                    icon = file.icon;
                }
                else if (item.GetType().Name == "Quiz")
                {
                    Quiz quiz = item as Quiz;
                    name = quiz.Title.Replace(' ', '_') + "_" + quiz.ID;
                    icon = "../Resources/quiz.png";
                }
                Image image = new Image
                {
                    Name = "image_" + name,
                    Height = 30,
                    Width = 30,
                    Source = new BitmapImage(new Uri(icon, UriKind.Relative)),
                    Margin = new Thickness(3),
                    Visibility = Visibility.Collapsed
                };
                image.MouseEnter += delegate (object sender, MouseEventArgs args)
                {
                    image.Opacity = 0.5;
                };
                image.MouseLeave += delegate (object sender, MouseEventArgs args)
                {
                    image.Opacity = 1;
                };
                image.MouseUp += delegate (object sender, MouseButtonEventArgs args)
                {
                    ContentClicked(sender as Image);
                };
                container.Children.Add(image);
            }
        }

        private void WeekClicked(Button button)
        {
            WrapPanel clickedPanel = null;
            for (int i = 0; i < Weeks.Children.Count; i++)
            {
                if ((Weeks.Children[i] as WrapPanel).Name == "panel_" + button.Name.Substring("button_".Length))
                {
                    clickedPanel = Weeks.Children[i] as WrapPanel;
                    break;
                }
            }
            if (!Equals(_previousWeekPanel, clickedPanel))
            {
                if (_previousWeekPanel != null)
                {
                    _previousWeekPanel.Height = 30;
                    for (int i = 1; i < _previousWeekPanel.Children.Count; i++)
                    {
                        WrapPanel wrap = _previousWeekPanel.Children[i] as WrapPanel;
                        wrap.Visibility = Visibility.Collapsed;
                    }
                }
                if (clickedPanel != null)
                {
                    long id = Convert.ToInt64(clickedPanel.Name.Split('_').Last());
                    if (_termI.Exists(x => x.ID == id))
                        _choosenWeek = _termI.Find(x => x.ID == id);
                    else if (_termII.Exists(x => x.ID == id))
                        _choosenWeek = _termII.Find(x => x.ID == id);
                    else if (_termIII.Exists(x => x.ID == id))
                        _choosenWeek = _termIII.Find(x => x.ID == id);
                    else
                        _choosenWeek = null;
                    if (_choosenWeek != null)
                    {
                        double height = 0;
                        for (int i = 1; i < clickedPanel.Children.Count; i++)
                        {
                            height += (clickedPanel.Children[i] as WrapPanel).Height;
                        }
                        clickedPanel.Height += height;
                        for (int i = 1; i < clickedPanel.Children.Count; i++)
                        {
                            WrapPanel wrap = clickedPanel.Children[i] as WrapPanel;
                            wrap.Visibility = Visibility.Visible;
                        }
                        _previousWeekPanel = clickedPanel; 
                    }
                } 
            }
        }

        private void LessonClicked(Button button)
        {
            WrapPanel clickedPanel = null;
            for (int i = 1; i < _previousWeekPanel.Children.Count; i++)
            {
                if ((_previousWeekPanel.Children[i] as WrapPanel).Name == "panel_" + button.Name.Substring("button_".Length))
                {
                    clickedPanel = _previousWeekPanel.Children[i] as WrapPanel;
                    break;
                }
            }
            if (!Equals(_previousLessonPanel, clickedPanel))
            {
                if (_previousLessonPanel != null)
                {
                    _previousLessonPanel.Height = 30;
                    for (int i = 0; i < _previousLessonPanel.Children.Count; i++)
                    {
                        WrapPanel wrap = _previousLessonPanel.Children[i] as WrapPanel;
                        if (wrap != null)
                        wrap.Visibility = Visibility.Collapsed;
                    }
                }
                if (clickedPanel != null)
                {
                    string sub = clickedPanel.Name.Substring(clickedPanel.Name.IndexOf("num_"));
                    string idStr = sub.Split('_')[2];
                    long id = Convert.ToInt64(idStr);
                    double r1 = clickedPanel.ActualWidth / 30;
                    double r2 = (double)(clickedPanel.Children.Count - 1)/ (r1 - (r1 - (int)(r1)));
                    int num = (int)(r2);
                    if (r2 - num > 0)
                        num++;
                    //if (num == 0)
                    //    num = 10;
                    if (_choosenWeek.Components.Exists(x => x.ID == id))
                        _choosenLesson = _choosenWeek.Components.Find(x => x.ID == id) as Lesson;
                    else
                        _choosenLesson = null;
                    if (_choosenLesson != null)
                    {
                        clickedPanel.Height += 40 * num;
                        for (int i = 1; i < clickedPanel.Children.Count; i++)
                        {
                            Image img = clickedPanel.Children[i] as Image;
                            img.Visibility = Visibility.Visible;
                        }
                        _previousLessonPanel = clickedPanel;
                        //update previous level height
                        double height = 0;
                        for (int i = 1; i < _previousWeekPanel.Children.Count; i++)
                        {
                            height += (_previousWeekPanel.Children[i] as WrapPanel).Height;
                        }
                        double difference = _previousWeekPanel.Height - 30;
                        difference = height - difference;
                        _previousWeekPanel.Height += difference; 
                    }
                }
            }
        }

        private void ContentClicked(Image image)
        {
            string idStr = image.Name.Split('_').Last();
            long id = Convert.ToInt64(idStr);
            long weekID = Convert.ToInt64(_previousWeekPanel.Name.Split('_').Last());
            int term = Convert.ToInt16(_previousWeekPanel.Name.Substring(_previousWeekPanel.Name.IndexOf("term")).Split('_')[1]);
            long lessonID = Convert.ToInt64(_previousLessonPanel.Name.Split('_').Last());
            Component component = null;
            switch (term)
            {
                case 1:
                    component = _termI.Find(x => x.ID == weekID);
                    break;
                case 2:
                    component = _termII.Find(x => x.ID == weekID);
                    break;
                case 3:
                    component = _termIII.Find(x => x.ID == weekID);
                    break;
                default:
                    break;
            }
            if (component != null)
            {
                component = (component as Week).Components.Find(x => x.ID == lessonID);
                if (component != null)
                {
                    var lesson = component as Lesson;
                    if (lesson != null)
                        component = lesson.Components.Find(x => x.ID == id);
                    CreateContentViewer(id, component);
                }
            }
        }

        private void CreateContentViewer(long id, Component component)
        {
            if (component != null && id != 0)
            {
                GridContent.Children.Clear();
                if (component.GetType().Name == "ContentFile")
                {
                    ContentFile file = component as ContentFile;
                    if (file != null)
                    {
                        switch (file.ComponentType)
                        {
                            case ComponentType.Audio:
                            case ComponentType.Video:
                                MediaViewer viewer = new MediaViewer
                                {
                                    RenderSize = GridContent.RenderSize,
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    VerticalAlignment = VerticalAlignment.Stretch
                                };
                                viewer.SetContent(_choosenLesson.Components.Find(x => x.ID == id) as ContentFile);
                                viewer.right.MouseUp += delegate
                                {
                                    Component next = NextFile(_choosenLesson.Components.Find(x => x.ID == id));
                                    if (next != null)
                                    {
                                        CreateContentViewer(next.ID, next);
                                    }
                                };
                                viewer.left.MouseUp += delegate
                                {
                                    Component next = PreviousFile(_choosenLesson.Components.Find(x => x.ID == id));
                                    if (next != null)
                                    {
                                        CreateContentViewer(next.ID, next);
                                    }
                                };
                                GridContent.Children.Add(viewer);
                                break;
                            case ComponentType.Document:
                                Diction_Master.UserControls.DocumentViewer docViewer = new Diction_Master.UserControls.DocumentViewer
                                {
                                    RenderSize = GridContent.RenderSize,
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    VerticalAlignment = VerticalAlignment.Stretch
                                };
                                docViewer.SetContent(_choosenLesson.Components.Find(x => x.ID == id) as ContentFile);
                                docViewer.right.MouseUp += delegate
                                {
                                    //choosen lesson null
                                    Component next = NextFile(_choosenLesson.Components.Find(x => x.ID == id));
                                    if (next != null)
                                    {
                                        CreateContentViewer(next.ID, next);
                                    }
                                };
                                docViewer.left.MouseUp += delegate
                                {
                                    Component previous = PreviousFile(_choosenLesson.Components.Find(x => x.ID == id));
                                    if (previous != null)
                                    {
                                        CreateContentViewer(previous.ID, previous);
                                    }
                                };
                                GridContent.Children.Add(docViewer);
                                break;
                        }
                    }
                }
                else if (component.GetType().Name == "Quiz")
                {
                    QuizViewer viewer = new QuizViewer
                    {
                        RenderSize = GridContent.RenderSize,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    viewer.SetContent(component as Quiz);
                    viewer.right.MouseUp += delegate
                    {
                        Component next = NextFile(_choosenLesson.Components.Find(x => x.ID == id));
                        if (next != null)
                            CreateContentViewer(next.ID, next);
                        else
                            CreateContentViewer(0, null);
                    };
                    viewer.left.MouseUp += delegate
                    {
                        Component previous = PreviousFile(_choosenLesson.Components.Find(x => x.ID == id));
                        if (previous != null)
                        {
                            CreateContentViewer(previous.ID, previous);
                        }
                    };
                    GridContent.Children.Add(viewer);
                }
            }
            else
            {
                ContentEnd end = new ContentEnd();
                end.left.MouseUp += delegate
                {
                    Component previous = PreviousFile(null);
                    if (previous != null)
                    {
                        CreateContentViewer(previous.ID, previous);
                    }
                };
                GridContent.Children.Clear();
                GridContent.Children.Add(end);
            }
        }

        private Component NextFile(Component contentFile)
        {
            int index = _choosenLesson.Components.IndexOf(contentFile);
            if (index != -1)
            {
                if (index < _choosenLesson.Components.Count - 1) //this week, this lesson, just next file
                    return _choosenLesson.Components.ElementAt(++index);
                if (index == _choosenLesson.Components.Count - 1)
                {
                    //choose next lesson
                    int lessonIndex = _choosenWeek.Components.IndexOf(_choosenLesson);
                    if (lessonIndex == _choosenWeek.Components.Count - 1)
                    {
                        //choose next week
                        switch (_choosenWeek.Term)
                        {
                            //find week
                            case 1:
                                if (_termI.Count == _choosenWeek.Num - 1) //next term, first week
                                    _choosenWeek = _termII.ElementAt(0);
                                //this term, next week
                                _choosenWeek = _termI.ElementAt(_termI.IndexOf(_choosenWeek) + 1);
                                break;
                            case 2:
                                if (_termII.Count == _choosenWeek.Num - 1) //next term, first week
                                    _choosenWeek = _termIII.ElementAt(0);
                                //this term, next week
                                _choosenWeek = _termII.ElementAt(_termII.IndexOf(_choosenWeek) + 1);
                                break;
                            case 3:
                                if (_termIII.Count == _choosenWeek.Num - 1)
                                {
                                    //last week, last lesson, last file - the end
                                    _choosenWeek = null;
                                    _choosenLesson = null;
                                    return null;
                                }
                                //this term, next week
                                _choosenWeek = _termIII.ElementAt(_termIII.IndexOf(_choosenWeek) + 1);
                                break;
                        }
                        if (_choosenWeek.Components.Count != 0)
                            _choosenLesson = _choosenWeek.Components.ElementAt(0) as Lesson;
                        else
                            _choosenLesson = null;
                        //set next  panel
                        return _choosenLesson?.Components.ElementAt(0);
                    }
                    //this week, next lesson, first file - this part is not good
                    if (_choosenWeek.Components.Count != 0)
                        _choosenLesson = _choosenWeek.Components.ElementAt(++lessonIndex) as Lesson;
                    else
                        _choosenLesson = null;
                    //set next  panel
                    if (_choosenLesson?.Components.Count != 0)
                        return _choosenLesson?.Components.ElementAt(0); //null propagation
                    else
                        return null;
                }
            }
            return null;
        }

        private Component PreviousFile(Component contentFile)
        {
            if (contentFile != null)
            {
                int index = _choosenLesson.Components.IndexOf(contentFile);
                if (index > 0) //this week, this lesson, just previous file
                    return _choosenLesson.Components.ElementAt(--index);
                if (index == 0)
                {
                    //choose previous lesson
                    int lessonIndex = _choosenWeek.Components.IndexOf(_choosenLesson);
                    if (lessonIndex == 0)
                    {
                        //choose previous week
                        switch (_choosenWeek.Term)
                        {
                            //find week
                            case 1:
                                if (0 == _choosenWeek.Num - 1)
                                {
                                    //first week, first lesson, first file - start
                                    //_choosenWeek = null;
                                    //_choosenLesson = null;
                                    return null;
                                }
                                //this term, previous week
                                _choosenWeek = _termI.ElementAt(_termI.IndexOf(_choosenWeek) - 1);
                                break;
                            case 2:
                                if (0 == _choosenWeek.Num - 1) //previous term, last week
                                    _choosenWeek = _termI.ElementAt(_termI.Count - 1);
                                //this term, previous week
                                _choosenWeek = _termII.ElementAt(_termII.IndexOf(_choosenWeek) - 1);
                                break;
                            case 3:
                                if (0 == _choosenWeek.Num - 1) //previous term, last week
                                    _choosenWeek = _termII.ElementAt(_termII.Count - 1);
                                //this term, previous week
                                _choosenWeek = _termIII.ElementAt(_termIII.IndexOf(_choosenWeek) - 1);
                                break;
                        }
                        if (_choosenWeek.Components.Count != 0)
                            _choosenLesson = _choosenWeek.Components.ElementAt(_choosenWeek.Components.Count - 1) as Lesson;
                        else
                            _choosenLesson = null;
                        //set previous  panel
                        return _choosenLesson?.Components.ElementAt(0);
                    }
                    //this week, previous lesson, last file
                    if (_choosenWeek.Components.Count != 0)
                        _choosenLesson = _choosenWeek.Components.ElementAt(--lessonIndex) as Lesson;
                    else
                        _choosenLesson = null;
                    //set previous  panel
                    return _choosenLesson?.Components.Last(); //null propagation
                }
                return null; 
            }
            if (_choosenLesson != null)
            {
                int indexLesson = _choosenWeek.Components.IndexOf(_choosenLesson);
                if (indexLesson != -1)
                {
                    if (_choosenWeek.Components.Count != 0)
                        _choosenLesson = _choosenWeek.Components.ElementAt(--indexLesson) as Lesson;
                    else
                        _choosenLesson = null; 
                }
                return null;
            }
            if (_choosenWeek != null)
                if (_choosenWeek.Components.Count != 0)
                    _choosenLesson = _choosenWeek.Components.Last() as Lesson;
                else
                {
                    if (_choosenWeek.Term == 1)
                    {
                        if (_termI.IndexOf(_choosenWeek) > 0)
                            _choosenWeek = _termI.ElementAt(_termI.IndexOf(_choosenWeek) - 1);
                        else
                            return null;
                    }
                    else if (_choosenWeek.Term == 2)
                    {
                        _choosenWeek = _termII.IndexOf(_choosenWeek) > 0
                            ? _termII.ElementAt(_termII.IndexOf(_choosenWeek) - 1)
                            : _termI.Last();
                    }
                    else if (_choosenWeek.Term == 3)
                    {
                        _choosenWeek = _termIII.IndexOf(_choosenWeek) > 0
                            ? _termIII.ElementAt(_termIII.IndexOf(_choosenWeek) - 1)
                            : _termII.Last();
                    }
                    _choosenLesson = _choosenWeek.Components.Last() as Lesson;
                }
            else
                _choosenLesson = null;
            //
            //set previous  panel
            return _choosenLesson?.Components.Last(); //null propagation
        }

        private void Menu_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_menuMinimized)
            {
                Navigation.Width = new GridLength(0.04, GridUnitType.Star);
                //Border.Visibility = Visibility.Collapsed;
            }
            else
            {
                Navigation.Width = new GridLength(0.25, GridUnitType.Star);
                //Border.Visibility = Visibility.Visible;
            }
            _menuMinimized = !_menuMinimized;
        }

        private void CreateContent()
        {
            _state._enabledGrades.Add(new Grade
                    {
                        ID = 17,
                        ParentID = 5,
                        GradeNum = GradeType.NurseryI,
                        Icon = "../Resources/nursery1.png"
                    });
            (_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Add(new Week
            {
                ID = 73,
                ParentID = 17,
                Term = 1,
                Num = 1,
                Title = "Week 1"
            });
            ((_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Find(y => y.ID == 73) as Week).Components.Add(new Lesson
            {
                ID = 83,
                ParentID = 17,
                Num = 1,
                Title = "Lesson 1"
            });
            (((_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Find(y => y.ID == 73) as Week).Components.Find(z => z.ID == 83) as Lesson)
                .Components.Add(new ContentFile
            {
                ID = 88,
                ParentID = 83,
                URI = "Artist - Black - Wonderful life HD.mp3",
                icon = "../Resources/audio.png",
                Title = "Audio 1",
                Description = "Wonderful life song from Black.",
                ComponentType = ComponentType.Audio
            });
            (((_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Find(y => y.ID == 73) as Week).Components.Find(z => z.ID == 83) as Lesson)
                .Components.Add(new ContentFile
                {
                    ID = 89,
                    ParentID = 83,
                    URI = "Content definition.avi",
                    icon = "../Resources/video.png",
                    Title = "Video 1",
                    Description = "Video that explains how to define content on server",
                    ComponentType = ComponentType.Video
                });
            (((_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Find(y => y.ID == 73) as Week).Components.Find(z => z.ID == 83) as Lesson)
                .Components.Add(new ContentFile
                {
                    ID = 90,
                    ParentID = 83,
                    URI = "hgignore_global.txt",
                    icon = "../Resources/document.png",
                    Title = "Document 1",
                    Description = "Git local ignore file",
                    ComponentType = ComponentType.Document
                });
            (((_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Find(y => y.ID == 73) as Week).Components.Find(z => z.ID == 83) as Lesson)
                .Components.Add(new Quiz
                {
                    ID = 91,
                    ParentID = 83,
                    Title = "Quiz 1"
                });
            ((((_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Find(y => y.ID == 73) as Week).Components.Find(z => z.ID == 83) as Lesson)
                .Components.Find(k => k.ID == 91 ) as Quiz).Components.Add(new Question
                {
                    ID = 92,
                    ParentID = 91,
                    Type = QuestionType.Text,
                    Text = "Hello! What is your name, how old are you and where are you from?",
                    Answer = "Hello! My name is Aleksandar Rajic. I am 23-year old and i am coming from Serbia."
                });
            ((((_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Find(y => y.ID == 73) as Week).Components.Find(z => z.ID == 83) as Lesson)
                .Components.Find(k => k.ID == 91) as Quiz).Components.Add(new Question
                {
                    ID = 93,
                    ParentID = 91,
                    Type = QuestionType.Choice,
                    Text = "Hello! What is your name, how old are you and where are you from?",
                    Answer = "Hello! My name is Aleksandar Rajic. I am 23-year old and i am coming from Serbia.",
                    WrongAnswers = new ObservableCollection<string>(new []
                    {
                        "Hello! My name is Aleksandar Rajic and am coming from Serbia.",
                        "Hello! I am 23-year old and i am coming from Serbia.",
                        "Hello! My name is Aleksandar Rajic."
                    })
                });
            ((((_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Find(y => y.ID == 73) as Week).Components.Find(z => z.ID == 83) as Lesson)
                .Components.Find(k => k.ID == 91) as Quiz).Components.Add(new Question
                {
                    ID = 94,
                    ParentID = 91,
                    Type = QuestionType.Puzzle,
                    Text = "Hello! What is your name, how old are you and where are you from?",
                    Answer = "Hello! My name is Aleksandar Rajic. I am 23-year old and i am coming from Serbia.",
                    Pieces = new ObservableCollection<string>(new[]
                    {
                        "Hello!",
                        "My name is Aleksandar Rajic.",
                        "I am 23-year old",
                        "and i am coming from Serbia."
                    })
                });
            ((_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Find(y => y.ID == 73) as Week).Components.Add(new Lesson
            {
                ID = 85,
                ParentID = 17,
                Num = 3,
                Title = "Lesson 3"
            });
            ((_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Find(y => y.ID == 73) as Week).Components.Add(new Lesson
            {
                ID = 86,
                ParentID = 17,
                Num = 4,
                Title = "Lesson 4"
            });
            ((_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Find(y => y.ID == 73) as Week).Components.Add(new Lesson
            {
                ID = 87,
                ParentID = 17,
                Num = 5,
                Title = "Lesson 5"
            });

            (_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Add(new Week
            {
                ID = 74,
                ParentID = 17,
                Term = 1,
                Num = 2,
                Title = "Week 2"
            });
            (_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Add(new Week
            {
                ID = 75,
                ParentID = 17,
                Term = 1,
                Num = 3,
                Title = "Week 3"
            });
            (_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Add(new Week
            {
                ID = 76,
                ParentID = 17,
                Term = 2,
                Num = 4,
                Title = "Week 4"
            });
            (_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Add(new Week
            {
                ID = 77,
                ParentID = 17,
                Term = 2,
                Num = 5,
                Title = "Week 5"
            });
            (_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Add(new Week
            {
                ID = 78,
                ParentID = 17,
                Term = 2,
                Num = 6,
                Title = "Week 6"
            });
            (_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Add(new Week
            {
                ID = 79,
                ParentID = 17,
                Term = 3,
                Num = 7,
                Title = "Week 7"
            });
            (_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Add(new Week
            {
                ID = 80,
                ParentID = 17,
                Term = 3,
                Num = 8,
                Title = "Week 8"
            });
            (_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Add(new Week
            {
                ID = 81,
                ParentID = 17,
                Term = 3,
                Num = 9,
                Title = "Week 9"
            });
            (_state._enabledGrades.Find(x => x.ID == 17) as Grade).Components.Add(new Week
            {
                ID = 82,
                ParentID = 17,
                Term = 3,
                Num = 10,
                Title = "Week 10"
            });
        }
    }
}
