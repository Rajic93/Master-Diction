﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Master_Diction.User_Controlls
{
    public partial class LevelSelection : UserControl
    {
        private FlowLayoutPanel _flowLayout;
        private Welcome _welcome;

        public LevelSelection(FlowLayoutPanel container, Welcome welcomeRef)
        {
            _welcome = welcomeRef;
            _flowLayout = container;
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            _flowLayout.Controls.Clear();
            _flowLayout.Controls.Add(new Secondary(_welcome));
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            _flowLayout.Controls.Clear();
            _flowLayout.Controls.Add(new Primary(_welcome));
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            _flowLayout.Controls.Clear();
            _flowLayout.Controls.Add(new Nursery(_welcome));
        }
    }
}
