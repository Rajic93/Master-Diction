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
using System.Windows.Shapes;
using Diction_Master___Library.UserControls;

namespace Diction_Master.UserControls
{
    /// <summary>
    /// Interaction logic for FullScreenVideo.xaml
    /// </summary>
    public partial class FullScreenVideo : Window
    {
        private MediaViewer _mediaViewer;

        public FullScreenVideo(string uri, long time, MediaViewer mediaViewer)
        {
            _mediaViewer = mediaViewer;
            InitializeComponent();
            MediaElement.Source = new Uri(uri, UriKind.Relative);
            MediaElement.Position = new TimeSpan(time);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _mediaViewer.SetPosition(MediaElement.Position.Ticks);
                Close();
            }
            else if (e.Key == Key.Space)
            {
                if (MediaElement.LoadedBehavior == MediaState.Play)
                    MediaElement.LoadedBehavior = MediaState.Pause;
                else if (MediaElement.LoadedBehavior == MediaState.Pause)
                    MediaElement.LoadedBehavior = MediaState.Play;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MediaElement.Width = Width;
            MediaElement.Height = Height;
        }

        private void MediaElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MediaElement.LoadedBehavior == MediaState.Play)
                MediaElement.LoadedBehavior = MediaState.Pause;
            else if (MediaElement.LoadedBehavior == MediaState.Pause)
                MediaElement.LoadedBehavior = MediaState.Play;
        }
    }
}
