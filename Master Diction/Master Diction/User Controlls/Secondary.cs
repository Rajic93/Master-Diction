using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Master_Diction.Classes;

namespace Master_Diction.User_Controlls
{
    public partial class Secondary : UserControl
    {
        private Welcome _welcome;

        public Secondary(Welcome welcomeRef)
        {
            _welcome = welcomeRef;
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            new Content(_welcome, Grades.SecondaryJunior1).Show();
            _welcome.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            new Content(_welcome, Grades.SecondaryJunior2).Show();
            _welcome.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            new Content(_welcome, Grades.SecondaryJunior3).Show();
            _welcome.Hide();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            new Content(_welcome, Grades.SecondarySenior4).Show();
            _welcome.Hide();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            new Content(_welcome, Grades.SecondarySenior5).Show();
            _welcome.Hide();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            new Content(_welcome, Grades.SecondarySenior6).Show();
            _welcome.Hide();
        }
    }
}
