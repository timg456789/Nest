using System.Collections.Generic;
using Xunit.Abstractions;

namespace NestTests
{
    class ListLogger : ITestOutputHelper
    {
        public List<string> log = new List<string>();
        
        public void WriteLine(string message)
        {
            log.Add(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            log.Add(string.Format(format, args));
        }
    }
}
