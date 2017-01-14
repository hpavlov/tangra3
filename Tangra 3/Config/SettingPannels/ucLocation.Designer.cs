namespace Tangra.Config.SettingPannels
{
    partial class ucLocation
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.btnMPCHeader = new System.Windows.Forms.Button();
            this.tbxMPCCode = new System.Windows.Forms.TextBox();
            this.rbMPCCode = new System.Windows.Forms.RadioButton();
            this.rbCoordinates = new System.Windows.Forms.RadioButton();
            this.pnlCoordinates = new System.Windows.Forms.Panel();
            this.tbxLatitude = new System.Windows.Forms.TextBox();
            this.tbxLongitude = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.cbxLongitude = new System.Windows.Forms.ComboBox();
            this.label30 = new System.Windows.Forms.Label();
            this.cbxLatitude = new System.Windows.Forms.ComboBox();
            this.label32 = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.pnlCoordinates.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnMPCHeader
            // 
            this.btnMPCHeader.Location = new System.Drawing.Point(223, 66);
            this.btnMPCHeader.Name = "btnMPCHeader";
            this.btnMPCHeader.Size = new System.Drawing.Size(121, 23);
            this.btnMPCHeader.TabIndex = 46;
            this.btnMPCHeader.Text = "MPC Reports Header";
            this.btnMPCHeader.UseVisualStyleBackColor = true;
            this.btnMPCHeader.Click += new System.EventHandler(this.btnMPCHeader_Click);
            // 
            // tbxMPCCode
            // 
            this.tbxMPCCode.Enabled = false;
            this.tbxMPCCode.Location = new System.Drawing.Point(162, 67);
            this.tbxMPCCode.Name = "tbxMPCCode";
            this.tbxMPCCode.Size = new System.Drawing.Size(56, 20);
            this.tbxMPCCode.TabIndex = 45;
            // 
            // rbMPCCode
            // 
            this.rbMPCCode.AutoSize = true;
            this.rbMPCCode.Location = new System.Drawing.Point(21, 68);
            this.rbMPCCode.Name = "rbMPCCode";
            this.rbMPCCode.Size = new System.Drawing.Size(139, 17);
            this.rbMPCCode.TabIndex = 44;
            this.rbMPCCode.Text = "MPC Observatory Code:";
            this.rbMPCCode.UseVisualStyleBackColor = true;
            this.rbMPCCode.CheckedChanged += new System.EventHandler(this.rbCoordinates_CheckedChanged);
            // 
            // rbCoordinates
            // 
            this.rbCoordinates.AutoSize = true;
            this.rbCoordinates.Checked = true;
            this.rbCoordinates.Location = new System.Drawing.Point(21, 35);
            this.rbCoordinates.Name = "rbCoordinates";
            this.rbCoordinates.Size = new System.Drawing.Size(14, 13);
            this.rbCoordinates.TabIndex = 43;
            this.rbCoordinates.TabStop = true;
            this.rbCoordinates.UseVisualStyleBackColor = true;
            this.rbCoordinates.CheckedChanged += new System.EventHandler(this.rbCoordinates_CheckedChanged);
            // 
            // pnlCoordinates
            // 
            this.pnlCoordinates.Controls.Add(this.tbxLatitude);
            this.pnlCoordinates.Controls.Add(this.tbxLongitude);
            this.pnlCoordinates.Controls.Add(this.label27);
            this.pnlCoordinates.Controls.Add(this.cbxLongitude);
            this.pnlCoordinates.Controls.Add(this.label30);
            this.pnlCoordinates.Controls.Add(this.cbxLatitude);
            this.pnlCoordinates.Location = new System.Drawing.Point(41, 26);
            this.pnlCoordinates.Name = "pnlCoordinates";
            this.pnlCoordinates.Size = new System.Drawing.Size(368, 26);
            this.pnlCoordinates.TabIndex = 42;
            // 
            // tbxLatitude
            // 
            this.tbxLatitude.Location = new System.Drawing.Point(214, 3);
            this.tbxLatitude.Name = "tbxLatitude";
            this.tbxLatitude.Size = new System.Drawing.Size(90, 20);
            this.tbxLatitude.TabIndex = 7;
            // 
            // tbxLongitude
            // 
            this.tbxLongitude.Location = new System.Drawing.Point(25, 1);
            this.tbxLongitude.Name = "tbxLongitude";
            this.tbxLongitude.Size = new System.Drawing.Size(90, 20);
            this.tbxLongitude.TabIndex = 6;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Symbol", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label27.Location = new System.Drawing.Point(192, 2);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(20, 19);
            this.label27.TabIndex = 12;
            this.label27.Text = "j";
            // 
            // cbxLongitude
            // 
            this.cbxLongitude.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLongitude.FormattingEnabled = true;
            this.cbxLongitude.Items.AddRange(new object[] {
            "East",
            "West"});
            this.cbxLongitude.Location = new System.Drawing.Point(121, 1);
            this.cbxLongitude.Name = "cbxLongitude";
            this.cbxLongitude.Size = new System.Drawing.Size(56, 21);
            this.cbxLongitude.TabIndex = 8;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Symbol", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label30.Location = new System.Drawing.Point(5, 1);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(18, 19);
            this.label30.TabIndex = 10;
            this.label30.Text = "l";
            // 
            // cbxLatitude
            // 
            this.cbxLatitude.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLatitude.FormattingEnabled = true;
            this.cbxLatitude.Items.AddRange(new object[] {
            "North",
            "South"});
            this.cbxLatitude.Location = new System.Drawing.Point(310, 3);
            this.cbxLatitude.Name = "cbxLatitude";
            this.cbxLatitude.Size = new System.Drawing.Size(55, 21);
            this.cbxLatitude.TabIndex = 9;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(3, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(282, 13);
            this.label32.TabIndex = 41;
            this.label32.Text = "Observer Location (Used for Ephemeris and MPC Reports)";
            // 
            // ucLocation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnMPCHeader);
            this.Controls.Add(this.tbxMPCCode);
            this.Controls.Add(this.rbMPCCode);
            this.Controls.Add(this.rbCoordinates);
            this.Controls.Add(this.pnlCoordinates);
            this.Controls.Add(this.label32);
            this.Name = "ucLocation";
            this.Size = new System.Drawing.Size(455, 388);
            this.pnlCoordinates.ResumeLayout(false);
            this.pnlCoordinates.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Button btnMPCHeader;
		private System.Windows.Forms.TextBox tbxMPCCode;
		private System.Windows.Forms.RadioButton rbMPCCode;
		private System.Windows.Forms.RadioButton rbCoordinates;
		private System.Windows.Forms.Panel pnlCoordinates;
		private System.Windows.Forms.TextBox tbxLatitude;
		private System.Windows.Forms.TextBox tbxLongitude;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.ComboBox cbxLongitude;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.ComboBox cbxLatitude;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
	}
}
