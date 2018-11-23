using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


/*******************************************************************************
 * 
 *    文件名：UserCurve.cs
 *    程序功能：显示曲线信息，包含了曲线，坐标轴，鼠标移动信息
 *    
 *    创建人：Richard.Hu
 *    时间：2018年1月21日 18:36:08
 * 
 *******************************************************************************/


namespace HslCommunication.Controls
{
    /// <summary>
    /// 曲线控件对象
    /// </summary>
    /// <remarks>
    /// 详细参照如下的博客:
    /// </remarks>
    public partial class UserCurve : UserControl
    {
        #region Constructor

        /// <summary>
        /// 实例化一个曲线显示的控件
        /// </summary>
        public UserCurve( )
        {
            InitializeComponent( );
            DoubleBuffered = true;
            random = new Random( );
            data_list = new Dictionary<string, HslCurveItem>( );
            auxiliary_lines = new List<AuxiliaryLine>( );

            format_left = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near
            };

            format_right = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Far,
            };

            format_center = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center,
            };



            font_size9 = new Font( "宋体", 9 );
            font_size12 = new Font( "宋体", 12 );
            InitializationColor( );
            pen_dash = new Pen( color_deep );
            pen_dash.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            pen_dash.DashPattern = new float[] { 5, 5 };
        }

        #endregion

        #region Const Data

        private const int value_count_max = 2048;           // 缓存的数据的最大量

        #endregion

        #region Private Member

        private float value_max_left = 100;                 // 左坐标的最大值
        private float value_min_left = 0;                   // 左坐标的最小值
        private float value_max_right = 100;                // 右坐标的最大值
        private float value_min_right = 0;                  // 右坐标的最小值

        private int value_Segment = 5;                      // 纵轴的片段分割
        private bool value_IsAbscissaStrech = false;        // 指示横坐标是否填充满整个坐标系
        private int value_StrechDataCountMax = 300;         // 拉伸模式下的最大数据量
        private bool value_IsRenderDashLine = true;         // 是否显示虚线的信息
        // private bool value_IsRenderAbscissaText = false;    // 指示是否显示横轴的文本信息
        private string textFormat = "HH:mm";                // 时间文本的信息
        private int value_IntervalAbscissaText = 100;       // 指示显示横轴文本的间隔数据
        private Random random = null;                       // 获取随机颜色使用
        private string value_title = "";                    // 图表的标题


        private int leftRight = 50;
        private int upDowm = 25;


        #endregion

        #region Data Member

        Dictionary<string, HslCurveItem> data_list = null;  // 等待显示的实际数据
        private string[] data_text = null;                  // 等待显示的横轴信息

        #endregion

        #region Public Properties

        /// <summary>
        /// 获取或设置图形的纵坐标的最大值，该值必须大于最小值
        /// </summary>
        [Category( "外观" )]
        [Description( "获取或设置图形的左纵坐标的最大值，该值必须大于最小值" )]
        [Browsable( true )]
        [DefaultValue( 100f )]
        public float ValueMaxLeft
        {
            get { return value_max_left; }
            set
            {
                if (value > value_min_left)
                {
                    value_max_left = value;
                    Invalidate( );
                }
            }
        }

        /// <summary>
        /// 获取或设置图形的纵坐标的最小值，该值必须小于最大值
        /// </summary>
        [Category( "外观" )]
        [Description( "获取或设置图形的左纵坐标的最小值，该值必须小于最大值" )]
        [Browsable( true )]
        [DefaultValue( 0f )]
        public float ValueMinLeft
        {
            get { return value_min_left; }
            set
            {
                if (value < value_max_left)
                {
                    value_min_left = value;
                    Invalidate( );
                }
            }
        }

        /// <summary>
        /// 获取或设置图形的纵坐标的最大值，该值必须大于最小值
        /// </summary>
        [Category( "外观" )]
        [Description( "获取或设置图形的右纵坐标的最大值，该值必须大于最小值" )]
        [Browsable( true )]
        [DefaultValue( 100f )]
        public float ValueMaxRight
        {
            get { return value_max_right; }
            set
            {
                if (value > value_min_right)
                {
                    value_max_right = value;
                    Invalidate( );
                }
            }
        }

        /// <summary>
        /// 获取或设置图形的纵坐标的最小值，该值必须小于最大值
        /// </summary>
        [Category( "外观" )]
        [Description( "获取或设置图形的右纵坐标的最小值，该值必须小于最大值" )]
        [Browsable( true )]
        [DefaultValue( 0f )]
        public float ValueMinRight
        {
            get { return value_min_right; }
            set
            {
                if (value < value_max_right)
                {
                    value_min_right = value;
                    Invalidate( );
                }
            }
        }

        /// <summary>
        /// 获取或设置图形的纵轴分段数
        /// </summary>
        [Category( "外观" )]
        [Description( "获取或设置图形的纵轴分段数" )]
        [Browsable( true )]
        [DefaultValue( 5 )]
        public int ValueSegment
        {
            get { return value_Segment; }
            set
            {
                value_Segment = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// 获取或设置所有的数据是否强制在一个界面里显示
        /// </summary>
        [Category( "外观" )]
        [Description( "获取或设置所有的数据是否强制在一个界面里显示" )]
        [Browsable( true )]
        [DefaultValue( false )]
        public bool IsAbscissaStrech
        {
            get { return value_IsAbscissaStrech; }
            set
            {
                value_IsAbscissaStrech = value;
                Invalidate( );
            }
        }


        /// <summary>
        /// 获取或设置拉伸模式下的最大数据量
        /// </summary>
        [Category( "外观" )]
        [Description( "获取或设置拉伸模式下的最大数据量" )]
        [Browsable( true )]
        [DefaultValue( 300 )]
        public int StrechDataCountMax
        {
            get { return value_StrechDataCountMax; }
            set
            {
                value_StrechDataCountMax = value;
                Invalidate( );
            }
        }


        /// <summary>
        /// 获取或设置虚线是否进行显示
        /// </summary>
        [Category( "外观" )]
        [Description( "获取或设置虚线是否进行显示" )]
        [Browsable( true )]
        [DefaultValue( true )]
        public bool IsRenderDashLine
        {
            get { return value_IsRenderDashLine; }
            set
            {
                value_IsRenderDashLine = value;
                Invalidate( );
            }
        }


        /// <summary>
        /// 获取或设置坐标轴及相关信息文本的颜色
        /// </summary>
        [Category( "外观" )]
        [Description( "获取或设置坐标轴及相关信息文本的颜色" )]
        [Browsable( true )]
        [DefaultValue( typeof( Color ), "DimGray" )]
        public Color ColorLinesAndText
        {
            get { return color_deep; }
            set
            {
                color_deep = value;
                InitializationColor( );
                Invalidate( );
            }
        }


        /// <summary>
        /// 获取或设置虚线的颜色
        /// </summary>
        [Category( "外观" )]
        [Description( "获取或设置虚线的颜色" )]
        [Browsable( true )]
        [DefaultValue( typeof( Color ), "Gray" )]
        public Color ColorDashLines
        {
            get { return color_dash; }
            set
            {
                color_dash = value;
                pen_dash?.Dispose( );
                pen_dash = new Pen( color_dash );
                pen_dash.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                pen_dash.DashPattern = new float[] { 5, 5 };
                Invalidate( );
            }
        }


        /// <summary>
        /// 获取或设置纵向虚线的分隔情况，单位为多少个数据
        /// </summary>
        [Category( "外观" )]
        [Description( "获取或设置纵向虚线的分隔情况，单位为多少个数据" )]
        [Browsable( true )]
        [DefaultValue( 100 )]
        public int IntervalAbscissaText
        {
            get { return value_IntervalAbscissaText; }
            set
            {
                value_IntervalAbscissaText = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// 获取或设置实时数据新增时文本相对应于时间的格式化字符串，默认HH:mm
        /// </summary>
        [Category( "外观" )]
        [Description( "获取或设置实时数据新增时文本相对应于时间的格式化字符串，默认HH:mm" )]
        [Browsable( true )]
        [DefaultValue( "HH:mm" )]
        public string TextAddFormat
        {
            get { return textFormat; }
            set { textFormat = value; Invalidate(); }
        }


        /// <summary>
        /// 获取或设置图标的标题信息
        /// </summary>
        [Category("外观")]
        [Description("获取或设置图标的标题信息")]
        [Browsable(true)]
        [DefaultValue("")]
        public string Title
        {
            get { return value_title; }
            set { value_title = value; Invalidate(); }
        }


        private void InitializationColor( )
        {
            pen_normal?.Dispose( );
            brush_deep?.Dispose( );
            pen_normal = new Pen( color_deep );
            brush_deep = new SolidBrush( color_deep );
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 设置曲线的横坐标文本，适用于显示一些固定的曲线信息
        /// </summary>
        /// <param name="descriptions">应该和曲线的点数一致</param>
        public void SetCurveText(string[] descriptions)
        {
            data_text = descriptions;

            // 重绘图形
            Invalidate( );
        }


        /// <summary>
        /// 新增或修改一条指定关键字的左参考系曲线数据，需要指定数据，颜色随机，没有数据上限，线条宽度为1
        /// </summary>
        /// <param name="key">曲线关键字</param>
        /// <param name="data">曲线的具体数据</param>
        public void SetLeftCurve( string key, float[] data )
        {
            SetLeftCurve( key, data, Color.FromArgb( random.Next( 256 ), random.Next( 256 ), random.Next( 256 ) ) );
        }
        

        /// <summary>
        /// 新增或修改一条指定关键字的左参考系曲线数据，需要指定数据，颜色，没有数据上限，线条宽度为1
        /// </summary>
        /// <param name="key">曲线关键字</param>
        /// <param name="data"></param>
        /// <param name="lineColor"></param>
        public void SetLeftCurve( string key, float[] data, Color lineColor )
        {
            SetCurve( key, true, data, lineColor, 1f );
        }

        /// <summary>
        /// 新增或修改一条指定关键字的右参考系曲线数据，需要指定数据，颜色随机，没有数据上限，线条宽度为1
        /// </summary>
        /// <param name="key">曲线关键字</param>
        /// <param name="data"></param>
        public void SetRightCurve( string key, float[] data )
        {
            SetRightCurve( key, data, Color.FromArgb( random.Next( 256 ), random.Next( 256 ), random.Next( 256 ) ) );
        }

        /// <summary>
        /// 新增或修改一条指定关键字的右参考系曲线数据，需要指定数据，颜色，没有数据上限，线条宽度为1
        /// </summary>
        /// <param name="key">曲线关键字</param>
        /// <param name="data"></param>
        /// <param name="lineColor"></param>
        public void SetRightCurve( string key, float[] data, Color lineColor )
        {
            SetCurve( key, false, data, lineColor, 1f );
        }


        /// <summary>
        /// 新增或修改一条指定关键字的曲线数据，需要指定参考系及数据，颜色，线条宽度
        /// </summary>
        /// <param name="key">曲线关键字</param>
        /// <param name="isLeft">是否以左侧坐标轴为参照系</param>
        /// <param name="data">数据</param>
        /// <param name="lineColor">线条颜色</param>
        /// <param name="thickness">线条宽度</param>
        public void SetCurve( string key, bool isLeft, float[] data, Color lineColor, float thickness )
        {
            if (data_list.ContainsKey( key ))
            {
                if (data == null) data = new float[] { };
                data_list[key].Data = data;
            }
            else
            {
                if (data == null) data = new float[] { };
                data_list.Add( key, new HslCurveItem( )
                {
                    Data = data,
                    LineThickness = thickness,
                    LineColor = lineColor,
                    IsLeftFrame = isLeft,
                } );

                if (data_text == null) data_text = new string[data.Length];
            }

            // 重绘图形
            Invalidate( );
        }


        /// <summary>
        /// 移除指定关键字的曲线
        /// </summary>
        /// <param name="key">曲线关键字</param>
        public void RemoveCurve( string key )
        {
            if (data_list.ContainsKey( key ))
            {
                data_list.Remove( key );
            }
            // 重绘图形
            Invalidate( );
        }


        /// <summary>
        /// 移除指定关键字的曲线
        /// </summary>
        public void RemoveAllCurve( )
        {
            int count = data_list.Count;
            data_list.Clear( );
            // 重绘图形
            if (count > 0) Invalidate( );
        }


        // ======================================================================================================



        /// <summary>
        /// 新增指定关键字曲线的一个数据，注意该关键字的曲线必须存在，否则无效
        /// </summary>
        /// <param name="key">新增曲线的关键字</param>
        /// <param name="values"></param>
        /// <param name="isUpdateUI">是否刷新界面</param>
        private void AddCurveData( string key, float[] values, bool isUpdateUI )
        {
            if (values?.Length < 1) return;                              // 没有传入数据

            if (data_list.ContainsKey( key ))
            {
                HslCurveItem curve = data_list[key];
                if (curve.Data != null)
                {
                    if (value_IsAbscissaStrech)
                    {
                        // 填充玩整个图形的情况
                        BasicFramework.SoftBasic.AddArrayData( ref curve.Data, values, value_StrechDataCountMax );
                    }
                    else
                    {
                        // 指定点的情况，必然存在最大值情况
                        BasicFramework.SoftBasic.AddArrayData( ref curve.Data, values, value_count_max );
                    }

                    if (isUpdateUI) Invalidate( );
                }
            }
        }

        // 新增曲线的时间节点
        private void AddCurveTime( int count )
        {
            if (data_text == null) return;
            string[] values = new string[count];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = DateTime.Now.ToString( textFormat );
            }


            if(value_IsAbscissaStrech)
            {
                BasicFramework.SoftBasic.AddArrayData( ref data_text, values, value_StrechDataCountMax );
            }
            else
            {
                BasicFramework.SoftBasic.AddArrayData( ref data_text, values, value_count_max );
            }
        }

        /// <summary>
        /// 新增指定关键字曲线的一个数据，注意该关键字的曲线必须存在，否则无效
        /// </summary>
        /// <param name="key">曲线的关键字</param>
        /// <param name="value">数据值</param>
        public void AddCurveData( string key, float value )
        {
            AddCurveData( key, new float[] { value } );
        }

        /// <summary>
        /// 新增指定关键字曲线的一组数据，注意该关键字的曲线必须存在，否则无效
        /// </summary>
        /// <param name="key">曲线的关键字</param>
        /// <param name="values">数组值</param>
        public void AddCurveData( string key, float[] values )
        {
            AddCurveData( key, values, false );
            if (values?.Length > 0)
            {
                AddCurveTime( values.Length );
            }
            Invalidate( );
        }

        /// <summary>
        /// 新增指定关键字数组曲线的一组数据，注意该关键字的曲线必须存在，否则无效，一个数据对应一个数组
        /// </summary>
        /// <param name="keys">曲线的关键字数组</param>
        /// <param name="values">数组值</param>
        public void AddCurveData( string[] keys, float[] values )
        {
            if (keys == null) throw new ArgumentNullException( "keys" );
            if (values == null) throw new ArgumentNullException( "values" );
            if (keys.Length != values.Length) throw new Exception( "两个参数的数组长度不一致。" );

            
            for (int i = 0; i < keys.Length; i++)
            {
                AddCurveData( keys[i], new float[] { values[i] }, false );
            }

            AddCurveTime( 1 );
            // 统一的更新显示
            Invalidate( );
        }
        

        /// <summary>
        /// 设置一条曲线是否是可见的，如果该曲线不存在，则无效。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="visible">是否可见</param>
        public void SetCurveVisible( string key, bool visible )
        {
            if (data_list.ContainsKey( key ))
            {
                HslCurveItem curve = data_list[key];
                curve.Visible = visible;
                Invalidate( );
            }
        }

        /// <summary>
        /// 设置多条曲线是否是可见的，如果该曲线不存在，则无效。
        /// </summary>
        /// <param name="keys">关键字</param>
        /// <param name="visible">是否可见</param>
        public void SetCurveVisible( string[] keys, bool visible )
        {
            foreach (var key in keys)
            {
                if (data_list.ContainsKey( key ))
                {
                    HslCurveItem curve = data_list[key];
                    curve.Visible = visible;
                }
            }
            Invalidate( );
        }


        #endregion

        #region Auxiliary Line

        // 辅助线的信息，允许自定义辅助线信息，来标注特殊的线条

        private List<AuxiliaryLine> auxiliary_lines;               // 所有辅助线的列表

        /// <summary>
        /// 新增一条左侧的辅助线，使用默认的文本颜色
        /// </summary>
        /// <param name="value">数据值</param>
        public void AddLeftAuxiliary( float value )
        {
            AddLeftAuxiliary( value, ColorLinesAndText );
        }

        /// <summary>
        /// 新增一条左侧的辅助线，使用指定的颜色
        /// </summary>
        /// <param name="value">数据值</param>
        /// <param name="lineColor">线条颜色</param>
        public void AddLeftAuxiliary( float value, Color lineColor )
        {
            AddLeftAuxiliary( value, lineColor, 1f );
        }

        /// <summary>
        /// 新增一条左侧的辅助线
        /// </summary>
        /// <param name="value">数据值</param>
        /// <param name="lineColor">线条颜色</param>
        /// <param name="lineThickness">线条宽度</param>
        public void AddLeftAuxiliary( float value, Color lineColor, float lineThickness )
        {
            AddAuxiliary( value, lineColor, lineThickness, true );
        }


        /// <summary>
        /// 新增一条右侧的辅助线，使用默认的文本颜色
        /// </summary>
        /// <param name="value">数据值</param>
        public void AddRightAuxiliary( float value )
        {
            AddRightAuxiliary( value, ColorLinesAndText );
        }

        /// <summary>
        /// 新增一条右侧的辅助线，使用指定的颜色
        /// </summary>
        /// <param name="value">数据值</param>
        /// <param name="lineColor">线条颜色</param>
        public void AddRightAuxiliary( float value, Color lineColor )
        {
            AddRightAuxiliary( value, lineColor, 1f );
        }


        /// <summary>
        /// 新增一条右侧的辅助线
        /// </summary>
        /// <param name="value">数据值</param>
        /// <param name="lineColor">线条颜色</param>
        /// <param name="lineThickness">线条宽度</param>
        public void AddRightAuxiliary( float value, Color lineColor, float lineThickness )
        {
            AddAuxiliary( value, lineColor, lineThickness, false );
        }


        private void AddAuxiliary( float value, Color lineColor, float lineThickness, bool isLeft )
        {
            auxiliary_lines.Add( new AuxiliaryLine( )
            {
                Value = value,
                LineColor = lineColor,
                PenDash = new Pen( lineColor )
                {
                    DashStyle = System.Drawing.Drawing2D.DashStyle.Custom,
                    DashPattern = new float[] { 5, 5 }
                },
                IsLeftFrame = isLeft,
                LineThickness = lineThickness,
                LineTextBrush = new SolidBrush( lineColor ),
            } );
            Invalidate( );
        }

        /// <summary>
        /// 移除所有的指定值的辅助曲线，包括左边的和右边的
        /// </summary>
        /// <param name="value"></param>
        public void RemoveAuxiliary( float value )
        {
            int removeCount = 0;
            for (int i = auxiliary_lines.Count - 1; i >= 0; i--)
            {
                if (auxiliary_lines[i].Value == value)
                {
                    auxiliary_lines[i].Dispose( );
                    auxiliary_lines.RemoveAt( i );
                    removeCount++;
                }
            }
            if (removeCount > 0) Invalidate( );
        }


        /// <summary>
        /// 移除所有的辅助线
        /// </summary>
        public void RemoveAllAuxiliary( )
        {
            int removeCount = auxiliary_lines.Count;
            auxiliary_lines.Clear( );

            if (removeCount > 0) Invalidate( );
        }


        #endregion

        #region Private Method




        #endregion

        #region Core Paint


        private Font font_size9 = null;
        private Font font_size12 = null;

        private Brush brush_deep = null;                  // 文本的颜色

        private Pen pen_normal = null;                    // 绘制极轴和分段符的坐标线
        private Pen pen_dash = null;                      // 绘制图形的虚线

        private Color color_normal = Color.DeepPink;      // 文本的颜色
        private Color color_deep = Color.DimGray;         // 坐标轴及数字文本的信息
        private Color color_dash = Color.Gray;            // 虚线的颜色

        private StringFormat format_left = null;          // 靠左对齐的文本
        private StringFormat format_right = null;         // 靠右对齐的文本
        private StringFormat format_center = null;        // 中间对齐的文本


        private void UserCurve_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            if (BackColor != Color.Transparent)
            {
                g.Clear( BackColor );
            }


            int width_totle = Width;
            int heigh_totle = Height;

            if (width_totle < 120 || heigh_totle < 60) return;


            // 绘制极轴
            g.DrawLines( pen_normal, new Point[] {
                new Point(leftRight-1, upDowm - 8),
                new Point(leftRight-1, heigh_totle - upDowm),
                new Point(width_totle - leftRight, heigh_totle - upDowm),
                new Point(width_totle - leftRight, upDowm - 8)
            } );

            // 绘制图表的标题
            if(!string.IsNullOrEmpty(value_title))
            {
                g.DrawString(value_title, font_size9, brush_deep, new Rectangle(0, 0, width_totle - 1, 20), format_center);
            }

            // 绘制倒三角
            BasicFramework.SoftPainting.PaintTriangle( g, brush_deep, new Point( leftRight - 1, upDowm - 8 ), 4, BasicFramework.GraphDirection.Upward );
            BasicFramework.SoftPainting.PaintTriangle( g, brush_deep, new Point( width_totle - leftRight, upDowm - 8 ), 4, BasicFramework.GraphDirection.Upward );

            // 计算所有辅助线的实际值
            for (int i = 0; i < auxiliary_lines.Count; i++)
            {
                if (auxiliary_lines[i].IsLeftFrame)
                {
                    auxiliary_lines[i].PaintValue = BasicFramework.SoftPainting.ComputePaintLocationY( value_max_left, value_min_left, (heigh_totle - upDowm - upDowm), auxiliary_lines[i].Value ) + upDowm;
                }
                else
                {
                    auxiliary_lines[i].PaintValue = BasicFramework.SoftPainting.ComputePaintLocationY( value_max_right, value_min_right, (heigh_totle - upDowm - upDowm), auxiliary_lines[i].Value ) + upDowm;
                }
            }

            // 绘制刻度线，以及刻度文本
            for (int i = 0; i <= value_Segment; i++)
            {
                float valueTmpLeft = i * (value_max_left - value_min_left) / value_Segment + value_min_left;
                float paintTmp = BasicFramework.SoftPainting.ComputePaintLocationY( value_max_left, value_min_left, (heigh_totle - upDowm - upDowm), valueTmpLeft ) + upDowm;
                if (IsNeedPaintDash( paintTmp ))
                {
                    // 左坐标轴
                    g.DrawLine( pen_normal, leftRight - 4, paintTmp, leftRight - 1, paintTmp );
                    RectangleF rectTmp = new RectangleF( 0, paintTmp - 9, leftRight - 4, 20 );
                    g.DrawString( valueTmpLeft.ToString( ), font_size9, brush_deep, rectTmp, format_right );

                    // 右坐标轴
                    float valueTmpRight = i * (value_max_right - value_min_right) / value_Segment + value_min_right;
                    g.DrawLine( pen_normal, width_totle - leftRight + 1, paintTmp, width_totle - leftRight + 4, paintTmp );
                    rectTmp.Location = new PointF( width_totle - leftRight + 4, paintTmp - 9 );
                    g.DrawString( valueTmpRight.ToString( ), font_size9, brush_deep, rectTmp, format_left );

                    if (i > 0 && value_IsRenderDashLine) g.DrawLine( pen_dash, leftRight, paintTmp, width_totle - leftRight, paintTmp );
                }
            }

            // 绘制纵向虚线信息
            if (value_IsRenderDashLine)
            {
                if (value_IsAbscissaStrech)
                {
                    // 拉伸模式下，因为错位是均匀的，所以根据数据来显示
                    float offect = (width_totle - leftRight * 2) * 1.0f / (value_StrechDataCountMax - 1);
                    int dataCount = CalculateDataCountByOffect( offect );
                    for (int i = 0; i < value_StrechDataCountMax; i += dataCount)
                    {
                        if (i > 0 && i < value_StrechDataCountMax - 1)
                        {
                            g.DrawLine( pen_dash, i * offect + leftRight, upDowm, i * offect + leftRight, heigh_totle - upDowm - 1 );
                        }

                        if (data_text != null)
                        {
                            if (i < data_text.Length && ((i * offect + leftRight) < (data_text.Length - 1) * offect + leftRight -40 ))
                            {
                                Rectangle rec = new Rectangle( (int)(i * offect), heigh_totle - upDowm + 1, 100, upDowm );
                                g.DrawString( data_text[i], font_size9, brush_deep, rec, format_center );
                            }
                        }
                    }

                    if (data_text?.Length > 1)
                    {
                        if (data_text.Length < value_StrechDataCountMax)
                        {
                            // 绘制最前端的虚线
                            g.DrawLine( pen_dash, (data_text.Length - 1) * offect + leftRight, upDowm, (data_text.Length - 1) * offect + leftRight, heigh_totle - upDowm - 1 );
                        }

                        Rectangle rec = new Rectangle( (int)((data_text.Length - 1) * offect + leftRight) - leftRight, heigh_totle - upDowm + 1, 100, upDowm );
                        g.DrawString( data_text[data_text.Length - 1], font_size9, brush_deep, rec, format_center );
                    }
                }
                else
                {
                    int countTmp = width_totle - 2 * leftRight + 1;
                    // 普通模式下绘制图形
                    for (int i = leftRight; i < width_totle - leftRight; i += value_IntervalAbscissaText)
                    {
                        if (i != leftRight)
                        {
                            g.DrawLine( pen_dash, i, upDowm, i, heigh_totle - upDowm - 1 );
                        }

                        if (data_text != null)
                        {
                            int right_limit = countTmp > data_text.Length ? data_text.Length : countTmp;

                            if ((i - leftRight) < data_text.Length)
                            {
                                if ((right_limit - (i - leftRight)) > 40)
                                {
                                    // 点数大于1的时候才绘制
                                    if (data_text.Length <= countTmp)
                                    {
                                        Rectangle rec = new Rectangle( i - leftRight, heigh_totle - upDowm + 1, 100, upDowm );
                                        g.DrawString( data_text[i - leftRight], font_size9, brush_deep, rec, format_center );
                                    }
                                    else
                                    {
                                        Rectangle rec = new Rectangle( i - leftRight, heigh_totle - upDowm + 1, 100, upDowm );
                                        g.DrawString( data_text[i - leftRight + data_text.Length - countTmp], font_size9, brush_deep, rec, format_center );
                                    }
                                }
                            }
                        }
                    }

                    if (data_text?.Length > 1)
                    {
                        if (data_text.Length < countTmp)
                        {
                            // 绘制最前端的虚线
                            g.DrawLine( pen_dash, (data_text.Length + leftRight - 1), upDowm, (data_text.Length + leftRight - 1), heigh_totle - upDowm - 1 );
                            Rectangle rec = new Rectangle( (data_text.Length + leftRight - 1) - leftRight, heigh_totle - upDowm + 1, 100, upDowm );
                            g.DrawString( data_text[data_text.Length - 1], font_size9, brush_deep, rec, format_center );
                        }
                        else
                        {
                            Rectangle rec = new Rectangle( width_totle - leftRight - leftRight, heigh_totle - upDowm + 1, 100, upDowm );
                            g.DrawString( data_text[data_text.Length - 1], font_size9, brush_deep, rec, format_center );
                        }
                    }
                }
            }

            // 绘制辅助线信息
            for (int i = 0; i < auxiliary_lines.Count; i++)
            {
                if (auxiliary_lines[i].IsLeftFrame)
                {
                    // 左坐标轴
                    g.DrawLine( auxiliary_lines[i].PenDash, leftRight - 4, auxiliary_lines[i].PaintValue, leftRight - 1, auxiliary_lines[i].PaintValue );
                    RectangleF rectTmp = new RectangleF( 0, auxiliary_lines[i].PaintValue - 9, leftRight - 4, 20 );
                    g.DrawString( auxiliary_lines[i].Value.ToString( ), font_size9, auxiliary_lines[i].LineTextBrush, rectTmp, format_right );
                }
                else
                {
                    g.DrawLine( auxiliary_lines[i].PenDash, width_totle - leftRight + 1, auxiliary_lines[i].PaintValue, width_totle - leftRight + 4, auxiliary_lines[i].PaintValue );
                    RectangleF rectTmp = new RectangleF( width_totle - leftRight + 4, auxiliary_lines[i].PaintValue - 9, leftRight - 4, 20 );
                    g.DrawString( auxiliary_lines[i].Value.ToString( ), font_size9, auxiliary_lines[i].LineTextBrush, rectTmp, format_left );
                }
                g.DrawLine( auxiliary_lines[i].PenDash, leftRight, auxiliary_lines[i].PaintValue, width_totle - leftRight, auxiliary_lines[i].PaintValue );
            }

            // 绘制线条
            if (value_IsAbscissaStrech)
            {
                // 横坐标充满图形
                foreach (var line in data_list.Values)
                {
                    if (line.Data?.Length > 1)
                    {
                        float offect = (width_totle - leftRight * 2) * 1.0f / (value_StrechDataCountMax - 1);

                        // 点数大于1的时候才绘制
                        PointF[] points = new PointF[line.Data.Length];
                        for (int i = 0; i < line.Data.Length; i++)
                        {
                            points[i].X = leftRight + i * offect;
                            points[i].Y = BasicFramework.SoftPainting.ComputePaintLocationY(
                                line.IsLeftFrame ? value_max_left : value_max_right,
                                line.IsLeftFrame ? value_min_left : value_min_right,
                                (heigh_totle - upDowm - upDowm), line.Data[i] ) + upDowm;
                        }

                        using (Pen penTmp = new Pen( line.LineColor, line.LineThickness ))
                        {
                            g.DrawLines( penTmp, points );
                        }
                    }
                }
            }
            else
            {
                // 横坐标对应图形
                foreach (var line in data_list.Values)
                {
                    if (!line.Visible) continue;

                    if (line.Data?.Length > 1)
                    {
                        int countTmp = width_totle - 2 * leftRight + 1;
                        PointF[] points;
                        // 点数大于1的时候才绘制
                        if (line.Data.Length <= countTmp)
                        {
                            points = new PointF[line.Data.Length];
                            for (int i = 0; i < line.Data.Length; i++)
                            {
                                points[i].X = leftRight + i;
                                points[i].Y = BasicFramework.SoftPainting.ComputePaintLocationY(
                                    line.IsLeftFrame ? value_max_left : value_max_right,
                                    line.IsLeftFrame ? value_min_left : value_min_right,
                                    (heigh_totle - upDowm - upDowm), line.Data[i] ) + upDowm;
                            }
                        }
                        else
                        {
                            points = new PointF[countTmp];
                            for (int i = 0; i < points.Length; i++)
                            {
                                points[i].X = leftRight + i;
                                points[i].Y = BasicFramework.SoftPainting.ComputePaintLocationY(
                                    line.IsLeftFrame ? value_max_left : value_max_right,
                                    line.IsLeftFrame ? value_min_left : value_min_right,
                                    (heigh_totle - upDowm - upDowm), line.Data[i + line.Data.Length - countTmp] ) + upDowm;
                            }
                        }

                        using (Pen penTmp = new Pen( line.LineColor, line.LineThickness ))
                        {
                            g.DrawLines( penTmp, points );
                        }
                    }
                }
            }


        }


        private bool IsNeedPaintDash( float paintValue )
        {
            // 遍历所有的数据组
            for (int i = 0; i < auxiliary_lines.Count; i++)
            {
                if (Math.Abs( auxiliary_lines[i].PaintValue - paintValue ) < font_size9.Height)
                {
                    // 与辅助线冲突，不需要绘制
                    return false;
                }
            }

            // 需要绘制虚线
            return true;
        }

        private int CalculateDataCountByOffect(float offect)
        {
            if (offect > 40) return 1;
            offect = 40f / offect;
            return (int)Math.Ceiling( offect );
        }

        #endregion

        #region Size Changed

        private void UserCurve_SizeChanged( object sender, EventArgs e )
        {
            Invalidate( );
        }

        #endregion
    }


    /// <summary>
    /// 曲线数据对象
    /// </summary>
    internal class HslCurveItem
    {
        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public HslCurveItem( )
        {
            LineThickness = 1.0f;
            IsLeftFrame = true;
            Visible = true;
        }


        /// <summary>
        /// 数据
        /// </summary>
        public float[] Data = null;

        /// <summary>
        /// 线条的宽度
        /// </summary>
        public float LineThickness { get; set; }

        /// <summary>
        /// 曲线颜色
        /// </summary>
        public Color LineColor { get; set; }

        /// <summary>
        /// 是否左侧参考系，True为左侧，False为右侧
        /// </summary>
        public bool IsLeftFrame { get; set; }

        /// <summary>
        /// 本曲线是否显示出来，默认为显示
        /// </summary>
        public bool Visible { get; set; }
    }

    /// <summary>
    /// 辅助线对象
    /// </summary>
    internal class AuxiliaryLine : IDisposable
    {
        /// <summary>
        /// 实际的数据值
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// 实际的数据绘制
        /// </summary>
        public float PaintValue { get; set; }

        /// <summary>
        /// 辅助线的颜色
        /// </summary>
        public Color LineColor { get; set; }

        /// <summary>
        /// 辅助线的画笔资源
        /// </summary>
        public Pen PenDash { get; set; }

        /// <summary>
        /// 辅助线的宽度
        /// </summary>
        public float LineThickness { get; set; }

        /// <summary>
        /// 辅助线文本的画刷
        /// </summary>
        public Brush LineTextBrush { get; set; }

        /// <summary>
        /// 是否左侧参考系，True为左侧，False为右侧
        /// </summary>
        public bool IsLeftFrame { get; set; }

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose( bool disposing )
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    PenDash?.Dispose( );
                    LineTextBrush.Dispose( );
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~AuxiliaryLine() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。

        /// <summary>
        /// 释放内存信息
        /// </summary>
        public void Dispose( )
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose( true );
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
