using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TourWriter.Utilities
{
    public static class ImageHelper
    {
        /// <summary>
        /// Retrieve an Image from a block of Base64-encoded text.
        /// </summary>
        /// <param name="text">Text to deserialize</param>
        /// <returns>Image</returns>
        public static Image DeserializeFromBase64Text(string text)
        {
            byte[] memBytes = System.Convert.FromBase64String(text);
            System.Runtime.Serialization.IFormatter formatter =
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var stream = new MemoryStream(memBytes);
            var img = (Image)formatter.Deserialize(stream);
            stream.Close();
            return img;
        }

        public static Image ByteArrayToImage(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return Image.FromStream(ms);
            }
        }

        public static byte[] ImageToByteArray(Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

        public static Bitmap CombineBitmaps(ICollection<Bitmap> bitmaps)
        {
            if (bitmaps.Count == 0)
                return new Bitmap(1, 1);

            int width = 0;
            int height = 0;

            foreach (var bmp in bitmaps)
            {
                width += bmp.Width;
                if (height < bmp.Height)
                    height = bmp.Height;
            }

            var bitmap = new Bitmap(width, height);

            using (var grfx = Graphics.FromImage(bitmap))
            {
                int x = 0;
                foreach (var bmp in bitmaps)
                {
                    grfx.DrawImage(bmp, x, 0, bmp.Width, height);
                    x += bmp.Width;
                }
            }

            return bitmap;
        }
    }
}
