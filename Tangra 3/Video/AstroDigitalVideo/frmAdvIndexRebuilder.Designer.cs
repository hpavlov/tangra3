namespace Tangra.Video.AstroDigitalVideo
{
	partial class frmAdvIndexRebuilder
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAdvIndexRebuilder));
			this.pbar1 = new System.Windows.Forms.ProgressBar();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblProcess = new System.Windows.Forms.Label();
			this.lblStatus = new System.Windows.Forms.Label();
			this.actionTimer = new System.Windows.Forms.Timer(this.components);
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.SuspendLayout();
			// 
			// pbar1
			// 
			this.pbar1.Location = new System.Drawing.Point(21, 31);
			this.pbar1.Name = "pbar1";
			this.pbar1.Step = 1;
			this.pbar1.Size = new System.Drawing.Size(274, 18);
			this.pbar1.TabIndex = 18;
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(320, 26);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 17;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// lblProcess
			// 
			this.lblProcess.AutoSize = true;
			this.lblProcess.Location = new System.Drawing.Point(21, 12);
			this.lblProcess.Name = "lblProcess";
			this.lblProcess.Size = new System.Drawing.Size(145, 13);
			this.lblProcess.TabIndex = 19;
			this.lblProcess.Text = "Searching for ADV frames ...";
			// 
			// lblStatus
			// 
			this.lblStatus.AutoSize = true;
			this.lblStatus.Location = new System.Drawing.Point(21, 55);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(125, 13);
			this.lblStatus.TabIndex = 20;
			this.lblStatus.Text = "0 potential frames found";
			// 
			// actionTimer
			// 
			this.actionTimer.Interval = 10;
			this.actionTimer.Tick += new System.EventHandler(this.actionTimer_Tick);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "adv";
			this.saveFileDialog.Filter = "Astro Digital Video (*.adv)|*.adv";
			this.saveFileDialog.Title = "Save recovered ADV file ...";
			// 
			// frmAdvIndexRebuilder
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(412, 85);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.lblProcess);
			this.Controls.Add(this.pbar1);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmAdvIndexRebuilder";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Repair ADV File";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAdvIndexRebuilder_FormClosing);
			this.Load += new System.EventHandler(this.frmAdvIndexRebuilder_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar pbar1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblProcess;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Timer actionTimer;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
	}
}