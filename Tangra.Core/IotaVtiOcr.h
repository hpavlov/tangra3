#pragma once
#include "Tangra.Orc.IotaVti.h"

extern IotaVtiOrcEngine* s_IotaVtiOrcEngine;
extern bool s_UseOddFieldTimeStamp

extern bool s_UseOddFieldTimeStamp;
extern int s_FrameWidth;
extern int s_FrameHeight;
extern int s_FieldHeight;;

extern long s_OcrWidth;
extern long s_OcrHeight;
extern unsigned long* s_Pixels;


extern unsigned long s_ImageMedian;
extern unsigned long s_Image95Max;
extern float s_ImageOcrTolerance;

extern long s_MinVDis;
extern long s_MaxVDis;
extern long s_MinHDis;
extern long s_MaxHDis;
extern long s_MaxX = 0;
extern long s_MaxY = 0;
extern long s_Maxh = 0;
extern long s_Maxv = 0;

extern float s_CharacterWidth;
extern float s_CharacterHeight;
extern float s_CharacterGap;
extern float s_ColonTop;
extern float s_ColonLeft;