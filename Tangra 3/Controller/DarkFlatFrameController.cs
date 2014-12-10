/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
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

		private bool LoadDarkOrFlatFrameInternal(string title, ref uint[,] pixels, ref uint medianValue)
		{
			string filter = m_VideoController.VideoBitPix > 8
								? "FITS Image 16 bit (*.fit;*.fits)|*.fit;*.fits"
								: "Bitmaps (*.bmp)|*.bmp|FITS Image 16 bit (*.fit;*.fits)|*.fit;*.fits";
								 
			string fileName;
			if (m_VideoController.ShowOpenFileDialog(title, filter, out fileName) == DialogResult.OK &&
				File.Exists(fileName))
			{
				string extension = Path.GetExtension(fileName);

				if (extension == ".bmp")
				{
					using (Bitmap bmp = (Bitmap) Image.FromFile(fileName))
					{
						uint[,] pixelsCopy = new uint[bmp.Width, bmp.Height];
						var medianCalcList = new List<uint>();

						BitmapFilter.BitmapPixelOperation(
							bmp,
							(int x, int y, byte r, byte g, byte b) =>
							{
								uint val = Pixelmap.GetColourChannelValue(TangraConfig.Settings.Photometry.ColourChannel, r, g, b);

								pixelsCopy[x, y] = val;

								medianCalcList.Add(val);
							}
						);

						if (medianCalcList.Count > 0)
						{
							medianCalcList.Sort();

							if (medianCalcList.Count % 2 == 1)
								medianValue = medianCalcList[medianCalcList.Count / 2];
							else
								medianValue = (medianCalcList[medianCalcList.Count / 2] + medianCalcList[1 + (medianCalcList.Count / 2)]) / 2;
						}

						pixels = pixelsCopy;
					}

					return true;
				}
				else if (extension == ".fit" || extension == ".fits")
				{
					return FITSHelper.Load16BitFitsFile(
						fileName,
						out pixels,
						out medianValue,
						delegate(BasicHDU imageHDU)
						{
							if (
								imageHDU.Axes.Count() != 2 ||
								imageHDU.Axes[0] != TangraContext.Current.FrameHeight ||
								imageHDU.Axes[1] != TangraContext.Current.FrameWidth ||
								imageHDU.BitPix != 16)
							{
								m_VideoController.ShowMessageBox(
									"Selected image is not compatible with the currently loaded video.", "Tangra",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
								return false;
							}

							return true;
						});
				
				}
				else
				{
					MessageBox.Show(m_MainFormView, "Tangra", "Unsupported file type.", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
			}

			return false;
		}

		public void LoadDarkFrame()
		{
			uint[,] pixels = new uint[0, 0];
			uint medianValue = 0;

			if (LoadDarkOrFlatFrameInternal("Load dark frame ...", ref pixels, ref medianValue))
			{
				UsageStats.Instance.DarkFramesUsed++;
				UsageStats.Instance.Save();

				TangraCore.PreProcessors.AddDarkFrame(pixels, medianValue);

				TangraContext.Current.DarkFrameLoaded = true;
				m_VideoController.UpdateViews();

				m_VideoController.RefreshCurrentFrame();
			}
		}

		public void LoadFlatFrame()
		{
			uint[,] pixels = new uint[0, 0];
			uint medianValue = 0;

			if (LoadDarkOrFlatFrameInternal("Load flat frame ...", ref pixels, ref medianValue))
			{
				UsageStats.Instance.FlatFramesUsed++;
				UsageStats.Instance.Save();

				TangraCore.PreProcessors.AddFlatFrame(pixels, medianValue);

				TangraContext.Current.FlatFrameLoaded = true;
				m_VideoController.UpdateViews();

				m_VideoController.RefreshCurrentFrame();
			}
		}
	}
}
