namespace Tangra.KweeVanWoerden
{
    partial class frmHJDCalculation
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
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxVarStar = new System.Windows.Forms.TextBox();
            this.btnFindGSVSStar = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxRA = new System.Windows.Forms.TextBox();
            this.tbxDE = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tbxCorrection = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CustomFormat = "dd MMM yyyy";
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker.Location = new System.Drawing.Point(120, 19);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(105, 20);
            this.dateTimePicker.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Date of observation:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Variable Star:";
            // 
            // tbxVarStar
            // 
            this.tbxVarStar.Location = new System.Drawing.Point(120, 50);
            this.tbxVarStar.Name = "tbxVarStar";
            this.tbxVarStar.Size = new System.Drawing.Size(105, 20);
            this.tbxVarStar.TabIndex = 3;
            // 
            // btnFindGSVSStar
            // 
            this.btnFindGSVSStar.Location = new System.Drawing.Point(231, 50);
            this.btnFindGSVSStar.Name = "btnFindGSVSStar";
            this.btnFindGSVSStar.Size = new System.Drawing.Size(41, 23);
            this.btnFindGSVSStar.TabIndex = 4;
            this.btnFindGSVSStar.Text = "Set";
            this.btnFindGSVSStar.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(53, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "RA (J2000):";
            // 
            // tbxRA
            // 
            this.tbxRA.Location = new System.Drawing.Point(120, 84);
            this.tbxRA.Name = "tbxRA";
            this.tbxRA.Size = new System.Drawing.Size(105, 20);
            this.tbxRA.TabIndex = 7;
            // 
            // tbxDE
            // 
            this.tbxDE.Location = new System.Drawing.Point(120, 121);
            this.tbxDE.Name = "tbxDE";
            this.tbxDE.Size = new System.Drawing.Size(105, 20);
            this.tbxDE.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(53, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "DE (J2000):";
            // 
            // btnCalculate
            // 
            this.btnCalculate.Location = new System.Drawing.Point(56, 220);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(105, 23);
            this.btnCalculate.TabIndex = 10;
            this.btnCalculate.Text = "Calculate";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 176);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Correction (JD):";
            // 
            // tbxCorrection
            // 
            this.tbxCorrection.Location = new System.Drawing.Point(120, 173);
            this.tbxCorrection.Name = "tbxCorrection";
            this.tbxCorrection.ReadOnly = true;
            this.tbxCorrection.Size = new System.Drawing.Size(105, 20);
            this.tbxCorrection.TabIndex = 12;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(183, 220);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(105, 23);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(39, 154);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(186, 1);
            this.panel1.TabIndex = 14;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(14, 208);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(302, 2);
            this.panel2.TabIndex = 15;
            // 
            // frmHJDCalculation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 258);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.tbxCorrection);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.tbxDE);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbxRA);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnFindGSVSStar);
            this.Controls.Add(this.tbxVarStar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmHJDCalculation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Calculate HJD";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxVarStar;
        private System.Windows.Forms.Button btnFindGSVSStar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxRA;
        private System.Windows.Forms.TextBox tbxDE;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbxCorrection;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}