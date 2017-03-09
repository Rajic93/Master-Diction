using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for PuzzleQuestion.xaml
    /// </summary>
    public partial class PuzzleQuestion : UserControl
    {
        private bool _isDown;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _realDragSource;
        private UIElement _dummyDragSource = new UIElement();

        private Question _question;
        private List<string> Pieces { get; set; }

        public PuzzleQuestion(Question question)
        {
            _question = question;
            InitializeComponent();

            Pieces = new List<string>();
            foreach (string piece in _question.Pieces)
            {

                Pieces.Add(piece);
            }
            var rnd = new Random();
            var res = Pieces.OrderBy(item => rnd.Next());
            textBlock.Text = _question.Text;
            foreach (string puzzlePiece in res)
            {
                Button button = new Button()
                {
                    Content = puzzlePiece,
                    Height = 25,
                    Width = double.NaN,
                    Margin = new Thickness(5,5,0,0)
                };
                button.Click += delegate(object sender, RoutedEventArgs args)
                {
                    Button btn = sender as Button;
                    if (btn != null)
                    {
                        if (PiecesContainer.Children.Contains(btn))
                        {
                            PiecesContainer.Children.Remove(btn);
                            Answer.Children.Add(btn);
                        }
                        else if (Answer.Children.Contains(btn))
                        {
                            Answer.Children.Remove(btn);
                            PiecesContainer.Children.Add(btn);
                        }
                    }
                };
                PiecesContainer.Children.Add(button);
            }
        }

        public string GetAnswer()
        {
            string answer = "";
            foreach (Button answerChild in Answer.Children)
            {
                answer += answerChild.Content + " ";
            }
            if (answer != "")
            {
                answer = answer.Substring(0, answer.Length - 1); 
            }
            return answer;
        }
    }
}
