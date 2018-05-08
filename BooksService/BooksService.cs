using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BooksService
{
    public partial class BooksService : ServiceBase
    {
        private WCFService m_Service = new WCFService();
        public BooksService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_Service.Start();
        }

        protected override void OnStop()
        {
            m_Service.Stop();
        }
    }
}
