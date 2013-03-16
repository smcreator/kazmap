using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapRoutingDemo
{
    public partial class FormQuickView : Form
    {
        public FormQuickView(string coord)
        {
            InitializeComponent();
            try
            {
                var split = coord.Split(',');
                var lat = Convert.ToDouble(split[0]);
                var lon = Convert.ToDouble(split[1]);

                var mv = new MapDrawer(lon, lat, 19);
                mv.AppendMark(lon, lat);
                webBrowser.DocumentText = mv.ActiveHTML;
            }
            catch (Exception)
            {
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
