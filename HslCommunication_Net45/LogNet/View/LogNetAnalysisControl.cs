using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace HslCommunication.LogNet
{
    /// <summary>
    /// 一个用于日志分析的控件
    /// </summary>
    public partial class LogNetAnalysisControl : UserControl
    {
        /// <summary>
        /// 实例化一个控件信息
        /// </summary>
        public LogNetAnalysisControl()
        {
            InitializeComponent();
        }

        private void LogNetAnalysisControl_Load(object sender, EventArgs e)
        {

        }


        private string m_LogSource = string.Empty;

        /// <summary>
        /// 设置日志的数据源
        /// </summary>
        /// <param name="logSource">直接从日志文件中读到的数据或是来自网络的数据</param>
        public void SetLogNetSource(string logSource)
        {
            m_LogSource = logSource;
            SetLogNetSourceView();
        }

        private void SetLogNetSourceView()
        {
            if (!string.IsNullOrEmpty(m_LogSource))
            {
                AnalysisLogSource(DateTime.MinValue, DateTime.MaxValue, StringResources.Language.LogNetAll);
                if (selectButton != null) selectButton.Selected = false;
                selectButton = userButton_All;
            }
        }

        /// <summary>
        /// 从现有的日志中筛选数据
        /// </summary>
        /// <param name="degree"></param>
        private void FilterLogSource(string degree)
        {
            if (!string.IsNullOrEmpty(m_LogSource))
            {
                if (!DateTime.TryParse(textBox2.Text, out DateTime start))
                {
                    MessageBox.Show("起始时间的格式不正确，请重新输入");
                    return;
                }
                if(!DateTime.TryParse(textBox3.Text,out DateTime end))
                {
                    MessageBox.Show("结束时间的格式不正确，请重新输入");
                    return;
                }

                AnalysisLogSource(start, end, degree);
            }
        }
        /// <summary>
        /// 底层的数据分析筛选
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="degree"></param>
        private void AnalysisLogSource(DateTime start,DateTime end,string degree)
        {
            if (!string.IsNullOrEmpty(m_LogSource))
            {
                StringBuilder sb = new StringBuilder();
                
                List<Match> collection = new List<Match>(Regex.Matches(m_LogSource, "\u0002\\[[^\u0002]+").OfType<Match>());
                int debug = 0;
                int info = 0;
                int warn = 0;
                int error = 0;
                int fatal = 0;
                int all = 0;

                List<DateTime> list = new List<DateTime>();


                for (int i = 0; i < collection.Count; i++)
                {
                    Match m = collection[i];
                    string deg = m.Value.Substring( 2, 5 );
                    DateTime dateTime = Convert.ToDateTime( m.Value.Substring( m.Value.IndexOf('2'), 19 ) );



                    if (start == DateTime.MinValue)
                    {
                        if (i == 0)
                        {
                            // 提取第一个时间
                            textBox2.Text = m.Value.Substring( m.Value.IndexOf( '2' ), 19 );
                        }

                        if (i == collection.Count - 1)
                        {
                            // 提取最后一个时间
                            textBox3.Text = m.Value.Substring( m.Value.IndexOf( '2' ), 19 );
                        }
                    }

                    if (start <= dateTime && dateTime <= end)
                    {

                        if (checkBox1.Checked)
                        {
                            // 正则表达式过滤
                            if (!Regex.IsMatch( m.Value, textBox4.Text ))
                            {
                                continue;
                            }
                        }

                        if (deg.StartsWith( StringResources.Language.LogNetDebug ))
                            debug++;
                        else if (deg.StartsWith( StringResources.Language.LogNetInfo ))
                            info++;
                        else if (deg.StartsWith( StringResources.Language.LogNetWarn ))
                            warn++;
                        else if (deg.StartsWith( StringResources.Language.LogNetError ))
                            error++;
                        else if (deg.StartsWith( StringResources.Language.LogNetFatal ))
                            fatal++;
                        all++;
                        if (degree == StringResources.Language.LogNetAll || deg.StartsWith(degree))
                        {
                            sb.Append( m.Value.Substring( 1 ) );
                            list.Add( dateTime );
                        }
                    }
                }


                userButton_Debug.UIText = $"{StringResources.Language.LogNetDebug} ({debug})";
                userButton_Info.UIText = $"{StringResources.Language.LogNetInfo} ({info})";
                userButton_Warn.UIText = $"{StringResources.Language.LogNetWarn} ({warn})";
                userButton_Error.UIText = $"{StringResources.Language.LogNetError} ({error})";
                userButton_Fatal.UIText = $"{StringResources.Language.LogNetFatal} ({fatal})";
                userButton_All.UIText = $"{StringResources.Language.LogNetAll} ({all})";

                textBox1.Text = sb.ToString();

                listPaint = list;

                // 设置图形显示
                if (pictureBox1.Width > 10)
                {
                    pictureBox1.Image = PaintData(pictureBox1.Width, pictureBox1.Height);
                }
            }
        }

        private HslCommunication.Controls.UserButton selectButton = null;

        private void UserButtonSetSelected(Controls.UserButton userButton)
        {
            if(!ReferenceEquals(selectButton,userButton))
            {
                if (selectButton != null) selectButton.Selected = false;
                userButton.Selected = true;
                selectButton = userButton;
            }
        }

        private void userButton_Debug_Click(object sender, EventArgs e)
        {
            // 调试
            UserButtonSetSelected(userButton_Debug);
            FilterLogSource( StringResources.Language.LogNetDebug );
        }

        private void userButton_Info_Click(object sender, EventArgs e)
        {
            // 信息
            UserButtonSetSelected(userButton_Info);
            FilterLogSource( StringResources.Language.LogNetInfo );
        }

        private void userButton_Warn_Click(object sender, EventArgs e)
        {
            // 警告
            UserButtonSetSelected(userButton_Warn);
            FilterLogSource( StringResources.Language.LogNetWarn );
        }

        private void userButton_Error_Click(object sender, EventArgs e)
        {
            // 错误
            UserButtonSetSelected(userButton_Error);
            FilterLogSource( StringResources.Language.LogNetError );
        }

        private void userButton_Fatal_Click(object sender, EventArgs e)
        {
            // 致命
            UserButtonSetSelected(userButton_Fatal);
            FilterLogSource( StringResources.Language.LogNetFatal );

        }

        private void userButton_All_Click(object sender, EventArgs e)
        {
            // 全部
            UserButtonSetSelected(userButton_All);
            FilterLogSource( StringResources.Language.LogNetAll );
        }

        private void userButton_source_Click(object sender, EventArgs e)
        {
            // 源日志
            SetLogNetSourceView();
        }




        #region 自动绘图块

        private List<DateTime> listPaint = new List<DateTime>();

        private List<PaintItem> listRender = new List<PaintItem>();

        private Bitmap PaintData(int width,int height)
        {
            if (width < 200) width = 200;
            if (height < 100) height = 100;

            Bitmap bitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmap);

            Font font12 = new Font("宋体", 12f);
            StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center,
            };
            Pen dash = new Pen(Color.LightGray, 1f);
            dash.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            dash.DashPattern = new float[] { 5, 5 };


            g.Clear(Color.White);

            if (listPaint.Count <= 5)
            {
                g.DrawString("数据太少了", font12, Brushes.DeepSkyBlue, new Rectangle(0,0,width, height), sf);
                goto P1;
            }


            int count = (width - 60) / 6;

            TimeSpan sp = listPaint.Max() - listPaint.Min();
            DateTime datetime_min= listPaint.Min();
            double sep = sp.TotalSeconds / count;

            int[] counts = new int[count];

            for (int i = 0; i < listPaint.Count; i++)
            {
                TimeSpan timeSpan = listPaint[i] - datetime_min;

                int index = (int)(timeSpan.TotalSeconds / sep);
                if (index < 0) index = 0;
                if (index == count) index--;
                counts[index]++;
            }

            int Max = counts.Max();
            int Min = 0;

            PaintItem[] list = new PaintItem[count];
            for (int i = 0; i < counts.Length; i++)
            {
                PaintItem item = new PaintItem();
                item.Count = counts[i];
                item.Start = listPaint[0].AddSeconds(i * sep);
                if (i == counts.Length - 1)
                {
                    item.End = listPaint[listPaint.Count - 1];
                }
                else
                {
                    item.End = listPaint[0].AddSeconds((i + 1) * sep);
                }

                list[i] = item;
            }

            listRender = new List<PaintItem>(list);


            // 左边空50，右边空10，上面20，下面30
            int left = 50;
            int right = 10;
            int up = 20;
            int down = 30;

            g.DrawLine(Pens.DimGray, left, up - 10, left, height - down);
            g.DrawLine(Pens.DimGray, left, height - down + 1, width - right, height - down + 1);


            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            // 绘制箭头
            BasicFramework.SoftPainting.PaintTriangle(g, Brushes.DimGray, new Point(left, up - 10), 5, BasicFramework.GraphDirection.Upward);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            int degree = 8;
            if (height < 500)
            {
                if (Max < 15 && Max > 1) degree = Max;
            }
            else if (height < 700)
            {
                if (Max < 25 && Max > 1) degree = Max;
                else
                {
                    degree = 16;
                }
            }
            else
            {
                if (Max < 40 && Max > 1) degree = Max;
                else degree = 24;
            }

            // 绘制刻度
            BasicFramework.SoftPainting.PaintCoordinateDivide(g, Pens.DimGray, dash, font12, Brushes.DimGray,
                sf, degree, Max, Min, width, height, left, right, up, down);

            sf.Alignment = StringAlignment.Center;
            // 绘制总数
            g.DrawString("Totle: " + listPaint.Count, font12, Brushes.DodgerBlue,
                new RectangleF(left, 0, width - left - right, up), sf);


            int paint_x = left + 2;

            for (int i = 0; i < list.Length; i++)
            {
                // 计算绘制位置
                float location = BasicFramework.SoftPainting.ComputePaintLocationY(Max, Min, height - up - down, list[i].Count) + up;
                RectangleF rectangleF = new RectangleF(paint_x, location, 5, height - down - location);
                if (rectangleF.Height <= 0 && list[i].Count > 0)
                {
                    rectangleF = new RectangleF(paint_x, height - down - 1, 5, 1);
                }
                g.FillRectangle(Brushes.Tomato, rectangleF);
                paint_x += 6;
            }

            g.DrawLine(Pens.DimGray, paint_x, up - 10, paint_x, height - down);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            BasicFramework.SoftPainting.PaintTriangle(g, Brushes.DimGray, new Point(paint_x, up - 10), 5, BasicFramework.GraphDirection.Upward);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            P1:
            sf.Dispose();
            font12.Dispose();
            dash.Dispose();
            g.Dispose();
            return bitmap;

        }
        




        #endregion








        private class PaintItem
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public int Count { get; set; }
        }

        private bool IsMouseEnter { get; set; }
        private PaintItem ClickSelected { get; set; }
        private Point pointMove { get; set; }

        private StringFormat stringFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (IsMouseEnter)
            {
                if (ClickSelected != null)
                {
                    if (pictureBox1.Width > 100)
                    {
                        string text = ClickSelected.Start.ToString("yyyy-MM-dd HH:mm:ss") + "  -  " +
                            ClickSelected.End.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + "Count:" +
                            ClickSelected.Count;
                        e.Graphics.DrawString(text, Font, Brushes.DimGray, new Rectangle(50, pictureBox1.Height - 27, pictureBox1.Width - 60, 30), stringFormat);
                        // 绘制位置提示线
                        e.Graphics.DrawLine(Pens.DeepPink, pointMove.X, 15, pointMove.X, pictureBox1.Height - 30);
                    }
                }
            }
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            IsMouseEnter = true;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            IsMouseEnter = false;
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseEnter)
            {
                if (e.Y > 20 &&
                    e.Y < pictureBox1.Height - 30 &&
                    e.X > 51 &&
                    e.X < pictureBox1.Width - 10)
                {
                    if ((e.X - 52) % 6 == 5) return;
                    int index = (e.X - 52) / 6;
                    if (index < listRender.Count)
                    {
                        pointMove = e.Location;
                        ClickSelected = listRender[index];
                        pictureBox1.Refresh();
                    }
                }
            }
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            // 设置图形显示
            if (pictureBox1.Width > 10)
            {
                pictureBox1.Image = PaintData(pictureBox1.Width, pictureBox1.Height);
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (IsMouseEnter)
            {
                if (pointMove.Y > 20 &&
                    pointMove.Y < pictureBox1.Height - 30 &&
                    pointMove.X > 51 &&
                    pointMove.X < pictureBox1.Width - 10)
                {
                    if (selectButton != null)
                    {
                        if ((ClickSelected.End - ClickSelected.Start).TotalSeconds > 3)
                        {
                            textBox2.Text = ClickSelected.Start.ToString("yyyy-MM-dd HH:mm:ss");
                            textBox3.Text = ClickSelected.End.ToString("yyyy-MM-dd HH:mm:ss");
                            AnalysisLogSource(ClickSelected.Start, ClickSelected.End, selectButton.UIText.Substring(0, 2));
                        }
                    }
                }
            }
        }
    }
}
