using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UpdateReport.Properties;

namespace UpdateReport
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        CultureInfo info = null;
        static Timer tm = new Timer();

        public static Timer tm2 = new Timer();
        public static bool IsLastExecPreviousDate = false;

        protected override void OnStart(string[] args)
        {
            info = System.Threading.Thread.CurrentThread.CurrentCulture;
            IsLastExecPreviousDate = (DateTime.Parse(Settings.Default.DataUltimaAtualizacao.ToString(info.DateTimeFormat.ShortDatePattern)) < DateTime.Parse(DateTime.Now.ToString(info.DateTimeFormat.ShortDatePattern)));

            tm.Elapsed += new ElapsedEventHandler(OnElapsedTimeAtualiza);
            tm.Interval = Settings.Default.Intervalo;
            tm.Enabled = true;

           
           if (Settings.Default.AtrasaUltimaAtualizacao)
            {
                Settings.Default.DataUltimaAtualizacao = DateTime.Now.AddDays(-1);
                Settings.Default.Save();
            }
        }


        private void OnElapsedTimeAtualiza(object source, ElapsedEventArgs e)
        {
            try
            {
                TimeSpan ts = TimeSpan.Parse(Settings.Default.Horario);
                DateTime dtexec = DateTime.Today.Add(ts);

                bool IsExecute = ((dtexec.Day == DateTime.Now.Day && dtexec.Month == DateTime.Now.Month && dtexec.Year == DateTime.Now.Year &&
                                   dtexec.Hour == DateTime.Now.Hour && dtexec.Minute == DateTime.Now.Minute) &&
                                   Convert.ToDateTime(Settings.Default.DataUltimaAtualizacao.ToString(info.DateTimeFormat.ShortDatePattern)) < Convert.ToDateTime(dtexec.ToString(info.DateTimeFormat.ShortDatePattern))) ||
                                   IsLastExecPreviousDate
                                  ;

                if (IsExecute)
                {
                    tm.Stop();
                    WriteToEventLog("Emite relatório");

                  
                }
            }
            catch (Exception ex)
            {
                WriteToEventLog("Erro " + ex.Message);
            }

        }

        private void WriteToEventLog(string message)
        {
            string cs = "Service1";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

        protected override void OnStop()
        {
        }
    }
}
