using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace TestTool.TestForm
{
    public partial class FormJsonTest : Form
    {
        public FormJsonTest()
        {
            InitializeComponent();
        }

        private void userButton1_Click(object sender, EventArgs e)
        {
            try
            {
                JObject jObject = JObject.Parse("{\"UserName\":\"46346\",\"Password\":\"46346\",\"LoginWay\":\"Andriod\",\"DeviceUniqueID\":\"Missing\",\"FrameworkVersion\":\"1.7.5\"}");
                textBox2.Text = "成功";
            }
            catch(Exception ex)
            {
                textBox2.Text = HslCommunication.BasicFramework.SoftBasic.GetExceptionMessage(ex);
            }
        }
    }
}
