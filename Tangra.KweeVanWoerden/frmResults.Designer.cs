namespace Tangra.KweeVanWoerden
{
	partial class frmResults
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
            this.btnSaveFiles = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxT0JD = new System.Windows.Forms.TextBox();
            this.tbxT0Uncertainty = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxT0 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lblCalcStatus = new System.Windows.Forms.Label();
            this.tbxErrorMessage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxTotalObs = new System.Windows.Forms.TextBox();
            this.tbxIncludedObs = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tbxT0HJD = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbxUncertaintyInSec = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.picGraph = new System.Windows.Forms.PictureBox();
            this.btnCalcHJD = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSaveFiles
            // 
            this.btnSaveFiles.Location = new System.Drawing.Point(320, 241);
            this.btnSaveFiles.Name = "btnSaveFiles";
            this.btnSaveFiles.Size = new System.Drawing.Size(143, 23);
            this.btnSaveFiles.TabIndex = 0;
            this.btnSaveFiles.Text = "Save Calculation Details";
            this.btnSaveFiles.UseVisualStyleBackColor = true;
            this.btnSaveFiles.Click += new System.EventHandler(this.btnSaveFiles_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(325, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Time of Minimum (JD)";
            // 
            // tbxT0JD
            // 
            this.tbxT0JD.BackColor = System.Drawing.SystemColors.Info;
            this.tbxT0JD.Location = new System.Drawing.Point(439, 68);
            this.tbxT0JD.Name = "tbxT0JD";
            this.tbxT0JD.ReadOnly = true;
            this.tbxT0JD.Size = new System.Drawing.Size(161, 20);
            this.tbxT0JD.TabIndex = 2;
            // 
            // tbxT0Uncertainty
            // 
            this.tbxT0Uncertainty.BackColor = System.Drawing.Color.Honeydew;
            this.tbxT0Uncertainty.Location = new System.Drawing.Point(138, 93);
            this.tbxT0Uncertainty.Name = "tbxT0Uncertainty";
            this.tbxT0Uncertainty.ReadOnly = true;
            this.tbxT0Uncertainty.Size = new System.Drawing.Size(161, 20);
            this.tbxT0Uncertainty.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Uncertainty (JD)";
            // 
            // tbxT0
            // 
            this.tbxT0.BackColor = System.Drawing.SystemColors.Info;
            this.tbxT0.Location = new System.Drawing.Point(439, 134);
            this.tbxT0.Name = "tbxT0";
            this.tbxT0.ReadOnly = true;
            this.tbxT0.Size = new System.Drawing.Size(161, 20);
            this.tbxT0.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(410, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "T0";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(469, 241);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(131, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // lblCalcStatus
            // 
            this.lblCalcStatus.AutoSize = true;
            this.lblCalcStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCalcStatus.ForeColor = System.Drawing.Color.Green;
            this.lblCalcStatus.Location = new System.Drawing.Point(17, 11);
            this.lblCalcStatus.Name = "lblCalcStatus";
            this.lblCalcStatus.Size = new System.Drawing.Size(185, 13);
            this.lblCalcStatus.TabIndex = 8;
            this.lblCalcStatus.Text = "The calculation was successful";
            // 
            // tbxErrorMessage
            // 
            this.tbxErrorMessage.BackColor = System.Drawing.SystemColors.Info;
            this.tbxErrorMessage.ForeColor = System.Drawing.Color.Red;
            this.tbxErrorMessage.Location = new System.Drawing.Point(20, 27);
            this.tbxErrorMessage.Multiline = true;
            this.tbxErrorMessage.Name = "tbxErrorMessage";
            this.tbxErrorMessage.ReadOnly = true;
            this.tbxErrorMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxErrorMessage.Size = new System.Drawing.Size(580, 35);
            this.tbxErrorMessage.TabIndex = 9;
            this.tbxErrorMessage.Text = "The error message\r\ngoes here";
            this.tbxErrorMessage.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(334, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Total Data Point";
            // 
            // tbxTotalObs
            // 
            this.tbxTotalObs.BackColor = System.Drawing.SystemColors.Info;
            this.tbxTotalObs.Location = new System.Drawing.Point(439, 160);
            this.tbxTotalObs.Name = "tbxTotalObs";
            this.tbxTotalObs.ReadOnly = true;
            this.tbxTotalObs.Size = new System.Drawing.Size(161, 20);
            this.tbxTotalObs.TabIndex = 11;
            // 
            // tbxIncludedObs
            // 
            this.tbxIncludedObs.BackColor = System.Drawing.SystemColors.Info;
            this.tbxIncludedObs.Location = new System.Drawing.Point(439, 186);
            this.tbxIncludedObs.Name = "tbxIncludedObs";
            this.tbxIncludedObs.ReadOnly = true;
            this.tbxIncludedObs.Size = new System.Drawing.Size(161, 20);
            this.tbxIncludedObs.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(317, 189);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Included Observations";
            // 
            // tbxT0HJD
            // 
            this.tbxT0HJD.BackColor = System.Drawing.Color.Honeydew;
            this.tbxT0HJD.Location = new System.Drawing.Point(138, 68);
            this.tbxT0HJD.Name = "tbxT0HJD";
            this.tbxT0HJD.ReadOnly = true;
            this.tbxT0HJD.Size = new System.Drawing.Size(161, 20);
            this.tbxT0HJD.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Time of Minimum (HJD)";
            // 
            // tbxUncertaintyInSec
            // 
            this.tbxUncertaintyInSec.BackColor = System.Drawing.SystemColors.Info;
            this.tbxUncertaintyInSec.Location = new System.Drawing.Point(439, 93);
            this.tbxUncertaintyInSec.Name = "tbxUncertaintyInSec";
            this.tbxUncertaintyInSec.ReadOnly = true;
            this.tbxUncertaintyInSec.Size = new System.Drawing.Size(161, 20);
            this.tbxUncertaintyInSec.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(350, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Uncertainty (sec)";
            // 
            // picGraph
            // 
            this.picGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picGraph.Location = new System.Drawing.Point(20, 130);
            this.picGraph.Name = "picGraph";
            this.picGraph.Size = new System.Drawing.Size(279, 134);
            this.picGraph.TabIndex = 18;
            this.picGraph.TabStop = false;
            // 
            // btnCalcHJD
            // 
            this.btnCalcHJD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalcHJD.ForeColor = System.Drawing.Color.Red;
            this.btnCalcHJD.Location = new System.Drawing.Point(138, 66);
            this.btnCalcHJD.Name = "btnCalcHJD";
            this.btnCalcHJD.Size = new System.Drawing.Size(161, 23);
            this.btnCalcHJD.TabIndex = 19;
            this.btnCalcHJD.Text = "Calculate HJD";
            this.btnCalcHJD.UseVisualStyleBackColor = true;
            this.btnCalcHJD.Click += new System.EventHandler(this.btnCalcHJD_Click);
            // 
            // frmResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 276);
            this.Controls.Add(this.btnCalcHJD);
            this.Controls.Add(this.picGraph);
            this.Controls.Add(this.tbxUncertaintyInSec);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbxT0HJD);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbxIncludedObs);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbxTotalObs);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbxErrorMessage);
            this.Controls.Add(this.lblCalcStatus);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbxT0);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbxT0Uncertainty);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbxT0JD);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSaveFiles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmResults";
            this.Text = "Occulting Binary Minimum";
            this.Load += new System.EventHandler(this.frmResults_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picGraph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnSaveFiles;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbxT0JD;
		private System.Windows.Forms.TextBox tbxT0Uncertainty;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbxT0;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label lblCalcStatus;
		private System.Windows.Forms.TextBox tbxErrorMessage;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbxTotalObs;
		private System.Windows.Forms.TextBox tbxIncludedObs;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox tbxT0HJD;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbxUncertaintyInSec;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox picGraph;
        private System.Windows.Forms.Button btnCalcHJD;
	}
}