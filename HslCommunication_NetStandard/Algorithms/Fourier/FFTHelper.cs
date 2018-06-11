using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace HslCommunication.Algorithms.Fourier
{
    /// <summary>
    /// 离散傅氏变换的快速算法，处理的信号，适合单周期信号数为2的N次方个，支持变换及逆变换
    /// </summary>
    public class FFTHelper
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xreal"></param>
        /// <param name="ximag"></param>
        /// <param name="n"></param>
        private static void bitrp( double[] xreal, double[] ximag, int n )
        {
            // 位反转置换 Bit-reversal Permutation
            int i, j, a, b, p;

            for (i = 1, p = 0; i < n; i *= 2)
            {
                p++;
            }
            for (i = 0; i < n; i++)
            {
                a = i;
                b = 0;
                for (j = 0; j < p; j++)
                {
                    b = b * 2 + a % 2;
                    a = a / 2;
                }
                if (b > i)
                {
                    double t = xreal[i];
                    xreal[i] = xreal[b];
                    xreal[b] = t;

                    t = ximag[i];
                    ximag[i] = ximag[b];
                    ximag[b] = t;
                }
            }
        }


        /// <summary>
        /// 快速傅立叶变换
        /// </summary>
        /// <param name="xreal">实数部分</param>
        /// <returns></returns>
        public static double[] FFT( double[] xreal )
        {
            return FFT( xreal, new double[xreal.Length] );
        }


        

        /// <summary>
        /// 快速傅立叶变换
        /// </summary>
        /// <param name="xreal">实数部分</param>
        /// <param name="ximag">虚数部分</param>
        /// <returns></returns>
        public static double[] FFT( double[] xreal, double[] ximag )
        {
            //n值为2的N次方
            int n = 2;
            while (n <= xreal.Length)
            {
                n *= 2;
            }
            n /= 2;

            // 快速傅立叶变换，将复数 x 变换后仍保存在 x 中，xreal, ximag 分别是 x 的实部和虚部
            double[] wreal = new double[n / 2];
            double[] wimag = new double[n / 2];
            double treal, timag, ureal, uimag, arg;
            int m, k, j, t, index1, index2;

            bitrp( xreal, ximag, n );

            // 计算 1 的前 n / 2 个 n 次方根的共轭复数 W'j = wreal [j] + i * wimag [j] , j = 0, 1, ... , n / 2 - 1
            arg = (-2 * Math.PI / n);
            treal = Math.Cos( arg );
            timag = Math.Sin( arg );
            wreal[0] = 1.0f;
            wimag[0] = 0.0f;
            for (j = 1; j < n / 2; j++)
            {
                wreal[j] = wreal[j - 1] * treal - wimag[j - 1] * timag;
                wimag[j] = wreal[j - 1] * timag + wimag[j - 1] * treal;
            }

            for (m = 2; m <= n; m *= 2)
            {
                for (k = 0; k < n; k += m)
                {
                    for (j = 0; j < m / 2; j++)
                    {
                        index1 = k + j;
                        index2 = index1 + m / 2;
                        t = n * j / m;    // 旋转因子 w 的实部在 wreal [] 中的下标为 t
                        treal = wreal[t] * xreal[index2] - wimag[t] * ximag[index2];
                        timag = wreal[t] * ximag[index2] + wimag[t] * xreal[index2];
                        ureal = xreal[index1];
                        uimag = ximag[index1];
                        xreal[index1] = ureal + treal;
                        ximag[index1] = uimag + timag;
                        xreal[index2] = ureal - treal;
                        ximag[index2] = uimag - timag;
                    }
                }
            }

            double[] result = new double[n];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Math.Sqrt( Math.Pow( xreal[i], 2 ) + Math.Pow( ximag[i], 2 ) );
            }

            return result;
        }


        /// <summary>
        /// 快速傅立叶变换的逆变换
        /// </summary>
        /// <param name="xreal">实数部分</param>
        /// <param name="ximag">虚数部分</param>
        /// <returns></returns>
        public static int IFFT( double[] xreal, double[] ximag )
        {
            //n值为2的N次方
            int n = 2;
            while (n <= xreal.Length)
            {
                n *= 2;
            }
            n /= 2;

            // 快速傅立叶逆变换
            double[] wreal = new double[n / 2];
            double[] wimag = new double[n / 2];
            double treal, timag, ureal, uimag, arg;
            int m, k, j, t, index1, index2;

            bitrp( xreal, ximag, n );

            // 计算 1 的前 n / 2 个 n 次方根 Wj = wreal [j] + i * wimag [j] , j = 0, 1, ... , n / 2 - 1
            arg = (2 * Math.PI / n);
            treal = (Math.Cos( arg ));
            timag = (Math.Sin( arg ));
            wreal[0] = 1.0f;
            wimag[0] = 0.0f;
            for (j = 1; j < n / 2; j++)
            {
                wreal[j] = wreal[j - 1] * treal - wimag[j - 1] * timag;
                wimag[j] = wreal[j - 1] * timag + wimag[j - 1] * treal;
            }

            for (m = 2; m <= n; m *= 2)
            {
                for (k = 0; k < n; k += m)
                {
                    for (j = 0; j < m / 2; j++)
                    {
                        index1 = k + j;
                        index2 = index1 + m / 2;
                        t = n * j / m;    // 旋转因子 w 的实部在 wreal [] 中的下标为 t
                        treal = wreal[t] * xreal[index2] - wimag[t] * ximag[index2];
                        timag = wreal[t] * ximag[index2] + wimag[t] * xreal[index2];
                        ureal = xreal[index1];
                        uimag = ximag[index1];
                        xreal[index1] = ureal + treal;
                        ximag[index1] = uimag + timag;
                        xreal[index2] = ureal - treal;
                        ximag[index2] = uimag - timag;
                    }
                }
            }

            for (j = 0; j < n; j++)
            {
                xreal[j] /= n;
                ximag[j] /= n;
            }

            return n;
        }
    }
}
