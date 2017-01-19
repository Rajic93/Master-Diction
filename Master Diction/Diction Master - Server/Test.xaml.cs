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
using System.Windows.Shapes;
using Diction_Master___Library;

namespace Diction_Master___Server
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        private Diction_Master___Library.ContentManager manager;

        public Test()
        {
            manager = Diction_Master___Library.ContentManager.CreateInstance();
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            manager.AddCourse("Serbian", "");
            manager.EditCourse(1, null, "./Resources/Flag of Serbia.png");
            
            //manager.AddEducationalLevel(Icons.Nursery, 2);
            manager.AddEducationalLevel(1, "", EducationalLevelType.Nursery);
            manager.EditEducationalLevel(2, "./Resources/nursery.png", 0);
            //manager.DeleteEducationalLevel(2, 1);

            manager.AddGrade(2, "", GradeType.NurseryI);
            manager.EditGrade(3, "", GradeType.NurseryI);
            //manager.DeleteGrade(3, 2);

            manager.AddWeek(3, "", 1, 1);
            manager.EditWeek(4, "Week 1", 1, 1);
            //manager.DeleteWeek(4, 3);

            manager.AddLesson(4, "", 1);
            manager.EditLesson(5, "Lesson 1", 1);
            //manager.DeleteLesson(5, 4);

            manager.AddQuiz(5, "");
            manager.EditQuiz(6, "Quiz 1");
            //manager.DeleteQuiz(6, 5);

            manager.AddQuestion(6, "How old are you?", "23", QuestionType.Text);
            manager.EditQuestion(7, "How old are you", "I am 24 years old.", QuestionType.Puzzle);
            //manager.DeleteQuestion(7, 6);

            manager.AddContentFile(5, ComponentType.Audio, "", "", "", 2, "");
            manager.EditContentFile(8, ComponentType.Audio, "", "", "", 2, "HEHEHE");
            //manager.DeleteContentFile(8, 5);
         }
    } 
}
