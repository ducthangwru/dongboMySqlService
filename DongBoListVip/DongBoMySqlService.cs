using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DongBoListVip
{
    partial class DongBoMySqlService : ServiceBase
    {

        private static Log_Sytems logs = new Log_Sytems();
        bool stopping_GetData = false;
        private static SqlDataHelper db = new SqlDataHelper();
        ManualResetEvent stoppedEvent;
        public DongBoMySqlService()
        {
            InitializeComponent();

            this.stopping_GetData = false;

            this.stoppedEvent = new ManualResetEvent(false);
        }

        private void ThreadGetData(object state)
        {
            // Periodically check if the service is stopping.
            string err = "";


            while (!this.stopping_GetData)
            {
                // Perform main service function here...
                DataTable dt = NhanVienVIP.DanhSachNhanVienVIPSql();
                foreach(DataRow dr in dt.Rows)
                {
                    NhanVienVIP.InsertorUpdateNhanVienVIPMySql(dr["PhoneNumber"].ToString(), dr["Name"].ToString(), dr["Queue"].ToString());
                }

                NhanVienVIP.DeleteNhanVienVIPMySql();
               
                // string thoigian = ConfigurationManager.AppSettings["sophut"].ToString();
                //int value_time = 0;

                logs.ErrorLog("Dong bo luc :" + DateTime.Now.AddMinutes(double.Parse(ConfigurationManager.AppSettings["TANSUAT"])), "");
                Thread.Sleep((int)(60000 * double.Parse(ConfigurationManager.AppSettings["TANSUAT"])));  // Simulate some lengthy operations.
            }
            // Signal the stopped event.
            this.stoppedEvent.Set();
        }

        protected override void OnStart(string[] args)
        {
            logs.ErrorLog("DongBoPhieuService OnStart: " + DateTime.Now, null);


            // Log a service start message to the Application log. 
            // Queue the main service function for execution in a worker thread. 
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadGetData));
        }

        protected override void OnStop()
        {
            logs.ErrorLog("DongBoMySqlService OnStop: " + DateTime.Now, null);
            // Log a service stop message to the Application log. 

            // Indicate that the service is stopping and wait for the finish  
            // of the main service function (ThreadGetData). 
            this.stopping_GetData = true;
            this.stoppedEvent.WaitOne();
        }

        protected override void OnPause()
        {
            logs.ErrorLog("DongBoMySqlService OnPause: " + DateTime.Now, null);

            base.OnPause();
            // timmer.Stop();
            this.stoppedEvent.WaitOne();
        }
        protected override void OnShutdown()
        {
            logs.ErrorLog("DongBoMySqlService OnShutdown: " + DateTime.Now, null);
            base.OnShutdown();
            //timmer.Stop();
            this.stoppedEvent.WaitOne();
        }
    }
}
