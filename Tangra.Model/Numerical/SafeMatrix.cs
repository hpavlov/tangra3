using System;
using System.Collections.Generic;
using System.Text;

namespace Tangra.Model.Numerical
{
    public class SafeMatrix
    {
        private double[] m_Elements;

        private readonly int m_RowCount;
        private readonly int m_ColumnCount;
        private readonly int m_MatrixSize;

        /// <summary>
        /// Number of rows of the matrix.
        /// </summary>
        public int RowCount
        {
            get { return m_RowCount; }
        }

        /// <summary>
        /// Number of columns of the matrix.
        /// </summary>
        public int ColumnCount
        {
            get { return m_ColumnCount; }
        }

        public SafeMatrix(int rows, int columns)
        {
            m_RowCount = rows;
            m_ColumnCount = columns;
            m_MatrixSize = rows * columns;

            m_Elements = new double[m_MatrixSize];
        }

        public SafeMatrix(double[,] values)
        {
            m_RowCount = (int)values.GetLongLength(0);
            m_ColumnCount = (int)values.GetLongLength(1);
            m_MatrixSize = m_RowCount * m_ColumnCount;

            m_Elements = new double[m_MatrixSize];

            int idx = -1;
            for (int i = 0; i < m_RowCount; i++)
            {
                for (int j = 0; j < m_ColumnCount; j++)
                {
                    idx++;
                    m_Elements[idx] = values[i, j];
                }
            }
        }

        public double this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= m_RowCount || col < 0 || col >= m_ColumnCount)
                    throw new IndexOutOfRangeException();

                return m_Elements[row * m_ColumnCount + col];
            }
            set
            {
                if (row < 0 || row >= m_RowCount || col < 0 || col >= m_ColumnCount)
                    throw new IndexOutOfRangeException();

                m_Elements[row * m_ColumnCount + col] = value;
            }
        }

        public SafeMatrix Clone()
        {
            SafeMatrix A = new SafeMatrix(m_RowCount, m_ColumnCount);

            for (int i = 0; i < m_MatrixSize; i++)
                A.m_Elements[i] = m_Elements[i];

            return A;
        }

        public override bool Equals(object obj)
        {
            if (obj is SafeMatrix)
                return (SafeMatrix)obj == this;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return -1;
        }

        #region Operators Overloading

        public static bool operator ==(SafeMatrix A, SafeMatrix B)
        {
            if (A.RowCount != B.RowCount || A.ColumnCount != B.ColumnCount)
                return false;

            for (int i = 0; i < A.m_MatrixSize; i++)
            {
                if (A.m_Elements[i] != B.m_Elements[i]) return false;
            }

            return true;
        }

        public static bool operator !=(SafeMatrix A, SafeMatrix B)
        {
            return !(A == B);
        }

        public static SafeMatrix operator +(SafeMatrix A, SafeMatrix B)
        {
            if (A.RowCount != B.RowCount || A.ColumnCount != B.ColumnCount)
                throw new ArgumentException("Matrices must be of the same dimension.");

            for (int i = 0; i < A.m_MatrixSize; i++)
            {
                A.m_Elements[i] += B.m_Elements[i];
            }

            return A;
        }

        public static SafeMatrix operator -(SafeMatrix A, SafeMatrix B)
        {
            if (A.RowCount != B.RowCount || A.ColumnCount != B.ColumnCount)
                throw new ArgumentException("Matrices must be of the same dimension.");

            for (int i = 0; i < A.m_MatrixSize; i++)
            {
                A.m_Elements[i] -= B.m_Elements[i];
            }

            return A;
        }

        public static SafeMatrix operator -(SafeMatrix A)
        {
            for (int i = 0; i < A.m_MatrixSize; i++)
            {
                A.m_Elements[i] = -1 * A.m_Elements[i];
            }

            return A;
        }

        public static SafeMatrix operator *(SafeMatrix A, SafeMatrix B)
        {
            if (A.ColumnCount != B.RowCount)
                throw new ArgumentException("Inner matrix dimensions must agree.");

            SafeMatrix C = new SafeMatrix(A.RowCount, B.ColumnCount);

            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < B.ColumnCount; j++)
                {
                    C.m_Elements[i * C.m_ColumnCount + j] = Dot(A, i, B, j);
                }
            }

            return C;

        }

        public static SafeMatrix operator *(SafeMatrix A, double x)
        {
            return x * A;
        }

        public static SafeMatrix operator *(double x, SafeMatrix A)
        {
            SafeMatrix B = new SafeMatrix(A.RowCount, A.ColumnCount);

            for (int i = 0; i < A.m_MatrixSize; i++)
            {
                B.m_Elements[i] = A.m_Elements[i] * x;
            }

            return B;
        }

        public static SafeMatrix operator /(SafeMatrix A, double x)
        {
            return (1 / x) * A;
        }

        //public static Matrix operator ^(Matrix<T> A, int k)
        //{
        //    if (k < 0)
        //        if (A.IsSquare())
        //            return A.InverseLeverrier() ^ (-k);
        //        else throw new InvalidOperationException("Cannot take non-square matrix to the power of zero.");
        //    else if (k == 0)
        //        if (A.IsSquare())
        //            return Matrix.Identity(A.RowCount);
        //        else throw new InvalidOperationException("Cannot take non-square matrix to the power of zero.");
        //    else if (k == 1)
        //        if (A.IsSquare())
        //            return A;
        //        else throw new InvalidOperationException("Cannot take non-square matrix to the power of one.");
        //    else
        //    {
        //        Matrix M = A;
        //        for (int i = 1; i < k; i++)
        //        {
        //            M *= A;
        //        }

        //        return M;
        //    }
        //}

        #endregion

        public bool IsSquare()
        {
            return (this.ColumnCount == this.RowCount);
        }

        public static double Dot(SafeMatrix v, int vRow, SafeMatrix w, int wCol)
        {
            if (v.ColumnCount != w.RowCount)
                throw new ArgumentException("Vectors must be of the same length.");

            double buf = 0;

            for (int i = 0; i < v.ColumnCount; i++)
            {
                buf += v.m_Elements[vRow * v.m_ColumnCount + i] * w.m_Elements[i * w.m_ColumnCount + wCol];
            }

            return buf;
        }

        public double DiagProd()
        {
            double buf = 1;
            int dim = Math.Min(this.RowCount, this.ColumnCount);

            for (int i = 0; i < dim; i++)
            {
                buf *= m_Elements[i * m_ColumnCount + i];
            }

            return buf;
        }

        /// <summary>
        /// Performs LU-decomposition of this instance and saves L and U
        /// within, where the diagonal elements belong to U
        /// (the ones of L are ones...)
        /// </summary>
        public void LU()
        {
            if (!this.IsSquare())
                throw new InvalidOperationException("Cannot perform LU-decomposition of non-square matrix.");
         
            int p_row = 0;
            int p_col = 0;
            int n = m_ColumnCount;

            int p_k = 0;

            for (int k = 0; k < n; p_k += n, k++) 
            {
                for (int j = k; j < n; j++) 
                {
                  p_col = 0;

                  for (int p = 0; p < k; p_col += n, p++)
                    m_Elements[p_k + j] -= m_Elements[p_k + p] * m_Elements[p_col + j];
                }

                if (m_Elements[p_k + k] == 0.0) throw new DivideByZeroException("Warning: Matrix badly scaled or close to singular. Try LUSafe() instead. Check if det != 0.");

                p_row = p_k + n;

                for (int i = k + 1; i < n; p_row += n, i++) 
                {
                    p_col = 0;

                    for (int p = 0; p < k; p_col += n, p++)
                        m_Elements[p_row + k] -= m_Elements[p_row + p] * m_Elements[p_col + k];

                    m_Elements[p_row + k] /= m_Elements[p_k + k];
                }  
            }
        }

        ///// <summary>
        ///// Performs LU-decomposition of this instance and saves L and U
        ///// within, where the diagonal elements belong to U
        ///// (the ones of L are ones...)
        ///// </summary>
        //public void LU()
        //{
        //    if (!this.IsSquare())
        //        throw new InvalidOperationException("Cannot perform LU-decomposition of non-square matrix.");

        //    for (int j = 0; j < m_ColumnCount; j++)
        //    {
        //        if (this[j, j] == 0)
        //            throw new DivideByZeroException("Warning: Matrix badly scaled or close to singular. Try LUSafe() instead. Check if det != 0.");

        //        for (int k = 0; k < j; k++)
        //        {
        //            for (int i = k; i < m_ColumnCount; i++)
        //            {
        //                this[i, j] = this[i, j] - this[i, k] * this[k, j];
        //            }
        //        }

        //        for (int i = j; i < m_ColumnCount; i++)
        //        {
        //            this[i, j] = this[i, j] / this[j, j];
        //        }
        //    }
        //}

        public double Determinant()
        {
            if (!this.IsSquare())
                throw new InvalidOperationException("Cannot calc determinant of non-square matrix.");

            if (this.m_ColumnCount == 1)
                return this[0, 0];

            // perform LU-decomposition & return product of diagonal elements of U
            SafeMatrix X = this.Clone();
            // for speed concerns, use this
            try
            {
                X.LU();
                return X.DiagProd();
            }
            catch (DivideByZeroException)
            {
                // this is slower and needs more memory... .
                //Matrix P = X.LUSafe();
                //return (double)P.Signum() * X.DiagProd();

                throw;
            }
        }

        public SafeMatrix Minor(int row, int col)
        {
            // THIS IS THE LOW-LEVEL SOLUTION ~ O(n^2)
            SafeMatrix buf = new SafeMatrix(m_RowCount - 1, m_ColumnCount - 1);
            int r = 0;
            int c = 0;

            for (int i = 0; i < m_RowCount; i++)
            {
                if (i != row)
                {
                    for (int j = 0; j < m_ColumnCount; j++)
                    {
                        if (j != col)
                        {
                            buf.m_Elements[r * buf.m_ColumnCount + c] = m_Elements[i * m_ColumnCount + j];
                            c++;
                        }
                    }

                    c = 0;
                    r++;
                }
            }

            return buf;
        }

        public SafeMatrix Transpose()
        {
            SafeMatrix M = new SafeMatrix(m_ColumnCount, m_RowCount);

            for (int i = 0; i < m_ColumnCount; i++)
            {
                for (int j = 0; j < m_RowCount; j++)
                {
                    M.m_Elements[i * M.m_ColumnCount + j] = m_Elements[j * m_ColumnCount + i];
                }
            }

            return M;
        }

        /// <summary>
        /// Inverts square matrix as long as det != 0.
        /// </summary>
        /// <returns>Inverse of matrix.</returns>
        public SafeMatrix Inverse()
        {
            if (!this.IsSquare())
                throw new InvalidOperationException("Cannot invert non-square matrix.");

            double det = this.Determinant();

            if (det == 0)
                throw new InvalidOperationException("Cannot invert (nearly) singular matrix.");

            SafeMatrix buf = new SafeMatrix(m_ColumnCount, m_ColumnCount);

            for (int i = 0; i < m_ColumnCount; i++)
            {
                for (int j = 0; j < m_ColumnCount; j++)
                {
                    buf.m_Elements[i * buf.m_ColumnCount + j] = (Math.Pow(-1, i + j) * this.Minor(j, i).Determinant()) / det;
                }
            }

            return buf;
        }

    }
}
