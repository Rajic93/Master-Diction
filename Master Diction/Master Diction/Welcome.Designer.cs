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
            this.panelLevelSelection = new System.Windows.Forms.Panel();
            this.flowLayoutPanelGrades = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // panelLevelSelection
            // 
            this.panelLevelSelection.Location = new System.Drawing.Point(12, 12);
            this.panelLevelSelection.Name = "panelLevelSelection";
            this.panelLevelSelection.Size = new System.Drawing.Size(1154, 224);
            this.panelLevelSelection.TabIndex = 0;
            // 
            // flowLayoutPanelGrades
            // 
            this.flowLayoutPanelGrades.Location = new System.Drawing.Point(12, 242);
            this.flowLayoutPanelGrades.Name = "flowLayoutPanelGrades";
            this.flowLayoutPanelGrades.Size = new System.Drawing.Size(1154, 470);
            this.flowLayoutPanelGrades.TabIndex = 1;
            // 
            // Welcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1178, 724);
            this.Controls.Add(this.flowLayoutPanelGrades);
            this.Controls.Add(this.panelLevelSelection);
            this.Name = "Welcome";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Welcome_FormClosing);
            this.Load += new System.EventHandler(this.Welcome_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLevelSelection;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelGrades;
    }
}