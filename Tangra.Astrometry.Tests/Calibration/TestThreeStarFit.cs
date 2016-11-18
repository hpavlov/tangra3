using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Tangra.Model.Astro;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.StarCatalogues;

namespace Tangra.Astrometry.Tests.Calibration
{
    [TestFixture]
    public class TestThreeStarFit
    {
        [Test]
        public void Test1()
        {
            var matrix = new CCDMatrix(8.6, 8.3, 752, 582);
            var astroPlate = new AstroPlate(matrix, 720, 576, 8);

            var userStars = new Dictionary<ImagePixel, IStar>();

            var star1 = new TestStar(2890001240, 18.528885242458674, -32.262447583319769, 11.033);
            var pixel1 = new ImagePixel(72.0519465443632, 240.48754416283302);
            userStars.Add(pixel1, star1);

            var star2 = new TestStar(2890001234, 18.353495385568369, -32.296976944037546, 12.294);
            var pixel2 = new ImagePixel(421.79863331879409, 329.57539665223919);
            userStars.Add(pixel2, star2);

            var star3 = new TestStar(2890001229, 18.284537781225755, -32.213242615932892, 10.882);
            var pixel3 = new ImagePixel(559.51676838260289, 114.86160161500557);
            userStars.Add(pixel3, star3);

            var plateSolve = DirectTransRotAstrometry.SolveByThreeStars(astroPlate, userStars, 2);

            Assert.IsNotNull(plateSolve);
            Assert.AreEqual(1.2836, plateSolve.Aspect, 0.0001);
            Assert.AreEqual(-32.2808, plateSolve.DE0Deg, 0.0001);
            Assert.AreEqual(18.3845, plateSolve.RA0Deg, 0.0001);
            Assert.AreEqual(179.9401, plateSolve.EtaDeg, 0.0001);
            Assert.AreEqual(0.00, plateSolve.Residual, 0.01);

            var newAstro = new ThreeStarAstrometry(astroPlate, userStars, 2);
            Assert.IsTrue(newAstro.Success);

            Assert.AreEqual(-32.2808, newAstro.DE0Deg, 0.0001);
            Assert.AreEqual(18.3845, newAstro.RA0Deg, 0.0001);

            double ra, de;
            newAstro.GetRADEFromImageCoords(72.0519465443632, 240.48754416283302, out ra, out de);
            Assert.AreEqual(18.5288852, ra, 0.0000001);
            Assert.AreEqual(-32.2624475, de, 0.0000001);

            double x, y;
            newAstro.GetImageCoordsFromRADE(18.528885242458674, -32.262447583319769, out x, out y);
            Assert.AreEqual(72.0519, x, 0.0001);
            Assert.AreEqual(240.4875, y, 0.0001);
        }

        [Test]
        public void Test2_NearSouthPole()
        {
            var matrix = new CCDMatrix(8.6, 8.3, 752, 582);
            var astroPlate = new AstroPlate(matrix, 720, 576, 16);

            var userStars = new Dictionary<ImagePixel, IStar>();

            var star1 = new TestStar(670000669, 14.040622402203727, -76.691539882008868, 13.389);
            var pixel1 = new ImagePixel(111.28789147012657, 170.18336583345945);
            userStars.Add(pixel1, star1);

            var star2 = new TestStar(680000642, 13.3447869927272, -76.594950217617452, 9.932);
            var pixel2 = new ImagePixel(575.00594900921817, 446.45890095859744);
            userStars.Add(pixel2, star2);

            var star3 = new TestStar(670000641, 13.550035599758042, -76.722167259223085, 13.842);
            var pixel3 = new ImagePixel(425.86138030460097, 63.057739094752051);
            userStars.Add(pixel3, star3);

            var newAstro = new ThreeStarAstrometry(astroPlate, userStars, 2);
            Assert.IsTrue(newAstro.Success);
            Assert.AreEqual(-76.6498, newAstro.DE0Deg, 0.0001);
            Assert.AreEqual(13.6644, newAstro.RA0Deg, 0.0001);

            double ra, de;
            newAstro.GetRADEFromImageCoords(111.28789147012657, 170.18336583345945, out ra, out de);
            Assert.AreEqual(14.0406224, ra, 0.0000001);
            Assert.AreEqual(-76.6915398, de, 0.0000001);

            double x, y;
            newAstro.GetImageCoordsFromRADE(14.040622402203727, -76.691539882008868, out x, out y);
            Assert.AreEqual(111.2879, x, 0.0001);
            Assert.AreEqual(170.1833, y, 0.0001);
        }
    }
}
