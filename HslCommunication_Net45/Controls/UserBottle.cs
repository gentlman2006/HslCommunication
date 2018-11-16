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
    /// 瓶子控件
    /// </summary>
    public partial class UserBottle : UserControl
    {
        /// <summary>
        /// 实例化一个新的控件对象
        /// </summary>
        public UserBottle( )
        {
            InitializeComponent( );
            DoubleBuffered = true;
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
        }


        /// <summary>
        /// 获取或设置瓶子的液位值。
        /// </summary>
        [Browsable( true )]
        [DefaultValue( typeof( double ), "60" )]
        [Category( "外观" )]
        public double Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    this.value = value;
                    Invalidate( );
                }
            }
        }

        /// <summary>
        /// 获取或设置瓶子是否处于打开的状态。
        /// </summary>
        [Browsable( true )]
        [DefaultValue( typeof( bool ), "false" )]
        [Category( "外观" )]
        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                isOpen = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// 获取或设置瓶子的标签信息，用于绘制在瓶子上的信息。
        /// </summary>
        [Browsable( true )]
        [DefaultValue( typeof( string ), "" )]
        [Category( "外观" )]
        public string BottleTag
        {
            get
            {
                return bottleTag;
            }
            set
            {
                bottleTag = value;
                Invalidate( );
            }
        }


        /// <summary>
        /// 获取或设置瓶子的备注信息，用于绘制在瓶子顶部的信息。
        /// </summary>
        [Browsable( true )]
        [DefaultValue( typeof( string ), "原料1" )]
        [Category( "外观" )]
        public string HeadTag
        {
            get
            {
                return headTag;
            }
            set
            {
                headTag = value;
                Invalidate( );
            }
        }


        private double value = 50;
        private bool isOpen = false;
        private string bottleTag = "";
        private StringFormat stringFormat = new StringFormat( );
        private string headTag = "原料1";


        protected override void WndProc( ref Message m )
        {
            if (m.Msg == 0x14) return;
            base.WndProc( ref m );
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (Width < 15 || Height < 15) return;

            int middle = Width / 2;
            int value_Y = Height - Width - (Height - Width - 20) * Convert.ToInt32( value ) / 100;

            // 计算一些相关的参数信息
            GraphicsPath graphicsPath = new GraphicsPath( );
            graphicsPath.AddPolygon( new Point[] {
                new Point(0,20),
                new Point(0,Height - Width),
                new Point(middle + 1, Height-8),
                new Point(middle + 1,20),
                new Point(0,20),
            } );
            Brush b = new LinearGradientBrush( new Point( 0, 20 ), new Point( middle + 1, 20 ), Color.FromArgb( 142, 196, 216 ), Color.FromArgb( 240, 240, 240 ) );

            g.FillPath( b, graphicsPath );
            graphicsPath.Reset( );
            graphicsPath.AddPolygon( new Point[] {
                new Point(middle,20),
                new Point(middle,Height - 8),
                new Point(Width-1, Height-Width),
                new Point(Width-1,20),
                new Point(middle,20),
            } );
            b.Dispose( );
            b = new LinearGradientBrush( new Point( middle - 1, 20 ), new Point( Width - 1, 20 ), Color.FromArgb( 240, 240, 240 ), Color.FromArgb( 142, 196, 216 ) );
            g.FillPath( b, graphicsPath );
            b.Dispose( );

            b = new SolidBrush( Color.FromArgb( 151, 232, 244 ) );
            g.FillEllipse( b, 1, 17, Width - 3, 6 );
            b.Dispose( );

            graphicsPath.Reset( );
            graphicsPath.AddPolygon( new Point[] {
                new Point(0, value_Y),
                new Point(0,Height - Width),
                new Point(middle+1, Height-8),
                new Point(middle+1, value_Y),
                new Point(0,value_Y),
            } );
            b = new LinearGradientBrush( new Point( 0, 20 ), new Point( middle + 1, 20 ), Color.FromArgb( 194, 190, 77 ), Color.FromArgb( 226, 221, 98 ) );
            g.FillPath( b, graphicsPath );
            b.Dispose( );


            graphicsPath.Reset( );
            graphicsPath.AddPolygon( new Point[] {
                new Point(middle,value_Y),
                new Point(middle,Height - 8),
                new Point(Width-1, Height-Width),
                new Point(Width-1,value_Y),
                new Point(middle,value_Y),
            } );
            b = new LinearGradientBrush( new Point( middle - 1, 20 ), new Point( Width - 1, 20 ), Color.FromArgb( 226, 221, 98 ), Color.FromArgb( 194, 190, 77 ) );
            g.FillPath( b, graphicsPath );
            b.Dispose( );
            graphicsPath.Dispose( );

            b = new SolidBrush( Color.FromArgb( 243, 245, 139 ) );
            g.FillEllipse( b, 1, value_Y - 3, Width - 3, 6 );
            b.Dispose( );

            g.FillEllipse( Brushes.White, 4, Height - Width, Width - 9, Width - 9 );
            Pen pen = new Pen( Color.Gray, 3 );
            if (isOpen) pen.Color = Color.LimeGreen;
            g.DrawEllipse( pen, 4, Height - Width, Width - 9, Width - 9 );
            g.FillEllipse( isOpen ? Brushes.LimeGreen : Brushes.Gray, 8, Height - Width + 4, Width - 17, Width - 17 );
            pen.Dispose( );

            if (!string.IsNullOrEmpty( bottleTag ))
            {
                g.DrawString( bottleTag, Font, Brushes.Gray, new Rectangle( -10, 26, Width + 20, 20 ), stringFormat );
            }

            if (!string.IsNullOrEmpty( headTag ))
            {
                g.DrawString( headTag, Font, Brushes.DimGray, new Rectangle( -10, 0, Width + 20, 20 ), stringFormat );
            }
        }

        private void UserBottle_Paint( object sender, PaintEventArgs e )
        {
            
        }
    }
}
