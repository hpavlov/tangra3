using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Image;
using Tangra.PInvoke;
using Tangra.VideoOperations;

namespace Tangra.VideoTools
{
    public class MaskAreaSelector
    {
        private static Point m_SelectedCorner;
        private static List<Point> m_Corners = new List<Point>();
        private static double m_ClosestCornerDistance;

        public static void Initialize()
        {
            m_SelectedCorner = Point.Empty;
            IsPolygonDefined = false;
            m_Corners.Clear();
        }

        private static bool m_DraggingCorner;
        private static int m_InsertIndex = -1;
        private static int m_CornerIndexToMove = -1;

        public static void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Initialize();
                return;    
            }

            if (!IsPolygonDefined)
            {
                if (m_Corners.Count >= 4 && Distance(m_SelectedCorner, m_Corners[0]) < 5)
                    IsPolygonDefined = true;
                else
                {
                    m_SelectedCorner = e.Location;
                    m_Corners.Add(m_SelectedCorner);                    
                }
            }
            else
            {
                if (!m_DraggingCorner)
                {
                    if (m_CornerIndexToMove != -1)
                        m_DraggingCorner = true;
                    else if (m_InsertIndex != -1)
                    {
                        if (m_InsertIndex == m_Corners.Count)
                            m_Corners.Add(m_SelectedCorner);
                        else
                            m_Corners.Insert(m_InsertIndex, m_SelectedCorner);

                        m_CornerIndexToMove = m_InsertIndex;
                        m_DraggingCorner = true;
                    }
                }
            }
        }

        public static void OnMouseUp(MouseEventArgs e)
        {
            m_DraggingCorner = false;
            m_CornerIndexToMove = -1;
        }

        public static void OnMouseMove(MouseEventArgs e)
        {
            if (!IsPolygonDefined)
            {
                if (m_SelectedCorner != Point.Empty)
                    m_SelectedCorner = e.Location;
            }
            else if (!m_DraggingCorner)
            {
                int closestCornerIndex = -1;
                double closestDistance = double.MaxValue;
                for (int i = 0; i < m_Corners.Count; i++)
                {
                    var distance = Distance(m_Corners[i], e.Location);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCornerIndex = i;
                    }  
                }
                if (closestDistance < 5 && closestCornerIndex != -1)
                {
                    frmFullSizePreview.SetCursor(Cursors.Hand);
                    m_CornerIndexToMove = closestCornerIndex;
                    m_SelectedCorner = Point.Empty;
                }
                else
                {
                    frmFullSizePreview.SetCursor(Cursors.Default);
                    m_CornerIndexToMove = -1;
                    m_SelectedCorner = e.Location;
                }
                m_ClosestCornerDistance = closestDistance;
            }
            else if (m_DraggingCorner && m_CornerIndexToMove >= 0 && m_CornerIndexToMove < m_Corners.Count)
            {
                m_Corners[m_CornerIndexToMove] = e.Location;
            }

        }

        public static void OnPreviewKeyDown(KeyEventArgs e)
        {
            // Nothing special, just make sure we repaint so potential new corner can be drawn
        }

        private static double Distance(Point p1, Point p2)
        {
            var dx = p1.X - p2.X;
            var dy = p1.Y - p2.Y;
            return Math.Sqrt(dx*dx + dy*dy);
        }

        private static bool IsPolygonDefined { get; set; }

        private static Point ClosestPointToLine(Point a, Point b, Point p)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            var det = a.Y * b.X - a.X * b.Y;
            var dot = dx * p.X + dy * p.Y;
            var x = dot * dx + det * dy;
            var y = dot * dy - det * dx;
            var z = dx * dx + dy * dy;
            var zinv = 1.0 / z;

            return new Point((int)Math.Round(x * zinv), (int)Math.Round(y * zinv));
        }

        private static Tuple<int, int> FindPoligonIndexesToInsertNewCornerAt(Point location)
        {
            Tuple<int, int, Point>[] indexes = new Tuple<int, int, Point>[m_Corners.Count];
            double[] distances = new double[m_Corners.Count];
            Point cp;
            double cd;

            for (int i = 0; i < m_Corners.Count - 1; i++)
            {
                cp = ClosestPointToLine(m_Corners[i], m_Corners[i + 1], location);
                cd = Distance(cp, location);
                indexes[i] = Tuple.Create(i, i + 1, cp);
                distances[i] = cd;
            }

            cp = ClosestPointToLine(m_Corners.Last(), m_Corners.First(), location);
            cd = Distance(cp, location);
            indexes[m_Corners.Count - 1] = Tuple.Create(m_Corners.Count - 1, 0, cp);
            distances[m_Corners.Count - 1] = cd;

            Array.Sort(distances, indexes);

            var bestCandidate = indexes[0];
            var pt1 = m_Corners[bestCandidate.Item1];
            var pt2 = m_Corners[bestCandidate.Item2];

            var dist12 = Distance(pt1, pt2);
            var distL1 = Distance(location, pt1);
            var distL2 = Distance(location, pt2);

            var a1 = dist12 * dist12 + distL1 * distL1 - distL2 * distL2;
            var a2 = dist12 * dist12 + distL2 * distL2 - distL1 * distL1;

            if (a1 < 0)
            {
                if (bestCandidate.Item1 == 0)
                    return Tuple.Create(m_Corners.Count - 1, bestCandidate.Item1);
                else
                    return Tuple.Create(bestCandidate.Item1 - 1, bestCandidate.Item1);
            }
            else if (a2 < 0)
            {
                if (bestCandidate.Item2 == m_Corners.Count - 1)
                    return Tuple.Create(0, 1);
                else
                    return Tuple.Create(bestCandidate.Item2, bestCandidate.Item2 + 1);
            }
            else 
                return Tuple.Create(bestCandidate.Item1, bestCandidate.Item2);
            
        }

        private static Brush s_PolygonFillBrush = new SolidBrush(Color.FromArgb(10, 255, 165, 0));
        private static Pen s_TransparentPen = new Pen(Color.FromArgb(50, 255, 165, 0));

        public static void DrawOverlay(Graphics g)
        {
            Pen linePen = !IsPolygonDefined && m_Corners.Count > 0 && Distance(m_SelectedCorner, m_Corners[0]) < 5 ? Pens.Lime : Pens.Orange;

            if (IsPolygonDefined)
            {
                g.DrawPolygon(Pens.Orange, m_Corners.ToArray());
                g.FillPolygon(s_PolygonFillBrush, m_Corners.ToArray(), FillMode.Winding);
            }
            else if (m_Corners.Count > 1)
            {
                for (int i = 0; i < m_Corners.Count - 1; i++)
                {
                    g.DrawLine(linePen, m_Corners[i], m_Corners[i + 1]);
                }
            }

            if (m_SelectedCorner != Point.Empty)
            {
                bool shiftHeld = Control.ModifierKeys == Keys.Shift;

                if (!IsPolygonDefined)
                {
                    g.DrawLine(linePen, m_SelectedCorner, m_Corners.Last());
                }
                else if (m_ClosestCornerDistance > 10 && shiftHeld)
                {
                    var loc = FindPoligonIndexesToInsertNewCornerAt(m_SelectedCorner);

                    g.FillEllipse(s_PolygonFillBrush, m_SelectedCorner.X - 3, m_SelectedCorner.Y - 3, 6, 6);
                    g.DrawLine(s_TransparentPen, m_Corners[loc.Item1], m_SelectedCorner);
                    g.DrawLine(s_TransparentPen, m_Corners[loc.Item2], m_SelectedCorner);

                    m_InsertIndex = loc.Item1 + 1;
                }
            }

            for (int i = 0; i < m_Corners.Count; i++)
            {
                g.FillEllipse(Brushes.Orange, m_Corners[i].X - 3, m_Corners[i].Y - 3, 6, 6);
            }

            if (m_CornerIndexToMove != -1 && m_Corners.Count > m_CornerIndexToMove)
            {
                g.FillEllipse(Brushes.Lime, m_Corners[m_CornerIndexToMove].X - 3, m_Corners[m_CornerIndexToMove].Y - 3, 6, 6);
            }
        }

        public static void ReconfigurePreProcessing()
        {
            if (IsPolygonDefined)
            {
                uint[] polyPointsX = m_Corners.Select(x => (uint)x.X).ToArray();
                uint[] polyPointsY = m_Corners.Select(x => (uint)x.Y).ToArray();

                TangraCore.PreProcessors.PreProcessingDefineMaskArea(polyPointsX.Length, polyPointsX, polyPointsY, 0 /*Pass median instead ?*/);
            }
            else
                TangraCore.PreProcessors.PreProcessingDefineMaskArea(0, new uint[0], new uint[0], 0);
        }
    }
}
