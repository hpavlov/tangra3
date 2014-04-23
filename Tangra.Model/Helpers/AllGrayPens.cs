using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Tangra.Model.Helpers
{
    public class AllGrayPens
    {
        private static List<Pen> s_AllPens = new List<Pen>();
        private static object s_SyncRoot = new object();

        static AllGrayPens()
        {
            lock (s_SyncRoot)
            {
                for (int i = 0; i < 256; i++)
                {
                    s_AllPens.Add(new Pen(Color.FromArgb(i, i, i)));
                }
            }
        }

        public static Pen GrayPen(byte val)
        {
            return s_AllPens[val];
        }
    }

    public class AllGrayBrushes
    {
        private static List<Brush> s_AllPens = new List<Brush>();
        private static object s_SyncRoot = new object();

        static AllGrayBrushes()
        {
            lock (s_SyncRoot)
            {
                for (int i = 0; i < 256; i++)
                {
                    s_AllPens.Add(new SolidBrush(Color.FromArgb(i, i, i)));
                }
            }
        }

        public static Brush GrayBrush(byte val)
        {
            return s_AllPens[val];
        }
    }

    public class AllGrayColors
    {
        private static List<Color> s_AllColor = new List<Color>();
        private static object s_SyncRoot = new object();

        static AllGrayColors()
        {
            lock (s_SyncRoot)
            {
                for (int i = 0; i < 256; i++)
                {
                    s_AllColor.Add(Color.FromArgb(i, i, i));
                }
            }
        }

        public static Color GrayColor(byte val)
        {
            return s_AllColor[val];
        }
    }
}
