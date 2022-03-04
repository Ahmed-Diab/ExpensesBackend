//using System.Drawing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
namespace ExpensesApp.Helper
{
    public class Helper
    {
        //public static string Base64StringImage(string base64String, int width, int height, ImageFormat format)
        //{
        //    // Convert Base64 String to byte[]
        //    byte[] imageBytes = Convert.FromBase64String(base64String);
        //    MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
        //    // Convert byte[] to Image
        //    ms.Write(imageBytes, 0, imageBytes.Length);
        //    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
        //    var destRect = new Rectangle(0, 0, width, height);
        //    var destImage = new Bitmap(width, height);
        //    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
        //    using (var graphics = Graphics.FromImage(destImage))
        //    {
        //        graphics.CompositingMode = CompositingMode.SourceCopy;
        //        graphics.CompositingQuality = CompositingQuality.HighQuality;
        //        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //        graphics.SmoothingMode = SmoothingMode.HighQuality;
        //        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        //        using (var wrapMode = new ImageAttributes())
        //        {
        //            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
        //            graphics.DrawImage(image, destRect, 0, 0, image.Width,
        //            image.Height, GraphicsUnit.Pixel, wrapMode);
        //        }
        //    }
        //    using (MemoryStream ms1 = new MemoryStream())
        //    {
        //        // Convert Image to byte[]
        //        image.Save(ms1, format);
        //        byte[] imageBytes1 = ms1.ToArray();
        //        // Convert byte[] to Base64 String
        //        string outputBase64String = Convert.ToBase64String(imageBytes1);
        //        return outputBase64String;
        //    }
        //}
        public static string ResizeBase64ImageString(string Base64String, int desiredWidth, int desiredHeight)
        {
            //Base64String = Base64String.Replace("data:image/png;base64,", "");

            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(Base64String);

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);

                var imag = ScaleImage(image, desiredWidth, desiredHeight);

                using (MemoryStream ms1 = new MemoryStream())
                {
                    //First Convert Image to byte[]
                    imag.Save(ms1, ImageFormat.Png);
                    byte[] imageBytes1 = ms1.ToArray();

                    //Then Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes1);
                   // return "data:image/png;base64," + base64String;
                    return base64String;
                }
            }
        }



        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

         public static string SaveImageToDirectory(string ImgStr, string imageFormat, string invPath)
        {
            String path = System.IO.Path.Combine(invPath, "images");
            //Check if directory exist
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            var uniqueImageName = string.Format(@"{0}.{1}", Guid.NewGuid(), imageFormat);
            string imgPath = Path.Combine(path, uniqueImageName);
            byte[] imageBytes = Convert.FromBase64String(ImgStr);
            File.WriteAllBytes(imgPath, imageBytes);

            return uniqueImageName;
        }
    }
}
