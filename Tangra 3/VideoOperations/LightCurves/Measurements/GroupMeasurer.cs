/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.LightCurves.Measurements
{
	internal class MeasurmentGroup
	{
		private float m_FWHM;
		private List<TrackedObjectConfig> m_ObjectsInGroup = new List<TrackedObjectConfig>();
		private List<int> m_ObjectIds = new List<int>();

		public MeasurmentGroup(float fwhm, TrackedObjectConfig firstObjectInGroup, int targetNo)
		{
			m_FWHM = fwhm;
			AddObjectInGroup(firstObjectInGroup, targetNo);
		}

		public void AddObjectInGroup(TrackedObjectConfig objectToAdd, int targetNo)
		{
			if (m_ObjectIds.IndexOf(targetNo) == -1)
			{
				m_ObjectsInGroup.Add(objectToAdd);
				m_ObjectIds.Add(targetNo);
			}
		}

		public bool IsCloseEnoughToBelongToTheGroup(TrackedObjectConfig objectToCheck)
		{
			foreach (TrackedObjectConfig existingObject in m_ObjectsInGroup)
			{
				double distance =
					ImagePixel.ComputeDistance(existingObject.ApertureStartingX, objectToCheck.ApertureStartingX,
											   existingObject.ApertureStartingY, objectToCheck.ApertureStartingY);

				if (distance < m_FWHM * TangraConfig.Settings.Special.MinDistanceForPhotometricGroupingInFWHM)
				{
					return true;
				}
			}

			return false;
		}
	}

	internal class GroupMeasurer
	{
		private List<TrackedObjectConfig> m_TrackedObjects;
		private float m_FWHM;
		private List<MeasurmentGroup> m_MeasurementGroups = new List<MeasurmentGroup>();

		public GroupMeasurer(float fwhm, List<TrackedObjectConfig> trackedObjects, MeasurementsHelper measurementsHelper)
		{
			m_FWHM = fwhm;
			m_TrackedObjects = trackedObjects;

			GroupObjects();
		}

		private void GroupObjects()
		{
			List<int> objectsAlreadyInAGroup = new List<int>();

			for (int i = 0; i < m_TrackedObjects.Count; i++)
			{
				TrackedObjectConfig obj = m_TrackedObjects[i];
				MeasurmentGroup currentGroup = null;

				if (objectsAlreadyInAGroup.IndexOf(i) == -1)
				{
					currentGroup = new MeasurmentGroup(m_FWHM, obj, i);
					objectsAlreadyInAGroup.Add(i);
					m_MeasurementGroups.Add(currentGroup);
				}
				else
					continue;

				if (currentGroup != null)
				{
					for (int j = i + 1; j < m_TrackedObjects.Count; j++)
					{
						if (currentGroup.IsCloseEnoughToBelongToTheGroup(m_TrackedObjects[j]))
						{
							currentGroup.AddObjectInGroup(m_TrackedObjects[j], j);
							objectsAlreadyInAGroup.Add(j);
						}
					}
				}
			}
		}

		public bool HasGroups
		{
			get
			{
				return m_MeasurementGroups.Count < m_TrackedObjects.Count;
			}
		}
	}
}
