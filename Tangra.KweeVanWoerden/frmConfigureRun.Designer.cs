namespace Tangra.KweeVanWoerden
{
    partial class frmConfigureRun
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfigureRun));
            this.gbxVariable = new System.Windows.Forms.GroupBox();
            this.pb4 = new System.Windows.Forms.PictureBox();
            this.pb3 = new System.Windows.Forms.PictureBox();
            this.pb2 = new System.Windows.Forms.PictureBox();
            this.pb1 = new System.Windows.Forms.PictureBox();
            this.rbVar4 = new System.Windows.Forms.RadioButton();
            this.rbVar3 = new System.Windows.Forms.RadioButton();
            this.rbVar2 = new System.Windows.Forms.RadioButton();
            this.rbVar1 = new System.Windows.Forms.RadioButton();
            this.gbxComparison = new System.Windows.Forms.GroupBox();
            this.pb4c = new System.Windows.Forms.PictureBox();
            this.pb3c = new System.Windows.Forms.PictureBox();
            this.pb2c = new System.Windows.Forms.PictureBox();
            this.pb1c = new System.Windows.Forms.PictureBox();
            this.rbComp4 = new System.Windows.Forms.RadioButton();
            this.rbComp3 = new System.Windows.Forms.RadioButton();
            this.rbComp2 = new System.Windows.Forms.RadioButton();
            this.rbComp1 = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.cbxNormalisation = new System.Windows.Forms.CheckBox();
            this.pnlSelectedRange = new System.Windows.Forms.Panel();
            this.pnlBackground = new System.Windows.Forms.Panel();
            this.tbarTo = new System.Windows.Forms.TrackBar();
            this.picHistogram = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbsKvW = new System.Windows.Forms.CheckBox();
            this.cbxCurveFitting = new System.Windows.Forms.CheckBox();
            tbarFrom = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(tbarFrom)).BeginInit();
            this.gbxVariable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).BeginInit();
            this.gbxComparison.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb4c)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb3c)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb2c)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb1c)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHistogram)).BeginInit();
            this.SuspendLayout();
            // 
            // tbarFrom
            // 
            tbarFrom.LargeChange = 100;
            tbarFrom.Location = new System.Drawing.Point(1, 197);
            tbarFrom.Maximum = 256;
            tbarFrom.Name = "tbarFrom";
            tbarFrom.Size = new System.Drawing.Size(525, 42);
            tbarFrom.SmallChange = 100;
            tbarFrom.TabIndex = 18;
            tbarFrom.ValueChanged += new System.EventHandler(this.DynamicRangeChanged);
            // 
            // gbxVariable
            // 
            this.gbxVariable.Controls.Add(this.pb4);
            this.gbxVariable.Controls.Add(this.pb3);
            this.gbxVariable.Controls.Add(this.pb2);
            this.gbxVariable.Controls.Add(this.pb1);
            this.gbxVariable.Controls.Add(this.rbVar4);
            this.gbxVariable.Controls.Add(this.rbVar3);
            this.gbxVariable.Controls.Add(this.rbVar2);
            this.gbxVariable.Controls.Add(this.rbVar1);
            this.gbxVariable.Location = new System.Drawing.Point(15, 287);
            this.gbxVariable.Name = "gbxVariable";
            this.gbxVariable.Size = new System.Drawing.Size(142, 115);
            this.gbxVariable.TabIndex = 0;
            this.gbxVariable.TabStop = false;
            this.gbxVariable.Text = "Variable Star";
            // 
            // pb4
            // 
            this.pb4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb4.BackColor = System.Drawing.Color.Maroon;
            this.pb4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb4.Location = new System.Drawing.Point(35, 91);
            this.pb4.Name = "pb4";
            this.pb4.Size = new System.Drawing.Size(10, 11);
            this.pb4.TabIndex = 23;
            this.pb4.TabStop = false;
            // 
            // pb3
            // 
            this.pb3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb3.BackColor = System.Drawing.Color.Maroon;
            this.pb3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb3.Location = new System.Drawing.Point(35, 68);
            this.pb3.Name = "pb3";
            this.pb3.Size = new System.Drawing.Size(10, 11);
            this.pb3.TabIndex = 23;
            this.pb3.TabStop = false;
            // 
            // pb2
            // 
            this.pb2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb2.BackColor = System.Drawing.Color.Maroon;
            this.pb2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb2.Location = new System.Drawing.Point(35, 46);
            this.pb2.Name = "pb2";
            this.pb2.Size = new System.Drawing.Size(10, 11);
            this.pb2.TabIndex = 23;
            this.pb2.TabStop = false;
            // 
            // pb1
            // 
            this.pb1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb1.BackColor = System.Drawing.Color.Maroon;
            this.pb1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb1.Location = new System.Drawing.Point(35, 22);
            this.pb1.Name = "pb1";
            this.pb1.Size = new System.Drawing.Size(10, 11);
            this.pb1.TabIndex = 22;
            this.pb1.TabStop = false;
            // 
            // rbVar4
            // 
            this.rbVar4.AutoSize = true;
            this.rbVar4.Location = new System.Drawing.Point(19, 89);
            this.rbVar4.Name = "rbVar4";
            this.rbVar4.Size = new System.Drawing.Size(77, 17);
            this.rbVar4.TabIndex = 3;
            this.rbVar4.Tag = "3";
            this.rbVar4.Text = "    Object 4";
            this.rbVar4.UseVisualStyleBackColor = true;
            // 
            // rbVar3
            // 
            this.rbVar3.AutoSize = true;
            this.rbVar3.Location = new System.Drawing.Point(19, 66);
            this.rbVar3.Name = "rbVar3";
            this.rbVar3.Size = new System.Drawing.Size(77, 17);
            this.rbVar3.TabIndex = 2;
            this.rbVar3.Tag = "2";
            this.rbVar3.Text = "    Object 3";
            this.rbVar3.UseVisualStyleBackColor = true;
            // 
            // rbVar2
            // 
            this.rbVar2.AutoSize = true;
            this.rbVar2.Location = new System.Drawing.Point(19, 43);
            this.rbVar2.Name = "rbVar2";
            this.rbVar2.Size = new System.Drawing.Size(77, 17);
            this.rbVar2.TabIndex = 1;
            this.rbVar2.Tag = "1";
            this.rbVar2.Text = "    Object 2";
            this.rbVar2.UseVisualStyleBackColor = true;
            // 
            // rbVar1
            // 
            this.rbVar1.AutoSize = true;
            this.rbVar1.Checked = true;
            this.rbVar1.Location = new System.Drawing.Point(19, 20);
            this.rbVar1.Name = "rbVar1";
            this.rbVar1.Size = new System.Drawing.Size(77, 17);
            this.rbVar1.TabIndex = 0;
            this.rbVar1.TabStop = true;
            this.rbVar1.Tag = "0";
            this.rbVar1.Text = "    Object 1";
            this.rbVar1.UseVisualStyleBackColor = true;
            // 
            // gbxComparison
            // 
            this.gbxComparison.Controls.Add(this.pb4c);
            this.gbxComparison.Controls.Add(this.pb3c);
            this.gbxComparison.Controls.Add(this.pb2c);
            this.gbxComparison.Controls.Add(this.pb1c);
            this.gbxComparison.Controls.Add(this.rbComp4);
            this.gbxComparison.Controls.Add(this.rbComp3);
            this.gbxComparison.Controls.Add(this.rbComp2);
            this.gbxComparison.Controls.Add(this.rbComp1);
            this.gbxComparison.Location = new System.Drawing.Point(176, 287);
            this.gbxComparison.Name = "gbxComparison";
            this.gbxComparison.Size = new System.Drawing.Size(142, 115);
            this.gbxComparison.TabIndex = 1;
            this.gbxComparison.TabStop = false;
            this.gbxComparison.Text = "Comparison Star";
            // 
            // pb4c
            // 
            this.pb4c.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb4c.BackColor = System.Drawing.Color.Maroon;
            this.pb4c.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb4c.Location = new System.Drawing.Point(35, 91);
            this.pb4c.Name = "pb4c";
            this.pb4c.Size = new System.Drawing.Size(10, 11);
            this.pb4c.TabIndex = 23;
            this.pb4c.TabStop = false;
            // 
            // pb3c
            // 
            this.pb3c.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb3c.BackColor = System.Drawing.Color.Maroon;
            this.pb3c.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb3c.Location = new System.Drawing.Point(35, 68);
            this.pb3c.Name = "pb3c";
            this.pb3c.Size = new System.Drawing.Size(10, 11);
            this.pb3c.TabIndex = 23;
            this.pb3c.TabStop = false;
            // 
            // pb2c
            // 
            this.pb2c.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb2c.BackColor = System.Drawing.Color.Maroon;
            this.pb2c.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb2c.Location = new System.Drawing.Point(35, 46);
            this.pb2c.Name = "pb2c";
            this.pb2c.Size = new System.Drawing.Size(10, 11);
            this.pb2c.TabIndex = 23;
            this.pb2c.TabStop = false;
            // 
            // pb1c
            // 
            this.pb1c.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb1c.BackColor = System.Drawing.Color.Maroon;
            this.pb1c.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb1c.Location = new System.Drawing.Point(35, 22);
            this.pb1c.Name = "pb1c";
            this.pb1c.Size = new System.Drawing.Size(10, 11);
            this.pb1c.TabIndex = 23;
            this.pb1c.TabStop = false;
            // 
            // rbComp4
            // 
            this.rbComp4.AutoSize = true;
            this.rbComp4.Location = new System.Drawing.Point(19, 89);
            this.rbComp4.Name = "rbComp4";
            this.rbComp4.Size = new System.Drawing.Size(77, 17);
            this.rbComp4.TabIndex = 3;
            this.rbComp4.Tag = "3";
            this.rbComp4.Text = "    Object 4";
            this.rbComp4.UseVisualStyleBackColor = true;
            // 
            // rbComp3
            // 
            this.rbComp3.AutoSize = true;
            this.rbComp3.Location = new System.Drawing.Point(19, 66);
            this.rbComp3.Name = "rbComp3";
            this.rbComp3.Size = new System.Drawing.Size(77, 17);
            this.rbComp3.TabIndex = 2;
            this.rbComp3.Tag = "2";
            this.rbComp3.Text = "    Object 3";
            this.rbComp3.UseVisualStyleBackColor = true;
            // 
            // rbComp2
            // 
            this.rbComp2.AutoSize = true;
            this.rbComp2.Checked = true;
            this.rbComp2.Location = new System.Drawing.Point(19, 43);
            this.rbComp2.Name = "rbComp2";
            this.rbComp2.Size = new System.Drawing.Size(77, 17);
            this.rbComp2.TabIndex = 1;
            this.rbComp2.TabStop = true;
            this.rbComp2.Tag = "1";
            this.rbComp2.Text = "    Object 2";
            this.rbComp2.UseVisualStyleBackColor = true;
            // 
            // rbComp1
            // 
            this.rbComp1.AutoSize = true;
            this.rbComp1.Location = new System.Drawing.Point(19, 20);
            this.rbComp1.Name = "rbComp1";
            this.rbComp1.Size = new System.Drawing.Size(77, 17);
            this.rbComp1.TabIndex = 0;
            this.rbComp1.Tag = "0";
            this.rbComp1.Text = "    Object 1";
            this.rbComp1.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(340, 379);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cbxNormalisation
            // 
            this.cbxNormalisation.AutoSize = true;
            this.cbxNormalisation.Checked = true;
            this.cbxNormalisation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxNormalisation.Location = new System.Drawing.Point(340, 287);
            this.cbxNormalisation.Name = "cbxNormalisation";
            this.cbxNormalisation.Size = new System.Drawing.Size(111, 17);
            this.cbxNormalisation.TabIndex = 3;
            this.cbxNormalisation.Text = "Use Normalisation";
            this.cbxNormalisation.UseVisualStyleBackColor = true;
            // 
            // pnlSelectedRange
            // 
            this.pnlSelectedRange.BackColor = System.Drawing.Color.Lime;
            this.pnlSelectedRange.Location = new System.Drawing.Point(12, 229);
            this.pnlSelectedRange.Name = "pnlSelectedRange";
            this.pnlSelectedRange.Size = new System.Drawing.Size(504, 6);
            this.pnlSelectedRange.TabIndex = 19;
            // 
            // pnlBackground
            // 
            this.pnlBackground.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlBackground.Location = new System.Drawing.Point(15, 229);
            this.pnlBackground.Name = "pnlBackground";
            this.pnlBackground.Size = new System.Drawing.Size(501, 6);
            this.pnlBackground.TabIndex = 20;
            // 
            // tbarTo
            // 
            this.tbarTo.Location = new System.Drawing.Point(1, 236);
            this.tbarTo.Name = "tbarTo";
            this.tbarTo.Size = new System.Drawing.Size(525, 42);
            this.tbarTo.TabIndex = 17;
            this.tbarTo.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.tbarTo.ValueChanged += new System.EventHandler(this.DynamicRangeChanged);
            // 
            // picHistogram
            // 
            this.picHistogram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picHistogram.Location = new System.Drawing.Point(12, 12);
            this.picHistogram.Name = "picHistogram";
            this.picHistogram.Size = new System.Drawing.Size(504, 182);
            this.picHistogram.TabIndex = 16;
            this.picHistogram.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 150;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(431, 379);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 23);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cbsKvW
            // 
            this.cbsKvW.AutoSize = true;
            this.cbsKvW.Checked = true;
            this.cbsKvW.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbsKvW.Enabled = false;
            this.cbsKvW.Location = new System.Drawing.Point(340, 325);
            this.cbsKvW.Name = "cbsKvW";
            this.cbsKvW.Size = new System.Drawing.Size(147, 17);
            this.cbsKvW.TabIndex = 22;
            this.cbsKvW.Text = "Run Kwee-van Woerden ";
            this.cbsKvW.UseVisualStyleBackColor = true;
            this.cbsKvW.Visible = false;
            // 
            // cbxCurveFitting
            // 
            this.cbxCurveFitting.AutoSize = true;
            this.cbxCurveFitting.Location = new System.Drawing.Point(340, 345);
            this.cbxCurveFitting.Name = "cbxCurveFitting";
            this.cbxCurveFitting.Size = new System.Drawing.Size(108, 17);
            this.cbxCurveFitting.TabIndex = 23;
            this.cbxCurveFitting.Text = "Run Curve Fitting";
            this.cbxCurveFitting.UseVisualStyleBackColor = true;
            this.cbxCurveFitting.Visible = false;
            // 
            // frmConfigureRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 412);
            this.Controls.Add(this.cbxCurveFitting);
            this.Controls.Add(this.cbsKvW);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pnlSelectedRange);
            this.Controls.Add(this.pnlBackground);
            this.Controls.Add(tbarFrom);
            this.Controls.Add(this.tbarTo);
            this.Controls.Add(this.picHistogram);
            this.Controls.Add(this.cbxNormalisation);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbxComparison);
            this.Controls.Add(this.gbxVariable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfigureRun";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configure Reduction";
            this.Load += new System.EventHandler(this.frmConfigureRun_Load);
            ((System.ComponentModel.ISupportInitialize)(tbarFrom)).EndInit();
            this.gbxVariable.ResumeLayout(false);
            this.gbxVariable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).EndInit();
            this.gbxComparison.ResumeLayout(false);
            this.gbxComparison.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb4c)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb3c)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb2c)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb1c)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHistogram)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxVariable;
        private System.Windows.Forms.RadioButton rbVar4;
        private System.Windows.Forms.RadioButton rbVar3;
        private System.Windows.Forms.RadioButton rbVar2;
        private System.Windows.Forms.RadioButton rbVar1;
        private System.Windows.Forms.GroupBox gbxComparison;
        private System.Windows.Forms.RadioButton rbComp4;
        private System.Windows.Forms.RadioButton rbComp3;
        private System.Windows.Forms.RadioButton rbComp2;
        private System.Windows.Forms.RadioButton rbComp1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox cbxNormalisation;
		private System.Windows.Forms.Panel pnlSelectedRange;
        private System.Windows.Forms.Panel pnlBackground;
        private System.Windows.Forms.TrackBar tbarFrom;
        private System.Windows.Forms.TrackBar tbarTo;
		private System.Windows.Forms.PictureBox picHistogram;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pb4;
        private System.Windows.Forms.PictureBox pb3;
        private System.Windows.Forms.PictureBox pb2;
        private System.Windows.Forms.PictureBox pb1;
        private System.Windows.Forms.PictureBox pb4c;
        private System.Windows.Forms.PictureBox pb3c;
        private System.Windows.Forms.PictureBox pb2c;
        private System.Windows.Forms.PictureBox pb1c;
		private System.Windows.Forms.CheckBox cbsKvW;
		private System.Windows.Forms.CheckBox cbxCurveFitting;
    }
}