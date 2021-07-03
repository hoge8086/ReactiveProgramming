using System;
using System.ComponentModel;
using System.Reactive.Subjects;

namespace ReactiveProgramming
{
    class INotifyChangedSample
    {

        //delegate void Func();
        static public void Exec()
        {
            // [全然関係ないがデリゲードについて]
            //Func func = null;
            //func = Exec;  [これもOK]
            //func += Exec; [こっちもOK]
            // https://stackoverflow.com/questions/40482822/delegates-are-immutable-but-how
            //delegate _del = delegate.Combine(del, method2);
            //del = (MyDel) _del;

            var model = new Person() { Name = "aaa", Age = 10};

            model.PropertyChanged += StringChanged;
            model.PropertyChanged += StringChanged;

            model.Name = "bbb";
            model.Age = 99;
            // [単にPropertyChanged(this, new PropertyChangedEventArgs(null))を呼び出しても、]
            // [e.PropertyNameがnullで呼ばれるだけ(特に何も処理が入ることはない(インターフェースなので))]
            // [すべてのプロパティの変更として受け止めるのは、購読側つまりUI側のおかげ]
            model.RaisePropertyChanged(null);

            model.PropertyChanged -= StringChanged;
            model.PropertyChanged -= StringChanged;
        }
        private static void StringChanged(object sender, PropertyChangedEventArgs e)
        {
            var person = sender as Person;

            Console.WriteLine("PropertyChanged -> " + (e.PropertyName ?? "null") + ":" + person.Name + "  (" + person.Age + ")");
        }
    }
}
