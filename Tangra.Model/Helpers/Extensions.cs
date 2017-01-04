using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Tangra.Model.Video;

namespace Tangra.Model.Helpers
{
    public static class Extensions
    {
        public static string GetFullStackTrace(this Exception ex)
        {
            var output = new StringBuilder();
            Exception currEx = ex;
            while (currEx != null)
            {
                output.AppendFormat("{0} : {1}\r\n{2}\r\n-------------------------------------\r\n\r\n", currEx.GetType(), currEx.Message, currEx.StackTrace);
                currEx = currEx.InnerException;
            }

            return output.ToString();
        }

        public static string AsBase64String<T>(this T obj)
        {
            var ser = new XmlSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                ser.Serialize(stream, obj);
                stream.Position = 0;

                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static T FromBase64String<T>(this string data)
        {
            var ser = new XmlSerializer(typeof(T));
            using (var stream = new MemoryStream(Convert.FromBase64String(data)))
            {
                stream.Position = 0;
                T obj = (T)ser.Deserialize(stream);
                return obj;
            }
        }

        public static string AsBase64String(this uint[,] pixels)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                int width = pixels.GetLength(0);
                int height = pixels.GetLength(1);
                writer.Write(width);
                writer.Write(height);
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                    {
                        writer.Write(pixels[i, j]);
                    }

                writer.Flush();
                stream.Position = 0;

                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static uint[,] FromBase64String(this string data)
        {
            using (var stream = new MemoryStream(Convert.FromBase64String(data)))
            using (var reader = new BinaryReader(stream))
            {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();

                uint[,] rv = new uint[width, height];

                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                    {
                        rv[i, j] = reader.ReadUInt32();
                    }

                return rv;
            }
        }

		public static uint GetMaxValueForBitPix(this int bitPix)
		{
			if (bitPix == 8)
				return byte.MaxValue;
			else if (bitPix == 12)
				return 4095;
			else if (bitPix == 14)
				return 16383;
			else if (bitPix == 16)
				return ushort.MaxValue;
			else
				return uint.MaxValue;
		}

		public static T SortAndGetMedian<T>(this List<T> data)
		{
			if (data.Count == 0) return default(T);

			data.Sort();

			return data[data.Count / 2];
		}

        public static T Median<T>(this IList<T> list)
        {
            if (list.Count == 0) return default(T);
            if (list.Count == 1) return list[0];

            T[] copy = list.ToArray();
            Array.Sort(copy);

            return copy[copy.Length / 2];
        }

        public static bool TraceError(this TraceLevel traceLevel)
        {
            return (int)traceLevel >= 1;
        }

        public static bool TraceWarning(this TraceLevel traceLevel)
        {
            return (int)traceLevel >= 2;
        }

        public static bool TraceInfo(this TraceLevel traceLevel)
        {
            return (int) traceLevel >= 3;
        }

        public static bool TraceVerbose(this TraceLevel traceLevel)
        {
            return (int)traceLevel == 4;
        }

        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static bool IsAAV(this VideoFileFormat format)
        {
            return 
                format == VideoFileFormat.AAV || 
                format == VideoFileFormat.AAV2;
        }
    }
}
