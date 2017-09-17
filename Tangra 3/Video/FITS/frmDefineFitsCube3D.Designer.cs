namespace Tangra.Video.FITS
{
    partial class frmDefineFitsCube3D
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDefineFitsCube3D));
            this.cbxNaxisOrder = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.lblFrames = new System.Windows.Forms.Label();
            this.pnlExposure = new System.Windows.Forms.Panel();
            this.tbxExposureValue = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbxExposure = new System.Windows.Forms.ComboBox();
            this.cbxExposureUnits = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pbxExposureWarning = new System.Windows.Forms.PictureBox();
            this.pbxExposureOK = new System.Windows.Forms.PictureBox();
            this.ucTimestampControl = new Tangra.Video.FITS.ucFitsTimeStampConfigurator();
            this.ucNegativePixelsTreatment = new Tangra.Video.FITS.ucNegativePixelsTreatment();
            this.pnlExposure.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxExposureWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxExposureOK)).BeginInit();
            this.SuspendLayout();
            // 
            // cbxNaxisOrder
            // 
            this.cbxNaxisOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxNaxisOrder.FormattingEnabled = true;
            this.cbxNaxisOrder.Items.AddRange(new object[] {
            "Frames - Height - Width",
            "Frames - Width - Height",
            "Width - Height - Frames",
            "Height - Width - Frames",
            "Width - Frames - Height",
            "Height - Frames - Width"});
            this.cbxNaxisOrder.Location = new System.Drawing.Point(90, 26);
            this.cbxNaxisOrder.Name = "cbxNaxisOrder";
            this.cbxNaxisOrder.Size = new System.Drawing.Size(158, 21);
            this.cbxNaxisOrder.TabIndex = 0;
            this.cbxNaxisOrder.SelectedIndexChanged += new System.EventHandler(this.cbxNaxisOrder_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "NAXIS Order:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(271, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Width:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(345, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Height:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(433, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Frames:";
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.ForeColor = System.Drawing.Color.Navy;
            this.lblWidth.Location = new System.Drawing.Point(307, 29);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(31, 13);
            this.lblWidth.TabIndex = 5;
            this.lblWidth.Text = "1000";
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.ForeColor = System.Drawing.Color.Navy;
            this.lblHeight.Location = new System.Drawing.Point(382, 29);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(31, 13);
            this.lblHeight.TabIndex = 6;
            this.lblHeight.Text = "1000";
            // 
            // lblFrames
            // 
            this.lblFrames.AutoSize = true;
            this.lblFrames.ForeColor = System.Drawing.Color.Navy;
            this.lblFrames.Location = new System.Drawing.Point(474, 29);
            this.lblFrames.Name = "lblFrames";
            this.lblFrames.Size = new System.Drawing.Size(31, 13);
            this.lblFrames.TabIndex = 7;
            this.lblFrames.Text = "1000";
            // 
            // pnlExposure
            // 
            this.pnlExposure.Controls.Add(this.pbxExposureWarning);
            this.pnlExposure.Controls.Add(this.pbxExposureOK);
            this.pnlExposure.Controls.Add(this.tbxExposureValue);
            this.pnlExposure.Controls.Add(this.label5);
            this.pnlExposure.Controls.Add(this.label6);
            this.pnlExposure.Controls.Add(this.cbxExposure);
            this.pnlExposure.Controls.Add(this.cbxExposureUnits);
            this.pnlExposure.Location = new System.Drawing.Point(31, 156);
            this.pnlExposure.Name = "pnlExposure";
            this.pnlExposure.Size = new System.Drawing.Size(434, 100);
            this.pnlExposure.TabIndex = 8;
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(128, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Units";
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
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(349, 364);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 19;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(430, 364);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ucTimestampControl);
            this.groupBox1.Controls.Add(this.pnlExposure);
            this.groupBox1.Location = new System.Drawing.Point(17, 87);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(488, 271);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
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
            // ucTimestampControl
            // 
            this.ucTimestampControl.Location = new System.Drawing.Point(30, 15);
            this.ucTimestampControl.Name = "ucTimestampControl";
            this.ucTimestampControl.Size = new System.Drawing.Size(443, 134);
            this.ucTimestampControl.TabIndex = 9;
            // 
            // ucNegativePixelsTreatment
            // 
            this.ucNegativePixelsTreatment.Location = new System.Drawing.Point(11, 53);
            this.ucNegativePixelsTreatment.Name = "ucNegativePixelsTreatment";
            this.ucNegativePixelsTreatment.Size = new System.Drawing.Size(389, 33);
            this.ucNegativePixelsTreatment.TabIndex = 21;
            this.ucNegativePixelsTreatment.Visible = false;
            // 
            // frmDefineFitsCube3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 399);
            this.Controls.Add(this.ucNegativePixelsTreatment);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblFrames);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.lblWidth);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbxNaxisOrder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmDefineFitsCube3D";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Define 3D FITS Cube";
            this.pnlExposure.ResumeLayout(false);
            this.pnlExposure.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxExposureWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxExposureOK)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxNaxisOrder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.Label lblFrames;
        private System.Windows.Forms.Panel pnlExposure;
        private System.Windows.Forms.PictureBox pbxExposureWarning;
        private System.Windows.Forms.PictureBox pbxExposureOK;
        private System.Windows.Forms.TextBox tbxExposureValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbxExposure;
        private System.Windows.Forms.ComboBox cbxExposureUnits;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private ucFitsTimeStampConfigurator ucTimestampControl;
        private ucNegativePixelsTreatment ucNegativePixelsTreatment;
    }
}