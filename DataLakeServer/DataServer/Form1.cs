﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataServer
{
    public partial class Form1 : Form
    {
        CoreService sr;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sr= new CoreService();
            sr.Init();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sr != null)
            {
                sr.Close();
            }
        }

    }
}
