namespace Tangra.Video.FITS
{
    partial class frmChooseTimeHeaders
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChooseTimeHeaders));
            this.rbTimeDuration = new System.Windows.Forms.RadioButton();
            this.rbStartEndTimestamp = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlExposure = new System.Windows.Forms.Panel();
            this.pbxExposureWarning = new System.Windows.Forms.PictureBox();
            this.pbxExposureOK = new System.Windows.Forms.PictureBox();
            this.tbxExposureValue = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxExposure = new System.Windows.Forms.ComboBox();
            this.cbxExposureUnits = new System.Windows.Forms.ComboBox();
            this.ucTimestampControl2 = new Tangra.Video.FITS.ucFitsTimeStampConfigurator();
            this.ucTimestampControl = new Tangra.Video.FITS.ucFitsTimeStampConfigurator();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnPixelValueMapping = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.pnlExposure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxExposureWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxExposureOK)).BeginInit();
            this.SuspendLayout();
            // 
            // rbTimeDuration
            // 
            this.rbTimeDuration.AutoSize = true;
            this.rbTimeDuration.Checked = true;
            this.rbTimeDuration.Location = new System.Drawing.Point(15, 18);
            this.rbTimeDuration.Name = "rbTimeDuration";
            this.rbTimeDuration.Size = new System.Drawing.Size(132, 17);
            this.rbTimeDuration.TabIndex = 1;
            this.rbTimeDuration.TabStop = true;
            this.rbTimeDuration.Text = "Timestamp + Exposure";
            this.rbTimeDuration.UseVisualStyleBackColor = true;
            // 
            // rbStartEndTimestamp
            // 
            this.rbStartEndTimestamp.AutoSize = true;
            this.rbStartEndTimestamp.Location = new System.Drawing.Point(207, 18);
            this.rbStartEndTimestamp.Name = "rbStartEndTimestamp";
            this.rbStartEndTimestamp.Size = new System.Drawing.Size(137, 17);
            this.rbStartEndTimestamp.TabIndex = 2;
            this.rbStartEndTimestamp.Text = "Start + End Timestamps";
            this.rbStartEndTimestamp.UseVisualStyleBackColor = true;
            this.rbStartEndTimestamp.CheckedChanged += new System.EventHandler(this.rbStartEndTimestamp_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pnlExposure);
            this.groupBox1.Controls.Add(this.ucTimestampControl2);
            this.groupBox1.Controls.Add(this.ucTimestampControl);
            this.groupBox1.Location = new System.Drawing.Point(15, 47);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(447, 310);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // pnlExposure
            // 
            this.pnlExposure.Controls.Add(this.pbxExposureWarning);
            this.pnlExposure.Controls.Add(this.pbxExposureOK);
            this.pnlExposure.Controls.Add(this.tbxExposureValue);
            this.pnlExposure.Controls.Add(this.label5);
            this.pnlExposure.Controls.Add(this.label3);
            this.pnlExposure.Controls.Add(this.cbxExposure);
            this.pnlExposure.Controls.Add(this.cbxExposureUnits);
            this.pnlExposure.Location = new System.Drawing.Point(8, 186);
            this.pnlExposure.Name = "pnlExposure";
            this.pnlExposure.Size = new System.Drawing.Size(434, 104);
            this.pnlExposure.TabIndex = 4;
            // 
            // pbxExposureWarning
            // 
            this.pbxExposureWarning.BackColor = System.Drawing.Color.Transparent;
            this.pbxExposureWarning.Image = ((System.Drawing.Image)(resources.GetObject("pbxExposureWarning.Image")));
            this.pbxExposureWarning.Location = new System.Drawing.Point(2, 34);
            this.pbxExposureWarning.Name = "pbxExposureWarning";
            this.pbxExposureWarning.Size = new System.Drawing.Size(16, 16);
            this.pbxExposureWarning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxExposureWarning.TabIndex = 11;
            this.pbxExposureWarning.TabStop = false;
            // 
            // pbxExposureOK
            // 
            this.pbxExposureOK.BackColor = System.Drawing.Color.Transparent;
            this.pbxExposureOK.Image = ((System.Drawing.Image)(resources.GetObject("pbxExposureOK.Image")));
            this.pbxExposureOK.Location = new System.Drawing.Point(2, 34);
            this.pbxExposureOK.Name = "pbxExposureOK";
            this.pbxExposureOK.Size = new System.Drawing.Size(16, 16);
            this.pbxExposureOK.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbxExposureOK.TabIndex = 10;
            this.pbxExposureOK.TabStop = false;
            // 
            // tbxExposureValue
            // 
            this.tbxExposureValue.Location = new System.Drawing.Point(24, 58);
            this.tbxExposureValue.Multiline = true;
            this.tbxExposureValue.Name = "tbxExposureValue";
            this.tbxExposureValue.ReadOnly = true;
            this.tbxExposureValue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxExposureValue.Size = new System.Drawing.Size(403, 33);
            this.tbxExposureValue.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Exposure";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(128, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Units";
            // 
            // cbxExposure
            // 
            this.cbxExposure.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxExposure.FormattingEnabled = true;
            this.cbxExposure.Location = new System.Drawing.Point(24, 31);
            this.cbxExposure.Name = "cbxExposure";
            this.cbxExposure.Size = new System.Drawing.Size(101, 21);
            this.cbxExposure.TabIndex = 8;
            this.cbxExposure.SelectedIndexChanged += new System.EventHandler(this.cbxExposure_SelectedIndexChanged);
            // 
            // cbxExposureUnits
            // 
            this.cbxExposureUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxExposureUnits.FormattingEnabled = true;
            this.cbxExposureUnits.Items.AddRange(new object[] {
            "Seconds",
            "Milliseconds",
            "Microseconds",
            "Nanoseconds",
            "Minutes",
            "Hours",
            "Days"});
            this.cbxExposureUnits.Location = new System.Drawing.Point(131, 31);
            this.cbxExposureUnits.Name = "cbxExposureUnits";
            this.cbxExposureUnits.Size = new System.Drawing.Size(108, 21);
            this.cbxExposureUnits.TabIndex = 10;
            this.cbxExposureUnits.SelectedIndexChanged += new System.EventHandler(this.cbxExposureUnits_SelectedIndexChanged);
            // 
            // ucTimestampControl2
            // 
            this.ucTimestampControl2.Location = new System.Drawing.Point(6, 170);
            this.ucTimestampControl2.Name = "ucTimestampControl2";
            this.ucTimestampControl2.Size = new System.Drawing.Size(434, 134);
            this.ucTimestampControl2.TabIndex = 8;
            this.ucTimestampControl2.Visible = false;
            // 
            // ucTimestampControl
            // 
            this.ucTimestampControl.Location = new System.Drawing.Point(7, 19);
            this.ucTimestampControl.Name = "ucTimestampControl";
            this.ucTimestampControl.Size = new System.Drawing.Size(434, 134);
            this.ucTimestampControl.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(387, 368);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(306, 368);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnPixelValueMapping
            // 
            this.btnPixelValueMapping.Location = new System.Drawing.Point(15, 371);
            this.btnPixelValueMapping.Name = "btnPixelValueMapping";
            this.btnPixelValueMapping.Size = new System.Drawing.Size(123, 23);
            this.btnPixelValueMapping.TabIndex = 23;
            this.btnPixelValueMapping.Text = "Pixel Value Mapping";
            this.btnPixelValueMapping.UseVisualStyleBackColor = true;
            this.btnPixelValueMapping.Click += new System.EventHandler(this.btnPixelValueMapping_Click);
            // 
            // frmChooseTimeHeaders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 406);
            this.Controls.Add(this.btnPixelValueMapping);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rbStartEndTimestamp);
            this.Controls.Add(this.rbTimeDuration);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmChooseTimeHeaders";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose FITS Time Headers";
            this.groupBox1.ResumeLayout(false);
            this.pnlExposure.ResumeLayout(false);
            this.pnlExposure.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxExposureWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxExposureOK)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbTimeDuration;
        private System.Windows.Forms.RadioButton rbStartEndTimestamp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbxExposureValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbxExposureUnits;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbxExposure;
        private System.Windows.Forms.Panel pnlExposure;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.PictureBox pbxExposureWarning;
        private System.Windows.Forms.PictureBox pbxExposureOK;
        private ucFitsTimeStampConfigurator ucTimestampControl2;
        private ucFitsTimeStampConfigurator ucTimestampControl;
        private System.Windows.Forms.Button btnPixelValueMapping;
    }
}