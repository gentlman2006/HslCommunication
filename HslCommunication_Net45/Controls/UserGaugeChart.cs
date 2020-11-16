using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace HslCommunication.Controls
{


    /// <summary>
    /// 仪表盘控件类
    /// </summary>
    public partial class UserGaugeChart : UserControl
    {
        /// <summary>
        /// 实例化一个仪表盘控件
        /// </summary>
        public UserGaugeChart( )
        {
            InitializeComponent( );


            pen_gauge_border = new Pen( color_gauge_border );
            brush_gauge_pointer = new SolidBrush( color_gauge_pointer );
            centerFormat = new StringFormat( );
            centerFormat.Alignment = StringAlignment.Center;
            centerFormat.LineAlignment = StringAlignment.Center;

            pen_gauge_alarm = new Pen( Color.OrangeRed, 3f );
            pen_gauge_alarm.DashStyle = DashStyle.Custom;
            pen_gauge_alarm.DashPattern = new float[] { 5, 1 };

            hybirdLock = new Core.SimpleHybirdLock( );
            m_UpdateAction = new Action( Invalidate );
            timer_alarm_check = new Timer( );
            timer_alarm_check.Tick += Timer_alarm_check_Tick;
            timer_alarm_check.Interval = 1000;

            DoubleBuffered = true;

        }

        private void Timer_alarm_check_Tick( object sender, EventArgs e )
        {
            if (value_current > value_alarm_max || value_current < value_alarm_min)
            {
                alarm_check = !alarm_check;
            }
            else
            {
                alarm_check = false;
            }

            Invalidate( );
        }

        private void UserGaugeChart_Load( object sender, EventArgs e )
        {
            timer_alarm_check.Start( );
        }

        private void UserGaugeChart_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;                // 消除锯齿
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;        // 优化文本显示

            OperateResult<Point, int, double> setting = GetCenterPoint( );
            if (!setting.IsSuccess) return;                                                      // 不满足绘制条件

            Point center = setting.Content1;
            int radius = setting.Content2;
            float angle = Convert.ToSingle( setting.Content3 );
            Rectangle circular = new Rectangle( -radius, -radius, 2 * radius, 2 * radius );
            Rectangle circular_larger = new Rectangle( -radius - 5, -radius - 5, 2 * radius + 10, 2 * radius + 10 );
            Rectangle circular_mini = new Rectangle( -radius / 3, -radius / 3, 2 * radius / 3, 2 * radius / 3 );

            g.TranslateTransform( center.X, center.Y );

            g.DrawArc( pen_gauge_border, circular_mini, -angle, angle * 2 - 180 );
            g.DrawArc( pen_gauge_border, circular, angle - 180, 180 - angle * 2 );
            g.DrawLine( pen_gauge_border, (int)(-(radius / 3) * Math.Cos( angle / 180 * Math.PI )), -(int)((radius / 3) * Math.Sin( angle / 180 * Math.PI )), -(int)((radius - 30) * Math.Cos( angle / 180 * Math.PI )), -(int)((radius - 30) * Math.Sin( angle / 180 * Math.PI )) );
            g.DrawLine( pen_gauge_border, (int)((radius - 30) * Math.Cos( angle / 180 * Math.PI )), -(int)((radius - 30) * Math.Sin( angle / 180 * Math.PI )), (int)((radius / 3) * Math.Cos( angle / 180 * Math.PI )), -(int)((radius / 3) * Math.Sin( angle / 180 * Math.PI )) );

            // 开始绘制刻度
            g.RotateTransform( angle - 90 );
            int totle = segment_count;
            for (int i = 0; i <= totle; i++)
            {
                Rectangle rect = new Rectangle( -2, -radius, 3, 7 );
                g.FillRectangle( Brushes.DimGray, rect );
                rect = new Rectangle( -50, -radius + 7, 100, 20 );

                double current = ValueStart + (ValueMax - ValueStart) * i / totle;
                g.DrawString( current.ToString( ), Font, Brushes.DodgerBlue, rect, centerFormat );
                g.RotateTransform( (180 - 2 * angle) / totle / 2 );
                if (i != totle) g.DrawLine( Pens.DimGray, 0, -radius, 0, -radius + 3 );
                g.RotateTransform( (180 - 2 * angle) / totle / 2 );
            }
            g.RotateTransform( -(180 - 2 * angle) / totle );
            g.RotateTransform( angle - 90 );

            Rectangle text = new Rectangle( -36, -(radius * 2 / 3 - 3), 72, Font.Height + 3 );

            // 如果处于报警中，就闪烁
            if (value_current > value_alarm_max || value_current < value_alarm_min)
            {
                if (alarm_check)
                {
                    g.FillRectangle( Brushes.OrangeRed, text );
                }
            }

            // g.FillRectangle(Brushes.Wheat, text);
            if (IsTextUnderPointer)
            {
                g.DrawString( Value.ToString( ), Font, Brushes.DimGray, text, centerFormat );
                // g.DrawRectangle(pen_gauge_border, text);
                text.Offset( 0, Font.Height );
                if (!string.IsNullOrEmpty( UnitText ))
                {
                    g.DrawString( UnitText, Font, Brushes.Gray, text, centerFormat );
                }
            }

            g.RotateTransform( angle - 90 );
            g.RotateTransform( (float)((value_paint - ValueStart) / (ValueMax - ValueStart) * (180 - 2 * angle)) );

            Rectangle rectangle = new Rectangle( -5, -5, 10, 10 );
            g.FillEllipse( brush_gauge_pointer, rectangle );
            // g.DrawEllipse(Pens.Red, rectangle);
            Point[] points = new Point[] { new Point( 5, 0 ), new Point( 2, -radius + 40 ), new Point( 0, -radius + 20 ), new Point( -2, -radius + 40 ), new Point( -5, 0 ) };
            g.FillPolygon( brush_gauge_pointer, points );
            // g.DrawLines(Pens.Red, points);

            g.RotateTransform( (float)(-(value_paint - ValueStart) / (ValueMax - ValueStart) * (180 - 2 * angle)) );
            g.RotateTransform( 90 - angle );

            if (value_alarm_min > ValueStart && value_alarm_min <= ValueMax)
            {
                g.DrawArc( pen_gauge_alarm, circular_larger, angle - 180, (float)((ValueAlarmMin - ValueStart) / (ValueMax - ValueStart) * (180 - 2 * angle)) );
            }

            if (value_alarm_max >= ValueStart && value_alarm_max < ValueMax)
            {
                float angle_max = (float)((value_alarm_max - ValueStart) / (ValueMax - ValueStart) * (180 - 2 * angle));
                g.DrawArc( pen_gauge_alarm, circular_larger, -180 + angle + angle_max, 180 - 2 * angle - angle_max );
            }

            if (!IsTextUnderPointer)
            {
                g.DrawString( Value.ToString( ), Font, Brushes.DimGray, text, centerFormat );
                // g.DrawRectangle(pen_gauge_border, text);
                text.Offset( 0, Font.Height );
                if (!string.IsNullOrEmpty( UnitText ))
                {
                    g.DrawString( UnitText, Font, Brushes.Gray, text, centerFormat );
                }
            }

            g.ResetTransform( );


        }


        /// <summary>
        /// 获取中心点的坐标
        /// </summary>
        /// <returns></returns>
        private OperateResult<Point, int, double> GetCenterPoint( )
        {
            OperateResult<Point, int, double> result = new OperateResult<Point, int, double>( );
            if (Height <= 35) return result;
            if (Width <= 20) return result;

            result.IsSuccess = true;
            if (!IsBigSemiCircle)
            {
                // 以纵轴为标准创建的图像，特点是小于半圆~半圆变化
                result.Content2 = Height - 30;
                if ((Width - 40) / 2d > result.Content2)
                {
                    result.Content3 = Math.Acos( 1 ) * 180 / Math.PI;
                }
                else
                {
                    result.Content3 = Math.Acos( (Width - 40) / 2d / (Height - 30) ) * 180 / Math.PI;
                }
                result.Content1 = new Point( Width / 2, Height - 10 );
                return result;
            }
            else
            {
                // 以横轴为标准创建的图像，特点是半圆~整圆变化
                result.Content2 = (Width - 40) / 2;
                if ((Height - 30) < result.Content2)
                {
                    result.Content2 = Height - 30;
                    result.Content3 = Math.Acos( 1 ) * 180 / Math.PI;
                    result.Content1 = new Point( Width / 2, Height - 10 );
                    return result;
                }
                else
                {
                    int left = Height - 30 - result.Content2;
                    if (left > result.Content2) left = result.Content2;
                    result.Content3 = -Math.Asin( left * 1.0d / result.Content2 ) * 180 / Math.PI;
                    result.Content1 = new Point( Width / 2, result.Content2 + 20 );
                    return result;
                }
            }
        }

        private void ThreadPoolUpdateProgress( object obj )
        {
            try
            {
                // 开始计算更新细节
                int version = (int)obj;

                if (value_paint == value_current) return;
                double m_speed = Math.Abs( value_paint - value_current ) / 10;
                if (m_speed == 0) m_speed = 1;

                while (value_paint != value_current)
                {
                    System.Threading.Thread.Sleep( 17 );
                    if (version != m_version) break;

                    hybirdLock.Enter( );

                    double newActual = 0;
                    if (value_paint > value_current)
                    {
                        double offect = value_paint - value_current;
                        if (offect > m_speed) offect = m_speed;
                        newActual = value_paint - offect;
                    }
                    else
                    {
                        double offect = value_current - value_paint;
                        if (offect > m_speed) offect = m_speed;
                        newActual = value_paint + offect;
                    }
                    value_paint = newActual;

                    hybirdLock.Leave( );

                    if (version == m_version)
                    {
                        if (IsHandleCreated) Invoke( m_UpdateAction );
                    }
                    else
                    {
                        break;
                    }

                }
            }
            catch (Exception)
            {
                // BasicFramework.SoftBasic.ShowExceptionMessage(ex);
            }

        }



        #region Private Member

        private Color color_gauge_border = Color.DimGray;
        private Pen pen_gauge_border = null;                                  // 绘制仪表盘的边框色


        private Color color_gauge_pointer = Color.Tomato;
        private Brush brush_gauge_pointer = null;                             // 绘制仪表盘的指针的颜色

        private double value_start = 0;                                       // 仪表盘的初始值
        private double value_max = 100d;                                      // 仪表盘的结束值
        private double value_current = 0d;                                    // 仪表盘的当前值
        private double value_alarm_max = 80d;                                 // 仪表盘的上限报警值
        private double value_alarm_min = 20d;                                 // 仪表盘的下限报警值
        private Pen pen_gauge_alarm = null;                                   // 绘制仪表盘的报警区间的虚线画笔

        private int m_version = 0;                                            // 设置数据时的版本，用于更新时的版本验证
        private double value_paint = 0d;                                      // 绘制图形时候的中间值
        private Core.SimpleHybirdLock hybirdLock;                             // 数据的同步锁
        private Action m_UpdateAction;                                        // 更新界面的委托
        private Timer timer_alarm_check;                                      // 数据处于危险区域的报警闪烁
        private bool alarm_check = false;                                     // 每秒计时的报警反馈

        private int segment_count = 10;                                       // 显示区域的分割片段
        private StringFormat centerFormat = null;                             // 居中显示的格式化文本
        private string value_unit_text = string.Empty;                        // 数值的单位，可以设置并显示
        private bool text_under_pointer = true;                               // 指示文本是否在指针的下面
        private bool isBigSemiCircle = false;                                 // 是否显示超过半个圆的信息

        #endregion


        #region Public Member

        /// <summary>
        /// 获取或设置仪表盘的背景色
        /// </summary>
        [Browsable( true )]
        [Category( "外观" )]
        [Description( "获取或设置仪表盘的背景色" )]
        [DefaultValue( typeof( Color ), "DimGray" )]
        public Color GaugeBorder
        {
            get { return color_gauge_border; }
            set
            {
                pen_gauge_border?.Dispose( );
                pen_gauge_border = new Pen( value );
                color_gauge_border = value;
                Invalidate( );
            }
        }


        /// <summary>
        /// 获取或设置指针的颜色
        /// </summary>
        [Browsable( true )]
        [Category( "外观" )]
        [Description( "获取或设置仪表盘指针的颜色" )]
        [DefaultValue( typeof( Color ), "Tomato" )]
        public Color PointerColor
        {
            get
            {
                return color_gauge_pointer;
            }
            set
            {
                brush_gauge_pointer?.Dispose( );
                brush_gauge_pointer = new SolidBrush( value );
                color_gauge_pointer = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// 获取或设置数值的起始值，默认为0
        /// </summary>
        [Browsable( true )]
        [Category( "外观" )]
        [Description( "获取或设置数值的起始值，默认为0" )]
        [DefaultValue( 0d )]
        public double ValueStart
        {
            get
            {
                if (value_max <= value_start)
                {
                    return value_start + 1;
                }
                else
                {
                    return value_start;
                }
            }
            set
            {
                value_start = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// 获取或设置数值的最大值，默认为100
        /// </summary>
        [Browsable( true )]
        [Category( "外观" )]
        [Description( "获取或设置数值的最大值，默认为100" )]
        [DefaultValue( 100d )]
        public double ValueMax
        {
            get
            {
                if (value_max <= value_start)
                {
                    return value_start + 1;
                }
                else
                {
                    return value_max;
                }
            }
            set
            {
                value_max = value;
                Invalidate( );
            }
        }


        /// <summary>
        /// 获取或设置数值的当前值，应该处于最小值和最大值之间
        /// </summary>
        [Browsable( true )]
        [Category( "外观" )]
        [Description( "获取或设置数值的当前值，默认为0" )]
        [DefaultValue( 0d )]
        public double Value
        {
            get
            {
                return value_current;
            }
            set
            {
                if (ValueStart <= value && value <= ValueMax)
                {
                    if (value != value_current)
                    {
                        value_current = value;
                        int version = System.Threading.Interlocked.Increment( ref m_version );
                        System.Threading.ThreadPool.QueueUserWorkItem( new System.Threading.WaitCallback(
                            ThreadPoolUpdateProgress ), version );
                    }
                }
            }
        }


        /// <summary>
        /// 获取或设置数值的上限报警值，设置为超过最大值则无上限报警
        /// </summary>
        [Browsable( true )]
        [Category( "外观" )]
        [Description( "获取或设置数值的上限报警值，设置为超过最大值则无上限报警，默认为80" )]
        [DefaultValue( 80d )]
        public double ValueAlarmMax
        {
            get
            {
                return value_alarm_max;
            }
            set
            {
                if (ValueStart <= value)
                {
                    value_alarm_max = value;
                    Invalidate( );
                }
            }
        }


        /// <summary>
        /// 获取或设置数值的下限报警值，设置为超过最大值则无上限报警
        /// </summary>
        [Browsable( true )]
        [Category( "外观" )]
        [Description( "获取或设置数值的下限报警值，设置为小于最小值则无下限报警，默认为20" )]
        [DefaultValue( 20d )]
        public double ValueAlarmMin
        {
            get
            {
                return value_alarm_min;
            }
            set
            {
                if (value <= ValueMax)
                {
                    value_alarm_min = value;
                    Invalidate( );
                }
            }
        }


        /// <summary>
        /// 获取或设置仪表盘的分割段数，最小为2，最大1000
        /// </summary>
        [Browsable( true )]
        [Category( "外观" )]
        [Description( "获取或设置仪表盘的分割段数，最小为2，最大1000" )]
        [DefaultValue( 10 )]
        public int SegmentCount
        {
            get
            {
                return segment_count;
            }
            set
            {
                if (value > 1 && value < 1000)
                {
                    segment_count = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置仪表盘的单位描述文本
        /// </summary>
        [Browsable( true )]
        [Category( "外观" )]
        [Description( "获取或设置仪表盘的单位描述文本" )]
        [DefaultValue( "" )]
        public string UnitText
        {
            get
            {
                return value_unit_text;
            }
            set
            {
                value_unit_text = value;
                Invalidate( );
            }
        }


        /// <summary>
        /// 获取或设置文本是否是指针的下面
        /// </summary>
        [Browsable( true )]
        [Category( "外观" )]
        [Description( "获取或设置文本是否是指针的下面" )]
        [DefaultValue( true )]
        public bool IsTextUnderPointer
        {
            get
            {
                return text_under_pointer;
            }
            set
            {
                text_under_pointer = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// 通常情况，仪表盘不会大于半个圆，除非本属性设置为 True
        /// </summary>
        [Browsable( true )]
        [Category( "外观" )]
        [Description( "通常情况，仪表盘不会大于半个圆，除非本属性设置为 True" )]
        [DefaultValue( false )]
        public bool IsBigSemiCircle
        {
            get
            {
                return isBigSemiCircle;
            }
            set
            {
                isBigSemiCircle = value;
                Invalidate( );
            }
        }

        #endregion


    }
}
