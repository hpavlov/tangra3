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
			this.gbxVariable = new System.Windows.Forms.GroupBox();
			this.rbVar4 = new System.Windows.Forms.RadioButton();
			this.rbVar3 = new System.Windows.Forms.RadioButton();
			this.rbVar2 = new System.Windows.Forms.RadioButton();
			this.rbVar1 = new System.Windows.Forms.RadioButton();
			this.gbxComparison = new System.Windows.Forms.GroupBox();
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
			this.tbarFrom = new System.Windows.Forms.TrackBar();
			this.gbxVariable.SuspendLayout();
			this.gbxComparison.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbarTo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picHistogram)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbarFrom)).BeginInit();
			this.SuspendLayout();
			// 
			// gbxVariable
			// 
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
			// rbVar4
			// 
			this.rbVar4.AutoSize = true;
			this.rbVar4.Location = new System.Drawing.Point(19, 89);
			this.rbVar4.Name = "rbVar4";
			this.rbVar4.Size = new System.Drawing.Size(65, 17);
			this.rbVar4.TabIndex = 3;
			this.rbVar4.Tag = "3";
			this.rbVar4.Text = "Object 4";
			this.rbVar4.UseVisualStyleBackColor = true;
			// 
			// rbVar3
			// 
			this.rbVar3.AutoSize = true;
			this.rbVar3.Location = new System.Drawing.Point(19, 66);
			this.rbVar3.Name = "rbVar3";
			this.rbVar3.Size = new System.Drawing.Size(65, 17);
			this.rbVar3.TabIndex = 2;
			this.rbVar3.Tag = "2";
			this.rbVar3.Text = "Object 3";
			this.rbVar3.UseVisualStyleBackColor = true;
			// 
			// rbVar2
			// 
			this.rbVar2.AutoSize = true;
			this.rbVar2.Location = new System.Drawing.Point(19, 43);
			this.rbVar2.Name = "rbVar2";
			this.rbVar2.Size = new System.Drawing.Size(65, 17);
			this.rbVar2.TabIndex = 1;
			this.rbVar2.Tag = "1";
			this.rbVar2.Text = "Object 2";
			this.rbVar2.UseVisualStyleBackColor = true;
			// 
			// rbVar1
			// 
			this.rbVar1.AutoSize = true;
			this.rbVar1.Checked = true;
			this.rbVar1.Location = new System.Drawing.Point(19, 20);
			this.rbVar1.Name = "rbVar1";
			this.rbVar1.Size = new System.Drawing.Size(65, 17);
			this.rbVar1.TabIndex = 0;
			this.rbVar1.TabStop = true;
			this.rbVar1.Tag = "0";
			this.rbVar1.Text = "Object 1";
			this.rbVar1.UseVisualStyleBackColor = true;
			// 
			// gbxComparison
			// 
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
			// rbComp4
			// 
			this.rbComp4.AutoSize = true;
			this.rbComp4.Location = new System.Drawing.Point(19, 89);
			this.rbComp4.Name = "rbComp4";
			this.rbComp4.Size = new System.Drawing.Size(65, 17);
			this.rbComp4.TabIndex = 3;
			this.rbComp4.Tag = "3";
			this.rbComp4.Text = "Object 4";
			this.rbComp4.UseVisualStyleBackColor = true;
			// 
			// rbComp3
			// 
			this.rbComp3.AutoSize = true;
			this.rbComp3.Location = new System.Drawing.Point(19, 66);
			this.rbComp3.Name = "rbComp3";
			this.rbComp3.Size = new System.Drawing.Size(65, 17);
			this.rbComp3.TabIndex = 2;
			this.rbComp3.Tag = "2";
			this.rbComp3.Text = "Object 3";
			this.rbComp3.UseVisualStyleBackColor = true;
			// 
			// rbComp2
			// 
			this.rbComp2.AutoSize = true;
			this.rbComp2.Checked = true;
			this.rbComp2.Location = new System.Drawing.Point(19, 43);
			this.rbComp2.Name = "rbComp2";
			this.rbComp2.Size = new System.Drawing.Size(65, 17);
			this.rbComp2.TabIndex = 1;
			this.rbComp2.TabStop = true;
			this.rbComp2.Tag = "1";
			this.rbComp2.Text = "Object 2";
			this.rbComp2.UseVisualStyleBackColor = true;
			// 
			// rbComp1
			// 
			this.rbComp1.AutoSize = true;
			this.rbComp1.Location = new System.Drawing.Point(19, 20);
			this.rbComp1.Name = "rbComp1";
			this.rbComp1.Size = new System.Drawing.Size(65, 17);
			this.rbComp1.TabIndex = 0;
			this.rbComp1.Tag = "0";
			this.rbComp1.Text = "Object 1";
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
			this.tbarTo.Size = new System.Drawing.Size(525, 45);
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
			// tbarFrom
			// 
			this.tbarFrom.LargeChange = 100;
			this.tbarFrom.Location = new System.Drawing.Point(1, 197);
			this.tbarFrom.Maximum = 256;
			this.tbarFrom.Name = "tbarFrom";
			this.tbarFrom.Size = new System.Drawing.Size(525, 45);
			this.tbarFrom.SmallChange = 100;
			this.tbarFrom.TabIndex = 18;
			this.tbarFrom.ValueChanged += new System.EventHandler(this.DynamicRangeChanged);
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
			// frmConfigureRun
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(528, 412);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.pnlSelectedRange);
			this.Controls.Add(this.pnlBackground);
			this.Controls.Add(this.tbarFrom);
			this.Controls.Add(this.tbarTo);
			this.Controls.Add(this.picHistogram);
			this.Controls.Add(this.cbxNormalisation);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.gbxComparison);
			this.Controls.Add(this.gbxVariable);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmConfigureRun";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Configure Reduction";
			this.Load += new System.EventHandler(this.frmConfigureRun_Load);
			this.gbxVariable.ResumeLayout(false);
			this.gbxVariable.PerformLayout();
			this.gbxComparison.ResumeLayout(false);
			this.gbxComparison.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbarTo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picHistogram)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbarFrom)).EndInit();
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
    }
}