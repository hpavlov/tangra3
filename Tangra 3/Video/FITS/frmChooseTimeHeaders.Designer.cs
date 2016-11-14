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
            this.cbxTimeStamp = new System.Windows.Forms.ComboBox();
            this.rbTimeDuration = new System.Windows.Forms.RadioButton();
            this.rbStartEndTimestamp = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pbxTimeStampWarning = new System.Windows.Forms.PictureBox();
            this.cbxTimeStampFormat = new System.Windows.Forms.ComboBox();
            this.pbxTimeStampOK = new System.Windows.Forms.PictureBox();
            this.tbxTimeStampValue = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxTimestampType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlExposure = new System.Windows.Forms.Panel();
            this.pbxExposureWarning = new System.Windows.Forms.PictureBox();
            this.pbxExposureOK = new System.Windows.Forms.PictureBox();
            this.tbxExposureValue = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxExposure = new System.Windows.Forms.ComboBox();
            this.cbxExposureUnits = new System.Windows.Forms.ComboBox();
            this.pnlEndTimeStamp = new System.Windows.Forms.Panel();
            this.cbxTimeStamp2Format = new System.Windows.Forms.ComboBox();
            this.pbxTimeStamp2Warning = new System.Windows.Forms.PictureBox();
            this.tbxTimeStamp2Value = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.pbxTimeStamp2OK = new System.Windows.Forms.PictureBox();
            this.cbxTimestampType2 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cbxTimeStamp2 = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStampWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStampOK)).BeginInit();
            this.pnlExposure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxExposureWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxExposureOK)).BeginInit();
            this.pnlEndTimeStamp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStamp2Warning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStamp2OK)).BeginInit();
            this.SuspendLayout();
            // 
            // cbxTimeStamp
            // 
            this.cbxTimeStamp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimeStamp.FormattingEnabled = true;
            this.cbxTimeStamp.Location = new System.Drawing.Point(30, 42);
            this.cbxTimeStamp.Name = "cbxTimeStamp";
            this.cbxTimeStamp.Size = new System.Drawing.Size(102, 21);
            this.cbxTimeStamp.TabIndex = 0;
            this.cbxTimeStamp.SelectedIndexChanged += new System.EventHandler(this.cbxTimeStamp_SelectedIndexChanged);
            // 
            // rbTimeDuration
            // 
            this.rbTimeDuration.AutoSize = true;
            this.rbTimeDuration.Checked = true;
            this.rbTimeDuration.Location = new System.Drawing.Point(23, 22);
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
            this.rbStartEndTimestamp.Enabled = false;
            this.rbStartEndTimestamp.Location = new System.Drawing.Point(215, 22);
            this.rbStartEndTimestamp.Name = "rbStartEndTimestamp";
            this.rbStartEndTimestamp.Size = new System.Drawing.Size(137, 17);
            this.rbStartEndTimestamp.TabIndex = 2;
            this.rbStartEndTimestamp.Text = "Start + End Timestamps";
            this.rbStartEndTimestamp.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pbxTimeStampWarning);
            this.groupBox1.Controls.Add(this.cbxTimeStampFormat);
            this.groupBox1.Controls.Add(this.pbxTimeStampOK);
            this.groupBox1.Controls.Add(this.tbxTimeStampValue);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cbxTimestampType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbxTimeStamp);
            this.groupBox1.Controls.Add(this.pnlExposure);
            this.groupBox1.Controls.Add(this.pnlEndTimeStamp);
            this.groupBox1.Location = new System.Drawing.Point(23, 60);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(447, 234);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // pbxTimeStampWarning
            // 
            this.pbxTimeStampWarning.BackColor = System.Drawing.Color.Transparent;
            this.pbxTimeStampWarning.Image = ((System.Drawing.Image)(resources.GetObject("pbxTimeStampWarning.Image")));
            this.pbxTimeStampWarning.Location = new System.Drawing.Point(9, 44);
            this.pbxTimeStampWarning.Name = "pbxTimeStampWarning";
            this.pbxTimeStampWarning.Size = new System.Drawing.Size(16, 16);
            this.pbxTimeStampWarning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTimeStampWarning.TabIndex = 7;
            this.pbxTimeStampWarning.TabStop = false;
            // 
            // cbxTimeStampFormat
            // 
            this.cbxTimeStampFormat.FormattingEnabled = true;
            this.cbxTimeStampFormat.Items.AddRange(new object[] {
            "yyyy-MM-ddTHH:mm:ss.fff",
            "dd/MM/yyyy HH:mm:ss.fff"});
            this.cbxTimeStampFormat.Location = new System.Drawing.Point(138, 42);
            this.cbxTimeStampFormat.Name = "cbxTimeStampFormat";
            this.cbxTimeStampFormat.Size = new System.Drawing.Size(181, 21);
            this.cbxTimeStampFormat.TabIndex = 6;
            this.cbxTimeStampFormat.SelectedIndexChanged += new System.EventHandler(this.cbxTimeStampFormat_SelectedIndexChanged);
            // 
            // pbxTimeStampOK
            // 
            this.pbxTimeStampOK.BackColor = System.Drawing.Color.Transparent;
            this.pbxTimeStampOK.Image = ((System.Drawing.Image)(resources.GetObject("pbxTimeStampOK.Image")));
            this.pbxTimeStampOK.Location = new System.Drawing.Point(9, 44);
            this.pbxTimeStampOK.Name = "pbxTimeStampOK";
            this.pbxTimeStampOK.Size = new System.Drawing.Size(16, 16);
            this.pbxTimeStampOK.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbxTimeStampOK.TabIndex = 6;
            this.pbxTimeStampOK.TabStop = false;
            // 
            // tbxTimeStampValue
            // 
            this.tbxTimeStampValue.Location = new System.Drawing.Point(30, 69);
            this.tbxTimeStampValue.Multiline = true;
            this.tbxTimeStampValue.Name = "tbxTimeStampValue";
            this.tbxTimeStampValue.ReadOnly = true;
            this.tbxTimeStampValue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxTimeStampValue.Size = new System.Drawing.Size(403, 33);
            this.tbxTimeStampValue.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(322, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Type";
            // 
            // cbxTimestampType
            // 
            this.cbxTimestampType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimestampType.FormattingEnabled = true;
            this.cbxTimestampType.Items.AddRange(new object[] {
            "Start Exposure",
            "Middle Exposure",
            "End Exposure"});
            this.cbxTimestampType.Location = new System.Drawing.Point(325, 42);
            this.cbxTimestampType.Name = "cbxTimestampType";
            this.cbxTimestampType.Size = new System.Drawing.Size(108, 21);
            this.cbxTimestampType.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(135, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Format";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Timestamp";
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
            this.pnlExposure.Location = new System.Drawing.Point(7, 124);
            this.pnlExposure.Name = "pnlExposure";
            this.pnlExposure.Size = new System.Drawing.Size(434, 100);
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
            // pnlEndTimeStamp
            // 
            this.pnlEndTimeStamp.Controls.Add(this.cbxTimeStamp2Format);
            this.pnlEndTimeStamp.Controls.Add(this.pbxTimeStamp2Warning);
            this.pnlEndTimeStamp.Controls.Add(this.tbxTimeStamp2Value);
            this.pnlEndTimeStamp.Controls.Add(this.label6);
            this.pnlEndTimeStamp.Controls.Add(this.pbxTimeStamp2OK);
            this.pnlEndTimeStamp.Controls.Add(this.cbxTimestampType2);
            this.pnlEndTimeStamp.Controls.Add(this.label7);
            this.pnlEndTimeStamp.Controls.Add(this.label8);
            this.pnlEndTimeStamp.Controls.Add(this.cbxTimeStamp2);
            this.pnlEndTimeStamp.Location = new System.Drawing.Point(7, 124);
            this.pnlEndTimeStamp.Name = "pnlEndTimeStamp";
            this.pnlEndTimeStamp.Size = new System.Drawing.Size(434, 100);
            this.pnlEndTimeStamp.TabIndex = 5;
            this.pnlEndTimeStamp.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlEndTimeStamp_Paint);
            // 
            // cbxTimeStamp2Format
            // 
            this.cbxTimeStamp2Format.FormattingEnabled = true;
            this.cbxTimeStamp2Format.Items.AddRange(new object[] {
            "yyyy-MM-ddTHH:mm:ss.fff",
            "dd/MM/yyyy HH:mm:ss.fff"});
            this.cbxTimeStamp2Format.Location = new System.Drawing.Point(132, 31);
            this.cbxTimeStamp2Format.Name = "cbxTimeStamp2Format";
            this.cbxTimeStamp2Format.Size = new System.Drawing.Size(181, 21);
            this.cbxTimeStamp2Format.TabIndex = 7;
            this.cbxTimeStamp2Format.SelectedIndexChanged += new System.EventHandler(this.cbxTimeStamp2Format_SelectedIndexChanged);
            // 
            // pbxTimeStamp2Warning
            // 
            this.pbxTimeStamp2Warning.BackColor = System.Drawing.Color.Transparent;
            this.pbxTimeStamp2Warning.Image = ((System.Drawing.Image)(resources.GetObject("pbxTimeStamp2Warning.Image")));
            this.pbxTimeStamp2Warning.Location = new System.Drawing.Point(3, 33);
            this.pbxTimeStamp2Warning.Name = "pbxTimeStamp2Warning";
            this.pbxTimeStamp2Warning.Size = new System.Drawing.Size(16, 16);
            this.pbxTimeStamp2Warning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTimeStamp2Warning.TabIndex = 9;
            this.pbxTimeStamp2Warning.TabStop = false;
            // 
            // tbxTimeStamp2Value
            // 
            this.tbxTimeStamp2Value.Location = new System.Drawing.Point(24, 58);
            this.tbxTimeStamp2Value.Multiline = true;
            this.tbxTimeStamp2Value.Name = "tbxTimeStamp2Value";
            this.tbxTimeStamp2Value.ReadOnly = true;
            this.tbxTimeStamp2Value.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxTimeStamp2Value.Size = new System.Drawing.Size(403, 33);
            this.tbxTimeStamp2Value.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(316, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Type";
            // 
            // pbxTimeStamp2OK
            // 
            this.pbxTimeStamp2OK.BackColor = System.Drawing.Color.Transparent;
            this.pbxTimeStamp2OK.Image = ((System.Drawing.Image)(resources.GetObject("pbxTimeStamp2OK.Image")));
            this.pbxTimeStamp2OK.Location = new System.Drawing.Point(3, 33);
            this.pbxTimeStamp2OK.Name = "pbxTimeStamp2OK";
            this.pbxTimeStamp2OK.Size = new System.Drawing.Size(16, 16);
            this.pbxTimeStamp2OK.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbxTimeStamp2OK.TabIndex = 8;
            this.pbxTimeStamp2OK.TabStop = false;
            // 
            // cbxTimestampType2
            // 
            this.cbxTimestampType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimestampType2.FormattingEnabled = true;
            this.cbxTimestampType2.Items.AddRange(new object[] {
            "Start Exposure",
            "Middle Exposure",
            "End Exposure"});
            this.cbxTimestampType2.Location = new System.Drawing.Point(319, 31);
            this.cbxTimestampType2.Name = "cbxTimestampType2";
            this.cbxTimestampType2.Size = new System.Drawing.Size(108, 21);
            this.cbxTimestampType2.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(129, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Format";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(24, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(58, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Timestamp";
            // 
            // cbxTimeStamp2
            // 
            this.cbxTimeStamp2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimeStamp2.FormattingEnabled = true;
            this.cbxTimeStamp2.Location = new System.Drawing.Point(24, 31);
            this.cbxTimeStamp2.Name = "cbxTimeStamp2";
            this.cbxTimeStamp2.Size = new System.Drawing.Size(102, 21);
            this.cbxTimeStamp2.TabIndex = 8;
            this.cbxTimeStamp2.SelectedIndexChanged += new System.EventHandler(this.cbxTimeStamp2_SelectedIndexChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(395, 306);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(314, 306);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmChooseTimeHeaders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 343);
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
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStampWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStampOK)).EndInit();
            this.pnlExposure.ResumeLayout(false);
            this.pnlExposure.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxExposureWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxExposureOK)).EndInit();
            this.pnlEndTimeStamp.ResumeLayout(false);
            this.pnlEndTimeStamp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStamp2Warning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStamp2OK)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxTimeStamp;
        private System.Windows.Forms.RadioButton rbTimeDuration;
        private System.Windows.Forms.RadioButton rbStartEndTimestamp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxTimeStampValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxTimestampType;
        private System.Windows.Forms.TextBox tbxExposureValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbxExposureUnits;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbxExposure;
        private System.Windows.Forms.Panel pnlExposure;
        private System.Windows.Forms.Panel pnlEndTimeStamp;
        private System.Windows.Forms.TextBox tbxTimeStamp2Value;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbxTimestampType2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbxTimeStamp2;
        private System.Windows.Forms.ComboBox cbxTimeStampFormat;
        private System.Windows.Forms.ComboBox cbxTimeStamp2Format;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.PictureBox pbxExposureWarning;
        private System.Windows.Forms.PictureBox pbxExposureOK;
        private System.Windows.Forms.PictureBox pbxTimeStampOK;
        private System.Windows.Forms.PictureBox pbxTimeStampWarning;
        private System.Windows.Forms.PictureBox pbxTimeStamp2Warning;
        private System.Windows.Forms.PictureBox pbxTimeStamp2OK;
    }
}