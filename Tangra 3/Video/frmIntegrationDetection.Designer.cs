namespace Tangra.Video
{
	partial class frmIntegrationDetection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmIntegrationDetection));
            this.picFrameSpectrum = new System.Windows.Forms.PictureBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.picSigmas = new System.Windows.Forms.PictureBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.pnlResult = new System.Windows.Forms.Panel();
            this.btnReject = new System.Windows.Forms.Button();
            this.lblCertainty = new System.Windows.Forms.Label();
            this.lblStartingAt = new System.Windows.Forms.Label();
            this.lblIntegrationFrames = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAccept = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picFrameSpectrum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSigmas)).BeginInit();
            this.pnlResult.SuspendLayout();
            this.SuspendLayout();
            // 
            // picFrameSpectrum
            // 
            this.picFrameSpectrum.Location = new System.Drawing.Point(12, 108);
            this.picFrameSpectrum.Name = "picFrameSpectrum";
            this.picFrameSpectrum.Size = new System.Drawing.Size(477, 66);
            this.picFrameSpectrum.TabIndex = 2;
            this.picFrameSpectrum.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 78);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(477, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 3;
            // 
            // picSigmas
            // 
            this.picSigmas.Location = new System.Drawing.Point(11, 12);
            this.picSigmas.Name = "picSigmas";
            this.picSigmas.Size = new System.Drawing.Size(478, 56);
            this.picSigmas.TabIndex = 4;
            this.picSigmas.TabStop = false;
            this.picSigmas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picSigmas_MouseDown);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // pnlResult
            // 
            this.pnlResult.Controls.Add(this.btnReject);
            this.pnlResult.Controls.Add(this.lblCertainty);
            this.pnlResult.Controls.Add(this.lblStartingAt);
            this.pnlResult.Controls.Add(this.lblIntegrationFrames);
            this.pnlResult.Controls.Add(this.label3);
            this.pnlResult.Controls.Add(this.label2);
            this.pnlResult.Controls.Add(this.btnAccept);
            this.pnlResult.Controls.Add(this.label1);
            this.pnlResult.Location = new System.Drawing.Point(1, 108);
            this.pnlResult.Name = "pnlResult";
            this.pnlResult.Size = new System.Drawing.Size(488, 78);
            this.pnlResult.TabIndex = 5;
            // 
            // btnReject
            // 
            this.btnReject.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnReject.Location = new System.Drawing.Point(413, 43);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(75, 23);
            this.btnReject.TabIndex = 7;
            this.btnReject.Text = "Reject";
            this.btnReject.UseVisualStyleBackColor = true;
            // 
            // lblCertainty
            // 
            this.lblCertainty.AutoSize = true;
            this.lblCertainty.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCertainty.Location = new System.Drawing.Point(117, 53);
            this.lblCertainty.Name = "lblCertainty";
            this.lblCertainty.Size = new System.Drawing.Size(37, 13);
            this.lblCertainty.TabIndex = 6;
            this.lblCertainty.Text = "Good";
            // 
            // lblStartingAt
            // 
            this.lblStartingAt.AutoSize = true;
            this.lblStartingAt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblStartingAt.Location = new System.Drawing.Point(116, 31);
            this.lblStartingAt.Name = "lblStartingAt";
            this.lblStartingAt.Size = new System.Drawing.Size(21, 13);
            this.lblStartingAt.TabIndex = 5;
            this.lblStartingAt.Text = "12";
            // 
            // lblIntegrationFrames
            // 
            this.lblIntegrationFrames.AutoSize = true;
            this.lblIntegrationFrames.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblIntegrationFrames.Location = new System.Drawing.Point(117, 10);
            this.lblIntegrationFrames.Name = "lblIntegrationFrames";
            this.lblIntegrationFrames.Size = new System.Drawing.Size(55, 13);
            this.lblIntegrationFrames.TabIndex = 4;
            this.lblIntegrationFrames.Text = "4 frames";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(68, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Certainty:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Starting At Frame:";
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(332, 43);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 1;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Detected Integration:";
            // 
            // frmIntegrationDetection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 185);
            this.Controls.Add(this.picSigmas);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.pnlResult);
            this.Controls.Add(this.picFrameSpectrum);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmIntegrationDetection";
            this.Text = "Detecting Video Integration ...";
            this.Load += new System.EventHandler(this.frmIntegrationDetection_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picFrameSpectrum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSigmas)).EndInit();
            this.pnlResult.ResumeLayout(false);
            this.pnlResult.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox picFrameSpectrum;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.PictureBox picSigmas;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.Panel pnlResult;
		private System.Windows.Forms.Label lblCertainty;
		private System.Windows.Forms.Label lblStartingAt;
		private System.Windows.Forms.Label lblIntegrationFrames;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnAccept;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReject;
	}
}