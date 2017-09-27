using System;
using Xunit.Abstractions;

namespace Nest
{
    class ConsoleLogger : ITestOutputHelper
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(string.Format(format, args));
        }
    }
}
