using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for ContentViewer.xaml
    /// </summary>
    public partial class MediaViewer : UserControl
    {
        private ContentFile _contentFile;
        private Timer _timer;
        private object _content;
        private object _logicalChildren;

        public MediaViewer()
        {
            InitializeComponent();
            MediaElement.LoadedBehavior = MediaState.Manual;
            _timer = new Timer();
        }

        public void SetContent(ContentFile contentFile)
        {
            _contentFile = contentFile;
            Desrciption.Text = _contentFile.Description;
            if (_contentFile.URI != "")
                MediaElement.Source = new Uri(_contentFile.URI, UriKind.Relative);
            if (contentFile.ComponentType == ComponentType.Audio)
            {
                image.Visibility = Visibility.Visible;
            }
            //MediaElement.Pause();
        }

        private void Play_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            MediaElement.Stop();
            MediaElement.Play();
            MediaElement.MediaOpened += (o, args) =>
            {
                ProgressBar.Maximum = MediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                ProgressBar.SmallChange = ProgressBar.Maximum / 10000;
                ProgressBar.LargeChange = ProgressBar.Maximum / 1000;
            };
            slider.Value = MediaElement.Volume * 20;
            image.Source = new BitmapImage(new Uri("../Resources/pause.png", UriKind.Relative));
            //_timer.Start();
        }

        private void Pause_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            MediaElement.Pause();
            image.Source = new BitmapImage(new Uri("../Resources/play.png", UriKind.Relative));
        }

        private void Stop_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            MediaElement.Stop();
            image.Source = new BitmapImage(new Uri("../Resources/play.png", UriKind.Relative));
        }

        private void Fullscreen_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //Window window = new Window();
            //window.WindowState = WindowState.Maximized;
            //window.WindowStyle = WindowStyle.None;
            //_content = Content;
            //window.KeyUp += delegate(object o, KeyEventArgs args)
            //{
            //    if (args.Key == Key.Escape)
            //    {
            //        Content = _content;
                    
            //        window.Close();
            //    }
            //};
            //window.Show();
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaElement.Volume = slider.Value / 20;
        }

        private void Image_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            string name = image.Source.ToString();
            if (name.Contains("play.png"))
            {
                MediaElement.Play();
                image.Source = new BitmapImage(new Uri("../Resources/pause.png", UriKind.Relative));
            }
            else if (name.Contains("pause.png"))
            {
                MediaElement.Pause();
                image.Source = new BitmapImage(new Uri("../Resources/play.png", UriKind.Relative));
            }
        }
    }
}
