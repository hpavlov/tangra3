namespace Tangra.Video.SER
{
	partial class frmEnterSERFileInfo
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
            this.nudFrameRate = new System.Windows.Forms.NumericUpDown();
            this.cbxBitPix = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlFrameRate = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxTimeSource = new System.Windows.Forms.ComboBox();
            this.cbxColourHandling = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrameRate)).BeginInit();
            this.pnlFrameRate.SuspendLayout();
            this.SuspendLayout();
            // 
            // nudFrameRate
            // 
            this.nudFrameRate.DecimalPlaces = 2;
            this.nudFrameRate.Location = new System.Drawing.Point(111, 1);
            this.nudFrameRate.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudFrameRate.Name = "nudFrameRate";
            this.nudFrameRate.Size = new System.Drawing.Size(69, 20);
            this.nudFrameRate.TabIndex = 0;
            this.nudFrameRate.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // cbxBitPix
            // 
            this.cbxBitPix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBitPix.FormattingEnabled = true;
            this.cbxBitPix.Items.AddRange(new object[] {
            "12",
            "14",
            "16"});
            this.cbxBitPix.Location = new System.Drawing.Point(120, 18);
            this.cbxBitPix.Name = "cbxBitPix";
            this.cbxBitPix.Size = new System.Drawing.Size(69, 21);
            this.cbxBitPix.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Camera Bit Depth:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Video Frame Rate:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(186, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "fps";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(195, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "bpp";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(64, 133);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(145, 133);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pnlFrameRate
            // 
            this.pnlFrameRate.Controls.Add(this.label2);
            this.pnlFrameRate.Controls.Add(this.nudFrameRate);
            this.pnlFrameRate.Controls.Add(this.label3);
            this.pnlFrameRate.Location = new System.Drawing.Point(8, 101);
            this.pnlFrameRate.Name = "pnlFrameRate";
            this.pnlFrameRate.Size = new System.Drawing.Size(231, 21);
            this.pnlFrameRate.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Time Source:";
            // 
            // cbxTimeSource
            // 
            this.cbxTimeSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimeSource.FormattingEnabled = true;
            this.cbxTimeSource.Items.AddRange(new object[] {
            "Manually Entered",
            "Embedded UTC Time"});
            this.cbxTimeSource.Location = new System.Drawing.Point(119, 45);
            this.cbxTimeSource.Name = "cbxTimeSource";
            this.cbxTimeSource.Size = new System.Drawing.Size(143, 21);
            this.cbxTimeSource.TabIndex = 10;
            // 
            // cbxColourHandling
            // 
            this.cbxColourHandling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxColourHandling.FormattingEnabled = true;
            this.cbxColourHandling.Items.AddRange(new object[] {
            "Monochrome",
            "Grayscale"});
            this.cbxColourHandling.Location = new System.Drawing.Point(119, 73);
            this.cbxColourHandling.Name = "cbxColourHandling";
            this.cbxColourHandling.Size = new System.Drawing.Size(143, 21);
            this.cbxColourHandling.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Colour Handling:";
            // 
            // frmEnterSERFileInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(291, 167);
            this.Controls.Add(this.cbxColourHandling);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbxTimeSource);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pnlFrameRate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbxBitPix);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmEnterSERFileInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter SER File Info";
            ((System.ComponentModel.ISupportInitialize)(this.nudFrameRate)).EndInit();
            this.pnlFrameRate.ResumeLayout(false);
            this.pnlFrameRate.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown nudFrameRate;
		private System.Windows.Forms.ComboBox cbxBitPix;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlFrameRate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cbxTimeSource;
        private System.Windows.Forms.ComboBox cbxColourHandling;
        private System.Windows.Forms.Label label6;
	}
}