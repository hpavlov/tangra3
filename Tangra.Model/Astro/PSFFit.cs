using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Numerical;

namespace Tangra.Model.Astro
{
    public enum PSFFittingMethod
    {
        NonLinearFit = 0,
        LinearFitOfAveragedModel,
		NonLinearAsymetricFit
    }

	public enum PSFFittingDataRange
	{
		DataRange8Bit,
		DataRange12Bit,
		DataRange14Bit,
		DataRange16Bit
	}

	public interface ITrackedObjectPsfFit
	{
		double XCenter { get; }
		double YCenter { get; }
		double FWHM { get; }
		double IMax { get; }
		double I0 { get; }
		double X0 { get; }
		double Y0 { get; }
		int MatrixSize { get; }
		bool IsSolved { get; }

		double GetPSFValueAt(double x, double y);
		double GetResidualAt(int x, int y);

		void DrawDataPixels(Graphics g, Rectangle rect, float aperture, Pen aperturePen, int bpp, uint normVal);
		void DrawGraph(Graphics g, Rectangle rect, int bpp, uint normVal);
	}

    public interface IPSFFit
    {
		void DrawGraph(Graphics g, Rectangle rect, int bpp, uint normVal);
		void DrawDataPixels(Graphics g, Rectangle rect, int bpp, uint normVal);
        int MatrixSize { get; }
    }

	public abstract class PSFFitBase
	{
		public abstract void DrawInternalPoints(Graphics g, Rectangle rect, float aperture, Brush incldedPinBrush, int bpp);
		public abstract double FWHM { get; }
		public abstract float X0_Matrix { get; }
		public abstract float Y0_Matrix { get; }
	}

	public class PSFFit : PSFFitBase, IPSFFit, ITrackedObjectPsfFit
    {
        private static double CERTAINTY_CONST = 0.5 / 0.03;

        private static int NumberIterations = 10;
        private int m_xCenter;
        private int m_yCenter;

        private bool m_IsSolved = false;
        private double m_IBackground = double.NaN;
        private double m_IStarMax = double.NaN;
        private double m_X0 = double.NaN;
        private double m_Y0 = double.NaN;
        private double m_R0 = double.NaN;
		private double m_RX0 = double.NaN;
		private double m_RY0 = double.NaN;
        private double[,] m_Residuals;
        private int m_MatrixSize = 9;
        private int m_HalfWidth = 4;
		private uint m_Saturation = 256;

        public readonly int UniqueId;

        private static int s_Sequence = 0;
        private static object s_SyncRoot = new object();

        public PSFFittingMethod FittingMethod;

        private float m_ModelFWHM;

		private IBackgroundModelProvider m_BackgroundModel;
		private bool m_UsesBackgroundModel = false;

		public static PSFFittingDataRange DataRange = PSFFittingDataRange.DataRange8Bit;

		public static uint NormVal = 255;

        public void SetAveragedModelFWHM(float modelFWHM)
        {
            m_ModelFWHM = modelFWHM;
        }

        private PSFFit()
        {
            lock(s_SyncRoot)
            {
                UniqueId = s_Sequence;
                s_Sequence++;
            }
        }

        private PSFFit(int uniqueId)
            : base()
        {
            // Overwrite the unique id if already specified
            if (uniqueId != -1) UniqueId = uniqueId;
        }

        public PSFFit(int xCenter, int yCenter)
            : this()
        {
            m_xCenter = xCenter;
            m_yCenter = yCenter;
        }

        public void SetNewFieldCenterFrom17PixMatrix(int x, int y)
        {
            m_xCenter = x + m_xCenter - 8;
            m_yCenter = y + m_yCenter - 8;  
        }

        public void SetNewFieldCenterFrom35PixMatrix(int x, int y)
        {
            m_xCenter = x + m_xCenter - 17;
            m_yCenter = y + m_yCenter - 17;  
        }

        public bool IsSolved
        {
            get { return m_IsSolved; }
        }

        public double GetValue(double x, double y)
        {
            return GetPSFValueInternal(x, y);
        }

        public double XCenter
        {
            get { return m_xCenter + m_X0 - m_HalfWidth; }
        }

        public double YCenter
        {
            get { return m_yCenter + m_Y0 - m_HalfWidth; }
        }

		public bool UsesBackgroundModel
		{
			get { return m_UsesBackgroundModel; }
		}

        public int MatrixSize
        {
            get { return m_MatrixSize; }
        }

        public int X0
        {
            get
            {
                int xInx = (int) Math.Round(m_X0);
                if (xInx < 0) xInx = 0;
                if (xInx >= m_MatrixSize) xInx = m_MatrixSize - 1;

                return xInx;
            }
        }

        public int Y0
        {
            get
            {
                int yInx = (int)Math.Round(m_Y0);
                if (yInx < 0) yInx = 0;
                if (yInx >= m_MatrixSize) yInx = m_MatrixSize - 1;

                return yInx;
            }
        }

		double ITrackedObjectPsfFit.X0
		{
			get { return m_X0; }
		}

		double ITrackedObjectPsfFit.Y0
		{
			get { return m_Y0; }
		}

    	public override float X0_Matrix
    	{
			get { return (float)m_X0; }
    	}

        public override float Y0_Matrix
		{
			get { return (float)m_Y0; }
		}

		public double R0
		{
			get { return m_R0; }
            set { m_R0 = value; }
		}

		public double RX0
		{
			get { return m_RX0; }
			set { m_RX0 = value; }
		}

		public double RY0
		{
			get { return m_RY0; }
			set { m_RY0 = value; }
		}

		public double I0
		{
			get { return m_IBackground; }
		}

        public double IMax
        {
            get { return m_IBackground + m_IStarMax; }
        }

    	public int Brightness
    	{
    		get
    		{
				return (int)Math.Round(IMax - I0);
    		}
    	}

    	public override double FWHM
    	{
    		get
    		{
                //return 2 * Math.Sqrt(-0.5 * Math.Log(0.5) * m_R0 * m_R0);
				if (FittingMethod == PSFFittingMethod.NonLinearAsymetricFit)
					return 2 * Math.Sqrt(-0.5 * Math.Log(0.5) * m_RX0 * m_RY0);
				else
					return 2 * Math.Sqrt(Math.Log(2)) * m_R0;
    		}
    	}

		public double Certainty
		{

			get
			{
				// 0.03 = 0.5
				return CERTAINTY_CONST * (m_IStarMax / (m_Saturation - I0)) / (5 * Math.Log10(FWHM));
			}
		}

    	public double ElongationPercentage
    	{
    		get
    		{
				if (FittingMethod == PSFFittingMethod.NonLinearAsymetricFit)
					return Math.Abs(1 - (RX0 / RY0)) * 100;
				else
					return 0;
    		}
    	}

        public double[,] Residuals
        {
            get { return m_Residuals; }
        }

        public double GetVariance()
        {
            double sumSq = 0;
            int width = m_Residuals.GetLength(0);
            int height = m_Residuals.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    sumSq += m_Residuals[x, y] * m_Residuals[x, y];
                }
            }

            return Math.Sqrt(sumSq / (width * height));
        }

        internal int m_NewMatrixSize, m_NewMatrixX0, m_NewMatrixY0;
		public void Fit(uint[,] intensity, int newMatrixSize, int x0, int y0, bool zeroEdge)
        {
            m_NewMatrixSize = newMatrixSize;
            m_NewMatrixX0 = x0;
            m_NewMatrixY0 = y0;

			uint[,] newData = new uint[newMatrixSize, newMatrixSize];
            int halfSize = newMatrixSize / 2;

            int to = intensity.GetLength(0);
            int from = 0;

            if (zeroEdge)
            {
                to--;
                from++;
            }

            try
            {
                int xx = 0, yy = 0;
                for (int y = y0 - halfSize; y <= y0 + halfSize; y++)
                {
                    xx = 0;
                    for (int x = x0 - halfSize; x <= x0 + halfSize; x++)
                    {
                        if (x >= from && x < to && y >= from && y < to)
                        {
                            newData[xx, yy] = intensity[x, y];
                        }

                        xx++;
                    }
                    yy++;
                }

                Fit(newData);
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                m_IsSolved = false;
            }
        }

		public void Fit(uint[,] intensity, int newMatrixSize)
		{
			Fit(intensity, null, newMatrixSize);
		}

		public void Fit(uint[,] intensity, IBackgroundModelProvider backgroundModel, int newMatrixSize)
		{
			m_BackgroundModel = backgroundModel;
			m_UsesBackgroundModel = m_BackgroundModel != null;

            if (newMatrixSize == 17 || newMatrixSize == 35 || newMatrixSize == 0)
			{
				Fit(intensity);
			}
			else if (newMatrixSize < 17 && intensity.GetLength(0) == 17)
			{
				uint[,] newData = new uint[newMatrixSize,newMatrixSize];
				int halfSize = newMatrixSize/2;

				int xx = 0, yy = 0;
				for (int y = 9 - halfSize - 1; y <= 9 + halfSize - 1; y++)
				{
					xx = 0;
					for (int x = 9 - halfSize - 1; x <= 9 + halfSize - 1; x++)
					{
						newData[xx, yy] = intensity[x, y];
						xx++;
					}
					yy++;
				}

				Fit(newData);
			}
			else if (newMatrixSize < 35 && intensity.GetLength(0) == 35)
			{
				uint[,] newData = new uint[newMatrixSize, newMatrixSize];
				int halfSize = newMatrixSize / 2;

				int xx = 0, yy = 0;
				for (int y = 18 - halfSize - 1; y <= 18 + halfSize - 1; y++)
				{
					xx = 0;
					for (int x = 18 - halfSize - 1; x <= 18 + halfSize - 1; x++)
					{
						newData[xx, yy] = intensity[x, y];
						xx++;
					}
					yy++;
				}

				Fit(newData);
			}
			else
				throw new InvalidOperationException("Bad matrix size.");
		}

#if WIN32
		public void Fit(uint[,] intensity)
        {
            try
            {
	            if (FittingMethod == PSFFittingMethod.NonLinearFit)
	            {
					if (TangraConfig.Settings.Tuning.PsfMode == TangraConfig.PSFFittingMode.FullyNative)
						NonLinearFitNative(intensity);
					else
						NonLinearFit(intensity, TangraConfig.Settings.Tuning.PsfMode == TangraConfig.PSFFittingMode.NativeMatrixManagedFitting);
	            }
	            else if (FittingMethod == PSFFittingMethod.NonLinearAsymetricFit)
					NonLinearAsymetricFit(intensity, TangraConfig.Settings.Tuning.PsfMode == TangraConfig.PSFFittingMode.NativeMatrixManagedFitting);
	            else if (FittingMethod == PSFFittingMethod.LinearFitOfAveragedModel)
		            LinearFitOfAveragedModel(intensity);
            }
            catch(Exception)
            {
                // singular matrix, etc
                m_IsSolved = false;
            }
        }
#else
        // NOTE: Native PSF Fitting for now is not supported on non Windows Platforms

        public void Fit(uint[,] intensity)
        {
            try
            {
                if (FittingMethod == PSFFittingMethod.NonLinearFit)
                {
                    NonLinearFit(intensity, false);
                }
                else if (FittingMethod == PSFFittingMethod.NonLinearAsymetricFit)
                    NonLinearAsymetricFit(intensity, false);
                else if (FittingMethod == PSFFittingMethod.LinearFitOfAveragedModel)
                    LinearFitOfAveragedModel(intensity);
            }
            catch (Exception)
            {
                // singular matrix, etc
                m_IsSolved = false;
            }
        }

#endif

        private void NonLinearFitNative(uint[,] intensity)
		{
			int full_width = (int)Math.Round(Math.Sqrt(intensity.Length));
			m_MatrixSize = full_width;
			m_HalfWidth = full_width / 2;

			switch (DataRange)
			{
				case PSFFittingDataRange.DataRange8Bit:
					m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation8Bit;
					break;

				case PSFFittingDataRange.DataRange12Bit:
					m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation12Bit;
					break;

				case PSFFittingDataRange.DataRange14Bit:
					m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation14Bit;
					break;

				case PSFFittingDataRange.DataRange16Bit:
					m_Saturation = NormVal > 0
							? (uint)(TangraConfig.Settings.Photometry.Saturation.Saturation8Bit * NormVal / 255.0)
							: TangraConfig.Settings.Photometry.Saturation.Saturation16Bit;
					break;

				default:
					m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation8Bit;
					break;
			}

			bool isSolved = false;
			double iBackground = double.NaN;
			double iStarMax = double.NaN;
			double x0 = double.NaN;
			double y0 = double.NaN;
			double r0 = double.NaN;
			double[] residuals = new double[m_MatrixSize * m_MatrixSize];
			m_Residuals = new double[m_MatrixSize, m_MatrixSize];
			uint[] intensityLine = new uint[intensity.Length];
			for (int y = 0; y < intensity.GetLength(1); y++)
			{
				for (int x = 0; x < intensity.GetLength(0); x++)
				{
					if (m_BackgroundModel != null)
						intensityLine[x + y*intensity.GetLength(1)] = (uint)Math.Round(intensity[x, y] - m_BackgroundModel.ComputeValue(x, y));
					else
						intensityLine[x + y*intensity.GetLength(1)] = intensity[x, y];
				}
			}

			TangraModelCore.EnsureBuffers(m_MatrixSize, 5);
			TangraModelCore.DoNonLinearPfsFit(intensityLine, m_MatrixSize, (int)m_Saturation, ref isSolved, ref iBackground, ref iStarMax, ref x0, ref y0, ref r0, residuals);

			m_IsSolved = isSolved;
			m_IBackground = iBackground;
			m_IStarMax = iStarMax;
			m_X0 = x0;
			m_Y0 = y0;
			m_R0 = r0;

			for (int y = 0; y < intensity.GetLength(1); y++)
			{
				for (int x = 0; x < intensity.GetLength(0); x++)
				{
					m_Residuals[x, y] = residuals[x + y * intensity.GetLength(1)];
				}
			}
		}

        // I(x, y) = IBackground + IStarMax * Exp ( -((x - X0)*(x - X0) + (y - Y0)*(y - Y0)) / (r0 * r0))
		private void NonLinearFit(uint[,] intensity, bool useNativeMatrix)
		{
            m_IsSolved = false;

            try
            {
                int full_width = (int) Math.Round(Math.Sqrt(intensity.Length));

                m_MatrixSize = full_width;

                int half_width = full_width/2;

                m_HalfWidth = half_width;

				switch (DataRange)
				{
					case PSFFittingDataRange.DataRange8Bit:
						m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation8Bit;
						break;

					case PSFFittingDataRange.DataRange12Bit:
						m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation12Bit;
						break;

					case PSFFittingDataRange.DataRange14Bit:
						m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation14Bit;
						break;

					case PSFFittingDataRange.DataRange16Bit:
						m_Saturation = NormVal > 0
							? (uint)(TangraConfig.Settings.Photometry.Saturation.Saturation8Bit * NormVal / 255.0)
							: TangraConfig.Settings.Photometry.Saturation.Saturation16Bit;
						break;

					default:
						m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation8Bit;
						break;
				}

                int nonSaturatedPixels = 0;

                double IBackground = 0;
                double r0 = 4.0;
                double IStarMax = 0;

                double found_x = half_width;
                double found_y = half_width;

                for (int iter = NumberIterations; iter > 0; iter--)
                {
                    if (iter == NumberIterations)
                    {
						uint zval = 0;

                        IBackground = 0.0; /* assume no backgnd intensity at first... */
                        for (int i = 0; i < full_width; i++)
                        {
                            for (int j = 0; j < full_width; j++)
                            {
                                if (intensity[i, j] > zval) zval = intensity[i, j];
                                if (intensity[i, j] < m_Saturation) nonSaturatedPixels++;
                            }
                        }
                        IStarMax = (double) zval - IBackground;
                    }

                    double[] dx = new double[full_width];
                    double[] dy = new double[full_width];
                    double[] fx = new double[full_width];
                    double[] fy = new double[full_width];

                    double r0_squared = r0*r0;

                    for (int i = 0; i < full_width; i++)
                    {
                        dx[i] = (double) i - found_x;
                        fx[i] = Math.Exp(-dx[i]*dx[i]/r0_squared);
                        dy[i] = (double) i - found_y;
                        fy[i] = Math.Exp(-dy[i]*dy[i]/r0_squared);
                    }

                    SafeMatrix A = new SafeMatrix(nonSaturatedPixels, 5);
                    SafeMatrix X = new SafeMatrix(nonSaturatedPixels, 1);

                    int index = -1;
                    for (int i = 0; i < full_width; i++)
                    {
                        for (int j = 0; j < full_width; j++)
                        {
							double zval = intensity[i, j];

                            if (zval < m_Saturation)
                            {
                                index++;

								if (m_BackgroundModel != null) zval -= m_BackgroundModel.ComputeValue(i, j);

                                double exp_val = fx[i]*fy[j];

                                double residual = IBackground + IStarMax*exp_val - zval;
                                X[index, 0] = -residual;

                                A[index, 0] = 1.0; /* slope in i0 */
                                A[index, 1] = exp_val; /* slope in a0 */
                                A[index, 2] = 2.0*IStarMax*dx[i]*exp_val/r0_squared;
                                A[index, 3] = 2.0*IStarMax*dy[j]*exp_val/r0_squared;
                                A[index, 4] = 2.0*IStarMax*(dx[i]*dx[i] + dy[j]*dy[j])*exp_val/(r0*r0_squared);
                            }
                        }
                    }

					SafeMatrix Y;

					if (useNativeMatrix)
					{
						Y = TangraModelCore.SolveLinearSystemFast(A, X);
					}
					else
					{
						SafeMatrix a_T = A.Transpose();
						SafeMatrix aa = a_T * A;
						SafeMatrix aa_inv = aa.Inverse();
						Y = (aa_inv * a_T) * X;						
					}

                    /* we need at least 6 unsaturated pixels to solve 5 params */
                    if (nonSaturatedPixels > 6) /* Request all pixels to be good! */
                    {
                        IBackground += Y[0, 0];
                        IStarMax += Y[1, 0];

                        for (int i = 2; i < 4; i++)
                        {
                            if (Y[i, 0] > 1.0) Y[i, 0] = 1.0;
                            if (Y[i, 0] < -1.0) Y[i, 0] = -1.0;
                        }

                        found_x += Y[2, 0];
                        found_y += Y[3, 0];

                        if (Y[4, 0] > r0/10.0) Y[4, 0] = r0/10.0;
                        if (Y[4, 0] < -r0/10.0) Y[4, 0] = -r0/10.0;

                        r0 += Y[4, 0];
                    }
                    else
                    {
                        m_IsSolved = false;
                        return;
                    }
                }

                m_IsSolved = true;
                m_IBackground = IBackground;
                m_IStarMax = IStarMax;
                m_X0 = found_x;
                m_Y0 = found_y;
                m_R0 = r0;

                m_Residuals = new double[full_width,full_width];

                for (int x = 0; x < full_width; x++)
                    for (int y = 0; y < full_width; y++)
                    {
                        m_Residuals[x, y] = intensity[x, y] - GetPSFValueInternal(x, y);
                    }
            }
            catch(DivideByZeroException)
            { }
        }


		private void LinearFitOfAveragedModel(uint[,] intensity)
        {
            // First do a non linear fit to find X0 and Y0
            NonLinearFit(intensity, false);

            if (m_IsSolved)
            {
                // Then do a linear fit to find IBackground and IStarMax
                // I(x, y) = IBackground + IStarMax * Exp ( -((x - X0)*(x - X0) + (y - Y0)*(y - Y0)) / (r0 * r0))
                LinearRegression linearFit = new LinearRegression();

                double modelR = m_ModelFWHM/(2*Math.Sqrt(Math.Log(2)));
                double modelRSquare = modelR*modelR;
                double[,] modelData = new double[m_MatrixSize, m_MatrixSize];

                for (int x = 0; x < m_MatrixSize; x++)
                    for (int y = 0; y < m_MatrixSize; y++)
                    {
                        double modelVal = Math.Exp(-((x - X0)*(x - X0) + (y - Y0)*(y - Y0))/(modelRSquare));
                        modelData[x, y] = modelVal;
                        linearFit.AddDataPoint(modelVal, intensity[x, y]);
                    }
                linearFit.Solve();

                for (int x = 0; x < m_MatrixSize; x++)
                    for (int y = 0; y < m_MatrixSize; y++)
                    {
                        m_Residuals[x, y] = intensity[x, y] - linearFit.ComputeY(modelData[x, y]);
                    }

                m_IBackground = linearFit.B;
                m_IStarMax = linearFit.A;
                m_R0 = modelR;
            }
        }

		// I(x, y) = IBackground + IStarMax * Exp ( -(x - X0)*(x - X0)/ (rx0 * rx0) + (y - Y0)*(y - Y0) / (ry0 * ry0))
		private void NonLinearAsymetricFit(uint[,] intensity, bool useNativeMatrix)
		{
			m_IsSolved = false;

			try
			{
				int full_width = (int)Math.Round(Math.Sqrt(intensity.Length));

				m_MatrixSize = full_width;

				int half_width = full_width / 2;

				m_HalfWidth = half_width;

				switch (DataRange)
				{
					case PSFFittingDataRange.DataRange8Bit:
						m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation8Bit;
						break;

					case PSFFittingDataRange.DataRange12Bit:
						m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation12Bit;
						break;

					case PSFFittingDataRange.DataRange14Bit:
						m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation14Bit;
						break;

					case PSFFittingDataRange.DataRange16Bit:
						m_Saturation = NormVal > 0
							? (uint)(TangraConfig.Settings.Photometry.Saturation.Saturation8Bit * NormVal / 255.0)
							: TangraConfig.Settings.Photometry.Saturation.Saturation16Bit;
						break;

					default:
						m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation8Bit;
						break;
				}
				
				int nonSaturatedPixels = 0;

				double IBackground = 0;
				double rx0 = 4.0;
				double ry0 = 4.0;
				double IStarMax = 0;

				double found_x = half_width;
				double found_y = half_width;

				for (int iter = NumberIterations; iter > 0; iter--)
				{
					if (iter == NumberIterations)
					{
						uint zval = 0;

						IBackground = 0.0; /* assume no backgnd intensity at first... */
						for (int i = 0; i < full_width; i++)
						{
							for (int j = 0; j < full_width; j++)
							{
								if (intensity[i, j] > zval) zval = intensity[i, j];
								if (intensity[i, j] < m_Saturation) nonSaturatedPixels++;
							}
						}
						IStarMax = (double)zval - IBackground;
					}

					double[] dx = new double[full_width];
					double[] dy = new double[full_width];
					double[] fx = new double[full_width];
					double[] fy = new double[full_width];

					double rx0_squared = rx0 * rx0;
					double ry0_squared = ry0 * ry0;

					for (int i = 0; i < full_width; i++)
					{
						dx[i] = (double)i - found_x;
						fx[i] = Math.Exp(-dx[i] * dx[i] / rx0_squared);
						dy[i] = (double)i - found_y;
						fy[i] = Math.Exp(-dy[i] * dy[i] / ry0_squared);
					}

					SafeMatrix A = new SafeMatrix(nonSaturatedPixels, 6);
					SafeMatrix X = new SafeMatrix(nonSaturatedPixels, 1);

					int index = -1;
					for (int i = 0; i < full_width; i++)
					{
						for (int j = 0; j < full_width; j++)
						{
							double zval = intensity[i, j];

							if (zval < m_Saturation)
							{
								index++;

								if (m_BackgroundModel != null) zval -= m_BackgroundModel.ComputeValue(i, j);

								double exp_val = fx[i] * fy[j];

								double residual = IBackground + IStarMax * exp_val - (double)zval;
								X[index, 0] = -residual;

								A[index, 0] = 1.0; /* slope in i0 */
								A[index, 1] = exp_val; /* slope in a0 */
								A[index, 2] = 2.0 * IStarMax * dx[i] * exp_val / rx0_squared;
								A[index, 3] = 2.0 * IStarMax * dy[j] * exp_val / ry0_squared;
								A[index, 4] = 2.0 * IStarMax * (dx[i] * dx[i]) * exp_val / (rx0 * rx0_squared);
								A[index, 5] = 2.0 * IStarMax * (dy[j] * dy[j]) * exp_val / (ry0 * ry0_squared);
							}
						}
					}

					SafeMatrix Y;

					if (useNativeMatrix)
					{
						Y = TangraModelCore.SolveLinearSystemFast(A, X);
					}
					else
					{
						SafeMatrix a_T = A.Transpose();
						SafeMatrix aa = a_T * A;
						SafeMatrix aa_inv = aa.Inverse();
						Y = (aa_inv * a_T) * X;
					}

					/* we need at least 7 unsaturated pixels to solve 6 params */
					if (nonSaturatedPixels > 7) /* Request all pixels to be good! */
					{
						IBackground += Y[0, 0];
						IStarMax += Y[1, 0];

						for (int i = 2; i < 5; i++)
						{
							if (Y[i, 0] > 1.0) Y[i, 0] = 1.0;
							if (Y[i, 0] < -1.0) Y[i, 0] = -1.0;
						}

						found_x += Y[2, 0];
						found_y += Y[3, 0];

						if (Y[4, 0] > rx0 / 10.0) Y[4, 0] = rx0 / 10.0;
						if (Y[4, 0] < -rx0 / 10.0) Y[4, 0] = -rx0 / 10.0;

						rx0 += Y[4, 0];

						if (Y[5, 0] > ry0 / 10.0) Y[5, 0] = ry0 / 10.0;
						if (Y[5, 0] < -ry0 / 10.0) Y[5, 0] = -ry0 / 10.0;

						ry0 += Y[5, 0];
					}
					else
					{
						m_IsSolved = false;
						return;
					}
				}

				m_IsSolved = true;
				m_IBackground = IBackground;
				m_IStarMax = IStarMax;
				m_X0 = found_x;
				m_Y0 = found_y;
				m_R0 = double.NaN;
				m_RX0 = rx0;
				m_RY0 = ry0;

				m_Residuals = new double[full_width, full_width];

				for (int x = 0; x < full_width; x++)
					for (int y = 0; y < full_width; y++)
					{
						m_Residuals[x, y] = intensity[x, y] - GetPSFValueInternalAsymetric(x, y);
					}
			}
			catch (DivideByZeroException)
			{ }
		}


        private double GetPSFValueInternal(double x, double y)
        {
            return m_IBackground + m_IStarMax * Math.Exp(-((x - m_X0) * (x - m_X0) + (y - m_Y0) * (y - m_Y0)) / (m_R0 * m_R0));
        }

		private double GetPSFValueInternalAsymetric(double x, double y)
		{
			return m_IBackground + m_IStarMax * Math.Exp(-(x - m_X0) * (x - m_X0) / (m_RX0 * m_RX0) + (y - m_Y0) * (y - m_Y0) / (m_RY0 * m_RY0));
		}

		double ITrackedObjectPsfFit.GetPSFValueAt(double x, double y)
		{
			return GetPSFValueInternal(x, y);
		}
		
		double ITrackedObjectPsfFit.GetResidualAt(int x, int y)
		{
			return m_Residuals[x, y];
		}


		public void DrawGraph(Graphics g, Rectangle rect, int bpp, uint normVal)
		{
			DrawGraph(g, rect, bpp, normVal, this, m_Saturation);
		}

		public static void DrawGraph(Graphics g, Rectangle rect, int bpp, uint normVal, ITrackedObjectPsfFit trackedPsf, uint saturation)
        {
            float margin = 6.0f;
            double maxZ = 256;

			if (bpp == 14) maxZ = 16384;
			else if (bpp == 12) maxZ = 4096;
			else if (bpp == 16 && normVal != 0) maxZ = normVal;
			else if (bpp == 16 && normVal == 0) maxZ = 65535;

            int totalSteps = 100;

			float halfWidth = (float)trackedPsf.MatrixSize / 2.0f;
			float step = (float)(trackedPsf.MatrixSize * 1.0 / totalSteps);

			float yScale = (float)((rect.Height * 1.0 - 2 * margin) / (maxZ - trackedPsf.I0));
            float xScale = (float)(rect.Width * 1.0 - 2 * margin) / (halfWidth * 2);

            float xPrev = float.NaN;
            float yPrev = float.NaN;

        	Brush bgBrush = SystemBrushes.ControlDarkDark;
			Brush includedPointsBrush = Brushes.Yellow;
			Brush excludedPointBrush = Brushes.Gray;

			g.FillRectangle(bgBrush, rect);

			if (!trackedPsf.IsSolved)
                return;

			for (int x = 0; x < trackedPsf.MatrixSize; x++)
				for (int y = 0; y < trackedPsf.MatrixSize; y++)
                {
					double z0 = trackedPsf.GetPSFValueAt(x, y);
					double z = z0 + trackedPsf.GetResidualAt(x, y);
					double d = Math.Sqrt((x - trackedPsf.X0) * (x - trackedPsf.X0) + (y - trackedPsf.Y0) * (y - trackedPsf.Y0));

					int sign = Math.Sign(x - trackedPsf.X0); if (sign == 0) sign = 1;

                    float xVal = (float)(margin + (halfWidth + sign * d) * xScale);
					float yVal = rect.Height - margin - (float)(z - trackedPsf.I0) * yScale;

					if (xVal >= 0 && xVal <= rect.Width && yVal >= 0 && yVal <= rect.Height)
					{
						Brush pointBrush = z >= saturation ? excludedPointBrush : includedPointsBrush;
						g.FillRectangle(pointBrush, rect.Left + xVal - 1, rect.Top + yVal - 1, 3, 3);
					}
                }

            for (float width = -halfWidth; width < halfWidth; width += step)
            {
				float z = (float)trackedPsf.GetPSFValueAt(width + trackedPsf.X0, trackedPsf.Y0);
                float x = margin + (width + halfWidth) * xScale;
				float y = rect.Height - margin - (z - (float)trackedPsf.I0) * yScale;

                if (!float.IsNaN(xPrev) &&
                    y > margin && yPrev > margin &&
                    y < rect.Height && yPrev < rect.Height)
                {
					g.DrawLine(Pens.LimeGreen, rect.Left + xPrev, rect.Top + yPrev, rect.Left + x, rect.Top + y);
                }

                xPrev = x;
                yPrev = y;
            }
        }

		public override void DrawInternalPoints(Graphics g, Rectangle rect, float aperture, Brush incldedPinBrush, int bpp)
		{
			DrawInternalPoints(g, rect, aperture, incldedPinBrush, Brushes.Gray, Pens.GreenYellow, bpp);
		}

		private void DrawInternalPoints(
			Graphics g, Rectangle rect, float aperture,
			Brush incldedPinBrush, Brush excludedBrush, Pen curvePen, int bpp)
        {
            float margin = 3.0f;
			double maxZ = 256.0;
			if (bpp == 14) maxZ = 16384.0;
			else if (bpp == 12) maxZ = 4096.0;
			else if (bpp == 16 && NormVal > 0) maxZ = NormVal;
			else if (bpp == 16 && NormVal == 0) maxZ = 65535.0;
			
			maxZ = Math.Min(maxZ, GetPSFValueInternal(m_X0, m_Y0) + 5);

            int totalSteps = 100;

            float halfWidth = (float)m_MatrixSize / 2.0f;
            float step = (float)(m_MatrixSize * 1.0 / totalSteps);

            float yScale = (float)((rect.Height * 1.0 - 2 * margin) / (maxZ - m_IBackground));
            float xScale = (float)(rect.Width * 1.0 - 2 * margin) / (halfWidth * 2);

            float xPrev = float.NaN;
            float yPrev = float.NaN;

            g.FillRectangle(SystemBrushes.ControlDarkDark, rect);

            for (float width = -halfWidth; width < halfWidth; width += step)
            {
                float z = (float)GetPSFValueInternal(width + m_X0, m_Y0);
                float x = margin + (width + halfWidth) * xScale;
                float y = rect.Height - margin - (z - (float)m_IBackground) * yScale;

                if (!float.IsNaN(xPrev) &&
                    y > margin && yPrev > margin &&
                    y < rect.Height && yPrev < rect.Height)
                {
					g.DrawLine(curvePen, xPrev, yPrev, x, y);
                }

                xPrev = x;
                yPrev = y;
            }

            if (m_Residuals == null) 
                return;

            for (int x = 0; x < m_MatrixSize; x++)
                for (int y = 0; y < m_MatrixSize; y++)
                {
                    double z0 = GetPSFValueInternal(x, y);
                    double z = z0 + m_Residuals[x, y];
                    double d = Math.Sqrt((x - m_X0) * (x - m_X0) + (y - m_Y0) * (y - m_Y0));

                    float xVal = (float)(margin + (halfWidth + Math.Sign(x - m_X0) * d) * xScale);
                    float yVal = rect.Height - margin - (float)(z - m_IBackground) * yScale;

					g.FillRectangle(
						d <= aperture ? incldedPinBrush : excludedBrush, 
                        xVal - 1, yVal -1, 3,  3);
                }

            // Uncomment to draw the FWHM
            //g.DrawLine(
            //    Pens.White,
            //    rect.Width / 2 - ((float)FWHM * xScale / 2),
            //    rect.Height - margin - ((float)m_IStarMax - (float)m_IBackground) * yScale / 2,
            //    rect.Width / 2 + ((float)FWHM * xScale / 2),
            //    rect.Height - margin - ((float)m_IStarMax - (float)m_IBackground) * yScale / 2);
        }

		public void DrawDataPixels(Graphics g, Rectangle rect, int bpp, uint normVal)
        {
			DrawDataPixels(g, rect, 1, Pens.Yellow, bpp, normVal);
        }

		public void DrawDataPixels(Graphics g, Rectangle rect, float aperture, Pen aperturePen, int bpp, uint normVal)
		{
			DrawDataPixels(g, rect, aperture, aperturePen, bpp, normVal, this);
		}

		public static void DrawDataPixels(Graphics g, Rectangle rect, float aperture, Pen aperturePen, int bpp, uint normVal, ITrackedObjectPsfFit trackedPsf)
        {
            try
            {
                if (rect.Width != rect.Height) return;

				int coeff = rect.Width / trackedPsf.MatrixSize;
                if (coeff == 0) return;

				int size = trackedPsf.MatrixSize;

                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
						double z = Math.Round(trackedPsf.GetPSFValueAt(x, y) + trackedPsf.GetResidualAt(x, y));

						if (bpp == 16 && normVal == 0)
							z = z * 255.0f / 65535;
						else if (bpp == 14)
							z = z * 255.0f / 16383;
						else if (bpp == 12) 
							z = z * 255.0f / 4095;
						else if (bpp == 16 && normVal > 0)
							z = z * 255.0f / normVal;

                        byte val = (byte) (Math.Max(0, Math.Min(255, z)));
                        g.FillRectangle(AllGrayBrushes.GrayBrush(val), rect.Left + x*coeff, rect.Top + y*coeff, coeff, coeff);
                    }
                }

                if (aperture > 0)
                    g.DrawEllipse(
                        aperturePen,
						rect.Left + ((float)trackedPsf.X0 - aperture + 0.5f) * coeff,
						rect.Top + ((float)trackedPsf.Y0 - aperture + 0.5f) * coeff,
                        aperture*2*coeff, aperture*2*coeff);
				else if (aperture < 0)
				{
					g.DrawLine(aperturePen, rect.Left + ((float)trackedPsf.X0 - 3 * aperture + 0.5f) * coeff, rect.Top + (float)(trackedPsf.Y0 + 0.5f) * coeff, rect.Left + ((float)trackedPsf.X0 - aperture + 0.5f) * coeff, rect.Top + (float)(trackedPsf.Y0 + 0.5f) * coeff);
					g.DrawLine(aperturePen, rect.Left + ((float)trackedPsf.X0 + 3 * aperture + 0.5f) * coeff, rect.Top + (float)(trackedPsf.Y0 + 0.5f) * coeff, rect.Left + ((float)trackedPsf.X0 + aperture + 0.5f) * coeff, rect.Top + (float)(trackedPsf.Y0 + 0.5f) * coeff);
					g.DrawLine(aperturePen, rect.Left + (float)(trackedPsf.X0 + 0.5f) * coeff, rect.Top + ((float)trackedPsf.Y0 - 3 * aperture + 0.5f) * coeff, rect.Left + (float)(trackedPsf.X0 + 0.5f) * coeff, rect.Top + ((float)trackedPsf.Y0 - aperture + 0.5f) * coeff);
					g.DrawLine(aperturePen, rect.Left + (float)(trackedPsf.X0 + 0.5f) * coeff, rect.Top + ((float)trackedPsf.Y0 + 3 * aperture + 0.5f) * coeff, rect.Left + (float)(trackedPsf.X0 + 0.5f) * coeff, rect.Top + ((float)trackedPsf.Y0 + aperture + 0.5f) * coeff);
				}
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        public double GetLeastSquareFittedAmplitude(double empericalPSFR0)
        {
            // I = A * Gauss (empericalPSFR0) + b
            // Quick Least Square fit for A
            // 
            //     Sum(GD) - Sum(G) * Sum (D) / n
            // A= -------------------------------
            //     Sum (G**2) - (Sum(G) ** 2)/n
            //
            double sumGD = 0;
            double sumG = 0;
            double sumD = 0;
            double sumG2 = 0;

            int numPixels = (m_MatrixSize*m_MatrixSize);

            double fwhm = empericalPSFR0 * 2.355 / Math.Sqrt(2);

            for (int x = 0; x < m_MatrixSize; x++)
            for (int y = 0; y < m_MatrixSize; y++)
            {
                double intensity = m_Residuals[x, y] + GetPSFValueInternal(x, y);
                double empericalValue = Math.Exp(-((x - m_X0) * (x - m_X0) + (y - m_Y0) * (y - m_Y0)) / (empericalPSFR0 * empericalPSFR0));
                    
                sumD += intensity;
                sumG += empericalValue;
                sumGD += intensity*empericalValue;
                sumG2 += empericalValue * empericalValue;
            }

            double amplitude = (sumGD - (sumG * sumD) / numPixels) / (sumG2 - (sumG * sumG) / numPixels);

            return amplitude;
        }


        #region Serialization
        private static byte STREAM_VERSION = 4;

        public void Save(BinaryWriter writer)
        {
            writer.Write(STREAM_VERSION);

            writer.Write(m_IBackground);
            writer.Write(m_IStarMax);
            writer.Write(m_X0);
            writer.Write(m_Y0);
            writer.Write(m_R0);

            writer.Write(m_MatrixSize);
            writer.Write(m_HalfWidth);
            writer.Write(m_Saturation);

            writer.Write(m_xCenter);
            writer.Write(m_yCenter);

            for (int x = 0; x < m_MatrixSize; x++)
                for (int y = 0; y < m_MatrixSize; y++)
                    writer.Write(m_Residuals[x, y]);

            writer.Write(m_NewMatrixSize);
            writer.Write(m_NewMatrixX0);
            writer.Write(m_NewMatrixY0);

			writer.Write(m_RX0);
			writer.Write(m_RY0);

	        writer.Write(m_UsesBackgroundModel);
        }

        public static PSFFit Load(BinaryReader reader)
        {
            return Load(reader, -1);
        }

        private static PSFFit Load(BinaryReader reader, int uniqueId)
        {
            byte version = reader.ReadByte();
            if (version > 0)
            {
                PSFFit loadedFit = new PSFFit(uniqueId);

                loadedFit.m_IBackground = reader.ReadDouble();
                loadedFit.m_IStarMax = reader.ReadDouble();
                loadedFit.m_X0 = reader.ReadDouble();
                loadedFit.m_Y0 = reader.ReadDouble();
                loadedFit.m_R0 = reader.ReadDouble();

                loadedFit.m_MatrixSize= reader.ReadInt32();
                loadedFit.m_HalfWidth = reader.ReadInt32();
                loadedFit.m_Saturation = reader.ReadUInt32();

                loadedFit.m_xCenter = reader.ReadInt32();
                loadedFit.m_yCenter = reader.ReadInt32();

                loadedFit.m_Residuals = new double[loadedFit.m_MatrixSize, loadedFit.m_MatrixSize];

                for (int x = 0; x < loadedFit.m_MatrixSize; x++)
                    for (int y = 0; y < loadedFit.m_MatrixSize; y++)
                        loadedFit.m_Residuals[x, y] = reader.ReadDouble();

                loadedFit.m_IsSolved = true;

                if (version > 1)
                {
                    loadedFit.m_NewMatrixSize = reader.ReadInt32();
                    loadedFit.m_NewMatrixX0 = reader.ReadInt32();
                    loadedFit.m_NewMatrixY0 = reader.ReadInt32();

					if (version > 2)
					{
						loadedFit.m_RX0 = reader.ReadDouble();
						loadedFit.m_RY0 = reader.ReadDouble();

						if (version > 3)
						{
							loadedFit.m_UsesBackgroundModel = reader.ReadBoolean();
						}						
					}
                }
                return loadedFit;
            }

            return null;
        }

        public override string ToString()
        {
            using(MemoryStream memStr = new MemoryStream())
            using (BinaryWriter wrt = new BinaryWriter(memStr))
            {
                this.Save(wrt);
                wrt.Flush();

                memStr.Position = 0;
                return Convert.ToBase64String(memStr.ToArray());
            }
        }

        public static PSFFit FromString(string base64String)
        {
            return FromString(base64String, -1);
        }

        private static PSFFit FromString(string base64String, int uniqueId)
        {
            using (MemoryStream memStr = new MemoryStream(Convert.FromBase64String(base64String)))
            using (BinaryReader rdr = new BinaryReader(memStr))
            {
                return PSFFit.Load(rdr, uniqueId);
            }
        }

        public PSFFit Clone()
        {
            return PSFFit.FromString(this.ToString(), this.UniqueId);
        }
        #endregion

		public void Show()
		{
			frmPSFViewer frm = new frmPSFViewer(this, 8, 0);
			frm.Show();
		}

		public void Show(int bpp)
		{
			frmPSFViewer frm = new frmPSFViewer(this, bpp, NormVal);
			frm.Show();
		}

		public void ShowDialog()
		{
			frmPSFViewer frm = new frmPSFViewer(this, 8, 0);
			frm.ShowDialog();
		}

		public void ShowDialog(int bpp)
		{
			frmPSFViewer frm = new frmPSFViewer(this, bpp, NormVal);
			frm.ShowDialog();
		}
    }
}
