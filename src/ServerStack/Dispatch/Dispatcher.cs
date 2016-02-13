using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServerStack.Serialization;

namespace ServerStack.Dispatch
{
    public class Dispatcher<T> : IObservable<Frame<T>>
    {
        private readonly IFrameDecoder<T> _decoder;
        private readonly List<IObserver<Frame<T>>> _observers = new List<IObserver<Frame<T>>>();
        private readonly ILogger<Dispatcher<T>> _logger;
        private readonly IOutputProducerFactory _producerFactory;

        public Dispatcher(ILogger<Dispatcher<T>> logger,
                          IFrameDecoder<T> decoder,
                          IOutputProducerFactory producerFactory,
                          IEnumerable<IObserver<Frame<T>>> observers)
        {
            _logger = logger;
            _decoder = decoder;
            _producerFactory = producerFactory;

            foreach (var observer in observers)
            {
                Subscribe(observer);
            }
        }

        public async Task Invoke(Stream stream)
        {
            var producer = _producerFactory.Create(stream);

            while (true)
            {
                try
                {
                    // Read some data, if the decoder doesn't consume any then stop
                    var frames = new List<T>();
                    await _decoder.Decode(stream, frames);

                    if (frames.Count == 0)
                    {
                        break;
                    }

                    lock (_observers)
                    {
                        foreach (var observer in _observers)
                        {
                            foreach (var frame in frames)
                            {
                                observer.OnNext(new Frame<T>(producer, frame));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lock (_observers)
                    {
                        foreach (var observer in _observers)
                        {
                            observer.OnError(ex);
                        }
                    }

                    _logger.LogError("Failed to process frame", ex);
                    break;
                }
            }

            (producer as IDisposable)?.Dispose();

            lock (_observers)
            {
                foreach (var observer in _observers)
                {
                    observer.OnCompleted();
                }
            }
        }

        public IDisposable Subscribe(IObserver<Frame<T>> observer)
        {
            lock (_observers)
            {
                _observers.Add(observer);
            }

            return new DisposableAction(() =>
            {
                lock (_observers)
                {
                    _observers.Remove(observer);
                }
            });
        }

        private class DisposableAction : IDisposable
        {
            private Action _action;
            public DisposableAction(Action action)
            {
                _action = action;
            }
            public void Dispose()
            {
                Interlocked.Exchange(ref _action, () => { }).Invoke();
            }
        }
    }
}
