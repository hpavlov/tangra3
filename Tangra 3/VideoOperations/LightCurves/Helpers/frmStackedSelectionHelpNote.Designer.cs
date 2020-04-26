namespace Tangra.VideoOperations.LightCurves.Helpers
{
    partial class frmStackedSelectionHelpNote
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStackedSelectionHelpNote));
            this.btnStackedSelection = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // btnStackedSelection
            // 
            this.btnStackedSelection.Image = global::Tangra.Properties.Resources.Stack1;
            this.btnStackedSelection.Location = new System.Drawing.Point(84, 174);
            this.btnStackedSelection.Name = "btnStackedSelection";
            this.btnStackedSelection.Size = new System.Drawing.Size(50, 50);
            this.btnStackedSelection.TabIndex = 47;
            this.btnStackedSelection.TabStop = false;
            this.btnStackedSelection.Tag = "";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(379, 142);
            this.label1.TabIndex = 48;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // button1
            // 
            this.button1.Image = global::Tangra.Properties.Resources.Stack3;
            this.button1.Location = new System.Drawing.Point(252, 174);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(50, 50);
            this.button1.TabIndex = 49;
            this.button1.TabStop = false;
            this.button1.Tag = "";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(40, 239);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 29);
            this.label2.TabIndex = 50;
            this.label2.Text = "    Stacked Image View is turned on, press to turn off";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(207, 239);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 29);
            this.label3.TabIndex = 51;
            this.label3.Text = "    Stacked Image View is turned off, press to turn on";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(126, 313);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(127, 23);
            this.btnOK.TabIndex = 52;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(15, 296);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(366, 1);
            this.panel1.TabIndex = 53;
            // 
            // frmStackedSelectionHelpNote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 358);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStackedSelection);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmStackedSelectionHelpNote";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Stacked Selection";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStackedSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panel1;
    }
}