using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace HslCommunication.BasicFramework
{
    /*******************************************************************************
     * 
     *    Create Date:2017-05-03 16:56:39
     *    应该包含常用的绘图方法，资源，比如绘制直方图，居中的文本等
     * 
     * 
     ********************************************************************************/


    /// <summary>
    /// 图形的方向
    /// </summary>
    public enum GraphDirection
    {
        /// <summary>
        /// 向上
        /// </summary>
        Upward=1,
        /// <summary>
        /// 向下
        /// </summary>
        Downward=2,
        /// <summary>
        /// 向左
        /// </summary>
        Ledtward=3,
        /// <summary>
        /// 向右
        /// </summary>
        Rightward=4,

    }





    /// <summary>
    /// 包含整型和字符串描述的数据类型
    /// </summary>
    public struct Paintdata
    {
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }





    /// <summary>
    /// 图形的呈现方式
    /// </summary>
    public enum GraphicRender
    {
        /// <summary>
        /// 直方图
        /// </summary>
        Histogram = 1,
        /// <summary>
        /// 饼图
        /// </summary>
        Piechart,
        /// <summary>
        /// 折线图
        /// </summary>
        Linegraph,
    }
    /// <summary>
    /// 静态类，包含了几个常用的画图方法，获取字符串，绘制小三角等
    /// </summary>
    public static class SoftPainting
    {

        /********************************************************************************************************
         * 
         *     以下都是辅助绘图的静态方法
         * 
         * 
         * 
         * 
         ***********************************************************************************************************/

        /// <summary>
        /// 获取一个直方图
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="degree">刻度划分等级</param>
        /// <param name="lineColor">线条颜色</param>
        /// <returns></returns>
        public static Bitmap GetGraphicFromArray(int[] array, int width, int height, int degree, Color lineColor)
        {
            if (width < 10 && height < 10) throw new ArgumentException("长宽不能小于等于10");
            int Max = array.Max();
            int Min = 0;
            int Count = array.Length;

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            Pen dash = new Pen(Color.LightGray, 1f);
            dash.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            dash.DashPattern = new float[] { 5, 5 };

            Font font_8 = new Font("宋体", 9f);
            Bitmap bitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Color.White);
            //计算边框长度及图像边距
            int left = 60, right = 8, up = 8, down = 8;
            int paint_width = width - left - right;
            int paint_heigh = height - up - down;

            Rectangle rect = new Rectangle(left - 1, up - 1, paint_width + 1, paint_heigh + 1);
            //g.FillRectangle(Brushes.WhiteSmoke, rect);
            g.DrawLine(Pens.Gray, left - 1, up, left + paint_width + 1, up);
            g.DrawLine(Pens.Gray, left - 1, up + paint_heigh + 1, left + paint_width + 1, up + paint_heigh + 1);
            g.DrawLine(Pens.Gray, left - 1, up - 1, left - 1, up + paint_heigh + 1);

            //画刻度
            //for (int i = 0; i <= degree; i++)
            //{
            //    int value = (Max - Min) * i / degree + Min;
            //    int location = (int)ComputePaintLocationY(Max, Min, paint_heigh, value) + up + 1;
            //    g.DrawLine(Pens.DimGray, left - 1, location, left - 4, location);
            //    if (i != 0 && i < degree)
            //    {
            //        g.DrawLine(dash, left, location, width - right, location);
            //    }
            //    g.DrawString(value.ToString(), font_8, Brushes.DimGray, new Rectangle(-5, location - 4, left, 10), sf);
            //}

            PaintCoordinateDivide(g, Pens.DimGray, dash, font_8, Brushes.DimGray, sf, degree, Max, Min, width, height, left, right, up, down);


            PointF[] alldata = new PointF[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                alldata[i].X = paint_width * 1.0f / (array.Length - 1) * i + left;
                alldata[i].Y = ComputePaintLocationY(Max, Min, paint_heigh, array[i]) + up + 1;
            }

            Pen pen = new Pen(lineColor);
            g.DrawLines(pen, alldata);

            pen.Dispose();
            dash.Dispose();
            font_8.Dispose();
            sf.Dispose();
            g.Dispose();
            return bitmap;
        }







        /// <summary>
        /// 计算绘图时的相对偏移值
        /// </summary>
        /// <param name="max">0-100分的最大值，就是指准备绘制的最大值</param>
        /// <param name="min">0-100分的最小值，就是指准备绘制的最小值</param>
        /// <param name="height">实际绘图区域的高度</param>
        /// <param name="value">需要绘制数据的当前值</param>
        /// <returns>相对于0的位置，还需要增加上面的偏值</returns>
        public static float ComputePaintLocationY(int max, int min, int height, int value)
        {
            return height - (value - min) * 1.0f / (max - min) * height;
        }

        /// <summary>
        /// 计算绘图时的相对偏移值
        /// </summary>
        /// <param name="max">0-100分的最大值，就是指准备绘制的最大值</param>
        /// <param name="min">0-100分的最小值，就是指准备绘制的最小值</param>
        /// <param name="height">实际绘图区域的高度</param>
        /// <param name="value">需要绘制数据的当前值</param>
        /// <returns>相对于0的位置，还需要增加上面的偏值</returns>
        public static float ComputePaintLocationY( float max, float min, int height, float value )
        {
            return height - (value - min) / (max - min) * height;
        }


        /// <summary>
        /// 绘制坐标系中的刻度线
        /// </summary>
        /// <param name="g"></param>
        /// <param name="penLine"></param>
        /// <param name="penDash"></param>
        /// <param name="font"></param>
        /// <param name="brush"></param>
        /// <param name="sf"></param>
        /// <param name="degree"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="up"></param>
        /// <param name="down"></param>
        public static void PaintCoordinateDivide(
            Graphics g,       
            Pen penLine,
            Pen penDash,
            Font font,
            Brush brush,
            StringFormat sf,
            int degree,
            int max,
            int min,
            int width,
            int height,
            int left = 60,
            int right = 8, 
            int up = 8,
            int down = 8
            )
        {
            for (int i = 0; i <= degree; i++)
            {
                int value = (max - min) * i / degree + min;
                int location = (int)ComputePaintLocationY(max, min, (height - up - down), value) + up + 1;
                g.DrawLine(penLine, left - 1, location, left - 4, location);
                if (i != 0)
                {
                    g.DrawLine(penDash, left, location, width - right, location);
                }
                g.DrawString(value.ToString(), font, brush, new Rectangle(-5, location - font.Height / 2, left, font.Height), sf);
            }
        }

        /// <summary>
        /// 根据指定的方向绘制一个箭头
        /// </summary>
        /// <param name="g"></param>
        /// <param name="brush"></param>
        /// <param name="point"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        public static void PaintTriangle(Graphics g, Brush brush, Point point, int size, GraphDirection direction)
        {
            Point[] points = new Point[4];
            if (direction == GraphDirection.Ledtward)
            {
                points[0] = new Point(point.X, point.Y - size);
                points[1] = new Point(point.X, point.Y + size);
                points[2] = new Point(point.X - 2 * size, point.Y);
            }
            else if (direction == GraphDirection.Rightward)
            {
                points[0] = new Point(point.X, point.Y - size);
                points[1] = new Point(point.X, point.Y + size);
                points[2] = new Point(point.X + 2 * size, point.Y);
            }
            else if (direction == GraphDirection.Upward)
            {
                points[0] = new Point(point.X - size, point.Y);
                points[1] = new Point(point.X + size, point.Y);
                points[2] = new Point(point.X, point.Y - 2 * size);
            }
            else
            {
                points[0] = new Point(point.X - size, point.Y);
                points[1] = new Point(point.X + size, point.Y);
                points[2] = new Point(point.X, point.Y + 2 * size);
            }

            points[3] = points[0];
            g.FillPolygon(brush, points);
        }








        /********************************************************************************************************
         * 
         *     以下都是生成图形类的静态方法
         * 
         * 
         * 
         * 
         ***********************************************************************************************************/



        /// <summary>
        /// 根据数据生成一个可视化的图形
        /// </summary>
        /// <param name="array">数据集合</param>
        /// <param name="width">需要绘制图形的宽度</param>
        /// <param name="height">需要绘制图形的高度</param>
        /// <param name="graphic">指定绘制成什么样子的图形</param>
        /// <returns>返回一个bitmap对象</returns>
        public static Bitmap GetGraphicFromArray(Paintdata[] array, int width, int height, GraphicRender graphic)
        {
            if (width < 10 && height < 10) throw new ArgumentException("长宽不能小于等于10");
            array.Max(m => m.Count);
            Action<Paintdata[], GraphicRender, Graphics> paintAction =
                delegate (Paintdata[] array1, GraphicRender graphic1, Graphics g)
                {

                };
            return null;
        }

        
    }
}
