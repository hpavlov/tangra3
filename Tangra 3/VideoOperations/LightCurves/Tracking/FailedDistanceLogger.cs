using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
	internal class FailedDistanceLogger
	{
		bool[] okayDistanceChecks = new bool[6] { true, true, true, true, true, true };
		private int numTrackedObjects;

		public FailedDistanceLogger(int numTrackedObjects)
		{
			this.numTrackedObjects = numTrackedObjects;
		}

		private int GetFailedDistanceArrayIndex(int i, int j)
		{
			if (i > j)
			{
				int tmp = i;
				i = j;
				j = tmp;
			}

			// ------------------------------------
			//   0     1     2     3     4     5     
			// (0,1) (0,2) (0,3) (1,2) (1,3) (2,3) 
			// ------------------------------------

			if (i == 0)
			{
				switch (j)
				{
					case 1:
						return 0;
					case 2:
						return 1;
					case 3:
						return 2;
					default:
						return -1;
				}
			}
			else if (i == 1)
			{
				switch (j)
				{
					case 2:
						return 3;
					case 3:
						return 4;
					default:
						return -1;
				}
			}
			else if (i == 2)
			{
				switch (j)
				{
					case 3:
						return 5;
					default:
						return -1;
				}
			}
			else if (i == 3)
			{
				return -1;
			}

			return -1;
		}

		public void MarkFailedDistanceCheck(int index1, int index2)
		{
			int flagIdx = GetFailedDistanceArrayIndex(index1, index2);
			okayDistanceChecks[flagIdx] = false;			
		}

		public void MarkFailedDistanceCheckForObject(int index)
		{
			switch (index)
			{
				case 0:
					okayDistanceChecks[0] = okayDistanceChecks[1] = okayDistanceChecks[2] = false;
					break;
				case 1:
					okayDistanceChecks[0] = okayDistanceChecks[3] = okayDistanceChecks[4] = false;
					break;
				case 2:
					okayDistanceChecks[1] = okayDistanceChecks[3] = okayDistanceChecks[5] = false;
					break;
				case 3:
					okayDistanceChecks[3] = okayDistanceChecks[4] = okayDistanceChecks[5] = false;
					break;
			}
		}

		public bool AreDistancesOkay(int trackedObjectNo)
		{
			if (numTrackedObjects == 1)
				// Tracking only a single object so distances are not checked. All assumed okay in terms of distances
				return true;

			// ------------------------------------
			//   0     1     2     3     4     5     
			// (0,1) (0,2) (0,3) (1,2) (1,3) (2,3) 
			// ------------------------------------

			// Distances are okay if the distances to at least one other object are okay
			if (trackedObjectNo == 0)
			{
				return okayDistanceChecks[0] || okayDistanceChecks[1] || okayDistanceChecks[2];
			}
			else if (trackedObjectNo == 1)
			{
				return okayDistanceChecks[0] || okayDistanceChecks[3] || okayDistanceChecks[4];
			}
			else if (trackedObjectNo == 2)
			{
				return okayDistanceChecks[1] || okayDistanceChecks[3] || okayDistanceChecks[5];
			}
			else if (trackedObjectNo == 3)
			{
				return okayDistanceChecks[3] || okayDistanceChecks[4] || okayDistanceChecks[5];
			}

			return false;
		}

		public bool AllDistancesOkay()
		{
			return 
				okayDistanceChecks[0] && okayDistanceChecks[1] &&
			    okayDistanceChecks[2] && okayDistanceChecks[3] &&
			     okayDistanceChecks[4] && okayDistanceChecks[5];
		}
	}
}
