using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Tangra.Model.Helpers
{
    public static class Extensions
    {
        public static string FullExceptionInfo(this Exception ex)
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
    }
}
