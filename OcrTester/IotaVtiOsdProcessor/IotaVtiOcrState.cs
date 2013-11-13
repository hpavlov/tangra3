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

        public abstract void InitialiseState(IotaVtiOcrProcessor stateManager);
        public abstract void FinaliseState(IotaVtiOcrProcessor stateManager);
        public abstract void Process(IotaVtiOcrProcessor stateManager, Graphics g);

        protected int GetDiffSignature(uint[] probe, uint[] etalon)
        {
            int rv = 0;

            for (int i = 0; i < probe.Length; i++)
            {
                if (etalon[i] != probe[i])
                    rv++;
            }

            return rv;
        }
    }

    public static class Extensions
    {
        public static T Median<T>(this IList<T> list)
        {
            if (list.Count == 0)
                return default(T);

            T[] arrayList = list.ToArray();
            Array.Sort(arrayList);

            return arrayList[list.Count / 2];
        }
    }
}
