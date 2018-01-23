using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


/*******************************************************************************
 * 
 *    文件名：UserCurve.cs
 *    程序功能：显示曲线信息，包含了曲线，坐标轴，鼠标移动信息
 *    
 *    创建人：Richard.Hu
 *    时间：2018年1月21日 18:36:08
 * 
 *******************************************************************************/


namespace HslCommunication.Controls
{
    /// <summary>
    /// 曲线控件对象
    /// </summary>
    /// <remarks>
    /// 详细参照如下的博客:
    /// </remarks>
    public partial class UserCurve : UserControl
    {
        #region Constructor

        /// <summary>
        /// 实例化一个曲线显示的控件
        /// </summary>
        public UserCurve( )
        {
            InitializeComponent( );
            DoubleBuffered = true;
            random = new Random( );
            data_list = new Dictionary<string, HslCurveItem>( );

            format_left = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near
            };

            format_right = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Far,
            };

            format_center = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center,
            };

        }

        #endregion

        #region Const Data

        private const int value_count_max = 2048;           // 缓存的数据的最大量

        #endregion

        #region Private Member

        private float value_Max = 100;                      // 坐标的最大值
        private float value_Min = 0;                        // 坐标的最小值
        private int value_Segment = 5;                      // 纵轴的片段分割
        private bool value_IsAbscissaStrech = false;        // 指示横坐标是否填充满整个坐标系
        private bool value_IsRenderDashLine = true;         // 是否显示虚线的信息
        private bool value_IsRenderAbscissaText = false;    // 指示是否显示横轴的文本信息
        private int value_IntervalAbscissaText = 100;       // 指示显示横轴文本的间隔数据
        private Random random = null;                       // 获取随机颜色使用




        private int leftRight = 50;
        private int upDowm = 25;



        #endregion

        #region Data Member

        Dictionary<string, HslCurveItem> data_list = null;  // 等待显示的实际数据
        private string[] data_text = null;                  // 等待显示的横轴信息

        #endregion

        #region Public Method


        /// <summary>
        /// 新增或修改一条指定关键字的曲线数据，使用指定的数据，颜色随机，线条宽度为1
        /// </summary>
        /// <param name="key">曲线关键字</param>
        /// <param name="data"></param>
        public void SetCurve( string key, float[] data )
        {
            SetCurve( key, data, Color.FromArgb( random.Next( 256 ), random.Next( 256 ), random.Next( 256 ) ) );
        }

        /// <summary>
        /// 新增或修改一条指定关键字的曲线数据，使用指定的数据，颜色，线条宽度为1
        /// </summary>
        /// <param name="key">曲线关键字</param>
        /// <param name="data"></param>
        /// <param name="lineColor"></param>
        public void SetCurve( string key, float[] data, Color lineColor )
        {
            SetCurve( key, data, lineColor, 1f );
        }

        /// <summary>
        /// 新增或修改一条指定关键字的曲线数据，使用指定的数据，颜色，线条宽度
        /// </summary>
        /// <param name="key">曲线关键字</param>
        /// <param name="data"></param>
        /// <param name="lineColor"></param>
        /// <param name="thickness"></param>
        public void SetCurve( string key, float[] data, Color lineColor ,float thickness)
        {
            if (data_list.ContainsKey( key ))
            {
                data_list[key].Data = data;
            }
            else
            {
                data_list.Add( key, new HslCurveItem( )
                {
                    Data = data,
                    LineThickness = thickness,
                    LineColor = lineColor,
                    ValueMax = value_Max,
                    ValueMin = value_Min
                } );
            }

            // 重绘图形
            Invalidate( );
        }


        /// <summary>
        /// 移除指定关键字的曲线
        /// </summary>
        /// <param name="key">曲线关键字</param>
        public void RemoveCurve( string key )
        {
            if (data_list.ContainsKey( key ))
            {
                data_list.Remove( key );
            }
            // 重绘图形
            Invalidate( );
        }





        // ======================================================================================================

        /// <summary>
        /// 新增指定关键字曲线的一个数据，注意该关键字的曲线必须存在，否则无效
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="isUpdateUI">是否刷新界面</param>
        private void AddCurveData( string key, float[] values ,bool isUpdateUI)
        {
            if (values?.Length < 1) return;                              // 没有传入数据
            int length = value_count_max;                                // 获取最大数据量

            if (data_list.ContainsKey( key ))
            {
                HslCurveItem curve = data_list[key];
                if (curve.Data != null)
                {
                    if (value_IsAbscissaStrech)
                    {
                        // 填充玩整个图形的情况
                        float[] tmp = new float[curve.Data.Length + values.Length];
                        for (int i = 0; i < curve.Data.Length; i++)
                        {
                            tmp[i] = curve.Data[i];
                        }
                        for (int i = 0; i < values.Length; i++)
                        {
                            tmp[tmp.Length - values.Length + i] = values[i];
                        }
                        curve.Data = tmp;
                    }
                    else
                    {
                        // 指定点的情况
                        if(curve.Data.Length == length)
                        {
                            // 点数相同
                            for (int i = 0; i < curve.Data.Length - values.Length; i++)
                            {
                                curve.Data[i] = curve.Data[i + values.Length];
                            }
                            for (int i = 0; i < values.Length; i++)
                            {
                                curve.Data[curve.Data.Length - values.Length + i] = values[i];
                            }
                        }
                        else if(curve.Data.Length < length)
                        {
                            // 点数比较小
                            if ((curve.Data.Length + values.Length) > length)
                            {
                                float[] tmp = new float[length];
                                for (int i = 0; i < (length - values.Length); i++)
                                {
                                    tmp[i] = curve.Data[i + (curve.Data.Length - length + values.Length)];
                                }
                                for (int i = 0; i < values.Length; i++)
                                {
                                    tmp[tmp.Length - values.Length + i] = values[i];
                                }
                                curve.Data = tmp;
                            }
                            else
                            {
                                float[] tmp = new float[curve.Data.Length + values.Length];
                                for (int i = 0; i < curve.Data.Length; i++)
                                {
                                    tmp[i] = curve.Data[i];
                                }
                                for (int i = 0; i < values.Length; i++)
                                {
                                    tmp[tmp.Length - values.Length + i] = values[i];
                                }
                                curve.Data = tmp;
                            }
                        }
                    }
                }
            }
        }



        

        #endregion
        
        #region Private Method

        
        

        #endregion

        #region Core Paint


        private Font font_size9 = null;
        private Font font_size12 = null;
        
        private Brush brush_deep = null;

        private Pen pen_normal = null;                    // 绘制极轴和分段符的坐标线
        private Pen pen_dash = null;                      // 绘制图形的虚线

        private Color color_normal = Color.DeepPink;
        private Color color_deep = Color.DimGray;

        private StringFormat format_left = null;          // 靠左对齐的文本
        private StringFormat format_right = null;         // 靠右对齐的文本
        private StringFormat format_center = null;        // 中间对齐的文本


        private void UserCurve_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            if (BackColor != Color.Transparent)
            {
                g.Clear( BackColor );
            }

            font_size9 = new Font( "宋体", 9 );
            font_size12 = new Font( "宋体", 12 );
            pen_normal = new Pen( color_deep );
            pen_dash = new Pen( color_deep );
            pen_dash.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            pen_dash.DashPattern = new float[] { 5, 5 };
            brush_deep = new SolidBrush( color_deep );
            

            int width_totle = Width;
            int heigh_totle = Height;

            if (width_totle < 120 || heigh_totle < 60) goto End;


            // 绘制极轴
            g.DrawLines( pen_normal, new Point[] {
                new Point(leftRight-1, upDowm - 8),
                new Point(leftRight-1, heigh_totle - upDowm),
                new Point(width_totle - leftRight, heigh_totle - upDowm),
                new Point(width_totle - leftRight, upDowm - 8)
            } );

            // 绘制倒三角
            BasicFramework.SoftPainting.PaintTriangle( g, brush_deep, new Point( leftRight - 1, upDowm - 8 ), 4, BasicFramework.GraphDirection.Upward );
            BasicFramework.SoftPainting.PaintTriangle( g, brush_deep, new Point( width_totle - leftRight, upDowm - 8 ), 4, BasicFramework.GraphDirection.Upward );

            // 绘制刻度线，以及刻度文本
            for (int i = 0; i <= value_Segment; i++)
            {
                float valueTmp = i * value_Max / value_Segment + value_Min;
                float paintTmp = BasicFramework.SoftPainting.ComputePaintLocationY( value_Max, value_Min, (heigh_totle - upDowm - upDowm), valueTmp );
                g.DrawLine( pen_normal, leftRight - 4, paintTmp + upDowm, leftRight - 1, paintTmp + upDowm );
                RectangleF rectTmp = new RectangleF( 0, paintTmp - 9 + upDowm, leftRight - 4, 20 );
                g.DrawString( valueTmp.ToString( ), font_size9, brush_deep, rectTmp, format_right );

                g.DrawLine( pen_normal, width_totle - leftRight + 1, paintTmp + upDowm, width_totle - leftRight + 4, paintTmp + upDowm );
                rectTmp.Location = new PointF( width_totle - leftRight + 4, paintTmp - 9 + upDowm);
                g.DrawString( valueTmp.ToString( ), font_size9, brush_deep, rectTmp, format_left );

                if (i > 0 && value_IsRenderDashLine) g.DrawLine( pen_dash, leftRight, paintTmp + upDowm, width_totle - leftRight, paintTmp + upDowm );
            }

            // 绘制纵向虚线信息
            if (value_IsRenderDashLine)
            {
                for (int i = leftRight + value_IntervalAbscissaText; i < width_totle - leftRight; i += value_IntervalAbscissaText)
                {
                    g.DrawLine( pen_dash, i, upDowm, i, heigh_totle - upDowm - 1 );
                }
            }


            // 绘制线条
            if(value_IsAbscissaStrech)
            {
                // 横坐标充满图形
                foreach(var line in data_list.Values)
                {
                    if (line.Data?.Length > 1)
                    {
                        float offect = (width_totle - leftRight * 2) * 1.0f / (line.Data.Length - 1);
                        // 点数大于1的时候才绘制
                        PointF[] points = new PointF[line.Data.Length];
                        for (int i = 0; i < line.Data.Length; i++)
                        {
                            points[i].X = leftRight + i * offect;
                            points[i].Y = BasicFramework.SoftPainting.ComputePaintLocationY(line.ValueMax, line.ValueMin, (heigh_totle - upDowm - upDowm), line.Data[i]) + upDowm;
                        }

                        using (Pen penTmp = new Pen(line.LineColor, line.LineThickness))
                        {
                            g.DrawLines(penTmp, points);
                        }
                    }
                }
            }
            else
            {
                // 横坐标对应图形
                foreach (var line in data_list.Values)
                {
                    if (line.Data?.Length > 1)
                    {
                        int countTmp = width_totle - 2 * leftRight + 1;
                        // 点数大于1的时候才绘制
                        PointF[] points = new PointF[line.Data.Length];
                        for (int i = 0; i < line.Data.Length; i++)
                        {
                            if (i <= countTmp)
                            {
                                points[i].X = leftRight + i;
                                points[i].Y = BasicFramework.SoftPainting.ComputePaintLocationY(line.ValueMax, line.ValueMin, (heigh_totle - upDowm - upDowm), line.Data[i]) + upDowm;
                            }
                            else
                            {
                                break;
                            }
                        }

                        using (Pen penTmp = new Pen(line.LineColor, line.LineThickness))
                        {
                            g.DrawLines(penTmp, points);
                        }
                    }
                }
            }


            End:
            pen_normal.Dispose( );
            pen_dash.Dispose( );
            brush_deep.Dispose( );
            font_size9.Dispose( );
            font_size12.Dispose( );
        }

        #endregion

    }


    /// <summary>
    /// 曲线数据对象
    /// </summary>
    internal class HslCurveItem
    {
        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public HslCurveItem()
        {
            LineThickness = 1.0f;
        }


        /// <summary>
        /// 数据
        /// </summary>
        public float[] Data { get; set; }

        /// <summary>
        /// 线条的宽度
        /// </summary>
        public float LineThickness { get; set; } 

        /// <summary>
        /// 曲线颜色
        /// </summary>
        public Color LineColor { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public float ValueMax { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        public float ValueMin { get; set; }
        
    }
}
