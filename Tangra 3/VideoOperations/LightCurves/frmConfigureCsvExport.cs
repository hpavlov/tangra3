using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class frmConfigureCsvExport : Form
    {
		public frmConfigureCsvExport()
		{
            InitializeComponent();

            TrackedObjects = new List<TrackedObjectConfig>();

            pb1.Image = new Bitmap(pb1.Width, pb1.Height, PixelFormat.Format24bppRgb);
            pb2.Image = new Bitmap(pb2.Width, pb1.Height, PixelFormat.Format24bppRgb);
            pb3.Image = new Bitmap(pb3.Width, pb1.Height, PixelFormat.Format24bppRgb);
            pb4.Image = new Bitmap(pb4.Width, pb1.Height, PixelFormat.Format24bppRgb);

            try
            {
                CoordsAndLocation savedSettings = Properties.Settings.Default.AtmExtRememberedConfig.Deserialize<CoordsAndLocation>();
                tbxRA.Text = AstroConvert.ToStringValue(savedSettings.RAHours, "HH MM SS");
                tbxDec.Text = AstroConvert.ToStringValue(savedSettings.DEDeg, "+DD MM SS");
                tbxLatitude.Text = AstroConvert.ToStringValue(savedSettings.LatitudeDeg, "+DD MM SS");
                tbxLongitude.Text = AstroConvert.ToStringValue(savedSettings.LongitudeDeg, "DDD MM SS");
                tbxHeightKm.Text = savedSettings.HeightKM.ToString("0.000");
            }
            catch { }
        }

        internal TangraConfig.LightCurvesDisplaySettings DisplaySettings;

        internal List<TrackedObjectConfig> TrackedObjects;

        internal LCFile LCFile;

        internal bool Binning;
        internal bool OnlyExportSignalMunusBg;

        private double[] m_MedianFluxes = new double[4];
        private double m_M0;

        private void frmConfigureCsvExport_Load(object sender, EventArgs e)
        {
            rbFlux.Checked = true;
            pnlFlux.BringToFront();
            pnlMagnitude.SendToBack();

            if (TrackedObjects.Count < 4)
            {
                pb4.Visible = false;
                nudMag4.Visible = false;
            }

            if (TrackedObjects.Count < 3)
            {
                pb3.Visible = false;
                nudMag3.Visible = false;
            }

            if (TrackedObjects.Count < 2)
            {
                pb2.Visible = false;
                nudMag2.Visible = false;
            }

            using (Graphics g = Graphics.FromImage(pb1.Image))
            {
                g.Clear(DisplaySettings.Target1Color);
            }
            using (Graphics g = Graphics.FromImage(pb2.Image))
            {
                g.Clear(DisplaySettings.Target2Color);
            }
            using (Graphics g = Graphics.FromImage(pb3.Image))
            {
                g.Clear(DisplaySettings.Target3Color);
            }
            using (Graphics g = Graphics.FromImage(pb4.Image))
            {
                g.Clear(DisplaySettings.Target4Color);
            }

            for (int k = 0; k < TrackedObjects.Count; k++)
            {
                var medianCalcList = new List<double>();
                for (int i = 0; i < 100 && i < LCFile.Data[k].Count; i++)
                {
                    double flux;
                    if (Binning)
                        flux = LCFile.Data[k][i].AdjustedReading;
                    else if (!OnlyExportSignalMunusBg)
                        flux = LCFile.Data[k][i].TotalReading - LCFile.Data[k][i].TotalBackground;
                    else
                        flux = LCFile.Data[k][i].TotalReading;

                    medianCalcList.Add(flux);
                }
                medianCalcList.Sort();
                m_MedianFluxes[k] = medianCalcList[medianCalcList.Count / 2];
            }

            RecalculateMagnitudes(0, 10);

	        cbxSpacingOptions.SelectedIndex = 0;
			if (Binning || OnlyExportSignalMunusBg)
			{
				lblSAndB.Visible = false;
				lblSmB.Visible = true;
				rbSeriesSmB.Enabled = true;
				rbSeriesSmB.Checked = true;
				rbSeriesSAndB.Enabled = false;
			}
			else
			{
				lblSAndB.Visible = true;
				lblSmB.Visible = false;
				rbSeriesSAndB.Enabled = true;
				rbSeriesSAndB.Checked = true;
			}

			nudExportStartFromFrame.Minimum = LCFile.Header.MinFrame;
			nudExportStartFromFrame.Maximum = LCFile.Header.MaxFrame - 1;
			nudExportStartFromFrame.SetNUDValue((decimal)LCFile.Header.MinFrame);
        }

        private void RecalculateMagnitudes(int refIndex, double refMagnitude)
        {
			if (refIndex < m_MedianFluxes.Length && refIndex > -1)
			{
				try
				{
					// m = m0 - 2.5 Log10(Flux);
					// m0 = refMagnitude + 2.5 Log10(Flux);

					m_M0 = refMagnitude + 2.5 * Math.Log10(Math.Max(1, m_MedianFluxes[refIndex]));

					NumericUpDown[] magControls = new NumericUpDown[] { nudMag1, nudMag2, nudMag3, nudMag4 };

					try
					{
						for (int i = 0; i < TrackedObjects.Count; i++) magControls[i].ValueChanged -= MagValueChanged;

						for (int i = 0; i < TrackedObjects.Count; i++)
						{
							magControls[i].Value = (decimal)(m_M0 - 2.5 * Math.Log10(m_MedianFluxes[i]));
						}
					}
					finally
					{
						for (int i = 0; i < TrackedObjects.Count; i++) magControls[i].ValueChanged += MagValueChanged;
					}
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.GetFullStackTrace());
				}
			}
        }

        internal CSVExportOptions GetSelectedOptions()
        {
            var rv = new CSVExportOptions();

            if (rbTimeString.Checked) rv.TimeFormat = TimeFormat.String;
            else if (rbDecimalDays.Checked) rv.TimeFormat = TimeFormat.DecimalDays;
            else if (rbJulianDays.Checked) rv.TimeFormat = TimeFormat.DecimalJulianDays;

            if (rbFlux.Checked) rv.PhotometricFormat = PhotometricFormat.RelativeFlux;
            else if (rbMagnitude.Checked) rv.PhotometricFormat = PhotometricFormat.Magnitudes;

            rv.M0 = m_M0;

            rv.ExportAtmosphericExtinction = cbxAtmExtExport.Checked;
            if (cbxAtmExtExport.Checked)
            {
                rv.RAHours = m_RAHours;
                rv.DEDeg = m_DEDeg;
                rv.LongitudeDeg = m_Longitude;
                rv.LatitudeDeg = m_Latitude;
                rv.HeightKM = m_HeightKm;
            }

            if (m_ConfirmedDate.HasValue)
            {
                rv.FistMeasurementDay = m_ConfirmedDate;
                rv.FistMeasurementTimeStamp = LCFile.GetTimeForFrame(LCFile.Header.MinFrame);
            }

	        rv.Spacing = cbxSpacingOptions.SelectedIndex + 1;
	        rv.ExportStartingFrame = (int)nudExportStartFromFrame.Value;
	        rv.ForceSignalMinusBackground = this.OnlyExportSignalMunusBg || rbSeriesSmB.Checked;

            return rv;
        }

        private void rbMagnitude_CheckedChanged(object sender, EventArgs e)
        {
            if (rbFlux.Checked)
            {
                pnlFlux.BringToFront();
                pnlMagnitude.SendToBack();
            }
            else if (rbMagnitude.Checked)
            {
                pnlMagnitude.BringToFront();
                pnlFlux.SendToBack();                
            }
        }

        private void cbxAtmExtExport_CheckedChanged(object sender, EventArgs e)
        {
            pnlExtinction.Enabled = cbxAtmExtExport.Checked;
        }

        private void MagValueChanged(object sender, EventArgs e)
        {
            var magControls = new List<NumericUpDown>( new [] { nudMag1, nudMag2, nudMag3, nudMag4 });
            NumericUpDown nud = sender as NumericUpDown;
            if (nud != null)
            {
                int idx = magControls.IndexOf(nud);
                if (idx >= 0 && idx < TrackedObjects.Count)
                {
                    RecalculateMagnitudes(idx, (double) nud.Value);
                }
            }            
        }

        private double m_RAHours;
        private double m_DEDeg;
        private double m_Longitude;
        private double m_Latitude;
        private double m_HeightKm;
        private DateTime? m_ConfirmedDate;

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_ConfirmedDate = null;

            bool dayConfirmationRequired = (rbJulianDays.Checked || (rbMagnitude.Checked && cbxAtmExtExport.Checked));

            if (dayConfirmationRequired)
            {
                DateTime firstTimestamp = LCFile.GetTimeForFrame(LCFile.Header.MinFrame);

                if (firstTimestamp.Year > 1900 && LCFile.Header.TimingType == MeasurementTimingType.EmbeddedTimeForEachFrame)
                {
                    // No need to confirm the date. This is either an ADV file or an AAV file with time OCR-ed on the flly during the recording or AAV file with NTP timestamp
                }
                else
                {
                    var frm = new frmConfirmDay();
                    frm.SelectedDay = firstTimestamp;
                    frm.StartPosition = FormStartPosition.CenterParent;
                    if (frm.ShowDialog(this) != DialogResult.OK)
                        return;
                    DateTime? selectedDate = frm.SelectedDay;

                    if (selectedDate.HasValue)
                        m_ConfirmedDate = selectedDate.Value;
                }
            }

			if (Binning && (nudExportStartFromFrame.Value != nudExportStartFromFrame.Minimum || cbxSpacingOptions.SelectedIndex > 0))
			{
				MessageBox.Show(this, "When binning is used 'spacing' and a specific starting frame cannot be used as export options.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
				tbxRA.Focus();
				return;
			}

            if (cbxAtmExtExport.Checked)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(tbxRA.Text)) throw new FormatException();
                    m_RAHours = AstroConvert.ToRightAcsension(tbxRA.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show(this, "Please enter a valid Right Ascension value (e.g. 23 03 12)", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxRA.Focus();
                    return;
                }

                try
                {
                    if (string.IsNullOrWhiteSpace(tbxDec.Text)) throw new FormatException();
                    m_DEDeg = AstroConvert.ToDeclination(tbxDec.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show(this, "Please enter a valid Declination value (e.g. +76 13 18)", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxDec.Focus();
                    return;
                }

                try
                {
                    if (string.IsNullOrWhiteSpace(tbxLatitude.Text)) throw new FormatException();
                    m_Latitude = AstroConvert.ToDeclination(tbxLatitude.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show(this, "Please enter a valid Latitude value (e.g. -33 12 12)", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxLatitude.Focus();
                    return;
                }

                try
                {
                    if (string.IsNullOrWhiteSpace(tbxLongitude.Text)) throw new FormatException();
                    m_Longitude = AstroConvert.ToDeclination(tbxLongitude.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show(this, "Please enter a valid Longitude value (e.g. -86 09 12)", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxLongitude.Focus();
                    return;
                }

                try
                {
                    if (string.IsNullOrWhiteSpace(tbxHeightKm.Text)) throw new FormatException();
                    m_HeightKm = double.Parse(tbxHeightKm.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show(this, "Please enter a valid Height value (e.g. 0.150)", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxHeightKm.Focus();
                    return;
                }

                var calc = new AtmosphericExtinctionCalculator(m_RAHours, m_DEDeg, m_Longitude, m_Latitude, m_HeightKm);
                DateTime firstTimestamp = LCFile.GetTimeForFrame(LCFile.Header.MinFrame);
                if (m_ConfirmedDate != null)
                    firstTimestamp = m_ConfirmedDate.Value.Date.AddTicks(firstTimestamp.Ticks - firstTimestamp.Date.Ticks);

                double zenithAngle = calc.CalculateZenithAngle(firstTimestamp);
                if (zenithAngle > 90)
                {
                    MessageBox.Show(this, "The object is below the horizon.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                var savedSettings = new CoordsAndLocation()
                {
                    RAHours = m_RAHours,
                    DEDeg = m_DEDeg,
                    LatitudeDeg = m_Latitude,
                    LongitudeDeg = m_Longitude,
                    HeightKM = m_HeightKm
                };
                try
                {
                    Properties.Settings.Default.AtmExtRememberedConfig = savedSettings.AsSerialized();
                    Properties.Settings.Default.Save();
                }
                catch { }            

            }

            // TODO: Confirm object is above horizon

            DialogResult = DialogResult.OK;

            Close();
        }
    }
}
