using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace Common.Logging.Poc
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogServiceAsync<LogServiceOptions> logService = LogServiceAsync<LogServiceOptions>.Instance;

            List<int> x = Enumerable.Repeat(1, 10000000).ToList();
            int counter = 0;

            x.AsParallel().WithDegreeOfParallelism(100).ForAll(item =>
                {
                    logService.LogMessage(new 
                    {
                        messag = String.Format("Loop count: {0}", counter++)
                    });
                });
        }
    }
}