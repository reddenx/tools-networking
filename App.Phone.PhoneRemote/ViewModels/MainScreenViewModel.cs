using App.Phone.PhoneRemote.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace App.Phone.PhoneRemote.ViewModels
{
    internal class MainScreenViewModel : ViewModelBase
    {
        private readonly NetworkManager NetworkManager;

        public MainScreenViewModel(CoreDispatcher dispatcher)
            :base(dispatcher)
        {
            NetworkManager = new NetworkManager();
        }


    }
}
