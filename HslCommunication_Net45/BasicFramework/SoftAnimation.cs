using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HslCommunication.BasicFramework
{
    /******************************************************************************
     * 
     *    系统的一些动画特效，颜色渐变，位置移动等等
     * 
     * 
     * 
     * 
     *******************************************************************************/




    /// <summary>
    /// 系统框架支持的一些常用的动画特效
    /// </summary>
    public class SoftAnimation
    {

        /// <summary>
        /// 最小的时间片段
        /// </summary>
        private static int TimeFragment { get; set; } = 20;

        /// <summary>
        /// 调整控件背景色，采用了线性的颜色插补方式，实现了控件的背景色渐变，需要指定控件，颜色，以及渐变的时间
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="color">设置的颜色</param>
        /// <param name="time">时间</param>
        public static void BeginBackcolorAnimation(Control control, Color color, int time)
        {
            if (control.BackColor != color)
            {
                Func<Control, Color> getcolor = m => m.BackColor;
                Action<Control, Color> setcolor = (m, n) => m.BackColor = n;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolColorAnimation),
                    new object[] { control, color, time, getcolor, setcolor });
            }
        }

        private static byte GetValue(byte Start, byte End, int i, int count)
        {
            if (Start == End) return Start;
            return (byte)((End - Start) * i / count + Start);
        }

        private static float GetValue(float Start, float End, int i, int count)
        {
            if (Start == End) return Start;
            return (End - Start) * i / count + Start;
        }

        private static void ThreadPoolColorAnimation(object obj)
        {
            object[] objs = obj as object[];
            Control control = objs[0] as Control;

            Color color = (Color)objs[1];
            int time = (int)objs[2];
            Func<Control, Color> getColor = (Func<Control, Color>)objs[3];
            Action<Control, Color> setcolor = (Action<Control, Color>)objs[4];
            int count = (time + TimeFragment - 1) / TimeFragment;
            Color color_old = getColor(control);

            try
            {
                for (int i = 0; i < count; i++)
                {
                    control.Invoke(new Action(() =>
                    {
                        setcolor(control, Color.FromArgb(
                            GetValue(color_old.R, color.R, i, count),
                            GetValue(color_old.G, color.G, i, count),
                            GetValue(color_old.B, color.B, i, count)));
                    }));
                    Thread.Sleep(TimeFragment);
                }
                control?.Invoke(new Action(() =>
                {
                    setcolor(control, color);
                }));
            }
            catch
            {

            }
        }

        private static void ThreadPoolFloatAnimation(object obj)
        {
            object[] objs = obj as object[];
            Control control = objs[0] as Control;

            lock (control)
            {
                float value = (float)objs[1];
                int time = (int)objs[2];
                Func<Control, float> getValue = (Func<Control, float>)objs[3];
                Action<Control, float> setValue = (Action<Control, float>)objs[4];
                int count = (time + TimeFragment - 1) / TimeFragment;
                float value_old = getValue(control);

                for (int i = 0; i < count; i++)
                {
                    if (control.IsHandleCreated && !control.IsDisposed)
                    {
                        control.Invoke(new Action(() =>
                        {
                            setValue(control, GetValue(value_old, value, i, count));
                        }));
                    }
                    else
                    {
                        return;
                    }
                    Thread.Sleep(TimeFragment);
                }
                if (control.IsHandleCreated && !control.IsDisposed)
                {
                    control.Invoke(new Action(() =>
                    {
                        setValue(control, value);
                    }));
                }
            }
        }
    }
}
