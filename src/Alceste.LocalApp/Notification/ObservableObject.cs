using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Alceste.LocalApp.Notification
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged<TProp>(Expression<Func<TProp>> expression)
        {
            var e = PropertyChanged;
            if (e != null)
                e(this, new PropertyChangedEventArgs(NameHelper.Name(expression)));
        }
    }
}
