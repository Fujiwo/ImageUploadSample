//#pragma warning disable CA1416
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageUploadSample.Utilities
{
    public class ImageUtility
    {
        // 縦横の大きい方をトリミング
        public static byte[] CreateThumbnail(byte[] image, (int width, int height) thumbnailSize)
        {
            using var originalMemoryStream = new MemoryStream(image);
            using var originalImage = Image.FromStream(originalMemoryStream);

            var scale = (width : (double)originalImage.Width  / (double)thumbnailSize.width ,
                            height: (double)originalImage.Height / (double)thumbnailSize.height);
            var sourceRectangle = GetTrimmingArea(originalImage, thumbnailSize, scale);

            using var thumbmailImage = new Bitmap(thumbnailSize.width, thumbnailSize.height);
            using var graphics = Graphics.FromImage(thumbmailImage);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Rectangle destinationRectangle = new Rectangle(0, 0, thumbnailSize.width, thumbnailSize.height);
            graphics.DrawImage(originalImage, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);

            using var thumbnailMemoryStream = new MemoryStream();
            thumbmailImage.Save(thumbnailMemoryStream, ImageFormat.Jpeg);
            return thumbnailMemoryStream.GetBuffer();
        }

        private static Rectangle GetTrimmingArea(Image originalImage, (int width, int height) thumbnailSize, (double width, double height) scale)
        {
            Rectangle trimmingArea = new Rectangle();

            if (scale.height == scale.width) { // 縦横同じ → トリミングなし
                trimmingArea.Width = originalImage.Width;
                trimmingArea.Height = originalImage.Height;
                trimmingArea.X = 0;
                trimmingArea.Y = 0;
            }  else if (scale.height > scale.width) { // 縦 > 横 → 縦のみトリミング
                trimmingArea.Width = originalImage.Width;
                trimmingArea.Height = Convert.ToInt32((double)thumbnailSize.height * scale.width);
                trimmingArea.X = 0;
                trimmingArea.Y = (originalImage.Height - trimmingArea.Height) / 2;
            } else { // 縦 < 横 → 横のみトリミング
                trimmingArea.Width = Convert.ToInt32((double)thumbnailSize.width * scale.height);
                trimmingArea.Height = originalImage.Height;
                trimmingArea.X = (originalImage.Width - trimmingArea.Width) / 2;
                trimmingArea.Y = 0;
            }
            return trimmingArea;
        }
    }
}
