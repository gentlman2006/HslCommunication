using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HslCommunicationDemo
{
    public partial class FormBasicControl : Form
    {
        public FormBasicControl( )
        {
            InitializeComponent( );
        }

        private void FormBasicControl_Load( object sender, EventArgs e )
        {
            random = new Random( );
            timerTick = new Timer( );
            timerTick.Interval = 1000;
            timerTick.Tick += TimerTick_Tick;
            timerTick.Start( );
        }

        private void TimerTick_Tick( object sender, EventArgs e )
        {
            if(userLantern3.LanternBackground == Color.Gray)
            {
                userLantern3.LanternBackground = Color.Tomato;
            }
            else
            {
                userLantern3.LanternBackground = Color.Gray;
            }

            int value = random.Next( 101 );
            userVerticalProgress7.Value = value;
            userVerticalProgress8.Value = value;


        }

        private Timer timerTick = null;
        private Random random;

        private void button1_Click( object sender, EventArgs e )
        {
            // 右下角弹窗，存在10s就关闭，时间小于0就是无穷大
            HslCommunication.BasicFramework.FormPopup popup = new HslCommunication.BasicFramework.FormPopup( "这是一条提示的消息！", Color.Blue, 5000 );
            popup.Show( );
        }
    }
}
