namespace Tangra.VideoOperations.LightCurves
{
    partial class frmConfigureCsvExport
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
			this.tabSettings = new System.Windows.Forms.TabControl();
			this.tabTime = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.rbJulianDays = new System.Windows.Forms.RadioButton();
			this.rbDecimalDays = new System.Windows.Forms.RadioButton();
			this.rbTimeString = new System.Windows.Forms.RadioButton();
			this.tabValues = new System.Windows.Forms.TabPage();
			this.pnlMagnitude = new System.Windows.Forms.Panel();
			this.pb4 = new System.Windows.Forms.PictureBox();
			this.pnlExtinction = new System.Windows.Forms.Panel();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.tbxHeightKm = new System.Windows.Forms.TextBox();
			this.tbxLongitude = new System.Windows.Forms.TextBox();
			this.tbxRA = new System.Windows.Forms.TextBox();
			this.tbxDec = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.tbxLatitude = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.nudMag4 = new System.Windows.Forms.NumericUpDown();
			this.nudMag1 = new System.Windows.Forms.NumericUpDown();
			this.pb3 = new System.Windows.Forms.PictureBox();
			this.cbxAtmExtExport = new System.Windows.Forms.CheckBox();
			this.nudMag3 = new System.Windows.Forms.NumericUpDown();
			this.pb1 = new System.Windows.Forms.PictureBox();
			this.pb2 = new System.Windows.Forms.PictureBox();
			this.nudMag2 = new System.Windows.Forms.NumericUpDown();
			this.pnlFlux = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.rbMagnitude = new System.Windows.Forms.RadioButton();
			this.rbFlux = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.tabSeries = new System.Windows.Forms.TabPage();
			this.lblSAndB = new System.Windows.Forms.Label();
			this.rbSeriesSAndB = new System.Windows.Forms.RadioButton();
			this.lblSmB = new System.Windows.Forms.Label();
			this.rbSeriesSmB = new System.Windows.Forms.RadioButton();
			this.label11 = new System.Windows.Forms.Label();
			this.cbxSpacingOptions = new System.Windows.Forms.ComboBox();
			this.tabSettings.SuspendLayout();
			this.tabTime.SuspendLayout();
			this.tabValues.SuspendLayout();
			this.pnlMagnitude.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pb4)).BeginInit();
			this.pnlExtinction.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMag4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMag1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMag3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMag2)).BeginInit();
			this.pnlFlux.SuspendLayout();
			this.tabSeries.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabSettings
			// 
			this.tabSettings.Controls.Add(this.tabTime);
			this.tabSettings.Controls.Add(this.tabValues);
			this.tabSettings.Controls.Add(this.tabSeries);
			this.tabSettings.Location = new System.Drawing.Point(12, 12);
			this.tabSettings.Name = "tabSettings";
			this.tabSettings.SelectedIndex = 0;
			this.tabSettings.Size = new System.Drawing.Size(329, 206);
			this.tabSettings.TabIndex = 0;
			// 
			// tabTime
			// 
			this.tabTime.Controls.Add(this.label2);
			this.tabTime.Controls.Add(this.rbJulianDays);
			this.tabTime.Controls.Add(this.rbDecimalDays);
			this.tabTime.Controls.Add(this.rbTimeString);
			this.tabTime.Location = new System.Drawing.Point(4, 22);
			this.tabTime.Name = "tabTime";
			this.tabTime.Padding = new System.Windows.Forms.Padding(3);
			this.tabTime.Size = new System.Drawing.Size(321, 180);
			this.tabTime.TabIndex = 0;
			this.tabTime.Text = "Timestamps";
			this.tabTime.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(37, 34);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(173, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "(Default format supported by AOTA)";
			// 
			// rbJulianDays
			// 
			this.rbJulianDays.AutoSize = true;
			this.rbJulianDays.Location = new System.Drawing.Point(21, 104);
			this.rbJulianDays.Name = "rbJulianDays";
			this.rbJulianDays.Size = new System.Drawing.Size(164, 17);
			this.rbJulianDays.TabIndex = 3;
			this.rbJulianDays.Text = "Julian Day (JD) with Decimals";
			this.rbJulianDays.UseVisualStyleBackColor = true;
			// 
			// rbDecimalDays
			// 
			this.rbDecimalDays.AutoSize = true;
			this.rbDecimalDays.Location = new System.Drawing.Point(21, 65);
			this.rbDecimalDays.Name = "rbDecimalDays";
			this.rbDecimalDays.Size = new System.Drawing.Size(259, 17);
			this.rbDecimalDays.TabIndex = 2;
			this.rbDecimalDays.Text = "Days with Decimals from 0 UT of First Frame Date";
			this.rbDecimalDays.UseVisualStyleBackColor = true;
			// 
			// rbTimeString
			// 
			this.rbTimeString.AutoSize = true;
			this.rbTimeString.Checked = true;
			this.rbTimeString.Location = new System.Drawing.Point(21, 14);
			this.rbTimeString.Name = "rbTimeString";
			this.rbTimeString.Size = new System.Drawing.Size(145, 17);
			this.rbTimeString.TabIndex = 0;
			this.rbTimeString.TabStop = true;
			this.rbTimeString.Text = "Text Value - HH:mm:ss.fff";
			this.rbTimeString.UseVisualStyleBackColor = true;
			// 
			// tabValues
			// 
			this.tabValues.Controls.Add(this.pnlMagnitude);
			this.tabValues.Controls.Add(this.pnlFlux);
			this.tabValues.Controls.Add(this.rbMagnitude);
			this.tabValues.Controls.Add(this.rbFlux);
			this.tabValues.Location = new System.Drawing.Point(4, 22);
			this.tabValues.Name = "tabValues";
			this.tabValues.Size = new System.Drawing.Size(321, 180);
			this.tabValues.TabIndex = 1;
			this.tabValues.Text = "Photometric Values";
			this.tabValues.UseVisualStyleBackColor = true;
			// 
			// pnlMagnitude
			// 
			this.pnlMagnitude.Controls.Add(this.pb4);
			this.pnlMagnitude.Controls.Add(this.pnlExtinction);
			this.pnlMagnitude.Controls.Add(this.nudMag4);
			this.pnlMagnitude.Controls.Add(this.nudMag1);
			this.pnlMagnitude.Controls.Add(this.pb3);
			this.pnlMagnitude.Controls.Add(this.cbxAtmExtExport);
			this.pnlMagnitude.Controls.Add(this.nudMag3);
			this.pnlMagnitude.Controls.Add(this.pb1);
			this.pnlMagnitude.Controls.Add(this.pb2);
			this.pnlMagnitude.Controls.Add(this.nudMag2);
			this.pnlMagnitude.Location = new System.Drawing.Point(5, 31);
			this.pnlMagnitude.Name = "pnlMagnitude";
			this.pnlMagnitude.Size = new System.Drawing.Size(315, 146);
			this.pnlMagnitude.TabIndex = 4;
			// 
			// pb4
			// 
			this.pb4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pb4.BackColor = System.Drawing.Color.Maroon;
			this.pb4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pb4.Location = new System.Drawing.Point(228, 12);
			this.pb4.Name = "pb4";
			this.pb4.Size = new System.Drawing.Size(10, 11);
			this.pb4.TabIndex = 19;
			this.pb4.TabStop = false;
			// 
			// pnlExtinction
			// 
			this.pnlExtinction.Controls.Add(this.label8);
			this.pnlExtinction.Controls.Add(this.label7);
			this.pnlExtinction.Controls.Add(this.tbxHeightKm);
			this.pnlExtinction.Controls.Add(this.tbxLongitude);
			this.pnlExtinction.Controls.Add(this.tbxRA);
			this.pnlExtinction.Controls.Add(this.tbxDec);
			this.pnlExtinction.Controls.Add(this.label1);
			this.pnlExtinction.Controls.Add(this.label3);
			this.pnlExtinction.Controls.Add(this.tbxLatitude);
			this.pnlExtinction.Controls.Add(this.label5);
			this.pnlExtinction.Controls.Add(this.label6);
			this.pnlExtinction.Enabled = false;
			this.pnlExtinction.Location = new System.Drawing.Point(19, 57);
			this.pnlExtinction.Name = "pnlExtinction";
			this.pnlExtinction.Size = new System.Drawing.Size(289, 86);
			this.pnlExtinction.TabIndex = 0;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(260, 62);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(25, 16);
			this.label8.TabIndex = 14;
			this.label8.Text = "km";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(164, 60);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(15, 16);
			this.label7.TabIndex = 13;
			this.label7.Text = "h";
			// 
			// tbxHeightKm
			// 
			this.tbxHeightKm.Location = new System.Drawing.Point(181, 60);
			this.tbxHeightKm.Name = "tbxHeightKm";
			this.tbxHeightKm.Size = new System.Drawing.Size(76, 20);
			this.tbxHeightKm.TabIndex = 12;
			// 
			// tbxLongitude
			// 
			this.tbxLongitude.Location = new System.Drawing.Point(181, 9);
			this.tbxLongitude.Name = "tbxLongitude";
			this.tbxLongitude.Size = new System.Drawing.Size(100, 20);
			this.tbxLongitude.TabIndex = 7;
			// 
			// tbxRA
			// 
			this.tbxRA.Location = new System.Drawing.Point(30, 9);
			this.tbxRA.Name = "tbxRA";
			this.tbxRA.Size = new System.Drawing.Size(100, 20);
			this.tbxRA.TabIndex = 3;
			// 
			// tbxDec
			// 
			this.tbxDec.Location = new System.Drawing.Point(30, 34);
			this.tbxDec.Name = "tbxDec";
			this.tbxDec.Size = new System.Drawing.Size(100, 20);
			this.tbxDec.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Symbol", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(18, 17);
			this.label1.TabIndex = 5;
			this.label1.Text = "a";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Symbol", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(12, 34);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(15, 17);
			this.label3.TabIndex = 6;
			this.label3.Text = "d";
			// 
			// tbxLatitude
			// 
			this.tbxLatitude.Location = new System.Drawing.Point(181, 34);
			this.tbxLatitude.Name = "tbxLatitude";
			this.tbxLatitude.Size = new System.Drawing.Size(100, 20);
			this.tbxLatitude.TabIndex = 8;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Symbol", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(164, 11);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(16, 17);
			this.label5.TabIndex = 9;
			this.label5.Text = "l";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Symbol", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(164, 32);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(16, 17);
			this.label6.TabIndex = 11;
			this.label6.Text = "j";
			// 
			// nudMag4
			// 
			this.nudMag4.DecimalPlaces = 1;
			this.nudMag4.Location = new System.Drawing.Point(241, 8);
			this.nudMag4.Name = "nudMag4";
			this.nudMag4.Size = new System.Drawing.Size(47, 20);
			this.nudMag4.TabIndex = 18;
			this.nudMag4.Value = new decimal(new int[] {
            122,
            0,
            0,
            65536});
			this.nudMag4.ValueChanged += new System.EventHandler(this.MagValueChanged);
			// 
			// nudMag1
			// 
			this.nudMag1.DecimalPlaces = 1;
			this.nudMag1.Location = new System.Drawing.Point(34, 9);
			this.nudMag1.Name = "nudMag1";
			this.nudMag1.Size = new System.Drawing.Size(47, 20);
			this.nudMag1.TabIndex = 12;
			this.nudMag1.Value = new decimal(new int[] {
            122,
            0,
            0,
            65536});
			this.nudMag1.ValueChanged += new System.EventHandler(this.MagValueChanged);
			// 
			// pb3
			// 
			this.pb3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pb3.BackColor = System.Drawing.Color.Maroon;
			this.pb3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pb3.Location = new System.Drawing.Point(159, 12);
			this.pb3.Name = "pb3";
			this.pb3.Size = new System.Drawing.Size(10, 11);
			this.pb3.TabIndex = 17;
			this.pb3.TabStop = false;
			// 
			// cbxAtmExtExport
			// 
			this.cbxAtmExtExport.AutoSize = true;
			this.cbxAtmExtExport.Location = new System.Drawing.Point(13, 37);
			this.cbxAtmExtExport.Name = "cbxAtmExtExport";
			this.cbxAtmExtExport.Size = new System.Drawing.Size(255, 17);
			this.cbxAtmExtExport.TabIndex = 2;
			this.cbxAtmExtExport.Text = "Export Atmospheric Extinction Correction Column";
			this.cbxAtmExtExport.UseVisualStyleBackColor = true;
			this.cbxAtmExtExport.CheckedChanged += new System.EventHandler(this.cbxAtmExtExport_CheckedChanged);
			// 
			// nudMag3
			// 
			this.nudMag3.DecimalPlaces = 1;
			this.nudMag3.Location = new System.Drawing.Point(172, 8);
			this.nudMag3.Name = "nudMag3";
			this.nudMag3.Size = new System.Drawing.Size(47, 20);
			this.nudMag3.TabIndex = 16;
			this.nudMag3.Value = new decimal(new int[] {
            122,
            0,
            0,
            65536});
			this.nudMag3.ValueChanged += new System.EventHandler(this.MagValueChanged);
			// 
			// pb1
			// 
			this.pb1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pb1.BackColor = System.Drawing.Color.Maroon;
			this.pb1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pb1.Location = new System.Drawing.Point(21, 13);
			this.pb1.Name = "pb1";
			this.pb1.Size = new System.Drawing.Size(10, 11);
			this.pb1.TabIndex = 13;
			this.pb1.TabStop = false;
			// 
			// pb2
			// 
			this.pb2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pb2.BackColor = System.Drawing.Color.Maroon;
			this.pb2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pb2.Location = new System.Drawing.Point(90, 12);
			this.pb2.Name = "pb2";
			this.pb2.Size = new System.Drawing.Size(10, 11);
			this.pb2.TabIndex = 15;
			this.pb2.TabStop = false;
			// 
			// nudMag2
			// 
			this.nudMag2.DecimalPlaces = 1;
			this.nudMag2.Location = new System.Drawing.Point(103, 8);
			this.nudMag2.Name = "nudMag2";
			this.nudMag2.Size = new System.Drawing.Size(47, 20);
			this.nudMag2.TabIndex = 14;
			this.nudMag2.Value = new decimal(new int[] {
            122,
            0,
            0,
            65536});
			this.nudMag2.ValueChanged += new System.EventHandler(this.MagValueChanged);
			// 
			// pnlFlux
			// 
			this.pnlFlux.Controls.Add(this.label4);
			this.pnlFlux.Location = new System.Drawing.Point(4, 30);
			this.pnlFlux.Name = "pnlFlux";
			this.pnlFlux.Size = new System.Drawing.Size(306, 150);
			this.pnlFlux.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 14);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(279, 72);
			this.label4.TabIndex = 0;
			this.label4.Text = "    This is the default format exporting the unmodifed readings. This is the form" +
    "at supported by AOTA";
			// 
			// rbMagnitude
			// 
			this.rbMagnitude.AutoSize = true;
			this.rbMagnitude.Location = new System.Drawing.Point(130, 14);
			this.rbMagnitude.Name = "rbMagnitude";
			this.rbMagnitude.Size = new System.Drawing.Size(75, 17);
			this.rbMagnitude.TabIndex = 1;
			this.rbMagnitude.Text = "Magnitude";
			this.rbMagnitude.UseVisualStyleBackColor = true;
			this.rbMagnitude.CheckedChanged += new System.EventHandler(this.rbMagnitude_CheckedChanged);
			// 
			// rbFlux
			// 
			this.rbFlux.AutoSize = true;
			this.rbFlux.Checked = true;
			this.rbFlux.Location = new System.Drawing.Point(21, 14);
			this.rbFlux.Name = "rbFlux";
			this.rbFlux.Size = new System.Drawing.Size(86, 17);
			this.rbFlux.TabIndex = 0;
			this.rbFlux.TabStop = true;
			this.rbFlux.Text = "Relative Flux";
			this.rbFlux.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(186, 224);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(267, 224);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// tabSeries
			// 
			this.tabSeries.Controls.Add(this.cbxSpacingOptions);
			this.tabSeries.Controls.Add(this.label11);
			this.tabSeries.Controls.Add(this.lblSmB);
			this.tabSeries.Controls.Add(this.rbSeriesSmB);
			this.tabSeries.Controls.Add(this.lblSAndB);
			this.tabSeries.Controls.Add(this.rbSeriesSAndB);
			this.tabSeries.Location = new System.Drawing.Point(4, 22);
			this.tabSeries.Name = "tabSeries";
			this.tabSeries.Padding = new System.Windows.Forms.Padding(3);
			this.tabSeries.Size = new System.Drawing.Size(321, 180);
			this.tabSeries.TabIndex = 2;
			this.tabSeries.Text = "Series";
			this.tabSeries.UseVisualStyleBackColor = true;
			// 
			// lblSAndB
			// 
			this.lblSAndB.AutoSize = true;
			this.lblSAndB.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSAndB.Location = new System.Drawing.Point(45, 34);
			this.lblSAndB.Name = "lblSAndB";
			this.lblSAndB.Size = new System.Drawing.Size(173, 13);
			this.lblSAndB.TabIndex = 6;
			this.lblSAndB.Text = "(Default format supported by AOTA)";
			// 
			// rbSeriesSAndB
			// 
			this.rbSeriesSAndB.AutoSize = true;
			this.rbSeriesSAndB.Checked = true;
			this.rbSeriesSAndB.Location = new System.Drawing.Point(21, 14);
			this.rbSeriesSAndB.Name = "rbSeriesSAndB";
			this.rbSeriesSAndB.Size = new System.Drawing.Size(189, 17);
			this.rbSeriesSAndB.TabIndex = 5;
			this.rbSeriesSAndB.Text = "Signal and Background Separately";
			this.rbSeriesSAndB.UseVisualStyleBackColor = true;
			// 
			// lblSmB
			// 
			this.lblSmB.AutoSize = true;
			this.lblSmB.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSmB.Location = new System.Drawing.Point(45, 78);
			this.lblSmB.Name = "lblSmB";
			this.lblSmB.Size = new System.Drawing.Size(173, 13);
			this.lblSmB.TabIndex = 8;
			this.lblSmB.Text = "(Default format supported by AOTA)";
			this.lblSmB.Visible = false;
			// 
			// rbSeriesSmB
			// 
			this.rbSeriesSmB.AutoSize = true;
			this.rbSeriesSmB.Location = new System.Drawing.Point(21, 58);
			this.rbSeriesSmB.Name = "rbSeriesSmB";
			this.rbSeriesSmB.Size = new System.Drawing.Size(145, 17);
			this.rbSeriesSmB.TabIndex = 7;
			this.rbSeriesSmB.Text = "Signal-minus-Background";
			this.rbSeriesSmB.UseVisualStyleBackColor = true;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(21, 115);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(49, 13);
			this.label11.TabIndex = 9;
			this.label11.Text = "Spacing:";
			// 
			// cbxSpacingOptions
			// 
			this.cbxSpacingOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxSpacingOptions.FormattingEnabled = true;
			this.cbxSpacingOptions.Items.AddRange(new object[] {
            "Export All Data Points",
            "Export Every 2-nd Data Point",
            "Export Every 3-rd Data Point",
            "Export Every 4-th Data Point",
            "Export Every 5-th Data Point"});
			this.cbxSpacingOptions.Location = new System.Drawing.Point(75, 112);
			this.cbxSpacingOptions.Name = "cbxSpacingOptions";
			this.cbxSpacingOptions.Size = new System.Drawing.Size(210, 21);
			this.cbxSpacingOptions.TabIndex = 10;
			// 
			// frmConfigureCsvExport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(354, 259);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.tabSettings);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmConfigureCsvExport";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configure CSV Export";
			this.Load += new System.EventHandler(this.frmConfigureCsvExport_Load);
			this.tabSettings.ResumeLayout(false);
			this.tabTime.ResumeLayout(false);
			this.tabTime.PerformLayout();
			this.tabValues.ResumeLayout(false);
			this.tabValues.PerformLayout();
			this.pnlMagnitude.ResumeLayout(false);
			this.pnlMagnitude.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pb4)).EndInit();
			this.pnlExtinction.ResumeLayout(false);
			this.pnlExtinction.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMag4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMag1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMag3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMag2)).EndInit();
			this.pnlFlux.ResumeLayout(false);
			this.tabSeries.ResumeLayout(false);
			this.tabSeries.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabSettings;
        private System.Windows.Forms.TabPage tabTime;
        private System.Windows.Forms.RadioButton rbJulianDays;
        private System.Windows.Forms.RadioButton rbDecimalDays;
        private System.Windows.Forms.RadioButton rbTimeString;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabValues;
        private System.Windows.Forms.RadioButton rbMagnitude;
        private System.Windows.Forms.RadioButton rbFlux;
        private System.Windows.Forms.NumericUpDown nudMag1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbxLatitude;
        private System.Windows.Forms.TextBox tbxLongitude;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxDec;
        private System.Windows.Forms.TextBox tbxRA;
        private System.Windows.Forms.CheckBox cbxAtmExtExport;
        private System.Windows.Forms.PictureBox pb4;
        private System.Windows.Forms.NumericUpDown nudMag4;
        private System.Windows.Forms.PictureBox pb3;
        private System.Windows.Forms.NumericUpDown nudMag3;
        private System.Windows.Forms.PictureBox pb2;
        private System.Windows.Forms.NumericUpDown nudMag2;
        private System.Windows.Forms.PictureBox pb1;
        private System.Windows.Forms.Panel pnlFlux;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnlMagnitude;
        private System.Windows.Forms.Panel pnlExtinction;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbxHeightKm;
		private System.Windows.Forms.TabPage tabSeries;
		private System.Windows.Forms.Label lblSmB;
		private System.Windows.Forms.RadioButton rbSeriesSmB;
		private System.Windows.Forms.Label lblSAndB;
		private System.Windows.Forms.RadioButton rbSeriesSAndB;
		private System.Windows.Forms.ComboBox cbxSpacingOptions;
		private System.Windows.Forms.Label label11;
    }
}