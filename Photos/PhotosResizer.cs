using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Photos;

public class PhotosResizer
{
    [FunctionName("PhotosResizer")]
    public async Task Run([BlobTrigger("photos/{name}", Connection = "")] Stream myBlob,
        [Blob("photos-small/{name}", FileAccess.Write, Connection = Literals.StorageConnectionString)]
        Stream imageSmall,
        [Blob("photos-medium/{name}", FileAccess.Write, Connection = Literals.StorageConnectionString)]
        Stream imageMedium,
        ILogger log)
    {
        log?.LogInformation("Resizing...");

        try
        {
            var msMedium = CreateMemoryStream(myBlob, ImageSize.Medium);
            await msMedium.CopyToAsync(imageMedium);

            var msSmall = CreateMemoryStream(myBlob, ImageSize.Small);
            await msSmall.CopyToAsync(imageSmall);
        }
        catch (Exception ex)
        {
            log?.LogError(ex.ToString());
        }
        finally
        {
            imageSmall.Close();
            imageMedium.Close();
        }
    }

    private static MemoryStream CreateMemoryStream(Stream image, ImageSize imageSize)
    {
        var ms = new MemoryStream();
        var img = Image.FromStream(image);
        var desiredWidth = imageSize == ImageSize.Medium ? img.Width / 2 : img.Width / 4;

        var ratio = (decimal)desiredWidth / img.Width;
        var resized = ResizeImage(img, desiredWidth, (int)Math.Floor(img.Height * ratio));
        resized.Save(ms, ImageFormat.Jpeg);
        ms.Position = 0;
        return ms;
    }

    private static Bitmap ResizeImage(Image image, int width, int height)
    {
        var destRect = new Rectangle(0, 0, width, height);

        var destImage = new Bitmap(width, height);

        using (var graphics = Graphics.FromImage(destImage))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (var wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }
        }

        return destImage;
    }

    private enum ImageSize
    {
        Small,
        Medium
    }
}