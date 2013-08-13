using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mygod.YinYangCropper
{
    public static class Cropper
    {
        public static Bitmap Crop(string yinPath, string yangPath)
        {
            Bitmap yin = new Bitmap(yinPath), yang = new Bitmap(yangPath);
            int width = Math.Min(yin.Width, yang.Width), height = Math.Min(yin.Height, yang.Height);
            var result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData yinData = yin.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb),
                       yangData = yang.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb),
                       data = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* yinPtr = (byte*) yinData.Scan0, yangPtr = (byte*) yangData.Scan0, dataPtr = (byte*) data.Scan0;
                int stride = width << 2, yinStride = yinData.Stride - stride, yangStride = yangData.Stride - stride,
                    dataStride = data.Stride - stride;
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        double beta = (yinPtr[0] + yinPtr[1] + yinPtr[2] - yangPtr[0] - yangPtr[1] - yangPtr[2]) / 3.0, alpha = beta + 255;
                        dataPtr[0] = (byte)Math.Round((yinPtr[0] + yangPtr[0] + beta) * 127.5 / alpha);
                        dataPtr[1] = (byte)Math.Round((yinPtr[1] + yangPtr[1] + beta) * 127.5 / alpha);
                        dataPtr[2] = (byte)Math.Round((yinPtr[2] + yangPtr[2] + beta) * 127.5 / alpha);
                        dataPtr[3] = (byte)Math.Round(alpha);
                        yinPtr += 4;
                        yangPtr += 4;
                        dataPtr += 4;
                    }
                    yinPtr += yinStride;
                    yangPtr += yangStride;
                    dataPtr += dataStride;
                }
            }
            yin.UnlockBits(yinData);
            yang.UnlockBits(yangData);
            result.UnlockBits(yangData);
            yin.Dispose();
            yang.Dispose();
            return result;
        }
    }
}
