using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Logging
{
    public enum GameLogLevel
    {
        Trace,
        Debug,
        Warning,
        Error,
        Fatal
    }

    public interface IGameLogger
    {
        void Log(GameLogLevel level, string message, params object[] args);

        void Trace(string message, params object[] args);
        void Debug(string message, params object[] args);
        void Warning(string message, params object[] args);
        void Error(string message, params object[] args);
        void Fatal(string message, params object[] args);
    }
}
