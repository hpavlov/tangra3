namespace Tangra.VideoOperations.LightCurves.InfoForms
{
    partial class frmPixelDistribution
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPixelDistribution));
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblBackgroundSigma = new System.Windows.Forms.Label();
            this.lblMedianBackground = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rbScaleLinear = new System.Windows.Forms.RadioButton();
            this.rbScaleLog = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.lblPixCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelClient = new System.Windows.Forms.Panel();
            this.picHistogram = new System.Windows.Forms.PictureBox();
            this.panel2.SuspendLayout();
            this.panelClient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHistogram)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblBackgroundSigma);
            this.panel2.Controls.Add(this.lblMedianBackground);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.rbScaleLinear);
            this.panel2.Controls.Add(this.rbScaleLog);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.lblPixCount);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 385);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(564, 45);
            this.panel2.TabIndex = 1;
            // 
            // lblBackgroundSigma
            // 
            this.lblBackgroundSigma.AutoSize = true;
            this.lblBackgroundSigma.Location = new System.Drawing.Point(421, 25);
            this.lblBackgroundSigma.Name = "lblBackgroundSigma";
            this.lblBackgroundSigma.Size = new System.Drawing.Size(25, 13);
            this.lblBackgroundSigma.TabIndex = 36;
            this.lblBackgroundSigma.Text = "123";
            // 
            // lblMedianBackground
            // 
            this.lblMedianBackground.AutoSize = true;
            this.lblMedianBackground.Location = new System.Drawing.Point(320, 25);
            this.lblMedianBackground.Name = "lblMedianBackground";
            this.lblMedianBackground.Size = new System.Drawing.Size(25, 13);
            this.lblMedianBackground.TabIndex = 35;
            this.lblMedianBackground.Text = "123";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(373, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "1 Sigma:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(215, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 33;
            this.label3.Text = "Median Background:";
            // 
            // rbScaleLinear
            // 
            this.rbScaleLinear.AutoSize = true;
            this.rbScaleLinear.Location = new System.Drawing.Point(130, 15);
            this.rbScaleLinear.Name = "rbScaleLinear";
            this.rbScaleLinear.Size = new System.Drawing.Size(54, 17);
            this.rbScaleLinear.TabIndex = 5;
            this.rbScaleLinear.Text = "Linear";
            this.rbScaleLinear.UseVisualStyleBackColor = true;
            this.rbScaleLinear.CheckedChanged += new System.EventHandler(this.rbScaleLinear_CheckedChanged);
            // 
            // rbScaleLog
            // 
            this.rbScaleLog.AutoSize = true;
            this.rbScaleLog.Checked = true;
            this.rbScaleLog.Location = new System.Drawing.Point(51, 15);
            this.rbScaleLog.Name = "rbScaleLog";
            this.rbScaleLog.Size = new System.Drawing.Size(79, 17);
            this.rbScaleLog.TabIndex = 4;
            this.rbScaleLog.TabStop = true;
            this.rbScaleLog.Text = "Logarithmic";
            this.rbScaleLog.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Scale:";
            // 
            // lblPixCount
            // 
            this.lblPixCount.AutoSize = true;
            this.lblPixCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblPixCount.Location = new System.Drawing.Point(321, 10);
            this.lblPixCount.Name = "lblPixCount";
            this.lblPixCount.Size = new System.Drawing.Size(0, 13);
            this.lblPixCount.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(207, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Number of pixels used:";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(477, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // panelClient
            // 
            this.panelClient.Controls.Add(this.picHistogram);
            this.panelClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelClient.Location = new System.Drawing.Point(0, 0);
            this.panelClient.Name = "panelClient";
            this.panelClient.Size = new System.Drawing.Size(564, 385);
            this.panelClient.TabIndex = 2;
            // 
            // picHistogram
            // 
            this.picHistogram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picHistogram.Location = new System.Drawing.Point(0, 0);
            this.picHistogram.Name = "picHistogram";
            this.picHistogram.Size = new System.Drawing.Size(564, 385);
            this.picHistogram.TabIndex = 0;
            this.picHistogram.TabStop = false;
            this.picHistogram.Resize += new System.EventHandler(this.picHistogram_Resize);
            // 
            // frmPixelDistribution
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 430);
            this.Controls.Add(this.panelClient);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPixelDistribution";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Measurement Areas Combined Histogram";
            this.Load += new System.EventHandler(this.frmPixelDistribution_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelClient.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picHistogram)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panelClient;
        private System.Windows.Forms.PictureBox picHistogram;
        private System.Windows.Forms.Label lblPixCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbScaleLinear;
        private System.Windows.Forms.RadioButton rbScaleLog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblBackgroundSigma;
        private System.Windows.Forms.Label lblMedianBackground;
        private System.Windows.Forms.Label label4;
    }
}