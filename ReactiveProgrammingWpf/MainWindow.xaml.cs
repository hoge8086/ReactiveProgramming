using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReactiveProgrammingWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // <https://qiita.com/okazuki/items/7572f46848d0e93516b1>
            // <https://blog.xin9le.net/entry/2012/01/24/120722>
            // <https://blog.okazuki.jp/entry/20120304/1329923070>
            // ・ReactivePropertyの簡易実装(内部的なつくり)
            // <http://neue.cc/2018/01/18_562.html>
            // <https://nosimok.hateblo.jp/entry/2017/02/05/021107>
            InitializeComponent();
            Debug.WriteLine("スケジューラ:" + ReactivePropertyScheduler.Default.ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReactivePropertyScheduler.SetDefault(new SynchronizationContextScheduler(SynchronizationContext.Current));
            Debug.WriteLine("スケジューラ:" + ReactivePropertyScheduler.Default.ToString());
            SchedulerTest();

            // [結果]
            // UI スレッドでイベントを発行するためにスレッドの切り替えが行われるので 
            // Value が変化しても直後に PropertyChanged イベントは起きない(latestBはふるいまま)
            //Subscribe=C, PropertyChanged=B
            //Subscribe=D, PropertyChanged=B
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
            Debug.WriteLine("スケジューラ:" + ReactivePropertyScheduler.Default.ToString());
            SchedulerTest();

            // [結果]
            // 同一スレッド上で反映される
            //Subscribe=C, PropertyChanged=C
            //Subscribe=D, PropertyChanged=D

        }
        private static void SchedulerTest()
        {
            var rp = new Reactive.Bindings.ReactiveProperty<string>();
            var latestA = "A";
            var latestB = "B";

            rp.Subscribe(x => latestA = x);
            rp.PropertyChanged += (s,e) => latestB = ((ReactiveProperty<string>)s).Value;
            rp.Value = "C";
            Debug.WriteLine("Subscribe={0}, PropertyChanged={1}", latestA, latestB);
            rp.Value = "D";
            Debug.WriteLine("Subscribe={0}, PropertyChanged={1}", latestA, latestB);
            Debug.WriteLine("");
        }

    }
}
