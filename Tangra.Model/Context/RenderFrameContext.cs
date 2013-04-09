using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Video;

namespace Tangra.Model.Context
{
	public struct RenderFrameContext
	{
		public static RenderFrameContext Empty = new RenderFrameContext();
		public int CurrentFrameIndex;
		public MovementType MovementType;
		public bool IsLastFrame;
		public int MsToWait;
	    public int FirstFrameInIntegrationPeriod;

		public RenderFrameContext Clone()
		{
			RenderFrameContext clone = new RenderFrameContext();

			clone.CurrentFrameIndex = this.CurrentFrameIndex;
			clone.MovementType = this.MovementType;
			clone.IsLastFrame = this.IsLastFrame;
			clone.MsToWait = this.MsToWait;
		    clone.FirstFrameInIntegrationPeriod = this.FirstFrameInIntegrationPeriod;

			return clone;
		}
	}
}
