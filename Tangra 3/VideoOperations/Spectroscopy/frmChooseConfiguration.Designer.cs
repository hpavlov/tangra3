namespace Tangra.VideoOperations.Spectroscopy
{
	partial class frmChooseConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChooseConfiguration));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pnlNotCalibrated = new System.Windows.Forms.Panel();
            this.label21 = new System.Windows.Forms.Label();
            this.lblNotCalibrated = new System.Windows.Forms.Label();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelConfig = new System.Windows.Forms.Button();
            this.btnNewConfig = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.cbxSavedConfigurations = new System.Windows.Forms.ComboBox();
            this.pnlCalibrated = new System.Windows.Forms.Panel();
            this.lblRMS = new System.Windows.Forms.Label();
            this.lblDispersion = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCalibratedCaption = new System.Windows.Forms.Label();
            this.pnlNotCalibrated.SuspendLayout();
            this.pnlCalibrated.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(306, 148);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(92, 32);
            this.btnCancel.TabIndex = 83;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(207, 148);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(93, 32);
            this.btnOK.TabIndex = 82;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pnlNotCalibrated
            // 
            this.pnlNotCalibrated.Controls.Add(this.label21);
            this.pnlNotCalibrated.Controls.Add(this.lblNotCalibrated);
            this.pnlNotCalibrated.Location = new System.Drawing.Point(11, 66);
            this.pnlNotCalibrated.Name = "pnlNotCalibrated";
            this.pnlNotCalibrated.Size = new System.Drawing.Size(394, 76);
            this.pnlNotCalibrated.TabIndex = 84;
            this.pnlNotCalibrated.Visible = false;
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(3, 27);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(379, 52);
            this.label21.TabIndex = 68;
            this.label21.Text = "     After a configuration has been calibrated once, Tangra will be able to auto-" +
    "calibrate spectras that have used the same configuration.";
            // 
            // lblNotCalibrated
            // 
            this.lblNotCalibrated.AutoSize = true;
            this.lblNotCalibrated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblNotCalibrated.ForeColor = System.Drawing.Color.Red;
            this.lblNotCalibrated.Location = new System.Drawing.Point(3, 2);
            this.lblNotCalibrated.Name = "lblNotCalibrated";
            this.lblNotCalibrated.Size = new System.Drawing.Size(167, 13);
            this.lblNotCalibrated.TabIndex = 53;
            this.lblNotCalibrated.Text = "Configuration Not Calibrated";
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(315, 25);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(41, 23);
            this.btnEdit.TabIndex = 81;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelConfig
            // 
            this.btnDelConfig.Location = new System.Drawing.Point(357, 25);
            this.btnDelConfig.Name = "btnDelConfig";
            this.btnDelConfig.Size = new System.Drawing.Size(41, 23);
            this.btnDelConfig.TabIndex = 80;
            this.btnDelConfig.Text = "Del";
            this.btnDelConfig.UseVisualStyleBackColor = true;
            this.btnDelConfig.Click += new System.EventHandler(this.btnDelConfig_Click);
            // 
            // btnNewConfig
            // 
            this.btnNewConfig.Location = new System.Drawing.Point(273, 25);
            this.btnNewConfig.Name = "btnNewConfig";
            this.btnNewConfig.Size = new System.Drawing.Size(41, 23);
            this.btnNewConfig.TabIndex = 79;
            this.btnNewConfig.Text = "New";
            this.btnNewConfig.UseVisualStyleBackColor = true;
            this.btnNewConfig.Click += new System.EventHandler(this.btnNewConfig_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(119, 13);
            this.label11.TabIndex = 78;
            this.label11.Text = "Calibrated Configuration";
            // 
            // cbxSavedConfigurations
            // 
            this.cbxSavedConfigurations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSavedConfigurations.FormattingEnabled = true;
            this.cbxSavedConfigurations.Items.AddRange(new object[] {
            "<No Configs Available - Press \'New\'>"});
            this.cbxSavedConfigurations.Location = new System.Drawing.Point(15, 27);
            this.cbxSavedConfigurations.Name = "cbxSavedConfigurations";
            this.cbxSavedConfigurations.Size = new System.Drawing.Size(252, 21);
            this.cbxSavedConfigurations.TabIndex = 77;
            this.cbxSavedConfigurations.SelectedIndexChanged += new System.EventHandler(this.cbxSavedConfigurations_SelectedIndexChanged);
            // 
            // pnlCalibrated
            // 
            this.pnlCalibrated.Controls.Add(this.lblRMS);
            this.pnlCalibrated.Controls.Add(this.lblDispersion);
            this.pnlCalibrated.Controls.Add(this.label3);
            this.pnlCalibrated.Controls.Add(this.label1);
            this.pnlCalibrated.Controls.Add(this.lblCalibratedCaption);
            this.pnlCalibrated.Location = new System.Drawing.Point(11, 66);
            this.pnlCalibrated.Name = "pnlCalibrated";
            this.pnlCalibrated.Size = new System.Drawing.Size(394, 76);
            this.pnlCalibrated.TabIndex = 85;
            this.pnlCalibrated.Visible = false;
            // 
            // lblRMS
            // 
            this.lblRMS.AutoSize = true;
            this.lblRMS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRMS.Location = new System.Drawing.Point(83, 46);
            this.lblRMS.Name = "lblRMS";
            this.lblRMS.Size = new System.Drawing.Size(52, 13);
            this.lblRMS.TabIndex = 87;
            this.lblRMS.Text = "0.45 pix";
            // 
            // lblDispersion
            // 
            this.lblDispersion.AutoSize = true;
            this.lblDispersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispersion.Location = new System.Drawing.Point(83, 27);
            this.lblDispersion.Name = "lblDispersion";
            this.lblDispersion.Size = new System.Drawing.Size(73, 13);
            this.lblDispersion.TabIndex = 86;
            this.lblDispersion.Text = "18.21 A/pix";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 55;
            this.label3.Text = "RMS:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 54;
            this.label1.Text = "Dispersion: ";
            // 
            // lblCalibratedCaption
            // 
            this.lblCalibratedCaption.AutoSize = true;
            this.lblCalibratedCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCalibratedCaption.ForeColor = System.Drawing.Color.Navy;
            this.lblCalibratedCaption.Location = new System.Drawing.Point(3, 2);
            this.lblCalibratedCaption.Name = "lblCalibratedCaption";
            this.lblCalibratedCaption.Size = new System.Drawing.Size(195, 13);
            this.lblCalibratedCaption.TabIndex = 53;
            this.lblCalibratedCaption.Text = "2-nd Order Polynomial Calibration";
            // 
            // frmChooseConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 195);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelConfig);
            this.Controls.Add(this.btnNewConfig);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.cbxSavedConfigurations);
            this.Controls.Add(this.pnlNotCalibrated);
            this.Controls.Add(this.pnlCalibrated);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmChooseConfiguration";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Wavelength Calibration";
            this.pnlNotCalibrated.ResumeLayout(false);
            this.pnlNotCalibrated.PerformLayout();
            this.pnlCalibrated.ResumeLayout(false);
            this.pnlCalibrated.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel pnlNotCalibrated;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label lblNotCalibrated;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Button btnDelConfig;
		private System.Windows.Forms.Button btnNewConfig;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.ComboBox cbxSavedConfigurations;
        private System.Windows.Forms.Panel pnlCalibrated;
        private System.Windows.Forms.Label lblRMS;
        private System.Windows.Forms.Label lblDispersion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCalibratedCaption;

	}
}