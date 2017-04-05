using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Astrometry.Recognition;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.StarCatalogues;

namespace Tangra.Astrometry.Tests.PlateSolve
{
    public class MockedOperationNotifier : IOperationNotifier
    {
        public void Subscribe(INotificationReceiver receiver, Type notificationType)
        { }

        public void Unsubscribe(INotificationReceiver receiver)
        { }

        public void SendNotification(object notification)
        { }

        public void NotifyBeginLongOperation(string description)
        { }

        public void NotifyEndLongOperation()
        { }
    }

    public class PlateSolveTester
    {
        public void PlateSolve(PlateSolveTesterConfig config)
        {
            var pixelMap = new Pixelmap(config.Width, config.Height, config.BitPix, config.Pixels, null, null);
            var image = new AstroImage(pixelMap);

            StarMap starMap = new StarMap(
                TangraConfig.Settings.Astrometry.PyramidRemoveNonStellarObject,
                TangraConfig.Settings.Astrometry.MinReferenceStarFWHM,
                TangraConfig.Settings.Astrometry.MaxReferenceStarFWHM,
                TangraConfig.Settings.Astrometry.MaximumPSFElongation,
                TangraConfig.Settings.Astrometry.LimitReferenceStarDetection);

            Rectangle excludeRect = new Rectangle(config.OSDRectToExclude.X, config.OSDRectToExclude.Y, config.OSDRectToExclude.Width, config.OSDRectToExclude.Height);
            Rectangle includeRect = new Rectangle(config.RectToInclude.X, config.RectToInclude.Y, config.RectToInclude.Width, config.RectToInclude.Height);

            starMap.FindBestMap(
                StarMapInternalConfig.Default,
                image,
                excludeRect,
                includeRect,
                config.LimitByInclusion);

            var facade = new StarCatalogueFacade(TangraConfig.Settings.StarCatalogue);
            var catalogueStars = facade.GetStarsInRegion(
                config.RADeg,
                config.DEDeg,
                (config.ErrFoVs + 1.0) * config.PlateConfig.GetMaxFOVInArcSec() / 3600.0,
                config.LimitMagn,
                config.Epoch);

            var distBasedMatcher = new DistanceBasedAstrometrySolver(
                new MockedOperationNotifier(),
                config.PlateConfig, 
                TangraConfig.Settings.Astrometry,
                catalogueStars,
                config.RADeg,
                config.DEDeg,
                config.DetermineAutoLimitMagnitude);

            distBasedMatcher.SetMinMaxMagOfStarsForAstrometry(config.PyramidMinMag, config.LimitMagn);
            distBasedMatcher.SetMinMaxMagOfStarsForPyramidAlignment(config.PyramidMinMag, config.PyramidMaxMag);

            distBasedMatcher.InitNewMatch(starMap, PyramidMatchType.PlateSolve, null);


            distBasedMatcher.InitNewFrame(starMap);

            distBasedMatcher.SetManuallyIdentifiedHints(new Dictionary<PSFFit, IStar>());

            LeastSquareFittedAstrometry astrometricFit;
            PerformMatchResult result = distBasedMatcher.PerformMatch(out astrometricFit);
        }
    }
}
