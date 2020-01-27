namespace Tangra.Video.SER
{
    partial class frmSerTimestampExposure
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
            this.cbxTimeReference = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nudExposureMs = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlExposure = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.nudExposureMs)).BeginInit();
            this.pnlExposure.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxTimeReference
            // 
            this.cbxTimeReference.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimeReference.FormattingEnabled = true;
            this.cbxTimeReference.Items.AddRange(new object[] {
            "Start Frame",
            "Mid Frame",
            "End Frame"});
            this.cbxTimeReference.Location = new System.Drawing.Point(121, 29);
            this.cbxTimeReference.Name = "cbxTimeReference";
            this.cbxTimeReference.Size = new System.Drawing.Size(96, 21);
            this.cbxTimeReference.TabIndex = 20;
            this.cbxTimeReference.SelectedIndexChanged += new System.EventHandler(this.cbxTimeReference_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Time Reference:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(94, 113);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Exposure:";
            // 
            // nudExposureMs
            // 
            this.nudExposureMs.DecimalPlaces = 2;
            this.nudExposureMs.Location = new System.Drawing.Point(108, 3);
            this.nudExposureMs.Maximum = new decimal(new int[] {
            86400000,
            0,
            0,
            0});
            this.nudExposureMs.Name = "nudExposureMs";
            this.nudExposureMs.Size = new System.Drawing.Size(69, 20);
            this.nudExposureMs.TabIndex = 21;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(183, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "ms";
            // 
            // pnlExposure
            // 
            this.pnlExposure.Controls.Add(this.nudExposureMs);
            this.pnlExposure.Controls.Add(this.label2);
            this.pnlExposure.Controls.Add(this.label3);
            this.pnlExposure.Location = new System.Drawing.Point(12, 56);
            this.pnlExposure.Name = "pnlExposure";
            this.pnlExposure.Size = new System.Drawing.Size(237, 26);
            this.pnlExposure.TabIndex = 24;
            // 
            // frmSerTimestampExposure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 158);
            this.Controls.Add(this.pnlExposure);
            this.Controls.Add(this.cbxTimeReference);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmSerTimestampExposure";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SER TimeStamp & Exposure";
            ((System.ComponentModel.ISupportInitialize)(this.nudExposureMs)).EndInit();
            this.pnlExposure.ResumeLayout(false);
            this.pnlExposure.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxTimeReference;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudExposureMs;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlExposure;
    }
}