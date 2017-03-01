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

namespace Diction_Master___Library.UserControls
{
    /// <summary>
    /// Interaction logic for TermSelection.xaml
    /// </summary>
    public partial class TermSelection : UserControl
    {
        private List<Component> _availableTerms;
        private int _selectedTerm;

        public TermSelection()
        {
            _availableTerms = new List<Component>();
            InitializeComponent();
        }

        internal void SetAvailableTerms(List<Component> components)
        {
            foreach (Week item in components)
            {
                switch (item.Term)
                {
                    case 1:
                        TermI.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        TermII.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        TermIII.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
        }

        internal int GetSelectedTerm()
        {
            return _selectedTerm;
        }

        private void TermI_Click(object sender, RoutedEventArgs e)
        {
            _selectedTerm = 1;
        }

        private void TermII_Click(object sender, RoutedEventArgs e)
        {
            _selectedTerm = 2;
        }

        private void TermIII_Click(object sender, RoutedEventArgs e)
        {
            _selectedTerm = 3;
        }
    }
}
