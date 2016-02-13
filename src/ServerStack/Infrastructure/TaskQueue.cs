using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServerStack.Infrastructure
{
    // Allows serial queuing of Task instances
    // The tasks are not called on the current synchronization context

    internal sealed class TaskQueue
    {
        private readonly object _lockObj = new object();
        private Task _lastQueuedTask;
        private volatile bool _drained;
        private readonly int? _maxSize;
        private long _size;

        public TaskQueue(int maxSize)
        {
            _lastQueuedTask = Task.FromResult(0);
            _maxSize = maxSize;
        }

        public bool IsDrained
        {
            get
            {
                return _drained;
            }
        }

        public Task Enqueue(Func<object, Task> taskFunc, object state)
        {
            // Lock the object for as short amount of time as possible
            lock (_lockObj)
            {
                if (_drained)
                {
                    return _lastQueuedTask;
                }

                if (_maxSize != null)
                {
                    // Increment the size if the queue
                    if (Interlocked.Increment(ref _size) > _maxSize)
                    {
                        Interlocked.Decrement(ref _size);

                        // We failed to enqueue because the size limit was reached
                        return null;
                    }
                }

                var newTask = _lastQueuedTask.ContinueWith(async task =>
                {
                    try
                    {
                        await task;
                        await taskFunc(state);
                    }
                    finally
                    {
                        Dequeue();
                    }
                });

                _lastQueuedTask = newTask;
                return newTask;
            }
        }

        private void Dequeue()
        {
            if (_maxSize != null)
            {
                // Decrement the number of items left in the queue
                Interlocked.Decrement(ref _size);
            }
        }

        public Task Enqueue(Func<Task> taskFunc)
        {
            return Enqueue(state => ((Func<Task>)state).Invoke(), taskFunc);
        }

        public Task Drain()
        {
            lock (_lockObj)
            {
                _drained = true;

                return _lastQueuedTask;
            }
        }
    }
}
