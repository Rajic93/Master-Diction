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
    /// Interaction logic for QuizResults.xaml
    /// </summary>
    public partial class QuizResults : UserControl
    {
        private List<Tuple<string, string, bool>> Questions { get; set; }

        public QuizResults(Quiz quiz, bool[] correctAnswers, string[] answers)
        {
            Questions = new List<Tuple<string, string, bool>>();
            for (int i = 0; i < quiz.Components.Count; i++)
            {
                Question question = quiz.Components[i] as Question;
                if (question != null)
                    Questions.Add(new Tuple<string, string, bool>(question.Text, answers[i], correctAnswers[i]));
            }
            InitializeComponent();
            ItemsControl.ItemsSource = Questions;
        }
    }
}
