using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Permissions;

namespace AutoUpdate
{
    /// <summary>
    /// ZIP File Header:
    ///     http://www.ta7.de/txt/computer/computer016.htm
    /// 
    /// Python script to zip/unzip files:
    ///     http://svn.python.org/projects/python/trunk/Lib/zipfile.py
    /// </summary>
    public class ZipUnzip
    {
        private const int FILE_HEADER_MAGIC = 0x04034B50;
        private const int CENTRAL_DIR_MAGIC = 0x02014B50;
        private const int END_OF_ARCHIVE_MAGIC = 0x06054B50;
        private const int STRUCT_64BIT_LOCATOR_MAGIC = 0x06074B50;
        private const int STRUCT_64BIT_END_OF_ARCHIVE_MAGIC = 0x06064B50;

        private const short ZIP_COMPRESSION_STORED = 0;
        private const short ZIP_COMPRESSION_DEFLATED = 8;

        private const short DEFALT_FILE_VERSION = 20;
        private const short DEFALT_DIR_VERSION = 10;
        private const short WIN_SHELL_ZIP_VERSION = 0xB14;

        public static void UnZip(Stream inputZipStream, string outputDirectory, bool restoreDirectoryStructure)
        {
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            int nextHeaderPosition = 0;

            using (BinaryReader headerReader = new BinaryReader(inputZipStream))
            {
                while (nextHeaderPosition < inputZipStream.Length)
                {
                    // After reading the same base stream with different stream readers including the DeflateStream
                    // the possition was not correct (assuming due to the fact when you read from the DeflateStream
                    // you specify the uncompressed bytes to read, not the compressed bytes) so we have to keep track of 
                    // the current position in the zipped stream in a different way, thats the nextHeaderPosition number
                    // Before we move to the next header move to the right position.
                    inputZipStream.Seek(nextHeaderPosition, SeekOrigin.Begin);

                    // Reading the header magic
                    // NOTE: The nextHeaderPosition for the magic is included in the Process...() function 
                    int magic = headerReader.ReadInt32();

                    if (magic == FILE_HEADER_MAGIC)
                    {
                        // File Header found. Unzip the file
                        nextHeaderPosition += ProcessFileHeader(inputZipStream, headerReader, outputDirectory, restoreDirectoryStructure);
                    }
                    else if (magic == CENTRAL_DIR_MAGIC)
                    {
                        // File Header found. Unzip the file
                        nextHeaderPosition += ProcessCentralDirHeader(inputZipStream, headerReader, outputDirectory);
                    }
                    else if (magic == END_OF_ARCHIVE_MAGIC)
                    {
                        // File Header found. Unzip the file
                        nextHeaderPosition += ProcessEndOfZipHeader(inputZipStream, headerReader, outputDirectory);
                    }
                    else if (magic == STRUCT_64BIT_LOCATOR_MAGIC || magic == STRUCT_64BIT_END_OF_ARCHIVE_MAGIC)
                    {
                        throw new NotSupportedException("64-bit zip files are not supported.");
                    }
                    else
                        throw new BadImageFormatException("Invalid or corrupted ZIP file data or unrecognized ZIP header.");
                }
            }
        }

        public static void UnZip(string zipFile, string outputDirectory, bool restoreDirectoryStructure)
        {
            outputDirectory = Path.GetFullPath(outputDirectory);
            zipFile = Path.GetFullPath(zipFile);

            if (!File.Exists(zipFile))
                throw new FileNotFoundException(zipFile);

            FileIOPermission readPerm = new FileIOPermission(FileIOPermissionAccess.Read, zipFile);
            readPerm.Demand();

            using (FileStream zippedFile = new FileStream(zipFile, FileMode.Open))
            {
                UnZip(zippedFile, outputDirectory, restoreDirectoryStructure);
            }
        }


        public static void Zip(string inputFile, string outputFile)
        {
            BinaryWriter centralDirWriter;
            FileStream outputStream;
            BinaryWriter outputWriter;

            string relativeDir = Path.GetDirectoryName(inputFile);

            StartZip(outputFile, out centralDirWriter, out outputStream, out outputWriter);
            try
            {
                AddFileToZip(inputFile, relativeDir, false, outputStream, outputWriter, centralDirWriter);

                EndZip(1, outputStream, outputWriter, centralDirWriter.BaseStream);
            }
            finally
            {
                outputStream.Flush();
                outputStream.Close();

                centralDirWriter.Close();
            }
        }


        public static void Zip(string inputDirectory, string outputFile, bool includeDirectoryStructure)
        {
            if (!Directory.Exists(inputDirectory))
                throw new DirectoryNotFoundException(inputDirectory);

            List<string> allFiles = new List<string>();

            if (includeDirectoryStructure)
                GetFilesRecursive(inputDirectory, ref allFiles);
            else
                allFiles.AddRange(Directory.GetFiles(inputDirectory));

            string relativeDir = Directory.GetParent(inputDirectory).ToString();

            BinaryWriter centralDirWriter;
            FileStream outputStream;
            BinaryWriter outputWriter;

            StartZip(outputFile, out centralDirWriter, out outputStream, out outputWriter);
            try
            {
                foreach (string inputFile in allFiles)
                {
                    AddFileToZip(inputFile, relativeDir, includeDirectoryStructure, outputStream, outputWriter, centralDirWriter);
                }

                EndZip((short)allFiles.Count, outputStream, outputWriter, centralDirWriter.BaseStream);
            }
            finally
            {
                outputStream.Flush();
                outputStream.Close();

                centralDirWriter.Close();
            }
        }


        private static void GetFilesRecursive(string dirName, ref List<string> fileList)
        {
            fileList.AddRange(Directory.GetFiles(dirName));
            foreach (string subDir in Directory.GetDirectories(dirName))
            {
                fileList.Add(dirName);
                GetFilesRecursive(subDir, ref fileList);
            }
        }

        private static void StartZip(
            string outputFile,
            out BinaryWriter centralDirWriter, 
            out FileStream outputStream, 
            out BinaryWriter outputWriter)
        {
            outputFile = Path.GetFullPath(outputFile);

            string outputDir = Path.GetDirectoryName(outputFile);
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            MemoryStream centralDirStream = new MemoryStream();
            centralDirWriter = new BinaryWriter(centralDirStream);

            FileIOPermission writePerm = new FileIOPermission(FileIOPermissionAccess.AllAccess, outputFile);
            writePerm.Demand();

            outputStream = new FileStream(outputFile, FileMode.Create);
            outputWriter = new BinaryWriter(outputStream);
        }

        private static void AddFileToZip(
            string fileName, 
            string relativeDir,
            bool keepDirectoryInfo,
            FileStream outputStream,
            BinaryWriter outputWriter,
            BinaryWriter centralDirWriter)
        {
            fileName = Path.GetFullPath(fileName);

            if (!File.Exists(fileName) && !Directory.Exists(fileName))
                throw new FileNotFoundException(fileName);

            bool isDir = Directory.Exists(fileName);

            int uncompressedSize = 0;
            int compressedSize = 0;
            UInt32 crc32 = 0;
            int relativePositionOfThisHeader = 0;

            if (isDir)
            {
                // Write the file header
                AppendFileHeader(outputWriter, fileName, 0, 0, 0, relativeDir, keepDirectoryInfo);
            }
            else
            {
                byte[] inputReadBuffer = null;
                using (MemoryStream compressedStream = new MemoryStream())
                {
                    FileIOPermission readPerm = new FileIOPermission(FileIOPermissionAccess.Read, fileName);
                    readPerm.Demand();

                    using (FileStream inputStream = new FileStream(fileName, FileMode.Open))
                    {
                        uncompressedSize = (int)inputStream.Length;

                        // Calculate the CRC32
                        CRC32 crc = new CRC32();
                        crc32 = crc.GetCrc32(inputStream);

                        inputStream.Seek(0, SeekOrigin.Begin);

                        inputReadBuffer = new byte[uncompressedSize];

                        // Compress the individual file into a temporary stream
                        using (DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionMode.Compress, true))
                        {
                            int bytesRead = inputStream.Read(inputReadBuffer, 0, inputReadBuffer.Length);
                            Debug.Assert(bytesRead == uncompressedSize);

                            deflateStream.Write(inputReadBuffer, 0, inputReadBuffer.Length);
                            deflateStream.Flush();
                        }
                    }

                    compressedStream.Flush();

                    compressedStream.Seek(0, SeekOrigin.Begin);
                    compressedSize = (int)compressedStream.Length;

                    relativePositionOfThisHeader = (int)outputStream.Position;

                    // Write the file header
                    AppendFileHeader(outputWriter, fileName, compressedSize, uncompressedSize, crc32, relativeDir, keepDirectoryInfo);

                    // Write the compressed data
                    byte[] buffer = new byte[compressedStream.Length];
                    compressedStream.Read(buffer, 0, buffer.Length);

#if DEBUG
                    using (MemoryStream memStr = new MemoryStream())
                    {
                        memStr.Write(buffer, 0, buffer.Length);
                        memStr.Flush();
                        memStr.Seek(0, SeekOrigin.Begin);

                        byte[] allContent = null;

                        using (DeflateStream testUnZip = new DeflateStream(memStr, CompressionMode.Decompress))
                        using (BinaryReader testRdr = new BinaryReader(testUnZip))
                        {

                            allContent = testRdr.ReadBytes(uncompressedSize);
                            Debug.Assert(allContent.Length == uncompressedSize);
                        }

                        for(int i = 0; i < uncompressedSize; i++)
                            Debug.Assert(allContent[i] == inputReadBuffer[i]);
                    }
#endif                    

                    // Add the compressed data to the output stream
                    outputStream.Write(buffer, 0, buffer.Length);
                }
            }

            
            // Write the central dir header
            AppendCentralDirHeader(centralDirWriter, fileName, compressedSize, uncompressedSize, crc32, relativePositionOfThisHeader, relativeDir, keepDirectoryInfo);
        }

        private static void EndZip(
            short numberOfFiles,
            FileStream outputStream,
            BinaryWriter outputWriter,
            Stream centralDirStream)
        {
            // Write the central dir to the main output stream
            centralDirStream.Seek(0, SeekOrigin.Begin);
            int centralDirLen = (int)centralDirStream.Length;
            int sentralDirOffset = (int)outputStream.Position;

            byte[] dirBuff = new byte[centralDirLen];
            centralDirStream.Read(dirBuff, 0, dirBuff.Length);
            outputStream.Write(dirBuff, 0, dirBuff.Length);

            // Write the end of archive entry
            AppendEndOfArchiveEntry(outputWriter, numberOfFiles, centralDirLen, sentralDirOffset);

            outputStream.Flush();
        }

        private static void AppendFileHeader(
            BinaryWriter writer,
            string fileName,
            int compressedSize,
            int uncompressedSize,
            UInt32 crc32,
            string relativeDir,
            bool keepDirectoryInfo)
        {
            bool isFile = false;
            if (File.Exists(fileName))
                isFile = true;
            else if (!Directory.Exists(fileName))
                throw new FileNotFoundException(fileName);

            writer.Write(FILE_HEADER_MAGIC);

            if (isFile)
                writer.Write(DEFALT_FILE_VERSION);
            else
                writer.Write(DEFALT_DIR_VERSION);

            writer.Write((short)0); // Flags

            if (isFile)
                writer.Write(ZIP_COMPRESSION_DEFLATED);
            else
                writer.Write(ZIP_COMPRESSION_STORED);

            DateTime dt = DateTime.Now.ToUniversalTime();

            if (isFile)
                dt = File.GetLastWriteTimeUtc(fileName);
            else
                dt = Directory.GetLastWriteTimeUtc(fileName);

            // dosdate = (dt[0] - 1980) << 9 | dt[1] << 5 | dt[2]
            // dostime = dt[3] << 11 | dt[4] << 5 | (dt[5] // 2)
            UInt16 dosDate = Convert.ToUInt16(((dt.Year - 1980) << 9) | (dt.Month << 5) | dt.Day);
            UInt16 dosTime = Convert.ToUInt16((dt.Hour << 11) | (dt.Minute << 5) | (dt.Second / 2));

            writer.Write(dosTime);
            writer.Write(dosDate);

            writer.Write(crc32);
            writer.Write(compressedSize);
            writer.Write(uncompressedSize);

            if (keepDirectoryInfo)
                fileName = fileName.Replace(relativeDir, "");
            else
                fileName = Path.GetFileName(fileName);

            fileName = fileName.Trim('\\').Replace("\\", "/");
            if (!isFile)
                // Trailing "/" for directories is necessary
                fileName = fileName + "/";

            short fileNameLen = (short)fileName.Length;
            writer.Write(fileNameLen); // File name length
            writer.Write((short)0); // Extra file header


            writer.Write(Encoding.ASCII.GetBytes(fileName));
        }



        private static void AppendCentralDirHeader(
            BinaryWriter writer,
            string fileName,
            int compressedSize,
            int uncompressedSize,
            UInt32 crc32,
            int relativePositionOfThisHeader,
            string relativeDir,
            bool keepDirectoryInfo)
        {
            bool isFile = false;
            if (File.Exists(fileName))
                isFile = true;
            else if (!Directory.Exists(fileName))
                throw new FileNotFoundException(fileName);


            writer.Write(CENTRAL_DIR_MAGIC);
            writer.Write(WIN_SHELL_ZIP_VERSION); // Pretend we are the Windows Shell ZIP 

            if (isFile)
                writer.Write(DEFALT_FILE_VERSION);
            else
                writer.Write(DEFALT_DIR_VERSION);

            writer.Write((short)0); // flags

            if (isFile)
                writer.Write(ZIP_COMPRESSION_DEFLATED);
            else
                writer.Write(ZIP_COMPRESSION_STORED);

            DateTime dt = DateTime.Now.ToUniversalTime();

            if (isFile)
                dt = File.GetLastWriteTimeUtc(fileName);
            else
                dt = Directory.GetLastWriteTimeUtc(fileName);

            // dosdate = (dt[0] - 1980) << 9 | dt[1] << 5 | dt[2]
            // dostime = dt[3] << 11 | dt[4] << 5 | (dt[5] // 2)
            UInt16 dosDate = Convert.ToUInt16(((dt.Year - 1980) << 9) | (dt.Month << 5) | dt.Day);
            UInt16 dosTime = Convert.ToUInt16((dt.Hour << 11) | (dt.Minute << 5) | (dt.Second / 2));

            writer.Write(dosTime);
            writer.Write(dosDate);

            writer.Write(crc32);
            writer.Write(compressedSize);
            writer.Write(uncompressedSize);

            string zipFileName = null;

            if (keepDirectoryInfo)
                zipFileName = fileName.Replace(relativeDir, "");
            else
                zipFileName = Path.GetFileName(fileName);

            zipFileName = zipFileName.Trim('\\').Replace("\\", "/");
            if (!isFile)
                // Trailing "/" for directories is necessary
                zipFileName = zipFileName + "/";

            short fileNameLen = (short)zipFileName.Length;
            writer.Write(fileNameLen);

            writer.Write((short)0); // Extra header
            writer.Write((short)0); // Comments length
            
            writer.Write((short)0); // Disk number start
            // TODO: Check the internal file attributes ???
            writer.Write((short)1); // Internal file attributes

            // This will also work for directories
            FileAttributes fileAtt = File.GetAttributes(fileName);
            writer.Write((int)fileAtt); // External file attributes

            writer.Write((int)relativePositionOfThisHeader); // Relative offset

            writer.Write(Encoding.ASCII.GetBytes(zipFileName));
        }


        private static void AppendEndOfArchiveEntry(BinaryWriter writer, short numberOfFiles, int sizeOfCentralDir, int offsetOfCentralDir)
        {
            writer.Write(END_OF_ARCHIVE_MAGIC);

            writer.Write((short)0); // Number of disks
            writer.Write((short)0); // Start of central dir on this disk

            writer.Write(numberOfFiles); // On this disk
            writer.Write(numberOfFiles); // In total

            writer.Write(sizeOfCentralDir);
            writer.Write(offsetOfCentralDir);

            writer.Write((short)0); // Zip file comment
        }


        /// <summary>
        /// 
        /// Local file header:
        /// ------------------      
        ///  local file header signature     4 bytes  (0x04034b50)
        ///  version needed to extract       2 bytes
        ///  general purpose bit flag        2 bytes
        ///  compression method              2 bytes
        ///  last mod file time              2 bytes
        ///  last mod file date              2 bytes
        ///  crc-32                          4 bytes
        ///  compressed size                 4 bytes
        ///  uncompressed size               4 bytes
        ///  filename length                 2 bytes
        ///  extra field length              2 bytes
        ///  
        ///  filename (variable size)
        ///  extra field (variable size)
        ///  
        /// Data descriptor:
        /// ----------------
        ///  crc-32                          4 bytes
        ///  compressed size                 4 bytes
        ///  uncompressed size               4 bytes
        /// 
        ///  This descriptor exists only if bit 3 of the general
        ///  purpose bit flag is set (see below).  It is byte aligned
        ///  and immediately follows the last byte of compressed data.
        /// 
        /// </summary>
        /// <param name="zippedFile"></param>
        /// <param name="headerReader"></param>
        private static int ProcessFileHeader(
            Stream zippedFile, 
            BinaryReader headerReader, 
            string outputFolder, 
            bool keepDirectoryStructure)
        {
            // This is the standard header size including the magic readin the main sub
            int totalBytesRead = 30;

            UInt16 version = headerReader.ReadUInt16();
            UInt16 flags = headerReader.ReadUInt16();
            UInt16 compressionMethod = headerReader.ReadUInt16();
            UInt16 lastModTime = headerReader.ReadUInt16();
            UInt16 lastModDate = headerReader.ReadUInt16();

            // dosdate = (dt[0] - 1980) << 9 | dt[1] << 5 | dt[2]
            // dostime = dt[3] << 11 | dt[4] << 5 | (dt[5] // 2)
            Debug.Assert(((lastModDate >> 5) & 0xF) > 0);
            Debug.Assert(((lastModDate >> 5) & 0xF) < 13);
            Debug.Assert((lastModDate & 0x1F) > 0);
            Debug.Assert((lastModDate & 0x1F) < 32);
            Debug.Assert(((lastModTime >> 11) & 0x1F) >= 0);
            Debug.Assert(((lastModTime >> 11) & 0x1F) <= 24);
            Debug.Assert(((lastModTime >> 5) & 0x3F) >= 0);
            Debug.Assert(((lastModTime >> 5) & 0x3F) <= 60);
            Debug.Assert(((lastModTime & 0x1F) * 2) >= 0);
            Debug.Assert(((lastModTime & 0x1F) * 2) <= 60);
            DateTime lastModified = new DateTime((lastModDate >> 9) + 1980, (lastModDate >> 5) & 0xF, lastModDate & 0x1F, (lastModTime >> 11) & 0x1F, (lastModTime >> 5) & 0x3F, (lastModTime & 0x1F) * 2);

            int crc32 = headerReader.ReadInt32();
            int compressedSize = headerReader.ReadInt32();
            int uncompressedSize = headerReader.ReadInt32();

            UInt16 fileNameLen = headerReader.ReadUInt16();
            UInt16 extraHeaderLen = headerReader.ReadUInt16();

            // Read file name
            string fileName = Encoding.ASCII.GetString(headerReader.ReadBytes(fileNameLen));
            totalBytesRead += fileNameLen;

            if (keepDirectoryStructure)
            {
                fileName = Path.GetFullPath(outputFolder + "\\" + fileName);
                string dirName = Path.GetDirectoryName(fileName);

                if (!Directory.Exists(dirName))
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }
            else
            {
                fileName = Path.GetFullPath(outputFolder + "\\" + Path.GetFileName(fileName));
            }

            // Skip the extra header (not sure what it means and what its used for. Assuming password protection)
            if (extraHeaderLen > 0)
            {
                totalBytesRead += extraHeaderLen;
                headerReader.ReadBytes(extraHeaderLen);
            }

            // When zipping with direcotries, the directories are also included as entries with zero size
            if (compressedSize > 0)
            {
                // Don't close/dispose stream because this will close the input stream as well
                Stream zippedStream = null;
                if (compressionMethod == ZIP_COMPRESSION_DEFLATED)
                {
                    zippedStream = new DeflateStream(zippedFile, CompressionMode.Decompress);
                }
                else if (compressionMethod == ZIP_COMPRESSION_STORED)
                {
                    // No compression used, the file is just stored. Read directly from the input stream
                    zippedStream = zippedFile;
                }
                else
                    throw new NotSupportedException("This compression method is not supported.");


                FileIOPermission writePerm = new FileIOPermission(FileIOPermissionAccess.AllAccess, fileName);
                writePerm.Demand();

                using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                {
                    // Don't close/dispose the the stream reader because this will close the input stream as well
                    BinaryReader reader = new BinaryReader(zippedStream);

                    using (BinaryWriter writer = new BinaryWriter(fileStream))
                    {
                        byte[] buffer = new byte[uncompressedSize];

                        buffer = reader.ReadBytes(buffer.Length);
                        writer.Write(buffer);

                        writer.Flush();
                    }

                    totalBytesRead += compressedSize;

                    if ((flags & 4) != 0)
                    {
                        // Read the data descriptor chunk after the compressed data
                        // If bit 3 of the general purpose flags is set
                        headerReader.ReadBytes(12);
                        totalBytesRead += 12;
                    }
                }
            }

            return totalBytesRead;
        }

        /// <summary>
        /// Central directory structure
        /// ---------------------------
        ///         central file header signature   4 bytes  (0x02014b50)
        ///         version made by                 2 bytes
        ///         version needed to extract       2 bytes
        ///         general purpose bit flag        2 bytes
        ///         compression method              2 bytes
        ///         last mod file time              2 bytes
        ///         last mod file date              2 bytes
        ///         crc-32                          4 bytes
        ///         compressed size                 4 bytes
        ///         uncompressed size               4 bytes
        ///         filename length                 2 bytes
        ///         extra field length              2 bytes
        ///         file comment length             2 bytes
        ///         disk number start               2 bytes
        ///         internal file attributes        2 bytes
        ///         external file attributes        4 bytes
        ///         relative offset of local header 4 bytes
        ///         
        ///         filename (variable size)
        ///         extra field (variable size)
        ///         file comment (variable size)
        ///         
        /// </summary>
        /// <param name="zippedFile"></param>
        /// <param name="headerReader"></param>
        /// <param name="outputFolder"></param>
        /// <returns></returns>
        private static int ProcessCentralDirHeader(Stream zippedFile, BinaryReader headerReader, string outputFolder)
        {
            // This is the standard header size including the magic readin the main sub
            int totalBytesRead = 46;
            
            // skip the first bytes
            UInt16 versionMadeBy = headerReader.ReadUInt16();
            UInt16 versionNeededToExtract = headerReader.ReadUInt16();
            UInt16 flags = headerReader.ReadUInt16();
            UInt16 compressionMethod = headerReader.ReadUInt16();
            int datetime = headerReader.ReadInt32();
            int crc32 = headerReader.ReadInt32();
            int compressedSize = headerReader.ReadInt32();
            int uncompressedSize = headerReader.ReadInt32();

            UInt16 fileNameLen = headerReader.ReadUInt16();
            UInt16 extraFieldLen = headerReader.ReadUInt16();
            UInt16 commentLen = headerReader.ReadUInt16();

            // skip some more rubish
            UInt16 diskNumberStart = headerReader.ReadUInt16();
            UInt16 internalFileAtt = headerReader.ReadUInt16();
            int externalFileAtt = headerReader.ReadInt32();
            int relativeOffsetOfLocalHeader = headerReader.ReadInt32();

            headerReader.ReadBytes(fileNameLen);
            totalBytesRead += fileNameLen;

            headerReader.ReadBytes(extraFieldLen);
            totalBytesRead += extraFieldLen;

            headerReader.ReadBytes(commentLen);
            totalBytesRead += commentLen;

            return totalBytesRead;
        }

        /// <summary>
        /// End of central dir record
        /// -------------------------
        ///         end of central dir signature    4 bytes  (0x06054b50)
        ///         number of this disk             2 bytes
        ///         number of the disk with the
        ///         start of the central directory  2 bytes
        ///         total number of entries in
        ///         the central dir on this disk    2 bytes
        ///         total number of entries in
        ///         the central dir                 2 bytes
        ///         size of the central directory   4 bytes
        ///         offset of start of central
        ///         directory with respect to
        ///         the starting disk number        4 bytes
        ///         zipfile comment length          2 bytes
        ///         zipfile comment (variable size)
        /// </summary>
        /// <param name="zippedFile"></param>
        /// <param name="headerReader"></param>
        /// <param name="outputFolder"></param>
        /// <returns></returns>
        private static int ProcessEndOfZipHeader(Stream zippedFile, BinaryReader headerReader, string outputFolder)
        {
            // This is the standard header size including the magic readin the main sub
            int totalBytesRead = 22;

            UInt16 numberOfDisks = headerReader.ReadUInt16();
            UInt16 startOfCentralDir = headerReader.ReadUInt16();
            UInt16 numberOfEntriesInCentralDir = headerReader.ReadUInt16();
            UInt16 totalNumberOfEntriesInCentralDir = headerReader.ReadUInt16();
            int sizeOfCentralDir = headerReader.ReadInt32();
            int centralDirOffsetFromFirstDisk = headerReader.ReadInt32();

            UInt16 zipCommentLen = headerReader.ReadUInt16();

            headerReader.ReadBytes(zipCommentLen);
            totalBytesRead += zipCommentLen;

            return totalBytesRead;
        }

    }

    /// <summary>
    /// Calculates a 32bit Cyclic Redundancy Checksum (CRC) using the
    /// same polynomial used by Zip.
    /// </summary>
    public class CRC32
    {
        private UInt32[] crc32Table;
        private const int BUFFER_SIZE = 8192;

        private Int32 _TotalBytesRead = 0;
        public Int32 TotalBytesRead
        {
            get
            {
                return _TotalBytesRead;
            }
        }

        /// <summary>
        /// Returns the CRC32 for the specified stream.
        /// </summary>
        /// <param name="input">The stream over which to calculate the CRC32</param>
        /// <returns>the CRC32 calculation</returns>
        public UInt32 GetCrc32(System.IO.Stream input)
        {
            return GetCrc32AndCopy(input, null);
        }

        /// <summary>
        /// Returns the CRC32 for the specified stream, and writes the input into the output stream.
        /// </summary>
        /// <param name="input">The stream over which to calculate the CRC32</param>
        /// <param name="output">The stream into which to deflate the input</param>
        /// <returns>the CRC32 calculation</returns>
        public UInt32 GetCrc32AndCopy(System.IO.Stream input, System.IO.Stream output)
        {
            unchecked
            {
                UInt32 crc32Result;
                crc32Result = 0xFFFFFFFF;
                byte[] buffer = new byte[BUFFER_SIZE];
                int readSize = BUFFER_SIZE;

                _TotalBytesRead = 0;
                int count = input.Read(buffer, 0, readSize);
                if (output != null) output.Write(buffer, 0, count);
                _TotalBytesRead += count;
                while (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        crc32Result = ((crc32Result) >> 8) ^ crc32Table[(buffer[i]) ^ ((crc32Result) & 0x000000FF)];
                    }
                    count = input.Read(buffer, 0, readSize);
                    if (output != null) output.Write(buffer, 0, count);
                    _TotalBytesRead += count;

                }

                return ~crc32Result;
            }
        }


        /// <summary>
        /// Construct an instance of the CRC32 class, pre-initialising the table
        /// for speed of lookup.
        /// </summary>
        public CRC32()
        {
            unchecked
            {
                // This is the official polynomial used by CRC32 in PKZip.
                // Often the polynomial is shown reversed as 0x04C11DB7.
                UInt32 dwPolynomial = 0xEDB88320;
                UInt32 i, j;

                crc32Table = new UInt32[256];

                UInt32 dwCrc;
                for (i = 0; i < 256; i++)
                {
                    dwCrc = i;
                    for (j = 8; j > 0; j--)
                    {
                        if ((dwCrc & 1) == 1)
                        {
                            dwCrc = (dwCrc >> 1) ^ dwPolynomial;
                        }
                        else
                        {
                            dwCrc >>= 1;
                        }
                    }
                    crc32Table[i] = dwCrc;
                }
            }
        }
    }   
}
