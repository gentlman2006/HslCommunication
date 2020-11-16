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
    /// 一个自定义的按钮控件
    /// </summary>
    [DefaultEvent("Click")]
    public partial class UserButton : UserControl
    {
        /// <summary>
        /// 实例化一个按钮对象
        /// </summary>
        public UserButton()
        {
            InitializeComponent();
        }

        private void UserButton_Load(object sender, EventArgs e)
        {
            sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            SizeChanged += UserButton_SizeChanged;
            Font = new Font("微软雅黑", Font.Size, Font.Style);
            MouseEnter += UserButton_MouseEnter;
            MouseLeave += UserButton_MouseLeave;

            MouseDown += UserButton_MouseDown;
            MouseUp += UserButton_MouseUp;

            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        private void UserButton_SizeChanged(object sender, EventArgs e)
        {
            if (Width > 1)
            {
                Invalidate();
            }
        }

        private int RoundCorner = 3;
        private StringFormat sf = null;
        private string _text = "button";
        /// <summary>
        /// 设置或获取显示的文本
        /// </summary>
        [Category("外观")]
        [DefaultValue("button")]
        [Description("用来设置显示的文本信息")]
        public string UIText
        {
            get { return _text; }
            set { _text = value; Invalidate(); }
        }
        /// <summary>
        /// 设置或获取显示文本的颜色
        /// </summary>
        [Category("外观")]
        [DefaultValue(typeof(Color), "Black")]
        [Description("用来设置显示的文本的颜色")]
        public Color TextColor { get; set; } = Color.Black;

        /// <summary>
        /// 设置按钮的圆角
        /// </summary>
        [Category("外观")]
        [DefaultValue(3)]
        [Description("按钮框的圆角属性")]
        public int CornerRadius
        {
            get { return RoundCorner; }
            set { RoundCorner = value; Invalidate(); }
        }

        private bool m_Selected = false;
        /// <summary>
        /// 用来设置按钮的选中状态
        /// </summary>
        [Category("外观")]
        [DefaultValue(false)]
        [Description("指示按钮的选中状态")]
        public bool Selected
        {
            get { return m_Selected; }
            set { m_Selected = value; Invalidate(); }
        }
        /// <summary>
        /// 已经弃用
        /// </summary>
        [Browsable(false)]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }
        /// <summary>
        /// 已经弃用
        /// </summary>
        [Browsable(false)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private Color m_backcor = Color.Lavender;
        /// <summary>
        /// 按钮的背景色
        /// </summary>
        [Category("外观")]
        [DefaultValue(typeof(Color), "Lavender")]
        [Description("按钮的背景色")]
        public Color OriginalColor
        {
            get{return m_backcor; }
            set{m_backcor = value;Invalidate(); }
        }

        private Color m_enablecolor = Color.FromArgb(190, 190, 190);
        /// <summary>
        /// 按钮的背景色
        /// </summary>
        [Category("外观")]
        [Description("按钮的活动色")]
        public Color EnableColor
        {
            get { return m_enablecolor; }
            set { m_enablecolor = value; Invalidate(); }
        }



        private Color m_active = Color.AliceBlue;
        /// <summary>
        /// 鼠标挪动时的活动颜色
        /// </summary>
        [Category("外观")]
        [DefaultValue(typeof(Color), "AliceBlue")]
        [Description("按钮的活动色")]
        public Color  ActiveColor
        {
            get { return m_active; }
            set { m_active = value; Invalidate(); }
        }

        private bool m_BorderVisiable = true;
        /// <summary>
        /// 设置按钮的边框是否可见
        /// </summary>
        [Category("外观")]
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("指示按钮是否存在边框")]
        public bool BorderVisiable
        {
            get { return m_BorderVisiable; }
            set { m_BorderVisiable = value; Invalidate(); }
        }
        /// <summary>
        /// 存放用户需要保存的一些额外的信息
        /// </summary>
        [Browsable(false)]
        public string CustomerInformation { get; set; } = "";


        #region 光标移动块
        private bool is_mouse_on { get; set; } = false;


        private void UserButton_MouseLeave(object sender, EventArgs e)
        {
            is_mouse_on = false;
            Invalidate();
        }

        private void UserButton_MouseEnter(object sender, EventArgs e)
        {
            is_mouse_on = true;
            Invalidate();
        }

        #endregion


        #region 鼠标点击块

        private bool is_left_mouse_down { get; set; } = false;
        private void UserButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                is_left_mouse_down = false;
                Invalidate();
            }
        }

        private void UserButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                is_left_mouse_down = true;
                Invalidate();
            }
        }

        #endregion

        


        /// <summary>
        /// 触发一次点击的事件
        /// </summary>
        public void PerformClick()
        {
            OnClick(new EventArgs());
        }


        /// <summary>
        /// 重绘数据区
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;



            GraphicsPath path = new GraphicsPath();
            path.AddLine(RoundCorner, 0, Width - RoundCorner - 1, 0);
            path.AddArc(Width - RoundCorner * 2 - 1, 0, RoundCorner * 2, RoundCorner * 2, 270f, 90f);
            path.AddLine(Width - 1, RoundCorner, Width - 1, Height - RoundCorner - 1);
            path.AddArc(Width - RoundCorner * 2 - 1, Height - RoundCorner * 2 - 1, RoundCorner * 2, RoundCorner * 2, 0f, 90f);
            path.AddLine(Width - RoundCorner - 1, Height - 1, RoundCorner, Height - 1);
            path.AddArc(0, Height - RoundCorner * 2 - 1, RoundCorner * 2, RoundCorner * 2, 90f, 90f);
            path.AddLine(0, Height - RoundCorner - 1, 0, RoundCorner);
            path.AddArc(0, 0, RoundCorner * 2, RoundCorner * 2, 180f, 90f);


            Brush brush_fore_text = null;
            Brush brush_back_text = null;
            Rectangle rect_text = new Rectangle(ClientRectangle.X, ClientRectangle.Y,
                ClientRectangle.Width, ClientRectangle.Height);
            if (Enabled)
            {
                brush_fore_text = new SolidBrush(TextColor);
                if(Selected)
                {
                    brush_back_text = new SolidBrush(Color.DodgerBlue);
                }
                else if (is_mouse_on)
                {
                    brush_back_text = new SolidBrush(ActiveColor);
                }
                else
                {
                    brush_back_text = new SolidBrush(OriginalColor);
                }
                if (is_left_mouse_down)
                {
                    rect_text.Offset(1, 1);
                }
            }
            else
            {
                brush_fore_text = new SolidBrush(Color.Gray);
                brush_back_text = new SolidBrush(EnableColor);
            }

            e.Graphics.FillPath(brush_back_text, path);
            Pen pen_border = new Pen(Color.FromArgb(170, 170, 170));
            if (BorderVisiable)
            {
                e.Graphics.DrawPath(pen_border, path);
            }
            e.Graphics.DrawString(UIText, Font, brush_fore_text, rect_text, sf);


            //base.OnPaint(e);
            brush_fore_text.Dispose();
            brush_back_text.Dispose();
            pen_border.Dispose();
            path.Dispose();
        }

        private void UserButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnClick(new EventArgs());
            }
        }


        /// <summary>
        /// 点击按钮的触发事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            if (Enabled)
            {
                base.OnClick(e);
            }
        }

        /// <summary>
        /// 点击的时候触发事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (Enabled)
            {
                base.OnMouseClick(e);
            }
        }
    }
}
