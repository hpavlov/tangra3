#pragma once

#include <map>

// NTSC Values (TV Safe): 0.0292 - 0.0458 (V) | 0.1047 - 0.1094 (H)
// PAL Values (TV Safe) : 0.0208 - 0.0347 (V) | 0.0917 - 0.1083 (H)
// Combined All in one values: 0.02 - 0.05 (V) | 0.09 - 0.11 (H)

// min distance between the two dots devided by the field height
const float MIN_COL_DIST_COEFF = 0.02;
// max distance between the two dots devided by the field height
const float MAX_COL_DIST_COEFF = 0.05;  

// min distance between the top/bottom left and right dot devided by the field width
const float MIN_HORIS_DIST_COEFF = 0.09;
// min distance between the top/bottom left and right dot devided by the field width
const float MAX_HORIS_DIST_COEFF = 0.11;

// NTSC Values : X = 109 / 640.0 (0.17) | Y = 192 / 240.0 (0.80)
// PAL Values :  X = 125 / 720.0 (0.17) | Y = 245 / 288.0 (0.85)
// Combined Values: X = 0.17 | Y = 0.80
// top corner X coordinate of the rect too look for the dots in 
const float RECT_FROM_X_COEFF = 0.17;
// top corner Y coordinate of the rect too look for the dots in 
const float RECT_FROM_Y_COEFF = 0.80;

// NTSC Values: X: 126 / 640 (0.20) | Y: 211 / 240 (0.88)
// PAL Values:  X: 150 / 720 (0.21) | Y = 275 / 288 (0.95)
// Combined Values: X = 0.20; Y = 0.95
const float RECT_TO_X_COEFF = 0.20;
const float RECT_TO_Y_COEFF = 0.95;

// NTSC VAlues: W: 3.96 | H : 0.54 | G : 0.32
// PAL VAlues:  W: 4.02 | H : 0.55 | G : 0.31
const float SYMBOL_WIDTH_COEFF_FROM = 3.9;
const float SYMBOL_WIDTH_COEFF_TO = 4.1;
const float SYMBOL_WIDTH_COEFF_STEP = 0.025;
const float SYMBOL_HEIGHT_COEFF_FROM = 0.54;
const float SYMBOL_HEIGHT_COEFF_TO = 0.55;
const float SYMBOL_HEIGHT_COEFF_STEP = 0.05;
const float SYMBOL_GAP_COEFF_FROM = 0.31;
const float SYMBOL_GAP_COEFF_TO = 0.32;
const float SYMBOL_GAP_COEFF_STEP = 0.025;

const float TOLERANCE_COEFF = 0.5;
const int ZOOM_LEVEL = 20;

const float PERCENT_IMMEDIATE_MATCH = 0.80;
const float PERCENT_IMMEDIATE_MATCH_WHITE = 0.98;
const float PERCENT_BEST_MATCH = 0.65;
const float PERCENT_BEST_MATCH_WHITE = 0.75;
const float PERCENT_SPECIAL_MATCH = 0.70;

struct VideoTimeStamp
{
	bool Recognized;
	unsigned char Hours;
	unsigned char Minutes;
	unsigned char Seconds;
	unsigned int FractionalSeconds;
	unsigned long long FrameNumber;
};

class SymbolEntry
{
	public:
		char* OcrConfig;
		char Symbol;
		float MatchEstimate;
		float MatchEstimateWhites;
		float MatchEstimateSpecial;
		bool* Cells;
		
		SymbolEntry(char symbol, char* config);
		~SymbolEntry();		

	private:
		void LoadSymbolOCRConfig(char* config);
};

class IotaVtiOrcEngine 
{
	private:
		std::map<char, SymbolEntry*> m_SymbolEntries;

		int m_Width;
		int m_Height;

		unsigned long* m_Pixels;

		long m_MinVDis;
		long m_MaxVDis;
		long m_MinHDis;
		long m_MaxHDis;

		long m_MaxX;
		long m_MaxY;
		long m_Maxh;
		long m_Maxv;

		float m_CharacterWidth;
		float m_CharacterHeight;
		float m_CharacterGap;
		float m_ColonTop;
		float m_ColonLeft;

		float m_WidthCoeff;
		float m_GapCoeff;
		float m_HeightCoeff;

		float m_ImageMedian;
		float m_Image95Max;
		float m_ImageOrcTolerance;

		float m_OcrCells[10 * 10 + 1];
		bool m_OcrCellsFlags[10 * 10 + 1];

	public:
		IotaVtiOrcEngine()
		{
			m_Pixels = NULL;
		};

		void SetImage(int width, int height, unsigned long* pixels);
		void SetCharacterConfiguration(char symbol, char* symbolConfig);

		void Calibrate();
		char Ocr(int charId);
		void ExtractTimeStamp(unsigned long* pixels, VideoTimeStamp* timeStamp);

	private:
		void CalibratePosition();
		void CalibrateScale();
		float CalibrateScale(float widthCoeff, float heightCoeff, float gapCoeff);//, char* buffDebug, int *buffDebugLen);
		char Ocr(int charId, int deltaX, int deltaY, bool* ocrCellsFlags, float* percMatch, float* percMatchWhite, float* percMatchSpecial);//, char* buffDebug);
		char Ocr(bool const * const cells, float* percMatch, float* percMatchWhite, float* percMatchSpecial);
		void GetDigitRectangle(int digitId, int deltaLeft, int deltaTop, int* rectLeft, int* rectTop, int* rectRight, int* rectBottom);	
		void MatchSingleEntry(SymbolEntry* const entry, bool const * const cells);			
};