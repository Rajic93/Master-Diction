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
    /// Interaction logic for Question.xaml
    /// </summary>
    public partial class TextQuestion : UserControl
    {
        private Question _question;

        public TextQuestion(Question question)
        {
            _question = question;
            InitializeComponent();
            Typeface myTypeface = new Typeface("Segoe UI");
            FormattedText ft = new FormattedText(_question.Text, CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, myTypeface, 16, Brushes.Black);
            Size textSize = new Size(ft.Width, ft.Height);
            QuestionText.Width = textSize.Width;
            QuestionText.Height = textSize.Height;
            Answer.Margin = new Thickness(10, QuestionText.Height + 20, 10, 10);
        }

    }
}
