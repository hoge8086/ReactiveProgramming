using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProgramming
{
    // HotとColdは全く別物
    // HotなObservable:Observerが複数可能でSubscribeごとに同じストリーム(内部的にSubject<T>な実装に由来する)
    // ColdなObservable:Observerが一人のみでSubscribeごとに別ストリーム
    // すごい分かりやすいサイト
    //<https://qiita.com/yutorisan/items/844eaeab392abf03ce80>
    //<https://qiita.com/acple@github/items/8d3a4d3414fa59adff70>
    class ColdHotDifference
    {

        static public void Exec()
        {
            //[Cold]
            Cold_Range();

            // [HotなRange(自前)]
            Hot_MyRange();

            // [HotなRange(Publish)]
            Hot_RangeUsingPublish();

            // [動作しない例]
            NotDrive();
        }

        private static void Cold_Range()
        {
            // [Cold(Range)] -> [Subscribe]
            var coldRange = Observable.Range(0, 3);
            Console.WriteLine("[cold range]");
            // Subscribeごとに0～3が出力される(別ストリーム)]
            coldRange.Subscribe(x => Console.WriteLine("A:" + x));
            coldRange.Subscribe(x => Console.WriteLine("B:" + x));
        }
        private static void Hot_MyRange()
        {
            // [Hot(MyRange)] -> [Subscribe]
            Console.WriteLine("[my hot range]");
            var hotRange = new Subject<int>();
            Action start = () => { for (int i = 0; i < 3; i++) hotRange.OnNext(i); };
            // [A,Bは同時に出力される]
            hotRange.Subscribe(x => Console.WriteLine("A:" + x));
            hotRange.Subscribe(x => Console.WriteLine("B:" + x));
            start();
            hotRange.Subscribe(x => Console.WriteLine("C:" + x));   // [1～2は出力されない]
            hotRange.OnNext(3);
        }

        private static void Hot_RangeUsingPublish()
        {
            // [Cold(Range)] -> [Hot(Publish)] -> [Subscribe]
            Console.WriteLine("[hot range(publish)]");
            var connectableHot = Observable.Range(0, 3).Publish();
            //connectableHot.Connect();ここでConnect()してしまうと、このタイミングで0～3を購読してしまうため、以下のSubscribeで何も表示されない]
            connectableHot.Subscribe(x => Console.WriteLine("A:" + x));
            connectableHot.Subscribe(x => Console.WriteLine("B:" + x));
            connectableHot.Connect();
        }


        private static void NotDrive()
        {
            // [Cold(Range)] -> [Hot(Publish)] -> [Cold(Select)] -> [Subscribe]
            // ※Hot_RangeUsingPublish()とほとんど変わらない
            Console.WriteLine("[動作しない例]");
            // <https://qiita.com/toRisouP/items/f6088963037bfda658d3> :「Subscribeされるまで動作しない性質」に該当
            var connectableHot2 = Observable.Range(0, 3).Publish();
            var cold2 = connectableHot2.Select(x => "cold " + x.ToString());
            connectableHot2.Connect();  // [この時点でSubscribeしていないので動作しない]
            // [これ以降は値がOnNextされないので何も表示されない]
            cold2.Subscribe(x => Console.WriteLine("A:" + x));
            cold2.Subscribe(x => Console.WriteLine("B:" + x));
        }
    }
}
