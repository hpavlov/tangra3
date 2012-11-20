#include "stdlib.h"
#include "Tangra.Orc.IotaVti.h"
#include <vector>
#include <string>

long s_OcrWidth;
long s_OcrHeight;
unsigned long* s_Pixels;


unsigned long s_ImageMedian;
unsigned long s_Image95Max;
float s_ImageOcrTolerance;

long s_MinVDis;
long s_MaxVDis;
long s_MinHDis;
long s_MaxHDis;
long s_MaxX = 0;
long s_MaxY = 0;
long s_Maxh = 0;
long s_Maxv = 0;

float s_CharacterWidth;
float s_CharacterHeight;
float s_CharacterGap;
float s_ColonTop;
float s_ColonLeft;

IotaVtiOrcEngine* s_IotaVtiOrcEngine = NULL;
bool s_UseOddFieldTimeStamp;
int s_FrameWidth;
int s_FrameHeight;
int s_FieldHeight;

extern "C" int IOTAVTIOCR_CalibratePixelMap(unsigned long* framePixels, unsigned long frameWidth, unsigned long frameHeight)
{
	if (NULL != s_IotaVtiOrcEngine)
	{
		s_FrameWidth = frameWidth;
		s_FrameHeight = frameHeight;
		int s_FieldHeight = frameHeight / 2;

		unsigned long* evenFieldPixels = new unsigned long[frameWidth * s_FieldHeight];
		unsigned long* oddFieldPixels = new unsigned long[frameWidth * s_FieldHeight];

		unsigned long* framePixelsPtr = framePixels;
		unsigned long* evenFieldPixelsPtr = evenFieldPixels;
		unsigned long* oddFieldPixelsPtr = oddFieldPixels;

		for(int idx = 0; idx < frameWidth * frameHeight; idx++)
		{
			bool isEvenField = (idx / frameWidth) % 2 == 1; // Index 0 is the first line i.e. odd field
			if (isEvenField)
			{
				*evenFieldPixelsPtr = *framePixelsPtr;
				evenFieldPixelsPtr++;
			}
			else
			{
				*oddFieldPixelsPtr = *framePixelsPtr;
				oddFieldPixelsPtr++;
			}

			framePixelsPtr++;
		}

		s_IotaVtiOrcEngine->SetImage(frameWidth, s_FieldHeight, evenFieldPixels);
		s_IotaVtiOrcEngine->Calibrate();

		// Determine which field has the smaller frame number and use that field (even or odd) for extracting the actual timestamp
		VideoTimeStamp evenTimeStamp;
		VideoTimeStamp oddTimeStamp;

		s_IotaVtiOrcEngine->ExtractTimeStamp(evenFieldPixels, &evenTimeStamp);
		s_IotaVtiOrcEngine->ExtractTimeStamp(oddFieldPixels, &oddTimeStamp);

		s_UseOddFieldTimeStamp = oddTimeStamp.FrameNumber < evenTimeStamp.FrameNumber;

		return 0;
	}	

	return -1;
}

extern "C" int IOTAVTIOCR_ExtractTimeStamp(unsigned long* pixels, VideoTimeStamp* timeStamp)
{
	if (NULL != s_IotaVtiOrcEngine)
	{
		unsigned long* fieldPixels = new unsigned long[s_FrameWidth * s_FieldHeight];
		
		unsigned long* framePixelsPtr = pixels;
		unsigned long* fieldPixelsPtr = fieldPixels;

		for(int idx = 0; idx < s_FrameWidth * s_FrameHeight; idx++)
		{
			bool isEvenField = (idx / s_FrameWidth) % 2 == 1; // Index 0 is the first line i.e. odd field
			bool addPixelsToField = (s_UseOddFieldTimeStamp && !isEvenField) || (!s_UseOddFieldTimeStamp && isEvenField);
			if (addPixelsToField)
			{
				*fieldPixelsPtr = *framePixelsPtr;
				fieldPixelsPtr++;
			}

			framePixelsPtr++;
		}

		s_IotaVtiOrcEngine->ExtractTimeStamp(fieldPixels, timeStamp);

		return 0;
	}	

	return -1;
}


extern "C" int IOTAVTIOCR_Initialise()
{
	if (NULL == s_IotaVtiOrcEngine)
	{
		s_IotaVtiOrcEngine = new IotaVtiOrcEngine();
		return 1;
	}	
	
	return 0;
}


extern "C" int IOTAVTIOCR_SetCharacterConfiguration(char symbol, char* symbolConfig)
{
	if (NULL != s_IotaVtiOrcEngine)
	{
		s_IotaVtiOrcEngine->SetCharacterConfiguration(symbol, symbolConfig);
		return 0;
	}	

	return -1;
}