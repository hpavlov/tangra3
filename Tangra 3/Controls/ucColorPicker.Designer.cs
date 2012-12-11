namespace Tangra.Controls
{
    partial class ucColorPicker
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
			this.pnlColor = new System.Windows.Forms.Panel();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.btnPick = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// pnlColor
			// 
			this.pnlColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlColor.Location = new System.Drawing.Point(5, 5);
			this.pnlColor.Name = "pnlColor";
			this.pnlColor.Size = new System.Drawing.Size(16, 16);
			this.pnlColor.TabIndex = 0;
			// 
			// btnPick
			// 
			this.btnPick.Location = new System.Drawing.Point(30, 2);
			this.btnPick.Name = "btnPick";
			this.btnPick.Size = new System.Drawing.Size(27, 21);
			this.btnPick.TabIndex = 33;
			this.btnPick.Text = "...";
			this.btnPick.Click += new System.EventHandler(this.btnPick_Click);
			// 
			// ucColorPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
			this.Controls.Add(this.btnPick);
			this.Controls.Add(this.pnlColor);
			this.Name = "ucColorPicker";
			this.Size = new System.Drawing.Size(60, 26);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Panel pnlColor;
        private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.Button btnPick;
    }
}
