using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Serialization;
using Master_Diction.Classes;
using Master_Diction.User_Controlls;

namespace Master_Diction
{
    public partial class Content : Form
    {
        private Welcome _welcome;
        private MaterialConfig _materialConfig;
        private Grades _grade;

        public Content(Welcome welcomeRef, Grades grade)
        {
            _grade = grade;
            _welcome = welcomeRef;
            InitializeComponent();
        }

        private void Content_FormClosing(object sender, FormClosingEventArgs e)
        {
            _welcome.Show();
        }

        private void Content_Load(object sender, EventArgs e)
        {
            panel2.Controls.Add(new Instructions()
            {
                Dock = DockStyle.Fill
            });
            LoadConfiguration();
            RenderContent();
        }

        private void RenderContent()
        {
            //List<Term> contentList;
            //switch (_grade)
            //{
            //    case Grades.NurseryLevel1:
            //        contentList = _materialConfig.NurseryLevel1;
            //        break;
            //    case Grades.NurseryLevel2:
            //        contentList = _materialConfig.NurseryLevel2;
            //        break;
            //    case Grades.PrimaryGrade1:
            //        contentList = _materialConfig.PrimaryGrade1;
            //        break;
            //    case Grades.PrimaryGrade2:
            //        contentList = _materialConfig.PrimaryGrade2;
            //        break;
            //    case Grades.PrimaryGrade3:
            //        contentList = _materialConfig.PrimaryGrade3;
            //        break;
            //    case Grades.PrimaryGrade4:
            //        contentList = _materialConfig.PrimaryGrade4;
            //        break;
            //    case Grades.PrimaryGrade5:
            //        contentList = _materialConfig.PrimaryGrade5;
            //        break;
            //    case Grades.PrimaryGrade6:
            //        contentList = _materialConfig.PrimaryGrade6;
            //        break;
            //    case Grades.SecondaryJunior1:
            //        contentList = _materialConfig.SecondaryJuniorGrade1;
            //        break;
            //    case Grades.SecondaryJunior2:
            //        contentList = _materialConfig.SecondaryJuniorGrade2;
            //        break;
            //    case Grades.SecondaryJunior3:
            //        contentList = _materialConfig.SecondaryJuniorGrade3;
            //        break;
            //    case Grades.SecondarySenior4:
            //        contentList = _materialConfig.SecondarySeniorGrade4;
            //        break;
            //    case Grades.SecondarySenior5:
            //        contentList = _materialConfig.SecondarySeniorGrade5;
            //        break;
            //    case Grades.SecondarySenior6:
            //        contentList = _materialConfig.SecondarySeniorGrade6;
            //        break;
            //    default:
            //        contentList = new List<Term>();
            //        break;
            //}

            for (int i = 0; i < 3; i++)
            {
                flowLayoutPanelNavigation.Controls.Add(new WeekControl(new Week() {WeekNum = i}, panel2)
                {
                    
                    Width = flowLayoutPanelNavigation.Width - 6
                });
            }
        }

        public void SaveConfiguration(MaterialConfig materialConfig)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(materialConfig.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, materialConfig);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save("AppConfiguration.xml");
                stream.Close();
            }
        }

        public void LoadConfiguration()
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(MaterialConfig));
                TextReader reader = new StreamReader("ContentManifest.xml");
                object obj = deserializer.Deserialize(reader);
                _materialConfig = (MaterialConfig)obj;
                reader.Close();
            }
            catch (Exception)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanelNavigation.Visible = !flowLayoutPanelNavigation.Visible)
            {
                button1.Text = "Hide";
                panelNavigation.Width += flowLayoutPanelNavigation.Width - button1.Width;
                panel2.Width -= flowLayoutPanelNavigation.Width - button1.Width;
                panel2.Location = new Point(panel2.Location.X + flowLayoutPanelNavigation.Width - button1.Width, panel2.Location.Y);
            }
            else
            {
                button1.Text = "Show";
                panelNavigation.Width -= flowLayoutPanelNavigation.Width - button1.Width;
                panel2.Width += flowLayoutPanelNavigation.Width - button1.Width;
                panel2.Location = new Point(panel2.Location.X - flowLayoutPanelNavigation.Width + button1.Width, panel2.Location.Y);
            }
        }
    }
}
