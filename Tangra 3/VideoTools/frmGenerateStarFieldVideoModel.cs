using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tangra.Astrometry;
using Tangra.Astrometry.Recognition;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.PInvoke;
using Tangra.StarCatalogues;
using Tangra.StarCatalogues.UCAC4;
using Tangra.VideoOperations.Astrometry;

namespace Tangra.VideoTools
{
    public partial class frmGenerateStarFieldVideoModel : Form
    {
        private double m_RA;
        private double m_DE;

        internal enum ModelledFilter
        {
            B,
            V,
            Sloan_g,
            Sloan_r,
            Sloan_i
        }
        internal class ModelConfig
        {
            public string FileName;
            public bool IsAAVFile;
            public int Integration;
            public int TotalFrames;
            public int FrameWidth;
            public int FrameHeight;
            public int NoiseMean;
            public uint MaxPixelValue;
            public int NoiseStdDev;
            public int FlickeringStdDev;
            public double BrighestUnsaturatedStarMag;
            public double LimitStarMag;
            public double FWHM;
            public double RADeg2000;
            public double DEDeg2000;
            public double PlatePixWidth;
            public double PlatePixHeight;
            public double PlateFocLength;
            public ModelledFilter PhotometricFilter;
            public double BVSlope;
            public double LinearityCoefficient;

            public string InfoLine1;
            public string InfoLine2;
            public string InfoLine3;
            public string InfoLine4;
        }


        public frmGenerateStarFieldVideoModel()
        {
            InitializeComponent();

            cbxVideoFormat.SelectedIndex = 0;
            cbxAAVIntegration.SelectedIndex = 5;
            cbxPhotometricFilter.SelectedIndex = 3;
        }

        private void cbxVideoFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxVideoFormat.SelectedIndex == 1)
            {
                cbxAAVIntegration.SelectedIndex = 0;
                cbxAAVIntegration.Enabled = false;
            }
            else
                cbxAAVIntegration.Enabled = true;
        }

        private bool ValidateParameters()
        {
            try
            {
                m_RA = AstroConvert.ToRightAcsension(tbxRA.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbxRA.Focus();
                return false;
            }

            try
            {
                m_DE = AstroConvert.ToDeclination(tbxDE.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbxDE.Focus();
                return false;
            }

            return true;
        }

        private void btnGenerateVideo_Click(object sender, EventArgs e)
        {
            if (!ValidateParameters())
                return;

            if (cbxVideoFormat.SelectedIndex == 0)
            {
                saveFileDialog.Filter = "AAV Files (*.aav)|*.aav";
                saveFileDialog.DefaultExt = "aav";
            }
            else if (cbxVideoFormat.SelectedIndex == 1)
            {
                saveFileDialog.Filter = "AVI Files (*.avi)|*.avi";
                saveFileDialog.DefaultExt = "avi";
            }

            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                UsageStats.Instance.ModelVideosGenerated++;
                UsageStats.Instance.Save();

                var config = new ModelConfig()
                {
                    FileName = saveFileDialog.FileName,
                    IsAAVFile = cbxVideoFormat.SelectedIndex == 0,
                    Integration = (int)Math.Pow(2, cbxAAVIntegration.SelectedIndex),
                    TotalFrames = (int)nudTotalFrames.Value,
                    FrameWidth = (int)nudFrameWidth.Value,
                    FrameHeight = (int)nudFrameHeight.Value,
                    NoiseMean = (int)nudNoiseMean.Value,
                    NoiseStdDev = (int)nudNoiseStdDev.Value,
                    FlickeringStdDev = (int)nudStarFlickering.Value,
                    FWHM = (double)nudStar1FWHM.Value,
                    BrighestUnsaturatedStarMag = (double)nudFirstSaturatedMag.Value,
                    LimitStarMag = (double)nudLimitMag.Value,
                    RADeg2000 = m_RA * 15,
                    DEDeg2000 = m_DE,
                    PlatePixWidth = (double)nudPlatePixWidth.Value,
                    PlatePixHeight = (double)nudPlatePixHeight.Value,
                    PlateFocLength = (double)nudPlateFocLength.Value,
                    PhotometricFilter = (ModelledFilter)cbxPhotometricFilter.SelectedIndex,
                    BVSlope = (double)nudBVSlope.Value,
                    LinearityCoefficient = (double)nudNonLinearity.Value
                };

                config.InfoLine1 = string.Format("Model Video Generated by Tangra v.{0}", ((AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true)[0]).Version);
                config.InfoLine2 = string.Format("Noise: {0} +/- {1}, Flickering: {2}, FWHM: {3}", config.NoiseMean, config.NoiseStdDev, config.FlickeringStdDev, config.FWHM.ToString("0.0"));
                config.InfoLine3 = String.Format("Field Center: RA={0} DE={1}", AstroConvert.ToStringValue(m_RA, "HHh MMm SSs"), AstroConvert.ToStringValue(m_DE, "+DDo MM' SS\""));
                config.InfoLine4 = "";

                ThreadPool.QueueUserWorkItem(new WaitCallback(GenerateSimulatedVideo), config);
            }
        }

        private delegate void UpdateUIDelegate(int pbarId, int percent, bool show);

        private void UpdateUI(int pbarId, int percent, bool show)
        {
            pbar.Value = percent;

            if (show && !pbar.Visible)
            {
                pbar.Visible = true;
                pnlConfig.Enabled = false;
            }
            else if (!show && pbar.Visible)
            {
                pbar.Visible = false;
                pnlConfig.Enabled = true;
            }

            pbar.Update();

            Update();
            Application.DoEvents();
        }

        private void InvokeUpdateUI(int pbarId, int percentDone, bool show)
        {
            try
            {
                Invoke(new UpdateUIDelegate(UpdateUI), new object[] { pbarId, percentDone, show });
            }
            catch (InvalidOperationException)
            { }
        }

        private void GenerateSimulatedVideo(object state)
        {
            InvokeUpdateUI(2, 0, true);

            try
            {
                ModelConfig modelConfig = (ModelConfig)state;
                int bitPix = modelConfig.IsAAVFile ? 16 : 8;

                modelConfig.MaxPixelValue = modelConfig.IsAAVFile ? (uint)modelConfig.Integration * 255 : 255;

                double dxRad = modelConfig.FrameWidth * modelConfig.PlatePixWidth / (modelConfig.PlateFocLength * 1000.0);
                double dyRad = modelConfig.FrameHeight* modelConfig.PlatePixHeight / (modelConfig.PlateFocLength * 1000.0);
                double fovDeg = AngleUtility.Elongation(0, 0, dxRad * 180 / Math.PI, dyRad * 180 / Math.PI);

                var catFac = new StarCatalogueFacade(TangraConfig.Settings.StarCatalogue);
                List<IStar> stars = catFac.GetStarsInRegion(m_RA * 15, m_DE, 2 * fovDeg, modelConfig.LimitStarMag, 2000);

                // Hardcoded linear constants from real video
                PlateConstantsFit plateConstants = PlateConstantsFit.ConstructLinearConstantsFit(
                    -0.000006765899190204664, -0.00000033176271357927884, 0.0025137491513314075,
                    -0.00000035811169680477461, 0.0000062149091643718959, -0.0016676136324660906,
                     -147383.59154233566, -7867.5937127855022, 357.36527162957549,
                     -8492.4472188551754, 160450.05587246153, 288.91658249198673);

                AavFileCreator.CloseFile();
                AavFileCreator.StartNewFile(modelConfig.FileName, modelConfig.FrameWidth, modelConfig.FrameHeight, modelConfig.Integration);
                try
                {
                    //Pixelmap pixmap = new Pixelmap(modelConfig.FrameWidth, modelConfig.FrameHeight, bitPix, new uint[modelConfig.FrameWidth * modelConfig.FrameHeight], null, null);
                    //AddOnScreenText(bmp, modelConfig, "The simulated video stars from the next frame");
                    //TangraVideo.AddAviVideoFrame(pixmap, modelConfig.Gamma, null);

                    int[,] simulatedBackground = new int[modelConfig.FrameWidth, modelConfig.FrameHeight];
                    for (int x = 0; x < modelConfig.FrameWidth; x++)
                        for (int y = 0; y < modelConfig.FrameHeight; y++)
                        {
                            simulatedBackground[x, y] = 0;
                        }
                    DateTime zeroFrameDT = DateTime.UtcNow;

                    for (int i = 0; i <= modelConfig.TotalFrames; i++)
                    {
                        using (Pixelmap pixmap = new Pixelmap(modelConfig.FrameWidth, modelConfig.FrameHeight, bitPix, new uint[modelConfig.FrameWidth*modelConfig.FrameHeight], null, null))
                        {
                            pixmap.SetMaxSignalValue((uint)(255 * modelConfig.Integration));

                            VideoModelUtils.GenerateNoise(pixmap, simulatedBackground, modelConfig.NoiseMean * modelConfig.Integration, modelConfig.NoiseStdDev * modelConfig.Integration);
                            GenerateFrame(plateConstants, pixmap, stars, modelConfig);

                            DateTime startDT = zeroFrameDT.AddMilliseconds(40 * modelConfig.Integration * i);
                            AavFileCreator.AddVideoFrame(startDT, startDT.AddMilliseconds(40 * modelConfig.Integration), pixmap);
                        }

                        InvokeUpdateUI(2, (int)(100.0 * i / modelConfig.TotalFrames), true);
                    }
                }
                finally
                {
                    AavFileCreator.CloseFile();
                }
            }
            finally
            {
                InvokeUpdateUI(2, 100, false);
            }
        }

        private void GenerateFrame(PlateConstantsFit plateConstants, Pixelmap pixmap, List<IStar> stars, ModelConfig modelConfig)
        {
            foreach (IStar star in stars)
            {
                double xTang, yTang, x, y;
                TangentPlane.CelestialToTangent(star.RADeg, star.DEDeg, modelConfig.RADeg2000, modelConfig.DEDeg2000, out xTang, out yTang);
                plateConstants.GetImageCoordsFromTangentCoords(xTang, yTang, out x, out y);

                if (x < 0 || x > modelConfig.FrameWidth || y < 0 || y > modelConfig.FrameHeight)
                    continue;

                float starMag = GetStarMag(star, modelConfig.PhotometricFilter);

                float iMax = ModelStarAmplitude(star, starMag, modelConfig);
                
                VideoModelUtils.GenerateStar(pixmap, (float)x, (float)y, (float)modelConfig.FWHM, iMax);
            }
        }

        private float ModelStarAmplitude(IStar star, float starMag, ModelConfig modelConfig)
        {
            if (Math.Abs(modelConfig.BVSlope) > 0.001)
            {
                UCAC4Entry ucac4Star = star as UCAC4Entry;
                if (ucac4Star != null)
                {
                    double bv = ucac4Star.MagB - ucac4Star.MagV;
                    if (double.IsNaN(bv)) bv = 0.5; // Default value

                    starMag += (float)(modelConfig.BVSlope * bv);
                }
            }

            return (float)((double)(modelConfig.MaxPixelValue - modelConfig.NoiseMean * modelConfig.Integration) / Math.Pow(10, (starMag - modelConfig.BrighestUnsaturatedStarMag) / 2.5));
        }

        private float GetStarMag(IStar star, ModelledFilter filter)
        {
            UCAC4Entry ucac4Star = star as UCAC4Entry;
            if (ucac4Star != null)
            {
                switch (filter)
                {
                    case ModelledFilter.B:
                        return double.IsNaN(ucac4Star.MagB) || ucac4Star.MagB > 25 ? (float)ucac4Star.Mag : (float)ucac4Star.MagB;

                    case ModelledFilter.V:
                        return double.IsNaN(ucac4Star.MagV) || ucac4Star.MagV > 25 ? (float)ucac4Star.Mag : (float)ucac4Star.MagV;

                    case ModelledFilter.Sloan_g:
                        return double.IsNaN(ucac4Star.Mag_g) || ucac4Star.Mag_g > 25 ? (float)ucac4Star.Mag : (float)ucac4Star.Mag_g;

                    case ModelledFilter.Sloan_r:
                        return double.IsNaN(ucac4Star.Mag_r) || ucac4Star.Mag_r > 25 ? (float)ucac4Star.Mag : (float)ucac4Star.Mag_r;

                    case ModelledFilter.Sloan_i:
                        return double.IsNaN(ucac4Star.Mag_i) || ucac4Star.Mag_i > 25 ? (float)ucac4Star.Mag : (float)ucac4Star.Mag_i;
                }
            }
            else
            {
                switch (filter)
                {
                    case ModelledFilter.B:
                        return double.IsNaN(star.MagB) || star.MagB > 25 ? (float)star.Mag : (float)star.MagB;

                    case ModelledFilter.V:
                    case ModelledFilter.Sloan_g:
                        return double.IsNaN(star.MagV) || star.MagV > 25 ? (float)star.Mag : (float)star.MagV;

                    case ModelledFilter.Sloan_r:
                        return double.IsNaN(star.MagR) || star.MagR > 25 ? (float)star.Mag : (float)star.MagR;

                    case ModelledFilter.Sloan_i:
                        return (float)star.Mag;
                }
            }

            return (float)star.Mag;
        }
    }
}
