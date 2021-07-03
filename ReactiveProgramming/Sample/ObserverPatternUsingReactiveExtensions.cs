using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Subjects;

namespace ReactiveProgramming
{
    class ObserverPatternUsingReactiveExtensions
    {
        // [サンプル内容]
        //・ReactiveExtensionを使って、SubscribメソッドでIObserver<T>を自動生成
        //・Subjectクラスで、IObservableの実装が不要

        //参考:<https://blog.okazuki.jp/entry/20111101/1320156608>
        static public void Exec()
        {
            
            // SubjectはIObserver<T>とIObservable<T>の両方を実装したクラス
            var observable = new Subject<int>();

            // [ReactiveExtensionのSubscribe()メソッドでラムダ→IObserverが自動生成される]
            var a = observable.Subscribe(
                x => Console.WriteLine("A:購読:" + x),
                () => Console.WriteLine("A:完了:"));
            // [ReactiveExtensionのSubscribe()メソッドでラムダ→IObserverが自動生成される]
            var b  = observable.Subscribe(
                x => Console.WriteLine("B:購読:" + x),
                () => Console.WriteLine("B:完了:"));

            observable.OnNext(111);
            b.Dispose();    // [Disposeで購読解除]
            observable.OnNext(999);
            observable.OnCompleted();
        }
    }
}
