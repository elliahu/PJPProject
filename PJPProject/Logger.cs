using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJPProject
{
    public enum LogLevel
    {
        DEBUG,
        WARNING,
        ERROR,
        INFO, // always display info
    }
    public class Logger
    {
        public static LogLevel level = LogLevel.INFO;

        public static void Log(LogLevel level, string message)
        {
            if(level >= Logger.level)
            {
                Console.WriteLine(Enum.GetName(level) + ": " + message);
            }
        }
    }
}
