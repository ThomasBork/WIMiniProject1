using JaccardSim.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCrawler;

namespace JaccardSim
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            txtSearchResult.Text = "Searching..." + Environment.NewLine;
            var result = SearchController.BooleanQuery(txtSearchQuery.Text);
            txtSearchResult.Text += "Done!" + Environment.NewLine;
            txtSearchResult.Text += result.Count + " results!" + Environment.NewLine;
            foreach (var doc in result)
            {
                txtSearchResult.Text += "URL: " + doc.Uri + Environment.NewLine;
            }
        }

        private void txtTFIDFSearch_Click(object sender, EventArgs e)
        {
            txtSearchResult.Text = "Searching..." + Environment.NewLine;
            var result = SearchController.TFIDFQuery(txtSearchQuery.Text);
            txtSearchResult.Text += "Done!" + Environment.NewLine;
            txtSearchResult.Text += result.Count + " results!" + Environment.NewLine;
            foreach (var doc in result)
            {
                txtSearchResult.Text += String.Format("{0:0.00}", doc.TempScore) + "URL: " + doc.Uri + Environment.NewLine;
            }
        }
    }
}
