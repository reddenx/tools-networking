using App.Phone.PhoneRemote.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;

namespace App.Phone.PhoneRemote.ViewModels
{
    internal class MainScreenViewModel : ViewModelBase
    {
        private readonly NetworkManager NetworkManager;

        public MainScreenViewModel(CoreDispatcher dispatcher)
            : base(dispatcher)
        {
            NetworkManager = new NetworkManager();
            NetworkManager.SetNewMessageHost("192.168.10.150");
        }

        //denormalized method api
        public void HandleMousepadTouchEvent(SimpleTouchEvent touchEventInfo) 
        {
            NetworkManager.SendMessage( touchEventInfo.TouchId + " " + (touchEventInfo.Pressed ? "0" : "1") + " " + touchEventInfo.Location.X + ":" + touchEventInfo.Location.Y);
        }
        public void MousepadTouchEvent(SimpleTouchEvent touchEventInfo) { }
        //public void MousepadTouchEvent(SimpleTouchEvent touchEventInfo) { }
    }
}
