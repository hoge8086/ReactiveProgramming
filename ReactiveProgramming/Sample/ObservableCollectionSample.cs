using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProgramming
{
    class ObservableCollectionSample
    {
        public static void Exec()
        {
            StringCollectionSample();
            Console.WriteLine("");
            PersonCollectionSample();
        }

        private static void StringCollectionSample()
        {
            Console.WriteLine("[StringCollectionSample]");
            var list = new ObservableCollection<string>();
            // [なぜかObservableCollectionのPropertyChangedは公開されていないが、CountプロパティとItems[]プロパティの変更が通知される]
            ((INotifyPropertyChanged)list).PropertyChanged += PropertyChangedEventHandler;
            list.CollectionChanged += Notify;
            list.Add("aaa");
            list.Add("bbb");
            list.RemoveAt(0);
            list.Clear();
        }

        private static void PersonCollectionSample()
        {
            Console.WriteLine("[PersonCollectionSample]");
            var list = new ObservableCollection<Person>();
            // [なぜかObservableCollectionのPropertyChangedは公開されていないが、Countプロパティの変更が通知される]
            ((INotifyPropertyChanged)list).PropertyChanged += PropertyChangedEventHandler;
            list.CollectionChanged += Notify;
            var personA = new Person() { Age = 20, Name = "やまだ" };
            var personB = new Person() { Age = 99, Name = "田中" };
            list.Add(personA);
            // [個々の要素の変更は、ObservableCollection自体には通知されない]
            personA.Name = "さとう";
            personA.Age = 22;
            list.Add(personB);
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
