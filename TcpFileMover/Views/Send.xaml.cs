using Microsoft.Win32;
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
    /// Interaction logic for Send.xaml
    /// </summary>
    public partial class Send : UserControl
    {
        private SendViewModel VM
        {
            get { return (SendViewModel)DataContext; }
            set { DataContext = value; }
        }

        public Send()
        {
            InitializeComponent();
        }

        private void BrowseForFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                VM.FilePath = dialog.FileName;
            }
        }

        private void SendFile(object sender, RoutedEventArgs e)
        {
            VM.SendFile();
        }
    }
}
