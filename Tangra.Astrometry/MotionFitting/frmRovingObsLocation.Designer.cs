namespace Tangra.MotionFitting
{
    partial class frmRovingObsLocation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRovingObsLocation));
            this.label1 = new System.Windows.Forms.Label();
            this.tbxLatitude = new System.Windows.Forms.TextBox();
            this.tbxLongitude = new System.Windows.Forms.TextBox();
            this.cbxLongitude = new System.Windows.Forms.ComboBox();
            this.cbxLatitude = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxAltitude = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.llMPC = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(363, 45);
            this.label1.TabIndex = 0;
            this.label1.Text = "    You have configured the \'Roving Observer\' observatory code (247). Please ente" +
    "r the coordinates of the observing location for this observation to be included " +
    "in the MPC OBS report lines:";
            // 
            // tbxLatitude
            // 
            this.tbxLatitude.Location = new System.Drawing.Point(143, 108);
            this.tbxLatitude.Name = "tbxLatitude";
            this.tbxLatitude.Size = new System.Drawing.Size(90, 20);
            this.tbxLatitude.TabIndex = 11;
            // 
            // tbxLongitude
            // 
            this.tbxLongitude.Location = new System.Drawing.Point(143, 81);
            this.tbxLongitude.Name = "tbxLongitude";
            this.tbxLongitude.Size = new System.Drawing.Size(90, 20);
            this.tbxLongitude.TabIndex = 10;
            // 
            // cbxLongitude
            // 
            this.cbxLongitude.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLongitude.FormattingEnabled = true;
            this.cbxLongitude.Items.AddRange(new object[] {
            "East",
            "West"});
            this.cbxLongitude.Location = new System.Drawing.Point(239, 81);
            this.cbxLongitude.Name = "cbxLongitude";
            this.cbxLongitude.Size = new System.Drawing.Size(56, 21);
            this.cbxLongitude.TabIndex = 12;
            // 
            // cbxLatitude
            // 
            this.cbxLatitude.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLatitude.FormattingEnabled = true;
            this.cbxLatitude.Items.AddRange(new object[] {
            "North",
            "South"});
            this.cbxLatitude.Location = new System.Drawing.Point(240, 108);
            this.cbxLatitude.Name = "cbxLatitude";
            this.cbxLatitude.Size = new System.Drawing.Size(55, 21);
            this.cbxLatitude.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Longitude";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(63, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Latitude";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(63, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Altitude";
            // 
            // tbxAltitude
            // 
            this.tbxAltitude.Location = new System.Drawing.Point(143, 136);
            this.tbxAltitude.Name = "tbxAltitude";
            this.tbxAltitude.Size = new System.Drawing.Size(90, 20);
            this.tbxAltitude.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(239, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "meters";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(110, 282);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 19;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(203, 282);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(16, 264);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(358, 1);
            this.panel1.TabIndex = 21;
            // 
            // llMPC
            // 
            this.llMPC.AutoSize = true;
            this.llMPC.Location = new System.Drawing.Point(62, 234);
            this.llMPC.Name = "llMPC";
            this.llMPC.Size = new System.Drawing.Size(238, 13);
            this.llMPC.TabIndex = 50;
            this.llMPC.TabStop = true;
            this.llMPC.Text = "https://www.minorplanetcenter.net/iau/mpc.html";
            this.llMPC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llMPC_LinkClicked);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(13, 182);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(361, 42);
            this.label6.TabIndex = 49;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // frmRovingObsLocation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 317);
            this.Controls.Add(this.llMPC);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbxAltitude);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbxLatitude);
            this.Controls.Add(this.tbxLongitude);
            this.Controls.Add(this.cbxLongitude);
            this.Controls.Add(this.cbxLatitude);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmRovingObsLocation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Roving Observer Location";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxLatitude;
        private System.Windows.Forms.TextBox tbxLongitude;
        private System.Windows.Forms.ComboBox cbxLongitude;
        private System.Windows.Forms.ComboBox cbxLatitude;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxAltitude;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel llMPC;
        private System.Windows.Forms.Label label6;
    }
}