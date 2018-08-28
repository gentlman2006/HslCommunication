using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HslCommunication.Controls
{
    /// <summary>
    /// 一个罐子形状的控件
    /// </summary>
    public partial class UserDrum : UserControl
    {
        #region Constructor

        /// <summary>
        /// 实例化一个罐子形状的控件
        /// </summary>
        public UserDrum( )
        {
            DoubleBuffered = true;
            InitializeComponent( );
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 获取或设置容器罐的背景色。
        /// </summary>
        [Browsable(true)]
        [DefaultValue(typeof(Color), "Silver" )]
        [Category("外观")]
        public Color DrumBackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
                backBrush.Dispose( );
                backBrush = new SolidBrush( value );
                Invalidate( );
            }
        }

        /// <summary>
        /// 获取或设置容器罐的边框色。
        /// </summary>
        [Browsable( true )]
        [DefaultValue( typeof( Color ), "DimGray" )]
        [Category( "外观" )]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
                borderPen.Dispose( );
                borderPen = new Pen( value );
                Invalidate( );
            }
        }


        #endregion

        #region Private Member

        private Color backColor = Color.Silver;
        private Brush backBrush = new SolidBrush( Color.Silver );

        private Color borderColor = Color.DimGray;
        private Pen borderPen = new Pen( Color.DimGray );

        #endregion





        private void UserDrum_Paint( object sender, PaintEventArgs e )
        {
            if (Width < 40 || Height < 50) return;


            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;


            Point[] points = new Point[]
            {
                new Point(Width / 2, 20),
                new Point(Width -10, Height*3/10),
                new Point(Width -10, Height*7/10),
                new Point(Width / 2, Height - 20),
                new Point(10, Height*7/10),
                new Point(10, Height*3/10),
                new Point(Width / 2, 20)
            };

            g.FillPolygon( backBrush, points );
            g.DrawLines( borderPen, points );
            g.DrawCurve( borderPen, new Point[] { new Point( 10, Height * 3 / 10 ), new Point( Width / 2, Height * 3 / 10 + Height / 25 ), new Point( Width - 10, Height * 3 / 10 ) } );
            g.DrawCurve( borderPen, new Point[] { new Point( 10, Height * 7 / 10 ), new Point( Width / 2, Height * 7 / 10 + Height / 25 ), new Point( Width - 10, Height * 7 / 10 ) } );
        }
    }
}
