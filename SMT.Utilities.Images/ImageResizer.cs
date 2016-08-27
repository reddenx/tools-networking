using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Images
{
    public enum ResizeTypes
    {
        Stretch,
        Grow,
        Shrink
    }

    public enum Anchors
    {
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        Center
    }

    public class ImageResizer
    {
        public ImageResizer()
        { }

        public Bitmap ResizeImage(
            Image input,
            int width,
            int height,
            ResizeTypes resizeMechanic,
            Anchors anchor)
        {
            var inputDimensions = new Point(input.Width, input.Height);
            var outputDimensions = new Point(width, height);

            var outputInfo = GetScaleInfo(inputDimensions, outputDimensions, resizeMechanic, anchor);

            var output = new Bitmap(outputInfo.DestinationDimensions.X, outputInfo.DestinationDimensions.Y);
            output.SetResolution(input.HorizontalResolution, input.VerticalResolution);

            using (var graphics = Graphics.FromImage(output))
            {
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                //not really necessary, unless image has been overriden with composites
                graphics.CompositingMode = CompositingMode.SourceCopy;

                using (var attributes = new ImageAttributes())
                {
                    //so sampling beyond boundaries takes nearby pixels instead of random, transparent, or default
                    attributes.SetWrapMode(WrapMode.TileFlipXY);

                    //finally render the image to the graphics devicecontrolling the destination bitmap
                    graphics.DrawImage(
                        image: input,
                        destRect: new Rectangle(0, 0, outputInfo.DestinationDimensions.X, outputInfo.DestinationDimensions.Y),
                        srcX: outputInfo.SourceRectangle.X,
                        srcY: outputInfo.SourceRectangle.Y,
                        srcWidth: outputInfo.SourceRectangle.Width,
                        srcHeight: outputInfo.SourceRectangle.Height,
                        srcUnit: GraphicsUnit.Pixel,
                        imageAttr: attributes);
                }
            }

            return output;
        }

        private struct ScaleInfo
        {
            public Rectangle SourceRectangle;
            public Point DestinationDimensions;
        }

        private ScaleInfo GetScaleInfo(Point inputDimensions, Point outputDimensions, ResizeTypes resizeMechanic, Anchors anchor)
        {
            //if it's stretch, 1 to 1 mapping
            if (resizeMechanic == ResizeTypes.Stretch)
            {
                return new ScaleInfo
                {
                    SourceRectangle = new Rectangle(0, 0, inputDimensions.X, inputDimensions.Y),
                    DestinationDimensions = outputDimensions,
                };
            }

            //determine aspect ratio of input and output
            var aspIn = ((double)inputDimensions.X) / ((double)inputDimensions.Y);
            var aspOut = ((double)outputDimensions.X) / ((double)outputDimensions.Y);

            if (resizeMechanic == ResizeTypes.Grow)
            {
                //sourceRectangle aspect ratio will be same as output
                //determine which dimension of output will constrain and by how much
                //use that dimension to build scale and dimensions of output size
                double scale = 0;
                if (aspIn > aspOut)
                {
                    //scale y by output
                    scale = ((double)inputDimensions.Y) / ((double)outputDimensions.Y);
                }
                else
                {
                    //scale x by output
                    scale = ((double)inputDimensions.X) / ((double)outputDimensions.X);
                }

                var sourceWidth = outputDimensions.X * scale;
                var sourceHeight = outputDimensions.Y * scale;

                //determine from anchor the top left of sourcerectangle
                //TODO hardcoded
                return new ScaleInfo
                {
                    SourceRectangle = new Rectangle(0, 0, (int)Math.Floor(sourceWidth), (int)Math.Floor(sourceHeight)),
                    DestinationDimensions = outputDimensions,
                };
            }

            if (resizeMechanic == ResizeTypes.Shrink)
            {
                //using simple scaling
                double outputWidth = 0;
                double outputHeight = 0;
                if (aspIn < aspOut)
                {
                    outputWidth = ((double)inputDimensions.X * (double)outputDimensions.Y) / (double)inputDimensions.Y;
                    outputHeight = outputDimensions.Y;
                }
                else
                {
                    outputWidth = outputDimensions.X;
                    outputHeight = ((double)inputDimensions.Y * (double)outputDimensions.X) / (double)inputDimensions.X;
                }

                return new ScaleInfo
                {
                    SourceRectangle = new Rectangle(0, 0, inputDimensions.X, inputDimensions.Y),
                    DestinationDimensions = new Point((int)Math.Floor(outputWidth), (int)Math.Floor(outputHeight)),
                };
            }

            throw new NotImplementedException("The resize type of " + resizeMechanic.ToString() + " has not been implemented");
        }
    }
}
