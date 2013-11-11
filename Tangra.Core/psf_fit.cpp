#include "psf_fit.h"
#include "math.h"
#include <cmath>
#include "Tangra.Math.h"

PsfFit::PsfFit(long xCenter, long yCenter, PSFFittingDataRange dataRange)
{
	this->FittingMethod = NonLinearFit;
	
	m_xCenter = xCenter;
	m_yCenter = yCenter;
	m_IsSolved = false;
	
	SetDataRange(dataRange);	
	
	m_Residuals = (double*)malloc(MAX_MATRIX_SIZE * MAX_MATRIX_SIZE * sizeof(double));
}

PsfFit::PsfFit(PSFFittingDataRange dataRange)
{
	this->FittingMethod = NonLinearFit;
	
	SetDataRange(dataRange);	
	
	m_Residuals = (double*)malloc(MAX_MATRIX_SIZE * MAX_MATRIX_SIZE * sizeof(double));
}

PsfFit::~PsfFit()
{
	if (NULL != m_Residuals)
	{
		delete m_Residuals;
		m_Residuals = NULL;
	}
}

void PsfFit::SetDataRange(PSFFittingDataRange dataRange)
{
	this->m_DataRange = dataRange;
	
	switch (dataRange)
	{
		case DataRange8Bit:
			m_Saturation = SATURATION_8BIT;
			break;

		case DataRange12Bit:
			m_Saturation = SATURATION_12BIT;
			break;

		case DataRange14Bit:
			m_Saturation = SATURATION_14BIT;
			break;

		default:
			m_Saturation = SATURATION_8BIT;
			break;
	};	
}

void PsfFit::SetNewFieldCenterFrom17PixMatrix(int x, int y)
{
	m_xCenter = x + m_xCenter - 8;
	m_yCenter = y + m_yCenter - 8;  
}

void PsfFit::SetNewFieldCenterFrom35PixMatrix(int x, int y)
{
	m_xCenter = x + m_xCenter - 17;
	m_yCenter = y + m_yCenter - 17;  
}

bool PsfFit::IsSolved()
{
	return m_IsSolved;
}

double PsfFit::GetValue(double x, double y)
{
	return GetPSFValueInternal(x, y);
}

double PsfFit::XCenter()
{
	return m_xCenter + m_X0 - m_HalfWidth;
}

double PsfFit::YCenter()
{
	return m_yCenter + m_Y0 - m_HalfWidth;
}

long PsfFit::MatrixSize()
{
	return m_MatrixSize;
}

long PsfFit::X0()
{
	long xInx = (long) round(m_X0);
	if (xInx < 0) xInx = 0;
	if (xInx >= m_MatrixSize) xInx = m_MatrixSize - 1;

	return xInx;
}

long PsfFit::Y0()
{
	long yInx = (long)round(m_Y0);
	if (yInx < 0) yInx = 0;
	if (yInx >= m_MatrixSize) yInx = m_MatrixSize - 1;

	return yInx;
}

double PsfFit::X0_Matrix()
{
	return m_X0;
}

double PsfFit::Y0_Matrix()
{
	return m_Y0;
}

double PsfFit::I0()
{
	return m_IBackground;
}

double PsfFit::IMax()
{
	return m_IBackground + m_IStarMax;
}

unsigned long PsfFit::Brightness()
{
	return (unsigned long)round(IMax() - I0());
}

double PsfFit::FWHM()
{	
	if (FittingMethod == NonLinearAsymetricFit)
		return 2 * sqrt(-0.5 * log10(0.5) * RX0 * RY0);
	else
		return 2 * sqrt(log10(2)) * R0;
}

double PsfFit::Certainty()
{
	return CERTAINTY_CONST * (m_IStarMax / m_Saturation) / FWHM();
}

double PsfFit::ElongationPercentage()
{
	if (FittingMethod == NonLinearAsymetricFit)
		return fabs(1 - (RX0 / RY0)) * 100;
	else
		return 0;
}

double PsfFit::GetPSFValueInternal(double x, double y)
{
	return m_IBackground + m_IStarMax * exp(-((x - m_X0) * (x - m_X0) + (y - m_Y0) * (y - m_Y0)) / (R0 * R0));
}

double PsfFit::GetPSFValueInternalAsymetric(double x, double y)
{
	return m_IBackground + m_IStarMax * exp(-(x - m_X0) * (x - m_X0) / (RX0 * RX0) + (y - m_Y0) * (y - m_Y0) / (RY0 * RY0));
}

void PsfFit::Fit(long xCenter, long yCenter, unsigned long* intensity, long width)
{
	m_xCenter = xCenter;
	m_yCenter = yCenter;
	m_IsSolved = false;	
	
	Fit(intensity, width);
}

void PsfFit::Fit(unsigned long* intensity, long width)
{
	m_IsSolved = false;
	
	if (FittingMethod == NonLinearFit)
		DoNonLinearFit(intensity, width);
	else if (FittingMethod == NonLinearAsymetricFit)
		DoNonLinearAsymetricFit(intensity, width);
	else if (FittingMethod == LinearFitOfAveragedModel)
		DoLinearFitOfAveragedModel(intensity, width);
}

void PsfFit::DoNonLinearFit(unsigned long* intensity, long width)
{
	bool isSolved;
	double iBackground;
	double iStarMax;
	double x0;
	double y0;
	double r0;
	
	m_MatrixSize = width;
	m_HalfWidth = width / 2;
	
	EnsureLinearSystemSolutionBuffers(5);
	
	DoNonLinearPfsFit(intensity, width, m_Saturation, &isSolved, &iBackground, &iStarMax, &x0, &y0, &r0, m_Residuals);
	
	m_IsSolved = isSolved;
	m_IBackground = iBackground;
	m_IStarMax = iStarMax;
	m_X0 = x0;
	m_Y0 = y0;
	R0 = r0;
}

void PsfFit::DoNonLinearAsymetricFit(unsigned long* intensity, long width)
{
	
}

void PsfFit::DoLinearFitOfAveragedModel(unsigned long* intensity, long width)
{
	
}

void PsfFit::CopyResiduals(double* buffer, long matrixSize)
{
	memcpy(buffer, m_Residuals, matrixSize * matrixSize * sizeof(double));
}
		
/*
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
            if (newMatrixSize != 17)
            {
				uint[,] newData = new uint[newMatrixSize, newMatrixSize];
                int halfSize = newMatrixSize / 2;

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
            else
                Fit(intensity);
        }
        

        // I(x, y) = IBackground + IStarMax * Exp ( -((x - X0)*(x - X0) + (y - Y0)*(y - Y0)) / (r0 * r0))
		private void NonLinearFit(uint[,] intensity)
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

                        IBackground = 0.0; // assume no backgnd intensity at first... 
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
							uint zval = intensity[i, j];

                            if (zval < m_Saturation)
                            {
                                index++;

                                double exp_val = fx[i]*fy[j];

                                double residual = IBackground + IStarMax*exp_val - (double) zval;
                                X[index, 0] = -residual;

                                A[index, 0] = 1.0; // slope in i0 
                                A[index, 1] = exp_val; // slope in a0 
                                A[index, 2] = 2.0*IStarMax*dx[i]*exp_val/r0_squared;
                                A[index, 3] = 2.0*IStarMax*dy[j]*exp_val/r0_squared;
                                A[index, 4] = 2.0*IStarMax*(dx[i]*dx[i] + dy[j]*dy[j])*exp_val/(r0*r0_squared);
                            }
                        }
                    }

                    SafeMatrix a_T = A.Transpose();
                    SafeMatrix aa = a_T*A;
                    SafeMatrix aa_inv = aa.Inverse();
                    SafeMatrix Y = (aa_inv*a_T)*X;

                    // we need at least 6 unsaturated pixels to solve 5 params
                    if (nonSaturatedPixels > 6) // Request all pixels to be good!
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
            NonLinearFit(intensity);

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
		private void NonLinearAsymetricFit(uint[,] intensity)
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

						IBackground = 0.0; //assume no backgnd intensity at first...
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
							uint zval = intensity[i, j];

							if (zval < m_Saturation)
							{
								index++;

								double exp_val = fx[i] * fy[j];

								double residual = IBackground + IStarMax * exp_val - (double)zval;
								X[index, 0] = -residual;

								A[index, 0] = 1.0; // slope in i0 
								A[index, 1] = exp_val; // slope in a0 
								A[index, 2] = 2.0 * IStarMax * dx[i] * exp_val / rx0_squared;
								A[index, 3] = 2.0 * IStarMax * dy[j] * exp_val / ry0_squared;
								A[index, 4] = 2.0 * IStarMax * (dx[i] * dx[i]) * exp_val / (rx0 * rx0_squared);
								A[index, 5] = 2.0 * IStarMax * (dy[j] * dy[j]) * exp_val / (ry0 * ry0_squared);
							}
						}
					}

					SafeMatrix a_T = A.Transpose();
					SafeMatrix aa = a_T * A;
					SafeMatrix aa_inv = aa.Inverse();
					SafeMatrix Y = (aa_inv * a_T) * X;

					// we need at least 7 unsaturated pixels to solve 6 params 
					if (nonSaturatedPixels > 7) // Request all pixels to be good!
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
*/

