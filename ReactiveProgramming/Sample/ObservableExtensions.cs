using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProgramming
{
    // ObseavableからObservableを生成する拡張メソッドのサンプル
    // 参考<https://blog.okazuki.jp/entry/20111110/1320849106>
    class ObservableExtensions
    {

        static public void Exec()
        {
            var obserbable = new MyObservable<int>();
            // WhereでフィルタするObseavableを生成
            var obserbable2 = obserbable.Where(x => (x % 2) == 0);
            // Selectで発行する値を変換したObseavableを生成
            var obserbable3 = obserbable.Select(x => "文字列:" + x.ToString());

            obserbable.Subscribe(x => Console.WriteLine("A:" + x));
            obserbable2.Subscribe(x => Console.WriteLine("B:" + x));
            obserbable3.Subscribe(x => Console.WriteLine("C:" + x));

            for (int i = 0; i < 10; i++)
                obserbable.Kick(i);

        }
    }
}
