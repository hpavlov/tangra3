#include "simplified_tracker.h"
#include "stdio.h"

static double MAX_ELONGATION;
static double MIN_FWHM;
static double MAX_FWHM;
static double MIN_CERTAINTY;


TrackedObject::TrackedObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels)
{
	ObjectId = objectId;
	IsFixedAperture = isFixedAperture;
	IsOccultedStar = isOccultedStar;
	StartingX = startingX;
	StartingY = startingY;
	ApertureInPixels = apertureInPixels;
}

TrackedObject::~TrackedObject()
{
	
}

void TrackedObject::NextFrame()
{
	// TODO
}

void TrackedObject::SetTrackedObjectMatch(PsfFit* psf)
{
	// TODO
}

void TrackedObject::SetIsMeasured(bool isLocated, NotMeasuredReasons reason)
{
	// TODO: Combine SetIsMeasured() and SetIsTracked() into one
}

void TrackedObject::SetIsTracked(bool isLocated, NotMeasuredReasons reason, PsfFit* fit)
{
	// TODO: Combine SetIsMeasured() and SetIsTracked() into one
}

void TrackedObject::SetIsTracked(bool isLocated, NotMeasuredReasons reason, double x, double y)
{
	// TODO: Combine SetIsMeasured() and SetIsTracked() into one
}

SimplifiedTracker::SimplifiedTracker(long width, long height, long numTrackedObjects, bool isFullDisappearance)
{
	m_Width = width;
	m_Height = height;
	m_NumTrackedObjects = numTrackedObjects;
	m_IsFullDisappearance = isFullDisappearance;
	
	m_TrackedObjects = (TrackedObject**)malloc(numTrackedObjects * sizeof(TrackedObject*));
	for(int i = 0; i < numTrackedObjects; i ++)
		m_TrackedObjects[i] = NULL;
}

SimplifiedTracker::~SimplifiedTracker()
{
	if (NULL != m_TrackedObjects)
	{
		for(int i = 0; i < m_NumTrackedObjects; i ++)
		{
			if (NULL != m_TrackedObjects[i])
			{
				delete m_TrackedObjects[i];
				m_TrackedObjects[i] = NULL;
			}
		}		
	}
}

void SimplifiedTracker::ConfigureObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels)
{
	if (objectId >=0 && objectId < m_NumTrackedObjects)
		m_TrackedObjects[objectId] = new TrackedObject(objectId, isFixedAperture, isOccultedStar, startingX, startingY, apertureInPixels);
}

unsigned long* SimplifiedTracker::GetPixelsArea(unsigned long* pixels, long centerX, long centerY, long squareWidth)
{
	// TODO
}

void SimplifiedTracker::NextFrame(int frameNo, unsigned long* pixels)
{
	m_IsTrackedSuccessfully = false;

	// For each of the non manualy positioned Tracked objects do a PSF fit in the area of its previous location 
	for (int i = 0; i < m_NumTrackedObjects; i++)
	{
		TrackedObject* trackedObject = m_TrackedObjects[i];
		trackedObject->NextFrame();

		if (trackedObject->IsFixedAperture ||
			(trackedObject->IsOccultedStar && m_IsFullDisappearance))
		{
			// Star position will be determined after the rest of the stars are found 
		}
		else
		{
			unsigned long* areaPixels = GetPixelsArea(pixels, trackedObject->CenterX, trackedObject->CenterY, 17);
			PsfFit* fit = new PsfFit(trackedObject->CenterX, trackedObject->CenterY);
			fit->FittingMethod = MAX_ELONGATION == 0 ? NonLinearFit : NonLinearAsymetricFit;
			fit->Fit(areaPixels, 17);

			if (fit->IsSolved())
			{
				if (fit->Certainty() < MIN_CERTAINTY)
				{
					trackedObject->SetIsMeasured(false, ObjectCertaintyTooSmall);
				}
				else if (fit->FWHM() < MIN_FWHM || fit->FWHM() > MAX_FWHM)
				{
					trackedObject->SetIsMeasured(false, FWHMOutOfRange);
				}
				else if (MAX_ELONGATION > 0 && fit->ElongationPercentage() > MAX_ELONGATION)
				{
					trackedObject->SetIsMeasured(false, ObjectTooElongated);
				}
				else
				{
					trackedObject->SetTrackedObjectMatch(fit);
					trackedObject->SetIsMeasured(true, TrackedSuccessfully);
				}
			}
		}					
	}

	bool atLeastOneObjectLocated = false;

	for (int i = 0; i < m_NumTrackedObjects; i++)
	{
		TrackedObject* trackedObject = m_TrackedObjects[i];

		double totalX = 0;
		double totalY = 0;
		int numReferences = 0;
		
		for (int j = 0; j < m_NumTrackedObjects; j++)
		{
			TrackedObject* referenceObject = m_TrackedObjects[j];
			
			if (referenceObject->IsLocated)
			{
				totalX += (trackedObject->StartingX - referenceObject->StartingX) + referenceObject->CenterXDouble;
				totalY += (trackedObject->StartingY - referenceObject->StartingY) + referenceObject->CenterYDouble;
				numReferences++;
				atLeastOneObjectLocated = true;
			}
		}

		if (numReferences == 0)
		{
			trackedObject->SetIsTracked(false, FitSuspectAsNoGuidingStarsAreLocated, NULL);
		}
		else
		{
			double x_double = totalX / numReferences;
			double y_double = totalY / numReferences;

			if (trackedObject->IsFixedAperture)
			{
				trackedObject->SetIsTracked(true, FixedObject, x_double, y_double);
			}
			else if (trackedObject->IsOccultedStar && m_IsFullDisappearance)
			{
				long x = (long)(x_double + 0.5); // rounded
				long y = (long)(y_double + 0.5); // rounded

				long matrixSize = (long)(trackedObject->ApertureInPixels * 1.5 + 0.5); // rounded
				if (matrixSize % 2 == 0) matrixSize++;
				if (matrixSize > 17) matrixSize = 17;

				unsigned long* areaPixels = GetPixelsArea(pixels, x, y, matrixSize);


				PsfFit* fit = new PsfFit(x, y);
				
				fit->Fit(areaPixels, matrixSize);


				if (fit->IsSolved() && fit->Certainty() > MIN_CERTAINTY)
				{
					trackedObject->SetIsTracked(true, TrackedSuccessfully, fit);
					trackedObject->SetTrackedObjectMatch(fit);
				}
				else
					trackedObject->SetIsTracked(false, FullyDisappearingStarMarkedTrackedWithoutBeingFound, NULL);
			}
		}
	}

	m_IsTrackedSuccessfully = atLeastOneObjectLocated;
}


static SimplifiedTracker* s_Tracker;

long TrackerSettings(double maxElongation, double minFWHM, double maxFWHM, double minCertainty)
{
	MAX_ELONGATION = maxElongation;
	MIN_FWHM = minFWHM;
	MAX_FWHM = maxFWHM;
	MIN_CERTAINTY = minCertainty;
}

long TrackerNewConfiguration(long width, long height, long numTrackedObjects, bool isFullDisappearance)
{
	if (NULL != s_Tracker)
	{
		delete s_Tracker;
		s_Tracker = NULL;
	}
	
	s_Tracker = new SimplifiedTracker(width, height, numTrackedObjects, isFullDisappearance);
}

long TrackerConfigureObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels)
{
	if (NULL != s_Tracker)
	{
		s_Tracker->ConfigureObject(objectId, isFixedAperture, isOccultedStar, startingX, startingY, apertureInPixels);
	}
}

long TrackerNextFrame(long frameId, unsigned long* pixels)
{
	if (NULL != s_Tracker)
		s_Tracker->NextFrame(frameId, pixels);
}

long TrackerGetTargetPsf(long objectId, NativePsfFitInfo* psfInfo, double* residuals)
{
	// TODO:
}
