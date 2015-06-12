namespace Tangra.VideoOperations.Spectroscopy
{
    partial class frmEnterWavelength
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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "A",
            "O2",
            "7593.7"}, -1);
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "B",
            "O2",
            "6867.2"}, -1);
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "C",
            "Hα",
            "6562.8"}, -1);
			System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "F",
            "Hβ",
            "4861.3"}, -1);
			this.nudManualWavelength = new System.Windows.Forms.NumericUpDown();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lvFraunhoferLines = new System.Windows.Forms.ListView();
			this.clmDesignation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.clmElement = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.clmWavelength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.rbEnterManually = new System.Windows.Forms.RadioButton();
			this.rbPickFromList = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.nudManualWavelength)).BeginInit();
			this.SuspendLayout();
			// 
			// nudManualWavelength
			// 
			this.nudManualWavelength.DecimalPlaces = 1;
			this.nudManualWavelength.Location = new System.Drawing.Point(30, 185);
			this.nudManualWavelength.Maximum = new decimal(new int[] {
            15000,
            0,
            0,
            0});
			this.nudManualWavelength.Name = "nudManualWavelength";
			this.nudManualWavelength.Size = new System.Drawing.Size(63, 20);
			this.nudManualWavelength.TabIndex = 0;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(128, 236);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(210, 236);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// lvFraunhoferLines
			// 
			this.lvFraunhoferLines.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmDesignation,
            this.clmElement,
            this.clmWavelength});
			this.lvFraunhoferLines.FullRowSelect = true;
			this.lvFraunhoferLines.HideSelection = false;
			this.lvFraunhoferLines.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4});
			this.lvFraunhoferLines.Location = new System.Drawing.Point(30, 35);
			this.lvFraunhoferLines.Name = "lvFraunhoferLines";
			this.lvFraunhoferLines.Size = new System.Drawing.Size(255, 112);
			this.lvFraunhoferLines.TabIndex = 3;
			this.lvFraunhoferLines.UseCompatibleStateImageBehavior = false;
			this.lvFraunhoferLines.View = System.Windows.Forms.View.Details;
			// 
			// clmDesignation
			// 
			this.clmDesignation.Text = "Designation";
			this.clmDesignation.Width = 80;
			// 
			// clmElement
			// 
			this.clmElement.Text = "Element";
			// 
			// clmWavelength
			// 
			this.clmWavelength.Text = "Wavelength";
			this.clmWavelength.Width = 100;
			// 
			// rbEnterManually
			// 
			this.rbEnterManually.AutoSize = true;
			this.rbEnterManually.Location = new System.Drawing.Point(13, 163);
			this.rbEnterManually.Name = "rbEnterManually";
			this.rbEnterManually.Size = new System.Drawing.Size(95, 17);
			this.rbEnterManually.TabIndex = 6;
			this.rbEnterManually.Text = "Enter Manually";
			this.rbEnterManually.UseVisualStyleBackColor = true;
			this.rbEnterManually.CheckedChanged += new System.EventHandler(this.UpdateCheckboxDerivedState);
			// 
			// rbPickFromList
			// 
			this.rbPickFromList.AutoSize = true;
			this.rbPickFromList.Checked = true;
			this.rbPickFromList.Location = new System.Drawing.Point(13, 12);
			this.rbPickFromList.Name = "rbPickFromList";
			this.rbPickFromList.Size = new System.Drawing.Size(91, 17);
			this.rbPickFromList.TabIndex = 7;
			this.rbPickFromList.TabStop = true;
			this.rbPickFromList.Text = "Pick From List";
			this.rbPickFromList.UseVisualStyleBackColor = true;
			this.rbPickFromList.CheckedChanged += new System.EventHandler(this.UpdateCheckboxDerivedState);
			// 
			// frmEnterWavelength
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(307, 268);
			this.Controls.Add(this.rbPickFromList);
			this.Controls.Add(this.rbEnterManually);
			this.Controls.Add(this.lvFraunhoferLines);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.nudManualWavelength);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmEnterWavelength";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Specify Wavelength";
			this.Load += new System.EventHandler(this.frmEnterWavelength_Load);
			((System.ComponentModel.ISupportInitialize)(this.nudManualWavelength)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudManualWavelength;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListView lvFraunhoferLines;
        private System.Windows.Forms.ColumnHeader clmDesignation;
        private System.Windows.Forms.ColumnHeader clmElement;
        private System.Windows.Forms.ColumnHeader clmWavelength;
        private System.Windows.Forms.RadioButton rbEnterManually;
        private System.Windows.Forms.RadioButton rbPickFromList;
    }
}