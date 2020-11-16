using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Core;

namespace HslCommunication.Controls
{


    /// <summary>
    /// 一个饼图的控件
    /// </summary>
    public partial class UserPieChart : UserControl
    {
        /// <summary>
        /// 实例化一个饼图的控件
        /// </summary>
        public UserPieChart( )
        {
            InitializeComponent( );
            random = new Random( );
            DoubleBuffered = true;

            formatCenter = new StringFormat( );
            formatCenter.Alignment = StringAlignment.Center;
            formatCenter.LineAlignment = StringAlignment.Center;

            pieItems = new HslPieItem[0];
        }

        private void UserPieChart_Load( object sender, EventArgs e )
        {

        }

        #region Private
        
        private HslPieItem[] pieItems = new HslPieItem[0];                      // 饼图的数据
        private Random random = null;                                           // 随机数
        private StringFormat formatCenter = null;                               // 格式化的文本位置
        private int margin = 40;                                                // 边界距离
        private bool m_IsRenderPercent = false;                                 // 是否显示百分比
        private bool m_IsRenderSmall = true;                                    // 是否在图形上显示占比非常小的文本信息

        #endregion

        /// <summary>
        /// 是否显示百分比信息
        /// </summary>
        [Browsable( true )]
        [Category( "外观" )]
        [DefaultValue( false )]
        [Description( "获取或设置是否显示百分比占用" )]
        public bool IsRenderPercent
        {
            get { return m_IsRenderPercent; }
            set { m_IsRenderPercent = value; Invalidate( ); }
        }


        /// <summary>
        /// 是否在图形上显示占比非常小的文本信息
        /// </summary>
        [Browsable(true)]
        [Category("外观")]
        [Description("获取或设置是否显示占比很小的文本信息")]
        [DefaultValue(true)]
        public bool IsRenderSmall
        {
            get { return m_IsRenderSmall; }
            set { m_IsRenderSmall = value; Invalidate( ); }
        }




        private void SetMarginPaint( int value )
        {
            if (value > 500)
            {
                margin = 80;
            }
            else if (value > 300)
            {
                margin = 60;
            }
            else
            {
                margin = 40;
            }
        }

        private Point GetCenterPoint( out int width )
        {
            if (Width > Height)
            {
                SetMarginPaint( Height );
                width = Height / 2 - margin;
                return new Point( Height / 2 - 1, Height / 2 - 1 );
            }
            else
            {
                SetMarginPaint( Width );
                width = Width / 2 - margin;
                return new Point( Width / 2 - 1, Width / 2 - 1 );
            }
        }


        /// <summary>
        /// 随机生成颜色，该颜色相对于白色为深色颜色
        /// </summary>
        /// <returns></returns>
        private Color GetRandomColor( )
        {
            int int_Red = random.Next( 256 );
            int int_Green = random.Next( 256 );
            int int_Blue = (int_Red + int_Green > 430) ? random.Next( 100 ) : random.Next( 200 );
            return Color.FromArgb( int_Red, int_Green, int_Blue );
        }


        private void UserPieChart_Paint( object sender, PaintEventArgs e )
        {
            // 画刷初始化
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 开始绘制饼图，寻找中心点
            Point center = GetCenterPoint( out int width );
            Rectangle rect_center = new Rectangle( center.X - width, center.Y - width, width + width, width + width );

            if (width > 0 && pieItems.Length > 0)
            {
                e.Graphics.FillEllipse( Brushes.AliceBlue, rect_center );
                e.Graphics.DrawEllipse( Pens.DodgerBlue, rect_center );

                Rectangle rect_tran = new Rectangle( rect_center.X - center.X, rect_center.Y - center.Y, rect_center.Width, rect_center.Height );
                e.Graphics.TranslateTransform( center.X, center.Y );
                e.Graphics.RotateTransform( 90 );
                e.Graphics.DrawLine( Pens.DimGray, 0, 0, width, 0 );


                // 计算分布
                int totle = pieItems.Sum( item => item.Value );
                float totleAngle = 0;     // 本次绘制的角度
                float lastAngle = -90;    // 上一次的角度
                for (int i = 0; i < pieItems.Length; i++)
                {
                    float single = 0;
                    if (totle == 0)
                    {
                        single = 360 / pieItems.Length;
                    }
                    else
                    {
                        single = Convert.ToSingle( pieItems[i].Value * 1.0d / totle * 360 );
                    }


                    using (Brush brush = new SolidBrush( pieItems[i].Back ))
                    {
                        e.Graphics.FillPie( brush, rect_tran, 0, -single );
                    }
                    e.Graphics.RotateTransform( 0 - single / 2 );


                    if (single < 2f && !IsRenderSmall)
                    {
                        totleAngle += single;
                    }
                    else
                    {
                        // 画标线
                        totleAngle += single / 2;
                        int mark = 8;
                        if (totleAngle < 45 || totleAngle > 315) mark = 15;
                        if (totleAngle > 135 && totleAngle < 225) mark = 15;

                        e.Graphics.DrawLine( Pens.DimGray, width * 2 / 3, 0, width + mark, 0 );
                        e.Graphics.TranslateTransform( width + mark, 0 );

                        if (totleAngle - lastAngle < 5)
                        {
                            // 两次的标线非常接近

                        }
                        lastAngle = totleAngle;



                        if (totleAngle < 90)
                        {
                            e.Graphics.RotateTransform( totleAngle - 90 );
                            e.Graphics.DrawLine( Pens.DimGray, 0, 0, margin - mark, 0 );
                            e.Graphics.DrawString( pieItems[i].Name, Font, Brushes.DimGray, new Point( 0, -Font.Height ) );
                            if (IsRenderPercent) e.Graphics.DrawString( Math.Round( single * 100 / 360, 2 ).ToString( ) + "%", Font, Brushes.DodgerBlue, new Point( 0, 1 ) );
                            e.Graphics.RotateTransform( 90 - totleAngle );
                        }
                        else if (totleAngle < 180)
                        {
                            e.Graphics.RotateTransform( totleAngle - 90 );
                            e.Graphics.DrawLine( Pens.DimGray, 0, 0, margin - mark, 0 );
                            e.Graphics.DrawString( pieItems[i].Name, Font, Brushes.DimGray, new Point( 0, -Font.Height ) );
                            if (IsRenderPercent) e.Graphics.DrawString( Math.Round( single * 100 / 360, 2 ).ToString( ) + "%", Font, Brushes.DodgerBlue, new Point( 0, 1 ) );
                            e.Graphics.RotateTransform( 90 - totleAngle );
                        }
                        else if (totleAngle < 270)
                        {
                            e.Graphics.RotateTransform( totleAngle - 270 );
                            e.Graphics.DrawLine( Pens.DimGray, 0, 0, margin - mark, 0 );
                            e.Graphics.TranslateTransform( margin - 8, 0 );
                            e.Graphics.RotateTransform( 180 );
                            e.Graphics.DrawString( pieItems[i].Name, Font, Brushes.DimGray, new Point( 0, -Font.Height ) );
                            if (IsRenderPercent) e.Graphics.DrawString( Math.Round( single * 100 / 360, 2 ).ToString( ) + "%", Font, Brushes.DodgerBlue, new Point( 0, 1 ) );
                            e.Graphics.RotateTransform( -180 );
                            e.Graphics.TranslateTransform( 8 - margin, 0 );
                            e.Graphics.RotateTransform( 270 - totleAngle );
                        }
                        else
                        {
                            e.Graphics.RotateTransform( totleAngle - 270 );
                            e.Graphics.DrawLine( Pens.DimGray, 0, 0, margin - mark, 0 );
                            e.Graphics.TranslateTransform( margin - 8, 0 );
                            e.Graphics.RotateTransform( 180 );
                            e.Graphics.DrawString( pieItems[i].Name, Font, Brushes.DimGray, new Point( 0, -Font.Height ) );
                            if (IsRenderPercent) e.Graphics.DrawString( Math.Round( single * 100 / 360, 2 ).ToString( ) + "%", Font, Brushes.DodgerBlue, new Point( 0, 1 ) );
                            e.Graphics.RotateTransform( -180 );
                            e.Graphics.TranslateTransform( 8 - margin, 0 );
                            e.Graphics.RotateTransform( 270 - totleAngle );
                        }

                        e.Graphics.TranslateTransform( -width - mark, 0 );
                        e.Graphics.RotateTransform( 0 - single / 2 );
                        totleAngle += single / 2;
                    }
                }
                e.Graphics.ResetTransform( );
            }
            else
            {
                e.Graphics.FillEllipse( Brushes.AliceBlue, rect_center );
                e.Graphics.DrawEllipse( Pens.DodgerBlue, rect_center );
                e.Graphics.DrawString( "空", Font, Brushes.DimGray, rect_center, formatCenter );
            }
        }




        #region Public Method

        /// <summary>
        /// 设置显示的数据源
        /// </summary>
        /// <param name="source">特殊的显示对象</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetDataSource( HslPieItem[] source )
        {
            if (source != null)
            {
                pieItems = source;
                Invalidate( );
            }
        }

        /// <summary>
        /// 根据名称和值进行数据源的显示，两者的长度需要一致
        /// </summary>
        /// <param name="names">名称</param>
        /// <param name="values">值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetDataSource( string[] names, int[] values )
        {
            if (names == null) throw new ArgumentNullException( "names" );
            if (values == null) throw new ArgumentNullException( "values" );
            if (names.Length != values.Length) throw new Exception( "两个数组的长度不一致！" );

            pieItems = new HslPieItem[names.Length];

            for (int i = 0; i < names.Length; i++)
            {
                pieItems[i] = new HslPieItem( )
                {
                    Name = names[i],
                    Value = values[i],
                    Back = GetRandomColor( ),
                };
            }

            Invalidate( );
        }

        #endregion






    }


}
