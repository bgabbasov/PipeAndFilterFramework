using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipeAndFilterFramework
{
    public class Pipe<T>
    {
        private readonly BlockingCollection<T> _queue = new BlockingCollection<T>(new ConcurrentQueue<T>());

        public bool IsCompleted => _queue.IsCompleted;

        public void MarkComplete()
        {
            _queue.CompleteAdding();
        }

        public void Write(T obj)
        {
            _queue.TryAdd(obj);
        }
        public void Write(T obj, TimeSpan timeout)
        {
            _queue.TryAdd(obj, timeout);
        }
        public void Write(T obj, int millisecondsTimeout)
        {
            _queue.TryAdd(obj, millisecondsTimeout);
        }
        public void Write(T obj, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            _queue.TryAdd(obj, millisecondsTimeout, cancellationToken);
        }

        public bool TryRead(out T obj)
        {
            return _queue.TryTake(out obj);
        }
        public bool TryRead(out T obj, TimeSpan timeout)
        {
            return _queue.TryTake(out obj, timeout);
        }
        public bool TryRead(out T obj, int millisecondsTimeout)
        {
            return _queue.TryTake(out obj, millisecondsTimeout);
        }
        public bool TryRead(out T obj, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return _queue.TryTake(out obj, millisecondsTimeout, cancellationToken);
        }
    }
}
