using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TcpFileMover.ViewModels;

namespace TcpFileMover.Views
{
    /// <summary>
    /// Interaction logic for Receive.xaml
    /// </summary>
    public partial class Receive : UserControl
    {
        private ReceiveViewModel VM
        {
            get { return (ReceiveViewModel)DataContext; }
            set { DataContext = value; }
        }

        public Receive()
        {
            InitializeComponent();
        }

        private void ReceiveFile(object sender, RoutedEventArgs e)
        {
            VM.ReceiveFile();
        }
    }
}
