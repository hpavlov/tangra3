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
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class frmReprocessSeries : Form
    {
        internal List<List<LCMeasurement>> AllReadings;
        internal LCMeasurementHeader Header;
        internal LCMeasurementFooter Footer;
        internal Color[] AllColor;
        internal frmLightCurve.LightCurveContext Context;

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
            bool useLowPassDiff = Context.Filter == frmLightCurve.LightCurveContext.FilterType.LowPassDifference;
            bool useLowPass = Context.Filter == frmLightCurve.LightCurveContext.FilterType.LowPass;

            List<LCMeasurement>[] workReadings = new List<LCMeasurement>[4] { new List<LCMeasurement>(), new List<LCMeasurement>(), new List<LCMeasurement>(), new List<LCMeasurement>() };


            MeasurementsHelper measurer = new MeasurementsHelper(
                 Context.BitPix,
                 Context.BackgroundMethod,
                 true,
                 TangraConfig.Settings.Photometry.Saturation.GetSaturationForBpp(Context.BitPix));

            measurer.SetCoreProperties(
                TangraConfig.Settings.Photometry.AnulusInnerRadius,
                TangraConfig.Settings.Photometry.AnulusMinPixels,
                TangraConfig.PhotometrySettings.REJECTION_BACKGROUND_PIXELS_STD_DEV,
                Header.PositionTolerance);

            m_EllapsedTime.Start();

            measurer.GetImagePixelsCallback += new MeasurementsHelper.GetImagePixelsDelegate(measurer_GetImagePixelsCallback);

            int refreshCount = m_RefreshProgressEveryHowManyItems;

	        List<LCMeasurement> readings0 = AllReadings[0];
			List<LCMeasurement> readings1 = Header.ObjectCount > 1 ? AllReadings[1] : null;
			List<LCMeasurement> readings2 = Header.ObjectCount > 2 ? AllReadings[2] : null;
			List<LCMeasurement> readings3 = Header.ObjectCount > 3 ? AllReadings[3] : null;

            int processedCount = 0;

			for (int i = 0; i < readings0.Count; i++)
			{
				LCMeasurement[] units = new LCMeasurement[Header.ObjectCount];
				units[0] = readings0[i];
				if (Header.ObjectCount > 1) units[1] = readings1[i];
				if (Header.ObjectCount > 2) units[2] = readings2[i];
				if (Header.ObjectCount > 3) units[3] = readings3[i];

                if (m_CancelExecution)
                {
                    Invoke(new MethodInvoker(Cancelled));
                    return;
                }

				LCMeasurement[] remeasuredUnits = ProcessSingleUnitSet(units, useLowPass, useLowPassDiff, measurer);

				workReadings[0].Add(remeasuredUnits[0]);
				if (Header.ObjectCount > 1) workReadings[1].Add(remeasuredUnits[1]);
				if (Header.ObjectCount > 2) workReadings[2].Add(remeasuredUnits[2]);
				if (Header.ObjectCount > 3) workReadings[3].Add(remeasuredUnits[3]);

                processedCount++;

                if (processedCount % refreshCount == 0)
                {
                    Invoke(new ProgressDelegate(OnProgress), 0, processedCount);
					if (Header.ObjectCount > 1) Invoke(new ProgressDelegate(OnProgress), 1, processedCount);
					if (Header.ObjectCount > 2) Invoke(new ProgressDelegate(OnProgress), 2, processedCount);
					if (Header.ObjectCount > 3) Invoke(new ProgressDelegate(OnProgress), 3, processedCount);
                }
            }

            AllReadings = new List<List<LCMeasurement>>(workReadings);

            Invoke(new MethodInvoker(Done));
        }

        uint[,] measurer_GetImagePixelsCallback(int x, int y, int matrixSize)
        {
            throw new NotImplementedException();
        }

	    private LCMeasurement[] ProcessSingleUnitSet(
			LCMeasurement[] units,
		    bool useLowPass,
		    bool useLowPassDiff,
		    MeasurementsHelper measurer)
	    {
		    LCMeasurement[] rv = new LCMeasurement[units.Length];

		    for (int i = 0; i < units.Length; i++)
		    {
			    LCMeasurement reading = units[i];
			    IImagePixel[] groupCenters = new IImagePixel[0];

				TrackedObjectConfig objConfig = Footer.TrackedObjects[reading.TargetNo];
				List<TrackedObjectConfig> gropedObjects = Footer.TrackedObjects.Where(x => x.GroupId == objConfig.GroupId).ToList();
				if (gropedObjects.Count > 1)
				{
					groupCenters = new IImagePixel[gropedObjects.Count];

					for (int j = 0; j < gropedObjects.Count; j++)
					{
						LCMeasurement mea = units[Footer.TrackedObjects.IndexOf(gropedObjects[j])];
						groupCenters[j] = new ImagePixel(mea.X0, mea.Y0);
					}
				}

				LCMeasurement remeasuredReading = ProcessSingleUnit(
					reading, useLowPass, useLowPassDiff, 
					Context.ReProcessFitAreas[i], Context.ReProcessApertures[i], Header.FixedApertureFlags[i], 
					measurer, groupCenters);

			    rv[i] = remeasuredReading;
		    }

		    return rv;
	    }

	    private LCMeasurement ProcessSingleUnit(
            LCMeasurement reading,
            bool useLowPass,
            bool useLowPassDiff,
            int newFitMatrixSize,
            float newSignalAperture,
            bool fixedAperture,
            MeasurementsHelper measurer,
			IImagePixel[] groupCenters)
        {

            // TODO: Handle the correct links to the m_PreviousMeasurement and m_PrevSuccessfulReading !!!
            LCMeasurement clonedValue = reading.Clone();
            clonedValue.ReProcessingPsfFitMatrixSize = newFitMatrixSize;

            TrackedObjectConfig objConfig = Footer.TrackedObjects[clonedValue.TargetNo];			
            ImagePixel center = new ImagePixel(clonedValue.X0, clonedValue.Y0);

		    int areaSize = groupCenters != null && groupCenters.Length > 1 ? 35 : 17;

		    if (Context.Filter != frmLightCurve.LightCurveContext.FilterType.NoFilter) areaSize += 2;
            
			uint[,] data = BitmapFilter.CutArrayEdges(clonedValue.PixelData, Math.Max(0, (35 - areaSize) / 2));

			NotMeasuredReasons rv = ReduceLightCurveOperation.MeasureObject(
				center,
				data,
				clonedValue.PixelData,
				Context.BitPix,
				measurer,
				(TangraConfig.PreProcessingFilter)((int)Context.Filter),
				false,
				Context.SignalMethod,
				Context.PsfQuadratureMethod,
				newSignalAperture,
				objConfig.RefinedFWHM,
				Footer.RefinedAverageFWHM,
				clonedValue,
				groupCenters,
				Footer.ReductionContext.FullDisappearance);

			clonedValue.SetIsMeasured(rv);
            clonedValue.TotalReading = (uint)measurer.TotalReading;
            clonedValue.TotalBackground = (uint)measurer.TotalBackground;

            return clonedValue;
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
