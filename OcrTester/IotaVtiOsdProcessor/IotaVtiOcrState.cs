using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Image;

namespace OcrTester.IotaVtiOsdProcessor
{
    public abstract class IotaVtiOcrState
    {
        protected int m_Width;
        protected int m_Height;

        protected int m_MinBlockWidth;
        protected int m_MaxBlockWidth;
        protected int m_MinBlockHeight;
        protected int m_MaxBlockHeight;

        public abstract void Process(IotaVtiOcrProcessor stateManager, Graphics g);
    }
}
