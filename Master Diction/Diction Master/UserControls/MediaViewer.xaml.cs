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
using Diction_Master.UserControls;

namespace Diction_Master___Library.UserControls
{
    /// <summary>
    /// Interaction logic for ContentViewer.xaml
    /// </summary>
    public partial class MediaViewer : UserControl
    {
        private ContentFile _contentFile;
        private bool _pause;
        private bool _stop;

        public MediaViewer()
        {
            InitializeComponent();
            MediaElement.LoadedBehavior = MediaState.Manual;
        }

        public void SetContent(ContentFile contentFile)
        {
            _contentFile = contentFile;
            Desrciption.Text = _contentFile.Description;
            if (_contentFile.URI == "")
                return;
            MediaElement.Source = new Uri(_contentFile.URI, UriKind.Relative);
            image.Visibility = Visibility.Visible;
            if (_contentFile.ComponentType == ComponentType.Audio)
                fullscreen.Visibility = Visibility.Collapsed;
        }

        private void Play_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_pause)
            {
                MediaElement.LoadedBehavior = MediaState.Stop;
                if (MediaElement.Source == null)
                    MediaElement.Source = new Uri(_contentFile.URI, UriKind.Relative);
                MediaElement.LoadedBehavior = MediaState.Play;
                MediaElement.MediaOpened += (o, args) =>
                {
                    ProgressBar.Maximum = MediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                    ProgressBar.SmallChange = ProgressBar.Maximum / 10000;
                    ProgressBar.LargeChange = ProgressBar.Maximum / 1000;
                };
                slider.Value = MediaElement.Volume * 20;
                image.Source = new BitmapImage(new Uri("../Resources/pause.png", UriKind.Relative)); 
                if (_contentFile.ComponentType == ComponentType.Video)
                    image.Visibility = Visibility.Collapsed;
            }
            else
            {
                MediaElement.LoadedBehavior = MediaState.Play;
                _pause = false;
            }
            _stop = false;
        }

        private void Pause_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_stop)
                return;
            _pause = true;
            MediaElement.LoadedBehavior = MediaState.Pause;
            image.Source = new BitmapImage(new Uri("../Resources/play.png", UriKind.Relative));
        }

        private void Stop_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            MediaElement.LoadedBehavior = MediaState.Stop;
            MediaElement.Source = null;
            image.Source = new BitmapImage(new Uri("../Resources/play.png", UriKind.Relative));
            if (_contentFile.ComponentType == ComponentType.Video)
                image.Visibility = Visibility.Visible;
            _stop = true;
        }

        private void Fullscreen_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MediaElement.Position.Ticks == 0)
                return;
            MediaElement.LoadedBehavior = MediaState.Pause;
            FullScreenVideo full = new FullScreenVideo(_contentFile.URI, MediaElement.Position.Ticks, this);
            full.ShowDialog();
            MediaElement.LoadedBehavior = MediaState.Play;
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
                if (MediaElement.Source == null)
                    MediaElement.Source = new Uri(_contentFile.URI, UriKind.Relative);
                MediaElement.LoadedBehavior = MediaState.Play;
                image.Source = new BitmapImage(new Uri("../Resources/pause.png", UriKind.Relative));
                if (_contentFile.ComponentType == ComponentType.Video)
                    image.Visibility = Visibility.Collapsed;
                _stop = false;
            }
            else if (name.Contains("pause.png"))
            {
                MediaElement.LoadedBehavior = MediaState.Pause;
                image.Source = new BitmapImage(new Uri("../Resources/play.png", UriKind.Relative));
            }
        }

        public void SetPosition(long ticks)
        {
            MediaElement.Position = new TimeSpan(ticks);
        }
    }
}
