using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ServerStack.Serialization;

namespace ServerStack.Dispatch
{
    public class ObservableFrameHandler<TFrame> : IFrameHandler<TFrame>, IObservable<TFrame>
    {
        private readonly List<IObserver<TFrame>> _observers = new List<IObserver<TFrame>>();

        public Task OnFrame(Stream output, TFrame value)
        {
            lock (_observers)
            {
                _observers.ForEach(o => o.OnNext(value));
            }
            return Task.FromResult(0);

        }

        public IDisposable Subscribe(IObserver<TFrame> observer)
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
