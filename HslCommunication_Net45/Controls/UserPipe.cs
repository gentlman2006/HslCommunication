using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace HslCommunication.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class UserPipe : UserControl
    {
        /// <summary>
        /// 管道控件信息
        /// </summary>
        public UserPipe( )
        {
            InitializeComponent( );
            DoubleBuffered = true;


            timer.Interval = 50;
            timer.Tick += Timer_Tick;
            timer.Start( );
        }

        /// <summary>
        /// 获取或设置管道线的宽度。
        /// </summary>
        [Browsable( true )]
        [Description( "获取或设置管道线的宽度" )]
        [Category( "外观" )]
        [DefaultValue(5f)]
        public float LineWidth
        {
            get { return lineWidth; }
            set
            {
                if (value > 0)
                {
                    lineWidth = value;
                    Invalidate( );
                }
            }
        }

        /// <summary>
        /// 获取或设置管道线是否处于活动状态。
        /// </summary>
        [Browsable( true )]
        [Description( "获取或设置管道线是否处于活动状态" )]
        [Category( "外观" )]
        [DefaultValue( true )]
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// 获取或设置管道活动状态的颜色。
        /// </summary>
        [Browsable( true )]
        [Description( "获取或设置管道活动状态的颜色" )]
        [Category( "外观" )]
        [DefaultValue( typeof(Color ), "Blue")]
        public Color ActiveColor
        {
            get { return activeColor; }
            set
            {
                activeColor = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// 获取或设置管道的背景色
        /// </summary>
        [Browsable( true )]
        [Description( "获取或设置管道的背景色" )]
        [Category( "外观" )]
        [DefaultValue( typeof( Color ), "(150, 150, 150 )" )]
        public Color LineColor
        {
            get { return lineColor; }
            set { lineColor = value; }
        }

        /// <summary>
        /// 获取或设置管道线的移动速度。该速度和管道的宽度有关
        /// </summary>
        [Browsable( true )]
        [Description( "获取或设置管道线的移动速度。该速度和管道的宽度有关" )]
        [Category( "外观" )]
        [DefaultValue( 1f )]
        public float MoveSpeed
        {
            get { return moveSpeed; }
            set
            {
                moveSpeed = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// 获取或设置管道线的坐标。
        /// </summary>
        [Browsable( true )]
        [Description( "获取或设置管道线的坐标，格式为0,0;1,1;2,2 分号间隔点" )]
        [DefaultValue( "" )]
        [Category( "外观" )]
        public string LinePoints
        {
            get
            {
                StringBuilder sb = new StringBuilder( );
                for (int i = 0; i < points.Count; i++)
                {
                    sb.Append( ";" );
                    sb.Append( points[i].X.ToString() );
                    sb.Append( "," );
                    sb.Append( points[i].Y.ToString( ) );

                }
                if (sb.Length > 0)
                    return sb.ToString( ).Substring( 1 );
                else return string.Empty;
            }
            set
            {
                try
                {
                    if (!string.IsNullOrEmpty( value ))
                    {
                        points.Clear( );
                        string[] all = value.Split( new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
                        for (int i = 0; i < all.Length; i++)
                        {
                            string[] data = all[i].Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
                            Point point = new Point( );
                            point.X = Convert.ToInt32( data[0] );
                            point.Y = Convert.ToInt32( data[1] );
                            points.Add( point );
                        }
                        Invalidate( );
                    }
                }
                catch
                {

                }
            }
        }

        private List<Point> points = new List<Point>( );
        private Timer timer = new Timer( );
        private float startOffect = 0;
        private float lineWidth = 5;
        private float moveSpeed = 1;
        private Color activeColor = Color.Blue;
        private bool isActive = true;
        private Color lineColor = Color.FromArgb( 150, 150, 150 );

        private void UserPipe_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // if (Width < 5 || Height < 5) return;

            Pen pen = new Pen( lineColor, lineWidth );
            if (points.Count > 1)
            {
                g.DrawLines( pen, points.ToArray() );
            }

            if (isActive)
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                pen.DashPattern = new float[] { 5, 5 };

                pen.DashOffset = startOffect;
                pen.Color = activeColor;
                if (points.Count > 1)
                {
                    g.DrawLines( pen, points.ToArray( ) );
                }
            }
            pen.Dispose( );
        }

        private void UserPipe_Load( object sender, EventArgs e )
        {
        }
        

        private void Timer_Tick( object sender, EventArgs e )
        {
            startOffect = startOffect - moveSpeed;
            if (startOffect <= -10 || startOffect >= 10) startOffect = 0;

            Invalidate( );
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="g"></param>
        public void OnPaintMainWindow( Graphics g )
        {
            g.TranslateTransform( this.Location.X, this.Location.Y );
            UserPipe_Paint( null, new PaintEventArgs( g, new Rectangle( ) ) );
            g.TranslateTransform( - this.Location.X, -this.Location.Y );
        }
    }

   
}
