using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Logging
{
    public enum TraceLevel
    {
        NONE,
        TRACE,
        WARNING,
        ERROR
    }

    public static class Logs
    {
        private static Dictionary<string, Trace> _streamsets = new Dictionary<string, Trace>();
        private static string DefaultTrace = null;

        public static void RegisterNewTrace(string traceID, string logroot, string logPrefix, bool bfileTime = true, bool bTimeStamp = true)
        {
            if (_streamsets.ContainsKey(traceID))
            {
                throw new ArgumentException($"{traceID} has already been registered");
            }
            _streamsets[traceID] = new Trace(traceID, logroot, logPrefix, bfileTime, bTimeStamp);

            if (DefaultTrace == null)
            {
                DefaultTrace = traceID;
            }
        }

        public static void SetDefaultTrace(string traceID)
        {
            if (!_streamsets.ContainsKey(traceID))
            {
                throw new ArgumentException($"{traceID} is not registered");
            }
            DefaultTrace = traceID;
        }

        public static void LogWarning(string message, string traceid = null)
        {
            WriteLine(message, traceid, TraceLevel.WARNING);
        }

        public static void LogError(Exception ex, string traceid = null, bool showStack = false)
        {
            LogError(ex.Message, traceid);

            if (showStack)
            {
                LogError(ex.StackTrace, traceid);
            }
        }

        public static void LogError(string message, string traceid = null)
        {
            WriteLine(message, traceid, TraceLevel.ERROR);
        }

        public static void LogTrace(string message, string traceid = null)
        {
            WriteLine(message, traceid, TraceLevel.TRACE);
        }

        public static void WriteLine(string message, string traceid = null, TraceLevel level = TraceLevel.NONE)
        {

            var id = traceid ?? DefaultTrace;

            if (id == null)
            {
                Console.WriteLine(message);
                return;
            }

            if (!_streamsets.ContainsKey(id))
            {
                throw new ArgumentException($"{id} is not registered");
            }
            _streamsets[id].WriteLine(message, level);
        }

        public static void CustomDispose()
        {
            foreach (var trace in _streamsets)
            {
                if (trace.Value.TraceStream != null)
                {
                    trace.Value.TraceStream.Dispose();
                }
            }
        }

        private class Trace : IDisposable
        {
            public StreamWriter TraceStream { get; private set; }
            static int logIncrement = 2;

            public bool UseTimeStamp { get; private set; }

            public Trace(string traceID, string logroot, string logPrefix, bool bfileTime, bool bTimeStamp)
            {
                bool flag = false;
                if (string.IsNullOrWhiteSpace(traceID))
                {
                    throw new ArgumentException("Failed to initialize new trace instance, trace ID not provided");
                }
                if (string.IsNullOrWhiteSpace(logPrefix))
                {
                    throw new ArgumentException("Failed to initialize new trace instance, logPrefix not provided");
                }

                if (!string.IsNullOrWhiteSpace(logroot))
                {
                    if (!Directory.Exists(logroot))
                    {
                        Directory.CreateDirectory(logroot);
                    }
                }

                var timestamp = bfileTime ? $"_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}" : "";

                var logName = Path.Combine(logroot ?? "", $"{logPrefix.Trim()}{timestamp}.log");

                
                try
                {
                    var tmpFile = File.Open(logName,FileMode.CreateNew);
                    tmpFile.Close();
                    TraceStream = new StreamWriter(logName);
                    Console.WriteLine($"Increment:{logIncrement}");
                }
                catch (IOException)
                {
                    logName = Path.Combine(logroot ?? "", $"{logPrefix.Trim()}_{logIncrement}_{timestamp}.log");
                    logIncrement++;
                    TraceStream = new StreamWriter(logName);
                    Console.WriteLine($"Increment:{logIncrement}");
                }

                flag = true;
                UseTimeStamp = bTimeStamp;
            }

            public void WriteLine(string message, TraceLevel lvl = TraceLevel.TRACE)
            {
                var timestamp = UseTimeStamp ? DateTime.Now.ToString("u") + " " : "";

                var lvlStr = lvl == TraceLevel.NONE ? "" : $"[{lvl.ToString()}]:";

                lock (TraceStream)
                {
                    TraceStream.WriteLine($"{timestamp}{lvlStr}{message}");
                }
            }

            public void Dispose()
            {
                if (TraceStream != null)
                {
                    TraceStream.Dispose();
                }
            }
        }
    }
}
