using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Services;
using System.Text;

namespace Tangra.Addins
{
	internal class AddinTrackingHandler : ITrackingHandler
	{
		// Notifies a handler that an object has been marshaled.
		public void MarshaledObject(Object obj, ObjRef or)
		{
			if (obj.GetType() != typeof(AppDomain))
				Trace.WriteLine(string.Format("Tangra Addins: Marshaled instance of {0} ({1} HashCode:{2})", or.TypeInfo != null ? or.TypeInfo.TypeName : obj.GetType().ToString(), or.URI != null ? or.URI.ToString() : "N/A", obj.GetHashCode().ToString()));
			else 
			{ 
				// Not interested in AppDomain marshalling
			}
		}

		// Notifies a handler that an object has been unmarshaled.
		public void UnmarshaledObject(Object obj, ObjRef or)
		{
			if (obj.GetType() != typeof(AppDomain))
				Trace.WriteLine(string.Format("Tangra Addins: Unmarshaled instance of {0} ({1} HashCode:{2})", or.TypeInfo != null ? or.TypeInfo.TypeName : obj.GetType().ToString(), or.URI != null ? or.URI.ToString() : "N/A", obj.GetHashCode().ToString()));
			else
			{
				// Not interested in AppDomain marshalling
			}
		}

		// Notifies a handler that an object has been disconnected.
		public void DisconnectedObject(Object obj)
		{
			if (obj.GetType() != typeof(AppDomain))
				Trace.WriteLine(string.Format("Tangra Addins: Disconnected instance of {0} (HashCode:{1})", obj.GetType().ToString(), obj.GetHashCode().ToString()));
			else
			{
				// Not interested in AppDomain marshalling
			}
		}

	}
}
