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
    public partial class FormControlCollection : Form
    {
        public FormControlCollection()
        {
            InitializeComponent();
        }

        private void FormControlCollection_Load(object sender, EventArgs e)
        {
            userSwitch1.SwitchStatusDescription = new string[] { "测试关", "测试开" };

            Random random = new Random();
            HslCommunication.Controls.UserPieChart[] charts = new HslCommunication.Controls.UserPieChart[2];
            charts[0] = userPieChart1;
            charts[1] = userPieChart2;


            for (int j = 0; j < 2; j++)
            {


                List<string> data = new List<string>();
                List<int> ints = new List<int>();
                for (int i = 0; i < random.Next(4, 12); i++)
                {
                    data.Add(random.Next(100, 999).ToString());
                    ints.Add(random.Next(0, 5));
                }

                charts[j].SetDataSource(
                    data.ToArray(),
                    ints.ToArray());
            }
        }

        private void userButton1_Click(object sender, EventArgs e)
        {
            if (userPieChart1.IsRenderSmall)
            {
                userPieChart1.IsRenderSmall = false;
                userPieChart2.IsRenderSmall = false;
            }
            else
            {
                userPieChart1.IsRenderSmall = true;
                userPieChart2.IsRenderSmall = true;
            }


            Random random = new Random( );

            float[] data = new float[300];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = random.Next( 201 );
            }

            userCurve1.SetLeftCurve( "A", data, Color.DodgerBlue );


            userCurve2.SetLeftCurve( "A", new float[] { }, Color.Tomato );            // 温度1
            userCurve2.SetLeftCurve( "B", new float[] { }, Color.DodgerBlue );        // 温度2
            userCurve2.SetRightCurve( "C", new float[] { }, Color.LimeGreen );         // 压力1
            userCurve2.SetRightCurve( "D", new float[] { }, Color.Orchid );            // 压力2

            Timer timer = new Timer( );
            timer.Interval = 100;
            timer.Tick += ( sender1, e1 ) =>
            {
                userCurve2.AddCurveData(
                    new string[] { "A", "B", "C", "D" },
                    new float[] { random.Next( 160, 181 ), random.Next( 150, 171 ), (float)random.NextDouble( ) * 2.5f + 1, (float)random.NextDouble( ) * 1f } );
            };
            timer.Start( );
        }
    }
}
