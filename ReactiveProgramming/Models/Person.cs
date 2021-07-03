using System.ComponentModel;

namespace ReactiveProgramming
{
    public class Person : INotifyPropertyChanged
    {
        public override string ToString()
        {
            return string.Format("{0}({1})", Name, Age);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }
        private string _name;
        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                _age = value;
                RaisePropertyChanged("Age");
            }
        }
        private int _age;
    }
}
