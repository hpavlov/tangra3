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
            this.gbxSimplifiedTracker.SuspendLayout();
            this.grpPSFFitting.SuspendLayout();
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
            // ucCompatibility
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.grpPSFFitting);
            this.Controls.Add(this.gbxSimplifiedTracker);
            this.Name = "ucCompatibility";
            this.Size = new System.Drawing.Size(502, 257);
            this.gbxSimplifiedTracker.ResumeLayout(false);
            this.gbxSimplifiedTracker.PerformLayout();
            this.grpPSFFitting.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private ComboBox cbxPsfOptimization;
        private GroupBox gbxSimplifiedTracker;
        private RadioButton rbSimplifiedTrackerManaged;
        private RadioButton rbSimplifiedTrackerNative;
        private GroupBox grpPSFFitting;

    }
}
