using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTool.TestForm
{
    public partial class FormCurve : Form
    {
        public FormCurve( )
        {
            InitializeComponent( );
        }

        private Random random = new Random( );

        private void userButton1_Click( object sender, EventArgs e )
        {
            float[] data = new float[300];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = random.Next( 201 );
            }

            userCurve1.SetLeftCurve( "A", data, Color.DodgerBlue );
        }


        private void userButton3_Click( object sender, EventArgs e )
        {
            // 假设你的data数组已经更新了
            float[] data = new float[300];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = random.Next( 100, 201 );
            }
            // 之前已经给A指定过颜色了，以后后续的数据更新不需要重新指定，指定了也无效
            // 如果需要重新设置颜色，或是线宽，需要先RemoveCurve，然后重新创建曲线信息
            userCurve1.SetLeftCurve( "A", data );
        }

        private void Test()
        {
            float[] data = new float[200];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = random.Next( 101 );
            }

            float[] data2 = new float[200];
            for (int i = 0; i < data.Length; i++)
            {
                data2[i] = (float)random.NextDouble( ) * 5;
            }

            userCurve1.SetLeftCurve( "A", new float[0], Color.DodgerBlue, 300 );
            userCurve1.SetRightCurve( "B", new float[0], Color.Tomato, 300 );
        }

        private void userButton2_Click( object sender, EventArgs e )
        {
            Timer timer = new Timer( );
            timer.Interval = 100;
            timer.Tick += ( sender1, e1 ) =>
            {
                // userCurve1.AddCurveData( "A", random.Next( 101 ) );
                userCurve1.AddCurveData( new string[] { "A", "B" }, new float[] { random.Next( 101 ), (float)random.NextDouble( ) * 3 } );
            };
            timer.Start( );
        }

        private void userButton4_Click( object sender, EventArgs e )
        {
            userCurve1.SetLeftCurve( "A", new float[] { }, Color.Tomato );            // 温度1
            userCurve1.SetLeftCurve( "B", new float[] { }, Color.DodgerBlue );        // 温度2
            userCurve1.SetRightCurve( "C", new float[] { }, Color.LimeGreen );         // 压力1
            userCurve1.SetRightCurve( "D", new float[] { }, Color.Orchid );            // 压力2
        }

        private void userButton5_Click( object sender, EventArgs e )
        {
            Timer timer = new Timer( );
            timer.Interval = 100;
            timer.Tick += ( sender1, e1 ) =>
            {
                userCurve1.AddCurveData(
                    new string[] { "A", "B", "C", "D" },
                    new float[] { random.Next( 160, 181 ), random.Next( 150, 171 ), (float)random.NextDouble( ) * 2.5f + 1, (float)random.NextDouble( ) * 1f } );
            };
            timer.Start( );
        }

        private void userButton6_Click( object sender, EventArgs e )
        {
            // 辅助线新增
            if(float.TryParse(textBox1.Text,out float value))
            {
                userCurve1.AddLeftAuxiliary( value ,Color.Chocolate);
            }
        }

        private void userButton7_Click( object sender, EventArgs e )
        {
            // 辅助线移除
            if (float.TryParse( textBox1.Text, out float value ))
            {
                userCurve1.RemoveAuxiliary( value );
            }
        }

        private void userButton8_Click( object sender, EventArgs e )
        {
            // 右辅助新增
            if (float.TryParse( textBox1.Text, out float value ))
            {
                userCurve1.AddRightAuxiliary( value , Color.Yellow );
            }
        }
    }
}
