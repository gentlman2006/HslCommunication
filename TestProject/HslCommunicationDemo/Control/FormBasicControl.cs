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
    }
}
