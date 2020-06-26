namespace Tangra.Video
{
    partial class frmJitterAndDroppedFrameStats
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmJitterAndDroppedFrameStats));
            this.label2 = new System.Windows.Forms.Label();
            this.gbExposureJitter = new System.Windows.Forms.GroupBox();
            this.lblDroppedFrames = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblJitterMs = new System.Windows.Forms.Label();
            this.lblJitter = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblExposureMs = new System.Windows.Forms.Label();
            this.lblExposure = new System.Windows.Forms.Label();
            this.gbExposureJitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(44, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Exposure:";
            // 
            // gbExposureJitter
            // 
            this.gbExposureJitter.Controls.Add(this.lblExposureMs);
            this.gbExposureJitter.Controls.Add(this.lblExposure);
            this.gbExposureJitter.Controls.Add(this.label2);
            this.gbExposureJitter.Controls.Add(this.lblDroppedFrames);
            this.gbExposureJitter.Controls.Add(this.label7);
            this.gbExposureJitter.Controls.Add(this.label6);
            this.gbExposureJitter.Controls.Add(this.lblJitterMs);
            this.gbExposureJitter.Controls.Add(this.lblJitter);
            this.gbExposureJitter.Controls.Add(this.label1);
            this.gbExposureJitter.Location = new System.Drawing.Point(13, 12);
            this.gbExposureJitter.Name = "gbExposureJitter";
            this.gbExposureJitter.Size = new System.Drawing.Size(358, 145);
            this.gbExposureJitter.TabIndex = 29;
            this.gbExposureJitter.TabStop = false;
            // 
            // lblDroppedFrames
            // 
            this.lblDroppedFrames.AutoSize = true;
            this.lblDroppedFrames.ForeColor = System.Drawing.Color.Red;
            this.lblDroppedFrames.Location = new System.Drawing.Point(234, 48);
            this.lblDroppedFrames.Name = "lblDroppedFrames";
            this.lblDroppedFrames.Size = new System.Drawing.Size(103, 13);
            this.lblDroppedFrames.TabIndex = 28;
            this.lblDroppedFrames.Text = "(Dropped Frames: 3)";
            this.lblDroppedFrames.Visible = false;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(5, 72);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(347, 53);
            this.label7.TabIndex = 26;
            this.label7.Text = resources.GetString("label7.Text");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label6.Location = new System.Drawing.Point(70, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "(3s)";
            // 
            // lblJitterMs
            // 
            this.lblJitterMs.AutoSize = true;
            this.lblJitterMs.Location = new System.Drawing.Point(193, 48);
            this.lblJitterMs.Name = "lblJitterMs";
            this.lblJitterMs.Size = new System.Drawing.Size(20, 13);
            this.lblJitterMs.TabIndex = 23;
            this.lblJitterMs.Text = "ms";
            // 
            // lblJitter
            // 
            this.lblJitter.AutoSize = true;
            this.lblJitter.ForeColor = System.Drawing.Color.Maroon;
            this.lblJitter.Location = new System.Drawing.Point(116, 48);
            this.lblJitter.Name = "lblJitter";
            this.lblJitter.Size = new System.Drawing.Size(20, 13);
            this.lblJitter.TabIndex = 26;
            this.lblJitter.Text = "ms";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(36, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Jitter        :";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(151, 174);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 28;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblExposureMs
            // 
            this.lblExposureMs.AutoSize = true;
            this.lblExposureMs.Location = new System.Drawing.Point(193, 16);
            this.lblExposureMs.Name = "lblExposureMs";
            this.lblExposureMs.Size = new System.Drawing.Size(20, 13);
            this.lblExposureMs.TabIndex = 29;
            this.lblExposureMs.Text = "ms";
            // 
            // lblExposure
            // 
            this.lblExposure.AutoSize = true;
            this.lblExposure.ForeColor = System.Drawing.Color.Black;
            this.lblExposure.Location = new System.Drawing.Point(116, 16);
            this.lblExposure.Name = "lblExposure";
            this.lblExposure.Size = new System.Drawing.Size(20, 13);
            this.lblExposure.TabIndex = 30;
            this.lblExposure.Text = "ms";
            // 
            // frmJitterAndDroppedFrameStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 214);
            this.Controls.Add(this.gbExposureJitter);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmJitterAndDroppedFrameStats";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Jitter Statistics";
            this.gbExposureJitter.ResumeLayout(false);
            this.gbExposureJitter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gbExposureJitter;
        private System.Windows.Forms.Label lblDroppedFrames;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblJitterMs;
        private System.Windows.Forms.Label lblJitter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblExposureMs;
        private System.Windows.Forms.Label lblExposure;
    }
}