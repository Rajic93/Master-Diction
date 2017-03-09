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
            string extension = _contentFile.URI.Split('\\').Last().Split('.').Last();
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "All Files (*.*)|*.*";
            dialog.AddExtension = true;
            dialog.DefaultExt = "." + extension;
            dialog.FileName = _contentFile.Title;
            bool? res = dialog.ShowDialog();
            if (res != null && res == true)
            {
                byte[] stream = File.ReadAllBytes(_contentFile.URI);
                File.WriteAllBytes(dialog.FileName, stream);
            }
        }

        private void Download_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var docImage = sender as Image;
            if (docImage == null) return;
            docImage.Opacity = 1;
            image.Opacity = docImage.Opacity;
        }

        private void Download_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var docImage = sender as Image;
            if (docImage == null) return;
            docImage.Opacity = 0.8;
            image.Opacity = docImage.Opacity;
        }

        public void SetContent(ContentFile contentFile)
        {
            _contentFile = contentFile;
            Desrciption.Text = _contentFile.Description;
        }
    }
}
