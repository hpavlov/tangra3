using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Tangra.SDK
{

	public enum AddinFiredEventType
	{
		Unknown = 0,
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
		
		void InitializeAddin(ITangraApplication application);
        void FinalizeAddin();
		void OnEventNotification(AddinFiredEventType eventType);
	}

	public enum AddinActionType
	{
		LightCurve,
		Astrometry,
		Generic
	}

	public interface ITangraAddinAction
	{
		string DisplayName { get; }
		AddinActionType ActionType { get; }
		Bitmap Icon { get; }
		Color IconTransparentColor { get; }

		void Execute();

		void InitializeAction(ITangraApplication application);
		void FinalizeAction();
	}
}
