using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Master_Diction.User_Controlls
{
    public partial class LessonControl : UserControl
    {

        public LessonControl()
        {
            InitializeComponent();
        }

        private void LessonControl_Load(object sender, EventArgs e)
        {
            string path;
#if DEBUG
            path = "Diction Exercise.mp4";
#else
            path = "Resources\\Diction Exercise.mp4";
#endif
            axWindowsMediaPlayer1.URL = path;
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        public void StopMedia()
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }
    }
}
