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
        private readonly Pipe<TIn> _input;
        private readonly Pipe<TOut> _output;
        private readonly int _millisecondsTimeout;

        public Filter(Pipe<TIn> input, Pipe<TOut> output, int millisecondsTimeout = 300)
        {
            _input = input;
            _output = output;
            _millisecondsTimeout = millisecondsTimeout;
        }

        public void Start(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() => ProcessInner(cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void ProcessInner(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();
                    if (_input.IsCompleted) break;

                    TIn input;
                    if (_input.TryRead(out input, _millisecondsTimeout, cancellationToken))
                    {
                        var output = Process(input);
                        _output.Write(output);
                    }
                }
            }
            finally
            {
                _output.MarkComplete();
            }
        }
        public abstract TOut Process(TIn input);
    }
}
