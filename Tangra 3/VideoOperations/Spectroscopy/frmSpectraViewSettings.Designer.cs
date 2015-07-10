namespace Tangra.VideoOperations.Spectroscopy
{
	partial class frmSpectraViewSettings
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
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.ucColorPickerBackground = new Tangra.Controls.ucColorPicker();
			this.label4 = new System.Windows.Forms.Label();
			this.ucColorPickerAperture = new Tangra.Controls.ucColorPicker();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.ucColorPickerGridLegend = new Tangra.Controls.ucColorPicker();
			this.label1 = new System.Windows.Forms.Label();
			this.ucColorPickerGridLines = new Tangra.Controls.ucColorPicker();
			this.ucColorPickerKnownLine = new Tangra.Controls.ucColorPicker();
			this.label39 = new System.Windows.Forms.Label();
			this.ucColorPickerReferenceStar = new Tangra.Controls.ucColorPicker();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.Location = new System.Drawing.Point(133, 252);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 30;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOk.Location = new System.Drawing.Point(52, 252);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 31;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.label5);
			this.groupBox6.Controls.Add(this.ucColorPickerBackground);
			this.groupBox6.Controls.Add(this.label4);
			this.groupBox6.Controls.Add(this.ucColorPickerAperture);
			this.groupBox6.Controls.Add(this.label3);
			this.groupBox6.Controls.Add(this.label2);
			this.groupBox6.Controls.Add(this.ucColorPickerGridLegend);
			this.groupBox6.Controls.Add(this.label1);
			this.groupBox6.Controls.Add(this.ucColorPickerGridLines);
			this.groupBox6.Controls.Add(this.ucColorPickerKnownLine);
			this.groupBox6.Controls.Add(this.label39);
			this.groupBox6.Controls.Add(this.ucColorPickerReferenceStar);
			this.groupBox6.Location = new System.Drawing.Point(19, 12);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(225, 224);
			this.groupBox6.TabIndex = 32;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Colours - Spectroscopy";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(68, 29);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(87, 13);
			this.label5.TabIndex = 11;
			this.label5.Text = "Grid Background";
			// 
			// ucColorPickerBackground
			// 
			this.ucColorPickerBackground.AutoSize = true;
			this.ucColorPickerBackground.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ucColorPickerBackground.BackColor = System.Drawing.SystemColors.Control;
			this.ucColorPickerBackground.Location = new System.Drawing.Point(6, 22);
			this.ucColorPickerBackground.Name = "ucColorPickerBackground";
			this.ucColorPickerBackground.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.ucColorPickerBackground.Size = new System.Drawing.Size(60, 26);
			this.ucColorPickerBackground.TabIndex = 10;
			this.ucColorPickerBackground.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(68, 125);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(87, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "Spectra Aperture";
			// 
			// ucColorPickerAperture
			// 
			this.ucColorPickerAperture.AutoSize = true;
			this.ucColorPickerAperture.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ucColorPickerAperture.BackColor = System.Drawing.SystemColors.Control;
			this.ucColorPickerAperture.Location = new System.Drawing.Point(6, 117);
			this.ucColorPickerAperture.Name = "ucColorPickerAperture";
			this.ucColorPickerAperture.SelectedColor = System.Drawing.Color.Red;
			this.ucColorPickerAperture.Size = new System.Drawing.Size(60, 26);
			this.ucColorPickerAperture.TabIndex = 8;
			this.ucColorPickerAperture.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(68, 187);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Grid Legend";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(68, 156);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Grid Lines";
			// 
			// ucColorPickerGridLegend
			// 
			this.ucColorPickerGridLegend.AutoSize = true;
			this.ucColorPickerGridLegend.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ucColorPickerGridLegend.BackColor = System.Drawing.SystemColors.Control;
			this.ucColorPickerGridLegend.Location = new System.Drawing.Point(6, 181);
			this.ucColorPickerGridLegend.Name = "ucColorPickerGridLegend";
			this.ucColorPickerGridLegend.SelectedColor = System.Drawing.Color.White;
			this.ucColorPickerGridLegend.Size = new System.Drawing.Size(60, 26);
			this.ucColorPickerGridLegend.TabIndex = 5;
			this.ucColorPickerGridLegend.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(68, 93);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(103, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Known Spectra Line";
			// 
			// ucColorPickerGridLines
			// 
			this.ucColorPickerGridLines.AutoSize = true;
			this.ucColorPickerGridLines.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ucColorPickerGridLines.BackColor = System.Drawing.SystemColors.Control;
			this.ucColorPickerGridLines.Location = new System.Drawing.Point(6, 149);
			this.ucColorPickerGridLines.Name = "ucColorPickerGridLines";
			this.ucColorPickerGridLines.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.ucColorPickerGridLines.Size = new System.Drawing.Size(60, 26);
			this.ucColorPickerGridLines.TabIndex = 3;
			this.ucColorPickerGridLines.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// ucColorPickerKnownLine
			// 
			this.ucColorPickerKnownLine.AutoSize = true;
			this.ucColorPickerKnownLine.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ucColorPickerKnownLine.BackColor = System.Drawing.SystemColors.Control;
			this.ucColorPickerKnownLine.Location = new System.Drawing.Point(6, 86);
			this.ucColorPickerKnownLine.Name = "ucColorPickerKnownLine";
			this.ucColorPickerKnownLine.SelectedColor = System.Drawing.Color.Blue;
			this.ucColorPickerKnownLine.Size = new System.Drawing.Size(60, 26);
			this.ucColorPickerKnownLine.TabIndex = 2;
			this.ucColorPickerKnownLine.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// label39
			// 
			this.label39.AutoSize = true;
			this.label39.Location = new System.Drawing.Point(68, 60);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(65, 13);
			this.label39.TabIndex = 1;
			this.label39.Text = "Spectra Plot";
			// 
			// ucColorPickerReferenceStar
			// 
			this.ucColorPickerReferenceStar.AutoSize = true;
			this.ucColorPickerReferenceStar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ucColorPickerReferenceStar.BackColor = System.Drawing.SystemColors.Control;
			this.ucColorPickerReferenceStar.Location = new System.Drawing.Point(6, 54);
			this.ucColorPickerReferenceStar.Name = "ucColorPickerReferenceStar";
			this.ucColorPickerReferenceStar.SelectedColor = System.Drawing.Color.Aqua;
			this.ucColorPickerReferenceStar.Size = new System.Drawing.Size(60, 26);
			this.ucColorPickerReferenceStar.TabIndex = 0;
			this.ucColorPickerReferenceStar.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// frmSpectraViewSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(262, 291);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSpectraViewSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Spectra Viewer Display Settings";
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label5;
		private Controls.ucColorPicker ucColorPickerBackground;
		private System.Windows.Forms.Label label4;
		private Controls.ucColorPicker ucColorPickerAperture;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private Controls.ucColorPicker ucColorPickerGridLegend;
		private System.Windows.Forms.Label label1;
		private Controls.ucColorPicker ucColorPickerGridLines;
		private Controls.ucColorPicker ucColorPickerKnownLine;
		private System.Windows.Forms.Label label39;
		private Controls.ucColorPicker ucColorPickerReferenceStar;
    }
}