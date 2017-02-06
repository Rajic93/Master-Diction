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
    /// Interaction logic for RegistrationKey.xaml
    /// </summary>
    public partial class RegistrationKey : UserControl
    {
        public RegistrationKey()
        {
            InitializeComponent();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text.Length == 4)
            {
                textBox1.Focus();
            }
        }

        private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text.Length == 4)
            {
                textBox2.Focus();
            }
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text.Length == 4)
            {
                textBox3.Focus();
            }
        }

        private void textBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text.Length == 4)
            {
                textBox4.Focus();
            }
        }

        private void textBox4_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text.Length == 4)
            {
                textBox5.Focus();
            }
        }

        private void textBox5_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text.Length == 4)
            {
                button.Focus();
            }
        }
    }
}
