using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Base
{
    internal class BaseObservable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal bool SetProperty<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            bool isEquals = EqualityComparer<T>.Default.Equals(oldValue, newValue);

            if (isEquals)
            {
                return false;
            }

            oldValue = newValue;
            Notify(propertyName);

            return true;
        }

        internal void Notify(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
