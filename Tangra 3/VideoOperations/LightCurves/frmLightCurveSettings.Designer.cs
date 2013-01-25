namespace Tangra.VideoOperations.LightCurves
{
    partial class frmLightCurveSettings
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
			this.label31 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.cbxTangraTargetColors = new System.Windows.Forms.CheckBox();
			this.cbxColorScheme = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.nudPointSize = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			this.gbColors = new System.Windows.Forms.GroupBox();
            this.cpBackground = new Tangra.Model.Controls.ucColorPicker();
            this.cpGrid = new Tangra.Model.Controls.ucColorPicker();
            this.cpLabels = new Tangra.Model.Controls.ucColorPicker();
            this.cpTarget1 = new Tangra.Model.Controls.ucColorPicker();
            this.cpTarget4 = new Tangra.Model.Controls.ucColorPicker();
            this.cpTarget2 = new Tangra.Model.Controls.ucColorPicker();
            this.cpTarget3 = new Tangra.Model.Controls.ucColorPicker();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.cbxDrawGrid = new System.Windows.Forms.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
            this.cpSelectionCursor = new Tangra.Model.Controls.ucColorPicker();
            this.cpFocusArea = new Tangra.Model.Controls.ucColorPicker();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.nudPointSize)).BeginInit();
			this.gbColors.SuspendLayout();
			this.SuspendLayout();
			// 
			// label31
			// 
			this.label31.AutoSize = true;
			this.label31.Location = new System.Drawing.Point(68, 46);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(63, 13);
			this.label31.TabIndex = 3;
			this.label31.Text = "Background";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(68, 80);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Grid Lines";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(68, 114);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 13);
			this.label2.TabIndex = 9;
			this.label2.Text = "Labels";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(262, 46);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(101, 13);
			this.label3.TabIndex = 11;
			this.label3.Text = "Lightcurve Target 1";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(262, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(101, 13);
			this.label4.TabIndex = 13;
			this.label4.Text = "Lightcurve Target 2";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(262, 114);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(101, 13);
			this.label5.TabIndex = 15;
			this.label5.Text = "Lightcurve Target 3";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(262, 147);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(101, 13);
			this.label6.TabIndex = 17;
			this.label6.Text = "Lightcurve Target 4";
			// 
			// cbxTangraTargetColors
			// 
			this.cbxTangraTargetColors.AutoSize = true;
			this.cbxTangraTargetColors.Location = new System.Drawing.Point(206, 17);
			this.cbxTangraTargetColors.Name = "cbxTangraTargetColors";
			this.cbxTangraTargetColors.Size = new System.Drawing.Size(162, 17);
			this.cbxTangraTargetColors.TabIndex = 18;
			this.cbxTangraTargetColors.Text = "Use Tangra\'s Target Colours";
			this.cbxTangraTargetColors.UseVisualStyleBackColor = true;
			this.cbxTangraTargetColors.CheckedChanged += new System.EventHandler(this.cbxTangraTargetColors_CheckedChanged);
			// 
			// cbxColorScheme
			// 
			this.cbxColorScheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxColorScheme.FormattingEnabled = true;
			this.cbxColorScheme.Items.AddRange(new object[] {
            "Original",
            "Pastel",
            "Contrast",
            "Custom"});
			this.cbxColorScheme.Location = new System.Drawing.Point(106, 19);
			this.cbxColorScheme.Name = "cbxColorScheme";
			this.cbxColorScheme.Size = new System.Drawing.Size(137, 21);
			this.cbxColorScheme.TabIndex = 19;
			this.cbxColorScheme.SelectedIndexChanged += new System.EventHandler(this.cbxColorScheme_SelectedIndexChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(18, 22);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(82, 13);
			this.label7.TabIndex = 20;
			this.label7.Text = "Colour Scheme:";
			// 
			// nudPointSize
			// 
			this.nudPointSize.DecimalPlaces = 1;
			this.nudPointSize.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.nudPointSize.Location = new System.Drawing.Point(104, 233);
			this.nudPointSize.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
			this.nudPointSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.nudPointSize.Name = "nudPointSize";
			this.nudPointSize.Size = new System.Drawing.Size(39, 21);
			this.nudPointSize.TabIndex = 21;
			this.nudPointSize.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nudPointSize.ValueChanged += new System.EventHandler(this.nudPointSize_ValueChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(18, 235);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(83, 13);
			this.label8.TabIndex = 22;
			this.label8.Text = "Data Point Size:";
			// 
			// gbColors
			// 
			this.gbColors.Controls.Add(this.cpBackground);
			this.gbColors.Controls.Add(this.label31);
			this.gbColors.Controls.Add(this.cpGrid);
			this.gbColors.Controls.Add(this.label1);
			this.gbColors.Controls.Add(this.cpLabels);
			this.gbColors.Controls.Add(this.cbxTangraTargetColors);
			this.gbColors.Controls.Add(this.label2);
			this.gbColors.Controls.Add(this.label6);
			this.gbColors.Controls.Add(this.cpTarget1);
			this.gbColors.Controls.Add(this.cpTarget4);
			this.gbColors.Controls.Add(this.label3);
			this.gbColors.Controls.Add(this.label5);
			this.gbColors.Controls.Add(this.cpTarget2);
			this.gbColors.Controls.Add(this.cpTarget3);
			this.gbColors.Controls.Add(this.label4);
			this.gbColors.Location = new System.Drawing.Point(21, 46);
			this.gbColors.Name = "gbColors";
			this.gbColors.Size = new System.Drawing.Size(390, 177);
			this.gbColors.TabIndex = 23;
			this.gbColors.TabStop = false;
			// 
			// cpBackground
			// 
			this.cpBackground.AutoSize = true;
			this.cpBackground.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpBackground.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
			this.cpBackground.Location = new System.Drawing.Point(6, 40);
			this.cpBackground.Name = "cpBackground";
			this.cpBackground.SelectedColor = System.Drawing.Color.Gray;
			this.cpBackground.Size = new System.Drawing.Size(60, 26);
			this.cpBackground.TabIndex = 2;
			this.cpBackground.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// cpGrid
			// 
			this.cpGrid.AutoSize = true;
			this.cpGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
			this.cpGrid.Location = new System.Drawing.Point(6, 74);
			this.cpGrid.Name = "cpGrid";
			this.cpGrid.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.cpGrid.Size = new System.Drawing.Size(60, 26);
			this.cpGrid.TabIndex = 4;
			this.cpGrid.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// cpLabels
			// 
			this.cpLabels.AutoSize = true;
			this.cpLabels.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpLabels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
			this.cpLabels.Location = new System.Drawing.Point(6, 108);
			this.cpLabels.Name = "cpLabels";
			this.cpLabels.SelectedColor = System.Drawing.Color.White;
			this.cpLabels.Size = new System.Drawing.Size(60, 26);
			this.cpLabels.TabIndex = 8;
			this.cpLabels.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// cpTarget1
			// 
			this.cpTarget1.AutoSize = true;
			this.cpTarget1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpTarget1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
			this.cpTarget1.Location = new System.Drawing.Point(200, 40);
			this.cpTarget1.Name = "cpTarget1";
			this.cpTarget1.SelectedColor = System.Drawing.SystemColors.ControlDark;
			this.cpTarget1.Size = new System.Drawing.Size(60, 26);
			this.cpTarget1.TabIndex = 10;
			this.cpTarget1.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// cpTarget4
			// 
			this.cpTarget4.AutoSize = true;
			this.cpTarget4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpTarget4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
			this.cpTarget4.Location = new System.Drawing.Point(200, 141);
			this.cpTarget4.Name = "cpTarget4";
			this.cpTarget4.SelectedColor = System.Drawing.SystemColors.ControlDark;
			this.cpTarget4.Size = new System.Drawing.Size(60, 26);
			this.cpTarget4.TabIndex = 16;
			this.cpTarget4.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// cpTarget2
			// 
			this.cpTarget2.AutoSize = true;
			this.cpTarget2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpTarget2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
			this.cpTarget2.Location = new System.Drawing.Point(200, 74);
			this.cpTarget2.Name = "cpTarget2";
			this.cpTarget2.SelectedColor = System.Drawing.SystemColors.ControlDark;
			this.cpTarget2.Size = new System.Drawing.Size(60, 26);
			this.cpTarget2.TabIndex = 12;
			this.cpTarget2.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// cpTarget3
			// 
			this.cpTarget3.AutoSize = true;
			this.cpTarget3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpTarget3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
			this.cpTarget3.Location = new System.Drawing.Point(200, 108);
			this.cpTarget3.Name = "cpTarget3";
			this.cpTarget3.SelectedColor = System.Drawing.SystemColors.ControlDark;
			this.cpTarget3.Size = new System.Drawing.Size(60, 26);
			this.cpTarget3.TabIndex = 14;
			this.cpTarget3.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(148, 235);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(19, 13);
			this.label9.TabIndex = 24;
			this.label9.Text = "px";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(283, 235);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(120, 13);
			this.label10.TabIndex = 26;
			this.label10.Text = "Small Graph Focus Area";
			// 
			// cbxDrawGrid
			// 
			this.cbxDrawGrid.AutoSize = true;
			this.cbxDrawGrid.Checked = true;
			this.cbxDrawGrid.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxDrawGrid.Location = new System.Drawing.Point(21, 263);
			this.cbxDrawGrid.Name = "cbxDrawGrid";
			this.cbxDrawGrid.Size = new System.Drawing.Size(73, 17);
			this.cbxDrawGrid.TabIndex = 27;
			this.cbxDrawGrid.Text = "Draw Grid";
			this.cbxDrawGrid.UseVisualStyleBackColor = true;
			this.cbxDrawGrid.CheckedChanged += new System.EventHandler(this.cbxDrawGrid_CheckedChanged);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(283, 262);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(133, 13);
			this.label11.TabIndex = 29;
			this.label11.Text = "Selected Datapoint Cursor";
			// 
			// cpSelectionCursor
			// 
			this.cpSelectionCursor.AutoSize = true;
			this.cpSelectionCursor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpSelectionCursor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
			this.cpSelectionCursor.Location = new System.Drawing.Point(221, 256);
			this.cpSelectionCursor.Name = "cpSelectionCursor";
			this.cpSelectionCursor.SelectedColor = System.Drawing.Color.White;
			this.cpSelectionCursor.Size = new System.Drawing.Size(60, 26);
			this.cpSelectionCursor.TabIndex = 28;
			this.cpSelectionCursor.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// cpFocusArea
			// 
			this.cpFocusArea.AutoSize = true;
			this.cpFocusArea.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpFocusArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
			this.cpFocusArea.Location = new System.Drawing.Point(221, 229);
			this.cpFocusArea.Name = "cpFocusArea";
			this.cpFocusArea.SelectedColor = System.Drawing.Color.White;
			this.cpFocusArea.Size = new System.Drawing.Size(60, 26);
			this.cpFocusArea.TabIndex = 25;
			this.cpFocusArea.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(206, 308);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 30;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(125, 308);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 31;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// frmLightCurveSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(429, 345);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.cpSelectionCursor);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.cbxDrawGrid);
			this.Controls.Add(this.cpFocusArea);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.gbColors);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.nudPointSize);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.cbxColorScheme);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmLightCurveSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Light Curve Display Settings";
			((System.ComponentModel.ISupportInitialize)(this.nudPointSize)).EndInit();
			this.gbColors.ResumeLayout(false);
			this.gbColors.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label31;
        private Tangra.Model.Controls.ucColorPicker cpBackground;
        private System.Windows.Forms.Label label1;
        private Tangra.Model.Controls.ucColorPicker cpGrid;
        private System.Windows.Forms.Label label2;
        private Tangra.Model.Controls.ucColorPicker cpLabels;
        private System.Windows.Forms.Label label3;
        private Tangra.Model.Controls.ucColorPicker cpTarget1;
        private System.Windows.Forms.Label label4;
        private Tangra.Model.Controls.ucColorPicker cpTarget2;
        private System.Windows.Forms.Label label5;
        private Tangra.Model.Controls.ucColorPicker cpTarget3;
        private System.Windows.Forms.Label label6;
        private Tangra.Model.Controls.ucColorPicker cpTarget4;
        private System.Windows.Forms.CheckBox cbxTangraTargetColors;
        private System.Windows.Forms.ComboBox cbxColorScheme;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nudPointSize;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox gbColors;
        private System.Windows.Forms.Label label9;
        private Tangra.Model.Controls.ucColorPicker cpFocusArea;
        private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox cbxDrawGrid;
		private Tangra.Model.Controls.ucColorPicker cpSelectionCursor;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
    }
}