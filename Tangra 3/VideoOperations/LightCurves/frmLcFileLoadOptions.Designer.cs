namespace Tangra.VideoOperations.LightCurves
{
	partial class frmLcFileLoadOptions
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
			this.label1 = new System.Windows.Forms.Label();
			this.rbSkipPixels = new System.Windows.Forms.RadioButton();
			this.rbNormally = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(394, 30);
			this.label1.TabIndex = 0;
			this.label1.Text = "    This .lc file is very large and loading it normally may cause an Out Of Memor" +
    "y condition. How do you want to load this file?";
			// 
			// rbSkipPixels
			// 
			this.rbSkipPixels.AutoSize = true;
			this.rbSkipPixels.Checked = true;
			this.rbSkipPixels.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.rbSkipPixels.Location = new System.Drawing.Point(35, 63);
			this.rbSkipPixels.Name = "rbSkipPixels";
			this.rbSkipPixels.Size = new System.Drawing.Size(200, 17);
			this.rbSkipPixels.TabIndex = 1;
			this.rbSkipPixels.TabStop = true;
			this.rbSkipPixels.Text = "Load the file without pixel data";
			this.rbSkipPixels.UseVisualStyleBackColor = true;
			// 
			// rbNormally
			// 
			this.rbNormally.AutoSize = true;
			this.rbNormally.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.rbNormally.Location = new System.Drawing.Point(35, 129);
			this.rbNormally.Name = "rbNormally";
			this.rbNormally.Size = new System.Drawing.Size(146, 17);
			this.rbNormally.TabIndex = 2;
			this.rbNormally.Text = "Load the file normally";
			this.rbNormally.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(166, 212);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(113, 23);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(50, 83);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(376, 30);
			this.label2.TabIndex = 4;
			this.label2.Text = "You will not be able to re-process the file or view the position of the measuring" +
    " apertures but low memory conditions will be avoided.";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(50, 149);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(376, 30);
			this.label3.TabIndex = 5;
			this.label3.Text = "Full functionality will be provided but the large amount of data may cause Tangra" +
    " to stop working.";
			// 
			// frmLcFileLoadOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(448, 253);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.rbNormally);
			this.Controls.Add(this.rbSkipPixels);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmLcFileLoadOptions";
			this.Text = "LC File Load Options";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		protected internal System.Windows.Forms.RadioButton rbNormally;
		protected internal System.Windows.Forms.RadioButton rbSkipPixels;
	}
}