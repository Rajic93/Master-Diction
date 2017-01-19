using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Diction_Master___Library;

namespace Diction_Master___Server.Custom_Controls
{
    /// <summary>
    /// Interaction logic for WeeksCreation.xaml
    /// </summary>
    public partial class WeeksCreation : UserControl
    {
        private ObservableCollection<Component> TermI;
        private ObservableCollection<Component> TermII;
        private ObservableCollection<Component> TermIII;
        private bool SavedI;
        private bool SavedII;
        private bool SavedIII;
        private bool emptyI = true;
        private bool emptyII = true;
        private bool emptyIII = true;

        public WeeksCreation()
        {
            TermI = new ObservableCollection<Component>();
            TermII = new ObservableCollection<Component>();
            TermIII = new ObservableCollection<Component>();
            InitializeComponent();
        }

        public bool IsEmpty()
        {
            return emptyI && emptyII && emptyIII;
        }

        public bool IsSaved()
        {
            return SavedI && SavedII && SavedIII;
        }

        public ObservableCollection<Component> GetTerm(int term)
        {
            switch (term)
            {
                case 1:
                    return TermI;
                case 2:
                    return TermII;
                case 3:
                    return TermIII;
                default:
                    return null;
            }
        }

        private void listBoxTermI_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxTermI.SelectedItem != null)
            {
                textBoxI.Text = ((Week)listBoxTermI.SelectedItem).Title;
                comboBox.Text = ((Week) listBoxTermI.SelectedItem).Num.ToString();
            }

        }

        private void listBoxTermII_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxTermII.SelectedItem != null)
            {
                textBoxII.Text = ((Week)listBoxTermII.SelectedItem).Title;
                comboBox1.Text = ((Week)listBoxTermII.SelectedItem).Num.ToString();
            }
        }

        private void listBoxTermIII_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxTermIII.SelectedItem != null)
            {
                textBoxIII.Text = ((Week)listBoxTermIII.SelectedItem).Title;
                comboBox2.Text = ((Week)listBoxTermIII.SelectedItem).Num.ToString();
            }
        }

        private void AddI_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxI.Text != "")
            {
                //TermI.Add((Week)ContentFactory.CreateCompositeComponent(ComponentType.Week, Convert.ToInt16(comboBox.SelectedValue), 1, textBoxI.Text));
                SavedI = false;
                ConfirmI.IsEnabled = true;
                emptyI = false;
            }
        }

        private void AddII_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxII.Text != "")
            {
                //TermII.Add((Week)ContentFactory.CreateCompositeComponent(ComponentType.Week, Convert.ToInt16(comboBox1.SelectedValue), 2, textBoxII.Text));
                SavedII = false;
                ConfirmII.IsEnabled = true;
                emptyII = false;
            }
        }

        private void AddIII_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxIII.Text != "")
            {
                //TermIII.Add((Week)ContentFactory.CreateCompositeComponent(ComponentType.Week, Convert.ToInt16(comboBox2.SelectedValue), 3, textBoxIII.Text));
                SavedIII = false;
                ConfirmIII.IsEnabled = true;
                emptyIII = false;
            }
        }

        private void EditI_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxTermI.SelectedItem != null)
            {
                ((Week) listBoxTermI.SelectedItem).Title = textBoxI.Text;
                ((Week) listBoxTermI.SelectedItem).Num = Convert.ToInt16(comboBox.SelectedValue);
                listBoxTermI.Items.Refresh();
                SavedI = false;
                ConfirmI.IsEnabled = true;
            }
        }

        private void EditII_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxTermII.SelectedItem != null)
            {
                ((Week) listBoxTermII.SelectedItem).Title = textBoxII.Text;
                ((Week)listBoxTermII.SelectedItem).Num = Convert.ToInt16(comboBox1.SelectedValue);
                listBoxTermII.Items.Refresh();
                SavedII = false;
                ConfirmII.IsEnabled = true;
            }
        }

        private void EditIII_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxTermIII.SelectedItem != null)
            {
                ((Week) listBoxTermIII.SelectedItem).Title = textBoxIII.Text;
                ((Week)listBoxTermIII.SelectedItem).Num = Convert.ToInt16(comboBox2.SelectedValue);
                listBoxTermIII.Items.Refresh();
                SavedIII = false;
                ConfirmIII.IsEnabled = true;
            }
        }

        private void DeleteI_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxTermI.SelectedItem != null)
            {
                listBoxTermI.Items.RemoveAt(listBoxTermI.SelectedIndex);
                textBoxI.Text = "";
                SavedI = false;
                ConfirmI.IsEnabled = true;
                if (TermII.Count == 0 && TermI.Count == 0 && TermIII.Count == 0)
                {
                    emptyI = true;
                }
            }
        }

        private void DeleteII_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxTermII.SelectedItem != null)
            {
                listBoxTermII.Items.RemoveAt(listBoxTermII.SelectedIndex);
                textBoxII.Text = "";
                SavedII = false;
                ConfirmII.IsEnabled = true;
                if (TermII.Count == 0 && TermI.Count == 0 && TermIII.Count == 0)
                {
                    emptyII = true;
                }
            }
        }

        private void DeleteIII_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxTermIII.SelectedItem != null)
            {
                listBoxTermIII.Items.RemoveAt(listBoxTermIII.SelectedIndex);
                textBoxIII.Text = "";
                SavedIII = false;
                ConfirmIII.IsEnabled = true;
                if (TermII.Count == 0 && TermI.Count == 0 && TermIII.Count == 0)
                {
                    emptyIII = true;
                }
            }
        }

        private void ConfirmI_Click(object sender, RoutedEventArgs e)
        {
            SavedI = true;
            ConfirmI.IsEnabled = false;
        }

        private void ConfirmII_Click(object sender, RoutedEventArgs e)
        {
            SavedII = true;
            ConfirmII.IsEnabled = false;
        }

        private void ConfirmIII_Click(object sender, RoutedEventArgs e)
        {
            SavedIII = true;
            ConfirmIII.IsEnabled = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                comboBox.Items.Add((i + 1).ToString());
                comboBox1.Items.Add((i + 1).ToString());
                comboBox2.Items.Add((i + 1).ToString());
            }
            comboBox2.Text = 1.ToString();
            comboBox1.Text = 1.ToString();
            comboBox.Text = 1.ToString();
            listBoxTermI.ItemsSource = TermI;
            listBoxTermI.DisplayMemberPath = "Title";
            listBoxTermII.ItemsSource = TermII;
            listBoxTermII.DisplayMemberPath = "Title";
            listBoxTermIII.ItemsSource = TermIII;
            listBoxTermIII.DisplayMemberPath = "Title";
        }
    }
}
