using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipeAndFilterFramework
{
    public abstract class Filter<TIn, TOut>
    {
        protected Filter()
        {
            MillisecondsTimeout = 300;
        }

        public Pipe<TIn> Input { get; set; }
        public Pipe<TOut> Output { get; set; }
        public int MillisecondsTimeout { get; set; }

        public Task Start(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => ProcessInner(cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void ProcessInner(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    if (Input.IsCompleted) break;

                    TIn input;
                    if (Input.TryRead(out input, MillisecondsTimeout))
                    {
                        var output = Process(input);
                        Output?.Write(output);
                    }
                }
            }
            finally
            {
                Output?.MarkComplete();
            }
        }
        public abstract TOut Process(TIn input);
    }
}
