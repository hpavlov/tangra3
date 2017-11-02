namespace Tangra.Config.SettingPannels
{
    partial class ucAdvanced
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
            this.cbxDebugOutput = new System.Windows.Forms.CheckBox();
            this.cbxOcrDebugMode = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbxDebugOutput
            // 
            this.cbxDebugOutput.AutoSize = true;
            this.cbxDebugOutput.Location = new System.Drawing.Point(12, 26);
            this.cbxDebugOutput.Name = "cbxDebugOutput";
            this.cbxDebugOutput.Size = new System.Drawing.Size(177, 17);
            this.cbxDebugOutput.TabIndex = 55;
            this.cbxDebugOutput.Text = "Enable Astrometry debug output";
            this.cbxDebugOutput.UseVisualStyleBackColor = true;
            // 
            // cbxOcrDebugMode
            // 
            this.cbxOcrDebugMode.AutoSize = true;
            this.cbxOcrDebugMode.Location = new System.Drawing.Point(12, 3);
            this.cbxOcrDebugMode.Name = "cbxOcrDebugMode";
            this.cbxOcrDebugMode.Size = new System.Drawing.Size(147, 17);
            this.cbxOcrDebugMode.TabIndex = 54;
            this.cbxOcrDebugMode.Text = "Enable OCR debug mode";
            this.cbxOcrDebugMode.UseVisualStyleBackColor = true;
            // 
            // ucAdvanced
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbxDebugOutput);
            this.Controls.Add(this.cbxOcrDebugMode);
            this.Name = "ucAdvanced";
            this.Size = new System.Drawing.Size(331, 279);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbxOcrDebugMode;
        private System.Windows.Forms.CheckBox cbxDebugOutput;
    }
}
