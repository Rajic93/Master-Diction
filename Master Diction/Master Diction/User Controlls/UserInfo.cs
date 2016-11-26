using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Master_Diction.Classes;



namespace Master_Diction.User_Controlls
{
    public partial class UserInfo : UserControl
    {
        private Licence _licence;
        private Login _login;

        public UserInfo(Login loginRef)
        {
            _login = loginRef;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //check licence
            if (textBoxKey.Text.Equals(AppConfiguration.MasterKEy()))
            {
                _licence = Licence.MasterKey;
            }
            else
                _licence = Licence.Trial;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //save
            if (!textBoxKey.Text.Equals("") || !textBoxName.Text.Equals("")
                || !textBoxPassword.Text.Equals("") || !textBoxUsername.Text.Equals(""))
                if (textBoxPassword.Text.Equals(textBoxRepeatPassword.Text))
                {
                    AppConfiguration configuration = new AppConfiguration()
                    {
                        Licence = _licence,
                        Key = textBoxKey.Text,
                        FirstStart = false,
                        Username = textBoxUsername.Text,
                        Name = textBoxName.Text,
                        KeyExpireDate =
                            new DateTime(DateTime.Now.Year, DateTime.Now.Month == 12 ? 1 : DateTime.Now.Month + 1,
                                DateTime.Now.Day),
                        Password = textBoxPassword.Text
                    };
                    _login.SaveConfiguration(configuration);
                    _login.SwitchControls(this, new Credetials(configuration, _login));
                }
                else

                    MessageBox.Show("Passwords do not match.");
            else
                MessageBox.Show("All fields must be filled.");
        }
    }
}
