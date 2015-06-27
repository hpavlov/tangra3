namespace Tangra.VideoOperations.Spectroscopy
{
	partial class frmEditWavelengthConfigName
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditWavelengthConfigName));
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.tbxConfigName = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.pnlSolvedWavelengthConf = new System.Windows.Forms.Panel();
			this.cbxEditConfig = new System.Windows.Forms.CheckBox();
			this.tbxSolvedA = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbxSolvedB = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tbxSolvedD = new System.Windows.Forms.TextBox();
			this.tbxSolvedC = new System.Windows.Forms.TextBox();
			this.nudConfigOrder = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.pnlSolvedWavelengthConf.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudConfigOrder)).BeginInit();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(241, 251);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 36;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Location = new System.Drawing.Point(160, 251);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 35;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// tbxConfigName
			// 
			this.tbxConfigName.Location = new System.Drawing.Point(16, 126);
			this.tbxConfigName.Name = "tbxConfigName";
			this.tbxConfigName.Size = new System.Drawing.Size(434, 20);
			this.tbxConfigName.TabIndex = 34;
			this.tbxConfigName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxConfigName_KeyPress);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(13, 110);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(103, 13);
			this.label11.TabIndex = 33;
			this.label11.Text = "Configuration Name:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(449, 82);
			this.label1.TabIndex = 37;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// pnlSolvedWavelengthConf
			// 
			this.pnlSolvedWavelengthConf.Controls.Add(this.label5);
			this.pnlSolvedWavelengthConf.Controls.Add(this.nudConfigOrder);
			this.pnlSolvedWavelengthConf.Controls.Add(this.label3);
			this.pnlSolvedWavelengthConf.Controls.Add(this.label4);
			this.pnlSolvedWavelengthConf.Controls.Add(this.tbxSolvedD);
			this.pnlSolvedWavelengthConf.Controls.Add(this.tbxSolvedC);
			this.pnlSolvedWavelengthConf.Controls.Add(this.label2);
			this.pnlSolvedWavelengthConf.Controls.Add(this.label12);
			this.pnlSolvedWavelengthConf.Controls.Add(this.tbxSolvedB);
			this.pnlSolvedWavelengthConf.Controls.Add(this.tbxSolvedA);
			this.pnlSolvedWavelengthConf.Enabled = false;
			this.pnlSolvedWavelengthConf.Location = new System.Drawing.Point(12, 179);
			this.pnlSolvedWavelengthConf.Name = "pnlSolvedWavelengthConf";
			this.pnlSolvedWavelengthConf.Size = new System.Drawing.Size(438, 54);
			this.pnlSolvedWavelengthConf.TabIndex = 77;
			// 
			// cbxEditConfig
			// 
			this.cbxEditConfig.AutoSize = true;
			this.cbxEditConfig.Location = new System.Drawing.Point(16, 155);
			this.cbxEditConfig.Name = "cbxEditConfig";
			this.cbxEditConfig.Size = new System.Drawing.Size(159, 17);
			this.cbxEditConfig.TabIndex = 78;
			this.cbxEditConfig.Text = "Edit Configuration Constants";
			this.cbxEditConfig.UseVisualStyleBackColor = true;
			this.cbxEditConfig.CheckedChanged += new System.EventHandler(this.cbxEditConfig_CheckedChanged);
			// 
			// tbxSolvedA
			// 
			this.tbxSolvedA.Location = new System.Drawing.Point(172, 5);
			this.tbxSolvedA.Name = "tbxSolvedA";
			this.tbxSolvedA.Size = new System.Drawing.Size(98, 20);
			this.tbxSolvedA.TabIndex = 66;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(152, 8);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(14, 13);
			this.label12.TabIndex = 67;
			this.label12.Text = "A";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(152, 34);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(14, 13);
			this.label2.TabIndex = 80;
			this.label2.Text = "B";
			// 
			// tbxSolvedB
			// 
			this.tbxSolvedB.Location = new System.Drawing.Point(172, 31);
			this.tbxSolvedB.Name = "tbxSolvedB";
			this.tbxSolvedB.Size = new System.Drawing.Size(98, 20);
			this.tbxSolvedB.TabIndex = 79;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(317, 34);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(15, 13);
			this.label3.TabIndex = 84;
			this.label3.Text = "D";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(317, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(14, 13);
			this.label4.TabIndex = 82;
			this.label4.Text = "C";
			// 
			// tbxSolvedD
			// 
			this.tbxSolvedD.Location = new System.Drawing.Point(337, 31);
			this.tbxSolvedD.Name = "tbxSolvedD";
			this.tbxSolvedD.Size = new System.Drawing.Size(98, 20);
			this.tbxSolvedD.TabIndex = 83;
			// 
			// tbxSolvedC
			// 
			this.tbxSolvedC.Location = new System.Drawing.Point(337, 5);
			this.tbxSolvedC.Name = "tbxSolvedC";
			this.tbxSolvedC.Size = new System.Drawing.Size(98, 20);
			this.tbxSolvedC.TabIndex = 81;
			// 
			// nudConfigOrder
			// 
			this.nudConfigOrder.Location = new System.Drawing.Point(16, 26);
			this.nudConfigOrder.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudConfigOrder.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudConfigOrder.Name = "nudConfigOrder";
			this.nudConfigOrder.Size = new System.Drawing.Size(42, 20);
			this.nudConfigOrder.TabIndex = 85;
			this.nudConfigOrder.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(13, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(86, 13);
			this.label5.TabIndex = 86;
			this.label5.Text = "Polynomial Order";
			// 
			// frmEditConfigName
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(473, 286);
			this.Controls.Add(this.cbxEditConfig);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.tbxConfigName);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.pnlSolvedWavelengthConf);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmEditConfigName";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Scope + Focal Reducer + Grating + Grating Position + Recorder + Grabber";
			this.pnlSolvedWavelengthConf.ResumeLayout(false);
			this.pnlSolvedWavelengthConf.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudConfigOrder)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox tbxConfigName;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Panel pnlSolvedWavelengthConf;
		private System.Windows.Forms.CheckBox cbxEditConfig;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown nudConfigOrder;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbxSolvedD;
		private System.Windows.Forms.TextBox tbxSolvedC;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox tbxSolvedB;
		private System.Windows.Forms.TextBox tbxSolvedA;
	}
}