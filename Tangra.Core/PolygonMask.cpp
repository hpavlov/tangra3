/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "PolygonMask.h"


// Polygon fill code based on http://alienryderflex.com/polygon/
// Soyrce code by Darel Rex Finley with modifications from Patrick Mullen 

//  Globals which should be set before calling these functions:
//
//  int    polyCorners  =  how many corners the polygon has (no repeats)
//  float  polyX[]      =  horizontal coordinates of corners
//  float  polyY[]      =  vertical coordinates of corners
//  float  x, y         =  point to be tested
//
//  The following global arrays should be allocated before calling these functions:
//
//  float  constant[] = storage for precalculated constants (same size as polyX)
//  float  multiple[] = storage for precalculated multipliers (same size as polyX)
//
//  (Globals are used in this example for purposes of speed.  Change as
//  desired.)
//
//  USAGE:
//  Call precalc_values() to initialize the constant[] and multiple[] arrays,
//  then call pointInPolygon(x, y) to determine if the point is in the polygon.
//
//  The function will return YES if the point x,y is inside the polygon, or
//  NO if it is not.  If the point is exactly on the edge of the polygon,
//  then the function may return YES or NO.
//
//  Note that division by zero is avoided because the division is protected
//  by the "if" clause which surrounds it.

float s_Constant[200];
float s_Multiple[200];

void precalc_values(int polyCorners, float* polyX, float* polyY) 
{
  int i, j=polyCorners-1;

  for(i=0; i<polyCorners; i++) 
  {
    if(polyY[j]==polyY[i]) 
	{
      s_Constant[i]=polyX[i];
      s_Multiple[i]=0; 
	}
    else 
	{
      s_Constant[i]=polyX[i]-(polyY[i]*polyX[j])/(polyY[j]-polyY[i])+(polyY[i]*polyX[i])/(polyY[j]-polyY[i]);
      s_Multiple[i]=(polyX[j]-polyX[i])/(polyY[j]-polyY[i]); 
	}
    j=i; 
  }
}

bool pointInPolygon(int polyCorners, float* polyY, float x, float y) 
{
  int i, j = polyCorners-1;
  bool oddNodes = false;

  for (i=0; i<polyCorners; i++) 
  {
    if ((polyY[i]< y && polyY[j]>=y || polyY[j]< y && polyY[i]>=y)) 
	{
      oddNodes^=(y*s_Multiple[i]+s_Constant[i]<x); 
	}
    j=i; 
  }

  return oddNodes; 
}

HRESULT PreProcessingMaskOutArea(unsigned int* pixels, int width, int height, unsigned int fillColour, unsigned int numCorners, unsigned int* xPos, unsigned int* yPos)
{
	int minX = width;
	int maxX = 0;
	int minY = height;
	int maxY = 0;
	
	float xPosF[numCorners];
	float yPosF[numCorners];
	
	for (int i = 0; i < numCorners; i++)
	{
		if (xPos[i] > maxX) maxX = xPos[i];
		if (xPos[i] < minX) minX = xPos[i];
		if (yPos[i] > maxY) maxY = yPos[i];
		if (yPos[i] < minY) minY = yPos[i];
		
		xPosF[i] = (float)xPos[i];
		yPosF[i] = (float)yPos[i];
	}

	precalc_values(numCorners, xPosF, yPosF);
	
	for (int y = minY; y <= maxY; y++)
	{
		for (int x = minX; x <= maxX; x++)
		{
			if (pointInPolygon(numCorners, yPosF, (float)x, (float)y))
				pixels[y * width + x] = fillColour;
		}
	}
	
	return S_OK;
}
