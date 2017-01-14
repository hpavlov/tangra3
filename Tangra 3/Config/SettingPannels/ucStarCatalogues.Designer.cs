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
            this.cbxCatalogPhotometryBand = new System.Windows.Forms.ComboBox();
            this.label58 = new System.Windows.Forms.Label();
            this.btnBrowseLocation = new System.Windows.Forms.Button();
            this.tbxCatalogueLocation = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.cbxCatalogue = new System.Windows.Forms.ComboBox();
            this.label34 = new System.Windows.Forms.Label();
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
            this.Controls.Add(this.btnBrowseLocation);
            this.Controls.Add(this.tbxCatalogueLocation);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.cbxCatalogue);
            this.Controls.Add(this.label34);
            this.Name = "ucStarCatalogues";
            this.Size = new System.Drawing.Size(455, 388);
            this.Load += new System.EventHandler(this.ucStarCatalogues_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cbxCatalogPhotometryBand;
        private System.Windows.Forms.Label label58;
		private System.Windows.Forms.Button btnBrowseLocation;
		protected internal System.Windows.Forms.TextBox tbxCatalogueLocation;
		private System.Windows.Forms.Label label33;
		protected internal System.Windows.Forms.ComboBox cbxCatalogue;
		private System.Windows.Forms.Label label34;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
	}
}
