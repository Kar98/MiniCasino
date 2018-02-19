using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Logging
{
    public abstract class InternalLog
    {
        protected bool outputToConsole = false;
        protected void Log(string log)
        {
            if (outputToConsole)
                Console.Write(log);
        }

        protected void Log(int log)
        {
            if (outputToConsole)
                Console.Write(log);
        }

        protected void Log(double log)
        {
            if (outputToConsole)
                Console.Write(log);
        }

        protected void LogLine(string logWithLineBreak)
        {
            if (outputToConsole)
                Console.WriteLine(logWithLineBreak);
        }

        protected void LogLine(int logWithLineBreak)
        {
            if (outputToConsole)
                Console.WriteLine(logWithLineBreak);
        }

        protected void LogLine(double logWithLineBreak)
        {
            if (outputToConsole)
                Console.WriteLine(logWithLineBreak);
        }

    }
}
