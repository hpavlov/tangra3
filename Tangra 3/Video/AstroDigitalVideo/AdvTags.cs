using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Video.AstroDigitalVideo
{
	public static class AdvKeywords
	{
		// Saved as binary data and made available via the keyword values
		public static string KEY_BITPIX = "BITPIX"; // Bits per pixel 
		public static string KEY_WIDTH = "WIDTH";
		public static string KEY_HEIGHT = "HEIGHT";

		// Saved at the end as part of the real dictionary
		public static string KEY_DATE = "DATE"; // Date the file was created
		public static string KEY_DATE_OBS = "DATE-OBS"; // Date of data acquisition
		public static string KEY_DATETIME_OBS_START = "UT-OBS-BEGING";
		public static string KEY_DATETIME_OBS_END = "UT-OBS-END";

		public static string KEY_DATAFRAME_COMPRESSION = "SECTION-DATA-COMPRESSION";
		public static string KEY_DIFFCORR_KEY_FRAME_FREQUENCY = "DIFFCODE-KEY-FRAME-FREQUENCY";
		public static string KEY_DATA_LAYOUT = "DATA-LAYOUT";
		public static string KEY_IMAGE_BYTE_ORDER = "IMAGE-BYTE-ORDER";

		public static string KEY_INSTRUMENT = "INSTRUME";
		public static string KEY_TELESCOPE = "TELESCOPE";
		public static string KEY_OBSERVER = "OBSERVER";
		public static string KEY_COMMENT = "COMMENT";
	    public static string KEY_RECORDER = "RECORDER";
		public static string KEY_LONGITUDE = "LONGITUDE";
		public static string KEY_LATITUDE = "LATITUDE";
	}

	public static class DataLayouts
	{
		public static string FULL_IMAGE_RAW = "FULL-IMAGE-RAW";
		public static string FULL_IMAGE_DIFFERENTIAL_CODING = "FULL-IMAGE-DIFFERENTIAL-CODING";
	}

	public static class AdvSectionTypes
	{
		public static string SECTION_IMAGE = "IMAGE";
		public static string SECTION_SYSTEM_STATUS = "STATUS";
	}

	public static class AdvCompressionMethods
	{
		public static string COMPR_UNCOMPRESSED = "UNCOMPRESSED";
		public static string COMPR_DIFF_CORR_HUFFMAN = "HUFFMAN";
		public static string COMPR_DIFF_CORR_QUICKLZ = "QUICKLZ";
	}

	public static class AdvRedundancyCheck
	{
		public static string CRC32 = "CRC32";
	}

	/*	 Astro Digital Video (.adv) File Format
	 *  
	 *    |  4 bytes |  1 byte |  4 bytes    |        8 bytes       |        8 bytes          |   1 byte           |
	 *    |   MAGIC  | VERSION |  NUM FRAMES | INDEX TABLE OFFSET   | METADATA TABLE OFFSET   |  SECTIONS COUNT    |
	 * 
	 *    |  IMAGE SECTION DEFINITION | SECTION DEFINITION 2 | ... |  SECTION DEFINITION N |
	 *    
	 *    |  SECTION HEADER 1 | SECTION HEADER 2 | ... |  SECTION HEADER N |
	 * 
	 *    | DATAFRAME 1 | DATAFRAME 2 | DATAFRAME 3 | ... | DATAFRAME Z | 
	 * 
	 *    |  INDEX TABLE | METADATA TABLE |
	 *    
	 *  -------------------------------------------------------------------------------------------------------------
	 *  
	 *   SECTION DEFINITION
	 *   
	 *   |     STR1-255   |    4 bytes      |
	 *   |  SECTION TYPE  | HEADER OFFSET   |
	 *   
	 * --------------------------------------------------------------------------------------------------------------
	 * 
	 *  SECTION HEADER - The format depends on the section type
	 *  
	 *  'IMAGE' Section header
	 *      
	 *   |  36 bytes  | 1 byte  |  2 bytes |  2 bytes |   1 byte  |
	 *   |  READER ID | VERSION |  WIDTH   |  HEIGHT  |  BITPIX   |
	 *   
	 * 'STATUS' Section header
	 * 
	 *   |  36 bytes  | 1 byte  |  1 byte       |     STR1-255   |   1 byte       | .. |    STR1-255   |   1 byte        |
	 *   |  READER ID | VERSION |  PARAMS COUNT |  PARAM NAME 1  |  PARAM TYPE 1  | .. |  PARAM NAME M  |  PARAM TYPE M  |
	 * 
	 *  The following Param Types are defined: UInt16 = 0,  UInt32 = 1, ULong64 = 2, AnsiString256 = 3
	 * 
	 * -----------------------------------------------------------------------------------------------------------------
	 * 
	 *  INDEX TABLE
	 * 
	 *  |      8 bytes        |    4 bytes       |    4 bytes       |
	 *  |  DATAFRAME OFFSET   | DATAFRAME LENGTH | ELAPSED TIME MS  |
	 *  
	 *  METADATA TABLE
	 *  
	 *  |  2 bytes     |  STR1-255     |  STR1-255      | .. |  STR1-255     |  STR1-255      |
	 *  | NUM ENTRIES  | ENTRY 1 NAME  | ENTRY 1 VALUE  | .. | ENTRY X NAME  | ENTRY X VALUE  |
	 *  
	 * ----------------------------------------------------------------------------------------------------------------
	 * 
	 *  DATAFRAME - For used compression see the metadata (keys DATA-COMPRESSION, COMPRESSION-KEY-FRAMES)
	 *              If there is compression used then the data referenced in the index is first decompressed and then the following format applies:
	 *  
	 *  |      8 bytes     |    4 bytes  |  4 bytes  |                |  4 bytes  |                |    |  4 bytes  |                |
	 *  |  TIMESTAMP UTC   | EXPOSURE MS |  LENGTH 1 | SECTION 1 DATA |  LENGTH 2 | SECTION 2 DATA | .. | LENGTH N  | SECTION N DATA |
	 *  
	 * ----------------------------------------------------------------------------------------------------------------
	 * 
	 * IMAGE DATAFRAME SECTION
	 * 
	 *  |  1 bytes |          |
	 *  |  FLAGS   | RAW DATA |
	 *  
	 * 
	 *  FLAGS = 0 (no key frame used), RAW DATA has the format:
	 *  
	 *  |                |
	 *  | RAW PIXEL DATA |
	 * 
	 *  FLAGS = 1 (key frame used. key frame data follows), RAW DATA has the format:
	 *  
	 *  | 4 bytes |                |
	 *  | MEDIAN  | RAW PIXEL DATA |
	 *
	 *  FLAGS = 2 (key frame used. diff corr data follows), RAW DATA has the format:
	 *  
	 *  |                 |                       |
	 *  | SIGN PIXEL BITS |  DIFF CORR PIXEL DATA |
	 *  
	 * ----------------------------------------------------------------------------------------------------------------
	 * 
	 * STATUS DATAFRAME SECTION - Based on the types defined in the header
	 * 
	 * 
	 */
}
