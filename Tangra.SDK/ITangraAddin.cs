using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tangra.SDK
{
    public interface ITangraAddin
    {
	    void Initialise(ITangraHost host);
		void Finalise();
    }
}
