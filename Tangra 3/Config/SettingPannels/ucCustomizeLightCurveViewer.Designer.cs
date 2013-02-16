namespace Tangra.Config.SettingPannels
{
	partial class ucCustomizeLightCurveViewer
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.nudPointSize = new System.Windows.Forms.NumericUpDown();
			this.nudSNFrameWindow = new System.Windows.Forms.NumericUpDown();
			this.cbxColorScheme = new System.Windows.Forms.ComboBox();
			this.cbxDrawGrid = new System.Windows.Forms.CheckBox();
			this.gbColors = new System.Windows.Forms.GroupBox();
			this.cbxTangraTargetColors = new System.Windows.Forms.CheckBox();
			this.cpBackground = new Tangra.Controls.ucColorPicker();
			this.label31 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.cpGrid = new Tangra.Controls.ucColorPicker();
			this.cpTarget3 = new Tangra.Controls.ucColorPicker();
			this.label1 = new System.Windows.Forms.Label();
			this.cpTarget2 = new Tangra.Controls.ucColorPicker();
			this.cpLabels = new Tangra.Controls.ucColorPicker();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cpTarget4 = new Tangra.Controls.ucColorPicker();
			this.label6 = new System.Windows.Forms.Label();
			this.cpTarget1 = new Tangra.Controls.ucColorPicker();
			this.label25 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.cpSelectionCursor = new Tangra.Controls.ucColorPicker();
			this.cpFocusArea = new Tangra.Controls.ucColorPicker();
			((System.ComponentModel.ISupportInitialize)(this.nudPointSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSNFrameWindow)).BeginInit();
			this.gbColors.SuspendLayout();
			this.SuspendLayout();
			// 
			// nudPointSize
			// 
			this.nudPointSize.Location = new System.Drawing.Point(92, 212);
			this.nudPointSize.Name = "nudPointSize";
			this.nudPointSize.Size = new System.Drawing.Size(39, 20);
			this.nudPointSize.TabIndex = 55;
			this.nudPointSize.ValueChanged += new System.EventHandler(this.nudPointSize_ValueChanged);
			// 
			// nudSNFrameWindow
			// 
			this.nudSNFrameWindow.Location = new System.Drawing.Point(175, 300);
			this.nudSNFrameWindow.Name = "nudSNFrameWindow";
			this.nudSNFrameWindow.Size = new System.Drawing.Size(47, 20);
			this.nudSNFrameWindow.TabIndex = 54;
			// 
			// cbxColorScheme
			// 
			this.cbxColorScheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxColorScheme.Items.AddRange(new object[] {
            "Original",
            "Pastel",
            "Contrast",
            "Custom"});
			this.cbxColorScheme.Location = new System.Drawing.Point(91, 4);
			this.cbxColorScheme.Name = "cbxColorScheme";
			this.cbxColorScheme.Size = new System.Drawing.Size(137, 21);
			this.cbxColorScheme.TabIndex = 53;
			this.cbxColorScheme.SelectedIndexChanged += new System.EventHandler(this.cbxColorScheme_SelectedIndexChanged);
			// 
			// cbxDrawGrid
			// 
			this.cbxDrawGrid.Location = new System.Drawing.Point(3, 243);
			this.cbxDrawGrid.Name = "cbxDrawGrid";
			this.cbxDrawGrid.Size = new System.Drawing.Size(75, 19);
			this.cbxDrawGrid.TabIndex = 51;
			this.cbxDrawGrid.Text = "Draw Grid";
			this.cbxDrawGrid.CheckedChanged += new System.EventHandler(this.cbxDrawGrid_CheckedChanged);
			// 
			// gbColors
			// 
			this.gbColors.Controls.Add(this.cbxTangraTargetColors);
			this.gbColors.Controls.Add(this.cpBackground);
			this.gbColors.Controls.Add(this.label31);
			this.gbColors.Controls.Add(this.label4);
			this.gbColors.Controls.Add(this.cpGrid);
			this.gbColors.Controls.Add(this.cpTarget3);
			this.gbColors.Controls.Add(this.label1);
			this.gbColors.Controls.Add(this.cpTarget2);
			this.gbColors.Controls.Add(this.cpLabels);
			this.gbColors.Controls.Add(this.label5);
			this.gbColors.Controls.Add(this.label3);
			this.gbColors.Controls.Add(this.label2);
			this.gbColors.Controls.Add(this.cpTarget4);
			this.gbColors.Controls.Add(this.label6);
			this.gbColors.Controls.Add(this.cpTarget1);
			this.gbColors.Location = new System.Drawing.Point(3, 31);
			this.gbColors.Name = "gbColors";
			this.gbColors.Size = new System.Drawing.Size(390, 172);
			this.gbColors.TabIndex = 50;
			this.gbColors.TabStop = false;
			// 
			// cbxTangraTargetColors
			// 
			this.cbxTangraTargetColors.Location = new System.Drawing.Point(210, 10);
			this.cbxTangraTargetColors.Name = "cbxTangraTargetColors";
			this.cbxTangraTargetColors.Size = new System.Drawing.Size(164, 19);
			this.cbxTangraTargetColors.TabIndex = 52;
			this.cbxTangraTargetColors.Text = "Use Tangra\'s Target Colours";
			this.cbxTangraTargetColors.CheckedChanged += new System.EventHandler(this.cbxTangraTargetColors_CheckedChanged);
			// 
			// cpBackground
			// 
			this.cpBackground.AutoSize = true;
			this.cpBackground.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpBackground.BackColor = System.Drawing.SystemColors.Control;
			this.cpBackground.Location = new System.Drawing.Point(12, 33);
			this.cpBackground.Name = "cpBackground";
			this.cpBackground.SelectedColor = System.Drawing.Color.Gray;
			this.cpBackground.Size = new System.Drawing.Size(60, 26);
			this.cpBackground.TabIndex = 2;
			this.cpBackground.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// label31
			// 
			this.label31.AutoSize = true;
			this.label31.Location = new System.Drawing.Point(74, 39);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(65, 13);
			this.label31.TabIndex = 3;
			this.label31.Text = "Background";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(268, 73);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 13);
			this.label4.TabIndex = 13;
			this.label4.Text = "Lightcurve Target 2";
			// 
			// cpGrid
			// 
			this.cpGrid.AutoSize = true;
			this.cpGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpGrid.BackColor = System.Drawing.SystemColors.Control;
			this.cpGrid.Location = new System.Drawing.Point(12, 67);
			this.cpGrid.Name = "cpGrid";
			this.cpGrid.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.cpGrid.Size = new System.Drawing.Size(60, 26);
			this.cpGrid.TabIndex = 4;
			this.cpGrid.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// cpTarget3
			// 
			this.cpTarget3.AutoSize = true;
			this.cpTarget3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpTarget3.BackColor = System.Drawing.SystemColors.Control;
			this.cpTarget3.Location = new System.Drawing.Point(206, 101);
			this.cpTarget3.Name = "cpTarget3";
			this.cpTarget3.SelectedColor = System.Drawing.SystemColors.ControlDark;
			this.cpTarget3.Size = new System.Drawing.Size(60, 26);
			this.cpTarget3.TabIndex = 14;
			this.cpTarget3.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(74, 73);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Grid Lines";
			// 
			// cpTarget2
			// 
			this.cpTarget2.AutoSize = true;
			this.cpTarget2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpTarget2.BackColor = System.Drawing.SystemColors.Control;
			this.cpTarget2.Location = new System.Drawing.Point(206, 67);
			this.cpTarget2.Name = "cpTarget2";
			this.cpTarget2.SelectedColor = System.Drawing.SystemColors.ControlDark;
			this.cpTarget2.Size = new System.Drawing.Size(60, 26);
			this.cpTarget2.TabIndex = 12;
			this.cpTarget2.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// cpLabels
			// 
			this.cpLabels.AutoSize = true;
			this.cpLabels.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpLabels.BackColor = System.Drawing.SystemColors.Control;
			this.cpLabels.Location = new System.Drawing.Point(12, 101);
			this.cpLabels.Name = "cpLabels";
			this.cpLabels.SelectedColor = System.Drawing.Color.White;
			this.cpLabels.Size = new System.Drawing.Size(60, 26);
			this.cpLabels.TabIndex = 8;
			this.cpLabels.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(268, 107);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 13);
			this.label5.TabIndex = 15;
			this.label5.Text = "Lightcurve Target 3";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(268, 39);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 13);
			this.label3.TabIndex = 11;
			this.label3.Text = "Lightcurve Target 1";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(74, 107);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 9;
			this.label2.Text = "Labels";
			// 
			// cpTarget4
			// 
			this.cpTarget4.AutoSize = true;
			this.cpTarget4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpTarget4.BackColor = System.Drawing.SystemColors.Control;
			this.cpTarget4.Location = new System.Drawing.Point(206, 134);
			this.cpTarget4.Name = "cpTarget4";
			this.cpTarget4.SelectedColor = System.Drawing.SystemColors.ControlDark;
			this.cpTarget4.Size = new System.Drawing.Size(60, 26);
			this.cpTarget4.TabIndex = 16;
			this.cpTarget4.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(268, 140);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 13);
			this.label6.TabIndex = 17;
			this.label6.Text = "Lightcurve Target 4";
			// 
			// cpTarget1
			// 
			this.cpTarget1.AutoSize = true;
			this.cpTarget1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpTarget1.BackColor = System.Drawing.SystemColors.Control;
			this.cpTarget1.Location = new System.Drawing.Point(206, 33);
			this.cpTarget1.Name = "cpTarget1";
			this.cpTarget1.SelectedColor = System.Drawing.SystemColors.ControlDark;
			this.cpTarget1.Size = new System.Drawing.Size(60, 26);
			this.cpTarget1.TabIndex = 10;
			this.cpTarget1.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// label25
			// 
			this.label25.AutoSize = true;
			this.label25.Location = new System.Drawing.Point(3, 303);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(169, 13);
			this.label25.TabIndex = 48;
			this.label25.Text = "Num Frames for S/N Computation:";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(268, 242);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(131, 13);
			this.label11.TabIndex = 40;
			this.label11.Text = "Selected Datapoint Cursor";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(268, 215);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(121, 13);
			this.label10.TabIndex = 37;
			this.label10.Text = "Small Graph Focus Area";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(133, 215);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(18, 13);
			this.label9.TabIndex = 35;
			this.label9.Text = "px";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(3, 215);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(83, 13);
			this.label8.TabIndex = 33;
			this.label8.Text = "Data Point Size:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(3, 7);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(82, 13);
			this.label7.TabIndex = 31;
			this.label7.Text = "Colour Scheme:";
			// 
			// cpSelectionCursor
			// 
			this.cpSelectionCursor.AutoSize = true;
			this.cpSelectionCursor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpSelectionCursor.BackColor = System.Drawing.SystemColors.Control;
			this.cpSelectionCursor.Location = new System.Drawing.Point(206, 236);
			this.cpSelectionCursor.Name = "cpSelectionCursor";
			this.cpSelectionCursor.SelectedColor = System.Drawing.Color.White;
			this.cpSelectionCursor.Size = new System.Drawing.Size(60, 26);
			this.cpSelectionCursor.TabIndex = 39;
			this.cpSelectionCursor.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// cpFocusArea
			// 
			this.cpFocusArea.AutoSize = true;
			this.cpFocusArea.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cpFocusArea.BackColor = System.Drawing.SystemColors.Control;
			this.cpFocusArea.Location = new System.Drawing.Point(206, 209);
			this.cpFocusArea.Name = "cpFocusArea";
			this.cpFocusArea.SelectedColor = System.Drawing.Color.White;
			this.cpFocusArea.Size = new System.Drawing.Size(60, 26);
			this.cpFocusArea.TabIndex = 36;
			this.cpFocusArea.SelectedColorChanged += new System.EventHandler(this.SelectedColorChanged);
			// 
			// ucCustomizeLightCurveViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.nudPointSize);
			this.Controls.Add(this.nudSNFrameWindow);
			this.Controls.Add(this.cbxColorScheme);
			this.Controls.Add(this.cbxDrawGrid);
			this.Controls.Add(this.gbColors);
			this.Controls.Add(this.label25);
			this.Controls.Add(this.cpSelectionCursor);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.cpFocusArea);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Name = "ucCustomizeLightCurveViewer";
			this.Size = new System.Drawing.Size(491, 356);
			((System.ComponentModel.ISupportInitialize)(this.nudPointSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSNFrameWindow)).EndInit();
			this.gbColors.ResumeLayout(false);
			this.gbColors.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Tangra.Controls.ucColorPicker cpSelectionCursor;
		private System.Windows.Forms.Label label11;
		private Tangra.Controls.ucColorPicker cpFocusArea;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private Tangra.Controls.ucColorPicker cpBackground;
		private System.Windows.Forms.Label label31;
		private Tangra.Controls.ucColorPicker cpGrid;
		private System.Windows.Forms.Label label1;
		private Tangra.Controls.ucColorPicker cpLabels;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label6;
		private Tangra.Controls.ucColorPicker cpTarget1;
		private Tangra.Controls.ucColorPicker cpTarget4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private Tangra.Controls.ucColorPicker cpTarget2;
		private Tangra.Controls.ucColorPicker cpTarget3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.GroupBox gbColors;
		private System.Windows.Forms.CheckBox cbxDrawGrid;
		private System.Windows.Forms.CheckBox cbxTangraTargetColors;
		private System.Windows.Forms.ComboBox cbxColorScheme;
		private System.Windows.Forms.NumericUpDown nudSNFrameWindow;
		private System.Windows.Forms.NumericUpDown nudPointSize;
	}
}
