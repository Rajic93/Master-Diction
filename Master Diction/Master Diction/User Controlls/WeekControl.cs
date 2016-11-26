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
using AxWMPLib;
using Master_Diction.Classes;

namespace Master_Diction.User_Controlls
{
    public partial class WeekControl : UserControl
    {
        private Week _week;
        public WeekControl(Week week)
        {
            _week = week;
            InitializeComponent();
        }

        private void WeekControl_Load(object sender, EventArgs e)
        {
            RenderContent();
        }

        private void RenderContent()
        {
            FlowLayoutPanel lessonFlowLayoutPanel = new FlowLayoutPanel();

            foreach (Lesson lesson in _week.lessons)
            {
                Label label = new Label();
                label.Text = lesson.LessonNum.ToString();
                lessonFlowLayoutPanel.Controls.Add(label);

                foreach (Video lessonVideo in lesson.videos)
                {
                    AxWindowsMediaPlayer player = new AxWindowsMediaPlayer();
                    player.CreateControl();
                    string url = Directory.GetCurrentDirectory();
                    url += @"\Diction Exercise.mp4";
                    player.URL =  url;
                    player.uiMode = "full";
                    player.Ctlcontrols.stop();
                    lessonFlowLayoutPanel.Controls.Add(player);
                }   
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
