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

        public QuizViewer()
        {
            InitializeComponent();
        }

        public void SetContent(Quiz quiz)
        {
            _quiz = quiz;
        }
    }
}
