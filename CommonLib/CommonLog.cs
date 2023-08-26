using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
   public class LOG
    {
        public static readonly NLog.Logger LogWriter = NLog.LogManager.GetCurrentClassLogger();
    }
}
