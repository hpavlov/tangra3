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
            this.pnlExposureJitter = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblJitter = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudExposureMs)).BeginInit();
            this.pnlExposure.SuspendLayout();
            this.pnlExposureJitter.SuspendLayout();
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
            this.btnOK.Location = new System.Drawing.Point(94, 116);
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
            this.pnlExposure.Size = new System.Drawing.Size(250, 25);
            this.pnlExposure.TabIndex = 24;
            // 
            // pnlExposureJitter
            // 
            this.pnlExposureJitter.Controls.Add(this.label6);
            this.pnlExposureJitter.Controls.Add(this.lblJitter);
            this.pnlExposureJitter.Controls.Add(this.label1);
            this.pnlExposureJitter.Controls.Add(this.label4);
            this.pnlExposureJitter.Location = new System.Drawing.Point(12, 80);
            this.pnlExposureJitter.Name = "pnlExposureJitter";
            this.pnlExposureJitter.Size = new System.Drawing.Size(252, 25);
            this.pnlExposureJitter.TabIndex = 25;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Jitter            :";
            // 
            // lblJitter
            // 
            this.lblJitter.AutoSize = true;
            this.lblJitter.ForeColor = System.Drawing.Color.Maroon;
            this.lblJitter.Location = new System.Drawing.Point(106, 7);
            this.lblJitter.Name = "lblJitter";
            this.lblJitter.Size = new System.Drawing.Size(20, 13);
            this.lblJitter.TabIndex = 26;
            this.lblJitter.Text = "ms";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(183, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "ms";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label6.Location = new System.Drawing.Point(55, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "(3-s)";
            // 
            // frmSerTimestampExposure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 158);
            this.Controls.Add(this.pnlExposureJitter);
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
            this.pnlExposureJitter.ResumeLayout(false);
            this.pnlExposureJitter.PerformLayout();
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
        private System.Windows.Forms.Panel pnlExposureJitter;
        private System.Windows.Forms.Label lblJitter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
    }
}