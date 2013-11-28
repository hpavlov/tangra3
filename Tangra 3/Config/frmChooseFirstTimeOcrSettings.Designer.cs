namespace Tangra.Config
{
	partial class frmChooseFirstTimeOcrSettings
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChooseFirstTimeOcrSettings));
			this.rbIOTAVTI = new System.Windows.Forms.RadioButton();
			this.rbOther = new System.Windows.Forms.RadioButton();
			this.rbMany = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// rbIOTAVTI
			// 
			this.rbIOTAVTI.AutoSize = true;
			this.rbIOTAVTI.Checked = true;
			this.rbIOTAVTI.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.rbIOTAVTI.Location = new System.Drawing.Point(43, 69);
			this.rbIOTAVTI.Name = "rbIOTAVTI";
			this.rbIOTAVTI.Size = new System.Drawing.Size(202, 17);
			this.rbIOTAVTI.TabIndex = 0;
			this.rbIOTAVTI.TabStop = true;
			this.rbIOTAVTI.Text = "My videos mostly use IOTA-VTI";
			this.rbIOTAVTI.UseVisualStyleBackColor = true;
			// 
			// rbOther
			// 
			this.rbOther.AutoSize = true;
			this.rbOther.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.rbOther.Location = new System.Drawing.Point(43, 127);
			this.rbOther.Name = "rbOther";
			this.rbOther.Size = new System.Drawing.Size(286, 17);
			this.rbOther.TabIndex = 1;
			this.rbOther.Text = "My video mostly use other video time inserters";
			this.rbOther.UseVisualStyleBackColor = true;
			// 
			// rbMany
			// 
			this.rbMany.AutoSize = true;
			this.rbMany.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.rbMany.Location = new System.Drawing.Point(43, 182);
			this.rbMany.Name = "rbMany";
			this.rbMany.Size = new System.Drawing.Size(304, 17);
			this.rbMany.TabIndex = 2;
			this.rbMany.Text = "My videos use IOTA-VTI and other time inserters ";
			this.rbMany.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(25, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(397, 26);
			this.label1.TabIndex = 3;
			this.label1.Text = "    To get the most of Tangra 3 and at the same time the least interruption, plea" +
    "se choose which configuration most closely matches your situation:";
			// 
			// btnOK
			// 
			this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnOK.Location = new System.Drawing.Point(169, 341);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(103, 23);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "Save && Close";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(59, 93);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(352, 26);
			this.label2.TabIndex = 5;
			this.label2.Text = "Tangra will try to read your IOTA-VTI timestamp every time you process a video";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(60, 149);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(339, 17);
			this.label3.TabIndex = 6;
			this.label3.Text = "Tangra will not try to read your timestamp";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(59, 206);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(352, 31);
			this.label4.TabIndex = 7;
			this.label4.Text = "Each time you process a video, Tangra will ask you whether to try and read your I" +
    "OTA-VTI timestamp ";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(40, 258);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(382, 65);
			this.label6.TabIndex = 9;
			this.label6.Text = "Please Note:\r\n\r\n- You can change this configuration later from the Settings\r\n- Ta" +
    "ngra cannot read your timestmp if video fields are missing";
			// 
			// frmChooseFirstTimeOcrSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(460, 376);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.rbMany);
			this.Controls.Add(this.rbOther);
			this.Controls.Add(this.rbIOTAVTI);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmChooseFirstTimeOcrSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Choose OSD Timestamp Reader Settings";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton rbIOTAVTI;
		private System.Windows.Forms.RadioButton rbOther;
		private System.Windows.Forms.RadioButton rbMany;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
	}
}