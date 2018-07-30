using System;
using System.Collections.Generic;
using System.Text;

namespace NETCoreSeed.Context.Provider
{
    public class TraceLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new TraceLogger(categoryName);

        public void Dispose() { }
    }
}