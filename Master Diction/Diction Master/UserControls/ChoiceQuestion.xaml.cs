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
    /// Interaction logic for ChoiceQuestion.xaml
    /// </summary>
    public partial class ChoiceQuestion : UserControl
    {
        private string _answer = "";
        private List<RadioButton> _radioButtons;

        public ChoiceQuestion(Question question)
        {
            var question1 = question;
            InitializeComponent();
            _radioButtons = new List<RadioButton>();
            foreach (string wrongAnswer in question1.WrongAnswers)
            {
                RadioButton radioButton = new RadioButton()
                {
                    Content = new TextBlock()
                    {
                        Text = wrongAnswer,
                        TextWrapping = TextWrapping.Wrap
                    }
                };
                radioButton.Checked += delegate(object sender, RoutedEventArgs args)
                {
                    var rb = sender as RadioButton;
                    if (rb != null) _answer = rb.Content.ToString();
                };
                _radioButtons.Add(radioButton);
            }
            RadioButton answerRadioButton = new RadioButton()
            {
                Content = new TextBlock()
                {
                    Text = question1.Answer,
                    TextWrapping = TextWrapping.Wrap
                }
            };
            answerRadioButton.Checked += delegate (object sender, RoutedEventArgs args)
            {
                var rb = sender as RadioButton;
                if (rb != null) _answer = rb.Content.ToString();
            };
            _radioButtons.Add(answerRadioButton);
            var rnd = new Random();
            var res = _radioButtons.OrderBy(item => rnd.Next());
            _radioButtons = res as List<RadioButton>;
            textBlock.Text = question1.Text;
            foreach (RadioButton radioButton in res)
            {
                StackPanel.Children.Add(radioButton);
            }
        }

        public string GetAnswer()
        {
            return _answer;
        }
    }
}
