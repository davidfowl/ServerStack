using System;
using System.Collections.Generic;
using ServerStack.Serialization;

namespace ServerStack.Dispatch
{
    public class DispatcherObservable<T> : IObservable<Frame<T>>, IDisposable
    {
        private readonly IDispatcher<T> _dispatcher;
        private readonly List<IDisposable> _topLevelSubscriptions = new List<IDisposable>();

        public DispatcherObservable(IDispatcher<T> dispatcher, IEnumerable<IObserver<Frame<T>>> observers)
        {
            _dispatcher = dispatcher;

            foreach (var observer in observers)
            {
                _topLevelSubscriptions.Add(Subscribe(observer));
            }
        }

        public IDisposable Subscribe(IObserver<Frame<T>> observer)
        {
            return _dispatcher.Subscribe(observer);
        }

        public void Dispose()
        {
            _topLevelSubscriptions.ForEach(s => s.Dispose());
            _topLevelSubscriptions.Clear();
        }
    }

}
