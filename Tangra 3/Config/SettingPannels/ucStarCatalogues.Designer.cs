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
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.cbxUseGaia = new System.Windows.Forms.CheckBox();
            this.pnlGaiaAIPNotes = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.tbxGaiaAPIToken = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.llRegisterAccount = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.llObtainToken = new System.Windows.Forms.LinkLabel();
            this.cbxCatalogPhotometryBand = new System.Windows.Forms.ComboBox();
            this.label58 = new System.Windows.Forms.Label();
            this.btnBrowseLocation = new System.Windows.Forms.Button();
            this.tbxCatalogueLocation = new System.Windows.Forms.TextBox();
            this.lblCatalogPath = new System.Windows.Forms.Label();
            this.cbxCatalogue = new System.Windows.Forms.ComboBox();
            this.label34 = new System.Windows.Forms.Label();
            this.pnlGaiaAIPNotes.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxUseGaia
            // 
            this.cbxUseGaia.AutoSize = true;
            this.cbxUseGaia.Location = new System.Drawing.Point(199, 22);
            this.cbxUseGaia.Name = "cbxUseGaia";
            this.cbxUseGaia.Size = new System.Drawing.Size(182, 17);
            this.cbxUseGaia.TabIndex = 50;
            this.cbxUseGaia.Text = "Use GaiaEDR3 positions (Online)";
            this.cbxUseGaia.UseVisualStyleBackColor = true;
            this.cbxUseGaia.CheckedChanged += new System.EventHandler(this.cbxUseGaia_CheckedChanged);
            // 
            // pnlGaiaAIPNotes
            // 
            this.pnlGaiaAIPNotes.Controls.Add(this.label6);
            this.pnlGaiaAIPNotes.Controls.Add(this.tbxGaiaAPIToken);
            this.pnlGaiaAIPNotes.Controls.Add(this.label5);
            this.pnlGaiaAIPNotes.Controls.Add(this.label4);
            this.pnlGaiaAIPNotes.Controls.Add(this.label3);
            this.pnlGaiaAIPNotes.Controls.Add(this.label2);
            this.pnlGaiaAIPNotes.Controls.Add(this.llRegisterAccount);
            this.pnlGaiaAIPNotes.Controls.Add(this.label1);
            this.pnlGaiaAIPNotes.Controls.Add(this.llObtainToken);
            this.pnlGaiaAIPNotes.Location = new System.Drawing.Point(-1, 149);
            this.pnlGaiaAIPNotes.Name = "pnlGaiaAIPNotes";
            this.pnlGaiaAIPNotes.Size = new System.Drawing.Size(381, 192);
            this.pnlGaiaAIPNotes.TabIndex = 49;
            this.pnlGaiaAIPNotes.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 10);
            this.label6.Name = "label6";
            this.label6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 54;
            this.label6.Text = "API Token";
            // 
            // tbxGaiaAPIToken
            // 
            this.tbxGaiaAPIToken.Location = new System.Drawing.Point(7, 26);
            this.tbxGaiaAPIToken.Name = "tbxGaiaAPIToken";
            this.tbxGaiaAPIToken.Size = new System.Drawing.Size(324, 20);
            this.tbxGaiaAPIToken.TabIndex = 51;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(149, 13);
            this.label5.TabIndex = 53;
            this.label5.Text = "4) Enter the API Token above";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(269, 13);
            this.label4.TabIndex = 52;
            this.label4.Text = "2) Complete registration by validating your email address";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(145, 13);
            this.label3.TabIndex = 51;
            this.label3.Text = "1) Register a free account at ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(4, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(254, 13);
            this.label2.TabIndex = 50;
            this.label2.Text = "How to obtain an API Token for Gaia EDR3";
            // 
            // llRegisterAccount
            // 
            this.llRegisterAccount.AutoSize = true;
            this.llRegisterAccount.Location = new System.Drawing.Point(146, 89);
            this.llRegisterAccount.Name = "llRegisterAccount";
            this.llRegisterAccount.Size = new System.Drawing.Size(185, 13);
            this.llRegisterAccount.TabIndex = 2;
            this.llRegisterAccount.TabStop = true;
            this.llRegisterAccount.Text = "https://gaia.aip.de/accounts/signup/";
            this.llRegisterAccount.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OpenLinkLabelLink);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 140);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "3) Copy your API token from";
            // 
            // llObtainToken
            // 
            this.llObtainToken.AutoSize = true;
            this.llObtainToken.Location = new System.Drawing.Point(143, 139);
            this.llObtainToken.Name = "llObtainToken";
            this.llObtainToken.Size = new System.Drawing.Size(181, 13);
            this.llObtainToken.TabIndex = 0;
            this.llObtainToken.TabStop = true;
            this.llObtainToken.Text = "https://gaia.aip.de/accounts/token/";
            this.llObtainToken.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OpenLinkLabelLink);
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
            // lblCatalogPath
            // 
            this.lblCatalogPath.AutoSize = true;
            this.lblCatalogPath.Location = new System.Drawing.Point(2, 54);
            this.lblCatalogPath.Name = "lblCatalogPath";
            this.lblCatalogPath.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblCatalogPath.Size = new System.Drawing.Size(29, 13);
            this.lblCatalogPath.TabIndex = 38;
            this.lblCatalogPath.Text = "Path";
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
            this.Controls.Add(this.cbxUseGaia);
            this.Controls.Add(this.pnlGaiaAIPNotes);
            this.Controls.Add(this.cbxCatalogPhotometryBand);
            this.Controls.Add(this.label58);
            this.Controls.Add(this.btnBrowseLocation);
            this.Controls.Add(this.tbxCatalogueLocation);
            this.Controls.Add(this.lblCatalogPath);
            this.Controls.Add(this.cbxCatalogue);
            this.Controls.Add(this.label34);
            this.Name = "ucStarCatalogues";
            this.Size = new System.Drawing.Size(455, 388);
            this.Load += new System.EventHandler(this.ucStarCatalogues_Load);
            this.pnlGaiaAIPNotes.ResumeLayout(false);
            this.pnlGaiaAIPNotes.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cbxCatalogPhotometryBand;
        private System.Windows.Forms.Label label58;
		private System.Windows.Forms.Button btnBrowseLocation;
		protected internal System.Windows.Forms.TextBox tbxCatalogueLocation;
		private System.Windows.Forms.Label lblCatalogPath;
		protected internal System.Windows.Forms.ComboBox cbxCatalogue;
		private System.Windows.Forms.Label label34;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Panel pnlGaiaAIPNotes;
        private System.Windows.Forms.LinkLabel llRegisterAccount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel llObtainToken;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        protected internal System.Windows.Forms.TextBox tbxGaiaAPIToken;
        private System.Windows.Forms.CheckBox cbxUseGaia;
	}
}
