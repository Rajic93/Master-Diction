using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            axWindowsMediaPlayer1.URL = Directory.GetCurrentDirectory() + "\\Diction Exercise.mp4";
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }
    }
}
