using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using PipeAndFilterFramework;

namespace PipeAndFilterFramework
{
    public abstract class Source<TOut>
    {
        private readonly Pipe<TOut> _output;
        private readonly int _millisecondsTimeout;

        public Source(Pipe<TOut> output, int millisecondsTimeout = 300)
        {
            _output = output;
            _millisecondsTimeout = millisecondsTimeout;
        }

        protected void Write(TOut obj)
        {
            _output.Write(obj);
        }
        public void Start(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() => Process(cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        protected abstract void Process(CancellationToken cancellationToken);
    }
}