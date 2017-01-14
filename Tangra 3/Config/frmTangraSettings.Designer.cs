namespace Tangra.Config
{
	partial class frmTangraSettings
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("General");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Analogue Video");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Astro Analogue Video");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Astro Digital Video");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Video", new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode3,
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Photometry");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Star Catalogues");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Location");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Stellar Objects");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Astrometry", new System.Windows.Forms.TreeNode[] {
            treeNode7,
            treeNode8,
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Spectroscopy");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Tracking");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Compatibility");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Addins");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Light Curves");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Light Curve Viewer");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Astrometry Colours");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Spectra Viewer");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Customize", new System.Windows.Forms.TreeNode[] {
            treeNode15,
            treeNode16,
            treeNode17,
            treeNode18});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTangraSettings));
            this.tvSettings = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlPropertyPage = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnResetDefaults = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnImport = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // tvSettings
            // 
            this.tvSettings.Location = new System.Drawing.Point(3, 5);
            this.tvSettings.Name = "tvSettings";
            treeNode1.Name = "ndGeneral";
            treeNode1.Tag = "0";
            treeNode1.Text = "General";
            treeNode2.Name = "ndAnalogueVideo";
            treeNode2.Tag = "2";
            treeNode2.Text = "Analogue Video";
            treeNode3.Name = "ndAAV";
            treeNode3.Tag = "10";
            treeNode3.Text = "Astro Analogue Video";
            treeNode4.Name = "ndADV";
            treeNode4.Tag = "3";
            treeNode4.Text = "Astro Digital Video";
            treeNode5.Name = "ndVideo";
            treeNode5.Tag = "1";
            treeNode5.Text = "Video";
            treeNode6.Name = "ndPhotometry";
            treeNode6.Tag = "4";
            treeNode6.Text = "Photometry";
            treeNode7.Name = "ndCatalogues";
            treeNode7.Tag = "14";
            treeNode7.Text = "Star Catalogues";
            treeNode8.Name = "ndLocation";
            treeNode8.Tag = "19";
            treeNode8.Text = "Location";
            treeNode9.Name = "ndStellar";
            treeNode9.Tag = "18";
            treeNode9.Text = "Stellar Objects";
            treeNode10.Name = "ndAstrometry";
            treeNode10.Tag = "13";
            treeNode10.Text = "Astrometry";
            treeNode11.Name = "ndSpectroscopy";
            treeNode11.Tag = "16";
            treeNode11.Text = "Spectroscopy";
            treeNode12.Name = "ndTracking";
            treeNode12.Tag = "6";
            treeNode12.Text = "Tracking";
            treeNode13.Name = "ndCompatibility";
            treeNode13.Tag = "8";
            treeNode13.Text = "Compatibility";
            treeNode14.Name = "ndAddins";
            treeNode14.Tag = "12";
            treeNode14.Text = "Addins";
            treeNode15.Name = "ndLightCurves";
            treeNode15.Tag = "7";
            treeNode15.Text = "Light Curves";
            treeNode16.Name = "ndCurveViewer";
            treeNode16.Tag = "9";
            treeNode16.Text = "Light Curve Viewer";
            treeNode17.Name = "ndAstrometryColours";
            treeNode17.Tag = "15";
            treeNode17.Text = "Astrometry Colours";
            treeNode18.Name = "ndSpectraViewer";
            treeNode18.Tag = "17";
            treeNode18.Text = "Spectra Viewer";
            treeNode19.Name = "ndCustomize";
            treeNode19.Tag = "7";
            treeNode19.Text = "Customize";
            this.tvSettings.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode5,
            treeNode6,
            treeNode10,
            treeNode11,
            treeNode12,
            treeNode13,
            treeNode14,
            treeNode19});
            this.tvSettings.Size = new System.Drawing.Size(176, 359);
            this.tvSettings.TabIndex = 7;
            this.tvSettings.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvSettings_BeforeSelect);
            this.tvSettings.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSettings_AfterSelect);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(185, 361);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(471, 3);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // pnlPropertyPage
            // 
            this.pnlPropertyPage.Location = new System.Drawing.Point(185, 6);
            this.pnlPropertyPage.Name = "pnlPropertyPage";
            this.pnlPropertyPage.Size = new System.Drawing.Size(471, 349);
            this.pnlPropertyPage.TabIndex = 11;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(500, 373);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(581, 373);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnResetDefaults
            // 
            this.btnResetDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetDefaults.Location = new System.Drawing.Point(4, 373);
            this.btnResetDefaults.Name = "btnResetDefaults";
            this.btnResetDefaults.Size = new System.Drawing.Size(106, 23);
            this.btnResetDefaults.TabIndex = 14;
            this.btnResetDefaults.Text = "Reset Defaults";
            this.btnResetDefaults.Click += new System.EventHandler(this.btnResetDefaults_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(116, 373);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 15;
            this.btnExport.Text = "Export";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xml";
            this.saveFileDialog1.FileName = "TangraSettings.xml";
            this.saveFileDialog1.Filter = "Xml files|*.xml|All files|*.*";
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Location = new System.Drawing.Point(197, 373);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 16;
            this.btnImport.Text = "Import";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "xml";
            this.openFileDialog1.FileName = "TangraSettings.xml";
            this.openFileDialog1.Filter = "Xml files|*.xml|All files|*.*";
            // 
            // frmTangraSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 404);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnResetDefaults);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pnlPropertyPage);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tvSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTangraSettings";
            this.Text = "Tangra Settings";
            this.Load += new System.EventHandler(this.frmTangraSettings2_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.TreeView tvSettings;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Panel pnlPropertyPage;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnResetDefaults;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
	}
}