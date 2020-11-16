﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HslCommunicationDemo
{
    public partial class FormImage : Form
    {
        public FormImage( Bitmap bitmap )
        {
            InitializeComponent( );
            this.bitmap = bitmap;
        }

        private Bitmap bitmap;

        private void FormImage_Load( object sender, EventArgs e )
        {
            pictureBox1.Image = bitmap;
        }
    }
}
