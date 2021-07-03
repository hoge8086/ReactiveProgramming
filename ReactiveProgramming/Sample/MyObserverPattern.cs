using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProgramming
{
    //IObserver<T>とIObservable<T>を自前で実装

    //参考:<https://blog.okazuki.jp/entry/20111101/1320156608>
    class MyObserverPattern
    {
        static public void Exec()
        {
            var observerA = new MyObserverImpl<string>(
                (x) => Console.WriteLine("A:購読:" + x),
                () => Console.WriteLine("A:購読終了"),
                () => Console.WriteLine("A:エラー"));
            var observerB = new MyObserverImpl<string>(
                (x) => Console.WriteLine("B:購読:" + x),
                () => Console.WriteLine("B:購読終了"),
                () => Console.WriteLine("B:エラー"));

            var observable = new MyObservable<string>();
            var unsubscriberA = observable.Subscribe(observerA);
            var unsubscriberB = observable.Subscribe(observerB);
            observable.Kick("ああああ");
            unsubscriberA.Dispose();    // [Disposeを呼ぶと購読解除]
            observable.Error(new Exception());
            observable.Kick("いいいいいいいいい");
            observable.End();
        }
    }

    class MyObserverImpl<T> : IObserver<T>
    {
        private Action<T> _onNext;
        private Action _onCompleted;
        private Action _onError;
        public MyObserverImpl(Action<T> onNext, Action onCompleted, Action onError)
        {
            _onNext = onNext;
            _onCompleted = onCompleted;
            _onError = onError;
        }
        public void OnCompleted()
        {
            _onCompleted();
        }

        public void OnError(Exception error)
        {
            _onError();
        }

        public void OnNext(T value)
        {
            _onNext(value);
        }
    }
    class MyObservable<T> : IObservable<T>
    {
        List<IObserver<T>> observers = new List<IObserver<T>>();

        // -------------[★結局ここがIObserverと同じI/Fになってる]---------
        public void Kick(T val)
        {
            foreach (var observer in observers)
                observer.OnNext(val);
        }
        public void End()
        {
            foreach (var observer in observers)
                observer.OnCompleted();
        }
        public void Error(Exception exception)
        {
            foreach (var observer in observers)
                observer.OnError(exception);
        }
        //----------------------------------------------------------------------------

        public IDisposable Subscribe(IObserver<T> observer)
        {
            observers.Add(observer);
            // [購読を解除するためのインスタンスを返す]
            return new Unsubscriber<T>(observers, observer);

        }
    }

    // [Disposeしたら購読を解除するためのクラス]
    class Unsubscriber<T> : IDisposable
    {
        private List<IObserver<T>> _observers;
        private IObserver<T> _observer;
        public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observers == null)
                return;
            _observers.Remove(_observer);
            _observers = null;
            _observer = null;
        }
    }
}
