using System;
using System.Collections.Generic;
using System.Drawing;

#if !NETSTANDARD2_0
using System.Drawing.Drawing2D;
#endif
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
        /// <returns>变换后的数组值</returns>
        public static double[] FFT( double[] xreal )
        {
            return FFT( xreal, new double[xreal.Length] );
        }


#if !NETSTANDARD2_0

        /// <summary>
        /// 获取FFT变换后的显示图形，需要指定图形的相关参数
        /// </summary>
        /// <param name="xreal">实数部分的值</param>
        /// <param name="width">图形的宽度</param>
        /// <param name="heigh">图形的高度</param>
        /// <param name="lineColor">线条颜色</param>
        /// <returns>等待呈现的图形</returns>
        /// <remarks>
        /// <note type="warning">.net standrard2.0 下不支持。</note>
        /// </remarks>
        public static Bitmap GetFFTImage( double[] xreal,int width,int heigh ,Color lineColor)
        {
            double[] ximag = new double[xreal.Length];                // 构造虚对象
            double[] array = FFT( xreal, ximag );                     // 傅立叶变换

            Bitmap bitmap = new Bitmap( width, heigh );               // 构造图形
            Graphics g = Graphics.FromImage( bitmap );
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear( Color.White );

            Pen pen_Line = new Pen( Color.DimGray, 1 );               // 定义画笔资源
            Pen pen_Dash = new Pen( Color.LightGray, 1 );
            Pen pen_Fourier = new Pen( lineColor, 1 );
            pen_Dash.DashPattern = new float[2] { 5, 5 };
            pen_Dash.DashStyle = DashStyle.Custom;

            Font Font_Normal = SystemFonts.DefaultFont;               // 定义字体资源

            StringFormat sf_right = new StringFormat( );
            sf_right.Alignment = StringAlignment.Far;
            sf_right.LineAlignment = StringAlignment.Center;

            StringFormat sf_center = new StringFormat( );
            sf_center.LineAlignment = StringAlignment.Center;
            sf_center.Alignment = StringAlignment.Center;

            int padding_top = 20;
            int padding_left = 49;
            int padding_right = 49;
            int padding_down = 30;
            int sections = 9;

            // g.DrawLine( pen_Line, new Point( padding_left, padding_top ), new Point( padding_left, heigh - padding_down ) );
            
            float paint_height = heigh - padding_top - padding_down;
            float paint_width = width - padding_left - padding_right;

            if (array.Length > 1)
            {
                double Max = array.Max( );
                double Min = array.Min( );

                Max = Max - Min > 1 ? Max : Min + 1;
                double Length = Max - Min;
                
                //提取峰值
                List<float> Peaks = new List<float>( );
                if (array.Length >= 2)
                {
                    if (array[0] > array[1])
                    {
                        Peaks.Add( 0 );
                    }

                    for (int i = 1; i < array.Length - 2; i++)
                    {
                        if (array[i - 1] < array[i] && array[i] > array[i + 1])
                        {
                            Peaks.Add( i );
                        }
                    }
                    
                    if (array[array.Length - 1] > array[array.Length - 2])
                    {
                        Peaks.Add( array.Length - 1 );
                    }
                }
                

                //高400
                for (int i = 0; i < sections; i++)
                {
                    RectangleF rec = new RectangleF( -10f, (float)i / (sections - 1) * paint_height, padding_left + 8f, 20f );
                    double n = (sections - 1 - i) * Length / (sections - 1) + Min;
                    g.DrawString( n.ToString( "F1" ), Font_Normal, Brushes.Black, rec, sf_right );
                    g.DrawLine( 
                        pen_Dash, padding_left - 3, paint_height * i / (sections - 1) + padding_top,
                        width - padding_right, paint_height * i / (sections - 1) + padding_top );
                }

                float intervalX = paint_width / array.Length;                        // 横向间隔

                for (int i = 0; i < Peaks.Count; i++)
                {
                    if (array[(int)Peaks[i]] * 200 / Max > 1)
                    {
                        g.DrawLine( pen_Dash, Peaks[i] * intervalX + padding_left + 1, padding_top, Peaks[i] * intervalX + padding_left + 1, heigh - padding_down );
                        RectangleF rec = new RectangleF( Peaks[i] * intervalX + padding_left + 1 - 40, heigh - padding_down + 1, 80f, 20f );

                        g.DrawString( Peaks[i].ToString( ), Font_Normal, Brushes.DeepPink, rec, sf_center );
                    }
                }
                
                for (int i = 0; i < array.Length; i++)
                {
                    PointF point = new PointF( );
                    point.X = i * intervalX + padding_left + 1;
                    point.Y = (float)(paint_height - (array[i] - Min) * paint_height / Length + padding_top);

                    PointF point2 = new PointF( );
                    point2.X = i * intervalX + padding_left + 1;
                    point2.Y = (float)(paint_height - (Min - Min) * paint_height / Length + padding_top);
                    

                    g.DrawLine( Pens.Tomato, point, point2 );
                }
            }
            else
            {
                double Max = 100;
                double Min = 0;
                double Length = Max - Min;
                //高400
                for (int i = 0; i < sections; i++)
                {
                    RectangleF rec = new RectangleF( -10f, (float)i / (sections - 1) * paint_height, padding_left + 8f, 20f );
                    double n = (sections - 1 - i) * Length / (sections - 1) + Min;
                    g.DrawString( n.ToString( "F1" ), Font_Normal, Brushes.Black, rec, sf_right );
                    g.DrawLine(
                        pen_Dash, padding_left - 3, paint_height * i / (sections - 1) + padding_top,
                        width - padding_right, paint_height * i / (sections - 1) + padding_top );
                }
            }


            pen_Dash.Dispose( );
            pen_Line.Dispose( );
            pen_Fourier.Dispose( );
            Font_Normal.Dispose( );
            sf_right.Dispose( );
            sf_center.Dispose( );
            g.Dispose( );
            return bitmap;
        }

#endif


        /// <summary>
        /// 快速傅立叶变换
        /// </summary>
        /// <param name="xreal">实数部分，数组长度最好为2的n次方</param>
        /// <param name="ximag">虚数部分，数组长度最好为2的n次方</param>
        /// <returns>变换后的数组值</returns>
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
        /// <param name="xreal">实数部分，数组长度最好为2的n次方</param>
        /// <param name="ximag">虚数部分，数组长度最好为2的n次方</param>
        /// <returns>2的多少次方</returns>
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
