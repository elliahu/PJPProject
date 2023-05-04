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
        public static LogLevel level = LogLevel.DEBUG;

        public static void Log(LogLevel level, string message, ConsoleColor color = ConsoleColor.White )
        {
            if(level >= Logger.level)
            {
                var c = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(Enum.GetName(level) + ": " + message);
            }
        }
    }
}
