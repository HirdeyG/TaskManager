using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optinuity.TaskManager.DataObjects
{
    public class LoggerBase
    {
        private static log4net.ILog logger;

        /// <summary>
        /// Logger
        /// </summary>
        public static log4net.ILog Logger
        {
            get
            {
                if (logger == null)
                {
                    log4net.Config.XmlConfigurator.Configure();
                    log4net.Config.BasicConfigurator.Configure();
                    logger = log4net.LogManager.GetLogger("TaskManager");
                }
                return logger;
            }
        }

    }
}
