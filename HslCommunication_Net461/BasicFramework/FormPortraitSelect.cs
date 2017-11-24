using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HslCommunication.BasicFramework
{
    /// <summary>
    /// 一个正方形图形选择窗口，可以获取指定的分辨率
    /// </summary>
    public partial class FormPortraitSelect : Form
    {
        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public FormPortraitSelect()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 是否有图片存在
        /// </summary>
        private bool HasPicture { get; set; }
        /// <summary>
        /// 已选择的图形大小
        /// </summary>
        private Rectangle RectangleSelected { get; set; }
        private Rectangle RectangleMoved { get; set; }
        /// <summary>
        /// 在控件显示的图片的大小，按照比例缩放以后
        /// </summary>
        private Rectangle RectangleImage { get; set; }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            IsMouseOver = true;
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            IsMouseOver = false;
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseOver && HasPicture)
            {
                if (RectangleSelected.Contains(e.Location))
                {
                    Cursor = Cursors.SizeAll;
                    if (IsMouseDown)
                    {
                        MoveByPoint(e.Location);
                    }
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void MoveByPoint(Point point)
        {
            //点击并移动图形
            if (RectangleImage.Width >= RectangleImage.Height)
            {
                //只能左右移动
                int offect_x = point.X - MouseMovePrecives.X;
                int location_x = offect_x + RectangleSelected.X;
                if (location_x >= 0 && location_x <= RectangleImage.Width - RectangleSelected.Width)
                {
                    //移动成功
                    RectangleSelected = new Rectangle(location_x, RectangleSelected.Y, RectangleSelected.Width, RectangleSelected.Height);
                    pictureBox1.Refresh();
                }
            }
            else
            {
                //只能上下移动
                int offect_y = point.Y - MouseMovePrecives.Y;
                int location_y = offect_y + RectangleSelected.Y;
                if (location_y >= 0 && location_y <= RectangleImage.Height - RectangleSelected.Height)
                {
                    //移动成功
                    RectangleSelected = new Rectangle(RectangleSelected.X, location_y, RectangleSelected.Width, RectangleSelected.Height);
                    pictureBox1.Refresh();
                }
            }
            MouseMovePrecives = point;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsMouseOver && HasPicture)
            {
                if(RectangleSelected.Contains(e.Location))
                {
                    IsMouseDown = true;
                    MouseDownPoint = e.Location;
                    MouseMovePrecives = e.Location;
                }
            }
            pictureBox1.Focus();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (HasPicture)
            {
                if (RectangleSelected.Contains(e.Location))
                {
                    SetImgaeMiniShow();
                }
            }
            IsMouseDown = false;
            pictureBox1.Refresh();
        }

        private void SetImgaeMiniShow()
        {
            //区域还原
            Rectangle des = RectangleRestore();


            pictureBox2.Image?.Dispose();
            pictureBox2.Image = GetSpecicalSizeFromImage(100,des);
            
            pictureBox3.Image?.Dispose();
            pictureBox3.Image = GetSpecicalSizeFromImage(64, des);

            pictureBox4.Image?.Dispose();
            pictureBox4.Image = GetSpecicalSizeFromImage(32, des);
        }

        private Rectangle RectangleRestore()
        {
            int x = pictureBox1.Image.Width;
            int y = pictureBox1.Image.Height;
            if (x < y)
            {
                return new Rectangle(0, RectangleSelected.Y * y / 370, x, x);
            }
            else
            {
                return new Rectangle(RectangleSelected.X * x / 370, 0, y, y);
            }
        }
        private Bitmap GetSpecicalSizeFromImage(int size,Rectangle des)
        {
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g2 = Graphics.FromImage(bitmap))
            {
                g2.DrawImage(pictureBox1.Image, new Rectangle(0, 0, size, size), des, GraphicsUnit.Pixel);
            }
            return bitmap;
        }

        private void userButton1_Click(object sender, EventArgs e)
        {
            //文件选择
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.Multiselect = false;
                open.Title = "请选择一张图片";
                open.Filter = "图片文件(*.jpg)|*.jpg|图片文件(*.png)|*.png";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    //加载文件图片
                    LoadPictureFile(open.FileName);
                }
            }
        }
        /// <summary>
        /// 增加一张图片的路径
        /// </summary>
        /// <param name="picPath"></param>
        private void LoadPictureFile(string picPath)
        {
            Bitmap bitmap = null;
            try
            {
                bitmap = (Bitmap)Image.FromFile(picPath);
                int x = bitmap.Width;
                int y = bitmap.Height;
                if (x > y)
                {
                    y = y * 370 / x;
                    x = 370 - 1;
                }
                else
                {
                    x = x * 370 / y;
                    y = 370 - 1;
                }
                label1.Text = $"({x},{y})";
                RectangleImage = new Rectangle((370 - x) / 2, (370 - y) / 2, x, y);
                if (RectangleImage.Width >= RectangleImage.Height)
                {
                    RectangleSelected = new Rectangle(RectangleImage.X, RectangleImage.Y, RectangleImage.Height, RectangleImage.Height);
                }
                else
                {
                    RectangleSelected = new Rectangle(RectangleImage.X, RectangleImage.Y, RectangleImage.Width, RectangleImage.Width);
                }
                HasPicture = true;
                pictureBox1.Refresh();
            }
            catch(Exception ex)
            {
                SoftBasic.ShowExceptionMessage(ex);
                return;
            }

            pictureBox1.Image = bitmap;
            SetImgaeMiniShow();
        }

        private void FormPortraitSelect_Load(object sender, EventArgs e)
        {

        }

        private Brush brush = new SolidBrush(Color.FromArgb(120, Color.Gray));

        private bool IsMouseOver { get; set; }
        private bool IsMouseOverOnImage { get; set; }
        private bool IsMouseDown { get; set; }
        private Point MouseDownPoint { get; set; }
        private Point MouseMovePrecives { get; set; }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //最终呈现的选择区域
            if (HasPicture)
            {
                Graphics g = e.Graphics;
                
                g.FillRectangle(brush, RectangleSelected);
                g.DrawRectangle(Pens.LightSkyBlue, RectangleSelected);
            }
        }

        private void userButton2_Click(object sender, EventArgs e)
        {
            if (HasPicture)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.Cancel;
            }
        }


        private void pictureBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //if (HasPicture)
            //{
            //    if (e.KeyCode == Keys.Left)
            //    {
            //        Point point = new Point(MouseMovePrecives.X - 1, MouseMovePrecives.Y);
            //        MoveByPoint(point);
            //        SetImgaeMiniShow();
            //    }
            //    else if(e.KeyCode==Keys.Right)
            //    {
            //        Point point = new Point(MouseMovePrecives.X + 1, MouseMovePrecives.Y);
            //        MoveByPoint(point);
            //        SetImgaeMiniShow();
            //    }
            //    else if(e.KeyCode==Keys.Up)
            //    {
            //        Point point = new Point(MouseMovePrecives.X, MouseMovePrecives.Y - 1);
            //        MoveByPoint(point);
            //        SetImgaeMiniShow();
            //    }
            //    else if(e.KeyCode==Keys.Down)
            //    {
            //        Point point = new Point(MouseMovePrecives.X, MouseMovePrecives.Y + 1);
            //        MoveByPoint(point);
            //        SetImgaeMiniShow();
            //    }
            //    pictureBox1.Focus();
            //}
        }
        /// <summary>
        /// 获取指定大小的图片，该图片将会按照比例压缩
        /// </summary>
        /// <param name="size">图片的横向分辨率</param>
        /// <returns>缩放后的图形</returns>
        public Bitmap GetSpecifiedSizeImage(int size)
        {
            if(HasPicture)
            {
                return GetSpecicalSizeFromImage(size, RectangleRestore());
            }
            else
            {
                return null;
            }
        }
    }
}
