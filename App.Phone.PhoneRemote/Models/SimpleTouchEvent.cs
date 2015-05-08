using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace App.Phone.PhoneRemote.Models
{
    internal class SimpleTouchEvent
    {
        public uint TouchId { get; set; }
        public bool Pressed { get; set; }
        public Point Location { get; set; }

        public SimpleTouchEvent(uint touchId, Point location, bool isPressed)
        {
            TouchId = touchId;
            Pressed = isPressed;
            Location = location;
        }
    }
}
