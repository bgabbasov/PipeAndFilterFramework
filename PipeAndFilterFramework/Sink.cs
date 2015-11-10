using System;
using System.Threading;
using System.Threading.Tasks;

namespace PipeAndFilterFramework
{
    public abstract class Sink<TIn>
    {
        protected Sink()
        {
            MillisecondsTimeout = 300;
        }
        public Pipe<TIn> Input { get; set; }
        public int MillisecondsTimeout { get; set; }

        public Task Start(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => ProcessInner(cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void ProcessInner(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) break;
                if (Input.IsCompleted) break;

                TIn input;
                if (Input.TryRead(out input, MillisecondsTimeout)) Process(input);
            }
        }
        public abstract void Process(TIn input);
    }
}