using Tangra.Model.Controls;

namespace Tangra.VideoOperations.Astrometry
{
	partial class frmConfigureAstrometricFit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfigureAstrometricFit));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pnlKnownField = new System.Windows.Forms.Panel();
            this.lblErrorDegrees = new System.Windows.Forms.Label();
            this.nudError = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlKnownObject = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.rbKnownObject = new System.Windows.Forms.RadioButton();
            this.rbKnownCenter = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pnlObject = new System.Windows.Forms.Panel();
            this.pnlTime = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblOCRTimeWarning = new System.Windows.Forms.Label();
            this.rbSelectedLimitMagnitude = new System.Windows.Forms.RadioButton();
            this.rbAutomaticLimitMagnitude = new System.Windows.Forms.RadioButton();
            this.pnlSelectedLimitMagnitude = new System.Windows.Forms.Panel();
            this.nudFaintestMag = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btnLastUsedDateTime = new System.Windows.Forms.Button();
            this.btnCurrDateTime = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.utcTime = new Tangra.Model.Controls.ucUtcTimePicker();
            this.cbxDE = new Tangra.Model.Controls.PersistableDropDown();
            this.cbxRA = new Tangra.Model.Controls.PersistableDropDown();
            this.cbxObject = new Tangra.Model.Controls.PersistableDropDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxObsCode = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.pnlKnownField.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudError)).BeginInit();
            this.pnlKnownObject.SuspendLayout();
            this.pnlObject.SuspendLayout();
            this.pnlTime.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.pnlSelectedLimitMagnitude.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFaintestMag)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pnlKnownField);
            this.groupBox2.Controls.Add(this.pnlKnownObject);
            this.groupBox2.Controls.Add(this.rbKnownObject);
            this.groupBox2.Controls.Add(this.rbKnownCenter);
            this.groupBox2.Location = new System.Drawing.Point(7, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(406, 205);
            this.groupBox2.TabIndex = 62;
            this.groupBox2.TabStop = false;
            // 
            // pnlKnownField
            // 
            this.pnlKnownField.Controls.Add(this.lblErrorDegrees);
            this.pnlKnownField.Controls.Add(this.cbxDE);
            this.pnlKnownField.Controls.Add(this.cbxRA);
            this.pnlKnownField.Controls.Add(this.nudError);
            this.pnlKnownField.Controls.Add(this.label5);
            this.pnlKnownField.Controls.Add(this.label3);
            this.pnlKnownField.Controls.Add(this.label4);
            this.pnlKnownField.Location = new System.Drawing.Point(28, 122);
            this.pnlKnownField.Name = "pnlKnownField";
            this.pnlKnownField.Size = new System.Drawing.Size(365, 59);
            this.pnlKnownField.TabIndex = 65;
            // 
            // lblErrorDegrees
            // 
            this.lblErrorDegrees.AutoSize = true;
            this.lblErrorDegrees.Location = new System.Drawing.Point(303, 32);
            this.lblErrorDegrees.Name = "lblErrorDegrees";
            this.lblErrorDegrees.Size = new System.Drawing.Size(0, 13);
            this.lblErrorDegrees.TabIndex = 72;
            // 
            // nudError
            // 
            this.nudError.DecimalPlaces = 1;
            this.nudError.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudError.Location = new System.Drawing.Point(243, 26);
            this.nudError.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudError.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudError.Name = "nudError";
            this.nudError.Size = new System.Drawing.Size(51, 20);
            this.nudError.TabIndex = 66;
            this.nudError.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudError.ValueChanged += new System.EventHandler(this.nudError_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(235, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 13);
            this.label5.TabIndex = 65;
            this.label5.Text = "Max error (fields of view):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 61;
            this.label3.Text = "RA (HH MM SS)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 63;
            this.label4.Text = "DE (DD MM SS.T)";
            // 
            // pnlKnownObject
            // 
            this.pnlKnownObject.Controls.Add(this.tbxObsCode);
            this.pnlKnownObject.Controls.Add(this.label1);
            this.pnlKnownObject.Controls.Add(this.label9);
            this.pnlKnownObject.Controls.Add(this.cbxObject);
            this.pnlKnownObject.Enabled = false;
            this.pnlKnownObject.Location = new System.Drawing.Point(31, 30);
            this.pnlKnownObject.Name = "pnlKnownObject";
            this.pnlKnownObject.Size = new System.Drawing.Size(362, 54);
            this.pnlKnownObject.TabIndex = 70;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(137, 4);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(154, 26);
            this.label9.TabIndex = 72;
            this.label9.Text = "e.g. 13203, 116P, C/2023 H3, \r\n        2000 CO101, CK06W030";
            // 
            // rbKnownObject
            // 
            this.rbKnownObject.AutoSize = true;
            this.rbKnownObject.Location = new System.Drawing.Point(10, 11);
            this.rbKnownObject.Name = "rbKnownObject";
            this.rbKnownObject.Size = new System.Drawing.Size(150, 17);
            this.rbKnownObject.TabIndex = 65;
            this.rbKnownObject.Text = "Known object in the field ()";
            this.rbKnownObject.UseVisualStyleBackColor = true;
            this.rbKnownObject.CheckedChanged += new System.EventHandler(this.rbKnownObject_CheckedChanged);
            // 
            // rbKnownCenter
            // 
            this.rbKnownCenter.AutoSize = true;
            this.rbKnownCenter.Checked = true;
            this.rbKnownCenter.Location = new System.Drawing.Point(10, 103);
            this.rbKnownCenter.Name = "rbKnownCenter";
            this.rbKnownCenter.Size = new System.Drawing.Size(173, 17);
            this.rbKnownCenter.TabIndex = 0;
            this.rbKnownCenter.TabStop = true;
            this.rbKnownCenter.Text = "Known approximate field center";
            this.rbKnownCenter.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(261, 224);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 63;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(342, 224);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 64;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pnlObject
            // 
            this.pnlObject.Controls.Add(this.groupBox2);
            this.pnlObject.Location = new System.Drawing.Point(3, -1);
            this.pnlObject.Name = "pnlObject";
            this.pnlObject.Size = new System.Drawing.Size(434, 216);
            this.pnlObject.TabIndex = 66;
            // 
            // pnlTime
            // 
            this.pnlTime.Controls.Add(this.groupBox4);
            this.pnlTime.Location = new System.Drawing.Point(2, 0);
            this.pnlTime.Name = "pnlTime";
            this.pnlTime.Size = new System.Drawing.Size(413, 218);
            this.pnlTime.TabIndex = 67;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblOCRTimeWarning);
            this.groupBox4.Controls.Add(this.rbSelectedLimitMagnitude);
            this.groupBox4.Controls.Add(this.rbAutomaticLimitMagnitude);
            this.groupBox4.Controls.Add(this.pnlSelectedLimitMagnitude);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.btnLastUsedDateTime);
            this.groupBox4.Controls.Add(this.btnCurrDateTime);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.utcTime);
            this.groupBox4.Location = new System.Drawing.Point(7, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(406, 206);
            this.groupBox4.TabIndex = 73;
            this.groupBox4.TabStop = false;
            // 
            // lblOCRTimeWarning
            // 
            this.lblOCRTimeWarning.AutoSize = true;
            this.lblOCRTimeWarning.ForeColor = System.Drawing.Color.Red;
            this.lblOCRTimeWarning.Location = new System.Drawing.Point(14, 102);
            this.lblOCRTimeWarning.Name = "lblOCRTimeWarning";
            this.lblOCRTimeWarning.Size = new System.Drawing.Size(386, 13);
            this.lblOCRTimeWarning.TabIndex = 81;
            this.lblOCRTimeWarning.Text = "The time was read from the OSD timestamp. Please set the correct date above!!!";
            this.lblOCRTimeWarning.Visible = false;
            // 
            // rbSelectedLimitMagnitude
            // 
            this.rbSelectedLimitMagnitude.AutoSize = true;
            this.rbSelectedLimitMagnitude.Location = new System.Drawing.Point(44, 175);
            this.rbSelectedLimitMagnitude.Name = "rbSelectedLimitMagnitude";
            this.rbSelectedLimitMagnitude.Size = new System.Drawing.Size(14, 13);
            this.rbSelectedLimitMagnitude.TabIndex = 80;
            this.rbSelectedLimitMagnitude.UseVisualStyleBackColor = true;
            // 
            // rbAutomaticLimitMagnitude
            // 
            this.rbAutomaticLimitMagnitude.AutoSize = true;
            this.rbAutomaticLimitMagnitude.Checked = true;
            this.rbAutomaticLimitMagnitude.Cursor = System.Windows.Forms.Cursors.Default;
            this.rbAutomaticLimitMagnitude.Location = new System.Drawing.Point(44, 150);
            this.rbAutomaticLimitMagnitude.Name = "rbAutomaticLimitMagnitude";
            this.rbAutomaticLimitMagnitude.Size = new System.Drawing.Size(135, 17);
            this.rbAutomaticLimitMagnitude.TabIndex = 79;
            this.rbAutomaticLimitMagnitude.TabStop = true;
            this.rbAutomaticLimitMagnitude.Text = "determine automatically";
            this.rbAutomaticLimitMagnitude.UseVisualStyleBackColor = true;
            this.rbAutomaticLimitMagnitude.CheckedChanged += new System.EventHandler(this.rbAutomaticLimitMagnitude_CheckedChanged);
            // 
            // pnlSelectedLimitMagnitude
            // 
            this.pnlSelectedLimitMagnitude.Controls.Add(this.nudFaintestMag);
            this.pnlSelectedLimitMagnitude.Controls.Add(this.label12);
            this.pnlSelectedLimitMagnitude.Enabled = false;
            this.pnlSelectedLimitMagnitude.Location = new System.Drawing.Point(64, 170);
            this.pnlSelectedLimitMagnitude.Name = "pnlSelectedLimitMagnitude";
            this.pnlSelectedLimitMagnitude.Size = new System.Drawing.Size(73, 25);
            this.pnlSelectedLimitMagnitude.TabIndex = 78;
            // 
            // nudFaintestMag
            // 
            this.nudFaintestMag.Location = new System.Drawing.Point(3, 3);
            this.nudFaintestMag.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudFaintestMag.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudFaintestMag.Name = "nudFaintestMag";
            this.nudFaintestMag.Size = new System.Drawing.Size(43, 20);
            this.nudFaintestMag.TabIndex = 76;
            this.nudFaintestMag.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(50, 10);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(15, 13);
            this.label12.TabIndex = 77;
            this.label12.Text = "m";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 127);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(244, 13);
            this.label11.TabIndex = 75;
            this.label11.Text = "Faintest detectable stellar magnitude on the video:";
            // 
            // btnLastUsedDateTime
            // 
            this.btnLastUsedDateTime.Location = new System.Drawing.Point(196, 76);
            this.btnLastUsedDateTime.Name = "btnLastUsedDateTime";
            this.btnLastUsedDateTime.Size = new System.Drawing.Size(150, 23);
            this.btnLastUsedDateTime.TabIndex = 74;
            this.btnLastUsedDateTime.Text = "Last Used Date && Time";
            this.btnLastUsedDateTime.UseVisualStyleBackColor = true;
            this.btnLastUsedDateTime.Click += new System.EventHandler(this.btnLastUsedDateTime_Click);
            // 
            // btnCurrDateTime
            // 
            this.btnCurrDateTime.Location = new System.Drawing.Point(40, 77);
            this.btnCurrDateTime.Name = "btnCurrDateTime";
            this.btnCurrDateTime.Size = new System.Drawing.Size(150, 23);
            this.btnCurrDateTime.TabIndex = 73;
            this.btnCurrDateTime.Text = "Current Date && Time (Now)";
            this.btnCurrDateTime.UseVisualStyleBackColor = true;
            this.btnCurrDateTime.Click += new System.EventHandler(this.btnCurrDateTime_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(177, 13);
            this.label10.TabIndex = 72;
            this.label10.Text = "Enter UTC time of the current frame:";
            // 
            // utcTime
            // 
            this.utcTime.DateTimeUtc = new System.DateTime(2009, 8, 9, 3, 37, 14, 390);
            this.utcTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.utcTime.Location = new System.Drawing.Point(37, 42);
            this.utcTime.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.utcTime.Name = "utcTime";
            this.utcTime.Size = new System.Drawing.Size(309, 34);
            this.utcTime.TabIndex = 0;
            // 
            // cbxDE
            // 
            this.cbxDE.FormattingEnabled = true;
            this.cbxDE.Location = new System.Drawing.Point(104, 32);
            this.cbxDE.Name = "cbxDE";
            this.cbxDE.PersistanceKey = "DE";
            this.cbxDE.RegistryKey = "Software\\Tangra";
            this.cbxDE.Size = new System.Drawing.Size(108, 21);
            this.cbxDE.TabIndex = 71;
            // 
            // cbxRA
            // 
            this.cbxRA.FormattingEnabled = true;
            this.cbxRA.Location = new System.Drawing.Point(104, 6);
            this.cbxRA.Name = "cbxRA";
            this.cbxRA.PersistanceKey = "RA";
            this.cbxRA.RegistryKey = "Software\\Tangra";
            this.cbxRA.Size = new System.Drawing.Size(108, 21);
            this.cbxRA.TabIndex = 70;
            // 
            // cbxObject
            // 
            this.cbxObject.FormattingEnabled = true;
            this.cbxObject.Location = new System.Drawing.Point(3, 2);
            this.cbxObject.Name = "cbxObject";
            this.cbxObject.PersistanceKey = "Object";
            this.cbxObject.RegistryKey = "Software\\Tangra";
            this.cbxObject.Size = new System.Drawing.Size(129, 21);
            this.cbxObject.TabIndex = 71;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 73;
            this.label1.Text = "MPC Obs Code:";
            // 
            // tbxObsCode
            // 
            this.tbxObsCode.Location = new System.Drawing.Point(101, 31);
            this.tbxObsCode.Name = "tbxObsCode";
            this.tbxObsCode.Size = new System.Drawing.Size(31, 20);
            this.tbxObsCode.TabIndex = 74;
            // 
            // frmConfigureAstrometricFit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 253);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pnlTime);
            this.Controls.Add(this.pnlObject);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfigureAstrometricFit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Identify Star Field";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.pnlKnownField.ResumeLayout(false);
            this.pnlKnownField.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudError)).EndInit();
            this.pnlKnownObject.ResumeLayout(false);
            this.pnlKnownObject.PerformLayout();
            this.pnlObject.ResumeLayout(false);
            this.pnlTime.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.pnlSelectedLimitMagnitude.ResumeLayout(false);
            this.pnlSelectedLimitMagnitude.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFaintestMag)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.RadioButton rbKnownCenter;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.RadioButton rbKnownObject;
		private System.Windows.Forms.Panel pnlKnownField;
		private System.Windows.Forms.Panel pnlKnownObject;
		private System.Windows.Forms.NumericUpDown nudError;
		private System.Windows.Forms.Label label5;
		private PersistableDropDown cbxRA;
		private PersistableDropDown cbxDE;
		private PersistableDropDown cbxObject;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Panel pnlObject;
		private System.Windows.Forms.Panel pnlTime;
		private ucUtcTimePicker utcTime;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label lblErrorDegrees;
        private System.Windows.Forms.Button btnLastUsedDateTime;
		private System.Windows.Forms.Button btnCurrDateTime;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown nudFaintestMag;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.RadioButton rbSelectedLimitMagnitude;
		private System.Windows.Forms.RadioButton rbAutomaticLimitMagnitude;
		private System.Windows.Forms.Panel pnlSelectedLimitMagnitude;
        private System.Windows.Forms.Label lblOCRTimeWarning;
        private System.Windows.Forms.TextBox tbxObsCode;
        private System.Windows.Forms.Label label1;
	}
}