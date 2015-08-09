﻿namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	partial class frmConfigureProcessing
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
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.nudResolution = new System.Windows.Forms.NumericUpDown();
			this.nudMaxWavelength = new System.Windows.Forms.NumericUpDown();
			this.nudMinWavelength = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.cbxUseBlurring = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.nudResolution)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxWavelength)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinWavelength)).BeginInit();
			this.SuspendLayout();
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(193, 73);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(14, 13);
			this.label6.TabIndex = 28;
			this.label6.Text = "A";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(193, 50);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(14, 13);
			this.label5.TabIndex = 27;
			this.label5.Text = "A";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(193, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(14, 13);
			this.label4.TabIndex = 26;
			this.label4.Text = "A";
			// 
			// nudResolution
			// 
			this.nudResolution.Location = new System.Drawing.Point(128, 69);
			this.nudResolution.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.nudResolution.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudResolution.Name = "nudResolution";
			this.nudResolution.Size = new System.Drawing.Size(64, 20);
			this.nudResolution.TabIndex = 25;
			this.nudResolution.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// nudMaxWavelength
			// 
			this.nudMaxWavelength.Location = new System.Drawing.Point(128, 45);
			this.nudMaxWavelength.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.nudMaxWavelength.Minimum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
			this.nudMaxWavelength.Name = "nudMaxWavelength";
			this.nudMaxWavelength.Size = new System.Drawing.Size(64, 20);
			this.nudMaxWavelength.TabIndex = 24;
			this.nudMaxWavelength.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			// 
			// nudMinWavelength
			// 
			this.nudMinWavelength.Location = new System.Drawing.Point(128, 20);
			this.nudMinWavelength.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.nudMinWavelength.Minimum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
			this.nudMinWavelength.Name = "nudMinWavelength";
			this.nudMinWavelength.Size = new System.Drawing.Size(64, 20);
			this.nudMinWavelength.TabIndex = 23;
			this.nudMinWavelength.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(30, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(91, 13);
			this.label3.TabIndex = 22;
			this.label3.Text = "Max Wavelength:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(33, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 13);
			this.label2.TabIndex = 21;
			this.label2.Text = "Min Wavelength:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(58, 71);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 13);
			this.label1.TabIndex = 20;
			this.label1.Text = "Resolution:";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(124, 238);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 29;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(205, 238);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 30;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// cbxUseBlurring
			// 
			this.cbxUseBlurring.AutoSize = true;
			this.cbxUseBlurring.Location = new System.Drawing.Point(33, 136);
			this.cbxUseBlurring.Name = "cbxUseBlurring";
			this.cbxUseBlurring.Size = new System.Drawing.Size(83, 17);
			this.cbxUseBlurring.TabIndex = 31;
			this.cbxUseBlurring.Text = "Use Blurring";
			this.cbxUseBlurring.UseVisualStyleBackColor = true;
			// 
			// frmConfigureProcessing
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.cbxUseBlurring);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.nudResolution);
			this.Controls.Add(this.nudMaxWavelength);
			this.Controls.Add(this.nudMinWavelength);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmConfigureProcessing";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configure Processing";
			((System.ComponentModel.ISupportInitialize)(this.nudResolution)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxWavelength)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinWavelength)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown nudResolution;
		private System.Windows.Forms.NumericUpDown nudMaxWavelength;
		private System.Windows.Forms.NumericUpDown nudMinWavelength;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.CheckBox cbxUseBlurring;
	}
}