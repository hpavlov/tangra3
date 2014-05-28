namespace Tangra.VideoOperations.LightCurves.InfoForms
{
	partial class frmSetReferenceMag
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
			this.rb1 = new System.Windows.Forms.RadioButton();
			this.rb2 = new System.Windows.Forms.RadioButton();
			this.rb3 = new System.Windows.Forms.RadioButton();
			this.rb4 = new System.Windows.Forms.RadioButton();
			this.nudMag1 = new System.Windows.Forms.NumericUpDown();
			this.nudMag2 = new System.Windows.Forms.NumericUpDown();
			this.nudMag3 = new System.Windows.Forms.NumericUpDown();
			this.nudMag4 = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.pnl1 = new System.Windows.Forms.Panel();
			this.pb1 = new System.Windows.Forms.PictureBox();
			this.pb4 = new System.Windows.Forms.PictureBox();
			this.pb3 = new System.Windows.Forms.PictureBox();
			this.pb2 = new System.Windows.Forms.PictureBox();
			this.pnl2 = new System.Windows.Forms.Panel();
			this.pnl3 = new System.Windows.Forms.Panel();
			this.pnl4 = new System.Windows.Forms.Panel();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.nudMag1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMag2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMag3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMag4)).BeginInit();
			this.pnl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pb1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb2)).BeginInit();
			this.pnl2.SuspendLayout();
			this.pnl3.SuspendLayout();
			this.pnl4.SuspendLayout();
			this.SuspendLayout();
			// 
			// rb1
			// 
			this.rb1.AutoSize = true;
			this.rb1.Location = new System.Drawing.Point(111, 3);
			this.rb1.Name = "rb1";
			this.rb1.Size = new System.Drawing.Size(14, 13);
			this.rb1.TabIndex = 21;
			this.rb1.TabStop = true;
			this.rb1.UseVisualStyleBackColor = true;
			this.rb1.CheckedChanged += new System.EventHandler(this.ReferenceStartRadioButtonChanged);
			// 
			// rb2
			// 
			this.rb2.AutoSize = true;
			this.rb2.Location = new System.Drawing.Point(111, 4);
			this.rb2.Name = "rb2";
			this.rb2.Size = new System.Drawing.Size(14, 13);
			this.rb2.TabIndex = 22;
			this.rb2.TabStop = true;
			this.rb2.UseVisualStyleBackColor = true;
			this.rb2.CheckedChanged += new System.EventHandler(this.ReferenceStartRadioButtonChanged);
			// 
			// rb3
			// 
			this.rb3.AutoSize = true;
			this.rb3.Location = new System.Drawing.Point(111, 5);
			this.rb3.Name = "rb3";
			this.rb3.Size = new System.Drawing.Size(14, 13);
			this.rb3.TabIndex = 23;
			this.rb3.TabStop = true;
			this.rb3.UseVisualStyleBackColor = true;
			this.rb3.CheckedChanged += new System.EventHandler(this.ReferenceStartRadioButtonChanged);
			// 
			// rb4
			// 
			this.rb4.AutoSize = true;
			this.rb4.Location = new System.Drawing.Point(111, 5);
			this.rb4.Name = "rb4";
			this.rb4.Size = new System.Drawing.Size(14, 13);
			this.rb4.TabIndex = 24;
			this.rb4.TabStop = true;
			this.rb4.UseVisualStyleBackColor = true;
			this.rb4.CheckedChanged += new System.EventHandler(this.ReferenceStartRadioButtonChanged);
			// 
			// nudMag1
			// 
			this.nudMag1.DecimalPlaces = 2;
			this.nudMag1.Enabled = false;
			this.nudMag1.Location = new System.Drawing.Point(157, 2);
			this.nudMag1.Name = "nudMag1";
			this.nudMag1.Size = new System.Drawing.Size(49, 20);
			this.nudMag1.TabIndex = 25;
			this.nudMag1.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
			this.nudMag1.ValueChanged += new System.EventHandler(this.ReferenceMagnitudeChanged);
			// 
			// nudMag2
			// 
			this.nudMag2.DecimalPlaces = 2;
			this.nudMag2.Enabled = false;
			this.nudMag2.Location = new System.Drawing.Point(157, 2);
			this.nudMag2.Name = "nudMag2";
			this.nudMag2.Size = new System.Drawing.Size(49, 20);
			this.nudMag2.TabIndex = 26;
			this.nudMag2.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
			this.nudMag2.ValueChanged += new System.EventHandler(this.ReferenceMagnitudeChanged);
			// 
			// nudMag3
			// 
			this.nudMag3.DecimalPlaces = 2;
			this.nudMag3.Enabled = false;
			this.nudMag3.Location = new System.Drawing.Point(157, 3);
			this.nudMag3.Name = "nudMag3";
			this.nudMag3.Size = new System.Drawing.Size(49, 20);
			this.nudMag3.TabIndex = 27;
			this.nudMag3.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
			this.nudMag3.ValueChanged += new System.EventHandler(this.ReferenceMagnitudeChanged);
			// 
			// nudMag4
			// 
			this.nudMag4.DecimalPlaces = 2;
			this.nudMag4.Enabled = false;
			this.nudMag4.Location = new System.Drawing.Point(157, 3);
			this.nudMag4.Name = "nudMag4";
			this.nudMag4.Size = new System.Drawing.Size(49, 20);
			this.nudMag4.TabIndex = 28;
			this.nudMag4.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
			this.nudMag4.ValueChanged += new System.EventHandler(this.ReferenceMagnitudeChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(42, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 29;
			this.label1.Text = "label1";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(42, 4);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 13);
			this.label2.TabIndex = 30;
			this.label2.Text = "label2";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(42, 5);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 13);
			this.label3.TabIndex = 31;
			this.label3.Text = "label3";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(42, 5);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(35, 13);
			this.label4.TabIndex = 32;
			this.label4.Text = "label4";
			// 
			// pnl1
			// 
			this.pnl1.Controls.Add(this.label1);
			this.pnl1.Controls.Add(this.pb1);
			this.pnl1.Controls.Add(this.rb1);
			this.pnl1.Controls.Add(this.nudMag1);
			this.pnl1.Location = new System.Drawing.Point(5, 36);
			this.pnl1.Name = "pnl1";
			this.pnl1.Size = new System.Drawing.Size(213, 24);
			this.pnl1.TabIndex = 33;
			// 
			// pb1
			// 
			this.pb1.BackColor = System.Drawing.Color.Maroon;
			this.pb1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pb1.Location = new System.Drawing.Point(10, 6);
			this.pb1.Name = "pb1";
			this.pb1.Size = new System.Drawing.Size(10, 11);
			this.pb1.TabIndex = 17;
			this.pb1.TabStop = false;
			// 
			// pb4
			// 
			this.pb4.BackColor = System.Drawing.Color.Maroon;
			this.pb4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pb4.Location = new System.Drawing.Point(10, 7);
			this.pb4.Name = "pb4";
			this.pb4.Size = new System.Drawing.Size(10, 11);
			this.pb4.TabIndex = 20;
			this.pb4.TabStop = false;
			// 
			// pb3
			// 
			this.pb3.BackColor = System.Drawing.Color.Maroon;
			this.pb3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pb3.Location = new System.Drawing.Point(10, 6);
			this.pb3.Name = "pb3";
			this.pb3.Size = new System.Drawing.Size(10, 11);
			this.pb3.TabIndex = 19;
			this.pb3.TabStop = false;
			// 
			// pb2
			// 
			this.pb2.BackColor = System.Drawing.Color.Maroon;
			this.pb2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pb2.Location = new System.Drawing.Point(10, 6);
			this.pb2.Name = "pb2";
			this.pb2.Size = new System.Drawing.Size(10, 11);
			this.pb2.TabIndex = 18;
			this.pb2.TabStop = false;
			// 
			// pnl2
			// 
			this.pnl2.Controls.Add(this.label2);
			this.pnl2.Controls.Add(this.pb2);
			this.pnl2.Controls.Add(this.rb2);
			this.pnl2.Controls.Add(this.nudMag2);
			this.pnl2.Location = new System.Drawing.Point(5, 63);
			this.pnl2.Name = "pnl2";
			this.pnl2.Size = new System.Drawing.Size(213, 24);
			this.pnl2.TabIndex = 34;
			// 
			// pnl3
			// 
			this.pnl3.Controls.Add(this.label3);
			this.pnl3.Controls.Add(this.pb3);
			this.pnl3.Controls.Add(this.rb3);
			this.pnl3.Controls.Add(this.nudMag3);
			this.pnl3.Location = new System.Drawing.Point(5, 89);
			this.pnl3.Name = "pnl3";
			this.pnl3.Size = new System.Drawing.Size(213, 26);
			this.pnl3.TabIndex = 35;
			// 
			// pnl4
			// 
			this.pnl4.Controls.Add(this.label4);
			this.pnl4.Controls.Add(this.pb4);
			this.pnl4.Controls.Add(this.rb4);
			this.pnl4.Controls.Add(this.nudMag4);
			this.pnl4.Location = new System.Drawing.Point(5, 117);
			this.pnl4.Name = "pnl4";
			this.pnl4.Size = new System.Drawing.Size(213, 26);
			this.pnl4.TabIndex = 36;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(33, 167);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 37;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(114, 167);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 38;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(46, 9);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(46, 13);
			this.label5.TabIndex = 39;
			this.label5.Text = "Intensity";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(97, 9);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(57, 13);
			this.label6.TabIndex = 40;
			this.label6.Text = "Reference";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(159, 9);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(62, 13);
			this.label7.TabIndex = 41;
			this.label7.Text = "Magnitudes";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(4, 9);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(38, 13);
			this.label8.TabIndex = 42;
			this.label8.Text = "Target";
			// 
			// frmSetReferenceMag
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(223, 196);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.pnl4);
			this.Controls.Add(this.pnl3);
			this.Controls.Add(this.pnl2);
			this.Controls.Add(this.pnl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmSetReferenceMag";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Set Reference Stellar Magnitude";
			((System.ComponentModel.ISupportInitialize)(this.nudMag1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMag2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMag3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMag4)).EndInit();
			this.pnl1.ResumeLayout(false);
			this.pnl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pb1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb2)).EndInit();
			this.pnl2.ResumeLayout(false);
			this.pnl2.PerformLayout();
			this.pnl3.ResumeLayout(false);
			this.pnl3.PerformLayout();
			this.pnl4.ResumeLayout(false);
			this.pnl4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pb4;
		private System.Windows.Forms.PictureBox pb3;
		private System.Windows.Forms.PictureBox pb2;
		private System.Windows.Forms.PictureBox pb1;
		private System.Windows.Forms.RadioButton rb1;
		private System.Windows.Forms.RadioButton rb2;
		private System.Windows.Forms.RadioButton rb3;
		private System.Windows.Forms.RadioButton rb4;
		private System.Windows.Forms.NumericUpDown nudMag1;
		private System.Windows.Forms.NumericUpDown nudMag2;
		private System.Windows.Forms.NumericUpDown nudMag3;
		private System.Windows.Forms.NumericUpDown nudMag4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Panel pnl1;
		private System.Windows.Forms.Panel pnl2;
		private System.Windows.Forms.Panel pnl3;
		private System.Windows.Forms.Panel pnl4;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
	}
}