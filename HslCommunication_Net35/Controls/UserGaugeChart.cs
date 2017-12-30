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
        public UserGaugeChart()
        {
            InitializeComponent();


            pen_gauge_border = new Pen(color_gauge_border);

            DoubleBuffered = true;

        }

        private void UserGaugeChart_Load(object sender, EventArgs e)
        {

        }

        private void UserGaugeChart_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;                // 消除锯齿
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;        // 优化文本显示

            OperateResult<Point, int, double> setting = GetCenterPoint();
            if (!setting.IsSuccess) return;                                                      // 不满足绘制条件

            Point center = setting.Content1;
            int radius = setting.Content2;
            float angle = Convert.ToSingle(setting.Content3);
            Rectangle circular = new Rectangle(-radius, -radius, 2 * radius, 2 * radius);
            Rectangle circular_mini = new Rectangle(-radius / 3, -radius / 3, 2 * radius / 3, 2 * radius / 3);


            g.TranslateTransform(center.X, center.Y);

            GraphicsPath path = new GraphicsPath(FillMode.Alternate);
            path.AddArc(circular_mini, -angle, angle * 2 - 180);
            path.AddLine((int)(-(radius / 3) * Math.Cos(angle / 180 * Math.PI)), -(int)((radius / 3) * Math.Sin(angle / 180 * Math.PI)), -(int)(radius * Math.Cos(angle / 180 * Math.PI)), -(int)(radius * Math.Sin(angle / 180 * Math.PI)));
            path.AddArc(circular, angle - 180, 180 - angle * 2);
            path.AddLine((int)(radius * Math.Cos(angle / 180 * Math.PI)), -(int)(radius * Math.Sin(angle / 180 * Math.PI)), (int)((radius / 3) * Math.Cos(angle / 180 * Math.PI)), -(int)((radius / 3) * Math.Sin(angle / 180 * Math.PI)));


            g.DrawPath(pen_gauge_border, path);

            // g.FillPath(brush_Gauge_background, path);

            //g.DrawArc(Pens.Tomato, circular_mini, -angle, angle * 2 - 180);
            //g.DrawArc(Pens.Tomato, circular, -angle, angle * 2 - 180);
            //g.DrawString(angle.ToString(), Font, Brushes.Chocolate, new Point(0, -20));

            g.ResetTransform();

        }

        private OperateResult<Point, int, double> GetCenterPoint()
        {
            OperateResult<Point, int, double> result = new OperateResult<Point, int, double>();
            if (Height <= 35) return result;
            if (Width <= 20) return result;

            result.IsSuccess = true;
            result.Content2 = Height - 30;
            if ((Width - 40) / 2d > result.Content2)
            {
                result.Content3 = Math.Acos(1) * 180 / Math.PI;
            }
            else
            {
                result.Content3 = Math.Acos((Width - 40) / 2d / (Height - 30)) * 180 / Math.PI;
            }
            result.Content1 = new Point(Width / 2, Height - 10);
            return result;
        }



        #region Private Member

        private Color color_gauge_border = Color.DimGray;
        private Pen pen_gauge_border = null;                                  // 绘制仪表盘的边框色


        private Color color_gauge_pointer = Color.Tomato;


        #endregion


        #region Public Member

        /// <summary>
        /// 获取或设置仪表盘的背景色
        /// </summary>
        [Browsable(true)]
        [Category("外观")]
        [Description("获取或设置仪表盘的背景色")]
        [DefaultValue(typeof(Color), "DimGray")]
        public Color GaugeBorder
        {
            get { return color_gauge_border; }
            set
            {
                pen_gauge_border?.Dispose();
                pen_gauge_border = new Pen(value);
                color_gauge_border = value;
                Invalidate();
            }
        }


        public Color PointerColor
        {

        }


        #endregion


    }
}
