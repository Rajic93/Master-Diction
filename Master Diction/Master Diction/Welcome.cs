using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Master_Diction.User_Controlls;

namespace Master_Diction
{
    public partial class Welcome : Form
    {
        private Login _parent;
        public Welcome(Login loginRef)
        {
            _parent = loginRef;
            InitializeComponent();
        }

        private void Welcome_Load(object sender, EventArgs e)
        {
           // _parent.Hide();
            panelLevelSelection.Controls.Add(new LevelSelection(flowLayoutPanelGrades, this) { Dock = DockStyle.Fill});
        }

        private void Welcome_FormClosing(object sender, FormClosingEventArgs e)
        {
            _parent.Show();
        }
    }
}
