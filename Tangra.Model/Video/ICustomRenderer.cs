using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.VideoOperations;

namespace Tangra.Model.Video
{
    public interface ICustomRenderer
    {
        void ShowModal(IVideoController videoController);
    }
}
