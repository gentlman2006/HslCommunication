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
    /// 信号灯的控件类
    /// </summary>
    public partial class UserLantern : UserControl
    {
        /// <summary>
        /// 实例化一个信号灯控件的对象
        /// </summary>
        public UserLantern( )
        {
            InitializeComponent( );

            DoubleBuffered = true;
            brush_lantern_background = new SolidBrush( color_lantern_background );
            pen_lantern_background = new Pen( color_lantern_background, 2f );
        }

        private void UserLantern_Load( object sender, EventArgs e )
        {

        }

        private void UserLantern_Paint( object sender, PaintEventArgs e )
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Point center = GetCenterPoint( );
            e.Graphics.TranslateTransform( center.X, center.Y );

            int radius = (center.X - 5);
            if (radius < 5) return;

            Rectangle rectangle_larger = new Rectangle( -radius - 4, -radius - 4, 2 * radius + 8, 2 * radius + 8 );
            Rectangle rectangle = new Rectangle( -radius, -radius, 2 * radius, 2 * radius );

            e.Graphics.DrawEllipse( pen_lantern_background, rectangle_larger );
            e.Graphics.FillEllipse( brush_lantern_background, rectangle );
        }

        #region Private Member

        private Color color_lantern_background = Color.LimeGreen;                  // 按钮的背景颜色，包括边线颜色
        private Brush brush_lantern_background = null;                             // 按钮的背景画刷
        private Pen pen_lantern_background = null;                                 // 按钮的背景画笔

        #endregion

        #region Public Member

        /// <summary>
        /// 获取或设置开关按钮的背景色
        /// </summary>
        [Browsable( true )]
        [Description( "获取或设置信号灯的背景色" )]
        [Category( "外观" )]
        [DefaultValue( typeof( Color ), "LimeGreen" )]
        public Color LanternBackground
        {
            get
            {
                return color_lantern_background;
            }
            set
            {
                color_lantern_background = value;
                brush_lantern_background?.Dispose( );
                pen_lantern_background?.Dispose( );
                brush_lantern_background = new SolidBrush( color_lantern_background );
                pen_lantern_background = new Pen( color_lantern_background, 2f );
                Invalidate( );
            }
        }

        #endregion

        #region Private Method

        private Point GetCenterPoint( )
        {
            if (Height > Width)
            {
                return new Point( (Width - 1) / 2, (Width - 1) / 2 );
            }
            else
            {
                return new Point( (Height - 1) / 2, (Height - 1) / 2 );
            }
        }

        #endregion


    }
}
