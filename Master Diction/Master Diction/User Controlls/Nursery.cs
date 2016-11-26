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
    public partial class Nursery : UserControl
    {
        private Welcome _welcome;

        public Nursery(Welcome welcomeRef)
        {
            _welcome = welcomeRef;
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            new Content(_welcome, Grades.NurseryLevel1).Show();
            _welcome.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            new Content(_welcome, Grades.NurseryLevel2).Show();
            _welcome.Hide();
        }
    }
}
