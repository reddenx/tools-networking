using SMT.Utilities.Images;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.TestingGrounds
{
    static class ImageSizeTesting
    {
        public static void Run()
        {
            var sizer = new ImageMultiSizer(@"C:\temp\ImageTesting\Resized", new Point(500,100), new Point(100,500));
            sizer.SizeAndSaveImages(Image.FromFile(@"C:\temp\ImageTesting\Herro.jpg"), "herro");
        }
    }
}
