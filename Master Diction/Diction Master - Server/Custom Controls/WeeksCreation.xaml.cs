using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
    /// Interaction logic for WeeksCreation.xaml
    /// </summary>
    public partial class WeeksCreation : UserControl
    {
        public List<string> TermI;
        public List<string> TermII;
        public List<string> TermIII;
        private bool savedI;
        private bool savedII;
        private bool savedIII;

        public WeeksCreation()
        {
            TermI = new List<string>();
            TermII = new List<string>();
            TermIII = new List<string>();
            InitializeComponent();
        }

        private void listBoxTermI_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxTermI.SelectedItem != null)
                textBoxI.Text = listBoxTermI.SelectedItem.ToString();
        }

        private void listBoxTermII_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textBoxII.Text = listBoxTermII.SelectedItem.ToString();
        }

        private void listBoxTermIII_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textBoxIII.Text = listBoxTermIII.SelectedItem.ToString();
        }

        private void AddI_Click(object sender, RoutedEventArgs e)
        {
            listBoxTermI.Items.Add(textBoxI.Text);
            savedI = false;
        }

        private void AddII_Click(object sender, RoutedEventArgs e)
        {
            listBoxTermII.Items.Add(textBoxII.Text);
            savedII = false;
        }

        private void AddIII_Click(object sender, RoutedEventArgs e)
        {
            listBoxTermIII.Items.Add(textBoxIII.Text);
            savedIII = false;
        }

        private void EditI_Click(object sender, RoutedEventArgs e)
        {
            listBoxTermI.Items.RemoveAt(listBoxTermI.SelectedIndex);
            listBoxTermI.Items.Add(textBoxI.Text);
            savedI = false;
        }

        private void EditII_Click(object sender, RoutedEventArgs e)
        {
            listBoxTermII.Items.RemoveAt(listBoxTermII.SelectedIndex);
            listBoxTermII.Items.Add(textBoxII.Text);
            savedII = false;
        }

        private void EditIII_Click(object sender, RoutedEventArgs e)
        {
            listBoxTermIII.Items.RemoveAt(listBoxTermIII.SelectedIndex);
            listBoxTermIII.Items.Add(textBoxIII.Text);
            savedIII = false;
        }

        private void DeleteI_Click(object sender, RoutedEventArgs e)
        {
            listBoxTermI.Items.RemoveAt(listBoxTermI.SelectedIndex);
            textBoxI.Text = "";
            savedI = false;
        }

        private void DeleteII_Click(object sender, RoutedEventArgs e)
        {
            listBoxTermII.Items.RemoveAt(listBoxTermII.SelectedIndex);
            textBoxII.Text = "";
            savedII = false;
        }

        private void DeleteIII_Click(object sender, RoutedEventArgs e)
        {
            listBoxTermIII.Items.RemoveAt(listBoxTermIII.SelectedIndex);
            textBoxIII.Text = "";
            savedIII = false;
        }

        private void ConfirmI_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listBoxTermI.Items)
            {
                TermI.Add(item.ToString());
            }
            savedI = true;
        }

        private void ConfirmII_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listBoxTermII.Items)
            {
                TermII.Add(item.ToString());
            }
            savedII = true;
        }

        private void ConfirmIII_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listBoxTermIII.Items)
            {
                TermIII.Add(item.ToString());
            }
            savedIII = true;
        }
    }
}
