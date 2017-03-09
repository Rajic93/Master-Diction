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
using Diction_Master___Library;

namespace Diction_Master___Server.Custom_Controls
{
    /// <summary>
    /// Interaction logic for QuizCreation.xaml
    /// </summary>
    public partial class QuizCreation : UserControl
    {
        private Diction_Master___Library.ContentManager _contentManager;
        private long _selectedQuiz;
        private long _selectedLesson;
        private bool _edit;
        private ObservableCollection<Question> questions;
        private ObservableCollection<string> _wrongAnswers;
        private ObservableCollection<string> _pieces;
        private Question _selectedQuestion;
        private bool _newQuiz;
        private long _newQuizID;

        private bool _saved;

        public QuizCreation(long parent, Diction_Master___Library.ContentManager manager, bool edit, long quiz)
        {
            _contentManager = manager;
            _selectedLesson = parent;
            _edit = edit;
            _wrongAnswers = new ObservableCollection<string>();
            _pieces = new ObservableCollection<string>();
            InitializeComponent();
            if (edit)
            {
                _selectedQuiz = quiz;
                textBoxTitle.Text = (_contentManager.GetComponent(quiz) as Quiz).Title;
            }
            LoadQuestions();
        }

        private void LoadQuestions()
        {
            if (_selectedQuiz != 0)
                questions = _contentManager.GetAllQuestions(_selectedQuiz);
            else
            {
                long id = _contentManager.AddQuiz(_selectedLesson, "");
                if (id > 0)
                {
                    _selectedQuiz = id;
                    _newQuiz = true;
                    _newQuizID = id;
                    questions = new ObservableCollection<Question>();
                }
                else if (id == 0)
                {
                    _selectedQuiz = _contentManager.GetComponent(_selectedLesson).ID;
                    questions = _contentManager.GetAllQuestions(_selectedQuiz);
                    Component comp = _contentManager.GetComponent(_selectedQuiz);
                    if (comp is Quiz)
                    {
                        textBoxTitle.Text = (comp as Quiz).Title; 
                    }
                }
            }
            listBox.ItemsSource = questions;
            listBox.DisplayMemberPath = "Text";
            _saved = true;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_saved)
            {
                if (listBox.SelectedItem != null)
                {
                    Question question = listBox.SelectedItem as Question;
                    textBoxQuestion.Text = question.Text;
                    textBoxAnswer.Text = question.Answer;
                    switch (question.Type)
                    {
                        case QuestionType.Text:
                            radioButtonText.IsChecked = true;
                            break;
                        case QuestionType.Puzzle:
                            radioButtonPuzzle.IsChecked = true;
                            break;
                        case QuestionType.Choice:
                            radioButtonChoice.IsChecked = true;
                            break;
                    }
                    _wrongAnswers = question.WrongAnswers ?? new ObservableCollection<string>();
                    listBox1.Items.Clear();
                    foreach (string wrongAnswer in _wrongAnswers)
                    {
                        listBox1.Items.Add(wrongAnswer);
                    }
                    _selectedQuestion = listBox.SelectedItem as Question;
                    EditQuestion.IsEnabled = true;
                    DeleteQuestion.IsEnabled = true;
                    Save.IsEnabled = false;
                }
            }
            else
            {
                MessageBox.Show("Progress is not saved!");
                //listBox.set
            }
        }

        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            QuestionType type;
            long id = 0;
            if (radioButtonText.IsChecked.Value)
            {
                type = QuestionType.Text;
                id = _contentManager.AddQuestion(_selectedQuiz, textBoxQuestion.Text, textBoxAnswer.Text, type, null, null);
            }
            else if (radioButtonPuzzle.IsChecked.Value)
            {
                type = QuestionType.Puzzle;
                id = _contentManager.AddQuestion(_selectedQuiz, textBoxQuestion.Text, textBoxAnswer.Text, type, null, _pieces);
            }
            else
            {
                type = QuestionType.Choice;
                id = _contentManager.AddQuestion(_selectedQuiz, textBoxQuestion.Text, textBoxAnswer.Text, type, _wrongAnswers, null);
            }
            if (id > 0)
            {
                questions.Add(_contentManager.GetComponent(id) as Question);
                _saved = false;
                Save.IsEnabled = true;
            }
        }

        private void EditQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                QuestionType type;
                int id = 0;
                if (radioButtonText.IsChecked.Value)
                {
                    type = QuestionType.Text;
                     _contentManager.EditQuestion((listBox.SelectedItem as Component).ID, textBoxQuestion.Text, textBoxAnswer.Text, type, null, null);
                }
                else if (radioButtonPuzzle.IsChecked.Value)
                {
                    type = QuestionType.Puzzle;
                    _contentManager.EditQuestion((listBox.SelectedItem as Component).ID, textBoxQuestion.Text, textBoxAnswer.Text, type, null, _pieces);
                }
                else
                {
                    type = QuestionType.Choice;
                    _contentManager.EditQuestion((listBox.SelectedItem as Component).ID, textBoxQuestion.Text, textBoxAnswer.Text, type, _wrongAnswers, null);
                }
                //_contentManager.EditQuestion((listBox.SelectedItem as Question).ID, textBoxQuestion.Text, textBoxAnswer.Text, type, _wrongAnswers);
                listBox.Items.Refresh();
                _saved = false;
                Save.IsEnabled = true;
            }
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                _contentManager.DeleteQuestion((listBox.SelectedItem as Question).ID, _selectedQuiz);
                questions.Remove(listBox.SelectedItem as Question);
                EditQuestion.IsEnabled = false;
                DeleteQuestion.IsEnabled = false;
                _saved = false;
                Save.IsEnabled = true;
            }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                textBoxWrongAnswer.Text = listBox1.SelectedItem.ToString();
                EditWrongAnswer.IsEnabled = true;
                DeleteWrongAnswer.IsEnabled = true;
            }
        }

        private void AddWrongAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxWrongAnswer.Text != "")
            {
                _wrongAnswers.Add(textBoxWrongAnswer.Text);
                listBox1.Items.Add(textBoxWrongAnswer.Text);
                _saved = false;
                Save.IsEnabled = true;
            }
        }

        private void EditWrongAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                int index = _wrongAnswers.IndexOf(listBox1.SelectedItem.ToString());
                _wrongAnswers.RemoveAt(index);
                _wrongAnswers.Insert(index, textBoxWrongAnswer.Text);
                index = listBox1.Items.IndexOf(listBox1.SelectedItem.ToString());
                listBox1.Items.RemoveAt(index);
                listBox1.Items.Insert(index, textBoxWrongAnswer.Text);
                _saved = false;
                Save.IsEnabled = true;
            }
        }

        private void DeleteWrongAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                _wrongAnswers.Remove(listBox1.SelectedItem.ToString());
                listBox1.Items.Remove(listBox1.SelectedItem.ToString());
                textBoxWrongAnswer.Text = "";
                EditWrongAnswer.IsEnabled = false;
                DeleteWrongAnswer.IsEnabled = false;
                _saved = false;
                Save.IsEnabled = true;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _saved = true;
            Save.IsEnabled = false;
        }

        public bool IsSaved()
        {
            return _saved;
        }

        public bool NewQuizCreated()
        {
            return _newQuiz;
        }

        public long NewQuizID()
        {
            return _newQuizID;
        }

        public string GetTitle()
        {
            return textBoxTitle.Text;
        }
    }
}
