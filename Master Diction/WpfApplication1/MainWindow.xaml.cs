using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HashSet<string> oneTermKeys { get; set; }
        public HashSet<string> fullYearKeys { get; set; }
        public HashSet<string> keys { get; set; }
        private object lock1 = new object();
        private object lock2 = new object();
        private bool go = false;

        public MainWindow()
        {
            oneTermKeys = new HashSet<string>();
            fullYearKeys = new HashSet<string>();
            keys = new HashSet<string>();
            InitializeComponent();
            RandomString();
        }
        
        public void RandomString()
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            
            for (int i = 0; i < 10; i++)
            {
                new Thread(() =>
                {
                    for (int j = 0; j < 100000; j++)
                    {
                        lock(lock1)
                            keys.Add(new string(Enumerable.Repeat(chars, 30).Select(s => s[random.Next(s.Length)]).ToArray()));
                    }
                    lock (lock1)
                    {
                        if (keys.Count == 1000000)
                            go = true;
                    }
                }).Start();
            }
            while (!go){ }
            go = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int j = 0; j < 10; j++)
            {
                new Thread(() =>
                {
                    for (int i = 0; i < 50000; i++)
                    {
                        lock (lock1)
                        {
                            oneTermKeys.Add(keys.First());
                            keys.Remove(keys.First());
                            fullYearKeys.Add(keys.First());
                            keys.Remove(keys.First());
                        }
                    }
                    lock (lock1)
                    {
                        if (keys.Count == 0)
                            go = true;
                    }
                }).Start();
            }
            while (!go) { }
            go = false;
            new Thread(() =>
            {
                TextWriter writer = new StreamWriter("One-term keys.txt");
                foreach (string oneTermKey in oneTermKeys)
                {
                    Dispatcher.Invoke(() =>
                    {
                        listBox.Items.Add(oneTermKey);
                        writer.WriteLine(oneTermKey);
                    });
                }
            }).Start();
            new Thread(() =>
            {
                TextWriter writer = new StreamWriter("full-year keys.txt");
                foreach (string fullYearKey in fullYearKeys)
                {
                    Dispatcher.Invoke(() =>
                    {
                        listBox1.Items.Add(fullYearKey);
                        writer.WriteLine(fullYearKey);
                    });
                }
            }).Start();
        }
    }
}
