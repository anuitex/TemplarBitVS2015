using System;
using System.Collections.Generic;
using System.IO;
using TemplarBit.Core;

namespace TemplarBit.UnitTests
{
    public class TestLogger : ITemplarBitLogger
    {
        public List<string> Logs { get; set; } = new List<string>();
        public void Log(string exception)
        {
            string path = Directory.GetCurrentDirectory();

            Logs.Add(String.Format("\nTemplarBitMiddlewareError: {0}\n", exception));
        }
    }
}
