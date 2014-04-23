namespace Tangra.Controls
{
    partial class ucUtcTimePicker
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
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tbxMiliseconds = new System.Windows.Forms.TextBox();
			this.tbxSeconds = new System.Windows.Forms.TextBox();
			this.tbxMinutes = new System.Windows.Forms.TextBox();
			this.tbxHours = new System.Windows.Forms.TextBox();
			this.dtpDate = new System.Windows.Forms.DateTimePicker();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label2.Location = new System.Drawing.Point(129, 5);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(11, 13);
			this.label2.TabIndex = 37;
			this.label2.Text = ":";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label3.Location = new System.Drawing.Point(160, 5);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(11, 13);
			this.label3.TabIndex = 38;
			this.label3.Text = ":";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label4.Location = new System.Drawing.Point(190, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(11, 13);
			this.label4.TabIndex = 39;
			this.label4.Text = ".";
			// 
			// tbxMiliseconds
			// 
			this.tbxMiliseconds.Location = new System.Drawing.Point(200, 3);
			this.tbxMiliseconds.Name = "tbxMiliseconds";
			this.tbxMiliseconds.Size = new System.Drawing.Size(28, 20);
			this.tbxMiliseconds.TabIndex = 40;
			this.tbxMiliseconds.TextChanged += new System.EventHandler(this.tbxMiliseconds_TextChanged);
			this.tbxMiliseconds.Enter += new System.EventHandler(this.tbxInput_Enter);
			// 
			// tbxSeconds
			// 
			this.tbxSeconds.Location = new System.Drawing.Point(169, 3);
			this.tbxSeconds.Name = "tbxSeconds";
			this.tbxSeconds.Size = new System.Drawing.Size(22, 20);
			this.tbxSeconds.TabIndex = 41;
			this.tbxSeconds.TextChanged += new System.EventHandler(this.tbxSeconds_TextChanged);
			this.tbxSeconds.Enter += new System.EventHandler(this.tbxInput_Enter);
			// 
			// tbxMinutes
			// 
			this.tbxMinutes.Location = new System.Drawing.Point(138, 3);
			this.tbxMinutes.Name = "tbxMinutes";
			this.tbxMinutes.Size = new System.Drawing.Size(22, 20);
			this.tbxMinutes.TabIndex = 42;
			this.tbxMinutes.TextChanged += new System.EventHandler(this.tbxMinutes_TextChanged);
			this.tbxMinutes.Enter += new System.EventHandler(this.tbxInput_Enter);
			// 
			// tbxHours
			// 
			this.tbxHours.Location = new System.Drawing.Point(108, 3);
			this.tbxHours.Name = "tbxHours";
			this.tbxHours.Size = new System.Drawing.Size(22, 20);
			this.tbxHours.TabIndex = 43;
			this.tbxHours.TextChanged += new System.EventHandler(this.tbxHours_TextChanged);
			this.tbxHours.Enter += new System.EventHandler(this.tbxInput_Enter);
			// 
			// dtpDate
			// 
			this.dtpDate.Value = new System.DateTime(2011, 8, 17, 21, 15, 2, 0);
			this.dtpDate.Location = new System.Drawing.Point(4, 3);
			this.dtpDate.Name = "dtpDate";
			this.dtpDate.Size = new System.Drawing.Size(98, 20);
			this.dtpDate.TabIndex = 44;
			this.dtpDate.ValueChanged += new System.EventHandler(this.dtpDate_ValueChanged);
			// 
			// ucUtcTimePicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.dtpDate);
			this.Controls.Add(this.tbxHours);
			this.Controls.Add(this.tbxMinutes);
			this.Controls.Add(this.tbxSeconds);
			this.Controls.Add(this.tbxMiliseconds);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Name = "ucUtcTimePicker";
			this.Size = new System.Drawing.Size(233, 26);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbxMiliseconds;
		private System.Windows.Forms.TextBox tbxSeconds;
		private System.Windows.Forms.TextBox tbxMinutes;
		private System.Windows.Forms.TextBox tbxHours;
		private System.Windows.Forms.DateTimePicker dtpDate;
    }
}
