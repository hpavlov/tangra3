﻿namespace Tangra.VideoOperations.LightCurves
{
    partial class frmAcquisitionDelayChooser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAcquisitionDelayChooser));
            this.btnOK = new System.Windows.Forms.Button();
            this.tbxAcquisitionDelay = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cbxCameraSystem = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxCameraModel = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxTimestamping = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tbxReferenceTimeOffset = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.pnlTimingCorrections = new System.Windows.Forms.Panel();
            this.cbxEnterRefertenceTimeOffset = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.pnlTimestamping = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.pnlReferenceTimeOffset = new System.Windows.Forms.Panel();
            this.pnlTimingCorrections.SuspendLayout();
            this.pnlTimestamping.SuspendLayout();
            this.pnlReferenceTimeOffset.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(161, 191);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(141, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tbxAcquisitionDelay
            // 
            this.tbxAcquisitionDelay.Location = new System.Drawing.Point(191, 10);
            this.tbxAcquisitionDelay.Name = "tbxAcquisitionDelay";
            this.tbxAcquisitionDelay.Size = new System.Drawing.Size(74, 20);
            this.tbxAcquisitionDelay.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(276, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "milliseconds";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 15000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // cbxCameraSystem
            // 
            this.cbxCameraSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCameraSystem.FormattingEnabled = true;
            this.cbxCameraSystem.Items.AddRange(new object[] {
            "ADVS",
            "QHY174-GPS",
            "DVTI",
            "Other"});
            this.cbxCameraSystem.Location = new System.Drawing.Point(112, 43);
            this.cbxCameraSystem.Name = "cbxCameraSystem";
            this.cbxCameraSystem.Size = new System.Drawing.Size(121, 21);
            this.cbxCameraSystem.TabIndex = 9;
            this.cbxCameraSystem.SelectedIndexChanged += new System.EventHandler(this.cbxCameraSystem_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Camera/System";
            // 
            // tbxCameraModel
            // 
            this.tbxCameraModel.Location = new System.Drawing.Point(239, 43);
            this.tbxCameraModel.Name = "tbxCameraModel";
            this.tbxCameraModel.Size = new System.Drawing.Size(121, 20);
            this.tbxCameraModel.TabIndex = 11;
            this.tbxCameraModel.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Timestamping";
            // 
            // cbxTimestamping
            // 
            this.cbxTimestamping.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimestamping.FormattingEnabled = true;
            this.cbxTimestamping.Items.AddRange(new object[] {
            "GPS Timestamps by the Camera",
            "Windows Timestamp by Recording Software",
            "Other"});
            this.cbxTimestamping.Location = new System.Drawing.Point(100, 3);
            this.cbxTimestamping.Name = "cbxTimestamping";
            this.cbxTimestamping.Size = new System.Drawing.Size(248, 21);
            this.cbxTimestamping.TabIndex = 13;
            this.cbxTimestamping.SelectedIndexChanged += new System.EventHandler(this.cbxTimestamping_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(358, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Help";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbxReferenceTimeOffset
            // 
            this.tbxReferenceTimeOffset.Location = new System.Drawing.Point(163, 5);
            this.tbxReferenceTimeOffset.Name = "tbxReferenceTimeOffset";
            this.tbxReferenceTimeOffset.Size = new System.Drawing.Size(74, 20);
            this.tbxReferenceTimeOffset.TabIndex = 18;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(248, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "milliseconds";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(330, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(86, 23);
            this.button2.TabIndex = 19;
            this.button2.Text = "Help";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // pnlTimingCorrections
            // 
            this.pnlTimingCorrections.Controls.Add(this.cbxEnterRefertenceTimeOffset);
            this.pnlTimingCorrections.Controls.Add(this.pnlReferenceTimeOffset);
            this.pnlTimingCorrections.Controls.Add(this.label6);
            this.pnlTimingCorrections.Controls.Add(this.label2);
            this.pnlTimingCorrections.Controls.Add(this.button1);
            this.pnlTimingCorrections.Controls.Add(this.tbxAcquisitionDelay);
            this.pnlTimingCorrections.Location = new System.Drawing.Point(12, 103);
            this.pnlTimingCorrections.Name = "pnlTimingCorrections";
            this.pnlTimingCorrections.Size = new System.Drawing.Size(461, 65);
            this.pnlTimingCorrections.TabIndex = 20;
            this.pnlTimingCorrections.Visible = false;
            // 
            // cbxEnterRefertenceTimeOffset
            // 
            this.cbxEnterRefertenceTimeOffset.AutoSize = true;
            this.cbxEnterRefertenceTimeOffset.Checked = true;
            this.cbxEnterRefertenceTimeOffset.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxEnterRefertenceTimeOffset.Location = new System.Drawing.Point(12, 43);
            this.cbxEnterRefertenceTimeOffset.Name = "cbxEnterRefertenceTimeOffset";
            this.cbxEnterRefertenceTimeOffset.Size = new System.Drawing.Size(15, 14);
            this.cbxEnterRefertenceTimeOffset.TabIndex = 24;
            this.cbxEnterRefertenceTimeOffset.UseVisualStyleBackColor = true;
            this.cbxEnterRefertenceTimeOffset.CheckedChanged += new System.EventHandler(this.cbxEnterRefertenceTimeOffset_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Acquisition Delay";
            // 
            // pnlTimestamping
            // 
            this.pnlTimestamping.Controls.Add(this.cbxTimestamping);
            this.pnlTimestamping.Controls.Add(this.label4);
            this.pnlTimestamping.Location = new System.Drawing.Point(12, 70);
            this.pnlTimestamping.Name = "pnlTimestamping";
            this.pnlTimestamping.Size = new System.Drawing.Size(391, 27);
            this.pnlTimestamping.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(260, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Enter information about used video camera and timing";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(12, 176);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(456, 1);
            this.panel1.TabIndex = 23;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(148, 13);
            this.label7.TabIndex = 24;
            this.label7.Text = "(ReferenceTime - UTC) Offset";
            // 
            // pnlReferenceTimeOffset
            // 
            this.pnlReferenceTimeOffset.Controls.Add(this.label7);
            this.pnlReferenceTimeOffset.Controls.Add(this.label5);
            this.pnlReferenceTimeOffset.Controls.Add(this.tbxReferenceTimeOffset);
            this.pnlReferenceTimeOffset.Controls.Add(this.button2);
            this.pnlReferenceTimeOffset.Location = new System.Drawing.Point(28, 33);
            this.pnlReferenceTimeOffset.Name = "pnlReferenceTimeOffset";
            this.pnlReferenceTimeOffset.Size = new System.Drawing.Size(428, 29);
            this.pnlReferenceTimeOffset.TabIndex = 25;
            // 
            // frmAcquisitionDelayChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 227);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlTimestamping);
            this.Controls.Add(this.pnlTimingCorrections);
            this.Controls.Add(this.tbxCameraModel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbxCameraSystem);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAcquisitionDelayChooser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Camera and Timing Corrections";
            this.pnlTimingCorrections.ResumeLayout(false);
            this.pnlTimingCorrections.PerformLayout();
            this.pnlTimestamping.ResumeLayout(false);
            this.pnlTimestamping.PerformLayout();
            this.pnlReferenceTimeOffset.ResumeLayout(false);
            this.pnlReferenceTimeOffset.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox tbxAcquisitionDelay;
        private System.Windows.Forms.ComboBox cbxCameraSystem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxCameraModel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxTimestamping;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbxReferenceTimeOffset;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel pnlTimingCorrections;
        private System.Windows.Forms.Panel pnlTimestamping;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbxEnterRefertenceTimeOffset;
        private System.Windows.Forms.Panel pnlReferenceTimeOffset;
        private System.Windows.Forms.Label label7;
    }
}