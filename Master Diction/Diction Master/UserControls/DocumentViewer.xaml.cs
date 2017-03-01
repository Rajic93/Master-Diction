using System;
using System.Collections.Generic;
using System.IO;
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
using Diction_Master___Library;
using Microsoft.Win32;

namespace Diction_Master.UserControls
{
    /// <summary>
    /// Interaction logic for DocumentViewer.xaml
    /// </summary>
    public partial class DocumentViewer : UserControl
    {
        private ContentFile _contentFile;

        public DocumentViewer()
        {
            InitializeComponent();
        }

        private void Download_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            bool? res = dialog.ShowDialog();
            if (res != null && res == true)
            {
                byte[] stream = File.ReadAllBytes(_contentFile.URI);
                File.WriteAllBytes(dialog.FileName, stream);
            }
        }

        private void Download_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var image = sender as Image;
            if (image != null) image.Opacity = 1;
        }

        private void Download_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var image = sender as Image;
            if (image != null) image.Opacity = 0.8;
        }

        public void SetContent(ContentFile contentFile)
        {
            _contentFile = contentFile;
            Desrciption.Text = _contentFile.Description;
        }
    }
}
