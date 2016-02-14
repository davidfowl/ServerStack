using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServerStack.Serialization;

namespace ServerStack.Dispatch
{
    public class Dispatcher<T> : IDispatcher<T>
    {
        private readonly List<IObserver<Frame<T>>> _observers = new List<IObserver<Frame<T>>>();
        private readonly IFrameDecoder<T> _decoder;
        private readonly ILogger<Dispatcher<T>> _logger;
        private readonly IOutputProducerFactory _producerFactory;

        public Dispatcher(ILogger<Dispatcher<T>> logger,
                          IFrameDecoder<T> decoder,
                          IOutputProducerFactory producerFactory)
        {
            _logger = logger;
            _decoder = decoder;
            _producerFactory = producerFactory;
        }

        public async Task Invoke(Stream stream)
        {
            var producer = _producerFactory.Create(stream);
            var frames = new List<T>();

            while (true)
            {
                try
                {
                    frames.Clear();
                    // Read some data, if the decoder doesn't consume any then stop
                    await _decoder.Decode(stream, frames);

                    if (frames.Count == 0)
                    {
                        break;
                    }

                    foreach (var frame in frames)
                    {
                        OnNext(producer, frame);
                    }
                }
                catch (Exception ex)
                {
                    OnError(ex);

                    _logger.LogError("Failed to process frame", ex);
                    break;
                }
            }

            OnCompleted();

            (producer as IDisposable)?.Dispose();
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

        private void OnCompleted()
        {
            lock (_observers)
            {
                foreach (var observer in _observers)
                {
                    observer.OnCompleted();
                }
            }
        }

        private void OnError(Exception exception)
        {
            lock (_observers)
            {
                foreach (var observer in _observers)
                {
                    observer.OnError(exception);
                }
            }
        }

        private void OnNext(IOutputProducer producer, T frame)
        {
            lock (_observers)
            {
                foreach (var observer in _observers)
                {
                    observer.OnNext(new Frame<T>(producer, frame));
                }
            }
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
