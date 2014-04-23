using System.Windows.Forms;

namespace Tangra.Config.SettingPannels
{
    partial class ucAddins
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
			this.gbxLoadedAddins = new System.Windows.Forms.GroupBox();
			this.btnConfigure = new System.Windows.Forms.Button();
			this.pnlAddinInfo = new System.Windows.Forms.Panel();
			this.lblUrl = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblDescription = new System.Windows.Forms.Label();
			this.lblDisplayName = new System.Windows.Forms.Label();
			this.lbxLoadedAddins = new System.Windows.Forms.ListBox();
			this.tbxAddinsDirectory = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btnReloadAddins = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.cbxIsolationLevel = new System.Windows.Forms.ComboBox();
			this.btnUnloadAddins = new System.Windows.Forms.Button();
			this.btnNavigateTo = new System.Windows.Forms.Button();
			this.cbxOwEventTimesExport = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.linkLblOW = new System.Windows.Forms.LinkLabel();
			this.pnlOccultWatcherSettings = new System.Windows.Forms.Panel();
			this.lblOWIntegrationError = new System.Windows.Forms.Label();
			this.gbxLoadedAddins.SuspendLayout();
			this.pnlAddinInfo.SuspendLayout();
			this.pnlOccultWatcherSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbxLoadedAddins
			// 
			this.gbxLoadedAddins.Controls.Add(this.btnConfigure);
			this.gbxLoadedAddins.Controls.Add(this.pnlAddinInfo);
			this.gbxLoadedAddins.Controls.Add(this.lbxLoadedAddins);
			this.gbxLoadedAddins.Location = new System.Drawing.Point(3, 80);
			this.gbxLoadedAddins.Name = "gbxLoadedAddins";
			this.gbxLoadedAddins.Size = new System.Drawing.Size(463, 217);
			this.gbxLoadedAddins.TabIndex = 0;
			this.gbxLoadedAddins.TabStop = false;
			this.gbxLoadedAddins.Text = "Loaded Addins";
			// 
			// btnConfigure
			// 
			this.btnConfigure.Enabled = false;
			this.btnConfigure.Location = new System.Drawing.Point(16, 179);
			this.btnConfigure.Name = "btnConfigure";
			this.btnConfigure.Size = new System.Drawing.Size(96, 23);
			this.btnConfigure.TabIndex = 2;
			this.btnConfigure.Text = "Configure";
			this.btnConfigure.UseVisualStyleBackColor = true;
			this.btnConfigure.Click += new System.EventHandler(this.btnConfigure_Click);
			// 
			// pnlAddinInfo
			// 
			this.pnlAddinInfo.Controls.Add(this.lblUrl);
			this.pnlAddinInfo.Controls.Add(this.label1);
			this.pnlAddinInfo.Controls.Add(this.lblVersion);
			this.pnlAddinInfo.Controls.Add(this.lblDescription);
			this.pnlAddinInfo.Controls.Add(this.lblDisplayName);
			this.pnlAddinInfo.Location = new System.Drawing.Point(204, 29);
			this.pnlAddinInfo.Name = "pnlAddinInfo";
			this.pnlAddinInfo.Size = new System.Drawing.Size(255, 173);
			this.pnlAddinInfo.TabIndex = 1;
			this.pnlAddinInfo.Visible = false;
			// 
			// lblUrl
			// 
			this.lblUrl.AutoSize = true;
			this.lblUrl.Location = new System.Drawing.Point(3, 47);
			this.lblUrl.Name = "lblUrl";
			this.lblUrl.Size = new System.Drawing.Size(29, 13);
			this.lblUrl.TabIndex = 4;
			this.lblUrl.TabStop = true;
			this.lblUrl.Text = "URL";
			this.lblUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblUrl_LinkClicked);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(3, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Version:";
			// 
			// lblVersion
			// 
			this.lblVersion.AutoSize = true;
			this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblVersion.ForeColor = System.Drawing.Color.Navy;
			this.lblVersion.Location = new System.Drawing.Point(53, 23);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(22, 13);
			this.lblVersion.TabIndex = 2;
			this.lblVersion.Text = "1.0";
			// 
			// lblDescription
			// 
			this.lblDescription.Location = new System.Drawing.Point(3, 73);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(245, 89);
			this.lblDescription.TabIndex = 1;
			this.lblDescription.Text = "Description";
			// 
			// lblDisplayName
			// 
			this.lblDisplayName.AutoSize = true;
			this.lblDisplayName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDisplayName.Location = new System.Drawing.Point(3, 0);
			this.lblDisplayName.Name = "lblDisplayName";
			this.lblDisplayName.Size = new System.Drawing.Size(68, 13);
			this.lblDisplayName.TabIndex = 0;
			this.lblDisplayName.Text = "New Addin";
			// 
			// lbxLoadedAddins
			// 
			this.lbxLoadedAddins.FormattingEnabled = true;
			this.lbxLoadedAddins.Location = new System.Drawing.Point(16, 28);
			this.lbxLoadedAddins.Name = "lbxLoadedAddins";
			this.lbxLoadedAddins.Size = new System.Drawing.Size(182, 147);
			this.lbxLoadedAddins.TabIndex = 0;
			this.lbxLoadedAddins.SelectedIndexChanged += new System.EventHandler(this.lbxLoadedAddins_SelectedIndexChanged);
			// 
			// tbxAddinsDirectory
			// 
			this.tbxAddinsDirectory.Location = new System.Drawing.Point(6, 19);
			this.tbxAddinsDirectory.Name = "tbxAddinsDirectory";
			this.tbxAddinsDirectory.ReadOnly = true;
			this.tbxAddinsDirectory.Size = new System.Drawing.Size(426, 20);
			this.tbxAddinsDirectory.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(87, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Addins Directory:";
			// 
			// btnReloadAddins
			// 
			this.btnReloadAddins.Location = new System.Drawing.Point(213, 46);
			this.btnReloadAddins.Name = "btnReloadAddins";
			this.btnReloadAddins.Size = new System.Drawing.Size(96, 23);
			this.btnReloadAddins.TabIndex = 3;
			this.btnReloadAddins.Text = "Reload Addins";
			this.btnReloadAddins.UseVisualStyleBackColor = true;
			this.btnReloadAddins.Click += new System.EventHandler(this.btnReloadAddins_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 51);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(84, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Addins Isolation:";
			// 
			// cbxIsolationLevel
			// 
			this.cbxIsolationLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxIsolationLevel.FormattingEnabled = true;
			this.cbxIsolationLevel.Items.AddRange(new object[] {
            "AppDomain",
            "None"});
			this.cbxIsolationLevel.Location = new System.Drawing.Point(93, 48);
			this.cbxIsolationLevel.Name = "cbxIsolationLevel";
			this.cbxIsolationLevel.Size = new System.Drawing.Size(108, 21);
			this.cbxIsolationLevel.TabIndex = 5;
			// 
			// btnUnloadAddins
			// 
			this.btnUnloadAddins.Location = new System.Drawing.Point(321, 46);
			this.btnUnloadAddins.Name = "btnUnloadAddins";
			this.btnUnloadAddins.Size = new System.Drawing.Size(96, 23);
			this.btnUnloadAddins.TabIndex = 6;
			this.btnUnloadAddins.Text = "Unload Addins";
			this.btnUnloadAddins.UseVisualStyleBackColor = true;
			this.btnUnloadAddins.Click += new System.EventHandler(this.btnUnloadAddins_Click);
			// 
			// btnNavigateTo
			// 
			this.btnNavigateTo.Location = new System.Drawing.Point(436, 17);
			this.btnNavigateTo.Name = "btnNavigateTo";
			this.btnNavigateTo.Size = new System.Drawing.Size(30, 23);
			this.btnNavigateTo.TabIndex = 7;
			this.btnNavigateTo.Text = "Go";
			this.btnNavigateTo.UseVisualStyleBackColor = true;
			this.btnNavigateTo.Click += new System.EventHandler(this.btnNavigateTo_Click);
			// 
			// cbxOwEventTimesExport
			// 
			this.cbxOwEventTimesExport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxOwEventTimesExport.FormattingEnabled = true;
			this.cbxOwEventTimesExport.Items.AddRange(new object[] {
            "Always Export Event Times",
            "Ask before Exporting Event Times",
            "Don\'t Export Event Times"});
			this.cbxOwEventTimesExport.Location = new System.Drawing.Point(193, 4);
			this.cbxOwEventTimesExport.Name = "cbxOwEventTimesExport";
			this.cbxOwEventTimesExport.Size = new System.Drawing.Size(202, 21);
			this.cbxOwEventTimesExport.TabIndex = 9;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(0, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(187, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Occult Watcher Reporting Integration:";
			// 
			// linkLblOW
			// 
			this.linkLblOW.AutoSize = true;
			this.linkLblOW.Location = new System.Drawing.Point(0, 7);
			this.linkLblOW.Name = "linkLblOW";
			this.linkLblOW.Size = new System.Drawing.Size(82, 13);
			this.linkLblOW.TabIndex = 10;
			this.linkLblOW.TabStop = true;
			this.linkLblOW.Text = "Occult Watcher";
			this.linkLblOW.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLblOW_LinkClicked);
			// 
			// pnlOccultWatcherSettings
			// 
			this.pnlOccultWatcherSettings.Controls.Add(this.lblOWIntegrationError);
			this.pnlOccultWatcherSettings.Controls.Add(this.linkLblOW);
			this.pnlOccultWatcherSettings.Controls.Add(this.label4);
			this.pnlOccultWatcherSettings.Controls.Add(this.cbxOwEventTimesExport);
			this.pnlOccultWatcherSettings.Location = new System.Drawing.Point(1, 303);
			this.pnlOccultWatcherSettings.Name = "pnlOccultWatcherSettings";
			this.pnlOccultWatcherSettings.Size = new System.Drawing.Size(498, 29);
			this.pnlOccultWatcherSettings.TabIndex = 11;
			// 
			// lblOWIntegrationError
			// 
			this.lblOWIntegrationError.AutoSize = true;
			this.lblOWIntegrationError.ForeColor = System.Drawing.Color.Red;
			this.lblOWIntegrationError.Location = new System.Drawing.Point(190, 8);
			this.lblOWIntegrationError.Name = "lblOWIntegrationError";
			this.lblOWIntegrationError.Size = new System.Drawing.Size(278, 13);
			this.lblOWIntegrationError.TabIndex = 11;
			this.lblOWIntegrationError.Text = "OW IOTA Reporting add-in ver 1.8 or later is not installed.";
			// 
			// ucAddins
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.pnlOccultWatcherSettings);
			this.Controls.Add(this.btnNavigateTo);
			this.Controls.Add(this.btnUnloadAddins);
			this.Controls.Add(this.cbxIsolationLevel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnReloadAddins);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbxAddinsDirectory);
			this.Controls.Add(this.gbxLoadedAddins);
			this.Name = "ucAddins";
			this.Size = new System.Drawing.Size(502, 332);
			this.gbxLoadedAddins.ResumeLayout(false);
			this.pnlAddinInfo.ResumeLayout(false);
			this.pnlAddinInfo.PerformLayout();
			this.pnlOccultWatcherSettings.ResumeLayout(false);
			this.pnlOccultWatcherSettings.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

        private GroupBox gbxLoadedAddins;
        private ListBox lbxLoadedAddins;
        private Panel pnlAddinInfo;
        private Label lblVersion;
        private Label lblDescription;
        private Label lblDisplayName;
        private Label label1;
        private LinkLabel lblUrl;
        private Button btnConfigure;
        private TextBox tbxAddinsDirectory;
        private Label label2;
        private Button btnReloadAddins;
        private Label label3;
        private ComboBox cbxIsolationLevel;
        private Button btnUnloadAddins;
		private Button btnNavigateTo;
		private ComboBox cbxOwEventTimesExport;
		private Label label4;
		private LinkLabel linkLblOW;
		private Panel pnlOccultWatcherSettings;
		private Label lblOWIntegrationError;


    }
}
