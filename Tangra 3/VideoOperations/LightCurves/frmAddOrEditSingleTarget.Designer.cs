namespace Tangra.VideoOperations.LightCurves
{
    partial class frmAddOrEditSingleTarget
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddOrEditSingleTarget));
            this.btnDontAdd = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.rbGuidingStar = new System.Windows.Forms.RadioButton();
            this.gbxObjectType = new System.Windows.Forms.GroupBox();
            this.pnlTolerance = new System.Windows.Forms.Panel();
            this.nudPositionTolerance = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pnlAutoFitSize = new System.Windows.Forms.Panel();
            this.nudFitMatrixSize = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rbOccultedStar = new System.Windows.Forms.RadioButton();
            this.rbComparisonStar = new System.Windows.Forms.RadioButton();
            this.lblFWHM1 = new System.Windows.Forms.Label();
            this.picTarget1PSF = new System.Windows.Forms.PictureBox();
            this.picTarget1Pixels = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nudAperture1 = new System.Windows.Forms.NumericUpDown();
            this.pnlUDLR = new System.Windows.Forms.Panel();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pbox1 = new System.Windows.Forms.PictureBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.gbAperture = new System.Windows.Forms.GroupBox();
            this.rbManuallyPositionedAperture = new System.Windows.Forms.RadioButton();
            this.rbAutoCenteredAperture = new System.Windows.Forms.RadioButton();
            this.btnExplain = new System.Windows.Forms.Button();
            this.gbxObjectType.SuspendLayout();
            this.pnlTolerance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPositionTolerance)).BeginInit();
            this.pnlAutoFitSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFitMatrixSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTarget1PSF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTarget1Pixels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAperture1)).BeginInit();
            this.pnlUDLR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbox1)).BeginInit();
            this.gbAperture.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDontAdd
            // 
            this.btnDontAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDontAdd.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDontAdd.Location = new System.Drawing.Point(246, 359);
            this.btnDontAdd.Name = "btnDontAdd";
            this.btnDontAdd.Size = new System.Drawing.Size(75, 23);
            this.btnDontAdd.TabIndex = 0;
            this.btnDontAdd.Text = "Don\'t Add";
            this.btnDontAdd.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAdd.Location = new System.Drawing.Point(165, 359);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // rbGuidingStar
            // 
            this.rbGuidingStar.AutoSize = true;
            this.rbGuidingStar.Checked = true;
            this.rbGuidingStar.Location = new System.Drawing.Point(12, 19);
            this.rbGuidingStar.Name = "rbGuidingStar";
            this.rbGuidingStar.Size = new System.Drawing.Size(143, 17);
            this.rbGuidingStar.TabIndex = 2;
            this.rbGuidingStar.TabStop = true;
            this.rbGuidingStar.Text = "Guiding/Comparison Star";
            this.rbGuidingStar.UseVisualStyleBackColor = true;
            this.rbGuidingStar.CheckedChanged += new System.EventHandler(this.rgObjectType_SelectedIndexChanged);
            // 
            // gbxObjectType
            // 
            this.gbxObjectType.Controls.Add(this.pnlTolerance);
            this.gbxObjectType.Controls.Add(this.pnlAutoFitSize);
            this.gbxObjectType.Controls.Add(this.rbOccultedStar);
            this.gbxObjectType.Controls.Add(this.rbGuidingStar);
            this.gbxObjectType.Controls.Add(this.rbComparisonStar);
            this.gbxObjectType.Location = new System.Drawing.Point(12, 12);
            this.gbxObjectType.Name = "gbxObjectType";
            this.gbxObjectType.Size = new System.Drawing.Size(309, 100);
            this.gbxObjectType.TabIndex = 3;
            this.gbxObjectType.TabStop = false;
            this.gbxObjectType.Text = "Add selected object as";
            // 
            // pnlTolerance
            // 
            this.pnlTolerance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlTolerance.Controls.Add(this.nudPositionTolerance);
            this.pnlTolerance.Controls.Add(this.label4);
            this.pnlTolerance.Controls.Add(this.label5);
            this.pnlTolerance.Enabled = false;
            this.pnlTolerance.Location = new System.Drawing.Point(171, 65);
            this.pnlTolerance.Name = "pnlTolerance";
            this.pnlTolerance.Size = new System.Drawing.Size(131, 26);
            this.pnlTolerance.TabIndex = 75;
            // 
            // nudPositionTolerance
            // 
            this.nudPositionTolerance.DecimalPlaces = 1;
            this.nudPositionTolerance.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudPositionTolerance.Location = new System.Drawing.Point(65, 2);
            this.nudPositionTolerance.Maximum = new decimal(new int[] {
            35,
            0,
            0,
            65536});
            this.nudPositionTolerance.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudPositionTolerance.Name = "nudPositionTolerance";
            this.nudPositionTolerance.Size = new System.Drawing.Size(41, 20);
            this.nudPositionTolerance.TabIndex = 72;
            this.toolTip1.SetToolTip(this.nudPositionTolerance, "To allow for effects of turbulence and other random position variance");
            this.nudPositionTolerance.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.nudPositionTolerance.ValueChanged += new System.EventHandler(this.nudPositionTolerance_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 74;
            this.label4.Text = "Tolerance";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(109, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 73;
            this.label5.Text = "px";
            // 
            // pnlAutoFitSize
            // 
            this.pnlAutoFitSize.Controls.Add(this.nudFitMatrixSize);
            this.pnlAutoFitSize.Controls.Add(this.label3);
            this.pnlAutoFitSize.Controls.Add(this.label1);
            this.pnlAutoFitSize.Location = new System.Drawing.Point(158, 16);
            this.pnlAutoFitSize.Name = "pnlAutoFitSize";
            this.pnlAutoFitSize.Size = new System.Drawing.Size(143, 23);
            this.pnlAutoFitSize.TabIndex = 75;
            // 
            // nudFitMatrixSize
            // 
            this.nudFitMatrixSize.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudFitMatrixSize.Location = new System.Drawing.Point(78, 2);
            this.nudFitMatrixSize.Maximum = new decimal(new int[] {
            17,
            0,
            0,
            0});
            this.nudFitMatrixSize.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudFitMatrixSize.Name = "nudFitMatrixSize";
            this.nudFitMatrixSize.Size = new System.Drawing.Size(41, 20);
            this.nudFitMatrixSize.TabIndex = 72;
            this.nudFitMatrixSize.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.nudFitMatrixSize.ValueChanged += new System.EventHandler(this.nudFitMatrixSize_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 74;
            this.label3.Text = "PSF Fit Area";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(122, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 73;
            this.label1.Text = "px";
            // 
            // rbOccultedStar
            // 
            this.rbOccultedStar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbOccultedStar.AutoSize = true;
            this.rbOccultedStar.Location = new System.Drawing.Point(13, 71);
            this.rbOccultedStar.Name = "rbOccultedStar";
            this.rbOccultedStar.Size = new System.Drawing.Size(90, 17);
            this.rbOccultedStar.TabIndex = 3;
            this.rbOccultedStar.Text = "Occulted Star";
            this.rbOccultedStar.UseVisualStyleBackColor = true;
            this.rbOccultedStar.CheckedChanged += new System.EventHandler(this.rgObjectType_SelectedIndexChanged);
            // 
            // rbComparisonStar
            // 
            this.rbComparisonStar.AutoSize = true;
            this.rbComparisonStar.Location = new System.Drawing.Point(12, 44);
            this.rbComparisonStar.Name = "rbComparisonStar";
            this.rbComparisonStar.Size = new System.Drawing.Size(102, 17);
            this.rbComparisonStar.TabIndex = 76;
            this.rbComparisonStar.Text = "Comparison Star";
            this.rbComparisonStar.UseVisualStyleBackColor = true;
            this.rbComparisonStar.CheckedChanged += new System.EventHandler(this.rgObjectType_SelectedIndexChanged);
            // 
            // lblFWHM1
            // 
            this.lblFWHM1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFWHM1.AutoSize = true;
            this.lblFWHM1.Location = new System.Drawing.Point(24, 322);
            this.lblFWHM1.Name = "lblFWHM1";
            this.lblFWHM1.Size = new System.Drawing.Size(0, 13);
            this.lblFWHM1.TabIndex = 68;
            // 
            // picTarget1PSF
            // 
            this.picTarget1PSF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.picTarget1PSF.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picTarget1PSF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTarget1PSF.Location = new System.Drawing.Point(152, 175);
            this.picTarget1PSF.Name = "picTarget1PSF";
            this.picTarget1PSF.Size = new System.Drawing.Size(154, 154);
            this.picTarget1PSF.TabIndex = 67;
            this.picTarget1PSF.TabStop = false;
            // 
            // picTarget1Pixels
            // 
            this.picTarget1Pixels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.picTarget1Pixels.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picTarget1Pixels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTarget1Pixels.Location = new System.Drawing.Point(24, 199);
            this.picTarget1Pixels.Name = "picTarget1Pixels";
            this.picTarget1Pixels.Size = new System.Drawing.Size(119, 119);
            this.picTarget1Pixels.TabIndex = 66;
            this.picTarget1Pixels.TabStop = false;
            this.picTarget1Pixels.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picTarget1Pixels_MouseMove);
            this.picTarget1Pixels.Click += new System.EventHandler(this.picTarget1Pixels_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 179);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 65;
            this.label2.Text = "px";
            // 
            // nudAperture1
            // 
            this.nudAperture1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudAperture1.DecimalPlaces = 2;
            this.nudAperture1.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nudAperture1.Location = new System.Drawing.Point(46, 175);
            this.nudAperture1.Maximum = new decimal(new int[] {
            25,
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
            this.nudAperture1.TabIndex = 64;
            this.nudAperture1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAperture1.ValueChanged += new System.EventHandler(this.nudAperture1_ValueChanged);
            // 
            // pnlUDLR
            // 
            this.pnlUDLR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlUDLR.Controls.Add(this.btnLeft);
            this.pnlUDLR.Controls.Add(this.btnRight);
            this.pnlUDLR.Controls.Add(this.btnDown);
            this.pnlUDLR.Controls.Add(this.btnUp);
            this.pnlUDLR.Location = new System.Drawing.Point(152, 173);
            this.pnlUDLR.Name = "pnlUDLR";
            this.pnlUDLR.Size = new System.Drawing.Size(157, 160);
            this.pnlUDLR.TabIndex = 69;
            this.pnlUDLR.Visible = false;
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(31, 76);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(20, 23);
            this.btnLeft.TabIndex = 10;
            this.btnLeft.Text = "L";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(109, 76);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(20, 23);
            this.btnRight.TabIndex = 9;
            this.btnRight.Text = "R";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(70, 108);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(20, 23);
            this.btnDown.TabIndex = 8;
            this.btnDown.Text = "D";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(69, 45);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(20, 23);
            this.btnUp.TabIndex = 7;
            this.btnUp.Text = "U";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel3.BackColor = System.Drawing.SystemColors.ControlText;
            this.panel3.Location = new System.Drawing.Point(15, 344);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(304, 2);
            this.panel3.TabIndex = 70;
            // 
            // pbox1
            // 
            this.pbox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbox1.Location = new System.Drawing.Point(26, 176);
            this.pbox1.Name = "pbox1";
            this.pbox1.Size = new System.Drawing.Size(16, 16);
            this.pbox1.TabIndex = 71;
            this.pbox1.TabStop = false;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Location = new System.Drawing.Point(12, 359);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 72;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // gbAperture
            // 
            this.gbAperture.Controls.Add(this.rbManuallyPositionedAperture);
            this.gbAperture.Controls.Add(this.rbAutoCenteredAperture);
            this.gbAperture.Location = new System.Drawing.Point(10, 118);
            this.gbAperture.Name = "gbAperture";
            this.gbAperture.Size = new System.Drawing.Size(309, 49);
            this.gbAperture.TabIndex = 73;
            this.gbAperture.TabStop = false;
            this.gbAperture.Text = "Aperture";
            // 
            // rbManuallyPositionedAperture
            // 
            this.rbManuallyPositionedAperture.AutoSize = true;
            this.rbManuallyPositionedAperture.Location = new System.Drawing.Point(132, 19);
            this.rbManuallyPositionedAperture.Name = "rbManuallyPositionedAperture";
            this.rbManuallyPositionedAperture.Size = new System.Drawing.Size(119, 17);
            this.rbManuallyPositionedAperture.TabIndex = 4;
            this.rbManuallyPositionedAperture.Text = "Manually Positioned";
            this.rbManuallyPositionedAperture.UseVisualStyleBackColor = true;
            this.rbManuallyPositionedAperture.CheckedChanged += new System.EventHandler(this.rbFixedAperture_CheckedChanged);
            // 
            // rbAutoCenteredAperture
            // 
            this.rbAutoCenteredAperture.AutoSize = true;
            this.rbAutoCenteredAperture.Checked = true;
            this.rbAutoCenteredAperture.Location = new System.Drawing.Point(14, 20);
            this.rbAutoCenteredAperture.Name = "rbAutoCenteredAperture";
            this.rbAutoCenteredAperture.Size = new System.Drawing.Size(93, 17);
            this.rbAutoCenteredAperture.TabIndex = 3;
            this.rbAutoCenteredAperture.TabStop = true;
            this.rbAutoCenteredAperture.Text = "Auto-Centered";
            this.rbAutoCenteredAperture.UseVisualStyleBackColor = true;
            this.rbAutoCenteredAperture.CheckedChanged += new System.EventHandler(this.rbFixedAperture_CheckedChanged);
            // 
            // btnExplain
            // 
            this.btnExplain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExplain.Location = new System.Drawing.Point(118, 359);
            this.btnExplain.Name = "btnExplain";
            this.btnExplain.Size = new System.Drawing.Size(25, 23);
            this.btnExplain.TabIndex = 77;
            this.btnExplain.Text = "?";
            this.btnExplain.UseVisualStyleBackColor = true;
            this.btnExplain.Click += new System.EventHandler(this.btnExplain_Click);
            // 
            // frmAddOrEditSingleTarget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 394);
            this.Controls.Add(this.btnExplain);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.gbxObjectType);
            this.Controls.Add(this.pbox1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.lblFWHM1);
            this.Controls.Add(this.picTarget1Pixels);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudAperture1);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDontAdd);
            this.Controls.Add(this.picTarget1PSF);
            this.Controls.Add(this.pnlUDLR);
            this.Controls.Add(this.gbAperture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddOrEditSingleTarget";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Object";
            this.gbxObjectType.ResumeLayout(false);
            this.gbxObjectType.PerformLayout();
            this.pnlTolerance.ResumeLayout(false);
            this.pnlTolerance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPositionTolerance)).EndInit();
            this.pnlAutoFitSize.ResumeLayout(false);
            this.pnlAutoFitSize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFitMatrixSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTarget1PSF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTarget1Pixels)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAperture1)).EndInit();
            this.pnlUDLR.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbox1)).EndInit();
            this.gbAperture.ResumeLayout(false);
            this.gbAperture.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDontAdd;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.RadioButton rbGuidingStar;
        private System.Windows.Forms.GroupBox gbxObjectType;
        private System.Windows.Forms.Label lblFWHM1;
        private System.Windows.Forms.PictureBox picTarget1PSF;
        private System.Windows.Forms.PictureBox picTarget1Pixels;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudAperture1;
        private System.Windows.Forms.Panel pnlUDLR;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox pbox1;
        private System.Windows.Forms.NumericUpDown nudFitMatrixSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlAutoFitSize;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Panel pnlTolerance;
        private System.Windows.Forms.NumericUpDown nudPositionTolerance;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbOccultedStar;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox gbAperture;
        private System.Windows.Forms.RadioButton rbManuallyPositionedAperture;
        private System.Windows.Forms.RadioButton rbAutoCenteredAperture;
        private System.Windows.Forms.RadioButton rbComparisonStar;
        private System.Windows.Forms.Button btnExplain;
    }
}