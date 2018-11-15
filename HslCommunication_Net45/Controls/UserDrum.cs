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
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
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

        /// <summary>
        /// 获取或设置显示的文本信息
        /// </summary>
        [Browsable( true )]
        [DefaultValue( "" )]
        [Category( "外观" )]
        public override string Text
        {
            get { return text; }
            set
            {
                this.text = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// 获取或设置文本的颜色
        /// </summary>
        [Browsable(true)]
        [Category( "外观" )]
        [DefaultValue( typeof( Color ), "White" )]
        public override Color ForeColor
        {
            get => textColor;
            set
            {
                textColor = value;
                textBrush.Dispose( );
                textBrush = new SolidBrush( value );
                Invalidate( );
            }
        }


        /// <summary>
        /// 获取或设置文本的背景色
        /// </summary>
        [Browsable( true )]
        [Category( "外观" )]
        [DefaultValue( typeof( Color ), "DarkGreen" )]
        public Color TextBackColor
        {
            get => textBackColor;
            set
            {
                textBackColor = value;
                textBackBrush.Dispose( );
                textBackBrush = new SolidBrush( value );
                Invalidate( );
            }
        }



        #endregion

        #region Private Member

        private Color backColor = Color.Silver;
        private Brush backBrush = new SolidBrush( Color.Silver );

        private Color borderColor = Color.DimGray;
        private Pen borderPen = new Pen( Color.DimGray );

        private Color textColor = Color.White;
        private Brush textBrush = new SolidBrush( Color.White );

        private Color textBackColor = Color.DarkGreen;
        private Brush textBackBrush = new SolidBrush( Color.DarkGreen );

        private string text = string.Empty;
        private StringFormat stringFormat = new StringFormat( );

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

            if (!string.IsNullOrEmpty( text ))
            {
                SizeF sizeF = g.MeasureString( text, Font, (Width - 20) * 3 / 5 );
                if (sizeF.Width < (Width - 20) * 4 / 5) sizeF.Width = (Width - 20) * 3 / 5;
                sizeF.Width += 10;
                sizeF.Height += 5;

                Rectangle textRectangle = new Rectangle( Width / 2 - (int)(sizeF.Width / 2), Height / 2 - (int)(sizeF.Height / 2), (int)sizeF.Width, (int)sizeF.Height );
                g.FillRectangle( Brushes.DarkGreen, textRectangle );
                g.DrawString( text, Font, textBrush, textRectangle, stringFormat );
            }
        }
    }
}
