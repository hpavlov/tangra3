using System.Windows.Forms;

namespace Tangra.Config.SettingPannels
{
    partial class ucCompatibility
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
			this.cbxPsfOptimization = new System.Windows.Forms.ComboBox();
			this.gbxSimplifiedTracker = new System.Windows.Forms.GroupBox();
			this.rbSimplifiedTrackerManaged = new System.Windows.Forms.RadioButton();
			this.rbSimplifiedTrackerNative = new System.Windows.Forms.RadioButton();
			this.grpPSFFitting = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rbOcrManagedMode = new System.Windows.Forms.RadioButton();
			this.rbOcrMixedMode = new System.Windows.Forms.RadioButton();
			this.gbxSimplifiedTracker.SuspendLayout();
			this.grpPSFFitting.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbxPsfOptimization
			// 
			this.cbxPsfOptimization.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxPsfOptimization.Items.AddRange(new object[] {
            "Fully Managed",
            "Mixed",
            "Fully Native"});
			this.cbxPsfOptimization.Location = new System.Drawing.Point(21, 33);
			this.cbxPsfOptimization.Name = "cbxPsfOptimization";
			this.cbxPsfOptimization.Size = new System.Drawing.Size(185, 21);
			this.cbxPsfOptimization.TabIndex = 43;
			// 
			// gbxSimplifiedTracker
			// 
			this.gbxSimplifiedTracker.Controls.Add(this.rbSimplifiedTrackerManaged);
			this.gbxSimplifiedTracker.Controls.Add(this.rbSimplifiedTrackerNative);
			this.gbxSimplifiedTracker.Location = new System.Drawing.Point(3, 84);
			this.gbxSimplifiedTracker.Name = "gbxSimplifiedTracker";
			this.gbxSimplifiedTracker.Size = new System.Drawing.Size(270, 78);
			this.gbxSimplifiedTracker.TabIndex = 56;
			this.gbxSimplifiedTracker.TabStop = false;
			this.gbxSimplifiedTracker.Text = "Simplified Tracking Implementation";
			// 
			// rbSimplifiedTrackerManaged
			// 
			this.rbSimplifiedTrackerManaged.AutoSize = true;
			this.rbSimplifiedTrackerManaged.Location = new System.Drawing.Point(103, 42);
			this.rbSimplifiedTrackerManaged.Name = "rbSimplifiedTrackerManaged";
			this.rbSimplifiedTrackerManaged.Size = new System.Drawing.Size(70, 17);
			this.rbSimplifiedTrackerManaged.TabIndex = 1;
			this.rbSimplifiedTrackerManaged.Text = "Managed";
			this.rbSimplifiedTrackerManaged.UseVisualStyleBackColor = true;
			// 
			// rbSimplifiedTrackerNative
			// 
			this.rbSimplifiedTrackerNative.AutoSize = true;
			this.rbSimplifiedTrackerNative.Checked = true;
			this.rbSimplifiedTrackerNative.Location = new System.Drawing.Point(21, 42);
			this.rbSimplifiedTrackerNative.Name = "rbSimplifiedTrackerNative";
			this.rbSimplifiedTrackerNative.Size = new System.Drawing.Size(56, 17);
			this.rbSimplifiedTrackerNative.TabIndex = 0;
			this.rbSimplifiedTrackerNative.TabStop = true;
			this.rbSimplifiedTrackerNative.Text = "Native";
			this.rbSimplifiedTrackerNative.UseVisualStyleBackColor = true;
			// 
			// grpPSFFitting
			// 
			this.grpPSFFitting.Controls.Add(this.cbxPsfOptimization);
			this.grpPSFFitting.Location = new System.Drawing.Point(3, 3);
			this.grpPSFFitting.Name = "grpPSFFitting";
			this.grpPSFFitting.Size = new System.Drawing.Size(270, 75);
			this.grpPSFFitting.TabIndex = 57;
			this.grpPSFFitting.TabStop = false;
			this.grpPSFFitting.Text = "PSF Fitting Implementation";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rbOcrManagedMode);
			this.groupBox1.Controls.Add(this.rbOcrMixedMode);
			this.groupBox1.Location = new System.Drawing.Point(3, 165);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(270, 78);
			this.groupBox1.TabIndex = 58;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "OCR Implementation";
			// 
			// rbOcrNativeMode
			// 
			this.rbOcrManagedMode.AutoSize = true;
			this.rbOcrManagedMode.Location = new System.Drawing.Point(103, 42);
			this.rbOcrManagedMode.Name = "rbOcrManagedMode";
			this.rbOcrManagedMode.Size = new System.Drawing.Size(70, 17);
			this.rbOcrManagedMode.TabIndex = 1;
			this.rbOcrManagedMode.Text = "Managed";
			this.rbOcrManagedMode.UseVisualStyleBackColor = true;
			// 
			// rbOcrMixedMode
			// 
			this.rbOcrMixedMode.AutoSize = true;
			this.rbOcrMixedMode.Checked = true;
			this.rbOcrMixedMode.Location = new System.Drawing.Point(21, 42);
			this.rbOcrMixedMode.Name = "rbOcrMixedMode";
			this.rbOcrMixedMode.Size = new System.Drawing.Size(53, 17);
			this.rbOcrMixedMode.TabIndex = 0;
			this.rbOcrMixedMode.TabStop = true;
			this.rbOcrMixedMode.Text = "Mixed";
			this.rbOcrMixedMode.UseVisualStyleBackColor = true;
			// 
			// ucCompatibility
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.grpPSFFitting);
			this.Controls.Add(this.gbxSimplifiedTracker);
			this.Name = "ucCompatibility";
			this.Size = new System.Drawing.Size(502, 257);
			this.gbxSimplifiedTracker.ResumeLayout(false);
			this.gbxSimplifiedTracker.PerformLayout();
			this.grpPSFFitting.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

        private ComboBox cbxPsfOptimization;
        private GroupBox gbxSimplifiedTracker;
        private RadioButton rbSimplifiedTrackerManaged;
        private RadioButton rbSimplifiedTrackerNative;
        private GroupBox grpPSFFitting;
		private GroupBox groupBox1;
		private RadioButton rbOcrManagedMode;
		private RadioButton rbOcrMixedMode;

    }
}
