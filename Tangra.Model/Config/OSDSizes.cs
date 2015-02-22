using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
	public class OSDSizes
	{
		private static double MIN_KIWI_X_PERC = 0; //0.08183;
		private static double MAX_KIWI_X_PERC = 1; //0.91540;
		private static double MIN_KIWI_Y_PERC = 0.83; //0.87370;
		private static double MAX_KIWI_Y_PERC = 1; //0.93772;

		private Dictionary<long, Rectangle> m_OSDSizes = new Dictionary<long, Rectangle>();

		public string PersistedStringValue
		{
			get
			{
				var output = new StringBuilder();
				foreach (long id in m_OSDSizes.Keys)
				{
					Rectangle rect = m_OSDSizes[id];
					output.AppendFormat("{0},{1},{2},{3},{4}|", id, rect.Left, rect.Top, rect.Width, rect.Height);
				}
				return output.ToString();
			}
			set
			{
				m_OSDSizes.Clear();
				if (value != null)
				{
					string[] entries = value.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					foreach (string entry in entries)
					{
						try
						{
							string[] tokens = entry.Split(',');
							long id = long.Parse(tokens[0]);
							int left = int.Parse(tokens[1]);
							int top = int.Parse(tokens[2]);
							int width = int.Parse(tokens[3]);
							int height = int.Parse(tokens[4]);

							m_OSDSizes.Add(id, new Rectangle(left, top, width, height));
						}
						catch
						{ }

					}
				}
			}
		}

		public Rectangle GetOSDRectangleForFrameSize(int width, int height)
		{
			long key = FrameSizeToLongKey(width, height);
			Rectangle rect;
			if (!m_OSDSizes.TryGetValue(key, out rect) ||
				rect == Rectangle.Empty)
			{
				// Return a default OSD rectangle
				return new Rectangle(
					(int)Math.Round(MIN_KIWI_X_PERC * width),
					(int)Math.Round(MIN_KIWI_Y_PERC * height),
					(int)Math.Round((MAX_KIWI_X_PERC - MIN_KIWI_X_PERC) * width),
					(int)Math.Round((MAX_KIWI_Y_PERC - MIN_KIWI_Y_PERC) * height));
			}
			return rect;
		}

		public void AddOrUpdateOSDRectangleForFrameSize(int width, int height, Rectangle rect)
		{
			long key = FrameSizeToLongKey(width, height);
			m_OSDSizes[key] = rect;
		}

		private long FrameSizeToLongKey(int width, int height)
		{
			return ((long)height << 32) + (long)width;
		}
	}
}
