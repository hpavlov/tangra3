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
			this.ucColorPickerReferenceStar = new Tangra.Controls.ucColorPicker();
			this.label39 = new System.Windows.Forms.Label();
			this.ucColorPickerKnownLine = new Tangra.Controls.ucColorPicker();
			this.ucColorPickerGridLines = new Tangra.Controls.ucColorPicker();
			this.label1 = new System.Windows.Forms.Label();
			this.ucColorPickerGridLegend = new Tangra.Controls.ucColorPicker();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(162, 213);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 30;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(81, 213);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 31;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.label3);
			this.groupBox6.Controls.Add(this.label2);
			this.groupBox6.Controls.Add(this.ucColorPickerGridLegend);
			this.groupBox6.Controls.Add(this.label1);
			this.groupBox6.Controls.Add(this.ucColorPickerGridLines);
			this.groupBox6.Controls.Add(this.ucColorPickerKnownLine);
			this.groupBox6.Controls.Add(this.label39);
			this.groupBox6.Controls.Add(this.ucColorPickerReferenceStar);
			this.groupBox6.Location = new System.Drawing.Point(12, 12);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(297, 178);
			this.groupBox6.TabIndex = 32;
			this.groupBox6.TabStop = false;
			// 
			// ucColorPickerReferenceStar
			// 
			this.ucColorPickerReferenceStar.AutoSize = true;
			this.ucColorPickerReferenceStar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ucColorPickerReferenceStar.BackColor = System.Drawing.SystemColors.Control;
			this.ucColorPickerReferenceStar.Location = new System.Drawing.Point(9, 33);
			this.ucColorPickerReferenceStar.Name = "ucColorPickerReferenceStar";
			this.ucColorPickerReferenceStar.SelectedColor = System.Drawing.Color.Aqua;
			this.ucColorPickerReferenceStar.Size = new System.Drawing.Size(60, 26);
			this.ucColorPickerReferenceStar.TabIndex = 0;
			this.ucColorPickerReferenceStar.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// label39
			// 
			this.label39.AutoSize = true;
			this.label39.Location = new System.Drawing.Point(71, 39);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(65, 13);
			this.label39.TabIndex = 1;
			this.label39.Text = "Spectra Plot";
			// 
			// ucColorPickerKnownLine
			// 
			this.ucColorPickerKnownLine.AutoSize = true;
			this.ucColorPickerKnownLine.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ucColorPickerKnownLine.BackColor = System.Drawing.SystemColors.Control;
			this.ucColorPickerKnownLine.Location = new System.Drawing.Point(9, 65);
			this.ucColorPickerKnownLine.Name = "ucColorPickerKnownLine";
			this.ucColorPickerKnownLine.SelectedColor = System.Drawing.Color.Blue;
			this.ucColorPickerKnownLine.Size = new System.Drawing.Size(60, 26);
			this.ucColorPickerKnownLine.TabIndex = 2;
			this.ucColorPickerKnownLine.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// ucColorPickerGridLines
			// 
			this.ucColorPickerGridLines.AutoSize = true;
			this.ucColorPickerGridLines.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ucColorPickerGridLines.BackColor = System.Drawing.SystemColors.Control;
			this.ucColorPickerGridLines.Location = new System.Drawing.Point(9, 97);
			this.ucColorPickerGridLines.Name = "ucColorPickerGridLines";
			this.ucColorPickerGridLines.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.ucColorPickerGridLines.Size = new System.Drawing.Size(60, 26);
			this.ucColorPickerGridLines.TabIndex = 3;
			this.ucColorPickerGridLines.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(71, 72);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(103, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Known Spectra Line";
			// 
			// ucColorPickerGridLegend
			// 
			this.ucColorPickerGridLegend.AutoSize = true;
			this.ucColorPickerGridLegend.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ucColorPickerGridLegend.BackColor = System.Drawing.SystemColors.Control;
			this.ucColorPickerGridLegend.Location = new System.Drawing.Point(9, 129);
			this.ucColorPickerGridLegend.Name = "ucColorPickerGridLegend";
			this.ucColorPickerGridLegend.SelectedColor = System.Drawing.Color.White;
			this.ucColorPickerGridLegend.Size = new System.Drawing.Size(60, 26);
			this.ucColorPickerGridLegend.TabIndex = 5;
			this.ucColorPickerGridLegend.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(71, 104);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Grid Lines";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(71, 135);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Grid Legend";
			// 
			// frmSpectraViewSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(321, 251);
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