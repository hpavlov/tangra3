/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "simplified_tracker.h"
#include "stdio.h"
#include "psf_fit.h"

static double MAX_ELONGATION;
static double MIN_FWHM;
static double MAX_FWHM;
static double MIN_CERTAINTY;


TrackedObject::TrackedObject(int objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels, PSFFittingDataRange dataRange, unsigned int maxPixelValue)
{
	ObjectId = objectId;
	IsFixedAperture = isFixedAperture;
	IsOccultedStar = isOccultedStar;
	StartingX = startingX;
	StartingY = startingY;
	ApertureInPixels = apertureInPixels;
	CurrentPsfFit = new PsfFit(dataRange, maxPixelValue);
	CurrentPsfFit->FittingMethod = MAX_ELONGATION == 0 ? NonLinearFit : NonLinearAsymetricFit;
}

TrackedObject::~TrackedObject()
{
	if (NULL != CurrentPsfFit)
	{
		delete CurrentPsfFit;
		CurrentPsfFit = NULL;
	}	
}

void TrackedObject::NextFrame()
{
	// TODO: Do we copy the current center to last known position here?
	
	TrackingFlags = 0;
	IsLocated = false;
	IsOffScreen = false;	
}

void TrackedObject::InitialiseNewTracking()
{
	CenterXDouble = StartingX;
	CenterYDouble = StartingY;
	CenterX = (int)(StartingX + 0.5); // rounding
	CenterY = (int)(StartingY + 0.5); // rounding
	
	LastKnownGoodPositionXDouble = CenterXDouble;
	LastKnownGoodPositionYDouble = CenterYDouble;
	LastKnownGoodPsfCertainty = 0;
}


void TrackedObject::SetIsTracked(bool isLocated, NotMeasuredReasons reason, double x, double y, double certainty)
{
	if (isLocated)
	{
		LastKnownGoodPositionXDouble = CenterXDouble;
		LastKnownGoodPositionYDouble = CenterYDouble;
		CenterXDouble = x;
		CenterYDouble = y;
		LastKnownGoodPsfCertainty = certainty;
		CenterX = (int)(x + 0.5); // rounding
		CenterY = (int)(y + 0.5); // rounding
	}

	IsLocated = isLocated;
	TrackingFlags = (unsigned int)reason;
}

SimplifiedTracker::SimplifiedTracker(int width, int height, int numTrackedObjects, bool isFullDisappearance, PSFFittingDataRange dataRange, unsigned int maxPixelValue)
{
	m_Width = width;
	m_Height = height;
	m_NumTrackedObjects = numTrackedObjects;
	m_IsFullDisappearance = isFullDisappearance;
	m_DataRange = dataRange;
	m_MaxPixelValue = maxPixelValue;
	
	m_TrackedObjects = (TrackedObject**)malloc(numTrackedObjects * sizeof(TrackedObject*));
	for(int i = 0; i < numTrackedObjects; i ++)
		m_TrackedObjects[i] = NULL;	
		
	m_AreaPixels = (unsigned int*)malloc(sizeof(unsigned int) * MAX_MATRIX_SIZE * MAX_MATRIX_SIZE);		
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
	
	if (NULL != m_AreaPixels)
	{
		delete m_AreaPixels;
		m_AreaPixels = NULL;
	}	
}

void SimplifiedTracker::ConfigureObject(int objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels)
{
	if (objectId >=0 && objectId < m_NumTrackedObjects)
		m_TrackedObjects[objectId] = new TrackedObject(objectId, isFixedAperture, isOccultedStar, startingX, startingY, apertureInPixels, m_DataRange, m_MaxPixelValue);
}

void SimplifiedTracker::UpdatePsfFittingMethod()
{
	for (int i = 0; i < m_NumTrackedObjects; i++)
		m_TrackedObjects[i]->CurrentPsfFit->FittingMethod = MAX_ELONGATION == 0 ? NonLinearFit : NonLinearAsymetricFit;
}

void SimplifiedTracker::InitialiseNewTracking()
{
	for (int i = 0; i < m_NumTrackedObjects; i++)
		m_TrackedObjects[i]->InitialiseNewTracking();
}

bool SimplifiedTracker::DoManualFrameCorrection(int objectId, int deltaX, int deltaY)
{
	for (int i = 0; i < m_NumTrackedObjects; i++)
	{
		if (m_TrackedObjects[i]->ObjectId == objectId)
		{
			m_TrackedObjects[i]->SetIsTracked(true, TrackedSuccessfully, m_TrackedObjects[i]->CenterXDouble + deltaX, m_TrackedObjects[i]->CenterYDouble + deltaY, 1);
			m_TrackedObjects[i]->LastKnownGoodPositionXDouble = m_TrackedObjects[i]->CenterXDouble;
			m_TrackedObjects[i]->LastKnownGoodPositionYDouble = m_TrackedObjects[i]->CenterYDouble;
			m_TrackedObjects[i]->IsLocated = true;
			return true;
		}
	}
}

bool SimplifiedTracker::IsTrackedSuccessfully()
{
	return this->m_IsTrackedSuccessfully;
}

unsigned int* SimplifiedTracker::GetPixelsArea(unsigned int* pixels, int centerX, int centerY, int squareWidth)
{
	int x0 = centerX;
	int y0 = centerY;

	int halfWidth = squareWidth / 2;

	for (int x = x0 - halfWidth; x <= x0 + halfWidth; x++)
		for (int y = y0 - halfWidth; y <= y0 + halfWidth; y++)
		{
			unsigned int pixelVal = 0;

			if (x >= 0 && x < m_Width && y >= 0 & y < m_Height)
			{
				pixelVal = *(pixels + x + y * m_Width);
			}

			*(m_AreaPixels + x - x0 + halfWidth + (y - y0 + halfWidth) * squareWidth) = pixelVal;
		}

	return m_AreaPixels;
}

/*
unsigned int* SimplifiedTracker::GetPixelsArea(unsigned int* pixels, int centerX, int centerY, int squareWidth)
{
	long halfWidth = squareWidth / 2;
	int areaLine = 0;
	for (int y = centerY - halfWidth; y <centerY + halfWidth; y++, areaLine++)
	{
		memcpy(m_AreaPixels + (areaLine * squareWidth), pixels + (y * m_Width + centerX - halfWidth), squareWidth);
	}
	
	for (int y = 0 ; y < squareWidth; y++)
	{
		for (int x = 0; x < squareWidth; x++)
		{
			int val = *(m_AreaPixels + y * squareWidth + x) / 256;
			printf("%d", val);
		}
		printf("\n\r");
	}
	
	return m_AreaPixels;
}*/

void SimplifiedTracker::NextFrame(int frameNo, unsigned int* pixels)
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
			unsigned int* areaPixels = GetPixelsArea(pixels, trackedObject->CenterX, trackedObject->CenterY, 17);
			
			trackedObject->UseCurrentPsfFit = false;
			trackedObject->CurrentPsfFit->Fit(trackedObject->CenterX, trackedObject->CenterY, areaPixels, 17);

			if (trackedObject->CurrentPsfFit->IsSolved())
			{
				if (trackedObject->CurrentPsfFit->Certainty() < MIN_CERTAINTY)
				{
					trackedObject->SetIsTracked(false, ObjectCertaintyTooSmall, 0, 0, 0);
				}
				else 
				if (trackedObject->CurrentPsfFit->FWHM() < MIN_FWHM || trackedObject->CurrentPsfFit->FWHM() > MAX_FWHM)
				{
					trackedObject->SetIsTracked(false, FWHMOutOfRange, 0, 0, 0);
				}
				else if (MAX_ELONGATION > 0 && trackedObject->CurrentPsfFit->ElongationPercentage() > MAX_ELONGATION)
				{
					trackedObject->SetIsTracked(false, ObjectTooElongated, 0, 0, 0);
				}
				else
				{
					trackedObject->UseCurrentPsfFit = true;
					trackedObject->SetIsTracked(true, TrackedSuccessfully, trackedObject->CurrentPsfFit->XCenter(), trackedObject->CurrentPsfFit->YCenter(), trackedObject->CurrentPsfFit->Certainty());
				}
			}
		}					
	}

	bool atLeastOneObjectLocated = false;

	for (int i = 0; i < m_NumTrackedObjects; i++)
	{
		TrackedObject* trackedObject = m_TrackedObjects[i];

		bool needsRelativePositioning = trackedObject->IsFixedAperture || (trackedObject->IsOccultedStar && m_IsFullDisappearance);
		
		if (!needsRelativePositioning && trackedObject->IsLocated)
			atLeastOneObjectLocated = true;
			
		if (!needsRelativePositioning) 
			continue;
		
		double totalX = 0;
		double totalY = 0;
		int numReferences = 0;
		
		for (int j = 0; j < m_NumTrackedObjects; j++)
		{
			TrackedObject* referenceObject = m_TrackedObjects[j];
			bool relativeReference = referenceObject->IsFixedAperture || (referenceObject->IsOccultedStar && m_IsFullDisappearance);
		
			if (referenceObject->IsLocated && !relativeReference)
			{
				totalX += (trackedObject->StartingX - referenceObject->StartingX) + referenceObject->CenterXDouble;
				totalY += (trackedObject->StartingY - referenceObject->StartingY) + referenceObject->CenterYDouble;
				numReferences++;
			}
		}

		if (numReferences == 0)
		{
			trackedObject->UseCurrentPsfFit = false;
			trackedObject->SetIsTracked(false, FitSuspectAsNoGuidingStarsAreLocated, 0, 0, 0);
		}
		else
		{
			double x_double = totalX / numReferences;
			double y_double = totalY / numReferences;

			if (trackedObject->IsFixedAperture)
			{
				trackedObject->UseCurrentPsfFit = false;
				trackedObject->SetIsTracked(true, FixedObject, x_double, y_double, 1);
			}
			else if (trackedObject->IsOccultedStar && m_IsFullDisappearance)
			{
				int x = (int)(x_double + 0.5); // rounded
				int y = (int)(y_double + 0.5); // rounded

				int matrixSize = (int)(trackedObject->ApertureInPixels * 1.5 + 0.5); // rounded
				if (matrixSize % 2 == 0) matrixSize++;
				if (matrixSize > 17) matrixSize = 17;

				unsigned int* areaPixels = GetPixelsArea(pixels, x, y, matrixSize);

				trackedObject->UseCurrentPsfFit = false;
				
				trackedObject->CurrentPsfFit->Fit(x, y, areaPixels, matrixSize);

				if (trackedObject->CurrentPsfFit->IsSolved() && trackedObject->CurrentPsfFit->Certainty() > MIN_CERTAINTY)
				{
					trackedObject->SetIsTracked(true, TrackedSuccessfully, trackedObject->CurrentPsfFit->XCenter(), trackedObject->CurrentPsfFit->YCenter(), trackedObject->CurrentPsfFit->Certainty());
					trackedObject->UseCurrentPsfFit = true;
				}
				else
					trackedObject->SetIsTracked(false, FullyDisappearingStarMarkedTrackedWithoutBeingFound, 0, 0, 0);
			}
		}
	}

	m_IsTrackedSuccessfully = atLeastOneObjectLocated;
}

int SimplifiedTracker::TrackerGetTargetState(int objectId, NativeTrackedObjectInfo* trackingInfo, NativePsfFitInfo* psfInfo, double* residuals)
{
	if (objectId < 0 || objectId >= m_NumTrackedObjects)
		return E_FAIL;
	
	TrackedObject* obj = m_TrackedObjects[objectId];
	
	trackingInfo->CenterXDouble = obj->CenterXDouble;
	trackingInfo->CenterYDouble = obj->CenterYDouble;
	trackingInfo->LastKnownGoodPositionXDouble = obj->LastKnownGoodPositionXDouble;
	trackingInfo->LastKnownGoodPositionYDouble = obj->LastKnownGoodPositionYDouble;
	trackingInfo->LastKnownGoodPsfCertainty = obj->LastKnownGoodPsfCertainty;
	trackingInfo->IsLocated = obj->IsLocated ? 1 : 0;
	trackingInfo->IsOffScreen = obj->IsOffScreen ? 1 : 0;
	trackingInfo->TrackingFlags = obj->TrackingFlags;
	
	PsfFit* psfFit = obj->CurrentPsfFit;
	if (obj->UseCurrentPsfFit)
	{
		psfInfo->FWHM = psfFit->FWHM();
		psfInfo->I0 = psfFit->I0();
		psfInfo->IMax = psfFit->IMax();
		psfInfo->IsAsymmetric = psfFit->FittingMethod == NonLinearAsymetricFit;
		psfInfo->IsSolved = psfFit->IsSolved();
		psfInfo->MatrixSize = psfFit->MatrixSize();
		psfInfo->R0 = psfInfo->IsAsymmetric ? psfFit->RX0 : psfFit->R0;
		psfInfo->R02 = psfInfo->IsAsymmetric ? psfFit->RY0 : 0;
		psfInfo->X0 = psfFit->X0();
		psfInfo->Y0 = psfFit->Y0();
		psfInfo->XCenter = psfFit->XCenter();
		psfInfo->YCenter = psfFit->YCenter();
		
		psfFit->CopyResiduals(residuals, psfInfo->MatrixSize);
	}
}


static SimplifiedTracker* s_Tracker;

int TrackerSettings(double maxElongation, double minFWHM, double maxFWHM, double minCertainty)
{
	MAX_ELONGATION = maxElongation;
	MIN_FWHM = minFWHM;
	MAX_FWHM = maxFWHM;
	MIN_CERTAINTY = minCertainty;
	
	if (NULL != s_Tracker)
		s_Tracker->UpdatePsfFittingMethod();
	
	return S_OK;
}

int TrackerNewConfiguration(int width, int height, int numTrackedObjects, bool isFullDisappearance, PSFFittingDataRange dataRange, unsigned int maxPixelValue)
{
	if (NULL != s_Tracker)
	{
		delete s_Tracker;
		s_Tracker = NULL;
	}
	
	s_Tracker = new SimplifiedTracker(width, height, numTrackedObjects, isFullDisappearance, dataRange, maxPixelValue);
	
	return S_OK;
}

int TrackerConfigureObject(int objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels)
{
	if (NULL != s_Tracker)
	{
		s_Tracker->ConfigureObject(objectId, isFixedAperture, isOccultedStar, startingX, startingY, apertureInPixels);
		return S_OK;
	}
	
	return E_POINTER;
}

int TrackerInitialiseNewTracking()
{
	if (NULL != s_Tracker)
	{
		s_Tracker->InitialiseNewTracking();
		return 0;
	}
	
	return -2;	
}

int TrackerNextFrame(int frameId, unsigned int* pixels)
{
	if (NULL != s_Tracker)
	{
		s_Tracker->NextFrame(frameId, pixels);
		return s_Tracker->IsTrackedSuccessfully() ? 0 : -1;
	}
	
	return -2;
}

int TrackerDoManualFrameCorrection(int objectId, int deltaX, int deltaY)
{
	if (NULL != s_Tracker) 
	{
		return s_Tracker->DoManualFrameCorrection(objectId, deltaX, deltaY) ? 0 : -1;
	}

	return -2;	
}
int TrackerGetTargetState(int objectId, NativeTrackedObjectInfo* trackingInfo, NativePsfFitInfo* psfInfo, double* residuals)
{
	if (NULL != s_Tracker)
	{
		return s_Tracker->TrackerGetTargetState(objectId, trackingInfo, psfInfo, residuals);
	}
	
	return E_POINTER;
}
