namespace Tangra.VideoOperations.LightCurves
{
    partial class frmReprocessSeries
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
            this.pbar1 = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pbar2 = new System.Windows.Forms.ProgressBar();
            this.pbar3 = new System.Windows.Forms.ProgressBar();
            this.pbar4 = new System.Windows.Forms.ProgressBar();
            this.lblEllapsedTime = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlColor4 = new System.Windows.Forms.PictureBox();
            this.pnlColor3 = new System.Windows.Forms.PictureBox();
            this.pnlColor2 = new System.Windows.Forms.PictureBox();
            this.pnlColor1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pnlColor4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlColor3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlColor2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlColor1)).BeginInit();
            this.SuspendLayout();
            // 
            // pbar1
            // 
            this.pbar1.Location = new System.Drawing.Point(30, 21);
            this.pbar1.Name = "pbar1";
            this.pbar1.Size = new System.Drawing.Size(274, 18);
            this.pbar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbar1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(320, 18);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pbar2
            // 
            this.pbar2.Location = new System.Drawing.Point(30, 45);
            this.pbar2.Name = "pbar2";
            this.pbar2.Size = new System.Drawing.Size(274, 18);
            this.pbar2.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbar2.TabIndex = 2;
            this.pbar2.Visible = false;
            // 
            // pbar3
            // 
            this.pbar3.Location = new System.Drawing.Point(30, 69);
            this.pbar3.Name = "pbar3";
            this.pbar3.Size = new System.Drawing.Size(274, 18);
            this.pbar3.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbar3.TabIndex = 3;
            this.pbar3.Visible = false;
            // 
            // pbar4
            // 
            this.pbar4.Location = new System.Drawing.Point(30, 93);
            this.pbar4.Name = "pbar4";
            this.pbar4.Size = new System.Drawing.Size(274, 18);
            this.pbar4.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbar4.TabIndex = 4;
            this.pbar4.Visible = false;
            // 
            // lblEllapsedTime
            // 
            this.lblEllapsedTime.Location = new System.Drawing.Point(206, 3);
            this.lblEllapsedTime.Name = "lblEllapsedTime";
            this.lblEllapsedTime.Size = new System.Drawing.Size(100, 14);
            this.lblEllapsedTime.TabIndex = 8;
            this.lblEllapsedTime.Text = "00:13";
            this.lblEllapsedTime.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Elapsed time";
            // 
            // pnlColor4
            // 
            this.pnlColor4.Location = new System.Drawing.Point(11, 93);
            this.pnlColor4.Name = "pnlColor4";
            this.pnlColor4.Size = new System.Drawing.Size(16, 16);
            this.pnlColor4.TabIndex = 11;
            this.pnlColor4.TabStop = false;
            this.pnlColor4.Visible = false;
            // 
            // pnlColor3
            // 
            this.pnlColor3.Location = new System.Drawing.Point(11, 69);
            this.pnlColor3.Name = "pnlColor3";
            this.pnlColor3.Size = new System.Drawing.Size(16, 16);
            this.pnlColor3.TabIndex = 12;
            this.pnlColor3.TabStop = false;
            this.pnlColor3.Visible = false;
            // 
            // pnlColor2
            // 
            this.pnlColor2.Location = new System.Drawing.Point(11, 45);
            this.pnlColor2.Name = "pnlColor2";
            this.pnlColor2.Size = new System.Drawing.Size(16, 16);
            this.pnlColor2.TabIndex = 13;
            this.pnlColor2.TabStop = false;
            this.pnlColor2.Visible = false;
            // 
            // pnlColor1
            // 
            this.pnlColor1.Location = new System.Drawing.Point(11, 21);
            this.pnlColor1.Name = "pnlColor1";
            this.pnlColor1.Size = new System.Drawing.Size(16, 16);
            this.pnlColor1.TabIndex = 14;
            this.pnlColor1.TabStop = false;
            // 
            // frmReprocessSeries
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 121);
            this.ControlBox = false;
            this.Controls.Add(this.pnlColor1);
            this.Controls.Add(this.pnlColor2);
            this.Controls.Add(this.pnlColor3);
            this.Controls.Add(this.pnlColor4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblEllapsedTime);
            this.Controls.Add(this.pbar4);
            this.Controls.Add(this.pbar3);
            this.Controls.Add(this.pbar2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pbar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmReprocessSeries";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Re-Processing Data ...";
            this.Load += new System.EventHandler(this.frmReprocessSeries_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pnlColor4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlColor3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlColor2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlColor1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbar1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar pbar2;
        private System.Windows.Forms.ProgressBar pbar3;
        private System.Windows.Forms.ProgressBar pbar4;
        private System.Windows.Forms.Label lblEllapsedTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pnlColor4;
        private System.Windows.Forms.PictureBox pnlColor3;
        private System.Windows.Forms.PictureBox pnlColor2;
        private System.Windows.Forms.PictureBox pnlColor1;
    }
}