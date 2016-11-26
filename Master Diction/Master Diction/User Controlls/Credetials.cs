using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Master_Diction.Classes;

namespace Master_Diction.User_Controlls
{
    public partial class Credetials : UserControl
    {
        private AppConfiguration configuration;
        private Login _login;

        public Credetials(AppConfiguration config, Login loginRef)
        {
            _login = loginRef;
            configuration = config;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (configuration.Username.Equals(textBoxUsername.Text) &&
                configuration.Password.Equals(textBoxPassword.Text))
            {
                //new Thread(() => new Welcome(_login)).Start();
                new Welcome(_login).Show();
                _login.Hide();
            }
            else
                MessageBox.Show("Bad credentials.");
        }
    }
}
