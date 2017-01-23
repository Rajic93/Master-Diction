namespace Master_Diction
{
    partial class Welcome
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Welcome));
            this.panelLevelSelection = new System.Windows.Forms.Panel();
            this.flowLayoutPanelGrades = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelLevelSelection
            // 
            this.panelLevelSelection.Location = new System.Drawing.Point(12, 12);
            this.panelLevelSelection.Name = "panelLevelSelection";
            this.panelLevelSelection.Padding = new System.Windows.Forms.Padding(100, 0, 0, 0);
            this.panelLevelSelection.Size = new System.Drawing.Size(838, 224);
            this.panelLevelSelection.TabIndex = 0;
            // 
            // flowLayoutPanelGrades
            // 
            this.flowLayoutPanelGrades.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanelGrades.Location = new System.Drawing.Point(12, 245);
            this.flowLayoutPanelGrades.Name = "flowLayoutPanelGrades";
            this.flowLayoutPanelGrades.Padding = new System.Windows.Forms.Padding(100, 0, 0, 0);
            this.flowLayoutPanelGrades.Size = new System.Drawing.Size(838, 470);
            this.flowLayoutPanelGrades.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 245);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(838, 467);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Welcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(60)))), ((int)(((byte)(81)))));
            this.ClientSize = new System.Drawing.Size(862, 724);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panelLevelSelection);
            this.Controls.Add(this.flowLayoutPanelGrades);
            this.MaximumSize = new System.Drawing.Size(880, 771);
            this.MinimumSize = new System.Drawing.Size(880, 771);
            this.Name = "Welcome";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Welcome_FormClosing);
            this.Load += new System.EventHandler(this.Welcome_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLevelSelection;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelGrades;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}