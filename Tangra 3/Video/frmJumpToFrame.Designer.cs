namespace Tangra.Video
{
	partial class frmJumpToFrame
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
			this.nudFrameToJumpTo = new System.Windows.Forms.NumericUpDown();
			this.button1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.nudFrameToJumpTo)).BeginInit();
			this.SuspendLayout();
			// 
			// nudFrameToJumpTo
			// 
			this.nudFrameToJumpTo.Location = new System.Drawing.Point(21, 25);
			this.nudFrameToJumpTo.Name = "nudFrameToJumpTo";
			this.nudFrameToJumpTo.Size = new System.Drawing.Size(96, 20);
			this.nudFrameToJumpTo.TabIndex = 0;
			this.nudFrameToJumpTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.nudFrameToJumpTo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nudFrameToJumpTo_KeyPress);
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(149, 22);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// frmJumpToFrame
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(247, 64);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.nudFrameToJumpTo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmJumpToFrame";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Jump To Frame";
			this.Load += new System.EventHandler(this.frmJumpToFrame_Load);
			((System.ComponentModel.ISupportInitialize)(this.nudFrameToJumpTo)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button1;
		protected internal System.Windows.Forms.NumericUpDown nudFrameToJumpTo;
	}
}