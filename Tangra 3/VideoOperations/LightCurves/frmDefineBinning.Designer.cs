namespace Tangra.VideoOperations.LightCurves
{
    partial class frmDefineBinning
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
            this.btnIntegrationDetection = new System.Windows.Forms.Button();
            this.nudNumFramesToBin = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudReferenceFrame = new System.Windows.Forms.NumericUpDown();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumFramesToBin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReferenceFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // btnIntegrationDetection
            // 
            this.btnIntegrationDetection.Location = new System.Drawing.Point(197, 43);
            this.btnIntegrationDetection.Name = "btnIntegrationDetection";
            this.btnIntegrationDetection.Size = new System.Drawing.Size(71, 23);
            this.btnIntegrationDetection.TabIndex = 0;
            this.btnIntegrationDetection.Text = "Detect";
            this.btnIntegrationDetection.UseVisualStyleBackColor = true;
            this.btnIntegrationDetection.Click += new System.EventHandler(this.btnIntegrationDetection_Click);
            // 
            // nudNumFramesToBin
            // 
            this.nudNumFramesToBin.Location = new System.Drawing.Point(54, 16);
            this.nudNumFramesToBin.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nudNumFramesToBin.Name = "nudNumFramesToBin";
            this.nudNumFramesToBin.Size = new System.Drawing.Size(52, 20);
            this.nudNumFramesToBin.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(113, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "frames";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Bin ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "First (Reference) Frame:";
            // 
            // nudReferenceFrame
            // 
            this.nudReferenceFrame.Location = new System.Drawing.Point(134, 46);
            this.nudReferenceFrame.Name = "nudReferenceFrame";
            this.nudReferenceFrame.Size = new System.Drawing.Size(52, 20);
            this.nudReferenceFrame.TabIndex = 10;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(112, 99);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(193, 99);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(15, 78);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(253, 2);
            this.panel1.TabIndex = 13;
            // 
            // frmDefineBinning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 131);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.nudReferenceFrame);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudNumFramesToBin);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnIntegrationDetection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmDefineBinning";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Define Beginning";
            this.Load += new System.EventHandler(this.frmDefineBinning_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudNumFramesToBin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReferenceFrame)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnIntegrationDetection;
        protected internal System.Windows.Forms.NumericUpDown nudNumFramesToBin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        protected internal System.Windows.Forms.NumericUpDown nudReferenceFrame;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
    }
}