using System.Reflection;
using System.ServiceProcess;
using log4net;
using Quartz;
using Quartz.Impl;
using SMS_Service.Jobs;

namespace SMS_Service
{
    public partial class SMSService : ServiceBase
    {

        private readonly ILog logger;
        private IScheduler scheduler;

        public SMSService()
        {
            InitializeComponent();

            logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();

            #region 一个job

            IJobDetail job1 = JobBuilder.Create<SendSMSJob>() 
                .WithIdentity("定时发短信", "作业组")
                .Build();

            ITrigger trigger1 = TriggerBuilder.Create()
                .WithIdentity("触发器名称", "触发器组")
                .StartNow()
                .WithSimpleSchedule(x => x 
                    .WithIntervalInMinutes(14)
                    .RepeatForever())
                .Build();

            #endregion


            scheduler.ScheduleJob(job1, trigger1); 

        }

        protected override void OnStart(string[] args)
        {
            //scheduler.Start();
            logger.Info("OnStart");
        }

        protected override void OnStop()
        {
            scheduler.Shutdown();
            logger.Info("OnStop");
        }

        protected override void OnPause()
        {
            scheduler.PauseAll();
            logger.Info("OnPause");
        }

        protected override void OnContinue()
        {
            scheduler.ResumeAll();
            logger.Info("OnContinue");
        }

        protected override void OnShutdown()
        {
            scheduler.Shutdown();
            logger.Info("OnShutdown");
        }

    }
}
