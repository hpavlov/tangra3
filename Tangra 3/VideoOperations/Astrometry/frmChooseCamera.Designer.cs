namespace Tangra.VideoOperations.Astrometry
{
	partial class frmChooseCamera
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChooseCamera));
			this.ucCameraSettings1 = new Tangra.VideoOperations.Astrometry.ucChooseCalibratedConfiguration();
			this.SuspendLayout();
			// 
			// ucCameraSettings1
			// 
			this.ucCameraSettings1.Location = new System.Drawing.Point(2, 1);
			this.ucCameraSettings1.Name = "ucCameraSettings1";
			this.ucCameraSettings1.Size = new System.Drawing.Size(414, 366);
			this.ucCameraSettings1.TabIndex = 0;
			// 
			// frmChooseCamera
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(417, 377);
			this.Controls.Add(this.ucCameraSettings1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmChooseCamera";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Video Camera and Recording Configuration";
			this.ResumeLayout(false);

		}

		#endregion

		private ucChooseCalibratedConfiguration ucCameraSettings1;
	}
}