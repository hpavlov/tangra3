namespace Tangra.VideoOperations.Astrometry.MPCReport
{
	partial class frmChooseMPCObject
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
			this.tbxLine = new System.Windows.Forms.TextBox();
			this.tbxObject = new System.Windows.Forms.TextBox();
			this.lblObjectName = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(232, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(98, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "123456789 123";
			// 
			// tbxLine
			// 
			this.tbxLine.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tbxLine.Location = new System.Drawing.Point(233, 35);
			this.tbxLine.Name = "tbxLine";
			this.tbxLine.Size = new System.Drawing.Size(102, 20);
			this.tbxLine.TabIndex = 1;
			this.tbxLine.TextChanged += new System.EventHandler(this.tbxLine_TextChanged);
			this.tbxLine.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbxLine_KeyDown);
			// 
			// tbxObject
			// 
			this.tbxObject.Location = new System.Drawing.Point(8, 36);
			this.tbxObject.Name = "tbxObject";
			this.tbxObject.Size = new System.Drawing.Size(133, 20);
			this.tbxObject.TabIndex = 0;
			this.tbxObject.TextChanged += new System.EventHandler(this.tbxObject_TextChanged);
			// 
			// lblObjectName
			// 
			this.lblObjectName.AutoSize = true;
			this.lblObjectName.Location = new System.Drawing.Point(8, 13);
			this.lblObjectName.Name = "lblObjectName";
			this.lblObjectName.Size = new System.Drawing.Size(97, 13);
			this.lblObjectName.TabIndex = 3;
			this.lblObjectName.Text = "Object Designation";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(163, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "MPC Format";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(260, 89);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Enabled = false;
			this.btnOK.Location = new System.Drawing.Point(179, 89);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 6;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// frmChooseMPCObject
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(351, 130);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblObjectName);
			this.Controls.Add(this.tbxObject);
			this.Controls.Add(this.tbxLine);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmChooseMPCObject";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Specify Object Designation";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbxObject;
		private System.Windows.Forms.Label lblObjectName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		protected internal System.Windows.Forms.TextBox tbxLine;
	}
}