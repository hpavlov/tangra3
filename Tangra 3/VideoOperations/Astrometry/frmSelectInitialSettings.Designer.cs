using Tangra.VideoTools;

namespace Tangra.VideoOperations.Astrometry
{
	partial class frmSelectInitialSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSelectInitialSettings));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPlateSolving = new System.Windows.Forms.TabPage();
            this.tabPreProcessing = new System.Windows.Forms.TabPage();
            this.tabDefectFixing = new System.Windows.Forms.TabPage();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ucCameraSettings1 = new Tangra.VideoOperations.Astrometry.ucChooseCalibratedConfiguration();
            this.ucStretching = new Tangra.VideoTools.ucPreProcessing();
            this.ucImageDefectSettings1 = new Tangra.VideoTools.ucImageDefectSettings();
            this.tabControl1.SuspendLayout();
            this.tabPlateSolving.SuspendLayout();
            this.tabPreProcessing.SuspendLayout();
            this.tabDefectFixing.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPlateSolving);
            this.tabControl1.Controls.Add(this.tabPreProcessing);
            this.tabControl1.Controls.Add(this.tabDefectFixing);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(413, 348);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPlateSolving
            // 
            this.tabPlateSolving.Controls.Add(this.ucCameraSettings1);
            this.tabPlateSolving.Location = new System.Drawing.Point(4, 22);
            this.tabPlateSolving.Name = "tabPlateSolving";
            this.tabPlateSolving.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlateSolving.Size = new System.Drawing.Size(405, 322);
            this.tabPlateSolving.TabIndex = 0;
            this.tabPlateSolving.Text = "Camera and Plate Solving";
            this.tabPlateSolving.UseVisualStyleBackColor = true;
            // 
            // tabPreProcessing
            // 
            this.tabPreProcessing.Controls.Add(this.ucStretching);
            this.tabPreProcessing.Location = new System.Drawing.Point(4, 22);
            this.tabPreProcessing.Name = "tabPreProcessing";
            this.tabPreProcessing.Padding = new System.Windows.Forms.Padding(3);
            this.tabPreProcessing.Size = new System.Drawing.Size(405, 322);
            this.tabPreProcessing.TabIndex = 1;
            this.tabPreProcessing.Text = "Pre-Processing";
            this.tabPreProcessing.UseVisualStyleBackColor = true;
            // 
            // tabDefectFixing
            // 
            this.tabDefectFixing.Controls.Add(this.ucImageDefectSettings1);
            this.tabDefectFixing.Location = new System.Drawing.Point(4, 22);
            this.tabDefectFixing.Name = "tabDefectFixing";
            this.tabDefectFixing.Padding = new System.Windows.Forms.Padding(3);
            this.tabDefectFixing.Size = new System.Drawing.Size(405, 322);
            this.tabDefectFixing.TabIndex = 2;
            this.tabDefectFixing.Text = "Image Defects";
            this.tabDefectFixing.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(269, 366);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(350, 366);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ucCameraSettings1
            // 
            this.ucCameraSettings1.Location = new System.Drawing.Point(-6, 0);
            this.ucCameraSettings1.Name = "ucCameraSettings1";
            this.ucCameraSettings1.Size = new System.Drawing.Size(414, 366);
            this.ucCameraSettings1.TabIndex = 0;
            // 
            // ucStretching
            // 
            this.ucStretching.Location = new System.Drawing.Point(6, 6);
            this.ucStretching.Name = "ucStretching";
            this.ucStretching.Size = new System.Drawing.Size(370, 221);
            this.ucStretching.TabIndex = 1;
            // 
            // ucImageDefectSettings1
            // 
            this.ucImageDefectSettings1.Location = new System.Drawing.Point(6, 8);
            this.ucImageDefectSettings1.Name = "ucImageDefectSettings1";
            this.ucImageDefectSettings1.Size = new System.Drawing.Size(393, 308);
            this.ucImageDefectSettings1.TabIndex = 0;
            // 
            // frmSelectInitialSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 397);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSelectInitialSettings";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Video Camera and Recording Configuration";
            this.Move += new System.EventHandler(this.frmSelectInitialSettings_Move);
            this.tabControl1.ResumeLayout(false);
            this.tabPlateSolving.ResumeLayout(false);
            this.tabPreProcessing.ResumeLayout(false);
            this.tabDefectFixing.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private ucChooseCalibratedConfiguration ucCameraSettings1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPlateSolving;
        private System.Windows.Forms.TabPage tabPreProcessing;
        private ucPreProcessing ucStretching;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabPage tabDefectFixing;
        private VideoTools.ucImageDefectSettings ucImageDefectSettings1;


	}
}