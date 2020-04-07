namespace Tangra.StarCatalogues.GaiaOnline
{
    partial class frmGaiaRestCaller
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGaiaRestCaller));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblJobInfo = new System.Windows.Forms.Label();
            this.lblStatusMessage = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblJobInfo
            // 
            this.lblJobInfo.AutoSize = true;
            this.lblJobInfo.ForeColor = System.Drawing.Color.PeachPuff;
            this.lblJobInfo.Location = new System.Drawing.Point(24, 22);
            this.lblJobInfo.Name = "lblJobInfo";
            this.lblJobInfo.Size = new System.Drawing.Size(35, 13);
            this.lblJobInfo.TabIndex = 0;
            this.lblJobInfo.Text = "label1";
            // 
            // lblStatusMessage
            // 
            this.lblStatusMessage.AutoSize = true;
            this.lblStatusMessage.ForeColor = System.Drawing.Color.PeachPuff;
            this.lblStatusMessage.Location = new System.Drawing.Point(24, 46);
            this.lblStatusMessage.Name = "lblStatusMessage";
            this.lblStatusMessage.Size = new System.Drawing.Size(35, 13);
            this.lblStatusMessage.TabIndex = 1;
            this.lblStatusMessage.Text = "label1";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(27, 107);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(328, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 2;
            // 
            // frmGaiaRestCaller
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSlateGray;
            this.ClientSize = new System.Drawing.Size(388, 163);
            this.ControlBox = false;
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblStatusMessage);
            this.Controls.Add(this.lblJobInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmGaiaRestCaller";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GaiaDR2 TAP REST Caller (https://gaia.aip.de/tap)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGaiaRestCaller_FormClosing);
            this.Load += new System.EventHandler(this.frmGaiaRestCaller_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblJobInfo;
        private System.Windows.Forms.Label lblStatusMessage;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}