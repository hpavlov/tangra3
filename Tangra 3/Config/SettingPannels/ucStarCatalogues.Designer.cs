namespace Tangra.Config.SettingPannels
{
	partial class ucStarCatalogues
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
			this.cbxCatalogPhotometryBand = new System.Windows.Forms.ComboBox();
			this.label58 = new System.Windows.Forms.Label();
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
			this.btnBrowseLocation = new System.Windows.Forms.Button();
			this.tbxCatalogueLocation = new System.Windows.Forms.TextBox();
			this.label33 = new System.Windows.Forms.Label();
			this.cbxCatalogue = new System.Windows.Forms.ComboBox();
			this.label34 = new System.Windows.Forms.Label();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.pnlCoordinates.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbxCatalogPhotometryBand
			// 
			this.cbxCatalogPhotometryBand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCatalogPhotometryBand.FormattingEnabled = true;
			this.cbxCatalogPhotometryBand.Items.AddRange(new object[] {
            "Model Fit Magnitude (fMag)",
            "Johnson V - Computed from fMag",
            "Cousins R - Computed from fMag"});
			this.cbxCatalogPhotometryBand.Location = new System.Drawing.Point(6, 117);
			this.cbxCatalogPhotometryBand.Name = "cbxCatalogPhotometryBand";
			this.cbxCatalogPhotometryBand.Size = new System.Drawing.Size(247, 21);
			this.cbxCatalogPhotometryBand.TabIndex = 48;
			// 
			// label58
			// 
			this.label58.AutoSize = true;
			this.label58.Location = new System.Drawing.Point(3, 101);
			this.label58.Name = "label58";
			this.label58.Size = new System.Drawing.Size(156, 13);
			this.label58.TabIndex = 47;
			this.label58.Text = "Magnitude Band for Photometry";
			// 
			// btnMPCHeader
			// 
			this.btnMPCHeader.Location = new System.Drawing.Point(223, 229);
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
			this.tbxMPCCode.Location = new System.Drawing.Point(162, 230);
			this.tbxMPCCode.Name = "tbxMPCCode";
			this.tbxMPCCode.Size = new System.Drawing.Size(56, 20);
			this.tbxMPCCode.TabIndex = 45;
			// 
			// rbMPCCode
			// 
			this.rbMPCCode.AutoSize = true;
			this.rbMPCCode.Location = new System.Drawing.Point(21, 231);
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
			this.rbCoordinates.Location = new System.Drawing.Point(21, 198);
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
			this.pnlCoordinates.Location = new System.Drawing.Point(41, 189);
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
			this.label32.Location = new System.Drawing.Point(3, 163);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(282, 13);
			this.label32.TabIndex = 41;
			this.label32.Text = "Observer Location (Used for Ephemeris and MPC Reports)";
			// 
			// btnBrowseLocation
			// 
			this.btnBrowseLocation.Location = new System.Drawing.Point(393, 66);
			this.btnBrowseLocation.Name = "btnBrowseLocation";
			this.btnBrowseLocation.Size = new System.Drawing.Size(30, 23);
			this.btnBrowseLocation.TabIndex = 40;
			this.btnBrowseLocation.Text = "...";
			this.btnBrowseLocation.UseVisualStyleBackColor = true;
			this.btnBrowseLocation.Click += new System.EventHandler(this.btnBrowseLocation_Click);
			// 
			// tbxCatalogueLocation
			// 
			this.tbxCatalogueLocation.Location = new System.Drawing.Point(5, 68);
			this.tbxCatalogueLocation.Name = "tbxCatalogueLocation";
			this.tbxCatalogueLocation.Size = new System.Drawing.Size(382, 20);
			this.tbxCatalogueLocation.TabIndex = 39;
			// 
			// label33
			// 
			this.label33.AutoSize = true;
			this.label33.Location = new System.Drawing.Point(2, 54);
			this.label33.Name = "label33";
			this.label33.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label33.Size = new System.Drawing.Size(29, 13);
			this.label33.TabIndex = 38;
			this.label33.Text = "Path";
			// 
			// cbxCatalogue
			// 
			this.cbxCatalogue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCatalogue.FormattingEnabled = true;
			this.cbxCatalogue.Items.AddRange(new object[] {
            "UCAC2 + BBS (Local Disk)",
            "NOMAD (Local Disk)",
            "UCAC3 (Local Disk)",
            "PPMXL (Local Disk)",
            "UCAC4 (Local Disk)"});
			this.cbxCatalogue.Location = new System.Drawing.Point(6, 19);
			this.cbxCatalogue.Name = "cbxCatalogue";
			this.cbxCatalogue.Size = new System.Drawing.Size(176, 21);
			this.cbxCatalogue.TabIndex = 37;
			this.cbxCatalogue.SelectedIndexChanged += new System.EventHandler(this.cbxCatalogue_SelectedIndexChanged);
			// 
			// label34
			// 
			this.label34.AutoSize = true;
			this.label34.Location = new System.Drawing.Point(3, 3);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(77, 13);
			this.label34.TabIndex = 36;
			this.label34.Text = "Star Catalogue";
			// 
			// ucStarCatalogues
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cbxCatalogPhotometryBand);
			this.Controls.Add(this.label58);
			this.Controls.Add(this.btnMPCHeader);
			this.Controls.Add(this.tbxMPCCode);
			this.Controls.Add(this.rbMPCCode);
			this.Controls.Add(this.rbCoordinates);
			this.Controls.Add(this.pnlCoordinates);
			this.Controls.Add(this.label32);
			this.Controls.Add(this.btnBrowseLocation);
			this.Controls.Add(this.tbxCatalogueLocation);
			this.Controls.Add(this.label33);
			this.Controls.Add(this.cbxCatalogue);
			this.Controls.Add(this.label34);
			this.Name = "ucStarCatalogues";
			this.Size = new System.Drawing.Size(455, 388);
			this.Load += new System.EventHandler(this.ucStarCatalogues_Load);
			this.pnlCoordinates.ResumeLayout(false);
			this.pnlCoordinates.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cbxCatalogPhotometryBand;
		private System.Windows.Forms.Label label58;
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
		private System.Windows.Forms.Button btnBrowseLocation;
		protected internal System.Windows.Forms.TextBox tbxCatalogueLocation;
		private System.Windows.Forms.Label label33;
		protected internal System.Windows.Forms.ComboBox cbxCatalogue;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
	}
}
