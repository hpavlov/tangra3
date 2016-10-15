namespace Tangra.Video
{
    partial class frmAviLoadOptions
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
            this.cbxRenderingEngineAttemptOrder = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbReInterlaceShiftForward = new System.Windows.Forms.RadioButton();
            this.rbReInterlaceNon = new System.Windows.Forms.RadioButton();
            this.rbReInterlaceSwapFields = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxRenderingEngineAttemptOrder
            // 
            this.cbxRenderingEngineAttemptOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxRenderingEngineAttemptOrder.Location = new System.Drawing.Point(11, 35);
            this.cbxRenderingEngineAttemptOrder.Name = "cbxRenderingEngineAttemptOrder";
            this.cbxRenderingEngineAttemptOrder.Size = new System.Drawing.Size(190, 21);
            this.cbxRenderingEngineAttemptOrder.TabIndex = 46;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 13);
            this.label2.TabIndex = 45;
            this.label2.Text = "Preferred rendering engine:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(128, 199);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(91, 23);
            this.btnOK.TabIndex = 47;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbxRenderingEngineAttemptOrder);
            this.groupBox1.Location = new System.Drawing.Point(7, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(212, 189);
            this.groupBox1.TabIndex = 48;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbReInterlaceShiftForward);
            this.groupBox2.Controls.Add(this.rbReInterlaceNon);
            this.groupBox2.Controls.Add(this.rbReInterlaceSwapFields);
            this.groupBox2.Location = new System.Drawing.Point(11, 78);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(190, 100);
            this.groupBox2.TabIndex = 47;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Frame Grabbing Corrections";
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
            // frmAviLoadOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 228);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmAviLoadOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AVI Load Options";
            this.Load += new System.EventHandler(this.frmAviLoadOptions_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxRenderingEngineAttemptOrder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbReInterlaceShiftForward;
        private System.Windows.Forms.RadioButton rbReInterlaceNon;
        private System.Windows.Forms.RadioButton rbReInterlaceSwapFields;
    }
}