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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSerTimestampExposure));
            this.cbxTimeReference = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nudExposureMs = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblJitter = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblJitterMs = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.gbExposureJitter = new System.Windows.Forms.GroupBox();
            this.gbExposure = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblDroppedFrames = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudExposureMs)).BeginInit();
            this.gbExposureJitter.SuspendLayout();
            this.gbExposure.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.cbxTimeReference.Location = new System.Drawing.Point(113, 12);
            this.cbxTimeReference.Name = "cbxTimeReference";
            this.cbxTimeReference.Size = new System.Drawing.Size(96, 21);
            this.cbxTimeReference.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(7, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Time Reference:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(150, 278);
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
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(45, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Exposure:";
            // 
            // nudExposureMs
            // 
            this.nudExposureMs.DecimalPlaces = 2;
            this.nudExposureMs.Location = new System.Drawing.Point(118, 13);
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
            this.label3.Location = new System.Drawing.Point(193, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "ms";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label6.Location = new System.Drawing.Point(70, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "(3s)";
            // 
            // lblJitter
            // 
            this.lblJitter.AutoSize = true;
            this.lblJitter.ForeColor = System.Drawing.Color.Maroon;
            this.lblJitter.Location = new System.Drawing.Point(116, 13);
            this.lblJitter.Name = "lblJitter";
            this.lblJitter.Size = new System.Drawing.Size(20, 13);
            this.lblJitter.TabIndex = 26;
            this.lblJitter.Text = "ms";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(36, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Jitter        :";
            // 
            // lblJitterMs
            // 
            this.lblJitterMs.AutoSize = true;
            this.lblJitterMs.Location = new System.Drawing.Point(193, 13);
            this.lblJitterMs.Name = "lblJitterMs";
            this.lblJitterMs.Size = new System.Drawing.Size(20, 13);
            this.lblJitterMs.TabIndex = 23;
            this.lblJitterMs.Text = "ms";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(5, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(347, 53);
            this.label7.TabIndex = 26;
            this.label7.Text = resources.GetString("label7.Text");
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(7, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(345, 28);
            this.label8.TabIndex = 24;
            this.label8.Text = "Calculated exposure assuming a constant frame rate and zero dead time. Please adj" +
    "ust to match the actual exposure.";
            // 
            // gbExposureJitter
            // 
            this.gbExposureJitter.Controls.Add(this.lblDroppedFrames);
            this.gbExposureJitter.Controls.Add(this.label7);
            this.gbExposureJitter.Controls.Add(this.label6);
            this.gbExposureJitter.Controls.Add(this.lblJitterMs);
            this.gbExposureJitter.Controls.Add(this.lblJitter);
            this.gbExposureJitter.Controls.Add(this.label1);
            this.gbExposureJitter.Location = new System.Drawing.Point(12, 170);
            this.gbExposureJitter.Name = "gbExposureJitter";
            this.gbExposureJitter.Size = new System.Drawing.Size(358, 98);
            this.gbExposureJitter.TabIndex = 26;
            this.gbExposureJitter.TabStop = false;
            // 
            // gbExposure
            // 
            this.gbExposure.Controls.Add(this.label8);
            this.gbExposure.Controls.Add(this.label2);
            this.gbExposure.Controls.Add(this.nudExposureMs);
            this.gbExposure.Controls.Add(this.label3);
            this.gbExposure.Location = new System.Drawing.Point(12, 95);
            this.gbExposure.Name = "gbExposure";
            this.gbExposure.Size = new System.Drawing.Size(358, 74);
            this.gbExposure.TabIndex = 27;
            this.gbExposure.TabStop = false;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(4, 38);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(353, 42);
            this.label9.TabIndex = 28;
            this.label9.Text = resources.GetString("label9.Text");
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.cbxTimeReference);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(358, 86);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            // 
            // lblDroppedFrames
            // 
            this.lblDroppedFrames.AutoSize = true;
            this.lblDroppedFrames.ForeColor = System.Drawing.Color.Red;
            this.lblDroppedFrames.Location = new System.Drawing.Point(234, 13);
            this.lblDroppedFrames.Name = "lblDroppedFrames";
            this.lblDroppedFrames.Size = new System.Drawing.Size(103, 13);
            this.lblDroppedFrames.TabIndex = 28;
            this.lblDroppedFrames.Text = "(Dropped Frames: 3)";
            this.lblDroppedFrames.Visible = false;
            // 
            // frmSerTimestampExposure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 315);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbExposure);
            this.Controls.Add(this.gbExposureJitter);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmSerTimestampExposure";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SER TimeStamp & Exposure";
            ((System.ComponentModel.ISupportInitialize)(this.nudExposureMs)).EndInit();
            this.gbExposureJitter.ResumeLayout(false);
            this.gbExposureJitter.PerformLayout();
            this.gbExposure.ResumeLayout(false);
            this.gbExposure.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxTimeReference;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudExposureMs;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblJitter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblJitterMs;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gbExposureJitter;
        private System.Windows.Forms.GroupBox gbExposure;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblDroppedFrames;
    }
}