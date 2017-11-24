using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Drawing2D;

namespace BasicFramework
{
    /// <summary>
    /// 自定义的进度条控件
    /// </summary>
    public partial class HuProcessBar : UserControl
    {
        /// <summary>
        /// 实例化一个进度条控件
        /// </summary>
        public HuProcessBar()
        {
            InitializeComponent();
        }


        #region 属性设置区

        private bool m_HasBorder = false;
        /// <summary>
        /// 边框是否有无
        /// </summary>
        [Description("边框的有无")]
        [DefaultValue(false)]
        public bool HasBorder
        {
            get { return m_HasBorder; }
            set { m_HasBorder = value; Invalidate(); }
        }
        //==============================================================
        private Color m_HuBorderColor = Color.Gray;
        [Description("边框的颜色")]
        [DefaultValue(typeof(Color),"Gray")]
        public Color HuBorderColor
        {
            get { return m_HuBorderColor; }
            set { m_HuBorderColor = value; Invalidate(); }
        }
        //==============================================================
        private int m_Subsections = 1;
        [Description("分割的段数，默认不分割")]
        [DefaultValue(1)]
        public int Subsections
        {
            get { return m_Subsections; }
            set { m_Subsections = value; Invalidate(); }
        }
        //==============================================================
        private Color m_FillColorStart = Color.Tomato;
        [Description("填充开始颜色")]
        [DefaultValue(typeof(Color), "Tomato")]
        public Color FillColorStart
        {
            get { return m_FillColorStart; }
            set { m_FillColorStart = value; Invalidate(); }
        }
        //==============================================================
        private Color m_FillColorEnd = Color.LimeGreen;
        [Description("填充结束颜色")]
        [DefaultValue(typeof(Color), "LimeGreen")]
        public Color FillColorEnd
        {
            get { return m_FillColorEnd; }
            set { m_FillColorEnd = value; Invalidate(); }
        }
        //=============================================================
        private double m_HuSetValue = 0;
        [Description("设定值")]
        [DefaultValue(0)]
        public double HuSetValue
        {
            get { return m_HuSetValue; }
            set
            {
                if (Math.Abs(value - m_HuSetValue) < 0.002) return;
                if (value < 0 || value > 1) return;
                m_HuSetValue = value;
                ActiualPosition = (int)(value * ActiualMaxPosition);
                Invalidate();
            }
        }
        //=============================================================
        private int m_ColorChangeStyle = 1;
        /// <summary>
        /// 进度条变化的颜色
        /// </summary>
        [Description("指定进度条颜色的变化值")]
        [DefaultValue(1)]
        public int ColorChangeStyle
        {
            get { return m_ColorChangeStyle; }
            set
            {
                if (value != 1 && value != 2) return;
                m_ColorChangeStyle = value;
                Invalidate();
            }
        }

        #endregion

        /// <summary>
        /// 实际画图的点位
        /// </summary>
        private int ActiualPosition { get; set; } = 0;
        private int ActiualMaxPosition { get; set; } = 0;

        private Thread ThreadCalculation;
        private bool IsQuit { get; set; } = false;
        private void HuProcessBar_Load(object sender, EventArgs e)
        {
            ActiualMaxPosition = Width;
            Disposed += HuProcessBar_Disposed;
            SizeChanged += HuProcessBar_SizeChanged;
            // 开启双缓冲
            //SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            //SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        private void HuProcessBar_SizeChanged(object sender, EventArgs e)
        {
            ActiualMaxPosition = Width;
            Invalidate();
        }

        private void HuProcessBar_Disposed(object sender, EventArgs e)
        {
            IsQuit = true;
        }

        private delegate void MethodVoidDelegate();
        

        private void HuProcessBar_Paint(object sender, PaintEventArgs e)
        {
            Brush MyBrush = new SolidBrush(BackColor);
            Rectangle Rec = new Rectangle(0, 0, Width - 1, Height - 1);
            e.Graphics.FillRectangle(MyBrush, Rec);

            if (ActiualPosition > 0 && ActiualMaxPosition > 0)
            {
                Rectangle Rec1 = new Rectangle(0, 0, ActiualPosition, Height - 1);
                if (ColorChangeStyle == 1)
                {
                    byte Color_R = FillColorStart.R;
                    byte Color_G = FillColorStart.G;
                    byte Color_B = FillColorStart.B;
                    Color_R += (byte)((FillColorEnd.R - FillColorStart.R) * ActiualPosition / ActiualMaxPosition);
                    Color_G += (byte)((FillColorEnd.G - FillColorStart.G) * ActiualPosition / ActiualMaxPosition);
                    Color_B += (byte)((FillColorEnd.B - FillColorStart.B) * ActiualPosition / ActiualMaxPosition);
                    Color MyColor = Color.FromArgb(Color_R, Color_G, Color_B);
                    Brush HuBrush = new SolidBrush(MyColor);
                    e.Graphics.FillRectangle(HuBrush, Rec1);
                    HuBrush.Dispose();
                }
                else
                {
                    LinearGradientBrush linear = new LinearGradientBrush(Rec, FillColorStart, FillColorEnd, 0f);
                    e.Graphics.FillRectangle(linear, Rec1);
                    linear.Dispose();
                }
            }
            if (HasBorder)
            {
                using (Pen pen1 = new Pen(HuBorderColor))
                {
                    e.Graphics.DrawRectangle(pen1, Rec);
                }
            }
            if (Subsections > 1)
            {
                for (int i = 1; i < Subsections; i++)
                {
                    int x = i * ActiualMaxPosition / Subsections;
                    e.Graphics.DrawLine(Pens.White, x, 0, x, Height - 1);
                    //e.Graphics.DrawLine(Pens.White, x + 1, 0, x + 1, Height - 1);
                }
            }
            MyBrush.Dispose();
        }
    }
}
