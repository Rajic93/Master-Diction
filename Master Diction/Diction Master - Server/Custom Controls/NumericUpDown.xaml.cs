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

namespace Diction_Master___Server.Custom_Controls
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public int Value { get; set; }
        private int previousValue;

        public NumericUpDown()
        {
            Value = 0;
            previousValue = 0;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            textBox.Text = Value.ToString();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ContainsNonNumeric(textBox.Text))
            {
                textBox.Text = previousValue.ToString();
            }
        }

        private bool ContainsNonNumeric(string textBoxText)
        {
            foreach (char character in textBoxText)
            {
                if (character < '0' || character > '9')
                {
                    return true;
                }
            }
            return false;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            previousValue = Value;
            textBox.Text = (++Value).ToString();
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            previousValue = Value;
            textBox.Text = (--Value).ToString();
        }
    }
}
