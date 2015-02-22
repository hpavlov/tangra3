using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tangra.Model.Helpers;

namespace Tangra.Model.Config
{
	public enum ObservedObjectType
	{
		NumberredAsteroid,
	}

    public enum CometMagnitudeType
    {
        Total,
        Nucleus
    }

    public enum MagnitudeBand
    {
        Johnson_B, 
        Johnson_V, 
        Cousins_R, 
        Cousins_I, 
        TwoMicron_J, 
        W, 
        Johnson_U, 
        Sloan_g, 
        Sloan_r, 
        Sloan_i,
        Sloan_z
    }

    // http://www.cfa.harvard.edu/iau/info/Astrometry.html
    // http://www.cfa.harvard.edu/iau/info/ObsDetails.html

	public class MPCObsLine
	{
		//The following examples show valid comet and minor-planet observations. 
		//                                    Column
		//         1         2         3         4         5         6         7         8
		//123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789

		//    CJ93K010  C1995 01 12.44658 23 20 12.59 -73 00 31.9                      413
		//    PJ93X010  C1995 01 13.71552 12 08 44.80 +01 55 10.6                      413
		//    PJ94P01d  C1994 10 14.82517 09 57 25.32 +09 06 28.5          13.3 T      360
		//    PJ95A010  C1995 01 27.42558 07 45 16.64 +21 40 44.4          20.6 N      691
		//0007P         C1995 01 07.49677 10 07 09.83 +31 58 36.9                      691
		//0047P         C1994 12 31.38076 07 40 47.47 +37 40 09.1                      693
		//0116P         C1995 01 03.21177 02 40 39.14 +18 09 23.5          20.8 T      691

		//     J91R04W F 1994 04 03.00278 11 28 41.20 +04 14 24.8                      033
		//     J93P00C cC1994 07 04.57639 17 28 54.97 -38 12 17.1          17.1 V      360
		//     PLS2645   1994 03 04.63681 11 40 57.89 +06 07 08.6                      399
		//     T1S3196   1994 05 04.56403 14 26 06.83 -15 41 20.8          16.6        474
		//     T2S3187  A1973 09 19.21250 00 24 34.59 -01 08 26.0                      675
		//     T3S2318  C1994 07 12.22157 19 47 43.23 -15 45 37.3                      801
		//02965         C1994 07 13.97693 17 35 15.33 -00 26 18.9          16.1 R      046

		//     94ORX0 * C1994 06 08.98877 16 22 02.78 -17 49 13.7          18.5        104

        //     K09SM9C* C2009 09 29.34888 03 07 49.67 +37 22 59.7          19.5   ES143704
        //     K09SM9C  C2009 09 29.36120 03 07 49.09 +37 23 00.9          20.3   ES143704
        //     K09SM9C  C2009 09 29.37366 03 07 48.36 +37 23 01.7          19.9   ES143704
        //     K09SM9C  C2009 09 29.38600 03 07 47.81 +37 23 02.3          19.8   ES143704
        //     K09SM9C  C2009 09 29.39835 03 07 47.21 +37 23 03.0          19.8   ES143704
        //     K09SM9C  C2009 09 29.99654 03 07 21.47 +37 23 26.4          19.7 R ES143473
        //     K09SM9C  C2009 09 30.07835 03 07 17.38 +37 23 29.8                 ES143204
        //     K09SM9C  C2009 09 30.08471 03 07 17.03 +37 23 30.4                 ES143204

        public string m_Designation_1_12;
        public string m_NewDiscoveryFlag_13 = " ";
        public string m_Note1_14 = " ";
        public string m_Note2_15 = "C";
        public string m_TimeString_16_32;
        public string m_RAString_33_44;
        public string m_DEString_45_56;
        public string m_Reserved_57_64 = "         ";
        public string m_Magnitude_65_70 = "     ";
        public string m_MagnitudeBand_71 = " ";
        public string m_Reserved_72_77 = "      ";
	    public string m_ObsCode_78_80 = "   ";

        private MPCObsLine()
        { }

		public MPCObsLine(string obsCode)
		{
            if (obsCode != null)
		        m_ObsCode_78_80 = obsCode;
        }

        public void SetObject(string objectDesignation)
        {
            m_Designation_1_12 = objectDesignation.PadRight(12).Substring(0, 12);
        }

        public void SetMagnitude(double mag, MagnitudeBand magBand)
        {
			m_Magnitude_65_70 = (mag.ToString("0.0", CultureInfo.InvariantCulture).PadLeft(4) + " ").Substring(0, 5);
            switch(magBand)
            {
                case MagnitudeBand.Johnson_U:
                    m_MagnitudeBand_71 = "U";
                    break;

                case MagnitudeBand.Johnson_B:
                    m_MagnitudeBand_71 = "B";
                    break;

                case MagnitudeBand.Johnson_V:
                    m_MagnitudeBand_71 = "V";
                    break;

                case MagnitudeBand.Cousins_R:
                    m_MagnitudeBand_71 = "R";
                    break;

                case MagnitudeBand.Cousins_I:
                    m_MagnitudeBand_71 = "I";
                    break;

                case MagnitudeBand.Sloan_g:
                    m_MagnitudeBand_71 = "g";
                    break;

                case MagnitudeBand.Sloan_r:
                    m_MagnitudeBand_71 = "r";
                    break;

                case MagnitudeBand.Sloan_i:
                    m_MagnitudeBand_71 = "i";
                    break;

                case MagnitudeBand.Sloan_z:
                    m_MagnitudeBand_71 = "z";
                    break;

                case MagnitudeBand.TwoMicron_J:
                    m_MagnitudeBand_71 = "J";
                    break;

                case MagnitudeBand.W:
                    m_MagnitudeBand_71 = "W";
                    break;
            }
        }

        public void SetMagnitudeComet(double mag, CometMagnitudeType magType)
        {
            m_Magnitude_65_70 = mag.ToString("0.0", CultureInfo.InvariantCulture).PadLeft(4) + " ";
            if (magType == CometMagnitudeType.Total) m_MagnitudeBand_71 = "T";
            else if (magType == CometMagnitudeType.Nucleus) m_MagnitudeBand_71 = "N";
        }

        public void SetPosition(double raHours, double deDeg, DateTime utcTime, bool isVideoNormalPosition)
		{
            double roundedTime = (utcTime.Hour + utcTime.Minute / 60.0 + (utcTime.Second + (utcTime.Millisecond / 1000.0))/ 3600.0) / 24;
            string format = "0.000000";

            if (isVideoNormalPosition)
            {
                roundedTime = Math.Truncate(Math.Round(roundedTime * 1000000)) / 1000000;
                format = "0.000000";
            }
            else
            {
                roundedTime = Math.Truncate(Math.Round(roundedTime * 100000))/100000;
                format = "0.00000";
            }

            m_TimeString_16_32 = (utcTime.ToString("yyyy MM ") + (utcTime.Day + roundedTime).ToString(format, CultureInfo.InvariantCulture)).PadRight(17).Substring(0, 17);

            m_RAString_33_44 = AstroConvert.ToStringValue(raHours, "HH MM SS.TT").PadRight(12).Substring(0, 12);
            m_DEString_45_56 = AstroConvert.ToStringValue(deDeg, "+HH MM SS.T").PadRight(12).Substring(0, 12);

            SetVideoNormalPosition(isVideoNormalPosition);
		}

		private void SetVideoNormalPosition(bool isVideoNormalPosition)
		{
			if (isVideoNormalPosition)
				m_Note2_15 = "n"; /* Normal Place */
			else
				m_Note2_15 = "C"; /* CCD */
		}

        public void SetNewDiscoveryFlag()
        {
            m_NewDiscoveryFlag_13 = "*";
        }

        public string BuildObservationASCIILine()
        {
            return m_Designation_1_12 + m_NewDiscoveryFlag_13 + m_Note1_14 + m_Note2_15 + m_TimeString_16_32 + m_RAString_33_44 + m_DEString_45_56 +
                   m_Reserved_57_64 + m_Magnitude_65_70 + m_MagnitudeBand_71 + m_Reserved_72_77 + m_ObsCode_78_80;
        }

		public DateTime GetObservationDateTime()
		{
			string[] tokens = m_TimeString_16_32.Split(' ');
			if (tokens.Length == 3)
			{

				int year, month;
				if (!int.TryParse(tokens[0].Trim(), out year)) return DateTime.MinValue;
				if (!int.TryParse(tokens[1].Trim(), out month)) return DateTime.MinValue;

				double day;
				if (!double.TryParse(tokens[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out day)) return DateTime.MinValue;

				var rv = new DateTime(year, month, (int) day);
				rv.AddDays(day - (int) day);
				return rv;
			}

			return DateTime.MinValue;
		}

		public double GetObservationRAHours()
		{
			try
			{
				return AstroConvert.ToRightAcsension(m_RAString_33_44.Trim());
			}
			catch(Exception)
			{ }

			return double.NaN;
		}

		public double GetObservationDEDeg()
		{
			try
			{
				return AstroConvert.ToDeclination(m_DEString_45_56.Trim());
			}
			catch (Exception)
			{ }

			return double.NaN;
		}

		public double GetObservationMag()
		{
			try
			{
				return double.Parse(m_Magnitude_65_70.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{ }

			return double.NaN;
		}

        public static MPCObsLine Parse(string asciiLine)
        {
            if (asciiLine.Length != 80) return null;

            MPCObsLine instance = new MPCObsLine();
            instance.m_Designation_1_12 = asciiLine.Substring(0, 12);
            instance.m_NewDiscoveryFlag_13 = asciiLine.Substring(12, 1);
            instance.m_Note1_14 = asciiLine.Substring(13, 1);
            instance.m_Note2_15 = asciiLine.Substring(14, 1);
            instance.m_TimeString_16_32 = asciiLine.Substring(15, 32 - 16 + 1);
            instance.m_RAString_33_44 = asciiLine.Substring(32, 44 - 33 + 1);
            instance.m_DEString_45_56 = asciiLine.Substring(44, 56 - 45 + 1);
            instance.m_Reserved_57_64 = asciiLine.Substring(56, 64 - 57 + 1);
            instance.m_Magnitude_65_70 = asciiLine.Substring(64, 70 - 65 + 1);
            instance.m_MagnitudeBand_71 = asciiLine.Substring(70, 1);
            instance.m_Reserved_72_77 = asciiLine.Substring(71, 77 - 72 + 1);
            instance.m_ObsCode_78_80 = asciiLine.Substring(77, 3);

            return instance;
        }

		private static bool IsHalfMonthLetter(char c)
		{
			return "ABCDEFGHIJKLMNOPQRSTUVWX".IndexOf(c) > -1;
		}

		private static bool IsLetter(char c)
		{
			return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(c) > -1;
		}

		private static bool IsFragmentLetter(char c)
		{
			return "0abcdefghijklmnopqrstuvwxyz".IndexOf(c) > -1;
		}

		private static bool IsDigit(char c)
		{
			return "1234567890".IndexOf(c) > -1;
		}

		private static Regex s_NewDesignRegEx = new Regex("^[\\d\\w]{3,6}$");

		public static string GetObjectCode(string objectDesignation)
		{
			string parsedDesignation;
			return GetObjectCode(objectDesignation, out parsedDesignation);
		}

		public static string GetObjectCode(string objectDesignation, out string parsedDesignation)
		{
			parsedDesignation = objectDesignation;
			if (string.IsNullOrEmpty(objectDesignation)) return null;

			string objDes = objectDesignation.Replace("(", "").Replace(")", "").Trim();

			if (objDes.Length == 0) return null;

			string[] tokens = objDes.Split(new char[] { ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);
			int firstTokenAsNumber = 0, secondTokenAsNumber = 0, thirdTokenAsNumber = 0;
			int.TryParse(tokens[0], out firstTokenAsNumber);
			if (tokens.Length > 1) int.TryParse(tokens[1], out secondTokenAsNumber);
			if (tokens.Length > 2) int.TryParse(tokens[2], out thirdTokenAsNumber);

			int number = firstTokenAsNumber;
			int desigNumber;
			char cometChar = ' ';
			string halfMonthToken = tokens.Length > 1 ? tokens[1] : string.Empty;
			if (tokens.Length > 2 &&
				tokens[0].Length == 1 &&
				"PCDXA".IndexOf(tokens[0][0]) > -1)
			{
				// The first token is a cometary orbit type
				number = secondTokenAsNumber;
				halfMonthToken = tokens[2];
				cometChar = tokens[0][0];
			}

			if (number == 0 &&
				tokens[0].Length > 1 &&
				"PD".IndexOf(tokens[0][tokens[0].Length - 1]) > -1)
			{
				// If the last char is 'P' or 'D', this is a numberred comet
				char lastCharFirstToken = tokens[0][tokens[0].Length - 1];
				string partBefore = tokens[0].Substring(0, tokens[0].Length - 1);
				int cometNumber;
				int.TryParse(partBefore, out cometNumber);
				if (cometNumber > 0)
				{
					return string.Format("{0}{1}", cometNumber.ToString("000#"), lastCharFirstToken).PadRight(13);
				}
			}
			else if (number > 0 && (number < 1900 || number > 2100))
			{
				// This looks like a minor planet number
				parsedDesignation = firstTokenAsNumber.ToString();
				return GetPackedAsteroidCode(firstTokenAsNumber);
			}
			else if (tokens.Length > 1 &&
				number >= 1900 && number <= 2100 &&
				halfMonthToken.Length > 1)
			{
				if (IsHalfMonthLetter(halfMonthToken[0]))
				{
					string[] fragmentTokens = halfMonthToken.Split(new char[] { '-' }, 2);
					string fragmentLetter = "0";
					if (fragmentTokens.Length == 2 &&
						fragmentTokens[1].Length == 1 &&
						IsHalfMonthLetter(fragmentTokens[1][0]))
					{
						fragmentLetter = fragmentTokens[1].ToLower();
						halfMonthToken = fragmentTokens[0];
					}

					int indexInHalfMonth = 0;
					int.TryParse(halfMonthToken.Substring(1), out indexInHalfMonth);

					if (indexInHalfMonth > 0 && indexInHalfMonth <= 99)
					{
						// This looks like 
						char centeryChar = 'K';
						if (number < 2000) centeryChar = 'J';
						if (number >= 2100) centeryChar = 'L';						

						return string.Format("    {0}{1}{2}{3}{4}{5} ",
							cometChar,
							centeryChar,
							(number % 100).ToString("0#"),
							halfMonthToken[0],
							indexInHalfMonth.ToString("0#"),
							fragmentLetter);
					}
					else
					{
						//2010 UK
						//2010 TN167
						//1998 TU3
						if (halfMonthToken.Length >= 2 && 
 							halfMonthToken.Length <= 5 &&
							IsLetter(halfMonthToken[0]) &&
							IsLetter(halfMonthToken[1]))
						{
							desigNumber = 0;
							if (halfMonthToken.Length == 2 ||
								int.TryParse(halfMonthToken.Substring(2), out desigNumber))
							{
								char centeryChar = 'K';
								if (number < 2000) centeryChar = 'J';
								if (number >= 2100) centeryChar = 'L';

								return string.Format("     {0}{1}{2}{3}{4} ",
								centeryChar,
								(number % 100).ToString("0#"),
								halfMonthToken[0],
								GetPackedDesignationCode(desigNumber),
								halfMonthToken[1]);
							}
						}
					}
				}
			}

			if (firstTokenAsNumber > 0)
			{
				parsedDesignation = firstTokenAsNumber.ToString();
				return GetPackedAsteroidCode(firstTokenAsNumber);
			}

			if ((tokens[0].Length == 7 && "JKL".IndexOf(tokens[0][0]) > -1) ||
				(tokens[0].Length == 8 && "CP".IndexOf(tokens[0][0]) > -1 && "JKL".IndexOf(tokens[0][1]) > -1))
			{
				string parseString = "CP".IndexOf(tokens[0][0]) > -1 ? tokens[0].Substring(1) : tokens[0];

				// Check if the designation is already a valid packed provisional destination of an object
				// J95A010
				//CJ94P01b
				// K10R16F

				string yearChars = parseString.Substring(1, 2);
				char letter1 = parseString[3];
				char char1 = parseString[4];
				char digit1 = parseString[5];
				char letter2 = parseString[6];

				if (int.TryParse(yearChars, out number) &&
					IsLetter(letter1) &&
					(IsLetter(char1) || IsDigit(char1)) &&
					IsDigit(digit1) &&
					(IsLetter(letter2) || IsFragmentLetter(letter2) || letter2 == '0'))
				{
					parsedDesignation = "CP".IndexOf(tokens[0][0]) > -1 ? tokens[0][0] + "/" : "";
					switch (parseString[0])
					{
						case 'J':
							parsedDesignation += "19";
							break;
						case 'K':
							parsedDesignation += "20";
							break;
						case 'L':
							parsedDesignation += "21";
							break;
					}
					parsedDesignation += number.ToString("00") + " ";
					parsedDesignation += "" + letter1;
					if (letter2 != '0') parsedDesignation += "" + letter2;
					parsedDesignation += GetNumberForPackedDesignationCode(char1, digit1);

					if ("CP".IndexOf(tokens[0][0]) > -1)
						return string.Format("    {0} ", tokens[0]);
					else
						return string.Format("     {0} ", tokens[0]);
				}
			}
			else if (
				tokens.Length == 1 &&
				tokens[0].Length == 5 &&
				IsLetter(tokens[0][0]) &&
				int.TryParse(tokens[0].Substring(1), out desigNumber))
			{
				// This is a packed number asteroid designation 
				parsedDesignation = GetNumberForPackedAsteroidCode(tokens[0]);
				return tokens[0].PadRight(13);
			}
			else if (
				tokens.Length == 1)
			{
			    string token = tokens[0].Trim();

                if (s_NewDesignRegEx.IsMatch(token))
                {
                    // Observer-assigned temporary designations should be unique--don't call everything `X'! 
                    // Observer-assigned temporary designations should be six characters or less long, and begin in column 6 of the observational record. 
                    // Observer-assigned temporary designations must not be of the form of the packed or unpacked designations used by the MPC. Note that this now includes the form single letter followed by four digits. 
                    // Do not continue to use your observer-assigned designations once official provisional or permanent designations have been assigned. 

                    // User designation should contain letters and numbers only				
                    return string.Format("     {0}  ", token.PadRight(6));
                }
			}

			return null;
		}

		private static string GetPackedAsteroidCode(int mpNumber)
		{
			//NUMBER 
			//Columns 1-5 contain a zero-padded, right-justified number--e.g., an observation of (1) would be given as 00001, an observation of (3202) would be 03202. If there is no number these columns must be blank. Six-digit numbers are to be stored in packed form (A0000 = 100000), in order to be consistent with the format specifier earlier in this document. 
			//PROVISIONAL/TEMPORARY DESIGNATION 
			//Columns 6-12 contain the provisional designation or the temporary designation. The provisional designation is stored in a 7-character packed form.
			//Temporary designations are designations assigned by the observer for new or unidentified objects. Such designations must begin in column 6, should not exceed 6 characters in length, and should start with one or more letters. 

			if (mpNumber > 0 && mpNumber < 100000)
				return mpNumber.ToString("0000#").PadRight(13);
			else if (mpNumber >= 100000 && mpNumber < 1000000)
			{
				int hdrsTsnds = (mpNumber / 10000) - 10;
				int reminder = mpNumber % 10000;

				if (hdrsTsnds < 26)
					return string.Format("{0}{1}", "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[hdrsTsnds], reminder.ToString("000#")).PadRight(13);
				else
					return "??????".PadRight(13);
			}

			return null;
		}

		private static string GetNumberForPackedAsteroidCode(string code)
		{
			if (IsLetter(code[0]))
			{
				int hdrsTsnds = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(code[0]) + 10;
				return (10000 * hdrsTsnds + int.Parse(code.Substring(1).Trim())).ToString();
			}
			else
				return int.Parse(code.Trim()).ToString();
		}

		private static string GetPackedDesignationCode(int number)
		{
			int decimals = number / 10;
			char char1 = decimals < 36 ? "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"[decimals] : '!';

			return char1 + (number % 10).ToString();
		}

		private static string GetNumberForPackedDesignationCode(char char1, char char2)
		{
			int num = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(char1) * 10 + int.Parse(char2 + "");
			if (num == 0)
				return string.Empty;
			else
				return num.ToString();
		}
	}

    // http://www.cfa.harvard.edu/iau/info/OpticalObs.html

	//Columns     Format   Use
	//14            A1     Note 1
	//15            A1     Note 2
	//16 - 32              Date of observation
	//33 - 44              Observed RA (J2000.0)
	//45 - 56              Observed Decl. (J2000.0)
	//57 - 65       9X     Must be blank
	//66 - 71    F5.3,A1   Observed magnitude and band
	//                        (or nuclear/total flag for comets)
	//72 - 77       X      Must be blank
	//78 - 80       A3     Observatory code


    // http://www.cfa.harvard.edu/iau/info/ObsNote.html

	//-NOTES (Column 14)
	//A  earlier approximate position inferior
	//a  sense of motion ambiguous
	//B  bright sky/black or dark plate
	//b  bad seeing
	//C  correction to earlier position (do not use on newly-submitted observations)
	//c  crowded star field
	//D  declination uncertain
	//d  diffuse image
	//E  at or near edge of plate
	//F  faint image
	//f  involved with emulsion or plate flaw
	//G  poor guiding
	//g  no guiding
	//H  hand measurement of CCD image
	//I  involved with star
	//i  inkdot measured
	//J  J2000.0 rereduction of previously-reported position
	//K  stacked image
	//k  stare-mode observation by scanning system
	//M  measurement difficult
	//m  image tracked on object motion
	//N  near edge of plate, measurement uncertain
	//O  image out of focus
	//o  plate measured in one direction only
	//P  position uncertain
	//p  poor image
	//R  right ascension uncertain
	//r  poor distribution of reference stars
	//S  poor sky
	//s  streaked image
	//T  time uncertain
	//t  trailed image
	//U  uncertain image
	//u  unconfirmed image
	//V  very faint image
	//W  weak image
	//w  weak solution

	// NOTES: Column 15
	// P   Photographic (default if column is blank)
	// e   Encoder
	// C   CCD
	// T   Meridian or transit circle
	// M   Micrometer
	//V/v  "Roving Observer" observation
	//R/r  Radar observation
	//S/s  Satellite observation
	// c   Corrected-without-republication CCD observation
	// E   Occultation-derived observations
	// O   Offset observations (used only for observations of natural satellites)
	// H   Hipparcos geocentric observations
	// N   Normal place
	// n   Mini-normal place derived from averaging observations from video frames

	//DETAILED NOTES:

	//MINOR PLANETS

	//NUMBER 
	//Columns 1-5 contain a zero-padded, right-justified number--e.g., an observation of (1) would be given as 00001, an observation of (3202) would be 03202. If there is no number these columns must be blank. Six-digit numbers are to be stored in packed form (A0000 = 100000), in order to be consistent with the format specifier earlier in this document. 
	//PROVISIONAL/TEMPORARY DESIGNATION 
	//Columns 6-12 contain the provisional designation or the temporary designation. The provisional designation is stored in a 7-character packed form.
	//Temporary designations are designations assigned by the observer for new or unidentified objects. Such designations must begin in column 6, should not exceed 6 characters in length, and should start with one or more letters. 

	//It is important that every observation has a designation and that the same designation is used for all observations of the same object. 

	//DISCOVERY ASTERISK 
	//Discovery observations for new (or unidentified) objects should contain `*' in column 13. Only one asterisked observation per object is expected. 

	//COMETS

	//PERIODIC COMET NUMBER 
	//Periodic comets that have been observed at more than one return are assigned numbers. Reference should be made to the editorial notices on MPC 23803-23804 and 24421 for more complete details of the circumstances under which numbers are assigned. 
	//Examples: 

	//      Comet                  P/ Number    Columns 1-4
	//                                          will contain
	//      P/Halley                  1P          0001
	//      P/Encke                   2P          0002
	//      P/Biela                   3D          0003
	//      P/Wild 4                116P          0116

	//See the complete list of periodic comet numbers. 
	//ORBIT TYPE 
	//Column 5 contains `C' for a long-period comet, `P' for a short-period comet, `D' for a `defunct' comet, `X' for an uncertain comet or `A' for a minor planet given a cometary designation. 
	//PROVISIONAL DESIGNATION 
	//Columns 6-12 contain a packed version of the provisional designation. The first two digits of the year are packed into a single character in column 6 (I = 18, J = 19, K = 20). Columns 7-8 contain the last two digits of the year. Column 9 contains the half-month letter. Columns 10-11 contain the order within the half-month. Column 12 will be normally be `0', except for split comets, when the fragment designation is stored there as a lower-case letter. 
	//   Examples:
	//   1995 A1   = J95A010
	//   1994 P1-B = J94P01b   refers to fragment B of 1994 P1
	//   1994 P1   = J94P010   refers to the whole comet 1994 P1

	//Columns 6-12 may contain a minor-planet provisional designation. In such a situation column 12 will contain a capital letter. 

	//NATURAL SATELLITES

	//PLANET IDENTIFIER 
	//A single character to represent the planet that the satellites belongs to. 
	//   Char   Planet
	//     J    Jupiter
	//     S    Saturn
	//     U    Uranus
	//     N    Neptune

	//This is given only for those objects with Roman numeral designations. 
	//SATELLITE NUMBER 
	//For those objects with Roman numeral designations, columns 2-4 contain the number of the satellite. 
	//COLUMN 5 
	//Column 5 is always "S" for a satellite observation. 
	//PROVISIONAL DESIGNATION 
	//Columns 6-12 contain a packed version of the provisional designation for those objects without Roman numeral designations.
	//The first two digits of the year are packed into a single character in column 6 (I = 18, J = 19, K = 20). Columns 7-8 contain the last two digits of the year. Column 9 contains the half-month letter. Columns 10-11 contain the order within the half-month. Column 12 will be always be `0'. This is similar to the scheme used for comets. 

	//   Examples
	//   123456789012
	//   J013S         Jupiter XIII
	//   N002S         Neptune II
	//       SJ99U030  S/1999 U 3    (Third new Uranian satellite discovered in 1999)
	//       SK20J010  S/2020 J 1    (First new Jovian satellite discovered in 2020)


	//COMETS, MINOR PLANETS AND NATURAL SATELLITES
	//NOTE 1 
	//This column contains a alphabetical publishable note or a numeric or non-alphanumeric character program code. The list of standard codes used for observations of minor planets is given in each batch of MPCs. 
	//NOTE 2 
	//This column serves two purposes. For those observations which have been converted to the J2000.0 system by rotating B1950.0 coordinates this column contains `A', to indicate that the value has been adjusted. For those observations reduced in the J2000.0 system this column is used to indicate how the observation was made. The following codes will be used: 
	//      P   Photographic (default if column is blank)
	//      e   Encoder
	//      C   CCD
	//      T   Meridian or transit circle
	//      M   Micrometer
	//     V/v  "Roving Observer" observation
	//     R/r  Radar observation
	//     S/s  Satellite observation
	//      c   Corrected-without-republication CCD observation
	//      E   Occultation-derived observations
	//      O   Offset observations (used only for observations of natural satellites)
	//      H   Hipparcos geocentric observations
	//      N   Normal place
	//      n   Mini-normal place derived from averaging observations from video frames

	//In addition, there is 'X' which is used only for already-filed observations. It was given originally only to discovery observations that were approximate or semi-accurate and that had accurate measures corresponding to the time of discovery: this has been extended to other replaced discovery observations. Observations marked 'X' are to be suppressed in residual blocks. They are retained so that there exists an original record of a discovery. 
	//DATE OF OBSERVATIONS 
	//Columns 16-32 contain the date and UTC time of the mid-point of observation. If the observation refers to one end of a trailed image, then the time of observation will be either the start time of the exposure or the finish time of the exposure. The format is "YYYY MM DD.dddddd", with the decimal day of observation normally being given to a precision of 0.00001 days. Where such precision is justified, there is the option of recording times to 0.000001 days. 
	//OBSERVED RA (J2000.0) 
	//Columns 33-44 contain the observed J2000.0 right ascension. The format is "HH MM SS.ddd", with the seconds of R.A. normally being given to a precision of 0.01s. There is the option of recording the right ascension to 0.001s, where such precision is justified. 
	//OBSERVED DECL (J2000.0) 
	//Columns 45-56 contain the observed J2000.0 declination. The format is "sDD MM SS.dd" (with "s" being the sign), with the seconds of Decl. normally being given to a precision of 0.1". There is the option of recording the declination to 0".01, where such precision is justified. 
	//OBSERVED MAGNITUDE AND BAND 
	//The observed magnitude (normally to a precision of 0.1 mag.) and the band in which the measurement was made. The observed magnitude can be given to 0.01 mag., where such precision is justified. The default magnitude scale is photographic, although magnitudes may be given in V- or R-band, for example. For comets, the magnitude must be specified as being nuclear, N, or total, T. 
	//The current list of acceptable magnitude bands is: B (default if band is not indicated), V, R, I, J, C, W, U, g, r, i and z. Non-recognized magnitude bands will cause observations to be rejected. Addition of new recognised bands requires knowledge of a standard correction to convert a magnitude in that band to V. 

	//OBSERVATORY CODE 
	//Observatory codes are stored in columns 78-80. Lists of observatory codes are published from time to time in the MPCs. Note that new observatory codes are assigned only upon receipt of acceptable astrometric observations. 
}
