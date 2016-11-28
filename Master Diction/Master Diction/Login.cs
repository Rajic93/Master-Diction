using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Master_Diction.Classes;
using Master_Diction.User_Controlls;

namespace Master_Diction
{
    public partial class Login : Form
    {
        private AppConfiguration configuration;
        private string _username;
        private string _password;


        public Login()
        {
            InitializeComponent();
        }

        public void SaveConfiguration(AppConfiguration configuration)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(configuration.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, configuration);
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
                string path;
#if DEBUG
                path = Directory.GetCurrentDirectory() + "\\AppConfiguration.xml";
#else
                path = Directory.GetCurrentDirectory() + "\\Resources\\AppConfiguration.xml";
#endif
                if (!File.Exists(path))
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    const string NAME = "Master_Diction.Resources.AppConfiguration.xml";
                    using (Stream stream = assembly.GetManifestResourceStream(NAME))
                    {
                        XmlDocument xmlDocument = new XmlDataDocument();
                        xmlDocument.Load(stream);
                        
                        xmlDocument.Save(path);
                    }
                }
                XmlSerializer deserializer = new XmlSerializer(typeof(AppConfiguration));
                TextReader reader = new StreamReader(path);
                object obj = deserializer.Deserialize(reader);
                configuration = (AppConfiguration)obj;
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            LoadConfiguration();
            if (configuration != null)
            {
                if (configuration.FirstStart)
                {
                    panel1.Controls.Add(new UserInfo(this));
                }
                else
                {
                    panel1.Controls.Add(new Credetials(configuration, this));
                }
            }
        }

        public void SwitchControls(UserControl currentControl, UserControl nextControl)
        {
            panel1.Controls.Remove(currentControl);
            panel1.Controls.Add(nextControl);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            MaterialConfig config = ResourcesManager.ConfigureResources();
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(AppConfiguration));
                TextReader reader = new StreamReader("Content.xml");
                object obj = deserializer.Deserialize(reader);
                AppConfiguration XmlData = (AppConfiguration)obj;
                reader.Close();
            }
            catch (Exception)
            {

            }
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(config.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, config);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save("Content.xml");
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }
        }
    }
}
