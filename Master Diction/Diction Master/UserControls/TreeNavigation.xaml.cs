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
    /// Interaction logic for TreeNavigation.xaml
    /// </summary>
    public partial class TreeNavigation : UserControl
    {
        public Course Course { get; set; }


        public TreeNavigation()
        {
            InitializeComponent();
        }

        public void Render()
        {
            foreach (CompositeComponent compositeComponent in Course.Components)
            {
                
            }
        }
    }
}
