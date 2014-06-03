namespace AdvLibTestApp
{
	partial class frmMain
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
			this.button1 = new System.Windows.Forms.Button();
			this.cbxLocationData = new System.Windows.Forms.CheckBox();
			this.rb16BitUShort = new System.Windows.Forms.RadioButton();
			this.rb16BitByte = new System.Windows.Forms.RadioButton();
			this.rb8BitByte = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.rbPixel8 = new System.Windows.Forms.RadioButton();
			this.rbPixel12 = new System.Windows.Forms.RadioButton();
			this.rbPixel16 = new System.Windows.Forms.RadioButton();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.rbCamera8 = new System.Windows.Forms.RadioButton();
			this.rbCamera12 = new System.Windows.Forms.RadioButton();
			this.rbCamera16 = new System.Windows.Forms.RadioButton();
			this.cbxCompress = new System.Windows.Forms.CheckBox();
			this.btnVerifyLibrary = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(33, 189);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(132, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Generate Test File";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// cbxLocationData
			// 
			this.cbxLocationData.AutoSize = true;
			this.cbxLocationData.Location = new System.Drawing.Point(33, 26);
			this.cbxLocationData.Name = "cbxLocationData";
			this.cbxLocationData.Size = new System.Drawing.Size(115, 17);
			this.cbxLocationData.TabIndex = 1;
			this.cbxLocationData.Text = "Save location data";
			this.cbxLocationData.UseVisualStyleBackColor = true;
			// 
			// rb16BitUShort
			// 
			this.rb16BitUShort.AutoSize = true;
			this.rb16BitUShort.Checked = true;
			this.rb16BitUShort.Location = new System.Drawing.Point(18, 24);
			this.rb16BitUShort.Name = "rb16BitUShort";
			this.rb16BitUShort.Size = new System.Drawing.Size(95, 17);
			this.rb16BitUShort.TabIndex = 2;
			this.rb16BitUShort.TabStop = true;
			this.rb16BitUShort.Text = "16-bit, ushort[] ";
			this.rb16BitUShort.UseVisualStyleBackColor = true;
			this.rb16BitUShort.CheckedChanged += new System.EventHandler(this.OnImageFormatChanged);
			// 
			// rb16BitByte
			// 
			this.rb16BitByte.AutoSize = true;
			this.rb16BitByte.Location = new System.Drawing.Point(18, 47);
			this.rb16BitByte.Name = "rb16BitByte";
			this.rb16BitByte.Size = new System.Drawing.Size(142, 17);
			this.rb16BitByte.TabIndex = 3;
			this.rb16BitByte.Text = "16-bit, little-endian, byte[]";
			this.rb16BitByte.UseVisualStyleBackColor = true;
			this.rb16BitByte.CheckedChanged += new System.EventHandler(this.OnImageFormatChanged);
			// 
			// rb8BitByte
			// 
			this.rb8BitByte.AutoSize = true;
			this.rb8BitByte.Location = new System.Drawing.Point(18, 70);
			this.rb8BitByte.Name = "rb8BitByte";
			this.rb8BitByte.Size = new System.Drawing.Size(77, 17);
			this.rb8BitByte.TabIndex = 4;
			this.rb8BitByte.Text = "8-bit, byte[]";
			this.rb8BitByte.UseVisualStyleBackColor = true;
			this.rb8BitByte.CheckedChanged += new System.EventHandler(this.OnImageFormatChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rb16BitUShort);
			this.groupBox1.Controls.Add(this.rb8BitByte);
			this.groupBox1.Controls.Add(this.rb16BitByte);
			this.groupBox1.Location = new System.Drawing.Point(33, 52);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(184, 100);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Image Format";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.rbPixel8);
			this.groupBox2.Controls.Add(this.rbPixel12);
			this.groupBox2.Controls.Add(this.rbPixel16);
			this.groupBox2.Location = new System.Drawing.Point(223, 52);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(161, 100);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Pixel Format";
			// 
			// rbPixel8
			// 
			this.rbPixel8.AutoSize = true;
			this.rbPixel8.Enabled = false;
			this.rbPixel8.Location = new System.Drawing.Point(15, 70);
			this.rbPixel8.Name = "rbPixel8";
			this.rbPixel8.Size = new System.Drawing.Size(122, 17);
			this.rbPixel8.TabIndex = 2;
			this.rbPixel8.Text = "8-bit (Saved as 8-bit)";
			this.rbPixel8.UseVisualStyleBackColor = true;
			this.rbPixel8.CheckedChanged += new System.EventHandler(this.OnPixelFormatChanged);
			// 
			// rbPixel12
			// 
			this.rbPixel12.AutoSize = true;
			this.rbPixel12.Location = new System.Drawing.Point(15, 47);
			this.rbPixel12.Name = "rbPixel12";
			this.rbPixel12.Size = new System.Drawing.Size(134, 17);
			this.rbPixel12.TabIndex = 1;
			this.rbPixel12.Text = "12-bit (Saved as 16-bit)";
			this.rbPixel12.UseVisualStyleBackColor = true;
			this.rbPixel12.CheckedChanged += new System.EventHandler(this.OnPixelFormatChanged);
			// 
			// rbPixel16
			// 
			this.rbPixel16.AutoSize = true;
			this.rbPixel16.Checked = true;
			this.rbPixel16.Location = new System.Drawing.Point(15, 24);
			this.rbPixel16.Name = "rbPixel16";
			this.rbPixel16.Size = new System.Drawing.Size(134, 17);
			this.rbPixel16.TabIndex = 0;
			this.rbPixel16.TabStop = true;
			this.rbPixel16.Text = "16-bit (Saved as 16-bit)";
			this.rbPixel16.UseVisualStyleBackColor = true;
			this.rbPixel16.CheckedChanged += new System.EventHandler(this.OnPixelFormatChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.rbCamera8);
			this.groupBox3.Controls.Add(this.rbCamera12);
			this.groupBox3.Controls.Add(this.rbCamera16);
			this.groupBox3.Location = new System.Drawing.Point(390, 52);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(122, 100);
			this.groupBox3.TabIndex = 7;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Camera Depth";
			// 
			// rbCamera8
			// 
			this.rbCamera8.AutoSize = true;
			this.rbCamera8.Enabled = false;
			this.rbCamera8.Location = new System.Drawing.Point(15, 70);
			this.rbCamera8.Name = "rbCamera8";
			this.rbCamera8.Size = new System.Drawing.Size(45, 17);
			this.rbCamera8.TabIndex = 2;
			this.rbCamera8.Text = "8-bit";
			this.rbCamera8.UseVisualStyleBackColor = true;
			// 
			// rbCamera12
			// 
			this.rbCamera12.AutoSize = true;
			this.rbCamera12.Location = new System.Drawing.Point(15, 47);
			this.rbCamera12.Name = "rbCamera12";
			this.rbCamera12.Size = new System.Drawing.Size(51, 17);
			this.rbCamera12.TabIndex = 1;
			this.rbCamera12.Text = "12-bit";
			this.rbCamera12.UseVisualStyleBackColor = true;
			// 
			// rbCamera16
			// 
			this.rbCamera16.AutoSize = true;
			this.rbCamera16.Checked = true;
			this.rbCamera16.Location = new System.Drawing.Point(15, 24);
			this.rbCamera16.Name = "rbCamera16";
			this.rbCamera16.Size = new System.Drawing.Size(51, 17);
			this.rbCamera16.TabIndex = 0;
			this.rbCamera16.TabStop = true;
			this.rbCamera16.Text = "16-bit";
			this.rbCamera16.UseVisualStyleBackColor = true;
			// 
			// cbxCompress
			// 
			this.cbxCompress.AutoSize = true;
			this.cbxCompress.Location = new System.Drawing.Point(33, 162);
			this.cbxCompress.Name = "cbxCompress";
			this.cbxCompress.Size = new System.Drawing.Size(140, 17);
			this.cbxCompress.TabIndex = 8;
			this.cbxCompress.Text = "Use Image Compression";
			this.cbxCompress.UseVisualStyleBackColor = true;
			// 
			// btnVerifyLibrary
			// 
			this.btnVerifyLibrary.Location = new System.Drawing.Point(380, 189);
			this.btnVerifyLibrary.Name = "btnVerifyLibrary";
			this.btnVerifyLibrary.Size = new System.Drawing.Size(132, 23);
			this.btnVerifyLibrary.TabIndex = 9;
			this.btnVerifyLibrary.Text = "Verify Library";
			this.btnVerifyLibrary.UseVisualStyleBackColor = true;
			this.btnVerifyLibrary.Click += new System.EventHandler(this.btnVerifyLibrary_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.DefaultExt = "dll";
			this.openFileDialog1.FileName = "Open Adv.Core dll";
			this.openFileDialog1.Filter = "Dynamic Link Libraries (*.dll)|*.dll";
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(541, 233);
			this.Controls.Add(this.btnVerifyLibrary);
			this.Controls.Add(this.cbxCompress);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.cbxLocationData);
			this.Controls.Add(this.button1);
			this.Name = "frmMain";
			this.Text = "AdvLib Test Form";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.CheckBox cbxLocationData;
		private System.Windows.Forms.RadioButton rb16BitUShort;
		private System.Windows.Forms.RadioButton rb16BitByte;
		private System.Windows.Forms.RadioButton rb8BitByte;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton rbPixel8;
		private System.Windows.Forms.RadioButton rbPixel12;
		private System.Windows.Forms.RadioButton rbPixel16;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton rbCamera8;
		private System.Windows.Forms.RadioButton rbCamera12;
		private System.Windows.Forms.RadioButton rbCamera16;
		private System.Windows.Forms.CheckBox cbxCompress;
		private System.Windows.Forms.Button btnVerifyLibrary;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
	}
}

