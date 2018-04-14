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
    public partial class FormGauge : Form
    {
        public FormGauge( )
        {
            InitializeComponent( );
        }

        private void FormGauge_Load( object sender, EventArgs e )
        {
            propertyGrid1.SelectedObject = userGaugeChart5;
            random = new Random( );
            timerTick = new Timer( );
            timerTick.Interval = 1000;
            timerTick.Tick += TimerTick_Tick;
            timerTick.Start( );
        }

        private void TimerTick_Tick( object sender, EventArgs e )
        {
            userGaugeChart1.Value = Math.Round( random.NextDouble( ) * 100, 1 );
            userGaugeChart2.Value = Math.Round( random.NextDouble( ) * 120, 1 );
            userGaugeChart3.Value = Math.Round( random.NextDouble( ) * 100, 1 );
            userGaugeChart4.Value = Math.Round( random.NextDouble( ) * 100, 1 );

        }

        private Timer timerTick = null;
        private Random random;
    }
}
