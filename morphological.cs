using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

#pragma warning disable CA1416


namespace Morphological
{
    class morphological
    {
        public static Bitmap Erode(Bitmap SrcImage, byte[,] Mat)
        {

            if (SrcImage.PixelFormat != PixelFormat.Format1bppIndexed)
            {
                throw new ArgumentException("The image should be (1bpp).");
            }

            int Width = SrcImage.Width, Height = SrcImage.Height;

            Bitmap tempbmp = new Bitmap(Width, Height, PixelFormat.Format1bppIndexed);

            ColorPalette palette = tempbmp.Palette;
            palette.Entries[0] = Color.Black;
            palette.Entries[1] = Color.White;
            tempbmp.Palette = palette;

            BitmapData SrcData = SrcImage.LockBits(new Rectangle(0, 0,
                Width, Height), ImageLockMode.ReadOnly,
                PixelFormat.Format1bppIndexed);

            BitmapData DestData = tempbmp.LockBits(new Rectangle(0, 0, Width,
                Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);



            int size = 3;
            bool ColorVal;
            int radius = size / 2;
            int ir, jr;

            unsafe
            {

                for (int colm = radius; colm < Height - radius; colm++)
                {
                    byte* ptr = (byte*)SrcData.Scan0 + (colm * SrcData.Stride);
                    byte* dstPtr = (byte*)DestData.Scan0 + (colm * DestData.Stride);

                    for (int row = radius; row < Width - radius; row++)
                    {
                        ColorVal = true;

                        for (int eleColm = 0; eleColm < size; eleColm++)
                        {
                            ir = eleColm - radius;
                            byte* tempPtr = (byte*)SrcData.Scan0 +
                                ((colm + ir) * SrcData.Stride);

                            for (int eleRow = 0; eleRow < size; eleRow++)
                            {
                                jr = eleRow - radius;

                                ColorVal = (ColorVal & ((tempPtr[(row + jr) >> 3]
                                 & (1 << (7 - ((row + jr) & 7)))) != 0
                                  && (Mat[eleColm, eleRow] != 0)));

                            }
                        }

                        if (ColorVal)
                            dstPtr[(row >> 3)] &= (byte)~(1 << (7 - (row & 7)));
                        else
                            dstPtr[(row >> 3)] |= (byte)(1 << (7 - (row & 7)));

                        ptr++;
                    }
                }
            }

            SrcImage.UnlockBits(SrcData);
            tempbmp.UnlockBits(DestData);

            return tempbmp;
        }

        public static Bitmap Dilate(Bitmap SrcImage, byte[,] Mat)
        {

            if (SrcImage.PixelFormat != PixelFormat.Format1bppIndexed)
            {
                throw new ArgumentException("The image should be (1bpp).");
            }

            int Width = SrcImage.Width, Height = SrcImage.Height;
            Bitmap tempbmp = new Bitmap(Width, Height, PixelFormat.Format1bppIndexed);

            BitmapData SrcData = SrcImage.LockBits(new Rectangle(0, 0,
                Width, Height), ImageLockMode.ReadOnly,
                PixelFormat.Format1bppIndexed);

            BitmapData DestData = tempbmp.LockBits(new Rectangle(0, 0, Width,
                Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);

            int size = 3;
            bool ColorVal;
            int radius = size / 2;
            int ir, jr;

            unsafe
            {

                for (int colm = radius; colm < Height - radius; colm++)
                {
                    byte* ptr = (byte*)SrcData.Scan0 + (colm * SrcData.Stride);
                    byte* dstPtr = (byte*)DestData.Scan0 + (colm * DestData.Stride);

                    for (int row = radius; row < Width - radius; row++)
                    {
                        ColorVal = false;

                        for (int eleColm = 0; eleColm < size; eleColm++)
                        {
                            ir = eleColm - radius;
                            byte* tempPtr = (byte*)SrcData.Scan0 +
                                ((colm + ir) * SrcData.Stride);

                            for (int eleRow = 0; eleRow < size; eleRow++)
                            {
                                jr = eleRow - radius;

                                ColorVal = (ColorVal | ((tempPtr[(row + jr) >> 3] &
                                (1 << (7 - ((row + jr) & 7)))) != 0 &&
                                 (Mat[eleColm, eleRow] != 0)));

                            }
                        }

                        if (ColorVal)
                            dstPtr[(row >> 3)] &= (byte)~(1 << (7 - (row & 7)));
                        else
                            dstPtr[(row >> 3)] |= (byte)(1 << (7 - (row & 7)));
                        
                        ptr++;
                    }
                }
            }

            SrcImage.UnlockBits(SrcData);
            tempbmp.UnlockBits(DestData);

            return tempbmp;
        }
        public void Run()
        {

            byte[,] mat =
            {
            {1, 1 , 1},
            {1, 1 , 1},
            {1, 1 , 1}
        };
            Bitmap resultErode;
            Bitmap resultDilate;

            Console.Write("Enter the image path : ");
            string path;
            path = Console.ReadLine();
            Bitmap test = new Bitmap(path);

            string dir = @"results";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);


            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();


            resultErode = Erode(test, mat);
            resultErode.Save("results/ErodedResult.bmp");

            resultDilate = Dilate(test, mat);
            resultDilate.Save("results/DilatedResult.bmp");

            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");

            /*
            Morphological opening and closure is a technique for improving image quality by manipulating the erosion and dilatation processes. In the opening phase, 
            the picture is eroded and then dilates, whereas in the closing process, the image is eroded and then dilates.
            */

        }
    }
}