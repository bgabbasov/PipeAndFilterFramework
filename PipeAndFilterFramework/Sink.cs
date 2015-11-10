using System.Threading;
using System.Threading.Tasks;

namespace PipeAndFilterFramework
{
    public abstract class Sink<TIn>
    {
        private readonly Pipe<TIn> _input;
        private readonly int _millisecondsTimeout;

        public Sink(Pipe<TIn> input, int millisecondsTimeout = 300)
        {
            _input = input;
            _millisecondsTimeout = millisecondsTimeout;
        }

        public void Start(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() => ProcessInner(cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void ProcessInner(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();
                if (_input.IsCompleted) break;

                TIn input;
                if (_input.TryRead(out input, _millisecondsTimeout, cancellationToken)) Process(input);
            }
        }
        public abstract void Process(TIn input);
    }
}