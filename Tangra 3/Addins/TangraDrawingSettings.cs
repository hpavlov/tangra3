using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.SDK;

namespace Tangra.Addins
{
    [Serializable]
    public class TangraDrawingSettings : ITangraDrawingSettings
    {
        public TangraDrawingSettings(TangraConfig.LightCurvesDisplaySettings settings)
        {
            Target1Color = settings.Target1Color;
            Target2Color = settings.Target2Color;
            Target3Color = settings.Target3Color;
            Target4Color = settings.Target4Color;
        }

        public Color Target1Color { get; private set; }
        public Color Target2Color { get; private set; }
        public Color Target3Color { get; private set; }
        public Color Target4Color { get; private set; }
    }
}
