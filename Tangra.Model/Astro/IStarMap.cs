using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;

namespace Tangra.Model.Astro
{
	public interface IStarMap
	{
		uint NoiseLevel { get; }

		int FeaturesCount { get; }

		int FindBestMap(StarMapInternalConfig config, AstroImage image, Rectangle osdFrameToExclude, Rectangle frameToInclude, bool limitByInclusion);
		void InitializeStarMapButDontProcess(StarMapInternalConfig config, AstroImage image, Rectangle osdFrameToExclude, Rectangle frameToInclude, bool limitByInclusion);

		List<StarMapFeature> Features { get; }
		StarMapFeature GetFeatureById(int feautreId);
		StarMapFeature this[int x, int y] { get; }

		StarMapFeature GetFeatureInRadius(int x, int y, int radius);
		ImagePixel GetCentroid(int x, int y, int radius);
		ImagePixel GetPSFFit(int x, int y, PSFFittingMethod method);
		ImagePixel GetPSFFit(int x, int y, PSFFittingMethod method, out PSFFit psfFit);
		ImagePixel GetPSFFit(int x, int y, int fitMatrixSize, out PSFFit psfFit);

		uint[,] GetImageArea(int x, int y, int width);
	}
}
