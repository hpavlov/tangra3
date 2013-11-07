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
	
	TrackedObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);
	~TrackedObject();
	
	void NextFrame();
	void SetTrackedObjectMatch(PsfFit* psf);
	
	void SetIsMeasured(bool isLocated, NotMeasuredReasons reason);
	void SetIsTracked(bool isLocated, NotMeasuredReasons reason, PsfFit* fit);
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
	
	unsigned long* GetPixelsArea(unsigned long* pixels, long centerX, long centerY, long squareWidth);
	
public:
	SimplifiedTracker(long width, long height, long numTrackedObjects, bool isFullDisappearance);
	~SimplifiedTracker();
	
	void ConfigureObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);
	void NextFrame(int frameNo, unsigned long* pixels);
};

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC long TrackerSettings(double maxElongation, double minFWHM, double maxFWHM, double minCertainty);
DLL_PUBLIC long TrackerNewConfiguration(long width, long height, long numTrackedObjects, bool isFullDisappearance);
DLL_PUBLIC long TrackerConfigureObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);
DLL_PUBLIC long TrackerNextFrame(long frameId, unsigned long* pixels);
DLL_PUBLIC long TrackerGetTargetPsf(long objectId, NativePsfFitInfo* psfInfo, double* residuals);

#ifdef __cplusplus
} // __cplusplus defined.
#endif

#endif // SIMPLIFIEDTRACKER_H
