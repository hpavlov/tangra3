namespace Tangra.KweeVanWoerden
{
	partial class frmResults
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
            this.btnSaveFiles = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxT0JD = new System.Windows.Forms.TextBox();
            this.tbxT0Uncertainty = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxT0 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lblCalcStatus = new System.Windows.Forms.Label();
            this.tbxErrorMessage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxTotalObs = new System.Windows.Forms.TextBox();
            this.tbxIncludedObs = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tbxT0HJD = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbxUncertaintyInSec = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnCalcHJD = new System.Windows.Forms.Button();
            this.picGraph = new System.Windows.Forms.PictureBox();
            this.picGraphPoly = new System.Windows.Forms.PictureBox();
            this.tbxT0UT = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tbxT0UT_CF = new System.Windows.Forms.TextBox();
            this.tbxUncertaintyInSec_CF = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbxT0JD_CF = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnCalcHJD2 = new System.Windows.Forms.Button();
            this.tbxT0HJD_CF = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbxT0Uncertainty_CF = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.nudM0 = new System.Windows.Forms.NumericUpDown();
            this.nudC = new System.Windows.Forms.NumericUpDown();
            this.nudD = new System.Windows.Forms.NumericUpDown();
            this.nudG = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.picGraph)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGraphPoly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudM0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudG)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSaveFiles
            // 
            this.btnSaveFiles.Location = new System.Drawing.Point(878, 213);
            this.btnSaveFiles.Name = "btnSaveFiles";
            this.btnSaveFiles.Size = new System.Drawing.Size(143, 23);
            this.btnSaveFiles.TabIndex = 0;
            this.btnSaveFiles.Text = "Save Calculation Details";
            this.btnSaveFiles.UseVisualStyleBackColor = true;
            this.btnSaveFiles.Click += new System.EventHandler(this.btnSaveFiles_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(540, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Time of Minimum (JD)";
            // 
            // tbxT0JD
            // 
            this.tbxT0JD.BackColor = System.Drawing.SystemColors.Info;
            this.tbxT0JD.Location = new System.Drawing.Point(654, 78);
            this.tbxT0JD.Name = "tbxT0JD";
            this.tbxT0JD.ReadOnly = true;
            this.tbxT0JD.Size = new System.Drawing.Size(161, 20);
            this.tbxT0JD.TabIndex = 2;
            // 
            // tbxT0Uncertainty
            // 
            this.tbxT0Uncertainty.BackColor = System.Drawing.Color.Honeydew;
            this.tbxT0Uncertainty.Location = new System.Drawing.Point(654, 220);
            this.tbxT0Uncertainty.Name = "tbxT0Uncertainty";
            this.tbxT0Uncertainty.ReadOnly = true;
            this.tbxT0Uncertainty.Size = new System.Drawing.Size(161, 20);
            this.tbxT0Uncertainty.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(561, 223);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Uncertainty (JD)";
            // 
            // tbxT0
            // 
            this.tbxT0.BackColor = System.Drawing.SystemColors.Info;
            this.tbxT0.Location = new System.Drawing.Point(878, 103);
            this.tbxT0.Name = "tbxT0";
            this.tbxT0.ReadOnly = true;
            this.tbxT0.Size = new System.Drawing.Size(161, 20);
            this.tbxT0.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(849, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "T0";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(620, 416);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(131, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // lblCalcStatus
            // 
            this.lblCalcStatus.AutoSize = true;
            this.lblCalcStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCalcStatus.ForeColor = System.Drawing.Color.Green;
            this.lblCalcStatus.Location = new System.Drawing.Point(9, 9);
            this.lblCalcStatus.Name = "lblCalcStatus";
            this.lblCalcStatus.Size = new System.Drawing.Size(185, 13);
            this.lblCalcStatus.TabIndex = 8;
            this.lblCalcStatus.Text = "The calculation was successful";
            // 
            // tbxErrorMessage
            // 
            this.tbxErrorMessage.BackColor = System.Drawing.SystemColors.Info;
            this.tbxErrorMessage.ForeColor = System.Drawing.Color.Red;
            this.tbxErrorMessage.Location = new System.Drawing.Point(12, 25);
            this.tbxErrorMessage.Multiline = true;
            this.tbxErrorMessage.Name = "tbxErrorMessage";
            this.tbxErrorMessage.ReadOnly = true;
            this.tbxErrorMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxErrorMessage.Size = new System.Drawing.Size(580, 35);
            this.tbxErrorMessage.TabIndex = 9;
            this.tbxErrorMessage.Text = "The error message\r\ngoes here";
            this.tbxErrorMessage.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(549, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Total Data Point";
            // 
            // tbxTotalObs
            // 
            this.tbxTotalObs.BackColor = System.Drawing.SystemColors.Info;
            this.tbxTotalObs.Location = new System.Drawing.Point(654, 129);
            this.tbxTotalObs.Name = "tbxTotalObs";
            this.tbxTotalObs.ReadOnly = true;
            this.tbxTotalObs.Size = new System.Drawing.Size(161, 20);
            this.tbxTotalObs.TabIndex = 11;
            // 
            // tbxIncludedObs
            // 
            this.tbxIncludedObs.BackColor = System.Drawing.SystemColors.Info;
            this.tbxIncludedObs.Location = new System.Drawing.Point(654, 155);
            this.tbxIncludedObs.Name = "tbxIncludedObs";
            this.tbxIncludedObs.ReadOnly = true;
            this.tbxIncludedObs.Size = new System.Drawing.Size(161, 20);
            this.tbxIncludedObs.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(532, 158);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Included Observations";
            // 
            // tbxT0HJD
            // 
            this.tbxT0HJD.BackColor = System.Drawing.Color.Honeydew;
            this.tbxT0HJD.Location = new System.Drawing.Point(654, 195);
            this.tbxT0HJD.Name = "tbxT0HJD";
            this.tbxT0HJD.ReadOnly = true;
            this.tbxT0HJD.Size = new System.Drawing.Size(161, 20);
            this.tbxT0HJD.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(531, 198);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Time of Minimum (HJD)";
            // 
            // tbxUncertaintyInSec
            // 
            this.tbxUncertaintyInSec.BackColor = System.Drawing.SystemColors.Info;
            this.tbxUncertaintyInSec.Location = new System.Drawing.Point(654, 103);
            this.tbxUncertaintyInSec.Name = "tbxUncertaintyInSec";
            this.tbxUncertaintyInSec.ReadOnly = true;
            this.tbxUncertaintyInSec.Size = new System.Drawing.Size(161, 20);
            this.tbxUncertaintyInSec.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(565, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Uncertainty (sec)";
            // 
            // btnCalcHJD
            // 
            this.btnCalcHJD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalcHJD.ForeColor = System.Drawing.Color.Red;
            this.btnCalcHJD.Location = new System.Drawing.Point(654, 193);
            this.btnCalcHJD.Name = "btnCalcHJD";
            this.btnCalcHJD.Size = new System.Drawing.Size(161, 23);
            this.btnCalcHJD.TabIndex = 19;
            this.btnCalcHJD.Text = "Calculate HJD";
            this.btnCalcHJD.UseVisualStyleBackColor = true;
            this.btnCalcHJD.Click += new System.EventHandler(this.btnCalcHJD_Click);
            // 
            // picGraph
            // 
            this.picGraph.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picGraph.Location = new System.Drawing.Point(12, 66);
            this.picGraph.Name = "picGraph";
            this.picGraph.Size = new System.Drawing.Size(504, 182);
            this.picGraph.TabIndex = 20;
            this.picGraph.TabStop = false;
            // 
            // picGraphPoly
            // 
            this.picGraphPoly.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picGraphPoly.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picGraphPoly.Location = new System.Drawing.Point(12, 257);
            this.picGraphPoly.Name = "picGraphPoly";
            this.picGraphPoly.Size = new System.Drawing.Size(504, 182);
            this.picGraphPoly.TabIndex = 21;
            this.picGraphPoly.TabStop = false;
            // 
            // tbxT0UT
            // 
            this.tbxT0UT.BackColor = System.Drawing.SystemColors.Info;
            this.tbxT0UT.Location = new System.Drawing.Point(878, 78);
            this.tbxT0UT.Name = "tbxT0UT";
            this.tbxT0UT.ReadOnly = true;
            this.tbxT0UT.Size = new System.Drawing.Size(161, 20);
            this.tbxT0UT.TabIndex = 22;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(844, 81);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(28, 13);
            this.label8.TabIndex = 23;
            this.label8.Text = "(UT)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(844, 269);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 31;
            this.label9.Text = "(UT)";
            // 
            // tbxT0UT_CF
            // 
            this.tbxT0UT_CF.BackColor = System.Drawing.SystemColors.Info;
            this.tbxT0UT_CF.Location = new System.Drawing.Point(878, 266);
            this.tbxT0UT_CF.Name = "tbxT0UT_CF";
            this.tbxT0UT_CF.ReadOnly = true;
            this.tbxT0UT_CF.Size = new System.Drawing.Size(161, 20);
            this.tbxT0UT_CF.TabIndex = 30;
            // 
            // tbxUncertaintyInSec_CF
            // 
            this.tbxUncertaintyInSec_CF.BackColor = System.Drawing.SystemColors.Info;
            this.tbxUncertaintyInSec_CF.Location = new System.Drawing.Point(654, 291);
            this.tbxUncertaintyInSec_CF.Name = "tbxUncertaintyInSec_CF";
            this.tbxUncertaintyInSec_CF.ReadOnly = true;
            this.tbxUncertaintyInSec_CF.Size = new System.Drawing.Size(161, 20);
            this.tbxUncertaintyInSec_CF.TabIndex = 29;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(565, 294);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(87, 13);
            this.label10.TabIndex = 28;
            this.label10.Text = "Uncertainty (sec)";
            // 
            // tbxT0JD_CF
            // 
            this.tbxT0JD_CF.BackColor = System.Drawing.SystemColors.Info;
            this.tbxT0JD_CF.Location = new System.Drawing.Point(654, 266);
            this.tbxT0JD_CF.Name = "tbxT0JD_CF";
            this.tbxT0JD_CF.ReadOnly = true;
            this.tbxT0JD_CF.Size = new System.Drawing.Size(161, 20);
            this.tbxT0JD_CF.TabIndex = 25;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(540, 269);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(108, 13);
            this.label12.TabIndex = 24;
            this.label12.Text = "Time of Minimum (JD)";
            // 
            // btnCalcHJD2
            // 
            this.btnCalcHJD2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalcHJD2.ForeColor = System.Drawing.Color.Red;
            this.btnCalcHJD2.Location = new System.Drawing.Point(654, 331);
            this.btnCalcHJD2.Name = "btnCalcHJD2";
            this.btnCalcHJD2.Size = new System.Drawing.Size(161, 23);
            this.btnCalcHJD2.TabIndex = 36;
            this.btnCalcHJD2.Text = "Calculate HJD";
            this.btnCalcHJD2.UseVisualStyleBackColor = true;
            this.btnCalcHJD2.Click += new System.EventHandler(this.btnCalcHJD_Click);
            // 
            // tbxT0HJD_CF
            // 
            this.tbxT0HJD_CF.BackColor = System.Drawing.Color.Honeydew;
            this.tbxT0HJD_CF.Location = new System.Drawing.Point(654, 333);
            this.tbxT0HJD_CF.Name = "tbxT0HJD_CF";
            this.tbxT0HJD_CF.ReadOnly = true;
            this.tbxT0HJD_CF.Size = new System.Drawing.Size(161, 20);
            this.tbxT0HJD_CF.TabIndex = 35;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(531, 336);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(116, 13);
            this.label13.TabIndex = 34;
            this.label13.Text = "Time of Minimum (HJD)";
            // 
            // tbxT0Uncertainty_CF
            // 
            this.tbxT0Uncertainty_CF.BackColor = System.Drawing.Color.Honeydew;
            this.tbxT0Uncertainty_CF.Location = new System.Drawing.Point(654, 358);
            this.tbxT0Uncertainty_CF.Name = "tbxT0Uncertainty_CF";
            this.tbxT0Uncertainty_CF.ReadOnly = true;
            this.tbxT0Uncertainty_CF.Size = new System.Drawing.Size(161, 20);
            this.tbxT0Uncertainty_CF.TabIndex = 33;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(561, 361);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(83, 13);
            this.label14.TabIndex = 32;
            this.label14.Text = "Uncertainty (JD)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.ForeColor = System.Drawing.Color.Lime;
            this.label11.Location = new System.Drawing.Point(15, 69);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(118, 13);
            this.label11.TabIndex = 37;
            this.label11.Text = "Kwee-van Woerden";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label15.ForeColor = System.Drawing.Color.Lime;
            this.label15.Location = new System.Drawing.Point(16, 261);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(75, 13);
            this.label15.TabIndex = 38;
            this.label15.Text = "Curve Fiting";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(12, 251);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1029, 2);
            this.panel1.TabIndex = 39;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(890, 397);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(54, 23);
            this.button2.TabIndex = 40;
            this.button2.Text = "Replot";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // nudM0
            // 
            this.nudM0.DecimalPlaces = 6;
            this.nudM0.Location = new System.Drawing.Point(878, 298);
            this.nudM0.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nudM0.Name = "nudM0";
            this.nudM0.Size = new System.Drawing.Size(120, 20);
            this.nudM0.TabIndex = 41;
            // 
            // nudC
            // 
            this.nudC.DecimalPlaces = 6;
            this.nudC.Location = new System.Drawing.Point(878, 320);
            this.nudC.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nudC.Name = "nudC";
            this.nudC.Size = new System.Drawing.Size(120, 20);
            this.nudC.TabIndex = 42;
            // 
            // nudD
            // 
            this.nudD.DecimalPlaces = 6;
            this.nudD.Location = new System.Drawing.Point(878, 342);
            this.nudD.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nudD.Name = "nudD";
            this.nudD.Size = new System.Drawing.Size(120, 20);
            this.nudD.TabIndex = 43;
            // 
            // nudG
            // 
            this.nudG.DecimalPlaces = 6;
            this.nudG.Location = new System.Drawing.Point(878, 364);
            this.nudG.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nudG.Name = "nudG";
            this.nudG.Size = new System.Drawing.Size(120, 20);
            this.nudG.TabIndex = 44;
            // 
            // frmResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1058, 455);
            this.Controls.Add(this.nudG);
            this.Controls.Add(this.nudD);
            this.Controls.Add(this.nudC);
            this.Controls.Add(this.nudM0);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.btnCalcHJD2);
            this.Controls.Add(this.tbxT0HJD_CF);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.tbxT0Uncertainty_CF);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tbxT0UT_CF);
            this.Controls.Add(this.tbxUncertaintyInSec_CF);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tbxT0JD_CF);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tbxT0UT);
            this.Controls.Add(this.picGraphPoly);
            this.Controls.Add(this.picGraph);
            this.Controls.Add(this.btnCalcHJD);
            this.Controls.Add(this.tbxUncertaintyInSec);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbxT0HJD);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbxIncludedObs);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbxTotalObs);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbxErrorMessage);
            this.Controls.Add(this.lblCalcStatus);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbxT0);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbxT0Uncertainty);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbxT0JD);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSaveFiles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmResults";
            this.Text = "Occulting Binary Minimum";
            this.Load += new System.EventHandler(this.frmResults_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picGraph)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGraphPoly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudM0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudG)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnSaveFiles;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbxT0JD;
		private System.Windows.Forms.TextBox tbxT0Uncertainty;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbxT0;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label lblCalcStatus;
		private System.Windows.Forms.TextBox tbxErrorMessage;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbxTotalObs;
		private System.Windows.Forms.TextBox tbxIncludedObs;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox tbxT0HJD;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbxUncertaintyInSec;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnCalcHJD;
        private System.Windows.Forms.PictureBox picGraph;
        private System.Windows.Forms.PictureBox picGraphPoly;
        private System.Windows.Forms.TextBox tbxT0UT;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbxT0UT_CF;
        private System.Windows.Forms.TextBox tbxUncertaintyInSec_CF;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbxT0JD_CF;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnCalcHJD2;
        private System.Windows.Forms.TextBox tbxT0HJD_CF;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbxT0Uncertainty_CF;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.NumericUpDown nudM0;
		private System.Windows.Forms.NumericUpDown nudC;
		private System.Windows.Forms.NumericUpDown nudD;
		private System.Windows.Forms.NumericUpDown nudG;
	}
}