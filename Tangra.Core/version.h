#pragma once

#define VERSION_MAJOR 3
#define VERSION_MINOR 0
#define VERSION_REVISION 68

/*
 * ACTION PLAN TO SUPPORT 16bit data w 16bit images
 * 
GetBitmapPixels(..., int dataBpp, unsigned long normVal)

- if normVal is specified the 16bit pixel values are devided by this value to produce the 8-bit pixel
- if dataBpp is 8, 12 or 14 then: 

    8  - pixels are assumed to be 8 bit rather than 16 bit
	12 - pixels are shifted right so the saved 16 bit value is converted to 12 bit (normalisation)
	14 - pixels are shifted right so the saved 16 bit value is converted to 14 bit (normalisation)	
	
Used by: 
  
  * TangraCore.ADVGetFrame(...)
		* ADVGetFrame2(...)
		* Tangra3.GetPixelmap(...)
  
  * TangraCore.ADVGetIntegratedFrame(...)
  
  * TangraCore.ApplyPreProcessing(..., int bpp, unsigned long normVal, ...)
		* TangraVideo.AviFileGetFrame(...)
			* TangraVideo.TangraVideoGetFrame(...)
				* Tangra3.VideoStream.GetPixelmap(...)
		* TangraVideo.DirectShowGetFrame(...)
			* TangraVideo.TangraVideoGetFrame(...)
				* Tangra3.VideoStream.GetPixelmap(...)
				
  * TangraCore.ApplyPreProcessingWithNormalValue(..., int bpp, ...)  
		* TangraCore.ADVGetFrame()
		
  * TangraCore.ApplyPreProcessingPixelsOnly(..., int bpp)
        * TangraCore.ApplyPreProcessing() 		
		* TangraCore.ApplyPreProcessingWithNormalValue()
		* TangraCore.ADVGetIntegratedFrame()
		* Tangra3.SingleFITSFileStream.GetPixelmap()
		* TangraVideo.AviFileGetFramePixels(...)
			* TangraVideo.TangraVideoGetFramePixels(...)
				* Tangra3.GetFramePixels(..) -> NOT USED BY CLIENT CODE
		* TangraVideo.DirectShowGetFramePixels(...)
			* TangraVideo.TangraVideoGetFramePixels(...)
				* Tangra3.GetFramePixels(..) -> NOT USED BY CLIENT CODE
				
  * Tangra3.SingleFITSFileStream.GetPixelmap(...)
  * TangraVideo.AviGetIntegratedFrame(...)  <-- REVIEW THOSE, Add a TangraCore.GetBitmapPixels8Bit() OVERLOAD in Tangra.CORE
		* TangraVideo.TangraVideoGetIntegratedFrame(...)
			* Tangra3.VideoStream.GetIntegratedFrame(...)
  * TangraVideo.DirectShowGetIntegratedFrame(...) <-- REVIEW THOSE, Add a TangraCore.GetBitmapPixels8Bit() OVERLOAD in Tangra.CORE
		* TangraVideo.TangraVideoGetIntegratedFrame(...)
			* Tangra3.VideoStream.GetIntegratedFrame(...)
------------------------

Methods in TangraCore() that work with the encapsulated ADV file:
   TangraCore.ADVGetFrame()
		TangraCore.ApplyPreProcessingWithNormalValue()
		
   TangraCore.ADVGetIntegratedFrame()

Methods Not used:
	TangraCore.ApplyPreProcessing()  -> TO BE DELETED
	
Methods to be renamed:
	TangraCore.ApplyPreProcessing() --> TangraCore.ApplyPreProcessing8Bit()
	NOTE: Also remove the "normVal" parameter as it is not needed
	NOTE: Update the references in TangraVideo
	
Things that could break:

 - FITS file processing (Currenty not exposed as an option. Testing will be done when this is exposed) -> NO NEED TO TEST
 - 12bit ADV file recorded by ADVS
 - 14bit ADV file recorded by ADVS
 - 8bit ADV files recorded by Genika
 - 16bit ADV files recorded by Genika
 - 8bit AAV files
 - 16bit AAV files
 - 8bit AVI files (DirectShow)
 - 8bit AVI files (VideoForWindows)


   
   
*/