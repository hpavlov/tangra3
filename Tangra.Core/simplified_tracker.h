/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef SIMPLIFIEDTRACKER_H
#define SIMPLIFIEDTRACKER_H

#include "cross_platform.h"
#include "psf_fit.h"

struct NativePsfFitInfo
{	
	float XCenter;
	float YCenter;
	float FWHM;
	float IMax;
	float I0;
	float X0;
	float Y0;
	unsigned char MatrixSize;
	unsigned char IsSolved;

	unsigned char IsAsymmetric;
	unsigned char Reserved;
	float R0;
	float R02;	
};

struct NativeTrackedObjectInfo
{
	float CenterXDouble;
	float CenterYDouble;
	float LastKnownGoodPositionXDouble;
	float LastKnownGoodPositionYDouble;
	unsigned int IsLocated;
	unsigned int IsOffScreen;	
	unsigned int TrackingFlags;
	float LastKnownGoodPsfCertainty;
};

enum NotMeasuredReasons
{	
	TrackedSuccessfully,
	ObjectCertaintyTooSmall,
	FWHMOutOfRange,
	ObjectTooElongated,
	FitSuspectAsNoGuidingStarsAreLocated,
	FixedObject,
	FullyDisappearingStarMarkedTrackedWithoutBeingFound
};

class TrackedObject
{
public:
	bool IsFixedAperture;
	bool IsOccultedStar;
	double StartingX;
	double StartingY;
	double ApertureInPixels;
	int ObjectId;
	
	int CenterX;
	int CenterY;
	double CenterXDouble;
	double CenterYDouble;
	bool IsLocated;
	double LastKnownGoodPositionXDouble;
	double LastKnownGoodPositionYDouble;
	double LastKnownGoodPsfCertainty;
	bool IsOffScreen;
	unsigned int TrackingFlags;
	
	PsfFit* CurrentPsfFit;
	bool UseCurrentPsfFit;
	
	TrackedObject(int objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels, PSFFittingDataRange dataRange, unsigned int maxPixelValue);
	~TrackedObject();
	
	void NextFrame();
	
	void InitialiseNewTracking();
	void SetIsTracked(bool isLocated, NotMeasuredReasons reason, double x, double y, double certainty);	
};

class SimplifiedTracker
{
private:
	int m_Width;
	int m_Height;
	int m_NumTrackedObjects;	
	bool m_IsFullDisappearance;
	
	bool m_IsTrackedSuccessfully;
	TrackedObject** m_TrackedObjects;
	unsigned int* m_AreaPixels;
	
	PSFFittingDataRange m_DataRange;
	unsigned int m_MaxPixelValue;
	
	unsigned int* GetPixelsArea(unsigned int* pixels, int centerX, int centerY, int squareWidth);
	
public:
	SimplifiedTracker(int width, int height, int numTrackedObjects, bool isFullDisappearance, PSFFittingDataRange dataRange, unsigned int maxPixelValue);
	~SimplifiedTracker();
	
	void ConfigureObject(int objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);
	void UpdatePsfFittingMethod();
	void InitialiseNewTracking();
	void NextFrame(int frameNo, unsigned int* pixels);
	int TrackerGetTargetState(int objectId, NativeTrackedObjectInfo* trackingInfo, NativePsfFitInfo* psfInfo, double* residuals);
	bool IsTrackedSuccessfully();
	bool DoManualFrameCorrection(int objectId, int deltaX, int deltaY);
};

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC int TrackerSettings(double maxElongation, double minFWHM, double maxFWHM, double minCertainty);
DLL_PUBLIC int TrackerNewConfiguration(int width, int height, int numTrackedObjects, bool isFullDisappearance, PSFFittingDataRange dataRange, unsigned int maxPixelValue);
DLL_PUBLIC int TrackerConfigureObject(int objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);
DLL_PUBLIC int TrackerNextFrame(int frameId, unsigned int* pixels);
DLL_PUBLIC int TrackerGetTargetState(int objectId, NativeTrackedObjectInfo* trackingInfo, NativePsfFitInfo* psfInfo, double* residuals);
DLL_PUBLIC int TrackerInitialiseNewTracking();
DLL_PUBLIC int TrackerDoManualFrameCorrection(int objectId, int deltaX, int deltaY);

#ifdef __cplusplus
} // __cplusplus defined.
#endif

#endif // SIMPLIFIEDTRACKER_H
