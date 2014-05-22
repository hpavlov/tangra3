using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class frmReprocessSeries : Form
    {
        //internal List<List<LCMeasurement>> AllReadings;
        internal LCMeasurementHeader Header;
        internal LCMeasurementFooter Footer;
        internal Color[] AllColor;
        internal LightCurveContext Context;
	    internal LightCurveController LightCurveController;

        private bool m_CancelExecution = false;

        private Thread m_WorkerThread = null;
        public bool RunParallel = false;

        private int m_RefreshProgressEveryHowManyItems = 10; /* TODO: Make this a core constant */

        private Stopwatch m_EllapsedTime = new Stopwatch();

        public frmReprocessSeries()
        {
            InitializeComponent();

            lblEllapsedTime.Text = string.Empty;
        }

        private void frmReprocessSeries_Load(object sender, EventArgs e)
        {
            m_CancelExecution = false;

            m_WorkerThread = new Thread(Worker);
            m_WorkerThread.IsBackground = true;
            m_WorkerThread.Start();

            pbar1.Maximum = pbar2.Maximum = pbar3.Maximum = pbar4.Maximum = (int)(Header.MeasuredFrames);

            if (Header.ObjectCount > 0)
            {
                pbar1.Visible = true;
                DrawCollorPanel(pnlColor1, 0);
            }
            if (Header.ObjectCount > 1)
            {
                pbar2.Visible = true;
                pnlColor2.Visible = true;
                DrawCollorPanel(pnlColor2, 1);
            }
            if (Header.ObjectCount > 2)
            {
                pbar3.Visible = true;
                pnlColor3.Visible = true;
                DrawCollorPanel(pnlColor3, 2);
            }
            if (Header.ObjectCount > 3)
            {
                pbar4.Visible = true;
                pnlColor4.Visible = true;
                DrawCollorPanel(pnlColor4, 3);
            }

            Height = new int[] { 78, 102, 126, 153 }[Header.ObjectCount - 1];
        }

        private void DrawCollorPanel(PictureBox pbox, int no)
        {
            pbox.Image = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(pbox.Image))
            {
                g.Clear(AllColor[no]);
                g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
            }
        }

        private void Done()
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancelled()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private delegate void ProgressDelegate(int objectId, int readingsDone);

        private void OnProgress(int objectId, int readingsDone)
        {
            if (objectId == 0)
            {
                pbar1.Value = Math.Min(pbar1.Maximum, readingsDone);
                pbar1.Update();
            }
            else if (objectId == 1)
            {
                pbar2.Value = Math.Min(pbar2.Maximum, readingsDone);
                pbar2.Update();
            }
            else if (objectId == 2)
            {
                pbar3.Value = Math.Min(pbar3.Maximum, readingsDone);
                pbar3.Update();
            }
            else if (objectId == 3)
            {
                pbar4.Value = Math.Min(pbar4.Maximum, readingsDone);
                pbar4.Update();
            }

            double ellapsedMinutes = m_EllapsedTime.Elapsed.TotalMinutes;
            lblEllapsedTime.Text = string.Format("{0}:{1}",
                ((int)ellapsedMinutes).ToString().PadLeft(2, '0'),
                ((int)(ellapsedMinutes * 60) % 60).ToString().PadLeft(2, '0'));
        }

        private SpinLock m_MutiCPULock;

        private void Worker(object state)
        {
            bool useLowPassDiff = Context.Filter == LightCurveContext.FilterType.LowPassDifference;
            bool useLowPass = Context.Filter == LightCurveContext.FilterType.LowPass;

			LightCurveController.ReloadAllReadingsFromLcFile();

            MeasurementsHelper measurer = new MeasurementsHelper(
                 Context.BitPix,
                 Context.BackgroundMethod,
                 true,
                 TangraConfig.Settings.Photometry.Saturation.GetSaturationForBpp(Context.BitPix));

            measurer.SetCoreProperties(
                TangraConfig.Settings.Photometry.AnnulusInnerRadius,
                TangraConfig.Settings.Photometry.AnnulusMinPixels,
                TangraConfig.PhotometrySettings.REJECTION_BACKGROUND_PIXELS_STD_DEV,
                Header.PositionTolerance);

            m_EllapsedTime.Start();

            measurer.GetImagePixelsCallback += new MeasurementsHelper.GetImagePixelsDelegate(measurer_GetImagePixelsCallback);

            int refreshCount = m_RefreshProgressEveryHowManyItems;


			List<LCMeasurement> readings0 = Context.AllReadings[0];
			List<LCMeasurement> readings1 = Header.ObjectCount > 1 ? Context.AllReadings[1] : null;
			List<LCMeasurement> readings2 = Header.ObjectCount > 2 ? Context.AllReadings[2] : null;
			List<LCMeasurement> readings3 = Header.ObjectCount > 3 ? Context.AllReadings[3] : null;

            int processedCount = 0;

			for (int i = 0; i < readings0.Count; i++)
			{
				List<LCMeasurement> units = new List<LCMeasurement>();
				units.Add(readings0[i]);
				if (Header.ObjectCount > 1) units.Add(readings1[i]);
				if (Header.ObjectCount > 2) units.Add(readings2[i]);
				if (Header.ObjectCount > 3) units.Add(readings3[i]);

                if (m_CancelExecution)
                {
                    Invoke(new MethodInvoker(Cancelled));
                    return;
                }

				ProcessSingleUnitSet(units, useLowPass, useLowPassDiff, measurer);

				readings0[i] = units[0];
				if (Header.ObjectCount > 1) readings1[i] = units[1];
				if (Header.ObjectCount > 2) readings2[i] = units[2];
				if (Header.ObjectCount > 3) readings3[i] = units[3];

                processedCount++;

                if (processedCount % refreshCount == 0)
                {
                    Invoke(new ProgressDelegate(OnProgress), 0, processedCount);
					if (Header.ObjectCount > 1) Invoke(new ProgressDelegate(OnProgress), 1, processedCount);
					if (Header.ObjectCount > 2) Invoke(new ProgressDelegate(OnProgress), 2, processedCount);
					if (Header.ObjectCount > 3) Invoke(new ProgressDelegate(OnProgress), 3, processedCount);
                }
            }

            Invoke(new MethodInvoker(Done));
        }

        uint[,] measurer_GetImagePixelsCallback(int x, int y, int matrixSize)
        {
            throw new NotImplementedException();
        }

	    private void ProcessSingleUnitSet(
			List<LCMeasurement> units,
		    bool useLowPass,
		    bool useLowPassDiff,
		    MeasurementsHelper measurer)
	    {
		    for (int i = 0; i < units.Count; i++)
		    {
			    LCMeasurement reading = units[i];
			    IImagePixel[] groupCenters = new IImagePixel[0];
			    float[] aperturesInGroup = new float[0];

				TrackedObjectConfig objConfig = Footer.TrackedObjects[reading.TargetNo];
				List<TrackedObjectConfig> gropedObjects = Footer.TrackedObjects.Where(x => objConfig.GroupId >= 0 && x.GroupId == objConfig.GroupId).ToList();
				if (gropedObjects.Count > 1)
				{
					groupCenters = new IImagePixel[gropedObjects.Count];
					aperturesInGroup = new float[gropedObjects.Count];

					for (int j = 0; j < gropedObjects.Count; j++)
					{
						LCMeasurement mea = units[Footer.TrackedObjects.IndexOf(gropedObjects[j])];
						groupCenters[j] = new ImagePixel(mea.X0, mea.Y0);
						aperturesInGroup[j] = gropedObjects[j].ApertureInPixels;
					}
				}

				units[i] = ProcessSingleUnit(
					reading, useLowPass, useLowPassDiff,
					Context.ReProcessFitAreas[i], Context.ReProcessApertures[i], Header.FixedApertureFlags[i],
					measurer, groupCenters, aperturesInGroup);
		    }
	    }

		private LCMeasurement ProcessSingleUnit(
            LCMeasurement reading,
            bool useLowPass,
            bool useLowPassDiff,
            int newFitMatrixSize,
            float newSignalAperture,
            bool fixedAperture,
            MeasurementsHelper measurer,
			IImagePixel[] groupCenters,
			float[] aperturesInGroup)
        {
			reading.ReProcessingPsfFitMatrixSize = newFitMatrixSize;

			TrackedObjectConfig objConfig = Footer.TrackedObjects[reading.TargetNo];
			ImagePixel center = new ImagePixel(reading.X0, reading.Y0);

		    int areaSize = groupCenters != null && groupCenters.Length > 1 ? 35 : 17;

		    if (Context.Filter != LightCurveContext.FilterType.NoFilter) areaSize += 2;

			uint[,] data = BitmapFilter.CutArrayEdges(reading.PixelData, (35 - areaSize) / 2);

		    var filter = TangraConfig.PreProcessingFilter.NoFilter;
			if (useLowPassDiff) filter = TangraConfig.PreProcessingFilter.LowPassDifferenceFilter;
			else if (useLowPass) filter = TangraConfig.PreProcessingFilter.LowPassFilter;

			NotMeasuredReasons rv = ReduceLightCurveOperation.MeasureObject(
				center,
				data,
				reading.PixelData,
				Context.BitPix,
				measurer,
				filter,
				false,
				Context.SignalMethod,
				Context.PsfQuadratureMethod,
				newSignalAperture,
				objConfig.RefinedFWHM,
				Footer.RefinedAverageFWHM,
				reading,
				groupCenters,
				aperturesInGroup,
				Footer.ReductionContext.FullDisappearance);

			reading.SetIsMeasured(rv);
			reading.TotalReading = (uint)measurer.TotalReading;
			reading.TotalBackground = (uint)measurer.TotalBackground;
			reading.ApertureX = measurer.XCenter;
			reading.ApertureY = measurer.YCenter;
			reading.ApertureSize = measurer.Aperture;

			return reading;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_CancelExecution = true;
            Cursor = Cursors.WaitCursor;
            try
            {
                // Give it 2 seconds to finish
                if (!m_WorkerThread.Join(2000))
                {
                    m_WorkerThread.Abort();
                    m_WorkerThread.Join(1000);
                }
            }
            finally
            {
                m_WorkerThread = null;
                Cursor = Cursors.Default;
            }

            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
