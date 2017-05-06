using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace iphlpapidemo
{
    public partial class frmPricePerMeg : Form
    {
        public frmPricePerMeg()
        {
            InitializeComponent();
        }

        private double _DownloadingPricePerMegabyte = 0;
        public double DownloadingPricePerMegabyte
        {
            get { return _DownloadingPricePerMegabyte; }
            set { tbDownloadingPrice.Text = value.ToString(); }
        }

        private double _UploadingPricePerMegabyte = 0;
        public double UploadingPricePerMegabyte
        {
            get { return _UploadingPricePerMegabyte; }
            set { tbUploadingPrice.Text = value.ToString(); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _DownloadingPricePerMegabyte = double.Parse(tbDownloadingPrice.Text);
            _UploadingPricePerMegabyte = double.Parse(tbUploadingPrice.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}