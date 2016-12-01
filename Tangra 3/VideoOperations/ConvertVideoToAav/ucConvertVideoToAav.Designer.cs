namespace Tangra.VideoOperations.ConvertVideoToAav
{
    partial class ucConvertVideoToAav
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbxSection = new System.Windows.Forms.GroupBox();
            this.nudFirstFrame = new System.Windows.Forms.NumericUpDown();
            this.label27 = new System.Windows.Forms.Label();
            this.nudLastFrame = new System.Windows.Forms.NumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.btnShowFields = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.gbxSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFirstFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLastFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxSection
            // 
            this.gbxSection.Controls.Add(this.nudFirstFrame);
            this.gbxSection.Controls.Add(this.label27);
            this.gbxSection.Controls.Add(this.nudLastFrame);
            this.gbxSection.Controls.Add(this.label26);
            this.gbxSection.Location = new System.Drawing.Point(11, 8);
            this.gbxSection.Name = "gbxSection";
            this.gbxSection.Size = new System.Drawing.Size(228, 86);
            this.gbxSection.TabIndex = 32;
            this.gbxSection.TabStop = false;
            this.gbxSection.Text = "Section";
            // 
            // nudFirstFrame
            // 
            this.nudFirstFrame.Location = new System.Drawing.Point(89, 21);
            this.nudFirstFrame.Name = "nudFirstFrame";
            this.nudFirstFrame.Size = new System.Drawing.Size(58, 20);
            this.nudFirstFrame.TabIndex = 22;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(16, 23);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(58, 13);
            this.label27.TabIndex = 24;
            this.label27.Text = "First Frame";
            // 
            // nudLastFrame
            // 
            this.nudLastFrame.Location = new System.Drawing.Point(89, 48);
            this.nudLastFrame.Name = "nudLastFrame";
            this.nudLastFrame.Size = new System.Drawing.Size(58, 20);
            this.nudLastFrame.TabIndex = 23;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(16, 50);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(59, 13);
            this.label26.TabIndex = 25;
            this.label26.Text = "Last Frame";
            // 
            // btnShowFields
            // 
            this.btnShowFields.Location = new System.Drawing.Point(11, 113);
            this.btnShowFields.Name = "btnShowFields";
            this.btnShowFields.Size = new System.Drawing.Size(147, 23);
            this.btnShowFields.TabIndex = 44;
            this.btnShowFields.Text = "Detect Integration Rate";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 152);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(147, 23);
            this.button1.TabIndex = 45;
            this.button1.Text = "Verify Integration";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(11, 190);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 23);
            this.button2.TabIndex = 46;
            this.button2.Text = "Convert";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(109, 190);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(92, 23);
            this.button3.TabIndex = 47;
            this.button3.Text = "Cancel";
            // 
            // ucConvertVideoToAav
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnShowFields);
            this.Controls.Add(this.gbxSection);
            this.Name = "ucConvertVideoToAav";
            this.Size = new System.Drawing.Size(273, 437);
            this.gbxSection.ResumeLayout(false);
            this.gbxSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFirstFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLastFrame)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxSection;
        private System.Windows.Forms.NumericUpDown nudFirstFrame;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.NumericUpDown nudLastFrame;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Button btnShowFields;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}
