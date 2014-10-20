/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;

namespace Shared
{
    public class AssemblyHelper
    {
        public static string GetEmbededResource(string nameSpace, string fileNameOnly)
        {
            return GetEmbededResource(Assembly.GetCallingAssembly(), nameSpace, fileNameOnly, Encoding.ASCII);
        }

        public static string GetEmbededResource(string nameSpace, string fileNameOnly, Encoding encoding)
        {
            return GetEmbededResource(Assembly.GetCallingAssembly(), nameSpace, fileNameOnly, encoding);
        }

        public static string GetEmbededResource(string nameSpace, string fileNameOnly, Encoding encoding, Encoding outEncoding)
        {
            return GetEmbededResource(Assembly.GetCallingAssembly(), nameSpace, fileNameOnly, encoding, outEncoding);
        }

        public static Stream GetEmbededResourceStreamThatClientMustDispose(string nameSpace, string fileNameOnly)
        {
            return GetEmbededResourceStreamThatClientMustDispose(Assembly.GetCallingAssembly(), nameSpace, fileNameOnly);
        }

        public static Stream GetEmbededResourceStreamThatClientMustDispose(string nameSpace, string fileNameOnly, Encoding encoding, Encoding outEncoding)
        {
            return GetEmbededResourceStreamThatClientMustDispose(Assembly.GetCallingAssembly(), nameSpace, fileNameOnly, encoding, outEncoding);
        }


        private static string GetEmbededResource(Assembly callingAssembly, string nameSpace, string fileNameOnly, Encoding encoding)
        {
            using (Stream data = GetEmbededResourceStreamThatClientMustDispose(callingAssembly, nameSpace, fileNameOnly))
            {
                byte[] buffer = new byte[data.Length];
                data.Read(buffer, 0, (int)data.Length);
                return encoding.GetString(buffer);
            }
        }

        private static string GetEmbededResource(Assembly callingAssembly, string nameSpace, string fileNameOnly, Encoding encoding, Encoding outEncoding)
        {
            using (Stream outStream = GetEmbededResourceStreamThatClientMustDispose(callingAssembly, nameSpace, fileNameOnly, encoding, outEncoding))
            {
                using (TextReader reader = new StreamReader(outStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static Stream GetEmbededResourceStreamThatClientMustDispose(Assembly callingAssembly, string nameSpace, string fileNameOnly, Encoding encoding, Encoding outEncoding)
        {
            MemoryStream outStream = new MemoryStream();
            
            using (Stream data = GetEmbededResourceStreamThatClientMustDispose(callingAssembly, nameSpace, fileNameOnly))
            {
                const int BUFFER_LEN = 1024;
                StreamWriter wrt = new StreamWriter(outStream, outEncoding);

                using (StreamReader rdr = new StreamReader(data, encoding))
                {
                    int bytesRead = 0;
                    do
                    {
                        char[] buffer = new char[BUFFER_LEN];
                        bytesRead = rdr.Read(buffer, 0, buffer.Length);

                        wrt.Write(buffer, 0, bytesRead);
                    }
                    while (bytesRead == BUFFER_LEN);

                    wrt.Flush();
                    outStream.Flush();
                }
            }

            outStream.Seek(0, SeekOrigin.Begin);
            return outStream;
        }

        private static Stream GetEmbededResourceStreamThatClientMustDispose(Assembly callingAssembly, string nameSpace, string fileNameOnly)
        {
            Stream data = callingAssembly.GetManifestResourceStream(string.Format("{0}.{1}", nameSpace, fileNameOnly));
            
            Stream decompressedData = Decompress(data);
            return decompressedData;
        }

        public static Stream Compress(Stream inputStream)
        {
            MemoryStream compressedStream = new MemoryStream();
            compressedStream.Write(new byte[] { (byte)'O', (byte)'W', (byte)'Z', (byte)'!' }, 0, 4);

            int uncompressedSize = (int)inputStream.Length;

            inputStream.Seek(0, SeekOrigin.Begin);

            byte[] inputReadBuffer = new byte[uncompressedSize];

            using (DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionMode.Compress, true))
            {
                int bytesRead = inputStream.Read(inputReadBuffer, 0, inputReadBuffer.Length);

                deflateStream.Write(inputReadBuffer, 0, bytesRead);
                deflateStream.Flush();
            }

            compressedStream.Flush();
            compressedStream.Seek(0, SeekOrigin.Begin);
            return compressedStream;
        }

        public static Stream Decompress(Stream input)
        {
            input.Position = 0;
            byte[] magic = new byte[4];
            input.Read(magic, 0, 4);

            if (magic[0] == (byte)'O' &&
                magic[1] == (byte)'W' &&
                magic[2] == (byte)'Z' &&
                magic[3] == (byte)'!')
            {
                MemoryStream output = new MemoryStream();

                using (DeflateStream zippedStream = new DeflateStream(input, CompressionMode.Decompress))
                using (BinaryReader reader = new BinaryReader(zippedStream))
                {
                    const int BUFFER_LEN = 1024;

                    // Don't close/dispose the the stream reader because this will close the input stream as well
                    BinaryWriter writer = new BinaryWriter(output);

                    int bytesRead = 0;
                    do
                    {
                        byte[] buffer = new byte[BUFFER_LEN];
                        bytesRead = reader.Read(buffer, 0, buffer.Length);

                        writer.Write(buffer, 0, bytesRead);
                    }
                    while (bytesRead == BUFFER_LEN);

                    writer.Flush();
                }

                output.Seek(0, SeekOrigin.Begin);
                input.Dispose();
                return output;
            }
            else
            {
                input.Position = 0;
                return input;
            }
        }
    }
}
