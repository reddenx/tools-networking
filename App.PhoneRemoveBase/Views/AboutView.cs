using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App.PhoneRemoveBase.Views
{
    public partial class AboutView : Form
    {
        public AboutView()
        {
            InitializeComponent();
            label1.Text = string.Format("Version: {0}", GetVersionNumber());
        }



        private string GetVersionNumber()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var version = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                return string.Format("{0}.{1}.{2}.{3}",
                    version.Major,
                    version.Minor,
                    version.Build,
                    version.Revision);
            }
            else
            {
                return "DEBUG";
            }
        }
    }
}
