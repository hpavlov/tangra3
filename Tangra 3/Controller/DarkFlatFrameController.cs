/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.PInvoke;
using nom.tam.fits;
using nom.tam.util;
using Tangra.Model.Helpers;

namespace Tangra.Controller
{
	public class DarkFlatFrameController
	{
		private Form m_MainFormView;
		private VideoController m_VideoController;

		public DarkFlatFrameController(Form mainFormView, VideoController videoController)
		{
			m_MainFormView = mainFormView;
			m_VideoController = videoController;			
		}

		public bool HasDarkFrameBytes
		{
			get { return TangraCore.PreProcessors.PreProcessingHasDarkFrameSet(); }
		}

        public bool HasBiasFrameBytes
        {
            get { return TangraCore.PreProcessors.PreProcessingHasBiasFrameSet(); }
        }

        private bool LoadDarkFlatOrBiasFrameInternal(string title, ref float[,] pixels, ref float medianValue, ref float exposureSeconds, ref int imagesCombined)
        {
			string filter = "FITS Image 16 bit (*.fit;*.fits)|*.fit;*.fits";

			string fileName;
			if (m_VideoController.ShowOpenFileDialog(title, filter, out fileName) == DialogResult.OK &&
				File.Exists(fileName))
			{
				
				Type pixelDataType;
			    int snapshot = 1;
			    bool hasNegativePixels;

				bool loaded = FITSHelper.LoadFloatingPointFitsFile(
					fileName,
                    null,
					out pixels,
					out medianValue,
					out pixelDataType,
                    out exposureSeconds,
                    out hasNegativePixels,
					delegate(BasicHDU imageHDU)
					{
						if (
							imageHDU.Axes.Count() != 2 ||
							imageHDU.Axes[0] != TangraContext.Current.FrameHeight ||
							imageHDU.Axes[1] != TangraContext.Current.FrameWidth)
						{
							m_VideoController.ShowMessageBox(
								"Selected image has a different frame size from the currently loaded video.", "Tangra",
								MessageBoxButtons.OK, MessageBoxIcon.Error);

                            return false;
						}						

						bool isFloatingPointImage = false;
						Array dataArray = (Array)imageHDU.Data.DataArray;
						object entry = dataArray.GetValue(0);
						if (entry is float[])
							isFloatingPointImage = true;
						else if (entry is Array)
						{
							isFloatingPointImage = ((Array)entry).GetValue(0) is float;
						}

					    HeaderCard imagesCombinedCard = imageHDU.Header.FindCard("SNAPSHOT");
					    if (imagesCombinedCard != null) int.TryParse(imagesCombinedCard.Value, out snapshot);
                        

						if (!isFloatingPointImage && imageHDU.BitPix != 16)
						{
							if (m_VideoController.ShowMessageBox(
								"Selected image data type may not be compatible with the currently loaded video. Do you wish to continue?", "Tangra",
								MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
							{
								return false;
							}
						}

						float usedEncodingGamma = float.NaN;
						string usedGammaString = null;
						HeaderCard tangraGammaCard = imageHDU.Header.FindCard("TANGAMMA");
						if (tangraGammaCard != null &&
						    float.TryParse(tangraGammaCard.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out usedEncodingGamma))
						{
							usedGammaString = usedEncodingGamma.ToString("0.0000", CultureInfo.InvariantCulture);
						}

						string gammaUsageError = null;
						string currGammaString = TangraConfig.Settings.Generic.ReverseGammaCorrection
							? TangraConfig.Settings.Photometry.EncodingGamma.ToString("0.0000", CultureInfo.InvariantCulture)
							: null;
                        if (TangraConfig.Settings.Generic.ReverseGammaCorrection && currGammaString != null && usedGammaString == null)
							gammaUsageError = string.Format("Selected image hasn't been Gamma corrected while the current video uses a gamma of {0}.", currGammaString);
                        else if (!TangraConfig.Settings.Generic.ReverseGammaCorrection && usedGammaString != null && currGammaString == null)
							gammaUsageError = string.Format("Selected image has been corrected for Gamma of {0} while the current video doesn't use gamma correction.", usedGammaString);
						else if (TangraConfig.Settings.Generic.ReverseGammaCorrection && !string.Equals(currGammaString, usedGammaString))
							gammaUsageError = string.Format("Selected image has been corrected for Gamma of {0} while the current video uses a gamma of {1}.", usedGammaString, currGammaString);

						if (gammaUsageError != null)
						{
							if (m_VideoController.ShowMessageBox(gammaUsageError + " Do you wish to continue?", "Tangra", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
							{
								return false;
							}
						}

						TangraConfig.KnownCameraResponse usedCameraResponse = TangraConfig.KnownCameraResponse.Undefined;
						string usedCameraResponseString = null;
						int usedCameraResponseInt = 0;
						HeaderCard tangraCamResponseCard = imageHDU.Header.FindCard("TANCMRSP");
						if (tangraCamResponseCard != null &&
							int.TryParse(tangraCamResponseCard.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out usedCameraResponseInt))
						{
							usedCameraResponse = (TangraConfig.KnownCameraResponse) usedCameraResponseInt;
							usedCameraResponseString = usedCameraResponse.ToString();
						}

						string cameraResponseUsageError = null;
						string currCameraResponseString = TangraConfig.Settings.Generic.ReverseCameraResponse
							? TangraConfig.Settings.Photometry.KnownCameraResponse.ToString()
							: null;
						if (TangraConfig.Settings.Generic.ReverseCameraResponse && currCameraResponseString != null && usedCameraResponseString == null)
							cameraResponseUsageError = string.Format("Selected image hasn't been corrected for camera reponse while the current video uses a camera response correction for {0}.", currCameraResponseString);
						else if (!TangraConfig.Settings.Generic.ReverseCameraResponse && usedCameraResponseString != null && currCameraResponseString == null)
							cameraResponseUsageError = string.Format("Selected image has been corrected for camera response of {0} while the current video doesn't use camera response correction.", usedCameraResponseString);
						else if (TangraConfig.Settings.Generic.ReverseCameraResponse && !string.Equals(currCameraResponseString, usedCameraResponseString))
							cameraResponseUsageError = string.Format("Selected image has been corrected for camera reponse of {0} while the current video uses a camera response correction for {1}.", usedCameraResponseString, currCameraResponseString);

						if (cameraResponseUsageError != null)
						{
							if (m_VideoController.ShowMessageBox(cameraResponseUsageError + " Do you wish to continue?", "Tangra", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
							{
								return false;
							}
						}

						return true;
					});

			    imagesCombined = snapshot;
			    return loaded;
			}

			return false;
		}

		public void LoadDarkFrame(bool isMasterDark, bool isSameExposure)
		{
			float[,] pixels = new float[0, 0];
			float medianValue = 0;
		    float exposureSeconds = 0;
		    int imagesCombined = 1;

            if (LoadDarkFlatOrBiasFrameInternal("Load dark frame ...", ref pixels, ref medianValue, ref exposureSeconds, ref imagesCombined))
			{
				UsageStats.Instance.DarkFramesUsed++;
				UsageStats.Instance.Save();

                TangraCore.PreProcessors.AddDarkFrame(pixels, exposureSeconds, isMasterDark, isSameExposure);

				TangraContext.Current.DarkFrameLoaded = true;
				m_VideoController.UpdateViews();

				m_VideoController.RefreshCurrentFrame();
			}
		}

		public void LoadFlatFrame()
		{
			float[,] pixels = new float[0, 0];
			float medianValue = 0;
            float exposureSeconds = 0;
            int imagesCombined = 1;

            if (LoadDarkFlatOrBiasFrameInternal("Load flat frame ...", ref pixels, ref medianValue, ref exposureSeconds, ref imagesCombined))
			{
				UsageStats.Instance.FlatFramesUsed++;
				UsageStats.Instance.Save();

                TangraCore.PreProcessors.AddFlatFrame(pixels, medianValue);

				TangraContext.Current.FlatFrameLoaded = true;
				m_VideoController.UpdateViews();

				m_VideoController.RefreshCurrentFrame();
			}
		}

        public void LoadBiasFrame()
        {
            float[,] pixels = new float[0, 0];
            float medianValue = 0;
            float exposureSeconds = 0;
            int imagesCombined = 1;

            if (LoadDarkFlatOrBiasFrameInternal("Load bias frame ...", ref pixels, ref medianValue, ref exposureSeconds, ref imagesCombined))
            {
                UsageStats.Instance.BiasFramesUsed++;
                UsageStats.Instance.Save();

                TangraCore.PreProcessors.AddBiasFrame(pixels);

                TangraContext.Current.BiasFrameLoaded = true;
                m_VideoController.UpdateViews();

                m_VideoController.RefreshCurrentFrame();
            }
        }
	}
}
