namespace Tangra.ImageTools
{
    partial class frmIdentifiedStarAction
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.rbIdentifyMoreStars = new System.Windows.Forms.RadioButton();
            this.rbAttemptPlateSolve = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "What do you want to do next?";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(89, 92);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(144, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rbIdentifyMoreStars
            // 
            this.rbIdentifyMoreStars.AutoSize = true;
            this.rbIdentifyMoreStars.Checked = true;
            this.rbIdentifyMoreStars.Location = new System.Drawing.Point(35, 52);
            this.rbIdentifyMoreStars.Name = "rbIdentifyMoreStars";
            this.rbIdentifyMoreStars.Size = new System.Drawing.Size(118, 17);
            this.rbIdentifyMoreStars.TabIndex = 2;
            this.rbIdentifyMoreStars.TabStop = true;
            this.rbIdentifyMoreStars.Text = "Identify another star";
            this.rbIdentifyMoreStars.UseVisualStyleBackColor = true;
            // 
            // rbAttemptPlateSolve
            // 
            this.rbAttemptPlateSolve.AutoSize = true;
            this.rbAttemptPlateSolve.Location = new System.Drawing.Point(175, 52);
            this.rbAttemptPlateSolve.Name = "rbAttemptPlateSolve";
            this.rbAttemptPlateSolve.Size = new System.Drawing.Size(124, 17);
            this.rbAttemptPlateSolve.TabIndex = 3;
            this.rbAttemptPlateSolve.Text = "Attempt a plate solve";
            this.rbAttemptPlateSolve.UseVisualStyleBackColor = true;
            // 
            // frmIdentifiedStarAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 127);
            this.Controls.Add(this.rbAttemptPlateSolve);
            this.Controls.Add(this.rbIdentifyMoreStars);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmIdentifiedStarAction";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tangra";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rbIdentifyMoreStars;
        private System.Windows.Forms.RadioButton rbAttemptPlateSolve;
    }
}