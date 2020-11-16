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
    /// 一个直立的进度条控件，满足不同的情况使用
    /// </summary>
    public partial class UserVerticalProgress : UserControl
    {
        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public UserVerticalProgress()
        {
            InitializeComponent();

            m_borderPen = new Pen(Color.DimGray);
            m_backBrush = new SolidBrush(Color.Transparent);
            m_foreBrush = new SolidBrush(Color.Tomato);
            m_progressColor = Color.Tomato;
            m_borderColor = Color.DimGray;

            m_formatCenter = new StringFormat();
            m_formatCenter.Alignment = StringAlignment.Center;
            m_formatCenter.LineAlignment = StringAlignment.Center;

            m_UpdateAction = new Action(UpdateRender);
            hybirdLock = new Core.SimpleHybirdLock();

            DoubleBuffered = true;
        }



        private void UserVerticalProgress_Load(object sender, EventArgs e)
        {

        }


        private void UserVerticalProgress_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                // 根据实际值来绘制图形
                Graphics g = e.Graphics;
                Rectangle rectangle = new Rectangle(0, 0, Width - 1, Height - 1);
                g.FillRectangle(m_backBrush, rectangle);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;


                switch(m_progressStyle)
                {
                    case ProgressStyle.Vertical:
                        {
                            // 垂直方向的进度条
                            int height = (int)(m_actual * 1L * (Height - 2) / m_Max);
                            rectangle = new Rectangle(0, Height - 1 - height, Width - 1, height);
                            g.FillRectangle(m_foreBrush, rectangle);

                            //if (m_actual != m_value)
                            //{
                            //    rectangle = new Rectangle(0, rectangle.Y - 5, Width - 1, 5);
                            //    using (Brush b = new LinearGradientBrush(rectangle, m_progressColor, BackColor, 270f))
                            //    {
                            //        g.FillRectangle(b, rectangle);
                            //    }
                            //}
                            break;
                        }
                    case ProgressStyle.Horizontal:
                        {
                            // 水平方向的进度条
                            int width = (int)(m_actual * 1L * (Width - 2) / m_Max);
                            rectangle = new Rectangle(0, 0, width + 1, Height - 1);
                            g.FillRectangle(m_foreBrush, rectangle);

                            //if (m_actual == m_value)
                            //{
                            //    rectangle = new Rectangle(width + 1, 0, 5, Height - 1);
                            //    using (Brush b = new LinearGradientBrush(rectangle, m_progressColor, BackColor, 0f))
                            //    {
                            //        g.FillRectangle(b, rectangle);
                            //    }
                            //}
                            break;
                        }
                }




                rectangle = new Rectangle(0, 0, Width - 1, Height - 1);
                if (m_isTextRender)
                {
                    string str = (m_actual * 100L / m_Max) + "%";
                    using (Brush brush = new SolidBrush(ForeColor))
                    {
                        g.DrawString(str, Font, brush, rectangle, m_formatCenter);
                    }
                }

                g.DrawRectangle(m_borderPen, rectangle);
            }
            catch (Exception)
            {
                // BasicFramework.SoftBasic.ShowExceptionMessage(ex);
            }
        }

        private void UserVerticalProgress_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        #region Private Members

        private int m_version = 0;                                          // 设置数据时的版本，用于更新时的版本验证
        private int m_Max = 100;                                            // 默认的最大值
        private int m_value = 0;                                            // 当前的设定值
        private int m_actual = 0;                                           // 当前的实际值
        private Pen m_borderPen;                                            // 边框的画笔
        private Color m_borderColor;                                        // 边框的背景色
        private Brush m_backBrush;                                          // 背景色
        private Brush m_foreBrush;                                          // 前景色
        private Color m_progressColor;                                      // 前景色颜色
        private StringFormat m_formatCenter;                                // 中间显示字符串的格式
        private bool m_isTextRender = true;                                 // 是否显示文本信息
        private Action m_UpdateAction;                                      // 更新界面的委托
        private Core.SimpleHybirdLock hybirdLock;                           // 数据的同步锁
        private int m_speed = 1;                                            // 进度条的升降快慢
        private ProgressStyle m_progressStyle = ProgressStyle.Vertical;     // 进度条的样式，指示水平的还是竖直的
        private bool m_UseAnimation = false;                                // 是否采用动画效果

        #endregion


        #region Public Properties

        /// <summary>
        /// 获取或设置光标在控件上显示的信息
        /// </summary>
        public override Cursor Cursor
        {
            get => base.Cursor;
            set => base.Cursor = value;
        }


        /// <summary>
        /// 获取或设置控件的背景颜色值
        /// </summary>
        [Description("获取或设置进度条的背景色")]
        [Category("外观")]
        [Browsable(true)]
        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                m_backBrush?.Dispose();
                m_backBrush = new SolidBrush(value);
                Invalidate();
            }
        }


        /// <summary>
        /// 获取或设置进度的颜色
        /// </summary>
        [Description("获取或设置进度条的前景色")]
        [Category("外观")]
        [Browsable(true)]
        [DefaultValue(typeof(Color),"Tomato")]
        public Color ProgressColor
        {
            get { return m_progressColor; }
            set
            {
                m_progressColor = value;
                m_foreBrush?.Dispose();
                m_foreBrush = new SolidBrush(value);
                Invalidate();
            }
        }


        /// <summary>
        /// 进度条的最大值，默认为100
        /// </summary>
        [Description("获取或设置进度条的最大值，默认为100")]
        [Category("外观")]
        [Browsable(true)]
        [DefaultValue(100)]
        public int Max
        {
            get { return m_Max; }
            set
            {
                if (value > 1)
                {
                    m_Max = value;
                }
                if (m_value > m_Max) m_value = m_Max;
                Invalidate();
            }
        }


        /// <summary>
        /// 当前进度条的值，不能大于最大值或小于0
        /// </summary>
        [Description("获取或设置当前进度条的值")]
        [Category("外观")]
        [Browsable(true)]
        [DefaultValue(0)]
        public int Value
        {
            get { return m_value; }
            set
            {
                if (value >= 0 && value <= m_Max)
                {
                    if (value != m_value)
                    {
                        m_value = value;
                        if (UseAnimation)
                        {
                            int version = System.Threading.Interlocked.Increment(ref m_version);
                            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(
                                ThreadPoolUpdateProgress), version);
                        }
                        else
                        {
                            m_actual = value;
                            Invalidate();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否显示进度
        /// </summary>
        [Description("获取或设置是否显示进度文本")]
        [Category("外观")]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool IsTextRender
        {
            get { return m_isTextRender; }
            set
            {
                m_isTextRender = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 设置进度条的边框颜色
        /// </summary>
        [Description("获取或设置进度条的边框颜色")]
        [Category("外观")]
        [Browsable(true)]
        [DefaultValue(typeof(Color),"DimGray")]
        public Color BorderColor
        {
            get { return m_borderColor; }
            set
            {
                m_borderColor = value;
                m_borderPen?.Dispose();
                m_borderPen = new Pen(value);
                Invalidate();
            }
        }

        /// <summary>
        /// 设置进度变更的速度
        /// </summary>
        [Description("获取或设置进度条的变化进度")]
        [Category("外观")]
        [Browsable(true)]
        [DefaultValue(1)]
        public int ValueChangeSpeed
        {
            get{return m_speed;}
            set
            {
                if (value >= 1)
                {
                    m_speed = value;
                }
            }
        }



        /// <summary>
        /// 获取或设置进度条变化的时候是否采用动画效果
        /// </summary>
        [Description("获取或设置进度条变化的时候是否采用动画效果")]
        [Category("外观")]
        [Browsable(true)]
        [DefaultValue(false)]
        public bool UseAnimation
        {
            get { return m_UseAnimation; }
            set { m_UseAnimation = value; }
        }


        /// <summary>
        /// 进度条的样式
        /// </summary>
        [Description("获取或设置进度条的样式")]
        [Category("外观")]
        [Browsable(true)]
        [DefaultValue(typeof(ProgressStyle),"Vertical")]
        public ProgressStyle ProgressStyle
        {
            get { return m_progressStyle; }
            set { m_progressStyle = value;Invalidate(); }
        }


        #endregion

        #region Update Progress


        private void ThreadPoolUpdateProgress(object obj)
        {
            try
            {
                // 开始计算更新细节
                int version = (int)obj;
                if (m_speed < 1) m_speed = 1;

                while (m_actual != m_value)
                {
                    System.Threading.Thread.Sleep(17);

                    if (version != m_version) break;

                    hybirdLock.Enter();

                    int newActual = 0;
                    if (m_actual > m_value)
                    {
                        int offect = m_actual - m_value;
                        if (offect > m_speed) offect = m_speed;
                        newActual = m_actual - offect;
                    }
                    else
                    {
                        int offect = m_value - m_actual;
                        if (offect > m_speed) offect = m_speed;
                        newActual = m_actual + offect;
                    }
                    m_actual = newActual;

                    hybirdLock.Leave();
                    
                    if (version == m_version)
                    {
                        if (IsHandleCreated) Invoke(m_UpdateAction);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch(Exception)
            {
                // BasicFramework.SoftBasic.ShowExceptionMessage(ex);
            }
            
        }

        private void UpdateRender()
        {
            Invalidate();
        }

        #endregion


    }

    /// <summary>
    /// 进度条的样式
    /// </summary>
    public enum ProgressStyle
    {
        /// <summary>
        /// 竖直的，纵向的进度条
        /// </summary>
        Vertical,
        /// <summary>
        /// 水平进度条
        /// </summary>
        Horizontal
    }
}
