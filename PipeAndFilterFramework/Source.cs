using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using PipeAndFilterFramework;

namespace PipeAndFilterFramework
{
    public abstract class Source<TOut>
    {
        public Pipe<TOut> Output { get; set; } 
        public int MillisecondsTimeout { get; set; }

        protected void Write(TOut obj)
        {
            Output.Write(obj);
        }
        public Task Start(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => ProcessInner(cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void ProcessInner(CancellationToken cancellationToken)
        {
            try
            {
                Process(cancellationToken);
            }
            finally
            {
                Output?.MarkComplete();
            }
        }
        protected abstract void Process(CancellationToken cancellationToken);
    }
}