﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
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
using System.Windows.Shapes;
using Diction_Master___Library;
using Diction_Master___Server.Custom_Controls;

namespace Diction_Master___Server
{
    /// <summary>
    /// Interaction logic for ContentManager.xaml
    /// </summary>
    public partial class ContentManager : Window
    {
        private int active_section = 0;
        private Diction_Master___Library.ContentManager manager;
        private MainWindow main;
        private Image selectedLanguage;

        public ContentManager(MainWindow main)
        {
            this.main = main;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            content.Children.Add(new LanguageSelection()
            {
                RenderSize = content.RenderSize,
                Visibility = Visibility.Visible
            });
            content.Children.Add(new LevelSelection()
            {
                RenderSize = content.RenderSize,
                Visibility = Visibility.Collapsed
            });
            content.Children.Add(new WeeksCreation()
            {
                RenderSize = content.RenderSize,
                Visibility = Visibility.Collapsed
            });
            content.Children.Add(new LessonsCreation()
            {
                RenderSize = content.RenderSize,
                Visibility = Visibility.Collapsed
            });
            manager = Diction_Master___Library.ContentManager.CreateInstance(ClientManager.CreateInstance());
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Labela.Text = "Choose language";
        }

        private void TextBlock_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            //Labela.Text = "Define Weeks";
        }

        private void TextBlock_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            //Labela.Text = "Define Lessons";
        }

        private void TextBlock_MouseUp_3(object sender, MouseButtonEventArgs e)
        {
            //Labela.Text = "Define Content";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            main.Show();
        }

        private void image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedLanguage != null)
            {
                selectedLanguage.Opacity = 0.5;
            }
            selectedLanguage = (Image) sender;
            selectedLanguage.Opacity = 1;
        }

        private void Next_OnClick(object sender, RoutedEventArgs e)
        {
            switch (active_section)
            {
                case 0:
                    selectedLanguage = ((LanguageSelection)content.Children[0]).selectedLanguage;
                    Previous.Visibility = Visibility.Visible;
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    Next.Visibility = Visibility.Collapsed;
                    SaveCourse.Visibility = Visibility.Visible;
                    break;
            }
            if (active_section != 3)
            {
                content.Children[active_section].Visibility = Visibility.Collapsed;
                content.Children[++active_section].Visibility = Visibility.Visible;
            }
        }

        private void SaveCourse_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void Previous_OnClick(object sender, RoutedEventArgs e)
        {
            switch (active_section)
            {
                case 1:
                    selectedLanguage = ((LanguageSelection)content.Children[0]).selectedLanguage;
                    Previous.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    Next.Visibility = Visibility.Visible;
                    SaveCourse.Visibility = Visibility.Collapsed;
                    break;
            }
            if (active_section != 0)
            {
                content.Children[active_section].Visibility = Visibility.Collapsed;
                content.Children[--active_section].Visibility = Visibility.Visible;
            }
        }
    }
}