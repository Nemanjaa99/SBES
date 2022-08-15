using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApp
{
    public static class Logger
    {
        public static void Log(string message, EventLogEntryType logEntryType)
        {
            string cs = "ComplaintSvc";
            if (!EventLog.SourceExists(cs))
                EventLog.CreateEventSource(cs, "Application");

            EventLog.WriteEntry(cs, message, logEntryType);
        }
    }
}
