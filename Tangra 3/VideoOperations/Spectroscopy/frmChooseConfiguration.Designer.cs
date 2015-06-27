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
			this.pnlNotSolved = new System.Windows.Forms.Panel();
			this.cbxSolveConstantsNow = new System.Windows.Forms.CheckBox();
			this.label21 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.btnEdit = new System.Windows.Forms.Button();
			this.btnDelConfig = new System.Windows.Forms.Button();
			this.btnNewConfig = new System.Windows.Forms.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.cbxSavedConfigurations = new System.Windows.Forms.ComboBox();
			this.pnlNotSolved.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(304, 175);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(92, 32);
			this.btnCancel.TabIndex = 83;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(205, 175);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(93, 32);
			this.btnOK.TabIndex = 82;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// pnlNotSolved
			// 
			this.pnlNotSolved.Controls.Add(this.cbxSolveConstantsNow);
			this.pnlNotSolved.Controls.Add(this.label21);
			this.pnlNotSolved.Controls.Add(this.label20);
			this.pnlNotSolved.Location = new System.Drawing.Point(11, 66);
			this.pnlNotSolved.Name = "pnlNotSolved";
			this.pnlNotSolved.Size = new System.Drawing.Size(394, 103);
			this.pnlNotSolved.TabIndex = 84;
			// 
			// cbxSolveConstantsNow
			// 
			this.cbxSolveConstantsNow.AutoSize = true;
			this.cbxSolveConstantsNow.Location = new System.Drawing.Point(177, 1);
			this.cbxSolveConstantsNow.Name = "cbxSolveConstantsNow";
			this.cbxSolveConstantsNow.Size = new System.Drawing.Size(92, 17);
			this.cbxSolveConstantsNow.TabIndex = 53;
			this.cbxSolveConstantsNow.Text = "Calibrate Now";
			this.cbxSolveConstantsNow.UseVisualStyleBackColor = true;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(3, 27);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(379, 52);
			this.label21.TabIndex = 68;
			this.label21.Text = resources.GetString("label21.Text");
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label20.ForeColor = System.Drawing.Color.Red;
			this.label20.Location = new System.Drawing.Point(3, 2);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(167, 13);
			this.label20.TabIndex = 53;
			this.label20.Text = "Configuration Not Calibrated";
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
			// frmChooseConfiguration
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(417, 237);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.pnlNotSolved);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.btnDelConfig);
			this.Controls.Add(this.btnNewConfig);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.cbxSavedConfigurations);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmChooseConfiguration";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Wavelength Calibration";
			this.pnlNotSolved.ResumeLayout(false);
			this.pnlNotSolved.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Panel pnlNotSolved;
		private System.Windows.Forms.CheckBox cbxSolveConstantsNow;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Button btnDelConfig;
		private System.Windows.Forms.Button btnNewConfig;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.ComboBox cbxSavedConfigurations;

	}
}