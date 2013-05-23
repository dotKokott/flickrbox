using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.FileConversion
{
    public class FileConverter
    {
        public static void FileToPng(string sourcePath, string destinationPath)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            using (var fs = new FileStream(sourcePath, FileMode.Open))
            {
                var length = (int)fs.Length;
                //TODO: dynamic width
                var width = 1000;
                var height = (int)Math.Ceiling((double)(length / 4) / width);
                
                var image = new Bitmap(width, height);                
                image.SetPixel(0, 0, Color.FromArgb(length));
                for(int x = 1; x < width; x++) 
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (fs.Position == length) break;

                        var bytes = new Byte[4] { getNextByte(fs), getNextByte(fs), getNextByte(fs), getNextByte(fs) };
                        var color = Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
                        image.SetPixel(x, y, color);
                    }
                }

                image.Save(destinationPath, ImageFormat.Png);
                image.Dispose();
            }

            stopWatch.Stop();
            Debug.WriteLine(String.Format("FileToPng: {0} s or {1} ms", stopWatch.Elapsed.Seconds, stopWatch.ElapsedMilliseconds));
        }

        private static Byte getNextByte(Stream stream)
        {
            if(stream.Position != stream.Length)
            {
                return (Byte)stream.ReadByte();
            }

            return 0;
        }

        public static void FileFromPng(string sourcePath, string destinationPath)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            using (var fs = new FileStream(destinationPath, FileMode.Create))
            {
                var image = new Bitmap(sourcePath);
                int length = image.GetPixel(0, 0).ToArgb();
                int bytesRead = 0;

                for (int x = 1; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (bytesRead == length) break;

                            switch (i)
                            {
                                case 0:
                                    fs.WriteByte(image.GetPixel(x, y).A);
                                    break;
                                case 1:
                                    fs.WriteByte(image.GetPixel(x, y).R);
                                    break;
                                case 2:
                                    fs.WriteByte(image.GetPixel(x, y).G);
                                    break;
                                case 3:
                                    fs.WriteByte(image.GetPixel(x, y).B);
                                    break;
                            }

                            bytesRead++;
                        }                       
                    }
                }
                image.Dispose();
            }

            stopWatch.Stop();
            Debug.WriteLine(String.Format("FileFromPng: {0} s or {1} ms", stopWatch.Elapsed.Seconds, stopWatch.ElapsedMilliseconds));
        }

    }
}
