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
            "7605.0"}, -1);
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "B",
            "O2",
            "6869.0"}, -1);
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "C",
            "Hα",
            "6562.8"}, -1);
			System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "F",
            "Hβ",
            "4861.3"}, -1);
			System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "G\'",
            "Hγ",
            "4340.5"}, -1);
			System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
            "h",
            "Hδ",
            "4101.75"}, -1);
			this.nudManualWavelength = new System.Windows.Forms.NumericUpDown();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lvFraunhoferLines = new System.Windows.Forms.ListView();
			this.clmDesignation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.clmElement = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.clmWavelength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.rbEnterManually = new System.Windows.Forms.RadioButton();
			this.rbPickFromList = new System.Windows.Forms.RadioButton();
			this.cbxSinglePoint = new System.Windows.Forms.CheckBox();
			this.nudDispersion = new System.Windows.Forms.NumericUpDown();
			this.pnlOnePointCalibration = new System.Windows.Forms.Panel();
			this.btnReset = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.cbxCalibrate = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.nudPolyOrder = new System.Windows.Forms.NumericUpDown();
			this.pnlCalibration = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.nudManualWavelength)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDispersion)).BeginInit();
			this.pnlOnePointCalibration.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudPolyOrder)).BeginInit();
			this.pnlCalibration.SuspendLayout();
			this.SuspendLayout();
			// 
			// nudManualWavelength
			// 
			this.nudManualWavelength.DecimalPlaces = 1;
			this.nudManualWavelength.Location = new System.Drawing.Point(30, 200);
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
			this.btnOK.Location = new System.Drawing.Point(166, 314);
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
			this.btnCancel.Location = new System.Drawing.Point(248, 314);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            listViewItem4,
            listViewItem5,
            listViewItem6});
			this.lvFraunhoferLines.Location = new System.Drawing.Point(30, 35);
			this.lvFraunhoferLines.Name = "lvFraunhoferLines";
			this.lvFraunhoferLines.Size = new System.Drawing.Size(266, 124);
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
			this.rbEnterManually.Location = new System.Drawing.Point(13, 178);
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
			// cbxSinglePoint
			// 
			this.cbxSinglePoint.AutoSize = true;
			this.cbxSinglePoint.Location = new System.Drawing.Point(9, 6);
			this.cbxSinglePoint.Name = "cbxSinglePoint";
			this.cbxSinglePoint.Size = new System.Drawing.Size(183, 17);
			this.cbxSinglePoint.TabIndex = 8;
			this.cbxSinglePoint.Text = "One Point Calibration, Dispersion:";
			this.cbxSinglePoint.UseVisualStyleBackColor = true;
			this.cbxSinglePoint.CheckedChanged += new System.EventHandler(this.cbxSinglePoint_CheckedChanged);
			// 
			// nudDispersion
			// 
			this.nudDispersion.DecimalPlaces = 2;
			this.nudDispersion.Enabled = false;
			this.nudDispersion.Location = new System.Drawing.Point(194, 5);
			this.nudDispersion.Name = "nudDispersion";
			this.nudDispersion.Size = new System.Drawing.Size(57, 20);
			this.nudDispersion.TabIndex = 9;
			// 
			// pnlOnePointCalibration
			// 
			this.pnlOnePointCalibration.Controls.Add(this.cbxSinglePoint);
			this.pnlOnePointCalibration.Controls.Add(this.nudDispersion);
			this.pnlOnePointCalibration.Location = new System.Drawing.Point(4, 271);
			this.pnlOnePointCalibration.Name = "pnlOnePointCalibration";
			this.pnlOnePointCalibration.Size = new System.Drawing.Size(261, 34);
			this.pnlOnePointCalibration.TabIndex = 10;
			// 
			// btnReset
			// 
			this.btnReset.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnReset.Location = new System.Drawing.Point(12, 314);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(75, 23);
			this.btnReset.TabIndex = 11;
			this.btnReset.Text = "Reset";
			this.btnReset.UseVisualStyleBackColor = true;
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.Black;
			this.panel1.Location = new System.Drawing.Point(13, 308);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(310, 1);
			this.panel1.TabIndex = 12;
			// 
			// cbxCalibrate
			// 
			this.cbxCalibrate.AutoSize = true;
			this.cbxCalibrate.Location = new System.Drawing.Point(166, 7);
			this.cbxCalibrate.Name = "cbxCalibrate";
			this.cbxCalibrate.Size = new System.Drawing.Size(67, 17);
			this.cbxCalibrate.TabIndex = 13;
			this.cbxCalibrate.Text = "Calibrate";
			this.cbxCalibrate.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 13);
			this.label1.TabIndex = 15;
			this.label1.Text = "Polynomial Order";
			// 
			// nudPolyOrder
			// 
			this.nudPolyOrder.Location = new System.Drawing.Point(95, 6);
			this.nudPolyOrder.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudPolyOrder.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudPolyOrder.Name = "nudPolyOrder";
			this.nudPolyOrder.Size = new System.Drawing.Size(44, 20);
			this.nudPolyOrder.TabIndex = 16;
			this.nudPolyOrder.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudPolyOrder.ValueChanged += new System.EventHandler(this.nudPolyOrder_ValueChanged);
			// 
			// pnlCalibration
			// 
			this.pnlCalibration.Controls.Add(this.label1);
			this.pnlCalibration.Controls.Add(this.nudPolyOrder);
			this.pnlCalibration.Controls.Add(this.cbxCalibrate);
			this.pnlCalibration.Location = new System.Drawing.Point(4, 231);
			this.pnlCalibration.Name = "pnlCalibration";
			this.pnlCalibration.Size = new System.Drawing.Size(261, 31);
			this.pnlCalibration.TabIndex = 17;
			this.pnlCalibration.Visible = false;
			// 
			// frmEnterWavelength
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(335, 347);
			this.Controls.Add(this.pnlCalibration);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnReset);
			this.Controls.Add(this.pnlOnePointCalibration);
			this.Controls.Add(this.rbPickFromList);
			this.Controls.Add(this.rbEnterManually);
			this.Controls.Add(this.lvFraunhoferLines);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.nudManualWavelength);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmEnterWavelength";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Specify Wavelength";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmEnterWavelength_FormClosed);
			this.Load += new System.EventHandler(this.frmEnterWavelength_Load);
			this.Shown += new System.EventHandler(this.frmEnterWavelength_Shown);
			((System.ComponentModel.ISupportInitialize)(this.nudManualWavelength)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDispersion)).EndInit();
			this.pnlOnePointCalibration.ResumeLayout(false);
			this.pnlOnePointCalibration.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudPolyOrder)).EndInit();
			this.pnlCalibration.ResumeLayout(false);
			this.pnlCalibration.PerformLayout();
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
        private System.Windows.Forms.CheckBox cbxSinglePoint;
        private System.Windows.Forms.NumericUpDown nudDispersion;
        private System.Windows.Forms.Panel pnlOnePointCalibration;
		private System.Windows.Forms.Button btnReset;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox cbxCalibrate;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nudPolyOrder;
		private System.Windows.Forms.Panel pnlCalibration;
    }
}