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
        private Diction_Master___Library.ContentManager _contentManager;
        private ObservableCollection<Component> TermI;
        private ObservableCollection<Component> TermII;
        private ObservableCollection<Component> TermIII;
        private bool SavedI = true;
        private bool SavedII = true;
        private bool SavedIII = true;
        private bool emptyI = true;
        private bool emptyII = true;
        private bool emptyIII = true;

        private long _selecetedGrade;

        public WeeksCreation(long parentID, Diction_Master___Library.ContentManager manager)
        {
            _contentManager = manager;
            TermI = new ObservableCollection<Component>();
            TermII = new ObservableCollection<Component>();
            TermIII = new ObservableCollection<Component>();
            _selecetedGrade = parentID;
            LoadWeeks();
            InitializeComponent();
        }

        private void LoadWeeks()
        {
            foreach (Week week in _contentManager.GetAllWeeks(_contentManager.GetComponent(_selecetedGrade)))
            {
                if (week.Term == 1)
                    TermI.Add(week);
                else if (week.Term == 2)
                    TermII.Add(week);
                else if (week.Term == 3)
                    TermIII.Add(week);
            }
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
                EditI.IsEnabled = true;
                DeleteI.IsEnabled = true;
            }

        }

        private void listBoxTermII_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxTermII.SelectedItem != null)
            {
                textBoxII.Text = ((Week)listBoxTermII.SelectedItem).Title;
                comboBox1.Text = ((Week)listBoxTermII.SelectedItem).Num.ToString();
                EditII.IsEnabled = true;
                DeleteII.IsEnabled = true;
            }
        }

        private void listBoxTermIII_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxTermIII.SelectedItem != null)
            {
                textBoxIII.Text = ((Week)listBoxTermIII.SelectedItem).Title;
                comboBox2.Text = ((Week)listBoxTermIII.SelectedItem).Num.ToString();
                EditIII.IsEnabled = true;
                DeleteIII.IsEnabled = true;
            }
        }

        private void AddI_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxI.Text != "")
            {
                long id = _contentManager.AddWeek(_selecetedGrade, textBoxI.Text, Convert.ToInt16(comboBox.SelectedValue), 1);
                if (id > 0)
                {
                    TermI.Add(_contentManager.GetComponent(id) as Week);
                    listBoxTermI.Items.Refresh();
                    SavedI = false;
                    ConfirmI.IsEnabled = true;
                    emptyI = false;
                }
            }
        }

        private void AddII_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxII.Text != "")
            {
                long id = _contentManager.AddWeek(_selecetedGrade, textBoxII.Text, Convert.ToInt16(comboBox1.SelectedValue), 2);
                if (id > 0)
                {
                    TermII.Add(_contentManager.GetComponent(id) as Week);
                    listBoxTermII.Items.Refresh();
                    SavedII = false;
                    ConfirmII.IsEnabled = true;
                    emptyII = false;
                }
            }
        }

        private void AddIII_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxIII.Text != "")
            {
                long id = _contentManager.AddWeek(_selecetedGrade, textBoxIII.Text, Convert.ToInt16(comboBox2.SelectedValue), 3);
                if (id > 0)
                {
                    TermIII.Add(_contentManager.GetComponent(id) as Week);
                    listBoxTermIII.Items.Refresh();
                    SavedIII = false;
                    ConfirmIII.IsEnabled = true;
                    emptyIII = false;
                }
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
                _contentManager.DeleteWeek((listBoxTermI.SelectedItem as Week).ID, _selecetedGrade);
                TermI.Remove(listBoxTermI.SelectedItem as Week);
                textBoxI.Text = "";
                SavedI = false;
                EditI.IsEnabled = false;
                DeleteI.IsEnabled = false;
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
                _contentManager.DeleteWeek((listBoxTermII.SelectedItem as Week).ID, _selecetedGrade);
                TermII.Remove(listBoxTermII.SelectedItem as Week);
                textBoxII.Text = "";
                SavedII = false;
                EditII.IsEnabled = false;
                DeleteII.IsEnabled = false;
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
                _contentManager.DeleteWeek((listBoxTermIII.SelectedItem as Week).ID, _selecetedGrade);
                TermIII.Remove(listBoxTermIII.SelectedItem as Week);
                textBoxIII.Text = "";
                SavedIII = false;
                EditIII.IsEnabled = false;
                DeleteIII.IsEnabled = false;
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
