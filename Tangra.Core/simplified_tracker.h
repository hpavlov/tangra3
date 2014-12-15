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
	long ObjectId;
	
	long CenterX;
	long CenterY;
	double CenterXDouble;
	double CenterYDouble;
	bool IsLocated;
	double LastKnownGoodPositionXDouble;
	double LastKnownGoodPositionYDouble;
	bool IsOffScreen;
	unsigned int TrackingFlags;
	
	PsfFit* CurrentPsfFit;
	bool UseCurrentPsfFit;
	
	TrackedObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels, PSFFittingDataRange dataRange, unsigned int maxPixelValue);
	~TrackedObject();
	
	void NextFrame();
	
	void InitialiseNewTracking();
	void SetIsTracked(bool isLocated, NotMeasuredReasons reason, double x, double y);	
};

class SimplifiedTracker
{
private:
	long m_Width;
	long m_Height;
	long m_NumTrackedObjects;	
	bool m_IsFullDisappearance;
	
	bool m_IsTrackedSuccessfully;
	TrackedObject** m_TrackedObjects;
	unsigned long* m_AreaPixels;
	
	PSFFittingDataRange m_DataRange;
	unsigned int m_MaxPixelValue;
	
	unsigned long* GetPixelsArea(unsigned long* pixels, long centerX, long centerY, long squareWidth);
	
public:
	SimplifiedTracker(long width, long height, long numTrackedObjects, bool isFullDisappearance, PSFFittingDataRange dataRange, unsigned int maxPixelValue);
	~SimplifiedTracker();
	
	void ConfigureObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);
	void UpdatePsfFittingMethod();
	void InitialiseNewTracking();
	void NextFrame(int frameNo, unsigned long* pixels);
	long TrackerGetTargetState(long objectId, NativeTrackedObjectInfo* trackingInfo, NativePsfFitInfo* psfInfo, double* residuals);
	bool IsTrackedSuccessfully();
};

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC long TrackerSettings(double maxElongation, double minFWHM, double maxFWHM, double minCertainty);
DLL_PUBLIC long TrackerNewConfiguration(long width, long height, long numTrackedObjects, bool isFullDisappearance, PSFFittingDataRange dataRange, unsigned int maxPixelValue);
DLL_PUBLIC long TrackerConfigureObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);
DLL_PUBLIC long TrackerNextFrame(long frameId, unsigned long* pixels);
DLL_PUBLIC long TrackerGetTargetState(long objectId, NativeTrackedObjectInfo* trackingInfo, NativePsfFitInfo* psfInfo, double* residuals);
DLL_PUBLIC long TrackerInitialiseNewTracking();

#ifdef __cplusplus
} // __cplusplus defined.
#endif

#endif // SIMPLIFIEDTRACKER_H
