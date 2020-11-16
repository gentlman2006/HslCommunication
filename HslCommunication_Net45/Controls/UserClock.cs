using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;


/**************************************************************
 * 
 *    一个显示时间的控件，用来显示当前的时间和额外的定制界面
 * 
 *
 * 
 *************************************************************/







namespace HslCommunication.Controls
{
    /// <summary>
    /// 一个时钟控件
    /// </summary>
    public partial class UserClock : UserControl
    {
        /// <summary>
        /// 实例化一个时钟控件
        /// </summary>
        public UserClock()
        {
            InitializeComponent();
            _Time1s.Interval=50;
            _Time1s.Tick+=_Time1s_Tick;
            DoubleBuffered = true;
        }

        private void ClockMy_Load(object sender, EventArgs e)
        {
            BackgroundImage = _BackGround();
            _Time1s.Start();
        }
        //1秒的时钟
        private Timer _Time1s = new Timer();
        private void  _Time1s_Tick(object sender,EventArgs e)
        {
            _NowTime = DateTime.Now;
            this.Invalidate();
        }
        private Bitmap _BackGround()
        {
            int _Width = this.Width;
            Bitmap temp = new Bitmap(_Width - 20, _Width - 20);
            Graphics g = Graphics.FromImage(temp);
            g.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //g.CompositingQuality = CompositingQuality.HighQuality;

            Point _CenterPoint = new Point(temp.Width / 2, temp.Height / 2);
            int _R = (temp.Width - 1) / 2;
            Rectangle _Rec1 = new Rectangle(0, 0, temp.Width - 1, temp.Width - 1);
            Rectangle _Rec2 = new Rectangle(2, 2, temp.Width - 5, temp.Width - 5);
            Rectangle _Rec3 = new Rectangle(_CenterPoint.X - 4, _CenterPoint.Y - 4, 8, 8);
            Rectangle _Rec4 = new Rectangle(5, 5, temp.Width - 11, temp.Width - 11);
            Rectangle _Rec5 = new Rectangle(8, 8, temp.Width - 17, temp.Width - 17);

            g.FillEllipse(Brushes.DarkGray, _Rec1);


            g.FillEllipse(Brushes.White, _Rec2);
            g.DrawEllipse(new Pen(Brushes.Black, (float)1.5), _Rec3);

            g.TranslateTransform(_CenterPoint.X, _CenterPoint.Y);
            for (int i = 0; i < 60; i++)
            {
                g.RotateTransform(6);
                g.DrawLine(Pens.DarkGray, new Point(_R - 3, 0), new Point(_R - 1, 0));
            }
            for (int i = 0; i < 12; i++)
            {
                g.RotateTransform(30);
                g.DrawLine(new Pen(Color.Chocolate, 2), new Point(_R - 6, 0), new Point(_R - 1, 0));
            }

            g.ResetTransform();

            System.Drawing.Font FontTemp = new Font("Microsoft YaHei UI", 12);


            int IntOffset_X = _R / 2;
            int IntOffset_Y = (int)(_R * Math.Sqrt(3) / 2);

            g.DrawString("1", FontTemp, Brushes.Green, new PointF(_R + IntOffset_X - 13, _R - IntOffset_Y + 4));
            g.DrawString("2", FontTemp, Brushes.Green, new PointF(_R + IntOffset_Y - 17, _R - IntOffset_X - 2));
            g.DrawString("3", FontTemp, Brushes.Green, new PointF(2 * _R - 18, _R - 8));
            g.DrawString("4", FontTemp, Brushes.Green, new PointF(_R + IntOffset_Y - 18, _R + IntOffset_X - 14));
            g.DrawString("5", FontTemp, Brushes.Green, new PointF(_R + IntOffset_X - 14, _R + IntOffset_Y - 19));
            g.DrawString("6", FontTemp, Brushes.Green, new PointF(_R - 6, 2 * _R - 22));
            g.DrawString("7", FontTemp, Brushes.Green, new PointF(_R - IntOffset_X + 2, _R + IntOffset_Y - 19));
            g.DrawString("8", FontTemp, Brushes.Green, new PointF(_R - IntOffset_Y + 5, _R + IntOffset_X - 14));
            g.DrawString("9", FontTemp, Brushes.Green, new PointF(8, _R - 9));
            g.DrawString("10", FontTemp, Brushes.Green, new PointF(_R - IntOffset_Y + 4, _R - IntOffset_X - 2));
            g.DrawString("11", FontTemp, Brushes.Green, new PointF(_R - IntOffset_X, _R - IntOffset_Y + 3));
            g.DrawString("12", FontTemp, Brushes.Green, new PointF(_R - 10, 7));
            // g.DrawString("Sweet", new Font("Courier New", 9), Brushes.Green, new PointF(_R - 18, _R/2));

            Bitmap _temp = new Bitmap(this.Width, this.Height);
            Graphics g1 = Graphics.FromImage(_temp);
            g1.DrawImage(temp, new Point(10, 10));
            temp.Dispose();
            return _temp;
        }

        //#############################################################################################################
        //属性设计器
        //#############################################################################################################
        //
        //==================================================================================================
        private DateTime _NowTime = DateTime.Now;
        /// <summary>
        /// 获取时钟的当前时间
        /// </summary>
        [Category("我的属性"), Description("设置边框的宽度")]
        public DateTime 当前时间
        {
            get { return _NowTime; }
        }
        //==================================================================================================
        //小时的指针颜色
        private Color _HourColor = Color.Chocolate;
        /// <summary>
        /// 获取或设置时钟指针的颜色
        /// </summary>
        [Category("我的属性"), Description("设置时钟的指针颜色")]
        [DefaultValue(typeof(Color), "Chocolate")]
        public Color 时钟指针颜色
        {
            set { _HourColor = value; }
            get { return _HourColor; }
        }
        //==================================================================================================
        //分钟的指针颜色
        private Color _MiniteColor = Color.Coral;
        /// <summary>
        /// 获取或设置时钟分钟指针颜色
        /// </summary>
        [Category("我的属性"), Description("设置分钟的指针颜色")]
        [DefaultValue(typeof(Color), "Coral")]
        public Color 分钟指针颜色
        {
            set { _MiniteColor = value; }
            get { return _MiniteColor; }
        }
        //==================================================================================================
        //秒钟的指针颜色
        private Color _SecondColor = Color.Green;
        /// <summary>
        /// 获取或设置秒钟指针颜色
        /// </summary>
        [Category("我的属性"), Description("设置秒钟的指针颜色")]
        [DefaultValue(typeof(Color), "Green")]
        public Color 秒钟指针颜色
        {
            set { _SecondColor = value; }
            get { return _SecondColor; }
        }
        //==================================================================================================
        //显示的文本
        private string _ShowText = "Sweet";
        /// <summary>
        /// 获取或设置时钟的个性化文本
        /// </summary>
        [Category("我的属性"), Description("设置时钟显示的字符串")]
        [DefaultValue(typeof(string), "Sweet")]
        public string 显示文本
        {
            set { _ShowText = value; }
            get { return _ShowText; }
        }
        //==================================================================================================
        //显示的文本字体
        private string _ShowTextFont = "Courier New";
        /// <summary>
        /// 字体
        /// </summary>
        [Category("我的属性"), Description("设置时钟显示的字符串")]
        [DefaultValue(typeof(string), "Courier New")]
        public string 显示文本字体
        {
            set { _ShowTextFont = value; }
            get { return _ShowTextFont; }
        }
        /// <summary>
        /// 重绘控件显示
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int _R = (this.Width - 21) / 2;

            int _Hour = _NowTime.Hour;
            int _Minite = _NowTime.Minute;
            float _Seconds = _NowTime.Second + (float)_NowTime.Millisecond / 1000;

            int _ArcHour = _Hour * (30) + 270 + _Minite / 2;
            int _ArcMinite = _Minite * 6 + 270;
            float _ArcSeconds = _Seconds * 6 + 270;

            Graphics g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            System.Drawing.Font _font1 = new System.Drawing.Font(_ShowTextFont, 14);
            System.Drawing.Size _SizeTemp = g.MeasureString(_ShowText, _font1).ToSize();
            g.DrawString(_ShowText, _font1, Brushes.Green, new PointF(_R - _SizeTemp.Width / 2 + 10, _R / 2 + 12));
            g.SmoothingMode = SmoothingMode.AntiAlias;

            SizeF _Size = g.MeasureString(_NowTime.DayOfWeek.ToString(), new Font(_ShowTextFont, 10));
            g.DrawString(_NowTime.DayOfWeek.ToString(),
                new Font(_ShowTextFont, 10), Brushes.Chocolate, new PointF(_R - _Size.ToSize().Width / 2 + 10, _R * 3 / 2 - 2));

            g.TranslateTransform(this.Width / 2, this.Width / 2);
            g.RotateTransform(_ArcHour, MatrixOrder.Prepend);
            g.DrawLine(new Pen(_HourColor, 2), new Point(4, 0), new Point(9, 0));
            g.DrawClosedCurve(new Pen(_HourColor, 1), new Point[]{new Point(12,2),new Point(10,0),new Point(12,-2),new Point(_R/2,-2),
            new Point(_R/2+6,0),new Point(_R/2,2)});
            g.RotateTransform(-_ArcHour);
            g.RotateTransform(_ArcMinite, MatrixOrder.Prepend);
            g.DrawLine(new Pen(_MiniteColor, 2), new Point(4, 0), new Point(9, 0));
            g.DrawClosedCurve(new Pen(_MiniteColor, 1), new Point[]{new Point(14,2),new Point(10,0),new Point(14,-2),new Point(_R-17,-2),
            new Point(_R-10,0),new Point(_R-17,2)});
            g.RotateTransform(-_ArcMinite);
            g.RotateTransform(_ArcSeconds, MatrixOrder.Prepend);
            g.DrawLine(new Pen(_SecondColor, 1), new Point(-13, 0), new Point(_R - 6, 0));

            g.ResetTransform();

            string _StrDate = _NowTime.Year + "-" + _NowTime.Month + "-" + _NowTime.Day;
            System.Drawing.Size _Size2 = g.MeasureString(_StrDate, new Font(_ShowTextFont, 12)).ToSize();
            g.DrawString(_StrDate, new Font(_ShowTextFont, 12), Brushes.Green, new PointF(_R - _Size2.Width / 2 + 10, _R * 2 + 15));
        }

        private void ClockMy_SizeChanged(object sender, EventArgs e)
        {
            this.BackgroundImage = _BackGround();
        }

    }
}
