using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Controller;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;

namespace Tangra.Helpers
{
	public class DuplicateFrameAvoider
	{
		private VideoController m_VideoController;
		private int m_FrameId;

		private Random m_Randomizer = new Random(5123);

		public DuplicateFrameAvoider(VideoController videoController, int frameId)
		{
			m_VideoController = videoController;

			m_FrameId = frameId;
		}

		public bool IsDuplicatedFrame()
		{
			Pixelmap prevFrame = m_VideoController.GetFrame(m_FrameId - 1);
			Pixelmap thisFrame = m_VideoController.GetFrame(m_FrameId);
			Pixelmap nextFrame = m_VideoController.GetFrame(m_FrameId + 1);

			try
			{
				return
					ArePixelmapsTheSame(prevFrame, thisFrame) ||
					ArePixelmapsTheSame(thisFrame, nextFrame);
			}
			finally
			{
				//if (prevFrame != null) prevFrame.Dispose();
				//if (thisFrame != null) thisFrame.Dispose();
				//if (nextFrame != null) nextFrame.Dispose();
			}
		}

		public int GetFirstGoodFrameId()
		{
			int probedFrame = m_FrameId;

			Pixelmap prevFrame = m_VideoController.GetFrame(probedFrame - 1);
			Pixelmap thisFrame = m_VideoController.GetFrame(probedFrame);
			Pixelmap nextFrame = null;

			try
			{
				do
				{
					nextFrame = m_VideoController.GetFrame(probedFrame + 1);

					if (!ArePixelmapsTheSame(prevFrame, thisFrame) &&
						!ArePixelmapsTheSame(thisFrame, nextFrame))
					{
						return probedFrame;
					}

					//if (prevFrame != null) prevFrame.Dispose();
					prevFrame = thisFrame;
					thisFrame = nextFrame;
					probedFrame++;
				}
				while (thisFrame != null && nextFrame != null && probedFrame < m_VideoController.VideoLastFrame);

			}
			finally
			{
				if (prevFrame != null) prevFrame.Dispose();
				if (thisFrame != null) thisFrame.Dispose();
				if (nextFrame != null) nextFrame.Dispose();
			}

			return m_FrameId;
		}

		public int GetLastGoodFrameId()
		{
			int probedFrame = m_FrameId;

			Pixelmap prevFrame = null;
			Pixelmap thisFrame = m_VideoController.GetFrame(probedFrame);
			Pixelmap nextFrame = m_VideoController.GetFrame(probedFrame + 1);

			try
			{
				do
				{
					prevFrame = m_VideoController.GetFrame(probedFrame - 1);

					if (!ArePixelmapsTheSame(prevFrame, thisFrame) &&
						!ArePixelmapsTheSame(thisFrame, nextFrame))
					{
						return probedFrame;
					}

					//if (nextFrame != null) nextFrame.Dispose();
					nextFrame = thisFrame;
					thisFrame = prevFrame;
					probedFrame--;
				}
				while (thisFrame != null && prevFrame != null);

			}
			finally
			{
				if (prevFrame != null) prevFrame.Dispose();
				if (thisFrame != null) thisFrame.Dispose();
				if (nextFrame != null) nextFrame.Dispose();
			}

			return m_FrameId;
		}

		public bool ArePixelmapsTheSame(Pixelmap bmp1, Pixelmap bmp2)
		{
			if (bmp1 == null && bmp2 == null) return true;
			if (bmp1 == null || bmp2 == null) return false;

			int x = m_Randomizer.Next(bmp1.Width - 1);
			int y = m_Randomizer.Next(bmp1.Height - 1);

			if (bmp1[x, y] != bmp2[x, y])
				return false;

			x = m_Randomizer.Next(bmp1.Width - 1);
			y = m_Randomizer.Next(bmp1.Height - 1);

			if (bmp1[x, y] != bmp2[x, y])
				return false;

			// Check all pixels
			int width = bmp1.Width;
			int height = bmp1.Height;


			for (y = 0; y < height; ++y)
			{
				for (x = 0; x < width; ++x)
				{
					if (bmp1[x, y] != bmp2[x, y])
						return false;
				}
			}

			return true;
		}
	}
}
