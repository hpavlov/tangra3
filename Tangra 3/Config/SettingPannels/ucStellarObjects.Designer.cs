namespace Tangra.Config.SettingPannels
{
	partial class ucStellarObjects
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
            this.gbxStellarObjectRequirements = new System.Windows.Forms.GroupBox();
            this.label67 = new System.Windows.Forms.Label();
            this.label66 = new System.Windows.Forms.Label();
            this.nudMaxElongation = new System.Windows.Forms.NumericUpDown();
            this.label63 = new System.Windows.Forms.Label();
            this.nudLimitMagDetection = new System.Windows.Forms.NumericUpDown();
            this.label65 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.nudMaxFWHM = new System.Windows.Forms.NumericUpDown();
            this.label52 = new System.Windows.Forms.Label();
            this.nudMinFWHM = new System.Windows.Forms.NumericUpDown();
            this.cbForceStellarObjectRequirements = new System.Windows.Forms.CheckBox();
            this.gbxStellarObjectRequirements.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxElongation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimitMagDetection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxFWHM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinFWHM)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxStellarObjectRequirements
            // 
            this.gbxStellarObjectRequirements.Controls.Add(this.label67);
            this.gbxStellarObjectRequirements.Controls.Add(this.label66);
            this.gbxStellarObjectRequirements.Controls.Add(this.nudMaxElongation);
            this.gbxStellarObjectRequirements.Controls.Add(this.label63);
            this.gbxStellarObjectRequirements.Controls.Add(this.nudLimitMagDetection);
            this.gbxStellarObjectRequirements.Controls.Add(this.label65);
            this.gbxStellarObjectRequirements.Controls.Add(this.label64);
            this.gbxStellarObjectRequirements.Controls.Add(this.label50);
            this.gbxStellarObjectRequirements.Controls.Add(this.nudMaxFWHM);
            this.gbxStellarObjectRequirements.Controls.Add(this.label52);
            this.gbxStellarObjectRequirements.Controls.Add(this.nudMinFWHM);
            this.gbxStellarObjectRequirements.Location = new System.Drawing.Point(3, 32);
            this.gbxStellarObjectRequirements.Name = "gbxStellarObjectRequirements";
            this.gbxStellarObjectRequirements.Size = new System.Drawing.Size(225, 146);
            this.gbxStellarObjectRequirements.TabIndex = 33;
            this.gbxStellarObjectRequirements.TabStop = false;
            this.gbxStellarObjectRequirements.Text = "Stellar Object Checks";
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(192, 86);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(15, 13);
            this.label67.TabIndex = 17;
            this.label67.Text = "%";
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Location = new System.Drawing.Point(6, 85);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(127, 13);
            this.label66.TabIndex = 16;
            this.label66.Text = "Maximum PSF Elongation";
            // 
            // nudMaxElongation
            // 
            this.nudMaxElongation.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudMaxElongation.Location = new System.Drawing.Point(141, 83);
            this.nudMaxElongation.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudMaxElongation.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudMaxElongation.Name = "nudMaxElongation";
            this.nudMaxElongation.Size = new System.Drawing.Size(50, 20);
            this.nudMaxElongation.TabIndex = 15;
            this.nudMaxElongation.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Location = new System.Drawing.Point(6, 113);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(117, 13);
            this.label63.TabIndex = 14;
            this.label63.Text = "Min Detection Certainty";
            // 
            // nudLimitMagDetection
            // 
            this.nudLimitMagDetection.DecimalPlaces = 2;
            this.nudLimitMagDetection.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nudLimitMagDetection.Location = new System.Drawing.Point(141, 111);
            this.nudLimitMagDetection.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudLimitMagDetection.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            131072});
            this.nudLimitMagDetection.Name = "nudLimitMagDetection";
            this.nudLimitMagDetection.Size = new System.Drawing.Size(50, 20);
            this.nudLimitMagDetection.TabIndex = 13;
            this.nudLimitMagDetection.Value = new decimal(new int[] {
            75,
            0,
            0,
            131072});
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(192, 61);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(18, 13);
            this.label65.TabIndex = 12;
            this.label65.Text = "px";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(192, 32);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(18, 13);
            this.label64.TabIndex = 11;
            this.label64.Text = "px";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(6, 56);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(88, 13);
            this.label50.TabIndex = 10;
            this.label50.Text = "Maximum FWHM";
            // 
            // nudMaxFWHM
            // 
            this.nudMaxFWHM.DecimalPlaces = 1;
            this.nudMaxFWHM.Increment = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.nudMaxFWHM.Location = new System.Drawing.Point(141, 54);
            this.nudMaxFWHM.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudMaxFWHM.Name = "nudMaxFWHM";
            this.nudMaxFWHM.Size = new System.Drawing.Size(50, 20);
            this.nudMaxFWHM.TabIndex = 9;
            this.nudMaxFWHM.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(6, 29);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(85, 13);
            this.label52.TabIndex = 8;
            this.label52.Text = "Minimum FWHM";
            // 
            // nudMinFWHM
            // 
            this.nudMinFWHM.DecimalPlaces = 1;
            this.nudMinFWHM.Increment = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.nudMinFWHM.Location = new System.Drawing.Point(141, 27);
            this.nudMinFWHM.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudMinFWHM.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.nudMinFWHM.Name = "nudMinFWHM";
            this.nudMinFWHM.Size = new System.Drawing.Size(50, 20);
            this.nudMinFWHM.TabIndex = 7;
            this.nudMinFWHM.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // cbForceStellarObjectRequirements
            // 
            this.cbForceStellarObjectRequirements.AutoSize = true;
            this.cbForceStellarObjectRequirements.Location = new System.Drawing.Point(3, 3);
            this.cbForceStellarObjectRequirements.Name = "cbForceStellarObjectRequirements";
            this.cbForceStellarObjectRequirements.Size = new System.Drawing.Size(154, 17);
            this.cbForceStellarObjectRequirements.TabIndex = 31;
            this.cbForceStellarObjectRequirements.Text = "Filter out non stellar objects";
            this.cbForceStellarObjectRequirements.UseVisualStyleBackColor = true;
            // 
            // ucStellarObjects
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbForceStellarObjectRequirements);
            this.Controls.Add(this.gbxStellarObjectRequirements);
            this.Name = "ucStellarObjects";
            this.Size = new System.Drawing.Size(304, 411);
            this.gbxStellarObjectRequirements.ResumeLayout(false);
            this.gbxStellarObjectRequirements.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxElongation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimitMagDetection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxFWHM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinFWHM)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox gbxStellarObjectRequirements;
		private System.Windows.Forms.Label label67;
		private System.Windows.Forms.Label label66;
		private System.Windows.Forms.NumericUpDown nudMaxElongation;
		private System.Windows.Forms.Label label63;
		private System.Windows.Forms.NumericUpDown nudLimitMagDetection;
		private System.Windows.Forms.Label label65;
		private System.Windows.Forms.Label label64;
		private System.Windows.Forms.Label label50;
		private System.Windows.Forms.NumericUpDown nudMaxFWHM;
		private System.Windows.Forms.Label label52;
        private System.Windows.Forms.NumericUpDown nudMinFWHM;
        private System.Windows.Forms.CheckBox cbForceStellarObjectRequirements;
	}
}
