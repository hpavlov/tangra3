namespace Tangra.VideoOperations.LightCurves.InfoForms
{
    partial class frmNoiseDistribution
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNoiseDistribution));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudSigmaRange = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.nudNumberBins = new System.Windows.Forms.NumericUpDown();
            this.pb4 = new System.Windows.Forms.PictureBox();
            this.pb3 = new System.Windows.Forms.PictureBox();
            this.pb2 = new System.Windows.Forms.PictureBox();
            this.pb1 = new System.Windows.Forms.PictureBox();
            this.rbTarget4 = new System.Windows.Forms.RadioButton();
            this.rbTarget3 = new System.Windows.Forms.RadioButton();
            this.rbTarget2 = new System.Windows.Forms.RadioButton();
            this.rbTarget1 = new System.Windows.Forms.RadioButton();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblFWHM = new System.Windows.Forms.Label();
            this.lblPFSMax = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblPSFMiddle = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSigmaRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberBins)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.nudSigmaRange);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.nudNumberBins);
            this.groupBox1.Controls.Add(this.pb4);
            this.groupBox1.Controls.Add(this.pb3);
            this.groupBox1.Controls.Add(this.pb2);
            this.groupBox1.Controls.Add(this.pb1);
            this.groupBox1.Controls.Add(this.rbTarget4);
            this.groupBox1.Controls.Add(this.rbTarget3);
            this.groupBox1.Controls.Add(this.rbTarget2);
            this.groupBox1.Controls.Add(this.rbTarget1);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(427, 71);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Symbol", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label3.Location = new System.Drawing.Point(390, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 16);
            this.label3.TabIndex = 19;
            this.label3.Text = "s";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(247, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Displayed range:";
            // 
            // nudSigmaRange
            // 
            this.nudSigmaRange.Location = new System.Drawing.Point(336, 18);
            this.nudSigmaRange.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudSigmaRange.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudSigmaRange.Name = "nudSigmaRange";
            this.nudSigmaRange.Size = new System.Drawing.Size(48, 20);
            this.nudSigmaRange.TabIndex = 17;
            this.nudSigmaRange.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.nudSigmaRange.ValueChanged += new System.EventHandler(this.nudSigmaRange_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(252, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Number of bins:";
            // 
            // nudNumberBins
            // 
            this.nudNumberBins.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudNumberBins.Location = new System.Drawing.Point(336, 40);
            this.nudNumberBins.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudNumberBins.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudNumberBins.Name = "nudNumberBins";
            this.nudNumberBins.Size = new System.Drawing.Size(48, 20);
            this.nudNumberBins.TabIndex = 15;
            this.nudNumberBins.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudNumberBins.ValueChanged += new System.EventHandler(this.nudNumberBins_ValueChanged);
            // 
            // pb4
            // 
            this.pb4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb4.BackColor = System.Drawing.Color.Maroon;
            this.pb4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb4.Location = new System.Drawing.Point(128, 47);
            this.pb4.Name = "pb4";
            this.pb4.Size = new System.Drawing.Size(10, 11);
            this.pb4.TabIndex = 14;
            this.pb4.TabStop = false;
            // 
            // pb3
            // 
            this.pb3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb3.BackColor = System.Drawing.Color.Maroon;
            this.pb3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb3.Location = new System.Drawing.Point(128, 22);
            this.pb3.Name = "pb3";
            this.pb3.Size = new System.Drawing.Size(10, 11);
            this.pb3.TabIndex = 13;
            this.pb3.TabStop = false;
            // 
            // pb2
            // 
            this.pb2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb2.BackColor = System.Drawing.Color.Maroon;
            this.pb2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb2.Location = new System.Drawing.Point(11, 45);
            this.pb2.Name = "pb2";
            this.pb2.Size = new System.Drawing.Size(10, 11);
            this.pb2.TabIndex = 12;
            this.pb2.TabStop = false;
            // 
            // pb1
            // 
            this.pb1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb1.BackColor = System.Drawing.Color.Maroon;
            this.pb1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb1.Location = new System.Drawing.Point(11, 20);
            this.pb1.Name = "pb1";
            this.pb1.Size = new System.Drawing.Size(10, 11);
            this.pb1.TabIndex = 11;
            this.pb1.TabStop = false;
            // 
            // rbTarget4
            // 
            this.rbTarget4.AutoSize = true;
            this.rbTarget4.Location = new System.Drawing.Point(143, 43);
            this.rbTarget4.Name = "rbTarget4";
            this.rbTarget4.Size = new System.Drawing.Size(65, 17);
            this.rbTarget4.TabIndex = 3;
            this.rbTarget4.Text = "Object 4";
            this.rbTarget4.UseVisualStyleBackColor = true;
            this.rbTarget4.CheckedChanged += new System.EventHandler(this.TargetChecked);
            // 
            // rbTarget3
            // 
            this.rbTarget3.AutoSize = true;
            this.rbTarget3.Location = new System.Drawing.Point(143, 20);
            this.rbTarget3.Name = "rbTarget3";
            this.rbTarget3.Size = new System.Drawing.Size(65, 17);
            this.rbTarget3.TabIndex = 2;
            this.rbTarget3.Text = "Object 3";
            this.rbTarget3.UseVisualStyleBackColor = true;
            this.rbTarget3.Resize += new System.EventHandler(this.TargetChecked);
            // 
            // rbTarget2
            // 
            this.rbTarget2.AutoSize = true;
            this.rbTarget2.Location = new System.Drawing.Point(26, 43);
            this.rbTarget2.Name = "rbTarget2";
            this.rbTarget2.Size = new System.Drawing.Size(65, 17);
            this.rbTarget2.TabIndex = 1;
            this.rbTarget2.Text = "Object 2";
            this.rbTarget2.UseVisualStyleBackColor = true;
            this.rbTarget2.CheckedChanged += new System.EventHandler(this.TargetChecked);
            // 
            // rbTarget1
            // 
            this.rbTarget1.AutoSize = true;
            this.rbTarget1.Checked = true;
            this.rbTarget1.Location = new System.Drawing.Point(26, 18);
            this.rbTarget1.Name = "rbTarget1";
            this.rbTarget1.Size = new System.Drawing.Size(65, 17);
            this.rbTarget1.TabIndex = 0;
            this.rbTarget1.TabStop = true;
            this.rbTarget1.Text = "Object 1";
            this.rbTarget1.UseVisualStyleBackColor = true;
            this.rbTarget1.CheckedChanged += new System.EventHandler(this.TargetChecked);
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(12, 89);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(427, 344);
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label4.Location = new System.Drawing.Point(18, 460);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 19);
            this.label4.TabIndex = 19;
            this.label4.Text = "s";
            // 
            // lblFWHM
            // 
            this.lblFWHM.AutoSize = true;
            this.lblFWHM.Location = new System.Drawing.Point(40, 464);
            this.lblFWHM.Name = "lblFWHM";
            this.lblFWHM.Size = new System.Drawing.Size(28, 13);
            this.lblFWHM.TabIndex = 20;
            this.lblFWHM.Text = "1.12";
            // 
            // lblPFSMax
            // 
            this.lblPFSMax.AutoSize = true;
            this.lblPFSMax.Location = new System.Drawing.Point(345, 464);
            this.lblPFSMax.Name = "lblPFSMax";
            this.lblPFSMax.Size = new System.Drawing.Size(37, 13);
            this.lblPFSMax.TabIndex = 22;
            this.lblPFSMax.Text = "50000";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(317, 464);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Max:";
            // 
            // lblPSFMiddle
            // 
            this.lblPSFMiddle.AutoSize = true;
            this.lblPSFMiddle.Location = new System.Drawing.Point(192, 464);
            this.lblPSFMiddle.Name = "lblPSFMiddle";
            this.lblPSFMiddle.Size = new System.Drawing.Size(37, 13);
            this.lblPSFMiddle.TabIndex = 24;
            this.lblPSFMiddle.Text = "50000";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(152, 464);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 13);
            this.label9.TabIndex = 23;
            this.label9.Text = "Middle:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 463);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = ":";
            // 
            // frmNoiseDistribution
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 445);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblPSFMiddle);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblPFSMax);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblFWHM);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNoiseDistribution";
            this.Text = "Signal Distribution";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSigmaRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberBins)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbTarget4;
        private System.Windows.Forms.RadioButton rbTarget3;
        private System.Windows.Forms.RadioButton rbTarget2;
        private System.Windows.Forms.RadioButton rbTarget1;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudSigmaRange;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudNumberBins;
        private System.Windows.Forms.PictureBox pb3;
        private System.Windows.Forms.PictureBox pb2;
        private System.Windows.Forms.PictureBox pb1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblFWHM;
        private System.Windows.Forms.Label lblPFSMax;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblPSFMiddle;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.PictureBox pb4;
        private System.Windows.Forms.Label label5;
    }
}