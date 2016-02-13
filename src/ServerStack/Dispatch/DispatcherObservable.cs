using System;
using System.Collections.Generic;
using System.Threading;
using ServerStack.Serialization;

namespace ServerStack.Dispatch
{
    public class DispatcherObservable<T> : IObservable<Frame<T>>, IDisposable
    {
        private readonly List<IObserver<Frame<T>>> _observers = new List<IObserver<Frame<T>>>();
        private readonly IDispatcher<T> _dispatcher;
        private readonly List<IDisposable> _topLevelSubscriptions = new List<IDisposable>();

        public DispatcherObservable(IDispatcher<T> dispatcher, IEnumerable<IObserver<Frame<T>>> observers)
        {
            _dispatcher = dispatcher;

            dispatcher.Callback += OnNext;
            dispatcher.OnError += OnError;
            dispatcher.OnCompleted += OnCompleted;

            foreach (var observer in observers)
            {
                _topLevelSubscriptions.Add(Subscribe(observer));
            }
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

        public void Dispose()
        {
            _dispatcher.Callback -= OnNext;
            _dispatcher.OnCompleted -= OnCompleted;
            _dispatcher.OnError -= OnError;

            _topLevelSubscriptions.ForEach(s => s.Dispose());
            _topLevelSubscriptions.Clear();
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
