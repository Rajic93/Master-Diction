using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Printing;
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
        private Panel _panel;
        private bool _startup = true;

        public WeekControl(Week week, Panel panelRef)
        {
            _week = week;
            _panel = panelRef;
            InitializeComponent();
        }

        private void WeekControl_Load(object sender, EventArgs e)
        {
            //RenderContent();
            label1.Text += " " + _week.WeekNum;
        }

        private void RenderContent()
        {
            FlowLayoutPanel lessonFlowLayoutPanel = new FlowLayoutPanel();

            //foreach (Lesson lesson in _week.lessons)
            //{
            //    Label label = new Label();
            //    label.Text = lesson.LessonNum.ToString();
            //    lessonFlowLayoutPanel.Controls.Add(label);

            //    foreach (Video lessonVideo in lesson.videos)
            //    {
            //        AxWindowsMediaPlayer player = new AxWindowsMediaPlayer();
            //        player.CreateControl();
            //        string url = Directory.GetCurrentDirectory();
            //        url += @"\Diction Exercise.mp4";
            //        player.URL =  url;
            //        player.uiMode = "full";
            //        player.Ctlcontrols.stop();
            //        lessonFlowLayoutPanel.Controls.Add(player);
            //    }   
            //}
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (_startup)
            {
                for (int i = 0; i < 3; i++)
                {
                    Label lesson = new Label()
                    {
                        Text = "Lesson " + i
                    };
                    lesson.Margin = new Padding(0);
                    lesson.ForeColor = Color.FromArgb(101, 108, 116);
                    lesson.Click += LessonOnClick;
                    flowLayoutPanel1.Controls.Add(lesson);
                }
                _startup = false;
            }
            flowLayoutPanel1.Visible = !flowLayoutPanel1.Visible;
        }

        private void LessonOnClick(object sender, EventArgs eventArgs)
        {
            if (_panel.Controls[0].GetType() == typeof(LessonControl))
            {
                ((LessonControl)_panel.Controls[0]).StopMedia();
                //function that changes the video
            }
            _panel.Controls.Clear();
            _panel.Visible = true;
            _panel.Controls.Add(new LessonControl()
            {
                Dock = DockStyle.Fill,
                Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right)
            });
        }
    }
}
