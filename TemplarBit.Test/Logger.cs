using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplarBit.Core;

namespace TemplarBit.Test
{
    public class Logger : ITemplarBitLogger
    {
        public void Log(string exception)
        {
            string path = Directory.GetCurrentDirectory();

            using (StreamWriter sw = File.AppendText(path + "/LogFile.txt"))
            {
                string logLine = String.Format("\nTemplarBitMiddlewareError: {0}\n", exception);
                sw.WriteLine(logLine);
            }
        }
    }
}
