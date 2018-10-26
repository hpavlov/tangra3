using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Video;

namespace Tangra.Automation.IntegrationDetection
{
    public class PixDetFile : IImagePixelProvider, IDisposable
    {
        private bool m_IsWrite;
        private string m_FileName;

        private string m_OriginalVideoFile;
        private int m_TotalFrames;
        private int m_FirstFrameId;
        private long m_FrameDataStartPosition;

        private int m_Interval;
        private int m_StartingAtFrame;
        private double m_Certainty;
        private double m_AboveSigmaRatio;

        private FileStream m_File;
        private BinaryWriter m_Writer;
        private BinaryReader m_Reader;

        private const int MAGIC = 0x7A1169A0;

        private static byte CURRENT_VERSION = 1;

        private static int CURRENT_PIX_FRAME_SIZE = 32 * 32 + 4;

        public PixDetFile(string fileName, FileAccess fileAccess)
        {
            m_FileName = fileName;

            if (fileAccess == FileAccess.Read)
            {
                m_IsWrite = false;
                m_File = new FileStream(m_FileName, FileMode.Open, FileAccess.Read);
                m_Reader = new BinaryReader(m_File);
                var magic = m_Reader.ReadInt32();
                if (magic != MAGIC)
                {
                    throw new InvalidOperationException("Not a valid PixDet file");
                }
                var version = m_Reader.ReadByte();
                if (version > CURRENT_VERSION)
                {
                    throw new InvalidOperationException("Don't know how to open newer versions of PixDet files.");
                }
                ReadHeader();
                ReadFooter();
            }
            else if (fileAccess == FileAccess.Write)
            {
                m_IsWrite = true;
                m_File = new FileStream(m_FileName, FileMode.Create, FileAccess.Write);
                m_Writer = new BinaryWriter(m_File);
                m_Writer.Write(MAGIC);
                m_Writer.Write(CURRENT_VERSION);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private void ReadHeader()
        {
            m_TotalFrames = m_Reader.ReadInt32();
            m_OriginalVideoFile = m_Reader.ReadString();
            m_FirstFrameId = m_Reader.ReadInt32();
            m_FrameDataStartPosition = m_Reader.ReadInt32();
        }

        private void ReadFooter()
        {
            long footerStartPos = m_FrameDataStartPosition + CURRENT_PIX_FRAME_SIZE * m_TotalFrames;
            m_File.Seek(footerStartPos, SeekOrigin.Begin);

            m_Interval= m_Reader.ReadInt32();
            m_StartingAtFrame= m_Reader.ReadInt32();
            m_Certainty = m_Reader.ReadDouble();
            m_AboveSigmaRatio = m_Reader.ReadDouble();
            m_TotalFrames = m_Reader.ReadInt32();
        }

        public void WriteHeader(string originalFile, int firstFrameId)
        {
            if (!m_IsWrite) throw new InvalidOperationException("Must be in write mode to write data.");

            m_TotalFrames = 0;
            m_OriginalVideoFile = originalFile;
            m_FirstFrameId = firstFrameId;

            m_Writer.Write(m_TotalFrames);
            m_Writer.Write(m_OriginalVideoFile);
            m_Writer.Write(m_FirstFrameId);

            m_FrameDataStartPosition = m_File.Position + sizeof(long);
            m_Writer.Write(m_FrameDataStartPosition);
        }

        public void WriteFooter(int interval, int startingAtFrame, double certainty, double aboveSigmaRatio)
        {
            if (!m_IsWrite) throw new InvalidOperationException("Must be in write mode to write data.");

            m_Interval = interval;
            m_StartingAtFrame = startingAtFrame;
            m_Certainty = certainty;
            m_AboveSigmaRatio = aboveSigmaRatio;

            m_Writer.Write(m_Interval);
            m_Writer.Write(m_StartingAtFrame);
            m_Writer.Write(m_Certainty);
            m_Writer.Write(m_AboveSigmaRatio);
            m_Writer.Write(m_TotalFrames);
        }

        public void AddFramePixels(int frameId, int[] pixels)
        {
            if (!m_IsWrite) throw new InvalidOperationException("Must be in write mode to write data.");
            if (pixels.Length != 32 * 32)
            {
                throw new InvalidOperationException("Pixel array much be 32x32 pixels");
            }

            m_Writer.Write(frameId);
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    var btData = (byte)pixels[x + 32 * y];
                    m_Writer.Write(btData);
                }
            }

            m_TotalFrames++;
        }

        public void AddFramePixels(int frameId, int[,] pixels)
        {
            if (!m_IsWrite) throw new InvalidOperationException("Must be in write mode to write data.");

            if (pixels.GetLength(0) != 32 || pixels.GetLength(1) != 32)
            {
                throw new InvalidOperationException("Pixel array much be 32x32 pixels");
            }

            m_Writer.Write(frameId);
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    var btData = (byte) pixels[x, y];
                    m_Writer.Write(btData);
                }
            }

            m_TotalFrames++;
        }

        public void Dispose()
        {
            if (m_Reader != null) m_Reader.Dispose();
            if (m_Writer != null) m_Writer.Dispose();
            if (m_File != null) m_File.Dispose();
        }

        public int Width 
        {
            get { return 32; }
        }

        public int Height
        {
            get { return 32; }
        }

        public int FirstFrame 
        {
            get
            {
                return m_FirstFrameId;
            }
        }
        
        public int LastFrame
        {
            get
            {
                return m_FirstFrameId + m_TotalFrames - 1;
            }
        }

        public int[,] GetPixelArray(int frameNo, Rectangle rect)
        {
            if (m_IsWrite) throw new InvalidOperationException("Cannot read in write mode.");

            long framePixPos = m_FrameDataStartPosition + CURRENT_PIX_FRAME_SIZE * (frameNo - m_FirstFrameId);
            m_File.Seek(framePixPos, SeekOrigin.Begin);

            var savedFrameNo = m_Reader.ReadInt32();
            if (savedFrameNo != frameNo) throw new InvalidOperationException(string.Format("Expected frame {0} but found frame {1} at this position.", frameNo, savedFrameNo));

            var pixels = new int[32, 32];
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    pixels[x, y] = (int)m_Reader.ReadByte();
                }
            }
            return pixels;
        }
    }
}
