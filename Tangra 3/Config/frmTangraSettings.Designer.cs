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
			System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Pre-Processing");
			System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Photometry");
			System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Tracking");
			System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Light Curves");
			System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Light Curve Viewer");
			System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Customize", new System.Windows.Forms.TreeNode[] {
            treeNode9,
            treeNode10});
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTangraSettings));
			this.tvSettings = new System.Windows.Forms.TreeView();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.pnlPropertyPage = new System.Windows.Forms.Panel();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnResetDefaults = new System.Windows.Forms.Button();
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
			treeNode6.Name = "ndPreProcessing";
			treeNode6.Tag = "11";
			treeNode6.Text = "Pre-Processing";
			treeNode7.Name = "ndPhotometry";
			treeNode7.Tag = "4";
			treeNode7.Text = "Photometry";
			treeNode8.Name = "ndTracking";
			treeNode8.Tag = "6";
			treeNode8.Text = "Tracking";
			treeNode9.Name = "ndLightCurves";
			treeNode9.Tag = "7";
			treeNode9.Text = "Light Curves";
			treeNode10.Name = "ndCurveViewer";
			treeNode10.Tag = "9";
			treeNode10.Text = "Light Curve Viewer";
			treeNode11.Name = "ndCustomize";
			treeNode11.Tag = "7";
			treeNode11.Text = "Customize";
			this.tvSettings.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode11});
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
			this.btnResetDefaults.Location = new System.Drawing.Point(185, 373);
			this.btnResetDefaults.Name = "btnResetDefaults";
			this.btnResetDefaults.Size = new System.Drawing.Size(106, 23);
			this.btnResetDefaults.TabIndex = 14;
			this.btnResetDefaults.Text = "Reset Defaults";
			this.btnResetDefaults.Click += new System.EventHandler(this.btnResetDefaults_Click);
			// 
			// frmTangraSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(665, 404);
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
	}
}