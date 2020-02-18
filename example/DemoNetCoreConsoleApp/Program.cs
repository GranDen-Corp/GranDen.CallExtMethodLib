using System;
using GranDen.CallExtMethodLib;
using Iso8601ExtMethodLib;

namespace DemoNetCoreConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Call Extension method normally ===");

            var utcNow = DateTime.UtcNow;
            var iso8601Str = utcNow.ToIso8601String(true);

            Console.WriteLine($"UTC Now = {{{utcNow}}}\r\nISO8601 : {{{iso8601Str}}}");

            Console.WriteLine("\r\n=== Call Extension method using CallExtMethodLib ===");

            var helper = new ExtMethodInvoker("Iso8601ExtMethodLib");
            var result = helper.Invoke<DateTime>("FromIso8601String", iso8601Str);

            if(result is DateTime converted)
            {
                Console.WriteLine($"Result is {{{converted}}}");
            }
        }
    }
}
