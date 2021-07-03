using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace WpfValidationSample
{
    // 参考<https://sourcechord.hatenablog.com/entry/2014/05/11/202656>
    //     <https://sourcechord.hatenablog.com/entry/2014/06/08/123738>
    class MainWindowViewModel : BindableBase, INotifyDataErrorInfo
    {
        // (0)何もしない
        private int _sample0;
        public int Sample0
        {
            get => _sample0;
            set
            {
                SetProperty(ref this._sample0, value);
            }

        }
        // (1)--------------- ValidatesOnExceptionsサンプル ---------------
        private int _sample1;
        public int Sample1
        {
            get => _sample1;
            set
            {
                if (value > 100)
                {
                    // xaml上で"ValidatesOnExceptions=True"とすると赤枠になる
                    throw new Exception("100より大きい(setter内で例外を投げる)");
                }

                // 例外ケースは値が入らない
                SetProperty(ref this._sample1, value);
            }

        }

        // (2)--------------- ValidationRuleサンプル ---------------
        // プロパティは特に何もすることはない
        // ValidationRuleクラスを作成しxaml上で指定する
        private int _sample2;
        public int Sample2
        {
            get => _sample2;
            set => SetProperty(ref this._sample2, value);
        }

        // (番外)IDataErrorInfoもあるが、古い方法なので省略

        //(3) --------------- INotifyDataErrorInfoを使う   -------------------------------
        // イメージはINotifyPropertyChangedのエラー版と思えばよい
        private int _sample3;
        public int Sample3
        {
            get => _sample3;
            set
            {
                SetProperty(ref this._sample3, value);
                if (_sample3 > 100)
                {
                    HasErrors = true;
                    if (ErrorsChanged != null)
                        ErrorsChanged(this, new DataErrorsChangedEventArgs(nameof(Sample3)));
                }
                else
                {
                    HasErrors = false;
                }
            }
        }
        // 本来はすべてのプロパティにエラー対して実施するがここでは、分かりやすさを重視してSample3のみに適応する
        #region INotifyDataErrorInfoの実装 
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors { get; private set; }

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName == nameof(Sample3))
                return new List<string>() { "100より大きい(INotifyDataErrorInfo)" };

            return null;
        }
        #endregion

        //(4) --------------- ReactivePropertyを使う   -------------------------------
        // <https://qiita.com/okazuki/items/7572f46848d0e93516b1#%E5%9F%BA%E6%9C%AC%E6%A9%9F%E8%83%BD>
        // ・入力値のバリデーション (Slim には無い機能)
        // ★一番簡単そう
        public ReactiveProperty<int> Sample4 { get; }

        public MainWindowViewModel()
        {
            Sample4 = new ReactiveProperty<int>().SetValidateNotifyError(x => x <= 100 ? null : "100より大きい(ReactiveProperty)");
        }


        //(5) --------------- ValidationAttributeを使う   -------------------------------
        // INotifyDataErrorInfoを実装したうえで,
        // SetterでValidateProperty()メソッドを呼ぶ。
        // ValidateProperty()内でValidatorクラスでアノテーションで指定した検証を行う
        // この仕組み自体を作れば、後は属性を指定するだけなので簡単。
        // 仕組み作りが結構面倒
        // <https://sourcechord.hatenablog.com/entry/2014/06/08/193510>
        // <https://sourcechord.hatenablog.com/entry/2015/02/15/005411>
        // <https://qiita.com/Koki_jp/items/ae00c525728b81f302b8>
        // サンプルはなし

    }

    // (2)ValidationRuleサンプル
    public class MaxIntRule : ValidationRule
    {
        public int Max { get; set; } = 100;

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int v;
            if (!int.TryParse(value.ToString(), out v))  
                return new ValidationResult(false, "intに変換できない(ValidationRule)");  

            if (v > Max)
                return new ValidationResult(false, Max.ToString() + "より大きい(ValidationRule)");

            return ValidationResult.ValidResult;
        }
    }
}
