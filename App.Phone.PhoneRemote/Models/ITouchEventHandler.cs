using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Phone.PhoneRemote.Models
{
    internal interface ITouchEventHandler
    {
        void HandleTouchEvent(SimpleTouchEvent touchEventInfo);
    }
}
