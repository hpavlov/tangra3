namespace Tangra.VideoTools
{
    partial class frmCorrectInterlacedDefects
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
            this.gbxInterlacedSettings = new System.Windows.Forms.GroupBox();
            this.rbReInterlaceShiftAndSwap = new System.Windows.Forms.RadioButton();
            this.rbReInterlaceShiftForward = new System.Windows.Forms.RadioButton();
            this.rbReInterlaceNon = new System.Windows.Forms.RadioButton();
            this.rbReInterlaceSwapFields = new System.Windows.Forms.RadioButton();
            this.btnClose = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.gbxInterlacedSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxInterlacedSettings
            // 
            this.gbxInterlacedSettings.Controls.Add(this.rbReInterlaceShiftAndSwap);
            this.gbxInterlacedSettings.Controls.Add(this.rbReInterlaceShiftForward);
            this.gbxInterlacedSettings.Controls.Add(this.rbReInterlaceNon);
            this.gbxInterlacedSettings.Controls.Add(this.rbReInterlaceSwapFields);
            this.gbxInterlacedSettings.Location = new System.Drawing.Point(12, 12);
            this.gbxInterlacedSettings.Name = "gbxInterlacedSettings";
            this.gbxInterlacedSettings.Size = new System.Drawing.Size(249, 135);
            this.gbxInterlacedSettings.TabIndex = 49;
            this.gbxInterlacedSettings.TabStop = false;
            this.gbxInterlacedSettings.Text = "Interlaced Video Frame Grabbing Corrections";
            // 
            // rbReInterlaceShiftAndSwap
            // 
            this.rbReInterlaceShiftAndSwap.AutoSize = true;
            this.rbReInterlaceShiftAndSwap.Location = new System.Drawing.Point(24, 93);
            this.rbReInterlaceShiftAndSwap.Name = "rbReInterlaceShiftAndSwap";
            this.rbReInterlaceShiftAndSwap.Size = new System.Drawing.Size(186, 17);
            this.rbReInterlaceShiftAndSwap.TabIndex = 50;
            this.rbReInterlaceShiftAndSwap.Text = "Swap and Shift One Field Forward";
            this.rbReInterlaceShiftAndSwap.UseVisualStyleBackColor = true;
            // 
            // rbReInterlaceShiftForward
            // 
            this.rbReInterlaceShiftForward.AutoSize = true;
            this.rbReInterlaceShiftForward.Location = new System.Drawing.Point(24, 70);
            this.rbReInterlaceShiftForward.Name = "rbReInterlaceShiftForward";
            this.rbReInterlaceShiftForward.Size = new System.Drawing.Size(135, 17);
            this.rbReInterlaceShiftForward.TabIndex = 49;
            this.rbReInterlaceShiftForward.Text = "Shift One Field Forward";
            this.rbReInterlaceShiftForward.UseVisualStyleBackColor = true;
            // 
            // rbReInterlaceNon
            // 
            this.rbReInterlaceNon.AutoSize = true;
            this.rbReInterlaceNon.Checked = true;
            this.rbReInterlaceNon.Location = new System.Drawing.Point(24, 24);
            this.rbReInterlaceNon.Name = "rbReInterlaceNon";
            this.rbReInterlaceNon.Size = new System.Drawing.Size(95, 17);
            this.rbReInterlaceNon.TabIndex = 47;
            this.rbReInterlaceNon.TabStop = true;
            this.rbReInterlaceNon.Text = "No Corrections";
            this.rbReInterlaceNon.UseVisualStyleBackColor = true;
            // 
            // rbReInterlaceSwapFields
            // 
            this.rbReInterlaceSwapFields.AutoSize = true;
            this.rbReInterlaceSwapFields.Location = new System.Drawing.Point(24, 47);
            this.rbReInterlaceSwapFields.Name = "rbReInterlaceSwapFields";
            this.rbReInterlaceSwapFields.Size = new System.Drawing.Size(154, 17);
            this.rbReInterlaceSwapFields.TabIndex = 48;
            this.rbReInterlaceSwapFields.Text = "Swap Even and Odd Fields";
            this.rbReInterlaceSwapFields.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(105, 153);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 50;
            this.btnClose.Text = "OK";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(186, 153);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 51;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // frmCorrectInterlacedDefects
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 186);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gbxInterlacedSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCorrectInterlacedDefects";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Correct Interlaced Video Defects";
            this.gbxInterlacedSettings.ResumeLayout(false);
            this.gbxInterlacedSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxInterlacedSettings;
        private System.Windows.Forms.RadioButton rbReInterlaceShiftAndSwap;
        private System.Windows.Forms.RadioButton rbReInterlaceShiftForward;
        private System.Windows.Forms.RadioButton rbReInterlaceNon;
        private System.Windows.Forms.RadioButton rbReInterlaceSwapFields;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button button1;
    }
}