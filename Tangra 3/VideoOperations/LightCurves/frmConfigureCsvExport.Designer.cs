namespace Tangra.VideoOperations.LightCurves
{
    partial class frmConfigureCsvExport
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
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabTime = new System.Windows.Forms.TabPage();
            this.rbTimeString = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.rbDecimalDays = new System.Windows.Forms.RadioButton();
            this.rbJulianDays = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabSettings.SuspendLayout();
            this.tabTime.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.tabTime);
            this.tabSettings.Location = new System.Drawing.Point(12, 12);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(294, 145);
            this.tabSettings.TabIndex = 0;
            // 
            // tabTime
            // 
            this.tabTime.Controls.Add(this.rbJulianDays);
            this.tabTime.Controls.Add(this.rbDecimalDays);
            this.tabTime.Controls.Add(this.label1);
            this.tabTime.Controls.Add(this.rbTimeString);
            this.tabTime.Location = new System.Drawing.Point(4, 22);
            this.tabTime.Name = "tabTime";
            this.tabTime.Padding = new System.Windows.Forms.Padding(3);
            this.tabTime.Size = new System.Drawing.Size(286, 119);
            this.tabTime.TabIndex = 0;
            this.tabTime.Text = "Time";
            this.tabTime.UseVisualStyleBackColor = true;
            // 
            // rbTimeString
            // 
            this.rbTimeString.AutoSize = true;
            this.rbTimeString.Checked = true;
            this.rbTimeString.Location = new System.Drawing.Point(27, 36);
            this.rbTimeString.Name = "rbTimeString";
            this.rbTimeString.Size = new System.Drawing.Size(145, 17);
            this.rbTimeString.TabIndex = 0;
            this.rbTimeString.TabStop = true;
            this.rbTimeString.Text = "Text Value - HH:mm:ss.fff";
            this.rbTimeString.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Export Timestamp As:";
            // 
            // rbDecimalDays
            // 
            this.rbDecimalDays.AutoSize = true;
            this.rbDecimalDays.Location = new System.Drawing.Point(27, 59);
            this.rbDecimalDays.Name = "rbDecimalDays";
            this.rbDecimalDays.Size = new System.Drawing.Size(138, 17);
            this.rbDecimalDays.TabIndex = 2;
            this.rbDecimalDays.Text = "Decimal days from 0 UT";
            this.rbDecimalDays.UseVisualStyleBackColor = true;
            // 
            // rbJulianDays
            // 
            this.rbJulianDays.AutoSize = true;
            this.rbJulianDays.Location = new System.Drawing.Point(27, 82);
            this.rbJulianDays.Name = "rbJulianDays";
            this.rbJulianDays.Size = new System.Drawing.Size(120, 17);
            this.rbJulianDays.TabIndex = 3;
            this.rbJulianDays.Text = "Decimal Julian Days";
            this.rbJulianDays.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(150, 163);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(231, 163);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmConfigureCsvExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 195);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmConfigureCsvExport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure CSV Export";
            this.tabSettings.ResumeLayout(false);
            this.tabTime.ResumeLayout(false);
            this.tabTime.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabSettings;
        private System.Windows.Forms.TabPage tabTime;
        private System.Windows.Forms.RadioButton rbJulianDays;
        private System.Windows.Forms.RadioButton rbDecimalDays;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbTimeString;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}