using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
	public abstract class AreaSizesBase
	{
		protected static double MIN_KIWI_X_PERC = 0; //0.08183;
		protected static double MAX_KIWI_X_PERC = 1; //0.91540;
		protected static double MIN_KIWI_Y_PERC = 0.83; //0.87370;
		protected static double MAX_KIWI_Y_PERC = 1; //0.93772;

		protected Dictionary<long, Rectangle> m_AreaSizes = new Dictionary<long, Rectangle>();

		public string PersistedStringValue
		{
			get
			{
				var output = new StringBuilder();
				foreach (long id in m_AreaSizes.Keys)
				{
					Rectangle rect = m_AreaSizes[id];
					output.AppendFormat("{0},{1},{2},{3},{4}|", id, rect.Left, rect.Top, rect.Width, rect.Height);
				}
				return output.ToString();
			}
			set
			{
				m_AreaSizes.Clear();
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

							m_AreaSizes.Add(id, new Rectangle(left, top, width, height));
						}
						catch
						{ }

					}
				}
			}
		}

		protected Rectangle GetRectangleForFrameSize(int width, int height)
		{
			long key = FrameSizeToLongKey(width, height);
			Rectangle rect;
			if (!m_AreaSizes.TryGetValue(key, out rect) ||
				rect == Rectangle.Empty)
			{
				return GetDefaultRectangle(width, height);
			}
			return rect;
		}

		protected void AddOrUpdateRectangleForFrameSize(int width, int height, Rectangle rect)
		{
			long key = FrameSizeToLongKey(width, height);
			m_AreaSizes[key] = rect;
		}

		private long FrameSizeToLongKey(int width, int height)
		{
			return ((long)height << 32) + (long)width;
		}

		protected abstract Rectangle GetDefaultRectangle(int width, int height);
	}

	public class IncludeAreaSizes : AreaSizesBase
	{
		public Rectangle GetInclusionRectangleForFrameSize(int width, int height)
		{
			return GetRectangleForFrameSize(width, height);
		}

		public void AddOrUpdateInclusionRectangleForFrameSize(int width, int height, Rectangle rect)
		{
			AddOrUpdateRectangleForFrameSize(width, height, rect);
		}

		protected override Rectangle GetDefaultRectangle(int width, int height)
		{
			return new Rectangle(0, 0,
								(int)Math.Round((MAX_KIWI_X_PERC - MIN_KIWI_X_PERC) * width),
								(int)Math.Round((1 - (MAX_KIWI_Y_PERC - MIN_KIWI_Y_PERC)) * height));
		}
	}

	public class OSDSizes : AreaSizesBase
	{
		public Rectangle GetOSDRectangleForFrameSize(int width, int height)
		{
			return GetRectangleForFrameSize(width, height);
		}

		public void AddOrUpdateOSDRectangleForFrameSize(int width, int height, Rectangle rect)
		{
			AddOrUpdateRectangleForFrameSize(width, height, rect);
		}

		protected override Rectangle GetDefaultRectangle(int width, int height)
		{
			return new Rectangle(
								(int)Math.Round(MIN_KIWI_X_PERC * width),
								(int)Math.Round(MIN_KIWI_Y_PERC * height),
								(int)Math.Round((MAX_KIWI_X_PERC - MIN_KIWI_X_PERC) * width),
								(int)Math.Round((MAX_KIWI_Y_PERC - MIN_KIWI_Y_PERC) * height));
		}
	}

}
