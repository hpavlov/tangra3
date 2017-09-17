namespace Tangra.Video.FITS
{
    partial class ucFitsTimeStampConfigurator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucFitsTimeStampConfigurator));
            this.cbxSecondCard = new System.Windows.Forms.ComboBox();
            this.cbxSecondCardFormat = new System.Windows.Forms.ComboBox();
            this.cbxSecondCardType = new System.Windows.Forms.ComboBox();
            this.rbSeparateDateTime = new System.Windows.Forms.RadioButton();
            this.rbSingleTimestamp = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cbxTimeStamp = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbxTimeStampFormat = new System.Windows.Forms.ComboBox();
            this.cbxTimestampType = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbxTimeStampValue = new System.Windows.Forms.TextBox();
            this.pbxTimeStamp2Warning = new System.Windows.Forms.PictureBox();
            this.pbxTimeStamp2OK = new System.Windows.Forms.PictureBox();
            this.pbxTimeStampWarning = new System.Windows.Forms.PictureBox();
            this.pbxTimeStampOK = new System.Windows.Forms.PictureBox();
            this.pnlSecondCard = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStamp2Warning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStamp2OK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStampWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStampOK)).BeginInit();
            this.pnlSecondCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxSecondCard
            // 
            this.cbxSecondCard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSecondCard.FormattingEnabled = true;
            this.cbxSecondCard.Location = new System.Drawing.Point(23, 2);
            this.cbxSecondCard.Name = "cbxSecondCard";
            this.cbxSecondCard.Size = new System.Drawing.Size(102, 21);
            this.cbxSecondCard.TabIndex = 39;
            this.cbxSecondCard.SelectedIndexChanged += new System.EventHandler(this.cbxSecondCard_SelectedIndexChanged);
            // 
            // cbxSecondCardFormat
            // 
            this.cbxSecondCardFormat.FormattingEnabled = true;
            this.cbxSecondCardFormat.Items.AddRange(new object[] {
            "yyyy-MM-ddTHH:mm:ss.fff",
            "dd/MM/yyyy HH:mm:ss.fff"});
            this.cbxSecondCardFormat.Location = new System.Drawing.Point(131, 2);
            this.cbxSecondCardFormat.Name = "cbxSecondCardFormat";
            this.cbxSecondCardFormat.Size = new System.Drawing.Size(181, 21);
            this.cbxSecondCardFormat.TabIndex = 41;
            this.cbxSecondCardFormat.SelectedIndexChanged += new System.EventHandler(this.cbxSecondCardFormat_SelectedIndexChanged);
            // 
            // cbxSecondCardType
            // 
            this.cbxSecondCardType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSecondCardType.Enabled = false;
            this.cbxSecondCardType.FormattingEnabled = true;
            this.cbxSecondCardType.Items.AddRange(new object[] {
            "Start Exposure",
            "Middle Exposure",
            "End Exposure"});
            this.cbxSecondCardType.Location = new System.Drawing.Point(318, 2);
            this.cbxSecondCardType.Name = "cbxSecondCardType";
            this.cbxSecondCardType.Size = new System.Drawing.Size(108, 21);
            this.cbxSecondCardType.TabIndex = 40;
            // 
            // rbSeparateDateTime
            // 
            this.rbSeparateDateTime.AutoSize = true;
            this.rbSeparateDateTime.Location = new System.Drawing.Point(215, -1);
            this.rbSeparateDateTime.Name = "rbSeparateDateTime";
            this.rbSeparateDateTime.Size = new System.Drawing.Size(141, 17);
            this.rbSeparateDateTime.TabIndex = 38;
            this.rbSeparateDateTime.Text = "Separate Date and Time";
            this.rbSeparateDateTime.UseVisualStyleBackColor = true;
            this.rbSeparateDateTime.CheckedChanged += new System.EventHandler(this.rbSeparateDateTime_CheckedChanged);
            // 
            // rbSingleTimestamp
            // 
            this.rbSingleTimestamp.AutoSize = true;
            this.rbSingleTimestamp.Checked = true;
            this.rbSingleTimestamp.Location = new System.Drawing.Point(101, -1);
            this.rbSingleTimestamp.Name = "rbSingleTimestamp";
            this.rbSingleTimestamp.Size = new System.Drawing.Size(108, 17);
            this.rbSingleTimestamp.TabIndex = 37;
            this.rbSingleTimestamp.TabStop = true;
            this.rbSingleTimestamp.Text = "Single Timestamp";
            this.rbSingleTimestamp.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(28, 1);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 13);
            this.label10.TabIndex = 36;
            this.label10.Text = "Composition:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(25, 31);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "Date / Time ";
            // 
            // cbxTimeStamp
            // 
            this.cbxTimeStamp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimeStamp.FormattingEnabled = true;
            this.cbxTimeStamp.Location = new System.Drawing.Point(25, 50);
            this.cbxTimeStamp.Name = "cbxTimeStamp";
            this.cbxTimeStamp.Size = new System.Drawing.Size(102, 21);
            this.cbxTimeStamp.TabIndex = 27;
            this.cbxTimeStamp.SelectedIndexChanged += new System.EventHandler(this.cbxTimeStamp_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(130, 31);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(39, 13);
            this.label8.TabIndex = 29;
            this.label8.Text = "Format";
            // 
            // cbxTimeStampFormat
            // 
            this.cbxTimeStampFormat.FormattingEnabled = true;
            this.cbxTimeStampFormat.Items.AddRange(new object[] {
            "yyyy-MM-ddTHH:mm:ss.fff",
            "dd/MM/yyyy HH:mm:ss.fff"});
            this.cbxTimeStampFormat.Location = new System.Drawing.Point(133, 50);
            this.cbxTimeStampFormat.Name = "cbxTimeStampFormat";
            this.cbxTimeStampFormat.Size = new System.Drawing.Size(181, 21);
            this.cbxTimeStampFormat.TabIndex = 31;
            this.cbxTimeStampFormat.SelectedIndexChanged += new System.EventHandler(this.cbxTimeStampFormat_SelectedIndexChanged);
            // 
            // cbxTimestampType
            // 
            this.cbxTimestampType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimestampType.FormattingEnabled = true;
            this.cbxTimestampType.Items.AddRange(new object[] {
            "Start Exposure",
            "Middle Exposure",
            "End Exposure"});
            this.cbxTimestampType.Location = new System.Drawing.Point(320, 50);
            this.cbxTimestampType.Name = "cbxTimestampType";
            this.cbxTimestampType.Size = new System.Drawing.Size(108, 21);
            this.cbxTimestampType.TabIndex = 30;
            this.cbxTimestampType.SelectedIndexChanged += new System.EventHandler(this.cbxTimestampType_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(317, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 13);
            this.label7.TabIndex = 33;
            this.label7.Text = "Type";
            // 
            // tbxTimeStampValue
            // 
            this.tbxTimeStampValue.Location = new System.Drawing.Point(25, 98);
            this.tbxTimeStampValue.Multiline = true;
            this.tbxTimeStampValue.Name = "tbxTimeStampValue";
            this.tbxTimeStampValue.ReadOnly = true;
            this.tbxTimeStampValue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxTimeStampValue.Size = new System.Drawing.Size(403, 33);
            this.tbxTimeStampValue.TabIndex = 35;
            // 
            // pbxTimeStamp2Warning
            // 
            this.pbxTimeStamp2Warning.BackColor = System.Drawing.Color.Transparent;
            this.pbxTimeStamp2Warning.Image = ((System.Drawing.Image)(resources.GetObject("pbxTimeStamp2Warning.Image")));
            this.pbxTimeStamp2Warning.Location = new System.Drawing.Point(2, 4);
            this.pbxTimeStamp2Warning.Name = "pbxTimeStamp2Warning";
            this.pbxTimeStamp2Warning.Size = new System.Drawing.Size(16, 16);
            this.pbxTimeStamp2Warning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTimeStamp2Warning.TabIndex = 43;
            this.pbxTimeStamp2Warning.TabStop = false;
            // 
            // pbxTimeStamp2OK
            // 
            this.pbxTimeStamp2OK.BackColor = System.Drawing.Color.Transparent;
            this.pbxTimeStamp2OK.Image = ((System.Drawing.Image)(resources.GetObject("pbxTimeStamp2OK.Image")));
            this.pbxTimeStamp2OK.Location = new System.Drawing.Point(2, 4);
            this.pbxTimeStamp2OK.Name = "pbxTimeStamp2OK";
            this.pbxTimeStamp2OK.Size = new System.Drawing.Size(16, 16);
            this.pbxTimeStamp2OK.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbxTimeStamp2OK.TabIndex = 42;
            this.pbxTimeStamp2OK.TabStop = false;
            // 
            // pbxTimeStampWarning
            // 
            this.pbxTimeStampWarning.BackColor = System.Drawing.Color.Transparent;
            this.pbxTimeStampWarning.Image = ((System.Drawing.Image)(resources.GetObject("pbxTimeStampWarning.Image")));
            this.pbxTimeStampWarning.Location = new System.Drawing.Point(4, 52);
            this.pbxTimeStampWarning.Name = "pbxTimeStampWarning";
            this.pbxTimeStampWarning.Size = new System.Drawing.Size(16, 16);
            this.pbxTimeStampWarning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxTimeStampWarning.TabIndex = 34;
            this.pbxTimeStampWarning.TabStop = false;
            // 
            // pbxTimeStampOK
            // 
            this.pbxTimeStampOK.BackColor = System.Drawing.Color.Transparent;
            this.pbxTimeStampOK.Image = ((System.Drawing.Image)(resources.GetObject("pbxTimeStampOK.Image")));
            this.pbxTimeStampOK.Location = new System.Drawing.Point(4, 52);
            this.pbxTimeStampOK.Name = "pbxTimeStampOK";
            this.pbxTimeStampOK.Size = new System.Drawing.Size(16, 16);
            this.pbxTimeStampOK.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbxTimeStampOK.TabIndex = 32;
            this.pbxTimeStampOK.TabStop = false;
            // 
            // pnlSecondCard
            // 
            this.pnlSecondCard.Controls.Add(this.cbxSecondCard);
            this.pnlSecondCard.Controls.Add(this.pbxTimeStamp2Warning);
            this.pnlSecondCard.Controls.Add(this.cbxSecondCardType);
            this.pnlSecondCard.Controls.Add(this.cbxSecondCardFormat);
            this.pnlSecondCard.Controls.Add(this.pbxTimeStamp2OK);
            this.pnlSecondCard.Location = new System.Drawing.Point(2, 72);
            this.pnlSecondCard.Name = "pnlSecondCard";
            this.pnlSecondCard.Size = new System.Drawing.Size(435, 25);
            this.pnlSecondCard.TabIndex = 44;
            // 
            // ucFitsTimeStampConfigurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlSecondCard);
            this.Controls.Add(this.rbSeparateDateTime);
            this.Controls.Add(this.rbSingleTimestamp);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cbxTimeStamp);
            this.Controls.Add(this.pbxTimeStampWarning);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cbxTimeStampFormat);
            this.Controls.Add(this.cbxTimestampType);
            this.Controls.Add(this.pbxTimeStampOK);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbxTimeStampValue);
            this.Name = "ucFitsTimeStampConfigurator";
            this.Size = new System.Drawing.Size(443, 134);
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStamp2Warning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStamp2OK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStampWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTimeStampOK)).EndInit();
            this.pnlSecondCard.ResumeLayout(false);
            this.pnlSecondCard.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxSecondCard;
        private System.Windows.Forms.PictureBox pbxTimeStamp2Warning;
        private System.Windows.Forms.ComboBox cbxSecondCardFormat;
        private System.Windows.Forms.ComboBox cbxSecondCardType;
        private System.Windows.Forms.PictureBox pbxTimeStamp2OK;
        private System.Windows.Forms.RadioButton rbSeparateDateTime;
        private System.Windows.Forms.RadioButton rbSingleTimestamp;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbxTimeStamp;
        private System.Windows.Forms.PictureBox pbxTimeStampWarning;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbxTimeStampFormat;
        private System.Windows.Forms.ComboBox cbxTimestampType;
        private System.Windows.Forms.PictureBox pbxTimeStampOK;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbxTimeStampValue;
        private System.Windows.Forms.Panel pnlSecondCard;
    }
}
