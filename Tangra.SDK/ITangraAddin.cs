/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Tangra.SDK
{
	public enum AddinFiredEventType
	{
		Invalid = 0,
		LightCurveDataRecomputed = 1,
		LightCurveSelectedFrameChanged = 2,
		BeginMultiFrameAstrometry = 3,
		EndMultiFrameAstrometry = 4
	}

	public interface ITangraAddin
	{
		void Configure();

		string DisplayName { get; }
		string Author { get; }
		string Version { get; }
		string Description { get; }
		string Url { get; }

		ITangraAddinAction[] GetAddinActions();

		void Initialise(ITangraHost host);
		void Finalise();

		void OnEventNotification(AddinFiredEventType eventType);
	}

	public enum AddinActionType
	{
		LightCurve = 0,
		LightCurveEventTimeExtractor = 1,
		LightCurveEventTimeExtractorSupportsBinning = 2,
		Astrometry = 3,
		Generic = 4,
        OcrEngine = 5
	}

	public interface ITangraAddinAction
	{
		string DisplayName { get; }
		AddinActionType ActionType { get; }
		IntPtr Icon { get; }
		int IconTransparentColorARGB { get; }

		void Execute();
	    void Finalise();
	}
}
