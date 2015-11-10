using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PipeAndFilterFramework;

namespace ConsoleApplication
{
    public class SimpleSource : Source<int>
    {
        protected override void Process(CancellationToken cancellationToken)
        {
            for (int i = 0; !cancellationToken.IsCancellationRequested && i < 10; i++)
            {
                Console.WriteLine("SimpleSource");
                Write(i);
                Thread.Sleep(50);
            }
        }
    }

    public class IntToStringFilter : Filter<int, string>
    {
        public override string Process(int input)
        {
            Console.WriteLine("IntToStringFilter");
            return input.ToString();
        }
    }

    public class ConsoleSink : Sink<string>
    {
        public override void Process(string input)
        {
            Console.WriteLine("ConsoleSink");
            Console.WriteLine(input);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var source = new SimpleSource();
            var pipe1 = new Pipe<int>();
            var filter = new IntToStringFilter();
            var pipe2 = new Pipe<string>();
            var sink = new ConsoleSink();

            source.Output = pipe1;
            filter.Input = pipe1;

            filter.Output = pipe2;
            sink.Input = pipe2;

            var t1 = sink.Start(cts.Token);
            var t2 = source.Start(cts.Token);
            var t3 = filter.Start(cts.Token);

            Task.WaitAll(t1, t2, t3);
        }
    }
}
