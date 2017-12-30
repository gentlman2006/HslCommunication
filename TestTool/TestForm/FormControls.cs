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
    public partial class FormControls : Form
    {
        public FormControls()
        {
            InitializeComponent();
        }


        private void userButton1_Click(object sender, EventArgs e)
        {

            Random random = new Random();
            HslCommunication.Controls.UserPieChart[] charts = new HslCommunication.Controls.UserPieChart[4];
            charts[0] = userPieChart1;
            charts[1] = userPieChart2;
            charts[2] = userPieChart3;
            charts[3] = userPieChart4;


            for (int j = 0; j < 4; j++)
            {


                List<string> data = new List<string>();
                List<int> ints = new List<int>();
                for (int i = 0; i < random.Next(4, 8); i++)
                {
                    data.Add(random.Next(100, 999).ToString());
                    ints.Add(random.Next(1, 5));
                }

                charts[j].SetDataSource(
                    data.ToArray(),
                    ints.ToArray());
            }



        }
    }
}
