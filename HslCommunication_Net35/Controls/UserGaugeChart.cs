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
            brush_gauge_pointer = new SolidBrush(color_gauge_pointer);
            centerFormat = new StringFormat();
            centerFormat.Alignment = StringAlignment.Center;
            centerFormat.LineAlignment = StringAlignment.Center;

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

            g.DrawArc(pen_gauge_border, circular_mini, -angle, angle * 2 - 180);
            g.DrawArc(pen_gauge_border, circular, angle - 180, 180 - angle * 2);
            g.DrawLine(pen_gauge_border, (int)(-(radius / 3) * Math.Cos(angle / 180 * Math.PI)), -(int)((radius / 3) * Math.Sin(angle / 180 * Math.PI)), -(int)((radius - 30) * Math.Cos(angle / 180 * Math.PI)), -(int)((radius - 30) * Math.Sin(angle / 180 * Math.PI)));
            g.DrawLine(pen_gauge_border, (int)((radius - 30) * Math.Cos(angle / 180 * Math.PI)), -(int)((radius - 30) * Math.Sin(angle / 180 * Math.PI)), (int)((radius / 3) * Math.Cos(angle / 180 * Math.PI)), -(int)((radius / 3) * Math.Sin(angle / 180 * Math.PI)));

            // 开始绘制刻度
            g.RotateTransform(angle - 90);
            int totle = GetGraduationNumber(angle, radius);
            for (int i = 0; i <= totle; i++)
            {
                Rectangle rect = new Rectangle(-2, -radius, 3, 7);
                g.FillRectangle(Brushes.DimGray, rect);
                rect = new Rectangle(-50, -radius + 7, 100, 20);

                double current = ValueStart + (ValueMax - ValueStart) * i / totle;
                g.DrawString(current.ToString(), Font, Brushes.DodgerBlue, rect, centerFormat);
                g.RotateTransform((180 - 2 * angle) / totle);
            }
            g.RotateTransform(-(180 - 2 * angle) / totle);
            g.RotateTransform(angle - 90);

            Rectangle text = new Rectangle(-40, -(radius * 2 / 3 - 3), 80, 25);
            // g.FillRectangle(Brushes.Wheat, text);
            g.DrawString(Value.ToString(), Font, Brushes.Gray, text, centerFormat);
            g.DrawRectangle(pen_gauge_border, text);

            g.RotateTransform(angle - 90);
            g.RotateTransform((float)((Value - ValueStart) / (ValueMax - ValueStart) * (180 - 2 * angle)));
            g.DrawLine(Pens.DodgerBlue, 0, 0, 0, -radius + 20);

            g.ResetTransform();

        }

        /// <summary>
        /// 获取刻度的层级，根据圆弧的长度以及差值
        /// </summary>
        /// <returns></returns>
        private int GetGraduationNumber(float angle, int radius)
        {
            float length = Convert.ToSingle(2 * Math.PI * radius * angle / 360);
            int max = (int)(length / 40);         // 最多的情况

            return 10;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        private Brush brush_gauge_pointer = null;                             // 绘制仪表盘的指针的颜色

        private double value_start = 0;                                       // 仪表盘的初始值
        private double value_max = 100d;                                      // 仪表盘的结束值
        private double value_current = 0d;                                    // 仪表盘的当前值



        private StringFormat centerFormat = null;                                   // 居中显示的格式化文本
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


        /// <summary>
        /// 获取或设置指针的颜色
        /// </summary>
        [Browsable(true)]
        [Category("外观")]
        [Description("获取或设置仪表盘指针的颜色")]
        [DefaultValue(typeof(Color), "Tomato")]
        public Color PointerColor
        {
            get
            {
                return color_gauge_pointer;
            }
            set
            {
                brush_gauge_pointer?.Dispose();
                brush_gauge_pointer = new SolidBrush(value);
                color_gauge_pointer = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置数值的起始值，默认为0
        /// </summary>
        [Browsable(true)]
        [Category("外观")]
        [Description("获取或设置数值的起始值，默认为0")]
        [DefaultValue(0d)]
        public double ValueStart
        {
            get
            {
                return value_start;
            }
            set
            {
                value_start = value;
                if (value_max <= value_start)
                {
                    value_max = value_start + 1;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置数值的最大值，默认为100
        /// </summary>
        [Browsable(true)]
        [Category("外观")]
        [Description("获取或设置数值的最大值，默认为100")]
        [DefaultValue(100d)]
        public double ValueMax
        {
            get
            {
                return value_max;
            }
            set
            {
                value_max = value;
                if (value_max <= value_start)
                {
                    value_max = value_start + 1;
                }
                Invalidate();
            }
        }


        /// <summary>
        /// 获取或设置数值的当前值，应该处于最小值和最大值之间
        /// </summary>
        [Browsable(true)]
        [Category("外观")]
        [Description("获取或设置数值的当前值，默认为0")]
        [DefaultValue(0d)]
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
                    value_current = value;
                    Invalidate();
                }
            }
        }


        #endregion


    }
}
