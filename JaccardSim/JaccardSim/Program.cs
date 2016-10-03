using JaccardSim.Controllers;
using JaccardSim.Libs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebCrawler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 


        public static Porter2 Stemmer { get; set; }

        [STAThread]
        static void Main()
        {
            Program.Stemmer = new Porter2();
            StartForm();
        }
        
        static void StartForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new JaccardSim.MainForm());
        }

        static void Index()
        {
            IndexController.Index();
        }
    }
}
