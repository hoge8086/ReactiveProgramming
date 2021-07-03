using System;

namespace ReactiveProgramming
{
    class Program
    {
        static void Main(string[] args)
        {

            //MyObserverPattern.Exec();
            //ObserverPatternUsingReactiveExtensions.Exec();
            //ObservableExtensions.Exec();

            //ColdHotDifference.Exec();
            // [TODO] Observableの配列からの変換関数
            // [TODO] Observableにもスケジューラの概念がある(ReactivePropertyだけでなく)、むしろこっちが先っぽい

            //INotifyChangedSample.Exec();
            //ObservableCollectionSample.Exec();

            //ReactivePropertySample.Exec();
            ReactivePropertySchedulerSample.Exec();//コンソールなので意味のあるサンプルになってない
            // バリデーション サンプル(WpfValidateionSample)
            // [TODO] コマンド
        }
    }
}
