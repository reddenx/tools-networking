using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using App.PhoneRemoveBase.ViewModels;

namespace App.PhoneRemoveBase.Views
{

    public partial class MainView : Form
    {
        /// <summary>
        /// phase 1 feature set:
        /// - can broadcast to phones(udp)
        /// - can turn broadcasting on and off
        /// - can receive messages from phones(udp)
        /// - can translate messages into events
        /// - uses hardware events to simulate input from phone
        /// phase 2 feature set:
        /// - can connect to home server(tcp)
        /// - can identify itself on home server
        /// - can receive messages from home server (tcp)
        /// - use existing logic for hardware eventing
        /// </summary>

        private MainViewModel ViewModel;

        public MainView()
        {
            ViewModel = new MainViewModel();
            InitializeComponent();
        }

        private void MenuAboutClick(object sender, EventArgs e)
        {
            var aboutForm = new AboutView();
            aboutForm.ShowDialog(this);
        }

        private void CheckboxBroadcastChanged(object sender, EventArgs e)
        {
            var isChecked = CheckboxBroadcast.Checked;
            ViewModel.SetBroadcastingStatus(isChecked);
        }
    }
}
