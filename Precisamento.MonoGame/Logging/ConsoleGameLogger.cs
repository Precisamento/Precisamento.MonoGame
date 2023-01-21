using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Precisamento.MonoGame.Logging
{
    public class ConsoleGameLogger : IGameLogger
    {
        public void Debug(string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(message, args);
        }

        public void Error(string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(message, args);
        }

        public void Log(GameLogLevel level, string message, params object[] args)
        {
            switch(level)
            {
                case GameLogLevel.Debug:
                    Debug(message, args);
                    break;
                case GameLogLevel.Error:
                    Error(message, args);
                    break;
                case GameLogLevel.Fatal:
                    Fatal(message, args);
                    break;
                case GameLogLevel.Trace:
                    Trace(message, args);
                    break;
                case GameLogLevel.Warning:
                    Warning(message, args);
                    break;
            }
        }

        public void Trace(string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(message, args);
        }

        public void Warning(string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(message, args);
        }
    }
}
