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
            Random random = new Random();
            HslCommunication.Controls.UserPieChart[] charts = new HslCommunication.Controls.UserPieChart[2];
            charts[0] = userPieChart1;
            charts[1] = userPieChart2;


            for (int j = 0; j < 2; j++)
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
