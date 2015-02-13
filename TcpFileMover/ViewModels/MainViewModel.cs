using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpFileMover.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public SendViewModel SendViewModel { get; private set; }
        public ReceiveViewModel ReceiveViewModel { get; private set; }

        public MainViewModel()
        {
            SendViewModel = new SendViewModel();
            ReceiveViewModel = new ReceiveViewModel();
        }

    }
}
