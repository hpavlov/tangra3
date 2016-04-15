using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Tangra.Model.Image;

namespace Tangra.Model.Astro
{
	[Serializable()]
	public class StarMapFeature : ICloneable, ISerializable
	{
		private int m_Width;
		private uint m_MaxBrightness = 0;
		private ulong m_MaxBrightnessFirstKey = 0;
		private int m_MaxBrightnessPixels = 0;
		internal int m_Generation = 1;
		private int m_Merges = 1;

		private int m_FeatureId;
		public int FeatureId
		{
			get { return m_FeatureId; }
		}

		internal SortedDictionary<ulong, uint> m_Pixels = new SortedDictionary<ulong, uint>();

		internal void FixFeatureIdAfterSetExcusion(int newId)
		{
			m_FeatureId = newId;
		}

		internal StarMapFeature(int featureId, int width)
		{
			m_FeatureId = featureId;
			m_Width = width;
		}

		public StarMapFeature(SerializationInfo info, StreamingContext context)
		{
			//m_Stride = info.GetInt32("m_Stride");
			//m_BytesPerPixel = info.GetInt32("m_BytesPerPixel");
			m_MaxBrightness = info.GetByte("m_MaxBrightness");

			m_MaxBrightnessFirstKey = info.GetUInt64("m_MaxBrightnessFirstKey");
			m_MaxBrightnessPixels = info.GetInt32("m_MaxBrightnessPixels");
			m_Generation = info.GetInt32("m_Generation");
			m_Merges = info.GetInt32("m_Merges");

			m_FeatureId = info.GetInt32("FeatureId");
			m_Intencity = info.GetUInt32("m_Intencity");

			int count = info.GetInt32("m_Pixels.Count");
			int idx = -1;
			m_Pixels = new SortedDictionary<ulong, uint>();
			for (int i = 0; i < count; i++)
			{
				idx++;
				ulong key = info.GetUInt64(string.Format("m_Pixels.{0}.Key", idx));
				uint val = info.GetUInt32(string.Format("m_Pixels.{0}.Value", idx));
				m_Pixels.Add(key, val);
			}
		}

		internal void AddPixel(int x, int y, uint brightness)
		{
			uint idx = (uint)m_Width * (uint)y + (uint)x;

            if (m_Pixels.ContainsKey(idx))
            {
#if ASTROMETRY_DEBUG
                Trace.Assert(m_Pixels[idx] == brightness);
#endif
            }
            else
            {
                m_Pixels.Add(idx, brightness);

                if (brightness > m_MaxBrightness)
                {
                    m_MaxBrightness = brightness;
                    m_MaxBrightnessFirstKey = idx;
                    m_MaxBrightnessPixels = 1;
                }
                else if (brightness == m_MaxBrightness)
                    m_MaxBrightnessPixels++;
            }

			m_Center = null;
			m_Intencity = UInt32.MaxValue;
		}

		public void MergePixels(StarMapFeature mergeWith)
		{
			bool pixelsMerged = false;

			foreach (ulong idx in mergeWith.m_Pixels.Keys)
			{
				if (!this.m_Pixels.ContainsKey(idx))
				{
					this.m_Pixels.Add(idx, mergeWith.m_Pixels[idx]);
					pixelsMerged = true;
				}
			}

			if (pixelsMerged)
				m_Merges++;

			m_Center = null;
		}

		public IEnumerable<KeyValuePair<int, int>> Pixels
		{
			get
			{
				foreach (ulong key in m_Pixels.Keys)
				{
					int y = (int)(key / (ulong)m_Width);
					int x = (int)(key % (ulong)m_Width);

					yield return new KeyValuePair<int, int>(x, y);
				}
			}
		}

		public int PixelCount
		{
			get
			{
				return m_Pixels.Count;
			}
		}

		public int MaxBrightnessPixels
		{
			get { return m_MaxBrightnessPixels; }
		}

		private uint m_Intencity = UInt32.MaxValue;
		public uint Intensity
		{
			get
			{
				if (m_Intencity == UInt32.MaxValue)
				{
					foreach (ulong key in m_Pixels.Keys)
						m_Intencity += m_Pixels[key];

					m_Intencity = (uint)Math.Round((1.0 * m_Intencity) / m_Pixels.Keys.Count);
				}

				return m_Intencity;
			}
		}

		public uint this[int x, int y]
		{
			get
			{
				ulong key = (ulong)(m_Width * y + x);
				if (m_Pixels.ContainsKey(key))
					return m_Pixels[key];
				else
					return 0;
			}
		}

		public override string ToString()
		{
			StringBuilder output = new StringBuilder();
			output.AppendLine(string.Format("FeatureId: {0}", this.FeatureId));

			foreach (ulong key in m_Pixels.Keys)
			{
				int yp = (int)(key / (ulong)m_Width);
				int xp = (int)(key % (ulong)m_Width);

				output.AppendLine(string.Format("[{0}, {1}] = {2}", xp, yp, m_Pixels[key]));
			}

			return output.ToString();
		}

		private ImagePixel m_Center = null;

		public ImagePixel GetCenter()
		{
			if (m_Center == null)
			{
				if (m_MaxBrightnessPixels == 1)
				{
					int y = (int)(m_MaxBrightnessFirstKey / (ulong)m_Width);
					int x = (int)(m_MaxBrightnessFirstKey % (ulong)m_Width);

					m_Center = ImagePixel.CreateImagePixelWithFeatureId(this.FeatureId, (int)m_MaxBrightness, x, y);
				}
				else
				{
					int minX = int.MaxValue;
					int minY = int.MaxValue;
					int maxX = int.MinValue;
					int maxY = int.MinValue;

					foreach (ulong key in m_Pixels.Keys)
					{
						if (m_Pixels[key] < m_MaxBrightness) continue;

						int y = (int)(key / (ulong)m_Width);
						int x = (int)(key % (ulong)m_Width);

						if (minX > x) minX = x;
						if (minY > y) minY = y;
						if (maxX < x) maxX = x;
						if (maxY < y) maxY = y;
					}

					int midX = (maxX + minX) / 2;
					bool singleMidX = ((maxX + minX) % 2) == 0;
					int midY = (maxY + minY) / 2;
					bool singleMidY = ((maxY + minY) % 2) == 0;

					List<ImagePixel> checkPixels = new List<ImagePixel>();

					if (singleMidX && singleMidY)
					{
						ulong midIdx = (ulong)(m_Width * midY + midX);
						if (!m_Pixels.ContainsKey(midIdx))
						{
							// TODO: Send a warning of irregularly shaped feature
						}

						return ImagePixel.CreateImagePixelWithFeatureId(this.FeatureId, (int)m_MaxBrightness, midX, midY);
					}
					else if (singleMidX)
					{
						// Analyse 2 possible center points
						checkPixels.Add(ImagePixel.CreateImagePixelWithFeatureId(this.FeatureId, (int)m_MaxBrightness, midX, midY));
						checkPixels.Add(ImagePixel.CreateImagePixelWithFeatureId(this.FeatureId, (int)m_MaxBrightness, midX, midY + 1));
					}
					else if (singleMidY)
					{
						// Analyse 2 possible center points
						checkPixels.Add(ImagePixel.CreateImagePixelWithFeatureId(this.FeatureId, (int)m_MaxBrightness, midX, midY));
						checkPixels.Add(ImagePixel.CreateImagePixelWithFeatureId(this.FeatureId, (int)m_MaxBrightness, midX + 1, midY));
					}
					else
					{
						// Analyse 4 possible center points
						checkPixels.Add(ImagePixel.CreateImagePixelWithFeatureId(this.FeatureId, (int)m_MaxBrightness, midX, midY));
						checkPixels.Add(ImagePixel.CreateImagePixelWithFeatureId(this.FeatureId, (int)m_MaxBrightness, midX, midY + 1));
						checkPixels.Add(ImagePixel.CreateImagePixelWithFeatureId(this.FeatureId, (int)m_MaxBrightness, midX + 1, midY));
						checkPixels.Add(ImagePixel.CreateImagePixelWithFeatureId(this.FeatureId, (int)m_MaxBrightness, midX + 1, midY + 1));
					}

					m_Center = null;

					long centerBrightness = 0;

					foreach (ImagePixel pixel in checkPixels)
					{
						long totalBrightness = 0;
						for (int i = -1; i <= 1; i++)
							for (int j = -1; j <= 1; j++)
							{
								totalBrightness += this[pixel.X + i, pixel.Y + j];
							}

						if (centerBrightness < totalBrightness)
						{
							centerBrightness = totalBrightness;
							m_Center = pixel;
						}
					}
				}
			}

			return m_Center;
		}

		public int Generation
		{
			get { return m_Generation; }
		}

		public int Merges
		{
			get { return m_Merges; }
		}

		private static int STREAM_VERSION = 2;

		public void Serialize(StreamWriter wrt)
		{
			wrt.Write(STREAM_VERSION);
			wrt.Write(m_Width);
			wrt.Write(m_MaxBrightness);
			wrt.Write(m_MaxBrightnessFirstKey);
			wrt.Write(m_MaxBrightnessPixels);
			wrt.Write(m_Generation);
			wrt.Write(m_Merges);
			wrt.Write(FeatureId);

			wrt.Write(m_Pixels.Keys.Count);
			foreach (ulong key in m_Pixels.Keys)
			{
				wrt.Write(key);
				wrt.Write(m_Pixels[key]);
			}
		}

		#region ICloneable Members

		public object Clone()
		{
			StarMapFeature clone = new StarMapFeature(FeatureId, m_Width);

			clone.m_MaxBrightness = m_MaxBrightness;
			clone.m_MaxBrightnessFirstKey = m_MaxBrightnessFirstKey;
			clone.m_MaxBrightnessPixels = m_MaxBrightnessPixels;
			clone.m_Generation = m_Generation;
			clone.m_Merges = m_Merges;

			foreach (ulong key in m_Pixels.Keys) clone.m_Pixels.Add(key, m_Pixels[key]);

			return clone;
		}

		#endregion

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("m_Width", m_Width);
			info.AddValue("m_MaxBrightness", m_MaxBrightness);
			info.AddValue("m_MaxBrightnessFirstKey", m_MaxBrightnessFirstKey);
			info.AddValue("m_MaxBrightnessPixels", m_MaxBrightnessPixels);
			info.AddValue("m_Generation", m_Generation);
			info.AddValue("m_Merges", m_Merges);
			info.AddValue("FeatureId", FeatureId);
			info.AddValue("m_Intencity", m_Intencity);

			info.AddValue("m_Pixels.Count", m_Pixels.Keys.Count);
			int idx = -1;
			foreach (ulong key in m_Pixels.Keys)
			{
				idx++;
				info.AddValue(string.Format("m_Pixels.{0}.Key", idx), key);
				info.AddValue(string.Format("m_Pixels.{0}.Value", idx), m_Pixels[key]);
			}
		}

		#endregion

		public static StarMapFeature FromReflectedObject(object refl)
		{
			int featureId = StarMap.GetPropValue<int>(refl, "FeatureId");
			int width = StarMap.GetPropValue<int>(refl, "Width");

			StarMapFeature feature = new StarMapFeature(featureId, width);

			feature.m_MaxBrightness = StarMap.GetPropValue<byte>(refl, "m_MaxBrightness");
			feature.m_MaxBrightnessFirstKey = StarMap.GetPropValue<ulong>(refl, "m_MaxBrightnessFirstKey");
			feature.m_MaxBrightnessPixels = StarMap.GetPropValue<int>(refl, "m_MaxBrightnessPixels");
			feature.m_Generation = StarMap.GetPropValue<int>(refl, "m_Generation");
			feature.m_Merges = StarMap.GetPropValue<int>(refl, "m_Merges");
			feature.m_Intencity = StarMap.GetPropValue<uint>(refl, "m_Intencity");

			feature.m_Pixels = StarMap.GetPropValue<SortedDictionary<ulong, uint>>(refl, "m_Pixels");

			return feature;
		}
	}
}
