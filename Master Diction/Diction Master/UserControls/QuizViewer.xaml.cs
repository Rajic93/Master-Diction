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
using Diction_Master___Library;

namespace Diction_Master.UserControls
{
    /// <summary>
    /// Interaction logic for QuizViewer.xaml
    /// </summary>
    public partial class QuizViewer : UserControl
    {
        private Quiz _quiz;
        private int _index;
        private bool[] _correctAnswer;
        private string[] _answer;

        public QuizViewer()
        {
            InitializeComponent();
        }

        public void SetContent(Quiz quiz)
        {
            _quiz = quiz;
            if (_quiz.Components.Count != 0)
            {
                Question question = _quiz.Components[0] as Question;
                _index = 0;
                _correctAnswer = new bool[_quiz.Components.Count];
                _answer = new string[_quiz.Components.Count];
                ProgressBar.Maximum = _quiz.Components.Count;
                ProgressBar.Minimum = 0;
                CreateQuestion(question); 
            }
        }

        private void CreateQuestion(Question question)
        {
            ProgressBar.Value = _index + 1;
            Grid.Children.Clear();
            switch (question.Type)
            {
                case QuestionType.Text:
                    TextQuestion txtQuestion = new TextQuestion(question);
                    txtQuestion.button.Click += delegate
                    {
                        var textQuestion = Grid.Children[0] as TextQuestion;
                        if (textQuestion != null)
                        {
                            string questionAnswer = textQuestion.GetAnswer();
                            _answer[_index] = questionAnswer;
                            if (question.Answer == questionAnswer)
                                _correctAnswer[_index] = true;
                            else
                                _correctAnswer[_index] = false;
                            if (_index < _quiz.Components.Count - 1)
                            {
                                Question nextQuestion = _quiz.Components[++_index] as Question;
                                CreateQuestion(nextQuestion);
                            }
                            else if (_index < _quiz.Components.Count - 1)
                            {
                                ShowResults();
                            } 
                        }
                    };
                    Grid.Children.Add(txtQuestion);
                    txtQuestion.HorizontalAlignment = HorizontalAlignment.Center;
                    txtQuestion.VerticalAlignment = VerticalAlignment.Top;
                    if (_index == _quiz.Components.Count - 1)
                        txtQuestion.button.Content = "Finish";
                    break;
                case QuestionType.Puzzle:
                    PuzzleQuestion puzzleQuestion = new PuzzleQuestion(question);
                    puzzleQuestion.button.Click += delegate
                    {
                        var pzzQuestion = Grid.Children[0] as PuzzleQuestion;
                        if (pzzQuestion != null)
                        {
                            string qAnswer = pzzQuestion.GetAnswer();
                            _answer[_index] = qAnswer;
                            if (question.Answer == qAnswer)
                                _correctAnswer[_index] = true;
                            else
                                _correctAnswer[_index] = false;
                            if (_index < _quiz.Components.Count - 1)
                            {
                                Question nextQuestion = _quiz.Components[++_index] as Question;
                                CreateQuestion(nextQuestion);
                            }
                            else if (_index == _quiz.Components.Count - 1)
                            {
                                ShowResults();
                            } 
                        }
                    };
                    Grid.Children.Add(puzzleQuestion);
                    puzzleQuestion.HorizontalAlignment = HorizontalAlignment.Center;
                    puzzleQuestion.VerticalAlignment = VerticalAlignment.Top;
                    if (_index == _quiz.Components.Count - 1)
                        puzzleQuestion.button.Content = "Finish";
                    break;
                case QuestionType.Choice:
                    ChoiceQuestion choiceQuestion = new ChoiceQuestion(question);
                    choiceQuestion.button.Click += delegate
                    {
                        var chcQuestion = Grid.Children[0] as ChoiceQuestion;
                        if (chcQuestion != null)
                        {
                            string qAnswer = chcQuestion.GetAnswer();
                            _answer[_index] = qAnswer;
                            if (question.Answer == qAnswer)
                                _correctAnswer[_index] = true;
                            else
                                _correctAnswer[_index] = false;
                            if (_index < _quiz.Components.Count - 1)
                            {
                                Question nextQuestion = _quiz.Components[++_index] as Question;
                                CreateQuestion(nextQuestion);
                            }
                            else if (_index == _quiz.Components.Count - 1)
                            {
                                ShowResults();
                            } 
                        }
                    };
                    Grid.Children.Add(choiceQuestion);
                    choiceQuestion.HorizontalAlignment = HorizontalAlignment.Center;
                    choiceQuestion.VerticalAlignment = VerticalAlignment.Top;
                    if (_index == _quiz.Components.Count - 1)
                        choiceQuestion.button.Content = "Finish";
                    break;
            }
        }

        private void ShowResults()
        {
            Grid.Children.Clear();
            QuizResults quizResults = new QuizResults(_quiz, _correctAnswer, _answer);
            Grid.Children.Add(quizResults);
            quizResults.HorizontalAlignment = HorizontalAlignment.Stretch;
            quizResults.VerticalAlignment = VerticalAlignment.Top;
            quizResults.Margin = new Thickness(10);
        }
    }
}
