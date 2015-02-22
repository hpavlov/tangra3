namespace Tangra.VideoOperations.Astrometry
{
	partial class frmSolveConfiguration
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
			this.tbxFocalLength = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.tbxLimitMag = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cbxDE = new Tangra.Model.Controls.PersistableDropDown();
			this.cbxRA = new Tangra.Model.Controls.PersistableDropDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.dtpEpoch = new System.Windows.Forms.DateTimePicker();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbxFocalLength
			// 
			this.tbxFocalLength.Location = new System.Drawing.Point(144, 56);
			this.tbxFocalLength.Name = "tbxFocalLength";
			this.tbxFocalLength.Size = new System.Drawing.Size(49, 20);
			this.tbxFocalLength.TabIndex = 66;
			this.tbxFocalLength.Text = "660";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(39, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 13);
			this.label2.TabIndex = 65;
			this.label2.Text = "Focal Length (mm)";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(64, 251);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 76;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(20, 63);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(95, 13);
			this.label4.TabIndex = 67;
			this.label4.Text = "DE (DD MM SS.T)";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(38, 90);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(95, 13);
			this.label1.TabIndex = 67;
			this.label1.Text = "Limiting Magnitude";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(145, 251);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 75;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// tbxLimitMag
			// 
			this.tbxLimitMag.Location = new System.Drawing.Point(144, 87);
			this.tbxLimitMag.Name = "tbxLimitMag";
			this.tbxLimitMag.Size = new System.Drawing.Size(49, 20);
			this.tbxLimitMag.TabIndex = 68;
			this.tbxLimitMag.Text = "13";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.dtpEpoch);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.tbxLimitMag);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.tbxFocalLength);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(13, 118);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(260, 118);
			this.groupBox2.TabIndex = 74;
			this.groupBox2.TabStop = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cbxDE);
			this.groupBox1.Controls.Add(this.cbxRA);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(261, 100);
			this.groupBox1.TabIndex = 73;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Approximate Coordinates of the Field Center";
			// 
			// cbxDE
			// 
			this.cbxDE.FormattingEnabled = true;
			this.cbxDE.Location = new System.Drawing.Point(121, 60);
			this.cbxDE.Name = "cbxDE";
			this.cbxDE.PersistanceKey = "DE";
			this.cbxDE.RegistryKey = "Software\\Tangra";
			this.cbxDE.Size = new System.Drawing.Size(121, 21);
			this.cbxDE.TabIndex = 70;
			// 
			// cbxRA
			// 
			this.cbxRA.FormattingEnabled = true;
			this.cbxRA.Location = new System.Drawing.Point(122, 31);
			this.cbxRA.Name = "cbxRA";
			this.cbxRA.PersistanceKey = "RA";
			this.cbxRA.RegistryKey = "Software\\Tangra";
			this.cbxRA.Size = new System.Drawing.Size(121, 21);
			this.cbxRA.TabIndex = 69;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(20, 34);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(85, 13);
			this.label3.TabIndex = 65;
			this.label3.Text = "RA (HH MM SS)";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(36, 27);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(98, 13);
			this.label5.TabIndex = 69;
			this.label5.Text = "Observation Epoch";
			// 
			// dtpEpoch
			// 
			this.dtpEpoch.CustomFormat = "MMM yyyy";
			this.dtpEpoch.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpEpoch.Location = new System.Drawing.Point(144, 25);
			this.dtpEpoch.Name = "dtpEpoch";
			this.dtpEpoch.Size = new System.Drawing.Size(97, 20);
			this.dtpEpoch.TabIndex = 70;
			// 
			// frmSolveConfiguration
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(286, 286);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSolveConfiguration";
			this.Text = "Solve Configuration";
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		protected internal System.Windows.Forms.TextBox tbxFocalLength;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnCancel;
		protected internal System.Windows.Forms.TextBox tbxLimitMag;
		private Tangra.Model.Controls.PersistableDropDown cbxDE;
		private Tangra.Model.Controls.PersistableDropDown cbxRA;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.DateTimePicker dtpEpoch;
	}
}