namespace Tangra.Addins
{
	partial class frmConfigureAddin
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rbClear = new System.Windows.Forms.RadioButton();
			this.rbV = new System.Windows.Forms.RadioButton();
			this.rbR = new System.Windows.Forms.RadioButton();
			this.rbB = new System.Windows.Forms.RadioButton();
			this.rbJ = new System.Windows.Forms.RadioButton();
			this.rbK = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rbK);
			this.groupBox1.Controls.Add(this.rbJ);
			this.groupBox1.Controls.Add(this.rbB);
			this.groupBox1.Controls.Add(this.rbR);
			this.groupBox1.Controls.Add(this.rbV);
			this.groupBox1.Controls.Add(this.rbClear);
			this.groupBox1.Location = new System.Drawing.Point(21, 23);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(201, 115);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Mesurements Exported Magnitude";
			this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
			// 
			// rbClear
			// 
			this.rbClear.AutoSize = true;
			this.rbClear.Checked = true;
			this.rbClear.Location = new System.Drawing.Point(28, 34);
			this.rbClear.Name = "rbClear";
			this.rbClear.Size = new System.Drawing.Size(49, 17);
			this.rbClear.TabIndex = 0;
			this.rbClear.TabStop = true;
			this.rbClear.Text = "Clear";
			this.rbClear.UseVisualStyleBackColor = true;
			// 
			// rbV
			// 
			this.rbV.AutoSize = true;
			this.rbV.Location = new System.Drawing.Point(28, 57);
			this.rbV.Name = "rbV";
			this.rbV.Size = new System.Drawing.Size(49, 17);
			this.rbV.TabIndex = 1;
			this.rbV.Text = "V (g\')";
			this.rbV.UseVisualStyleBackColor = true;
			// 
			// rbR
			// 
			this.rbR.AutoSize = true;
			this.rbR.Location = new System.Drawing.Point(28, 79);
			this.rbR.Name = "rbR";
			this.rbR.Size = new System.Drawing.Size(47, 17);
			this.rbR.TabIndex = 2;
			this.rbR.Text = "R (r\')";
			this.rbR.UseVisualStyleBackColor = true;
			// 
			// rbB
			// 
			this.rbB.AutoSize = true;
			this.rbB.Location = new System.Drawing.Point(103, 34);
			this.rbB.Name = "rbB";
			this.rbB.Size = new System.Drawing.Size(32, 17);
			this.rbB.TabIndex = 3;
			this.rbB.Text = "B";
			this.rbB.UseVisualStyleBackColor = true;
			// 
			// rbJ
			// 
			this.rbJ.AutoSize = true;
			this.rbJ.Location = new System.Drawing.Point(103, 57);
			this.rbJ.Name = "rbJ";
			this.rbJ.Size = new System.Drawing.Size(30, 17);
			this.rbJ.TabIndex = 4;
			this.rbJ.Text = "J";
			this.rbJ.UseVisualStyleBackColor = true;
			// 
			// rbK
			// 
			this.rbK.AutoSize = true;
			this.rbK.Location = new System.Drawing.Point(103, 80);
			this.rbK.Name = "rbK";
			this.rbK.Size = new System.Drawing.Size(32, 17);
			this.rbK.TabIndex = 5;
			this.rbK.Text = "K";
			this.rbK.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(43, 165);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(124, 164);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// frmConfigureAddin
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(242, 200);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmConfigureAddin";
			this.Text = "Tangra Addins Configuration";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rbClear;
		private System.Windows.Forms.RadioButton rbB;
		private System.Windows.Forms.RadioButton rbR;
		private System.Windows.Forms.RadioButton rbV;
		private System.Windows.Forms.RadioButton rbK;
		private System.Windows.Forms.RadioButton rbJ;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}