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
            ContentUpload.LoadWeeks();
            for (int i = 0; i < 5; i++)
            {
                ContentUpload.lessons.Add(new Lesson()
                {
                    ID = i,
                    Num = i + 1,
                    Title = "Lesson " + (i + 1)
                });
            }
        }
    }
}
