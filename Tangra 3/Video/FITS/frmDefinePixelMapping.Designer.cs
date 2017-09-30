namespace Tangra.Video.FITS
{
	partial class frmDefinePixelMapping
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
            this.components = new System.ComponentModel.Container();
            this.picHistogram = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rb8Bit = new System.Windows.Forms.RadioButton();
            this.rb12Bit = new System.Windows.Forms.RadioButton();
            this.rb14Bit = new System.Windows.Forms.RadioButton();
            this.rb16Bit = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.nudZeroPoint = new System.Windows.Forms.NumericUpDown();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnMinValue = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.rbScaleLinear = new System.Windows.Forms.RadioButton();
            this.rbScaleLog = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudZeroPoint)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picHistogram
            // 
            this.picHistogram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picHistogram.Location = new System.Drawing.Point(12, 96);
            this.picHistogram.Name = "picHistogram";
            this.picHistogram.Size = new System.Drawing.Size(504, 112);
            this.picHistogram.TabIndex = 2;
            this.picHistogram.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(261, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Tangra requires unsigned integer pixel values to work.";
            // 
            // rb8Bit
            // 
            this.rb8Bit.AutoSize = true;
            this.rb8Bit.Location = new System.Drawing.Point(6, 11);
            this.rb8Bit.Name = "rb8Bit";
            this.rb8Bit.Size = new System.Drawing.Size(45, 17);
            this.rb8Bit.TabIndex = 17;
            this.rb8Bit.TabStop = true;
            this.rb8Bit.Text = "8 bit";
            this.rb8Bit.UseVisualStyleBackColor = true;
            this.rb8Bit.CheckedChanged += new System.EventHandler(this.rb8Bit_CheckedChanged);
            // 
            // rb12Bit
            // 
            this.rb12Bit.AutoSize = true;
            this.rb12Bit.Location = new System.Drawing.Point(76, 11);
            this.rb12Bit.Name = "rb12Bit";
            this.rb12Bit.Size = new System.Drawing.Size(51, 17);
            this.rb12Bit.TabIndex = 18;
            this.rb12Bit.TabStop = true;
            this.rb12Bit.Text = "12 bit";
            this.rb12Bit.UseVisualStyleBackColor = true;
            this.rb12Bit.CheckedChanged += new System.EventHandler(this.rb12Bit_CheckedChanged);
            // 
            // rb14Bit
            // 
            this.rb14Bit.AutoSize = true;
            this.rb14Bit.Location = new System.Drawing.Point(146, 11);
            this.rb14Bit.Name = "rb14Bit";
            this.rb14Bit.Size = new System.Drawing.Size(51, 17);
            this.rb14Bit.TabIndex = 19;
            this.rb14Bit.TabStop = true;
            this.rb14Bit.Text = "14 bit";
            this.rb14Bit.UseVisualStyleBackColor = true;
            this.rb14Bit.CheckedChanged += new System.EventHandler(this.rb14Bit_CheckedChanged);
            // 
            // rb16Bit
            // 
            this.rb16Bit.AutoSize = true;
            this.rb16Bit.Location = new System.Drawing.Point(216, 11);
            this.rb16Bit.Name = "rb16Bit";
            this.rb16Bit.Size = new System.Drawing.Size(51, 17);
            this.rb16Bit.TabIndex = 20;
            this.rb16Bit.TabStop = true;
            this.rb16Bit.Text = "16 bit";
            this.rb16Bit.UseVisualStyleBackColor = true;
            this.rb16Bit.CheckedChanged += new System.EventHandler(this.rb16Bit_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(332, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Zero Point:";
            // 
            // nudZeroPoint
            // 
            this.nudZeroPoint.Location = new System.Drawing.Point(397, 58);
            this.nudZeroPoint.Name = "nudZeroPoint";
            this.nudZeroPoint.Size = new System.Drawing.Size(81, 20);
            this.nudZeroPoint.TabIndex = 22;
            this.nudZeroPoint.ValueChanged += new System.EventHandler(this.nudZeroPoint_ValueChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(360, 220);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 24;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(441, 220);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 23;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnMinValue
            // 
            this.btnMinValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMinValue.Location = new System.Drawing.Point(484, 57);
            this.btnMinValue.Name = "btnMinValue";
            this.btnMinValue.Size = new System.Drawing.Size(32, 21);
            this.btnMinValue.TabIndex = 25;
            this.btnMinValue.Text = "min";
            this.toolTip1.SetToolTip(this.btnMinValue, "Use MinPixel value as zero point");
            this.btnMinValue.UseVisualStyleBackColor = true;
            this.btnMinValue.Click += new System.EventHandler(this.btnMinValue_Click);
            // 
            // rbScaleLinear
            // 
            this.rbScaleLinear.AutoSize = true;
            this.rbScaleLinear.Location = new System.Drawing.Point(130, 223);
            this.rbScaleLinear.Name = "rbScaleLinear";
            this.rbScaleLinear.Size = new System.Drawing.Size(54, 17);
            this.rbScaleLinear.TabIndex = 28;
            this.rbScaleLinear.Text = "Linear";
            this.rbScaleLinear.UseVisualStyleBackColor = true;
            this.rbScaleLinear.CheckedChanged += new System.EventHandler(this.rbScaleLinear_CheckedChanged);
            // 
            // rbScaleLog
            // 
            this.rbScaleLog.AutoSize = true;
            this.rbScaleLog.Checked = true;
            this.rbScaleLog.Location = new System.Drawing.Point(51, 223);
            this.rbScaleLog.Name = "rbScaleLog";
            this.rbScaleLog.Size = new System.Drawing.Size(79, 17);
            this.rbScaleLog.TabIndex = 27;
            this.rbScaleLog.TabStop = true;
            this.rbScaleLog.Text = "Logarithmic";
            this.rbScaleLog.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 225);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Scale:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rb8Bit);
            this.groupBox1.Controls.Add(this.rb12Bit);
            this.groupBox1.Controls.Add(this.rb14Bit);
            this.groupBox1.Controls.Add(this.rb16Bit);
            this.groupBox1.Location = new System.Drawing.Point(12, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(281, 34);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(439, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "Define the mapping from the FITS image pixel values to the Tangra pixel value ran" +
    "ge below.";
            // 
            // frmDefinePixelMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 255);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rbScaleLinear);
            this.Controls.Add(this.rbScaleLog);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnMinValue);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.nudZeroPoint);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picHistogram);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmDefinePixelMapping";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Define Pixel Mapping";
            ((System.ComponentModel.ISupportInitialize)(this.picHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudZeroPoint)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.PictureBox picHistogram;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rb8Bit;
        private System.Windows.Forms.RadioButton rb12Bit;
        private System.Windows.Forms.RadioButton rb14Bit;
        private System.Windows.Forms.RadioButton rb16Bit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudZeroPoint;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnMinValue;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.RadioButton rbScaleLinear;
        private System.Windows.Forms.RadioButton rbScaleLog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
	}
}