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
    /// 一个开关按钮类
    /// </summary>
    [DefaultEvent("Click")]
    public partial class UserSwitch : UserControl
    {
        /// <summary>
        /// 实例化一个开关按钮对象
        /// </summary>
        public UserSwitch()
        {
            InitializeComponent();
            DoubleBuffered = true;
            brush_switch_background = new SolidBrush(color_switch_background);
            pen_switch_background = new Pen(color_switch_background, 2f);
            brush_switch_foreground = new SolidBrush(color_switch_foreground);

            centerFormat = new StringFormat();
            centerFormat.Alignment = StringAlignment.Center;
            centerFormat.LineAlignment = StringAlignment.Center;
        }

        private void UserSwitch_Load(object sender, EventArgs e)
        {

        }

        private void UserSwitch_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Point center = GetCenterPoint();
            e.Graphics.TranslateTransform(center.X, center.Y);

            int radius = 45 * (center.X * 2 - 30) / 130;
            if (radius < 5) return;

            Rectangle rectangle_larger = new Rectangle(-radius - 4, -radius - 4, 2 * radius + 8, 2 * radius + 8);
            Rectangle rectangle = new Rectangle(-radius, -radius, 2 * radius, 2 * radius);

            e.Graphics.DrawEllipse(pen_switch_background, rectangle_larger);
            e.Graphics.FillEllipse(brush_switch_background, rectangle);

            float angle = -36;

            if (SwitchStatus) angle = 36;

            e.Graphics.RotateTransform(angle);
            int temp = 20 * (center.X * 2 - 30) / 130;
            Rectangle rect_switch = new Rectangle(-center.X / 8, -radius - temp, center.X / 4, radius * 2 + temp * 2);
            e.Graphics.FillRectangle(brush_switch_foreground, rect_switch);

            Rectangle rect_mini = new Rectangle(-center.X / 16, -radius - 10, center.X / 8, center.X * 3 / 8);
            e.Graphics.FillEllipse(SwitchStatus ? Brushes.LimeGreen:Brushes.Tomato, rect_mini);

            Rectangle rect_text = new Rectangle(-50, -radius - temp - 15, 100, 15);
            e.Graphics.DrawString(SwitchStatus? description[1] : description[0], Font, SwitchStatus ? Brushes.LimeGreen : Brushes.Tomato, rect_text, centerFormat);

            e.Graphics.ResetTransform();
        }





        #region Private Member

        private Color color_switch_background = Color.DimGray;                    // 按钮的背景颜色，包括边线颜色
        private Brush brush_switch_background = null;                             // 按钮的背景画刷
        private Pen pen_switch_background = null;                                 // 按钮的背景画笔
        private bool switch_status = false;                                       // 按钮的开关状态
        private Color color_switch_foreground = Color.FromArgb(36, 36, 36);       // 按钮开关的前景色
        private Brush brush_switch_foreground = null;                             // 按钮开关的前景色画刷
        private StringFormat centerFormat = null;                                 // 居中显示的格式化文本
        private string[] description = new string[2] { "Off", "On" };             // 两种开关状态的文本描述

        #endregion

        #region Event Handle

        /// <summary>
        /// 开关按钮发生变化的事件
        /// </summary>
        [Category("操作")]
        [Description("点击了按钮开发后触发")]
        public event Action<object, bool> OnSwitchChanged;

        #endregion

        #region Private Method

        private Point GetCenterPoint()
        {
            if (Height > Width)
            {
                return new Point(Width / 2, Width / 2);
            }
            else
            {
                return new Point(Height / 2, Height / 2);
            }
        }

        #endregion

        #region Public Member


        /// <summary>
        /// 获取或设置开关按钮的背景色
        /// </summary>
        [Browsable(true)]
        [Description("获取或设置开关按钮的背景色")]
        [Category("外观")]
        [DefaultValue(typeof(Color), "DimGray")]
        public Color SwitchBackground
        {
            get
            {
                return color_switch_background;
            }
            set
            {
                color_switch_background = value;
                brush_switch_background?.Dispose();
                pen_switch_background?.Dispose();
                brush_switch_background = new SolidBrush(color_switch_background);
                pen_switch_background = new Pen(color_switch_background, 2f);
                Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置开关按钮的前景色
        /// </summary>
        [Browsable(true)]
        [Description("获取或设置开关按钮的前景色")]
        [Category("外观")]
        [DefaultValue(typeof(Color), "[36, 36, 36]")]
        public Color SwitchForeground
        {
            get
            {
                return color_switch_foreground;
            }
            set
            {
                color_switch_foreground = value;
                brush_switch_foreground = new SolidBrush(color_switch_foreground);
                Invalidate();
            }
        }


        /// <summary>
        /// 获取或设置开关按钮的开合状态
        /// </summary>
        [Browsable(true)]
        [Description("获取或设置开关按钮的开合状态")]
        [Category("外观")]
        [DefaultValue(false)]
        public bool SwitchStatus
        {
            get
            {
                return switch_status;
            }
            set
            {
                if(value != switch_status)
                {
                    switch_status = value;
                    Invalidate();
                    OnSwitchChanged?.Invoke(this, switch_status);
                }
            }
        }


        /// <summary>
        /// 获取或设置两种开关状态的文本描述，例如：new string[]{"Off","On"}
        /// </summary>
        [Browsable(false)]
        public string[] SwitchStatusDescription
        {
            get { return description; }
            set
            {
                if (value?.Length == 2)
                {
                    description = value;
                }
            }
        }

        #endregion

        private void UserSwitch_Click(object sender, EventArgs e)
        {
            SwitchStatus = !SwitchStatus;
        }
    }
}
