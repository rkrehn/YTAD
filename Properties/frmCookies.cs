using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTPD.Properties
{
    public partial class frmCookies : Form
    {
        public frmCookies()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btn_Chrome_Click(object sender, EventArgs e)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "https://chrome.google.com/webstore/detail/get-cookiestxt-locally/cclelndahbckbenkjhflpdbgdldlbecc",
                UseShellExecute = true
            };

            try
            {
                Process.Start(processInfo);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_Firefox_Click(object sender, EventArgs e)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "https://addons.mozilla.org/en-US/firefox/addon/export-cookies-txt/?utm_source=addons.mozilla.org&utm_medium=referral&utm_content=search",
                UseShellExecute = true
            };

            try
            {
                Process.Start(processInfo);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
