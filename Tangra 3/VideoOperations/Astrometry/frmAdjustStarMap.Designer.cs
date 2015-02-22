namespace Tangra.VideoOperations.Astrometry
{
	partial class frmAdjustStarMap
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAdjustStarMap));
			this.pnlControls = new System.Windows.Forms.Panel();
			this.rbAuto = new System.Windows.Forms.RadioButton();
			this.rbManual = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.lblDepth = new System.Windows.Forms.Label();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnDown = new System.Windows.Forms.Button();
			this.lblFrameNo = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.hsrcFrames = new System.Windows.Forms.HScrollBar();
			this.label1 = new System.Windows.Forms.Label();
			this.pnlImage = new System.Windows.Forms.Panel();
			this.imgFrame = new System.Windows.Forms.PictureBox();
			this.pnlControls.SuspendLayout();
			this.pnlImage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imgFrame)).BeginInit();
			this.SuspendLayout();
			// 
			// pnlControls
			// 
			this.pnlControls.Controls.Add(this.rbAuto);
			this.pnlControls.Controls.Add(this.rbManual);
			this.pnlControls.Controls.Add(this.btnOK);
			this.pnlControls.Controls.Add(this.lblDepth);
			this.pnlControls.Controls.Add(this.btnUp);
			this.pnlControls.Controls.Add(this.btnDown);
			this.pnlControls.Controls.Add(this.lblFrameNo);
			this.pnlControls.Controls.Add(this.label2);
			this.pnlControls.Controls.Add(this.hsrcFrames);
			this.pnlControls.Controls.Add(this.label1);
			this.pnlControls.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlControls.Location = new System.Drawing.Point(0, 400);
			this.pnlControls.Name = "pnlControls";
			this.pnlControls.Size = new System.Drawing.Size(600, 58);
			this.pnlControls.TabIndex = 1;
			// 
			// rbAuto
			// 
			this.rbAuto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.rbAuto.AutoSize = true;
			this.rbAuto.Location = new System.Drawing.Point(486, 2);
			this.rbAuto.Name = "rbAuto";
			this.rbAuto.Size = new System.Drawing.Size(46, 17);
			this.rbAuto.TabIndex = 83;
			this.rbAuto.Text = "auto";
			this.rbAuto.UseVisualStyleBackColor = true;
			this.rbAuto.CheckedChanged += new System.EventHandler(this.rbAuto_CheckedChanged);
			// 
			// rbManual
			// 
			this.rbManual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.rbManual.AutoSize = true;
			this.rbManual.Checked = true;
			this.rbManual.Location = new System.Drawing.Point(426, 2);
			this.rbManual.Name = "rbManual";
			this.rbManual.Size = new System.Drawing.Size(59, 17);
			this.rbManual.TabIndex = 82;
			this.rbManual.TabStop = true;
			this.rbManual.Text = "manual";
			this.rbManual.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(528, 23);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(60, 23);
			this.btnOK.TabIndex = 81;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// lblDepth
			// 
			this.lblDepth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lblDepth.AutoSize = true;
			this.lblDepth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblDepth.Location = new System.Drawing.Point(437, 28);
			this.lblDepth.Name = "lblDepth";
			this.lblDepth.Size = new System.Drawing.Size(14, 13);
			this.lblDepth.TabIndex = 80;
			this.lblDepth.Text = "2";
			// 
			// btnUp
			// 
			this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUp.Location = new System.Drawing.Point(456, 23);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(25, 23);
			this.btnUp.TabIndex = 79;
			this.btnUp.Text = ">";
			this.btnUp.UseVisualStyleBackColor = true;
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnDown
			// 
			this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDown.Location = new System.Drawing.Point(402, 23);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(25, 23);
			this.btnDown.TabIndex = 78;
			this.btnDown.Text = "<";
			this.btnDown.UseVisualStyleBackColor = true;
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// lblFrameNo
			// 
			this.lblFrameNo.AutoSize = true;
			this.lblFrameNo.Location = new System.Drawing.Point(49, 4);
			this.lblFrameNo.Name = "lblFrameNo";
			this.lblFrameNo.Size = new System.Drawing.Size(35, 13);
			this.lblFrameNo.TabIndex = 77;
			this.lblFrameNo.Text = "label3";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 4);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(42, 13);
			this.label2.TabIndex = 76;
			this.label2.Text = "Frame: ";
			// 
			// hsrcFrames
			// 
			this.hsrcFrames.Location = new System.Drawing.Point(13, 27);
			this.hsrcFrames.Name = "hsrcFrames";
			this.hsrcFrames.Size = new System.Drawing.Size(371, 17);
			this.hsrcFrames.TabIndex = 75;
			this.hsrcFrames.ValueChanged += new System.EventHandler(this.hsrcFrames_ValueChanged);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(385, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(39, 13);
			this.label1.TabIndex = 74;
			this.label1.Text = "Depth:";
			// 
			// pnlImage
			// 
			this.pnlImage.Controls.Add(this.imgFrame);
			this.pnlImage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlImage.Location = new System.Drawing.Point(0, 0);
			this.pnlImage.Name = "pnlImage";
			this.pnlImage.Size = new System.Drawing.Size(600, 400);
			this.pnlImage.TabIndex = 2;
			// 
			// imgFrame
			// 
			this.imgFrame.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.imgFrame.Dock = System.Windows.Forms.DockStyle.Fill;
			this.imgFrame.Location = new System.Drawing.Point(0, 0);
			this.imgFrame.Name = "imgFrame";
			this.imgFrame.Size = new System.Drawing.Size(600, 400);
			this.imgFrame.TabIndex = 0;
			this.imgFrame.TabStop = false;
			// 
			// frmAdjustStarMap
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(600, 458);
			this.Controls.Add(this.pnlImage);
			this.Controls.Add(this.pnlControls);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmAdjustStarMap";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Adjust Depth of Picked Image Stars";
			this.Load += new System.EventHandler(this.frmAdjustStarMap_Load);
			this.pnlControls.ResumeLayout(false);
			this.pnlControls.PerformLayout();
			this.pnlImage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.imgFrame)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlControls;
		private System.Windows.Forms.Panel pnlImage;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox imgFrame;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.HScrollBar hsrcFrames;
		private System.Windows.Forms.Label lblFrameNo;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.Label lblDepth;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.RadioButton rbAuto;
		private System.Windows.Forms.RadioButton rbManual;
	}
}