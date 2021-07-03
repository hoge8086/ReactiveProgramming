using Reactive.Bindings;
using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Threading;

namespace ReactiveProgramming
{
    class ReactivePropertySchedulerSample
    {
        public static void Exec()
        {
            Console.WriteLine("デフォルトスケジューラ:{0}", ReactivePropertyScheduler.Default.ToString());
            Sample_ImmediateScheduler();
            Sample_CurrentThreadScheduler();
            //Sample_SynchronizationContext();
        }

        //private static void Sample_SynchronizationContext()
        //{
        //    Console.WriteLine("[Sample_SynchronizationContext]");
        //    // ★コンソールだとSynchronizationContext.Currentがnullで死ぬ
        //    ReactivePropertyScheduler.SetDefault(new SynchronizationContextScheduler(SynchronizationContext.Current));
        //    Console.WriteLine("スケジューラ:" + ReactivePropertyScheduler.Default.ToString());
        //    SchedulerTest();
        //}
        private static void Sample_ImmediateScheduler()
        {
            Console.WriteLine("[Sample_ImmediateScheduler]");
            ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
            Console.WriteLine("スケジューラ:{0}", ReactivePropertyScheduler.Default.ToString());
            SchedulerTest();
        }
        private static void Sample_CurrentThreadScheduler()
        {
            Console.WriteLine("[Sample_CurrentThreadScheduler]");
            ReactivePropertyScheduler.SetDefault(CurrentThreadScheduler.Instance);
            Console.WriteLine("スケジューラ:{0}", ReactivePropertyScheduler.Default.ToString());
            SchedulerTest();
        }
        private static void SchedulerTest()
        {
            var rp = new Reactive.Bindings.ReactiveProperty<string>();
            var latestA = "A";
            var latestB = "B";

            rp.Subscribe(x => latestA = x);
            rp.PropertyChanged += (s,e) => latestB = ((ReactiveProperty<string>)s).Value;
            rp.Value = "C";
            Console.WriteLine("{0} {1}", latestA, latestB);
            rp.Value = "D";
            Console.WriteLine("{0} {1}", latestA, latestB);
            Console.WriteLine("");
        }
    }
}
