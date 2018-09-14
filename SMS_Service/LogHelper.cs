using log4net;

namespace SMS_Service
{
    public static class Log4netHelper
    {
        //使用log4net日志接口实现日志记录
        public static  ILog Loger { get; set; }

        private static readonly object _o = new object();

        static Log4netHelper()
        {
            if(Loger==null)
            {
                lock(_o)
                {
                    if(Loger==null)
                    {
                        Loger= LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                    }
                }
            }
        }

     
    }
}
