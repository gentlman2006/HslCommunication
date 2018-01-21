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

        #region Private Member

        private float value_Max = 100;                      // 坐标的最大值
        private float value_Min = 0;                        // 坐标的最小值
        private int value_Segment = 5;                      // 纵轴的片段分割
        private bool value_IsAbscissaStrech = false;        // 指示横坐标是否填充满整个坐标系
        private bool value_IsRenderAbscissaText = false;    // 指示是否显示横轴的文本信息
        private int value_IntervalAbscissaText = 100;        // 指示显示横轴文本的间隔数据
        private Random random = null;                       // 获取随机颜色使用

        #endregion

        #region Data Member

        Dictionary<string, HslCurveItem> data_list = null;  // 等待显示的实际数据
        private string[] data_text = null;                  // 等待显示的横轴信息

        #endregion

        #region Public Method


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void AddCurve( string key, float[] data )
        {
            AddCurve( key, data, Color.FromArgb( random.Next( 256 ), random.Next( 256 ), random.Next( 256 ) ) );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="lineColor"></param>
        public void AddCurve( string key, float[] data, Color lineColor )
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
                    LineColor = lineColor,
                    ValueMax = value_Max,
                    ValueMin = value_Min
                } );
            }

            // 重绘图形
            Invalidate( );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void RemoveCurve( string key )
        {

        }



        #endregion


        #region Core Paint


        private Font font_size9 = null;
        private Font font_size12 = null;

        private Brush brush_normal = null;
        private Brush brush_deep = null;

        private Pen pen_normal = null;                    // 绘制极轴和分段符的坐标线
        private Pen pen_deep = null;
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
            int leftRight = 50, upDowm = 25;
            g.DrawLines( pen_normal, new Point[] {
                new Point(leftRight-1, upDowm - 8),
                new Point(leftRight-1, heigh_totle - upDowm),
                new Point(width_totle - leftRight, heigh_totle - upDowm),
                new Point(width_totle - leftRight, upDowm - 8)
            } );
            // 绘制倒三角
            BasicFramework.SoftPainting.PaintTriangle( g, brush_deep, new Point( leftRight - 1, upDowm - 8 ), 8, BasicFramework.GraphDirection.Upward );
            BasicFramework.SoftPainting.PaintTriangle( g, brush_deep, new Point( width_totle - leftRight, upDowm - 8 ), 8, BasicFramework.GraphDirection.Upward );

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

                if (i > 0) g.DrawLine( pen_dash, leftRight, paintTmp + upDowm, width_totle - leftRight, paintTmp + upDowm );
            }

            // 绘制纵向虚线信息
            for (int i = leftRight + value_IntervalAbscissaText; i < width_totle - leftRight; i+= value_IntervalAbscissaText)
            {
                g.DrawLine( pen_dash, i, upDowm, i, heigh_totle - upDowm - 1 );
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
        /// 数据
        /// </summary>
        public float[] Data { get; set; }

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
