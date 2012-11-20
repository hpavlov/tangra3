#include "Tangra.Orc.IotaVti.h"
#include "stdlib.h"
#include <map>
#include <vector>
#include <algorithm>
#include <math.h>

using namespace std;


SymbolEntry::SymbolEntry(char symbol, char* config)
{
	OcrConfig = new char[10 * 10 + 1];
	OcrConfig[10 * 10] = '\0';

	Cells = new bool[10 * 10 + 1];
	Cells[10 * 10] = 0;

	Symbol = symbol;
	LoadSymbolOCRConfig(config);
}


SymbolEntry::~SymbolEntry()
{
	if (OcrConfig != NULL)
	{
		delete OcrConfig;
		OcrConfig = NULL;
	}

	if (Cells != NULL)
	{
		delete Cells;
		Cells = NULL;
	}
}


void SymbolEntry::LoadSymbolOCRConfig(char* config)
{
	int tokenX, tokenY;
	char tokenChar;
	int tokenCounter = 0;

	char buffer[10];
	char* prtBuffer = &buffer[0];

	while(*config)
	{
		if (*config == (int)',')
		{
			*prtBuffer = '\0';

			switch(tokenCounter % 3)
			{
				case 0:
					tokenX = atoi(buffer);
					break;

				case 1:
					tokenY = atoi(buffer);
					break;

				case 2:
					tokenChar = buffer[0];
					OcrConfig[(10 * tokenY) + tokenX] = char(tokenChar);
					break;
			};

			prtBuffer = &buffer[0];
			tokenCounter++;
		}
		else
		{
			*prtBuffer = *config;
			prtBuffer++;
		}

		config++;
	};
}

void IotaVtiOrcEngine::SetImage(int width, int height, unsigned long* pixels)
{
	m_MaxX = 0;
	m_MaxY = 0;
	m_Maxh = 0;
	m_Maxv = 0;

	m_Width = width;
	m_Height = height;

	if (NULL != m_Pixels)
	{
		delete m_Pixels;
		m_Pixels = NULL;
	}
	m_Pixels = new unsigned long[m_Width * m_Height + 1];

	vector<unsigned long> lstPix;

	unsigned long maxPixel = 0;

	for (int y = 0; y < m_Height; y++)
	{
		for (int x = 0; x < m_Width; x++)
		{
			unsigned int pixel = pixels[x + y * m_Width]; 
			lstPix.push_back(pixel);
			m_Pixels[x + y * m_Width] = pixel;
			if (maxPixel < pixel) maxPixel = pixel;
		}
	}

	int n = lstPix.size() / 2;
	nth_element(lstPix.begin(), lstPix.begin() + n, lstPix.end());
	
	m_ImageMedian = lstPix[n];
	m_Image95Max = (long)(0.95 * maxPixel);

	m_ImageOrcTolerance = TOLERANCE_COEFF * (m_Image95Max - m_ImageMedian);	
}

void IotaVtiOrcEngine::Calibrate()
{
	CalibratePosition();
	CalibrateScale();
}

inline int roundDbl(double x)
{
	return int(x > 0.0 ? x + 0.5 : x - 0.5);
}

void IotaVtiOrcEngine::CalibratePosition()
{
	m_MinVDis = roundDbl(MIN_COL_DIST_COEFF * m_Height);
	m_MaxVDis = roundDbl(MAX_COL_DIST_COEFF * m_Height);

	m_MinHDis = roundDbl(MIN_HORIS_DIST_COEFF * m_Width);
	m_MaxHDis = roundDbl(MAX_HORIS_DIST_COEFF * m_Width);

	int rectLeft = roundDbl(RECT_FROM_X_COEFF * m_Width);
	int rectTop = roundDbl(RECT_FROM_Y_COEFF * m_Height);
	int rectRight = roundDbl(RECT_TO_X_COEFF * m_Width);
	int rectBottom = roundDbl(RECT_TO_Y_COEFF * m_Height);

	m_MaxX = 0;
	m_MaxY = 0;
	m_Maxh = 0;
	m_Maxv = 0;

	float maxVal = -1;

	//int dbgInfoLen = (m_MaxHDis - m_MinHDis) * (m_MaxVDis - m_MinVDis) * (rectRight - rectLeft + 2) * (rectBottom - rectTop) * 30 + 2;

	//char* dbgInfo = (char*)malloc(dbgInfoLen);
	//char buff[30];
	
	//char* dbgInfoFill = dbgInfo;


	for (int h = m_MinHDis; h <= m_MaxHDis; h++)
	{
		for (int v = m_MinVDis; v <= m_MaxVDis; v++)
		{
			for (int x = rectLeft; x <= rectRight; x++)
			{
				for (int y = rectTop; y <= rectBottom; y++)
				{
					int p1x = x; int p1y = y;
					int p2x = x; int p2y = y + v;
					int p3x = x + h; int p3y = y;
					int p4x = x + h; int p4y = y + v;

					float val = -123456.7890;

					if ((p3x < m_Width - 8) && (p4y < m_Height - 8))
					{
						val  = 
							m_Pixels[p1x + m_Width * p1y]+
							m_Pixels[p2x + m_Width * p2y]+
							m_Pixels[p3x + m_Width * p3y]+
							m_Pixels[p4x + m_Width * p4y];

						float avrg = val / 4.0f;

						for (int i = p1y; i < p2y; i++)
							val += (avrg - m_Pixels[p1x + m_Width * i]);

						for (int i = p3y; i < p4y; i++)
							val += (avrg - m_Pixels[p3x + m_Width * i]);

						val +=
							m_Pixels[p1x - 2 + m_Width * (p1y - 2)] +
							m_Pixels[p1x - 2 + m_Width * p1y] +
							m_Pixels[p1x + m_Width * (p1y - 2)] +
							m_Pixels[p1x + 2 + m_Width * (p1y - 2)] +
							m_Pixels[p1x + 2 + m_Width * p1y] +
							m_Pixels[p1x + 2 + m_Width * (p1y + 2)] +
							m_Pixels[p1x - 2 + m_Width * (p1y + 2)] +
							m_Pixels[p1x + m_Width * (p1y + 2)];

						val +=
							m_Pixels[p2x - 2 + m_Width * (p2y - 2)] +
							m_Pixels[p2x - 2 + m_Width * p2y] +
							m_Pixels[p2x + m_Width * (p2y - 2)] +
							m_Pixels[p2x + 2 + m_Width * (p2y - 2)] +
							m_Pixels[p2x + 2 + m_Width * p2y] +
							m_Pixels[p2x + 2 + m_Width * (p2y + 2)] +
							m_Pixels[p2x - 2 + m_Width * (p2y + 2)] +
							m_Pixels[p2x + m_Width * (p2y + 2)];

						val +=
							m_Pixels[p3x - 2 + m_Width * (p3y - 2)] +
							m_Pixels[p3x - 2 + m_Width * p3y] +
							m_Pixels[p3x + m_Width * (p3y - 2)] +
							m_Pixels[p3x + 2 + m_Width * (p3y - 2)] +
							m_Pixels[p3x + 2 + m_Width * p3y] +
							m_Pixels[p3x + 2 + m_Width * (p3y + 2)] +
							m_Pixels[p3x - 2 + m_Width * (p3y + 2)] +
							m_Pixels[p3x + m_Width * (p3y + 2)];

						val +=
							m_Pixels[p4x - 2 + m_Width * (p4y - 2)] +
							m_Pixels[p4x - 2 + m_Width * p4y] +
							m_Pixels[p4x + m_Width * (p4y - 2)] +
							m_Pixels[p4x + 2 + m_Width * (p4y - 2)] +
							m_Pixels[p4x + 2 + m_Width * p4y] +
							m_Pixels[p4x + 2 + m_Width * (p4y + 2)] +
							m_Pixels[p4x - 2 + m_Width * (p4y + 2)] +
							m_Pixels[p4x + m_Width * (p4y + 2)];

						if (val > maxVal)
						{
							maxVal = val;
							m_MaxX = x;
							m_MaxY = y;
							m_Maxh = h;
							m_Maxv = v;

							/*
							int length = sprintf(buff, "%d-%d-%d-%d: %0.2f [NEW MAX VALUE]", x, y, h, v, val);
							strncpy(dbgInfoFill, buff, length);
							dbgInfoFill+=length;

							*dbgInfoFill = '\r';
							*(dbgInfoFill + 1) = '\n';
							dbgInfoFill+=2;*/
						}
						else
						{
							/*
							int length = sprintf(buff, "%d-%d-%d-%d: %0.2f", x, y, h, v, val);
							strncpy(dbgInfoFill, buff, length);
							dbgInfoFill+=length;
							*dbgInfoFill = '\r';
							*(dbgInfoFill + 1) = '\n';
							dbgInfoFill+=2; */
						}
					}
				}
			}
		}
	}
	//*dbgInfoFill = '\0';
}

void IotaVtiOrcEngine::CalibrateScale()
{
	float maxWidthCoeff;
	float maxHeightCoeff;
	float maxGapCoeff;
	int maxDeltaX = 0;
	int maxDeltaY = 0;

	float maxTotalMatch = 0;

	int widthCoeffLen = (int)ceil((SYMBOL_WIDTH_COEFF_TO - SYMBOL_WIDTH_COEFF_FROM) / SYMBOL_WIDTH_COEFF_STEP);
	int heightCoeffLen = (int)ceil((SYMBOL_HEIGHT_COEFF_TO - SYMBOL_HEIGHT_COEFF_FROM) / SYMBOL_HEIGHT_COEFF_STEP);
	int gapCoeffLen = (int)ceil((SYMBOL_GAP_COEFF_TO - SYMBOL_GAP_COEFF_FROM) / SYMBOL_GAP_COEFF_STEP);

	//TODO: Why is the gapCoeffLen 31 instead of 32 (or the other way around)

	float* probeWidthCoeffs = new float[widthCoeffLen];
	float* probeHeightCoeffs = new float[heightCoeffLen];
	float* probeGapCoeffs = new float[gapCoeffLen];

	probeWidthCoeffs[0] = (SYMBOL_WIDTH_COEFF_FROM + SYMBOL_WIDTH_COEFF_TO) / 2;
	for (int i = 1; i < widthCoeffLen; i++)
		probeWidthCoeffs[i] = (float)(probeWidthCoeffs[0] + (i % 2 == 0 ? 1 : -1) * ((i + 1) / 2) * SYMBOL_WIDTH_COEFF_STEP);

	probeHeightCoeffs[0] = (SYMBOL_HEIGHT_COEFF_FROM + SYMBOL_HEIGHT_COEFF_TO) / 2;
	for (int i = 1; i < heightCoeffLen; i++)
		probeHeightCoeffs[i] = (float)(probeHeightCoeffs[0] + (i % 2 == 1 ? 1 : -1) * ((i + 1) / 2) * SYMBOL_HEIGHT_COEFF_STEP);

	probeGapCoeffs[0] = (SYMBOL_GAP_COEFF_FROM + SYMBOL_GAP_COEFF_TO) / 2;
	for (int i = 1; i < gapCoeffLen; i++)
		probeGapCoeffs[i] = (float)(probeGapCoeffs[0] + (i % 2 == 1 ? 1 : -1) * ((i + 1) / 2) * SYMBOL_GAP_COEFF_STEP);
			

	//int dbgInfoLen = (gapCoeffLen) * (heightCoeffLen) * (widthCoeffLen) * (150 + 1000) + 2;
	//char* dbgInfo = (char*)malloc(dbgInfoLen);
	//char buff[100];
	//char buffCalibrate[30000];
	//int buffCalibrateLen;
	//char* dbgInfoFill = dbgInfo;
	
	for(int gapIdx = 0; gapIdx < gapCoeffLen; gapIdx++)
	{
		float gapCoeff  = probeGapCoeffs[gapIdx];

		for (int heightIdx = 0; heightIdx < heightCoeffLen; heightIdx++)
		{
			float heightCoeff = probeHeightCoeffs[heightIdx];

			for (int widthIdx = 0; widthIdx < widthCoeffLen; widthIdx++)
			{
				float widthCoeff = probeWidthCoeffs[widthIdx];

				float totalMatch = CalibrateScale(widthCoeff, heightCoeff, gapCoeff);//, buffCalibrate, &buffCalibrateLen);

				//int length = sprintf(buff, "Trying Width=%0.3f GAP=%0.3f Height=%0.3f | MatchCoeff = %0.3f", widthCoeff, gapCoeff, heightCoeff, totalMatch);
				//strncpy(dbgInfoFill, buff, length);
				//dbgInfoFill+=length;

				//*dbgInfoFill = '\r';
				//*(dbgInfoFill + 1) = '\n';
				//dbgInfoFill+=2;
				
				//if (buffCalibrateLen > 0)
				//{
				//	strncpy(dbgInfoFill, buffCalibrate, buffCalibrateLen);
				//	dbgInfoFill+=buffCalibrateLen;

				//	*dbgInfoFill = '\r';
				//	*(dbgInfoFill + 1) = '\n';
				//	dbgInfoFill+=2;
				//}

				if (totalMatch > maxTotalMatch)
				{
					maxTotalMatch = totalMatch;
					maxWidthCoeff = widthCoeff;
					maxHeightCoeff = heightCoeff;
					maxGapCoeff = gapCoeff;

					//int length = sprintf(buff, "New best match of %0.3f: W=%0.3f H=%0.3f G=%0.3f", totalMatch, widthCoeff, heightCoeff, gapCoeff);
					//strncpy(dbgInfoFill, buff, length);
					//dbgInfoFill+=length;

					//*dbgInfoFill = '\r';
					//*(dbgInfoFill + 1) = '\n';
					//dbgInfoFill+=2;
				}
			}
		}
	}

	//*dbgInfoFill = '\0';

	if (maxTotalMatch > 0)
	{
		CalibrateScale(maxWidthCoeff, maxHeightCoeff, maxGapCoeff);//, buffCalibrate, &buffCalibrateLen);

		m_WidthCoeff = maxWidthCoeff;
		m_GapCoeff = maxGapCoeff;
		m_HeightCoeff = maxHeightCoeff;
	}

	delete probeWidthCoeffs;
	delete probeHeightCoeffs;
	delete probeGapCoeffs;
}

char IotaVtiOrcEngine::Ocr(int charId)
{
	bool* ocrCellsFlags = new bool[ 10 * 10 + 1];
	float percMatch;
	float percMatchWhite;
	float percMatchSpecial;
			
	char c = '\x0';
	float maxMatch = 0;

	int deltas[3] = { 0, 1, -1 };

	for(int deltaYIdx = 0; deltaYIdx < 3; deltaYIdx++)
	{
		int deltaY = deltas[deltaYIdx];

		for(int deltaXIdx = 0; deltaXIdx < 3; deltaXIdx++)
		{
			int deltaX = deltas[deltaXIdx];
	
			percMatch = 0;
			percMatchWhite = 0;
			percMatchSpecial = 0;
			
			char probeC = Ocr(charId, deltaX, deltaY, ocrCellsFlags, &percMatch, &percMatchWhite, &percMatchSpecial);

			float matchCoeff = (2 * percMatch + 0.5f * percMatchWhite + 4 * percMatchSpecial) / 6.5f;

			if (probeC != '\x0' && probeC != ' ' && maxMatch < matchCoeff)
			{
				maxMatch = matchCoeff;

				c = probeC;
			}
		}
	}

	return c;
}

void IotaVtiOrcEngine::ExtractTimeStamp(unsigned long* pixels, VideoTimeStamp* timeStamp)
{
	for (int y = 0; y < m_Height; y++)
	{
		for (int x = 0; x < m_Width; x++)
		{
			unsigned int pixel = pixels[x + y * m_Width]; 
			m_Pixels[x + y * m_Width] = pixel;
		}
	}

	timeStamp->Recognized = true;

	char twoDigits[3];
	twoDigits[2] = '\0';

	twoDigits[0] = Ocr(3);
	twoDigits[1] = Ocr(4);

	try
	{
		timeStamp->Hours = atoi(twoDigits);
	}
	catch (...)
	{
		timeStamp->Hours = 0;
		timeStamp->Recognized = false;
	}

	twoDigits[0] = Ocr(6);
	twoDigits[1] = Ocr(7);

	try
	{
		timeStamp->Minutes = atoi(twoDigits);
	}
	catch (...)
	{
		timeStamp->Minutes = 0;
		timeStamp->Recognized = false;
	}

	twoDigits[0] = Ocr(9);
	twoDigits[1] = Ocr(10);
	try
	{
		timeStamp->Seconds = atoi(twoDigits);
	}
	catch (...)
	{
		timeStamp->Seconds = 0;
		timeStamp->Recognized = false;
	}

	char digits[10];
	char* fourDigitsPtr = digits;

	char c = Ocr(12);
	if (c != '\0') { *fourDigitsPtr = c; fourDigitsPtr++; };
	c = Ocr(13);
	if (c != '\0') { *fourDigitsPtr = c; fourDigitsPtr++; };
	c = Ocr(14);
	if (c != '\0') { *fourDigitsPtr = c; fourDigitsPtr++; };
	c = Ocr(15);
	if (c != '\0') { *fourDigitsPtr = c; fourDigitsPtr++; };

	if (fourDigitsPtr == digits)
	{
		c = Ocr(17);
		if (c != '\0') { *fourDigitsPtr = c; fourDigitsPtr++; };
		c = Ocr(18);
		if (c != '\0') { *fourDigitsPtr = c; fourDigitsPtr++; };
		c = Ocr(19);
		if (c != '\0') { *fourDigitsPtr = c; fourDigitsPtr++; };
		c = Ocr(20);
		if (c != '\0') { *fourDigitsPtr = c; fourDigitsPtr++; };
	}
	*fourDigitsPtr = '\0';

	try
	{
		timeStamp->FractionalSeconds = atoi(digits);
	}
	catch (...)
	{
		timeStamp->FractionalSeconds = 0;
		timeStamp->Recognized = false;
	}

	char* frameIdDigitsPtr = digits;


	c = Ocr(22);
	if (c != '\0') { *frameIdDigitsPtr = c; frameIdDigitsPtr++; };
	c = Ocr(23);
	if (c != '\0') { *frameIdDigitsPtr = c; frameIdDigitsPtr++; };
	c = Ocr(24);
	if (c != '\0') { *frameIdDigitsPtr = c; frameIdDigitsPtr++; };
	c = Ocr(25);
	if (c != '\0') { *frameIdDigitsPtr = c; frameIdDigitsPtr++; };
	c = Ocr(26);
	if (c != '\0') { *frameIdDigitsPtr = c; frameIdDigitsPtr++; };
	c = Ocr(27);
	if (c != '\0') { *frameIdDigitsPtr = c; frameIdDigitsPtr++; };
	c = Ocr(28);
	if (c != '\0') { *frameIdDigitsPtr = c; frameIdDigitsPtr++; };
	c = Ocr(29);
	if (c != '\0') { *frameIdDigitsPtr = c; frameIdDigitsPtr++; };
	*frameIdDigitsPtr = '\0';


	try
	{
		timeStamp->FrameNumber = atoi(digits);
	}
	catch (...)
	{
		timeStamp->FrameNumber = 0;
		timeStamp->Recognized = false;
	}
}

float IotaVtiOrcEngine::CalibrateScale(float widthCoeff, float heightCoeff, float gapCoeff)//, char* buffDebug, int *buffDebugLen)
{
	m_CharacterWidth = m_Maxh / widthCoeff;
	m_CharacterHeight = m_Maxv / heightCoeff;
	m_CharacterGap = gapCoeff * m_Maxh / widthCoeff;

	m_ColonTop = m_MaxY - (m_CharacterHeight - m_Maxv) / 2.0f;
	m_ColonLeft = 1 + m_MaxX - m_CharacterWidth / 2.0f;

	int digits[7] = {3, 4, 6, 7, 9, 10, 26};

	int maxDeltaX = 0, maxDeltaY = 0;
	float maxTotalMatch = 0;

	float totalMatch = 0;
	bool allCharsOk = true;
	int deltas[3] = { 0, 1, -1 };
	
	bool* ocrCellsFlags = new bool[ 10 * 10 + 1];
	for (int digitIdx = 0; digitIdx < 7; digitIdx++)
	{
		int digitId = digits[digitIdx];

		
		float percMatch = 0;
		float percMatchWhite = 0;
		float percMatchSpecial = 0;

		char c = '\x0';
		float maxPercMatch = 0;
		//char buffDbg[1600];
		
		for(int deltaYIdx = 0; deltaYIdx < 3; deltaYIdx++)
		{
			int deltaY = deltas[deltaYIdx];

			for(int deltaXIdx = 0; deltaXIdx < 3; deltaXIdx++)
			{
				int deltaX = deltas[deltaXIdx];
				percMatch = 0;
				percMatchWhite = 0;
				percMatchSpecial = 0;
				
				//char *buffDngCpy = buffDbg;
				char probeC = Ocr(digitId, deltaX, deltaY, ocrCellsFlags, &percMatch, &percMatchWhite, &percMatchSpecial);//, buffDngCpy);
				float matchCoeff = (2 * percMatch + 0.5f * percMatchWhite + 4 * percMatchSpecial) / 6.5f;

				//int len = sprintf(buffDebugCpy, "%s", buffDbg);
				//buffDebugCpy+=len;
				//*buffDebugLen+= len;

				if (probeC != '\x0' && probeC != ' ' && maxPercMatch < matchCoeff)
				{
					maxPercMatch = matchCoeff;

					c = probeC;

					//int len = sprintf(buffDebugCpy, "NEW MAX CHAR= '%d': %0.3f\r\n", probeC, maxPercMatch);
					//buffDebugCpy+=len;
					//*buffDebugLen+= len;
				}
			}
		}

		//*buffDebugCpy = '\0';
		//buffDebugCpy++;
		//*buffDebugLen++;

		if (c == '\x0' || c == ' ')
		{
			if (digitId < 11)
				// If we cannot recognize an HH:MM:SS digit then exit
				allCharsOk = false;
		}
		totalMatch += maxPercMatch;

		if (!allCharsOk)
			break;
	}

	if (maxTotalMatch < totalMatch && allCharsOk)
	{
		maxTotalMatch = totalMatch;
	}

	m_ColonLeft += maxDeltaX;
	m_ColonTop += maxDeltaY;

	delete ocrCellsFlags;
	return maxTotalMatch;
}

void IotaVtiOrcEngine::GetDigitRectangle(int digitId, int deltaLeft, int deltaTop, int* rectLeft, int* rectTop, int* rectRight, int* rectBottom)
{
	int leftDigitPos = round(m_ColonLeft + (digitId - 5) * (m_CharacterWidth + m_CharacterGap));

	*rectLeft = leftDigitPos + deltaLeft;
	*rectTop = (int)round(m_ColonTop) + deltaTop;
	*rectRight = *rectLeft + (int)round(m_CharacterWidth);
	*rectBottom = *rectTop + (int)round(m_CharacterHeight);
}

char IotaVtiOrcEngine::Ocr(int charId, int deltaX, int deltaY, bool* ocrCellsFlags, float* percMatch, float* percMatchWhite, float* percMatchSpecial)//, char* buffDebug)
{
	int rectLeft;
	int rectTop;
	int rectRight;
	int rectBottom;

	GetDigitRectangle(charId, deltaX, deltaY, &rectLeft, &rectTop, &rectRight, &rectBottom);

	int rectWidth = rectRight - rectLeft;
	int rectHeight = rectBottom - rectTop;

	float cellW = (ZOOM_LEVEL * rectWidth) / 10.0f;
	float cellH = (ZOOM_LEVEL * rectHeight) / 10.0f;
	int fullW = (int)round(cellW * 10);
	int fullH = (int)round(cellH * 10);
		
	unsigned int* zoomedPixels = new unsigned int [fullW * fullH + 1];

	//char pxlsDbg[1500];
	//char* pxlsDbgCpy = pxlsDbg;

	for (int x = 0; x < rectWidth; x++)
	{
		for (int y = 0; y < rectHeight; y++)
		{
			if (rectLeft + x >= 0 && rectLeft + x < m_Width && rectTop + y >= 0 && rectTop + y < m_Height)
			{
				unsigned int pixel = m_Pixels[rectLeft + x + m_Width * (rectTop + y)];

				for (int i = 0; i < ZOOM_LEVEL; i++)
				{
					for (int j = 0; j < ZOOM_LEVEL; j++)
					{
						
						zoomedPixels[ZOOM_LEVEL * x + i + fullW * (ZOOM_LEVEL * y + j)] = pixel;
					}
				}
			}
		}
	}

	int len;

	for (int i = 0; i < 10; i++)
	{
		for (int j = 0; j < 10; j++)
		{
			int fromX = (int)round(i * cellW);
			int fromY = (int)round(j * cellH);
			int toX = (int)round((i + 1) * cellW);
			int toY = (int)round((j + 1) * cellH);

			unsigned int sum = 0;
			int pixCount = 0;
			for (int x = fromX; x < toX; x++)
			{
				for (int y = fromY; y < toY; y++)
				{
					sum += zoomedPixels[x + fullW * y];
					pixCount++;
				}
			}

			float val = sum * 1.0f / pixCount;
			m_OcrCells[i + 10 * j] = val;
		}
	}

	for (int y = 0; y < 10; y++)
	{
		for (int x = 0; x < 10; x++)
		{
			float avrgPix = m_OcrCells[x + 10 * y];
			bool isOn = m_Image95Max - m_ImageOrcTolerance < avrgPix;
			m_OcrCellsFlags[x + 10 * y] = isOn;			
			
			//*pxlsDbgCpy = isOn ? '+' : '-';
			//pxlsDbgCpy++;
		}
	}
	//*pxlsDbgCpy = '\0';
	//pxlsDbgCpy++;

	ocrCellsFlags = &m_OcrCellsFlags[0];

	char c = Ocr(ocrCellsFlags, percMatch, percMatchWhite, percMatchSpecial);

	//len = sprintf(buffDebug, "OCR = '%d': %d [%d|%d] (%d-%d,%d-%d) %0.3f %0.3f %0.3f\r\n%s\r\n\0", c, charId, deltaX, deltaY, rectTop, rectBottom, rectLeft, rectRight, *percMatch, *percMatchWhite, *percMatchSpecial, pxlsDbg);
	//buffDebug+=len;

	return c;
}

char IotaVtiOrcEngine::Ocr(bool const * const cells, float* percMatch, float* percMatchWhite, float* percMatchSpecial)
{
	*percMatch = 0;
	*percMatchWhite = 0;
	*percMatchSpecial = 0;

	float bestMatchEstimate = -1;
	float bestMatchEstimateWhite = -1;
	float bestMatchEstimateSpecial = -1;
	char bestMatchSymbol = '\0';

	map<char, SymbolEntry*>::const_iterator it = m_SymbolEntries.begin();
	while(it != m_SymbolEntries.end())
	{
		SymbolEntry* entry  = it->second;
		entry->MatchEstimate = 0;
		entry->MatchEstimateWhites = 0;
		entry->MatchEstimateSpecial = 0;

		MatchSingleEntry(entry, cells);

		if (entry->MatchEstimate > PERCENT_IMMEDIATE_MATCH &&
			entry->MatchEstimateWhites > PERCENT_IMMEDIATE_MATCH_WHITE &&
			entry->MatchEstimateSpecial > PERCENT_SPECIAL_MATCH)
		{
			*percMatch = entry->MatchEstimate;
			*percMatchWhite = entry->MatchEstimateWhites;
			*percMatchSpecial = entry->MatchEstimateSpecial;
			return entry->Symbol;
		}

		if (entry->MatchEstimateWhites > bestMatchEstimateWhite)
		{
			bestMatchEstimate = entry->MatchEstimate;
			bestMatchEstimateWhite = entry->MatchEstimateWhites;
			bestMatchEstimateSpecial = entry->MatchEstimateSpecial;
			bestMatchSymbol = entry->Symbol;
		}

		it++;
	}

	if (bestMatchEstimate > PERCENT_BEST_MATCH &&
		bestMatchEstimateWhite > PERCENT_BEST_MATCH_WHITE &&
		bestMatchEstimateSpecial > PERCENT_SPECIAL_MATCH)
	{
		*percMatch = bestMatchEstimate;
		*percMatchWhite = bestMatchEstimateWhite;
		*percMatchSpecial = bestMatchEstimateSpecial;
		return bestMatchSymbol;
	}

	return '\0';
}

void IotaVtiOrcEngine::MatchSingleEntry(SymbolEntry* const entry, bool const * const cells)
{
	int totalExpectations = 0;
	int totalSpecialExpectations = 0;
	int totalMatches = 0;
	int totalWhiteExpectations = 0;
	int totalWhiteMatches = 0;
	int totalSpecialMatches = 0;

	for (int x = 0; x < 10; x++)
	{
		for (int y = 0; y < 10; y++)
		{
			char expected = entry->OcrConfig[x + 10 * y];

			if (expected == '+' || expected == '-' || expected == '*' || expected == '=') totalExpectations++;
			if (expected == '*' || expected == '=') totalSpecialExpectations++;

			if (expected == '+' || expected == '*') totalWhiteExpectations++;

			if ((expected == '+' || expected == '*') && cells[x + 10 * y])
			{
				totalMatches ++;
				totalWhiteMatches ++;
				if (expected == '*') totalSpecialMatches++;
			}
			else if ((expected == '-' || expected == '=') && !cells[x + 10 * y])
			{
				totalMatches++;
				if (expected == '=') totalSpecialMatches++;
			}
		}
	}

	entry->MatchEstimate = totalMatches * 1.0f / totalExpectations;
	entry->MatchEstimateWhites = totalWhiteMatches * 1.0f / totalWhiteExpectations;
	entry->MatchEstimateSpecial = totalSpecialExpectations > 0 ? totalSpecialMatches * 1.0f / totalSpecialExpectations : 1.0f;
}



void IotaVtiOrcEngine::SetCharacterConfiguration(char symbol, char* symbolConfig)
{	
	SymbolEntry* entry = new SymbolEntry(symbol, symbolConfig);

	m_SymbolEntries.insert(make_pair(symbol, entry));
}