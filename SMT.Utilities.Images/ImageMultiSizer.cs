using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Images
{
    public class ImageMultiSizer
    {
        private readonly ImageResizer Resizer;
        private readonly Point[] DestinationDimensions;
        private readonly string Directory;

        public ImageMultiSizer(string destinationDirectory, params Point[] destinationDimensions)
        {
            this.Resizer = new ImageResizer();
            this.DestinationDimensions = destinationDimensions;
            this.Directory = destinationDirectory;
        }

        public void SizeAndSaveImages(Image input, string name)
        {
            foreach (var destDimension in DestinationDimensions)
            {
                var resizedImage = Resizer.ResizeImage(input, destDimension.X, destDimension.Y, ResizeTypes.Grow, Anchors.Center);
                SaveImage(name, resizedImage);
            }
        }

        private void SaveImage(string name, Bitmap resizedImage)
        {
            var filename = name + "_" + resizedImage.Width + "_" + resizedImage.Height + ".png";
            var destinationFile = Path.Combine(Directory, filename);

            resizedImage.Save(
                destinationFile,
                ImageFormat.Png);
        }
    }
}
