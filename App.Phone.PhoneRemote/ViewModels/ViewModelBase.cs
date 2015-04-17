using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace App.Phone.PhoneRemote.ViewModels
{
    //slight variation on my tried and true ViewModelBase pattern
    class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly CoreDispatcher Dispatcher;

        public ViewModelBase(CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
        {
            string propertyName = ((propertyExpresssion.Body as MemberExpression).Member as PropertyInfo).Name;

            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected async virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler(this, e);
                }
            });
        }

    }
}
