using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Image;
using Tangra.Video.AstroDigitalVideo.UI;

namespace Tangra.Video.AstroDigitalVideo
{
	public partial class frmAdvViewer : Form
	{
		private string m_FileName;
		private AdvFile m_AdvFile;

		private AdvImageData m_CurrentImageData;
		private Pixelmap m_CurrentPixelmap;
		private AdvStatusData m_CurrentStatusData;

		public frmAdvViewer(string fileName)
		{
			InitializeComponent();

			timerScrolling.Enabled = false;

			m_FileName = fileName;
			m_AdvFile = AdvFile.OpenFile(fileName);
		}

		private void frmAdvViewer_Load(object sender, EventArgs e)
		{
			if (m_AdvFile.IsCorrupted)
				MessageBox.Show(
					this,
					"This is not an ADV/AAV file or it is corrupted. Try using [Tools] -> [ADV/AAV Tools] -> [Repair ADV/AAV File] to repair it.",
					"Tangra",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			else
				PresentFile();
		}

		private void PresentFile()
		{
			lblFileName.Text = m_FileName;
			lblNumberFrames.Text = m_AdvFile.NumberOfFrames.ToString();
			
			var bindingList = new List<AdvTagValuePair>();
			foreach(string key in m_AdvFile.AdvFileTags.Keys)
			{
				bindingList.Add(new AdvTagValuePair() {Tag = key, Value = m_AdvFile.AdvFileTags[key]});
			}
			dgvFileTags.DataSource = bindingList;

			lvImageLayouts.Items.Clear();
			foreach(byte layoutId in m_AdvFile.ImageSection.ImageLayouts.Keys)
			{
				AdvImageLayout layout = m_AdvFile.ImageSection.ImageLayouts[layoutId];

				ListViewItem li = lvImageLayouts.Items.Add(layoutId.ToString());
				li.SubItems.Add(layout.BitsPerPixel.ToString());
				li.SubItems.Add(layout.IsDiffCorrLayout ? "yes" : "no");
				li.Tag = layout;
			}
			if (lvImageLayouts.Items.Count > 0)
				lvImageLayouts.Items[0].Selected = true;


			lblWidth.Text = m_AdvFile.ImageSection.Width.ToString();
			lblHeight.Text = m_AdvFile.ImageSection.Height.ToString();
			lblBpp.Text = m_AdvFile.ImageSection.BitsPerPixel.ToString();

			var imageTagsList = new List<AdvTagValuePair>();
			foreach(string key in m_AdvFile.ImageSection.ImageSerializationProperties.Keys)
			{
				imageTagsList.Add(new AdvTagValuePair() {Tag = key, Value = m_AdvFile.ImageSection.ImageSerializationProperties[key]});
			}
			dgvImageTags.DataSource = imageTagsList;

			lvStatusTags.Items.Clear();
			int idx = 0;
			foreach (AdvTagDefinition tagDef in m_AdvFile.StatusSection.TagDefinitions)
			{
				ListViewItem li = lvStatusTags.Items.Add(idx.ToString());
				li.SubItems.Add(tagDef.Name);
				li.SubItems.Add(tagDef.Type.ToString());
				li.Tag = tagDef;

				idx++;
			}

			sbFrames.Maximum = (int)m_AdvFile.NumberOfFrames - 1;
			nudCropFirstFrame.Maximum = (int)m_AdvFile.NumberOfFrames - 1;
			nudCropFirstFrame.Value = 0;
			nudCropLastFrame.Maximum = (int)m_AdvFile.NumberOfFrames - 1;
			nudCropLastFrame.Value = nudCropLastFrame.Maximum;

			LoadFrame(0);
		}

		private void lvImageLayouts_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lvImageLayouts.SelectedItems.Count == 1)
			{
				AdvImageLayout layout = (AdvImageLayout)lvImageLayouts.SelectedItems[0].Tag;

				var bindingList = new List<AdvTagValuePair>();
				foreach (string key in layout.ImageSerializationProperties.Keys)
				{
					bindingList.Add(new AdvTagValuePair() { Tag = key, Value = layout.ImageSerializationProperties[key] });
				}
				dgvLayoutTags.DataSource = bindingList;
			}
		}

		private void sbFrames_Scroll(object sender, ScrollEventArgs e)
		{
			timerScrolling.Enabled = false;
			timerScrolling.Enabled = true;
		}

		private void timerScrolling_Tick(object sender, EventArgs e)
		{
			timerScrolling.Enabled = false;
			LoadFrame(sbFrames.Value);

		}

		private ushort[,] prevFramePixels;
		private int prevFrameNo = -1;

		internal Pixelmap GetFrameData(int index, out AdvImageData imageData, out AdvStatusData statusData,  out Bitmap displayBitmap)
		{
			imageData = null;

			if (index < m_AdvFile.NumberOfFrames)
			{
				byte layoutId;
				AdvImageLayout.GetByteMode byteMode;

				m_AdvFile.GetFrameImageSectionHeader(index, out layoutId, out byteMode);

				AdvImageLayout layout = m_AdvFile.ImageSection.GetImageLayoutFromLayoutId(layoutId);

				if (layout.IsDiffCorrLayout && byteMode == AdvImageLayout.GetByteMode.DiffCorrBytes && prevFrameNo != index - 1)
				{
					// Move back and find the nearest previous key frame
					int keyFrameIdx = index;
					do
					{
						keyFrameIdx--;
						m_AdvFile.GetFrameImageSectionHeader(keyFrameIdx, out layoutId, out byteMode);
					}
					while (keyFrameIdx > 0 && byteMode != AdvImageLayout.GetByteMode.KeyFrameBytes);

					object[] keyFrameData = m_AdvFile.GetFrameSectionData(keyFrameIdx, null);
					prevFramePixels = ((AdvImageData)keyFrameData[0]).ImageData;

					if (layout.DiffCorrFrame == DiffCorrFrameMode.PrevFrame)
					{
						for (int i = keyFrameIdx + 1; i < index; i++)
						{
							object[] frameData = m_AdvFile.GetFrameSectionData(i, prevFramePixels);

							prevFramePixels = ((AdvImageData)frameData[0]).ImageData;
						}
					}
				}

				object[] data;

				data = m_AdvFile.GetFrameSectionData(index, prevFramePixels);

				imageData = (AdvImageData)data[0];
				statusData = (AdvStatusData)data[1];

				if (prevFramePixels == null)
					prevFramePixels = new ushort[m_AdvFile.ImageSection.Width, m_AdvFile.ImageSection.Height];
				for (int x = 0; x < m_AdvFile.ImageSection.Width; x++)
					for (int y = 0; y < m_AdvFile.ImageSection.Height; y++)
					{
						prevFramePixels[x, y] = imageData.ImageData[x, y];
					}

				prevFrameNo = index;

				Pixelmap rv = m_AdvFile.ImageSection.CreatePixelmap(imageData);

				//Pixelmap rv = new Pixelmap((int)m_AdvFile.ImageSection.Width, (int)m_AdvFile.ImageSection.Height, m_AdvFile.ImageSection.BitsPerPixel, null, null, null);
				//rv.Width = (int) m_AdvFile.ImageSection.Width;
				//rv.Height = (int) m_AdvFile.ImageSection.Height;
				//rv.BitPixCamera = m_AdvFile.ImageSection.BitsPerPixel;
				//rv.Array = new uint[Width * Height];
				//rv.CopyPixelsFrom(imageData.ImageData, imageData.Bpp);
				//displayBitmap = PixelmapFactory.ConstructBitmapFrom12BitPixelmap(rv);

				displayBitmap = rv.DisplayBitmap;

				return rv;

			}
			else
			{
				displayBitmap = null;
				statusData = null;
				return null;
			}
		}

		private void LoadFrame(int frameId)
		{
			Bitmap displayBitmap;

			m_CurrentPixelmap = GetFrameData(frameId, out m_CurrentImageData, out m_CurrentStatusData, out displayBitmap);

			picSmallImage.Image = displayBitmap;

			int numSatellites = 0;
			string gamma = "";
			string gain = "";
			string shutter = "";
			string offset = "";
			string systemTime = string.Empty;
            string almanacOffsetStr = "";
            string almanacStatusStr = "";

			lvFrameStatusData.Items.Clear();

			foreach(AdvTagDefinition statusTag in m_CurrentStatusData.TagValues.Keys)
			{
				
				string tagValue = m_CurrentStatusData.TagValues[statusTag];
				if (statusTag.Name == "SystemTime" && !string.IsNullOrEmpty(tagValue))
				{
					systemTime = AdvFile.ADV_ZERO_DATE_REF.AddMilliseconds(long.Parse(tagValue)).ToString("dd-MMM-yyyy HH:mm:ss.fff");
					tagValue = systemTime;
				}
					

				if (statusTag.Name == "GPSTrackedSatellites" && !string.IsNullOrEmpty(tagValue))
					numSatellites = int.Parse(tagValue);
				else if (statusTag.Name == "Gamma" && !string.IsNullOrEmpty(tagValue))
				{
					gamma = string.Format("{0:0.000}", float.Parse(tagValue));
					tagValue = gamma;
				}
				else if (statusTag.Name == "Gain" && !string.IsNullOrEmpty(tagValue))
				{
					gain = string.Format("{0:0} dB", float.Parse(tagValue));
					tagValue = gain;
				}
				else if (statusTag.Name == "Shutter" && !string.IsNullOrEmpty(tagValue))
				{
					shutter = string.Format("{0:0.000} sec", float.Parse(tagValue));
					tagValue = shutter;
				}
				else if (statusTag.Name == "Offset" && !string.IsNullOrEmpty(tagValue))
				{
					offset = string.Format("{0:0.00} %", float.Parse(tagValue));
					tagValue = offset;
				}
				else if ((statusTag.Name == "VideoCameraFrameId" || statusTag.Name == "HardwareTimerFrameId") && !string.IsNullOrEmpty(tagValue))
				{
					tagValue = int.Parse(tagValue).ToString("#,###,###,###,###");
				}
				else if (statusTag.Name == "GPSAlmanacStatus" && !string.IsNullOrEmpty(tagValue))
				{
					int almanacStatus = int.Parse(tagValue);
					tagValue = AdvStatusValuesHelper.TranslateGpsAlmanacStatus(almanacStatus);
				    almanacStatusStr = tagValue;
				}
				else if (statusTag.Name == "GPSAlmanacOffset" && !string.IsNullOrEmpty(tagValue))
				{
					int almanacOffset = int.Parse(tagValue);
					if ((almanacOffset & 0x80) == 0x80)
						almanacOffset = (short) (almanacOffset + (0xFF << 8));

					tagValue = AdvStatusValuesHelper.TranslateGpsAlmanacOffset(1, almanacOffset, false);
                    almanacOffsetStr = tagValue;
				}
				else if (statusTag.Name == "GPSFixStatus" && !string.IsNullOrEmpty(tagValue))
				{
					int fixStatus = int.Parse(tagValue);
					tagValue = AdvStatusValuesHelper.TranslateGpsFixStatus(fixStatus);
				}

				if (!string.IsNullOrEmpty(tagValue) && (statusTag.Name == "UserCommand" || statusTag.Name == "SystemError" || statusTag.Name == "GPSFix"))
				{
					string[] tokens = tagValue.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < tokens.Length; i++)
					{
						ListViewItem li = lvFrameStatusData.Items.Add(string.Format("{0}[{1}]", statusTag.Name, i+1));
						li.SubItems.Add(tokens[i]);
						li.Tag = statusTag;	
					}
				}
				else
				{
					ListViewItem li = lvFrameStatusData.Items.Add(statusTag.Name);
					li.SubItems.Add(tagValue);
					li.Tag = statusTag;	
				} 
				
			}

			lblFrameId.Text = string.Format("Frame {0} of {1}", frameId, sbFrames.Maximum);
			lblFrameStart.Text = m_CurrentImageData.MidExposureUtc.AddMilliseconds(-0.5 * m_CurrentImageData.ExposureMilliseconds).ToString("dd MMM yyyy HH:mm:ss.fff");
			lblFrameEnd.Text = m_CurrentImageData.MidExposureUtc.AddMilliseconds(0.5 * m_CurrentImageData.ExposureMilliseconds).ToString("dd MMM yyyy HH:mm:ss.fff");

			lblFrameNumSatellites.Text = numSatellites.ToString();
			lblFrameGain.Text = gain;
			lblFrameGamma.Text = gamma;
			lblFrameOffset.Text = offset;
			lblFrameSystemTime.Text = systemTime;
            lblFrameAlmanacOffset.Text = almanacOffsetStr;
            lblFrameAlmanacStatus.Text = almanacStatusStr;

			double fps = 1000.0 / m_CurrentImageData.ExposureMilliseconds;
			lblFrameExposure.Text = string.Format("{0:0.0} ms ({1} {2})", m_CurrentImageData.ExposureMilliseconds, fps > 0 ? Math.Round(fps) : Math.Round(1 / fps), fps > 0 ? "fps" : "spf");

			lblFrameLayout.Text = string.Format("#{0} - {1}", m_CurrentImageData.LayoutId, m_CurrentImageData.ByteMode);
			lblDataBlockSize.Text = string.Format("{0} bytes", m_CurrentImageData.DataBlocksBytesCount.ToString("#,###,###,###"));
		}

		private void btnCropADV_Click(object sender, EventArgs e)
		{
			if (nudCropLastFrame.Value < nudCropFirstFrame.Value)
			{
				MessageBox.Show(
					this, 
					"The last frame cannot be 'before' the first frame.", 
					"Tangra", 
					MessageBoxButtons.OK, 
					MessageBoxIcon.Error);

				nudCropLastFrame.Focus();
				return;
			}

			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				m_AdvFile.CropAdvFile(saveFileDialog.FileName, (int) nudCropFirstFrame.Value, (int) nudCropLastFrame.Value);
			}
		}
	}	
}
