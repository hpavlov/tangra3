namespace Tangra.VideoOperations.LightCurves.AdjustApertures
{
	partial class frmAdjustApertures
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAdjustApertures));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rbSameApertures = new System.Windows.Forms.RadioButton();
            this.rbCusomApertures = new System.Windows.Forms.RadioButton();
            this.pnlSameApertures = new System.Windows.Forms.Panel();
            this.cbxCommonFWHMType = new System.Windows.Forms.ComboBox();
            this.cbxCommonUnit = new System.Windows.Forms.ComboBox();
            this.nudCommonAperture = new System.Windows.Forms.NumericUpDown();
            this.rbBoundToFWHM = new System.Windows.Forms.RadioButton();
            this.nudAperture1 = new System.Windows.Forms.NumericUpDown();
            this.nudAperture2 = new System.Windows.Forms.NumericUpDown();
            this.nudAperture3 = new System.Windows.Forms.NumericUpDown();
            this.nudAperture4 = new System.Windows.Forms.NumericUpDown();
            this.pb4 = new System.Windows.Forms.PictureBox();
            this.pb3 = new System.Windows.Forms.PictureBox();
            this.pb2 = new System.Windows.Forms.PictureBox();
            this.pb1 = new System.Windows.Forms.PictureBox();
            this.pnlCusomApertures = new System.Windows.Forms.Panel();
            this.nudFWHMAperture = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlBoundToFWHM = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.picTarget1Pixels = new System.Windows.Forms.PictureBox();
            this.picTarget2Pixels = new System.Windows.Forms.PictureBox();
            this.picTarget3Pixels = new System.Windows.Forms.PictureBox();
            this.picTarget4Pixels = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pnlSeparator = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.miRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlSameApertures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCommonAperture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAperture1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAperture2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAperture3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAperture4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).BeginInit();
            this.pnlCusomApertures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFWHMAperture)).BeginInit();
            this.pnlBoundToFWHM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTarget1Pixels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTarget2Pixels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTarget3Pixels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTarget4Pixels)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(192, 392);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(287, 392);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // rbSameApertures
            // 
            this.rbSameApertures.AutoSize = true;
            this.rbSameApertures.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbSameApertures.Location = new System.Drawing.Point(40, 170);
            this.rbSameApertures.Name = "rbSameApertures";
            this.rbSameApertures.Size = new System.Drawing.Size(211, 17);
            this.rbSameApertures.TabIndex = 2;
            this.rbSameApertures.Text = "Make all apertures the same size";
            this.rbSameApertures.UseVisualStyleBackColor = true;
            this.rbSameApertures.CheckedChanged += new System.EventHandler(this.OnSelectedAdjustmentModeChanged);
            // 
            // rbCusomApertures
            // 
            this.rbCusomApertures.AutoSize = true;
            this.rbCusomApertures.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbCusomApertures.Location = new System.Drawing.Point(40, 288);
            this.rbCusomApertures.Name = "rbCusomApertures";
            this.rbCusomApertures.Size = new System.Drawing.Size(221, 17);
            this.rbCusomApertures.TabIndex = 3;
            this.rbCusomApertures.Text = "Set own aperture values (in pixels)";
            this.rbCusomApertures.UseVisualStyleBackColor = true;
            this.rbCusomApertures.CheckedChanged += new System.EventHandler(this.OnSelectedAdjustmentModeChanged);
            // 
            // pnlSameApertures
            // 
            this.pnlSameApertures.Controls.Add(this.panel1);
            this.pnlSameApertures.Controls.Add(this.cbxCommonFWHMType);
            this.pnlSameApertures.Controls.Add(this.cbxCommonUnit);
            this.pnlSameApertures.Controls.Add(this.nudCommonAperture);
            this.pnlSameApertures.Location = new System.Drawing.Point(51, 233);
            this.pnlSameApertures.Name = "pnlSameApertures";
            this.pnlSameApertures.Size = new System.Drawing.Size(389, 42);
            this.pnlSameApertures.TabIndex = 4;
            // 
            // cbxCommonFWHMType
            // 
            this.cbxCommonFWHMType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCommonFWHMType.FormattingEnabled = true;
            this.cbxCommonFWHMType.Items.AddRange(new object[] {
            "  of the Largest Star",
            "  of Averaged Star"});
            this.cbxCommonFWHMType.Location = new System.Drawing.Point(141, 10);
            this.cbxCommonFWHMType.Name = "cbxCommonFWHMType";
            this.cbxCommonFWHMType.Size = new System.Drawing.Size(122, 21);
            this.cbxCommonFWHMType.TabIndex = 2;
            this.cbxCommonFWHMType.Visible = false;
            this.cbxCommonFWHMType.SelectedIndexChanged += new System.EventHandler(this.OnAperturesChanged);
            // 
            // cbxCommonUnit
            // 
            this.cbxCommonUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCommonUnit.FormattingEnabled = true;
            this.cbxCommonUnit.Items.AddRange(new object[] {
            "Pixels",
            "FWHM"});
            this.cbxCommonUnit.Location = new System.Drawing.Point(63, 10);
            this.cbxCommonUnit.Name = "cbxCommonUnit";
            this.cbxCommonUnit.Size = new System.Drawing.Size(72, 21);
            this.cbxCommonUnit.TabIndex = 1;
            this.cbxCommonUnit.SelectedIndexChanged += new System.EventHandler(this.cbxCommonUnit_SelectedIndexChanged);
            // 
            // nudCommonAperture
            // 
            this.nudCommonAperture.DecimalPlaces = 1;
            this.nudCommonAperture.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudCommonAperture.Location = new System.Drawing.Point(10, 10);
            this.nudCommonAperture.Maximum = new decimal(new int[] {
            35,
            0,
            0,
            0});
            this.nudCommonAperture.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCommonAperture.Name = "nudCommonAperture";
            this.nudCommonAperture.Size = new System.Drawing.Size(47, 20);
            this.nudCommonAperture.TabIndex = 0;
            this.nudCommonAperture.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCommonAperture.ValueChanged += new System.EventHandler(this.OnAperturesChanged);
            // 
            // rbBoundToFWHM
            // 
            this.rbBoundToFWHM.AutoSize = true;
            this.rbBoundToFWHM.Checked = true;
            this.rbBoundToFWHM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbBoundToFWHM.Location = new System.Drawing.Point(40, 73);
            this.rbBoundToFWHM.Name = "rbBoundToFWHM";
            this.rbBoundToFWHM.Size = new System.Drawing.Size(264, 17);
            this.rbBoundToFWHM.TabIndex = 5;
            this.rbBoundToFWHM.TabStop = true;
            this.rbBoundToFWHM.Text = "Adjust apertures to individual FWHM sizes";
            this.rbBoundToFWHM.UseVisualStyleBackColor = true;
            this.rbBoundToFWHM.CheckedChanged += new System.EventHandler(this.OnSelectedAdjustmentModeChanged);
            // 
            // nudAperture1
            // 
            this.nudAperture1.DecimalPlaces = 1;
            this.nudAperture1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudAperture1.Location = new System.Drawing.Point(30, 7);
            this.nudAperture1.Maximum = new decimal(new int[] {
            35,
            0,
            0,
            0});
            this.nudAperture1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAperture1.Name = "nudAperture1";
            this.nudAperture1.Size = new System.Drawing.Size(47, 20);
            this.nudAperture1.TabIndex = 6;
            this.nudAperture1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAperture1.ValueChanged += new System.EventHandler(this.OnCustomAperturesChanged);
            // 
            // nudAperture2
            // 
            this.nudAperture2.DecimalPlaces = 1;
            this.nudAperture2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudAperture2.Location = new System.Drawing.Point(116, 7);
            this.nudAperture2.Maximum = new decimal(new int[] {
            35,
            0,
            0,
            0});
            this.nudAperture2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAperture2.Name = "nudAperture2";
            this.nudAperture2.Size = new System.Drawing.Size(47, 20);
            this.nudAperture2.TabIndex = 7;
            this.nudAperture2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAperture2.ValueChanged += new System.EventHandler(this.OnCustomAperturesChanged);
            // 
            // nudAperture3
            // 
            this.nudAperture3.DecimalPlaces = 1;
            this.nudAperture3.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudAperture3.Location = new System.Drawing.Point(202, 7);
            this.nudAperture3.Maximum = new decimal(new int[] {
            35,
            0,
            0,
            0});
            this.nudAperture3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAperture3.Name = "nudAperture3";
            this.nudAperture3.Size = new System.Drawing.Size(47, 20);
            this.nudAperture3.TabIndex = 8;
            this.nudAperture3.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAperture3.Visible = false;
            this.nudAperture3.ValueChanged += new System.EventHandler(this.OnCustomAperturesChanged);
            // 
            // nudAperture4
            // 
            this.nudAperture4.DecimalPlaces = 1;
            this.nudAperture4.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudAperture4.Location = new System.Drawing.Point(286, 7);
            this.nudAperture4.Maximum = new decimal(new int[] {
            35,
            0,
            0,
            0});
            this.nudAperture4.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAperture4.Name = "nudAperture4";
            this.nudAperture4.Size = new System.Drawing.Size(47, 20);
            this.nudAperture4.TabIndex = 9;
            this.nudAperture4.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAperture4.Visible = false;
            this.nudAperture4.ValueChanged += new System.EventHandler(this.OnCustomAperturesChanged);
            // 
            // pb4
            // 
            this.pb4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb4.BackColor = System.Drawing.Color.Maroon;
            this.pb4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb4.Location = new System.Drawing.Point(270, 12);
            this.pb4.Name = "pb4";
            this.pb4.Size = new System.Drawing.Size(10, 11);
            this.pb4.TabIndex = 20;
            this.pb4.TabStop = false;
            this.pb4.Visible = false;
            // 
            // pb3
            // 
            this.pb3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb3.BackColor = System.Drawing.Color.Maroon;
            this.pb3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb3.Location = new System.Drawing.Point(186, 12);
            this.pb3.Name = "pb3";
            this.pb3.Size = new System.Drawing.Size(10, 11);
            this.pb3.TabIndex = 19;
            this.pb3.TabStop = false;
            this.pb3.Visible = false;
            // 
            // pb2
            // 
            this.pb2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb2.BackColor = System.Drawing.Color.Maroon;
            this.pb2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb2.Location = new System.Drawing.Point(99, 12);
            this.pb2.Name = "pb2";
            this.pb2.Size = new System.Drawing.Size(10, 11);
            this.pb2.TabIndex = 18;
            this.pb2.TabStop = false;
            // 
            // pb1
            // 
            this.pb1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb1.BackColor = System.Drawing.Color.Maroon;
            this.pb1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb1.Location = new System.Drawing.Point(14, 12);
            this.pb1.Name = "pb1";
            this.pb1.Size = new System.Drawing.Size(10, 11);
            this.pb1.TabIndex = 17;
            this.pb1.TabStop = false;
            // 
            // pnlCusomApertures
            // 
            this.pnlCusomApertures.Controls.Add(this.nudAperture4);
            this.pnlCusomApertures.Controls.Add(this.pb4);
            this.pnlCusomApertures.Controls.Add(this.nudAperture1);
            this.pnlCusomApertures.Controls.Add(this.pb3);
            this.pnlCusomApertures.Controls.Add(this.nudAperture2);
            this.pnlCusomApertures.Controls.Add(this.pb2);
            this.pnlCusomApertures.Controls.Add(this.nudAperture3);
            this.pnlCusomApertures.Controls.Add(this.pb1);
            this.pnlCusomApertures.Location = new System.Drawing.Point(49, 329);
            this.pnlCusomApertures.Name = "pnlCusomApertures";
            this.pnlCusomApertures.Size = new System.Drawing.Size(347, 36);
            this.pnlCusomApertures.TabIndex = 21;
            // 
            // nudFWHMAperture
            // 
            this.nudFWHMAperture.DecimalPlaces = 1;
            this.nudFWHMAperture.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudFWHMAperture.Location = new System.Drawing.Point(3, 3);
            this.nudFWHMAperture.Maximum = new decimal(new int[] {
            35,
            0,
            0,
            0});
            this.nudFWHMAperture.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFWHMAperture.Name = "nudFWHMAperture";
            this.nudFWHMAperture.Size = new System.Drawing.Size(47, 20);
            this.nudFWHMAperture.TabIndex = 22;
            this.nudFWHMAperture.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFWHMAperture.ValueChanged += new System.EventHandler(this.OnAperturesChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(56, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "FWHM";
            // 
            // pnlBoundToFWHM
            // 
            this.pnlBoundToFWHM.Controls.Add(this.nudFWHMAperture);
            this.pnlBoundToFWHM.Controls.Add(this.label1);
            this.pnlBoundToFWHM.Location = new System.Drawing.Point(61, 127);
            this.pnlBoundToFWHM.Name = "pnlBoundToFWHM";
            this.pnlBoundToFWHM.Size = new System.Drawing.Size(148, 32);
            this.pnlBoundToFWHM.TabIndex = 24;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(11, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(424, 46);
            this.label2.TabIndex = 25;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // picTarget1Pixels
            // 
            this.picTarget1Pixels.BackColor = System.Drawing.Color.White;
            this.picTarget1Pixels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTarget1Pixels.Location = new System.Drawing.Point(11, 16);
            this.picTarget1Pixels.Name = "picTarget1Pixels";
            this.picTarget1Pixels.Size = new System.Drawing.Size(70, 70);
            this.picTarget1Pixels.TabIndex = 26;
            this.picTarget1Pixels.TabStop = false;
            this.picTarget1Pixels.Visible = false;
            // 
            // picTarget2Pixels
            // 
            this.picTarget2Pixels.BackColor = System.Drawing.Color.White;
            this.picTarget2Pixels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTarget2Pixels.Location = new System.Drawing.Point(11, 99);
            this.picTarget2Pixels.Name = "picTarget2Pixels";
            this.picTarget2Pixels.Size = new System.Drawing.Size(70, 70);
            this.picTarget2Pixels.TabIndex = 27;
            this.picTarget2Pixels.TabStop = false;
            this.picTarget2Pixels.Visible = false;
            // 
            // picTarget3Pixels
            // 
            this.picTarget3Pixels.BackColor = System.Drawing.Color.White;
            this.picTarget3Pixels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTarget3Pixels.Location = new System.Drawing.Point(11, 182);
            this.picTarget3Pixels.Name = "picTarget3Pixels";
            this.picTarget3Pixels.Size = new System.Drawing.Size(70, 70);
            this.picTarget3Pixels.TabIndex = 28;
            this.picTarget3Pixels.TabStop = false;
            this.picTarget3Pixels.Visible = false;
            // 
            // picTarget4Pixels
            // 
            this.picTarget4Pixels.BackColor = System.Drawing.Color.White;
            this.picTarget4Pixels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTarget4Pixels.Location = new System.Drawing.Point(11, 265);
            this.picTarget4Pixels.Name = "picTarget4Pixels";
            this.picTarget4Pixels.Size = new System.Drawing.Size(70, 70);
            this.picTarget4Pixels.TabIndex = 29;
            this.picTarget4Pixels.TabStop = false;
            this.picTarget4Pixels.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.picTarget1Pixels);
            this.groupBox1.Controls.Add(this.picTarget4Pixels);
            this.groupBox1.Controls.Add(this.picTarget2Pixels);
            this.groupBox1.Controls.Add(this.picTarget3Pixels);
            this.groupBox1.Location = new System.Drawing.Point(446, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(92, 347);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(44, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(391, 34);
            this.label3.TabIndex = 31;
            this.label3.Text = "    This is the default option in Tangra. It provides higher SNR for occultation " +
    "detection and should be preferred when measuring a single occultation video file" +
    ".";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(45, 193);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(390, 47);
            this.label4.TabIndex = 32;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(56, 309);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(279, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Quckly change the aperture sizes of all measured objects.";
            // 
            // pnlSeparator
            // 
            this.pnlSeparator.BackColor = System.Drawing.Color.Black;
            this.pnlSeparator.Location = new System.Drawing.Point(40, 375);
            this.pnlSeparator.Name = "pnlSeparator";
            this.pnlSeparator.Size = new System.Drawing.Size(498, 2);
            this.pnlSeparator.TabIndex = 34;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.menuStrip1);
            this.panel1.Location = new System.Drawing.Point(273, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(111, 29);
            this.panel1.TabIndex = 3;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miRecent});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(111, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // miRecent
            // 
            this.miRecent.BackColor = System.Drawing.SystemColors.ControlLight;
            this.miRecent.Margin = new System.Windows.Forms.Padding(1);
            this.miRecent.Name = "miRecent";
            this.miRecent.Size = new System.Drawing.Size(93, 19);
            this.miRecent.Text = "&Recently Used";
            // 
            // frmAdjustApertures
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 432);
            this.Controls.Add(this.pnlSeparator);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pnlBoundToFWHM);
            this.Controls.Add(this.pnlCusomApertures);
            this.Controls.Add(this.rbBoundToFWHM);
            this.Controls.Add(this.pnlSameApertures);
            this.Controls.Add(this.rbCusomApertures);
            this.Controls.Add(this.rbSameApertures);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAdjustApertures";
            this.Text = "Adjust Aperture Sizes";
            this.Shown += new System.EventHandler(this.frmAdjustApertures_Shown);
            this.pnlSameApertures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudCommonAperture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAperture1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAperture2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAperture3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAperture4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).EndInit();
            this.pnlCusomApertures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudFWHMAperture)).EndInit();
            this.pnlBoundToFWHM.ResumeLayout(false);
            this.pnlBoundToFWHM.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTarget1Pixels)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTarget2Pixels)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTarget3Pixels)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTarget4Pixels)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.RadioButton rbSameApertures;
		private System.Windows.Forms.RadioButton rbCusomApertures;
		private System.Windows.Forms.Panel pnlSameApertures;
		private System.Windows.Forms.ComboBox cbxCommonFWHMType;
		private System.Windows.Forms.ComboBox cbxCommonUnit;
		private System.Windows.Forms.NumericUpDown nudCommonAperture;
		private System.Windows.Forms.RadioButton rbBoundToFWHM;
		private System.Windows.Forms.NumericUpDown nudAperture1;
		private System.Windows.Forms.NumericUpDown nudAperture2;
		private System.Windows.Forms.NumericUpDown nudAperture3;
		private System.Windows.Forms.NumericUpDown nudAperture4;
		private System.Windows.Forms.PictureBox pb4;
		private System.Windows.Forms.PictureBox pb3;
		private System.Windows.Forms.PictureBox pb2;
		private System.Windows.Forms.PictureBox pb1;
		private System.Windows.Forms.Panel pnlCusomApertures;
		private System.Windows.Forms.NumericUpDown nudFWHMAperture;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel pnlBoundToFWHM;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox picTarget1Pixels;
		private System.Windows.Forms.PictureBox picTarget2Pixels;
		private System.Windows.Forms.PictureBox picTarget3Pixels;
		private System.Windows.Forms.PictureBox picTarget4Pixels;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Panel pnlSeparator;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem miRecent;
	}
}