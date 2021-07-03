using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProgramming
{
    // 参考:<https://blog.okazuki.jp/entry/2014/05/07/014133>
    class ReactivePropertySample
    {
        public static void Exec()
        {
            Sample1();
            Sample2();
            Sample_ObserveProperty();
            Sample_ReactiveCollection();
            Sample_ReactiveCollection2();

            ExpressionSample();
        }
        private static void StringChanged(object sender, PropertyChangedEventArgs e)
        {
            var rp = sender as ReactiveProperty<string>;
            // [★ReactivePropertyのPropertyNameは"Value"固定]
            Console.WriteLine("PropertyChanged -> " + e.PropertyName + ":" + rp.Value);
        }

        private static void Sample1()
        {
            Console.WriteLine("[Sample1]");
            var rp = new ReactiveProperty<string>();
            // [(1) ★ReactivePropertyはIObservableを実装しているので、Subscribeが使える]
            // [    ※Subscribe直後に"null"が通知される]
            rp.Subscribe(x => Console.WriteLine("Subscribe ->" + (x ?? "null")));
            // [(2) ★ReactivePropertyはINotifyPropertyChangedを実装しているので、PropertyChangedも使える]
            // [    ※こちらは初期値"null"が通知されない]
            rp.PropertyChanged += StringChanged;

            rp.Value = "aa";
            rp.Value = "B";
            rp.PropertyChanged -= StringChanged;
            Console.WriteLine("");
        }
        private static void Sample2()
        {
            Console.WriteLine("[Sample2]");
            var rp = new ReactiveProperty<string>("a");
            // [Subscribe直後に"a"が通知される]
            rp.Subscribe(x => Console.WriteLine(x ?? "null"));
            rp.PropertyChanged += StringChanged;
            rp.Value = "aa";
            rp.Value = "B";
            rp.PropertyChanged -= StringChanged;
            Console.WriteLine("");
        }
        private static void Sample_ObserveProperty()
        {
            // INotifyPropertyChangedを実装したModelクラスを
            // ReactivePropertyを使ったViewModelのプロパティに変換するのによく使うらしい

            Console.WriteLine("[Sample_ObserveProperty]");
            var person = new Person() { Name = "aaa" }; // Modelクラス
            // INotifyPropertyChanged(Person) -> IObservable<string> -> ReactiveProperty<string>に変換
            var rp = person.ObserveProperty(x => x.Name).ToReactiveProperty();
            rp.Subscribe(x => Console.WriteLine("名前変更:" + x));

            person.Name = "ハヤカワ";
            person.Name = "山田";
            Console.WriteLine("");
        }
        private static void Sample_ReactiveCollection()
        {
            // ReactiveCollectionはObservableCollectionを継承している
            Console.WriteLine("[Sample_ReactiveCollection]");
            var list = new ReactiveCollection<string>();
            // [なぜかObservableCollectionのPropertyChangedは公開されていないが、CountプロパティとItems[]プロパティの変更が通知される]
            ((INotifyPropertyChanged)list).PropertyChanged += PropertyChangedEventHandler;
            list.CollectionChanged += Notify;

            // リストの各操作を監視するIObserverを生成できる
            list.ObserveAddChanged( ).Subscribe(x => Console.WriteLine("ObserveAddChanged:" + x));
            list.ObserveRemoveChanged( ).Subscribe(x => Console.WriteLine("ObserveRemoveChanged:" + x));
            list.ObserveResetChanged( ).Subscribe(x => Console.WriteLine("ObserveResetChanged:"));

            Console.WriteLine("");
            list.Add("aaa");
            Console.WriteLine("");
            list.Add("bbb");
            Console.WriteLine("");
            list.RemoveAt(0);
            Console.WriteLine("");
            list.Clear();
            Console.WriteLine("");
        }
        private static void Sample_ReactiveCollection2()
        {
            Console.WriteLine("[Sample_ReactiveCollection2]");
            var observable = new MyObservable<string>();
            // [IObservableからReactiveCollectionを生成できる]
            // [IObservableのOnNext()で発行された値が、自動的にReactiveCollectionに追加される]
            var list = observable.ToReactiveCollection();

            list.ObserveAddChanged( ).Subscribe(x => Console.WriteLine("ObserveAddChanged:" + x));
            list.ObserveRemoveChanged( ).Subscribe(x => Console.WriteLine("ObserveRemoveChanged:" + x));
            list.ObserveResetChanged( ).Subscribe(x => Console.WriteLine("ObserveResetChanged:"));
            observable.Kick("aaa");
            observable.Kick("bbb");
            observable.Kick("ccc");
            Console.WriteLine("");
        }
        private static void ExpressionSample()
        {
            // [関係ないがObserveProperty()で、Observableに変換できる理由]
            // ObserveProperty()で渡すExpression(式)からデリゲードのプロパティ名を取得できるので
            // PropertyChangedでそのプロパティ名の変更を購読したIObservable<T>を生成できる


            // Expressionはデリゲードから生成できるが、デリゲードではなく式を表す
            // Expressionは、実行時に式内のプロパティを取得できる
            System.Linq.Expressions.Expression<Func<Person, int>> exp = x => x.Age;

            var member = (System.Linq.Expressions.MemberExpression)exp.Body;
            var propertyInfo = (System.Reflection.PropertyInfo)member.Member;
            var name = propertyInfo.Name;   // "Age"が取得できる

        }

        static void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("PropertyChanged:" + e.PropertyName);
        }

        static void Notify(object sender, NotifyCollectionChangedEventArgs e)
        {
            var oldItems = e.OldItems != null ? string.Join(',', e.OldItems.Cast<object>().Select(x => x.ToString())) : "null";
            var newItems = e.NewItems != null ? string.Join(',', e.NewItems.Cast<object>().Select(x => x.ToString())) : "null";
            Console.WriteLine("{0},old=[{1}],new=[{2}]", e.Action.ToString(), oldItems, newItems);
        }
    }
}
