using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class BitmapExtension
    {
        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap Zoom(this Bitmap bitmap, int width, int height)
        {

            //若缩放大小与原图一样，则返回原图不做处理
            if (width == bitmap.Width && height == bitmap.Height)
            {
                return new Bitmap(bitmap);
            }
            double ratioW = width * 1.0 / bitmap.Width;
            double ratioH = height * 1.0 / bitmap.Height;
            return bitmap.Zoom(ratioW, ratioH);
        }
        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="ratioW"></param>
        /// <param name="ratioH"></param>
        /// <returns></returns>
        public static Bitmap Zoom(this Bitmap bitmap, double ratioW, double ratioH)
        {
            //若缩放大小与原图一样，则返回原图不做处理
            if ((ratioW == 1.0) && ratioH == 1.0)
            {
                return new Bitmap(bitmap);
            }
            //计算缩放高宽
            var newBitmap = new Bitmap((int)(ratioW * bitmap.Width), (int)(ratioH * bitmap.Height));

            BitmapData srcBmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData dstBmpData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* srcPtr = (byte*)srcBmpData.Scan0;
                byte* dstPtr = (byte*)dstBmpData.Scan0;
                double bitmapCurrentHieght = 0;
                double bitmapCurrentWidth = 0;
                double stepHieght = 1.0 / ratioH;
                double stepWidth = 1.0 / ratioW;

                byte* bitmapCurrentPoint = null;
                byte* newBitmapCurrentPoint = null;
                for (int i = 0; i < dstBmpData.Height; i++)
                {
                    newBitmapCurrentPoint = dstPtr;
                    bitmapCurrentWidth = 0;
                    srcPtr = (byte*)srcBmpData.Scan0 + (((int)bitmapCurrentHieght) * srcBmpData.Stride);
                    for (int j = 0; j < dstBmpData.Width; j++)
                    {
                        bitmapCurrentPoint = srcPtr + (((int)bitmapCurrentWidth) * 3);
                        *newBitmapCurrentPoint = *bitmapCurrentPoint;
                        *(newBitmapCurrentPoint + 1) = *(bitmapCurrentPoint + 1);
                        *(newBitmapCurrentPoint + 2) = *(bitmapCurrentPoint + 2);
                        newBitmapCurrentPoint += 3;
                        bitmapCurrentWidth += stepWidth;

                    }
                    dstPtr += dstBmpData.Stride;
                    bitmapCurrentHieght += stepHieght;
                }
            }
            bitmap.UnlockBits(srcBmpData);
            newBitmap.UnlockBits(dstBmpData);
            return newBitmap;
        }

        /// <summary>
        /// 对角线翻转
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Bitmap FlipDiagonal(this Bitmap bitmap)
        {
            Bitmap bitmap1 = new Bitmap(bitmap.Height, bitmap.Width);
            bitmap1.HandleImage((x, y, r, g, b, setter) =>
            {
                Color color = bitmap.GetPi(y, x);
                setter(color.R, color.G, color.B);
            });
            return bitmap1;
        }

        /// <summary>
        /// 旋转90
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Bitmap Rotate90(this Bitmap bitmap)
        {
            Bitmap bitmap1 = new Bitmap(bitmap.Height, bitmap.Width);
            bitmap1.HandleImage((x, y, r, g, b, setter) =>
            {
                Color color = bitmap.GetPi(y, bitmap.Height - 1 - x);
                setter(color.R, color.G, color.B);
            });
            return bitmap1;
        }

        /// <summary>
        /// 旋转-90
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Bitmap Rotate_90(this Bitmap bitmap)
        {
            Bitmap bitmap1 = new Bitmap(bitmap.Height, bitmap.Width);
            bitmap1.HandleImage((x, y, r, g, b, setter) =>
            {
                Color color = bitmap.GetPi(bitmap.Width - 1 - y, x);
                setter(color.R, color.G, color.B);
            });
            return bitmap1;
        }
        /// <summary>
        /// 设置像素点的颜色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public static void SetPi(this Bitmap bitmap, int x, int y, byte r, byte g, byte b)
        {
            var res = bitmap.LockBits(new Rectangle(x, y, 1, 1), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* first = (byte*)res.Scan0;
                *(first + 2) = r;
                *(first + 1) = g;
                *first = b;
            }
            bitmap.UnlockBits(res);
        }
        /// <summary>
        /// 设置像素点的颜色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public static void SetPi(this Bitmap bitmap, int x, int y, Color color)
        {
            var res = bitmap.LockBits(new Rectangle(x, y, 1, 1), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* first = (byte*)res.Scan0;
                *(first + 2) = color.R;
                *(first + 1) = color.G;
                *first = color.B;
            }
            bitmap.UnlockBits(res);
        }
        /// <summary>
        /// 设置多点像素的颜色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public static void SetPi(this Bitmap bitmap, Action<Action<int, int, Color>> action)
        {
            action((x, y, color) =>
            {
                bitmap.SetPi(x, y, color);
            });
        }
        /// <summary>
        /// 获取像素点的颜色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Color GetPi(this Bitmap bitmap, int x, int y)
        {
            var res = bitmap.LockBits(new Rectangle(x, y, 1, 1), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Color color;
            unsafe
            {
                byte* first = (byte*)res.Scan0;
                color = Color.FromArgb(*(first + 2), *(first + 1), *first);
            }
            bitmap.UnlockBits(res);
            return color;
        }
        /// <summary>
        /// 获取多个像素点的颜色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static void GetPi(this Bitmap bitmap, Action<Func<int, int, Color>> action)
        {
            action((x, y) => bitmap.GetPi(x, y));
        }
        /// <summary>
        /// 获取或设置多个像素点的颜色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static void GetSetPix(this Bitmap bitmap, Action<Func<int, int, Color>, Action<int, int, Color>> action)
        {
            action((x, y) => bitmap.GetPi(x, y), (x, y, color) => bitmap.SetPi(x, y, color));
        }
        /// <summary>
        /// 读取图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Bitmap ReadImage(this Bitmap bitmap, string name)
        {
            if (!File.Exists(name))
            {
                return null;
            }
            return Bitmap.FromFile(name) as Bitmap;
        }
        /// <summary>
        /// 变灰
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Bitmap GrayImage(this Bitmap bitmap)
        {
            bitmap.HandleImage((x, y, r, g, b, func) =>
            {
                var avg = (byte)((r + g + b) / 3);
                func(avg, avg, avg);
            });
            return bitmap;
        }
        /// <summary>
        /// 取图片的部分
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <returns></returns>
        public static Bitmap Reduce(this Bitmap bitmap, int startX, int startY, int endX, int endY)
        {
            if (startX < 0)
            {
                startX = 0;
            }
            else if (startX > bitmap.Width - 1)
            {
                startX = bitmap.Width - 1;
            }
            if (startY < 0)
            {
                startY = 0;
            }
            else if (startY > bitmap.Height - 1)
            {
                startY = bitmap.Height - 1;
            }
            int width = endX - startX;
            int height = endY - startY;
            if (startX + width > bitmap.Width)
            {
                width = bitmap.Width - 1 - startX;
            }
            if (startY + height > bitmap.Height)
            {
                height = bitmap.Height - 1 - startY;
            }
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bmp.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // 用白色清空 
                //g.Clear(Color.White);

                // 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。 
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // 指定高质量、低速度呈现。 
                g.SmoothingMode = SmoothingMode.HighQuality;

                // 在指定位置并且按指定大小绘制指定的 Image 的指定部分。 
                g.DrawImage(bitmap, new Rectangle(0, 0, width, height), new Rectangle(startX, startY, width, height), GraphicsUnit.Pixel);
            }
            return bmp;
        }
        /// <summary>
        /// 遍历每个像素
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="handleFunc">row column r g b</param>
        public static void PixForeach(this Bitmap bitmap, Action<int, int, byte, byte, byte> handleFunc)
        {
            bitmap.PixForeach(0, 0, bitmap.Width, bitmap.Height, handleFunc);
        }
        /// <summary>
        /// 遍历每个像素
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="handleFunc">row column r g b</param>
        public static void PixForeach(this Bitmap bitmap, int startX, int startY, int endX, int endY, Action<int, int, byte, byte, byte> handleFunc)
        {
            PixForeach(bitmap, new Rectangle(startX, startY, endX - startX, endY - startY), handleFunc);

            //简单写法
            //bitmap.HandleImage((x, y, r, g, b, func) =>
            //{
            //    handleFunc(x, y, r, g, b);
            //});
        }
        /// <summary>
        /// 遍历每个像素
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="handleFunc">row column r g b</param>
        public static void PixForeach(this Bitmap bitmap, Rectangle rectangle, Action<int, int, byte, byte, byte> handleFunc)
        {
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int width = bitmapData.Width;
                int height = bitmapData.Height;
                int stride = bitmapData.Stride;
                byte* currentPoint;
                byte* outCirclePointer = (byte*)bitmapData.Scan0;
                for (int i = 0; i < height; i++)
                {
                    currentPoint = outCirclePointer;
                    for (int j = 0; j < width; j++)
                    {
                        handleFunc(rectangle.X + j, rectangle.Y + i, *(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                        currentPoint += 3;
                    }
                    outCirclePointer += stride;
                }
            }
            bitmap.UnlockBits(bitmapData);
        }
        /// <summary>
        /// 处理每个像素
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="handleFunc">column row r g b</param>
        public static void HandleImage(this Bitmap bitmap, Action<int, int, byte, byte, byte, Action<byte, byte, byte>> handleFunc)
        {
            bitmap.HandleImage(new Rectangle(0, 0, bitmap.Width, bitmap.Height), handleFunc);
        }
        /// <summary>
        /// 处理每个像素
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="handleFunc">column row r g b</param>
        public static void HandleImage(this Bitmap bitmap, int startX, int startY, int endX, int endY, Action<int, int, byte, byte, byte, Action<byte, byte, byte>> handleFunc)
        {
            HandleImage(bitmap, new Rectangle(startX, startY, endX - startX, endY - startY), handleFunc);
        }

        /// <summary>
        /// 处理每个像素
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="handleFunc">column row r g b</param>
        public static void HandleImage(this Bitmap bitmap, Rectangle rectangle, Action<int, int, byte, byte, byte, Action<byte, byte, byte>> handleFunc)
        {
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int width = bitmapData.Width;
                int height = bitmapData.Height;
                int stride = bitmapData.Stride;
                byte* outCirclePointer = (byte*)bitmapData.Scan0;
                byte* currentPoint = null;
                Action<byte, byte, byte> action = (r, g, b) =>
                {
                    *(currentPoint + 2) = r;
                    *(currentPoint + 1) = g;
                    *(currentPoint) = b;
                };
                for (int i = 0; i < height; i++)
                {
                    currentPoint = outCirclePointer;
                    for (int j = 0; j < width; j++)
                    {
                        handleFunc(
                            rectangle.X + j, rectangle.Y + i,
                            *(currentPoint + 2), *(currentPoint + 1), *(currentPoint),
                            action);
                        currentPoint += 3;
                    }
                    outCirclePointer += stride;
                }
            }
            bitmap.UnlockBits(bitmapData);
        }
        /// <summary>
        /// 处理每个像素
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="handleFunc">column row r g b</param>
        public static void HandleImage(this Bitmap bitmap,
            Rectangle rectangle, int windowWidth, int windowHeight, Func<byte> fillValueFunc,
            Action<int, int, Action<Action<int, int, byte, byte, byte>>, Action<byte, byte, byte>> handleFunc, int windowPixLocationType = 0, bool outReturnFlag = true)
        {
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                var fillValue = fillValueFunc();
                int width = bitmapData.Width;
                int height = bitmapData.Height;
                int stride = bitmapData.Stride;
                byte* outCirclePointer = (byte*)bitmapData.Scan0;
                byte* currentPoint = null, currentPointTmp;

                Action<byte, byte, byte> action = (r, g, b) =>
                {
                    *(currentPoint + 2) = r;
                    *(currentPoint + 1) = g;
                    *(currentPoint) = b;
                };

                int y, yy = 0, x, xx = 0, dx = 0, dy = 0, halfWidth, halfHeight,
                    windowHlafWidthPointerStep, windowHlafHeightPointerStep,
                    currentWidthPointerOffset, currentHeightPointerOffset;
                halfWidth = windowWidth >> 1;
                halfHeight = windowHeight >> 1;
                windowHlafWidthPointerStep = halfWidth * 3;
                windowHlafHeightPointerStep = halfHeight * stride;
                Func<int> xxGetter = () =>
                {
                    return xx;
                };
                Func<int> yyGetter = () =>
                {
                    return yy;
                };
                if (windowPixLocationType == 1)
                {
                    xxGetter = () =>
                    {
                        return xx + halfWidth;
                    };
                    yyGetter = () =>
                    {
                        return yy + halfHeight;
                    };
                }
                else if (windowPixLocationType == 2)
                {
                    xxGetter = () =>
                    {
                        return dx;
                    };
                    yyGetter = () =>
                    {
                        return dy;
                    };
                }
                for (y = 0; y < height; y++)
                {
                    currentPoint = outCirclePointer;
                    for (x = 0; x < width; x++)
                    {
                        handleFunc(x, y, ss =>
                        {
                            currentHeightPointerOffset = -windowHlafHeightPointerStep;
                            for (yy = -halfHeight; yy < halfHeight + 1; yy++)
                            {
                                currentWidthPointerOffset = -windowHlafWidthPointerStep;
                                for (xx = -halfWidth; xx < halfWidth + 1; xx++)
                                {
                                    dx = x + xx;
                                    dy = y + yy;
                                    if (dx < rectangle.X || dx > rectangle.Right - 1 || dy < rectangle.Y || dy > rectangle.Bottom - 1)
                                    {
                                        if (outReturnFlag)
                                        {
                                            ss(xxGetter(), yyGetter(), fillValue, fillValue, fillValue);
                                        }
                                    }
                                    else
                                    {
                                        currentPointTmp = currentPoint + currentWidthPointerOffset + currentHeightPointerOffset;
                                        ss(xxGetter(), yyGetter(), *(currentPointTmp + 2), *(currentPointTmp + 1), *(currentPointTmp));
                                    }
                                    currentWidthPointerOffset += 3;
                                }
                                currentHeightPointerOffset += stride;
                            }
                        }, action);
                        currentPoint += 3;
                    }
                    outCirclePointer += stride;
                }
            }
            bitmap.UnlockBits(bitmapData);
        }
        /// <summary>
        /// 处理每个像素
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="handleFunc">column row r g b</param>
        public static void HandleImage(this Bitmap bitmap, Rectangle rectangle, Action<int, int, Action<byte, byte, byte>> handleFunc)
        {
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int width = bitmapData.Width;
                int height = bitmapData.Height;
                int stride = bitmapData.Stride;
                byte* outCirclePointer = (byte*)bitmapData.Scan0;
                byte* currentPoint = null;
                Action<byte, byte, byte> action = (r, g, b) =>
                {
                    *(currentPoint + 2) = r;
                    *(currentPoint + 1) = g;
                    *(currentPoint) = b;
                };
                for (int i = 0; i < height; i++)
                {
                    currentPoint = outCirclePointer;
                    for (int j = 0; j < width; j++)
                    {
                        handleFunc(
                            rectangle.X + j, rectangle.Y + i,
                            action);
                        currentPoint += 3;
                    }
                    currentPoint += stride;
                }
            }
            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// 遍历每行的数据
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rgbHandle"></param>
        /// <param name="oneRowEvt">row buffer</param>
        public static void RowForeach(this Bitmap bitmap, Func<byte, byte, byte, byte> rgbHandle, Action<int, byte[], Action> oneRowEvt)
        {
            bitmap.RowForeach(0, 0, bitmap.Width, bitmap.Height, rgbHandle, oneRowEvt);
        }
        /// <summary>
        /// 遍历每行的数据
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rgbHandle"></param>
        /// <param name="oneRowEvt">row buffer</param>
        public static void RowForeach(this Bitmap bitmap, int startX, int startY, int endX, int endY, Func<byte, byte, byte, byte> rgbHandle, Action<int, byte[], Action> oneRowEvt)
        {
            RowForeach(bitmap, new Rectangle(startX, startY, endX - startX, endY - startY), rgbHandle, oneRowEvt);
        }
        /// <summary>
        /// 遍历每行的数据
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rgbHandle"></param>
        /// <param name="oneRowEvt">row buffer</param>
        public static void RowForeach(this Bitmap bitmap, Rectangle rectangle, Func<byte, byte, byte, byte> rgbHandle, Action<int, byte[], Action> oneRowEvt)
        {
            bitmap.RowForeach(true, rectangle, rgbHandle, oneRowEvt);
        }

        /// <summary>
        /// 遍历每行的数据
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rgbHandle"></param>
        /// <param name="oneRowEvt">row buffer</param>
        public static void RowForeach(this Bitmap bitmap, bool isPositive, Rectangle rectangle, Func<byte, byte, byte, byte> rgbHandle, Action<int, byte[], Action> oneRowEvt)
        {
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int width = bitmapData.Width;
                int height = bitmapData.Height;
                int stride = bitmapData.Stride;
                byte* san0 = (byte*)bitmapData.Scan0;
                byte* currentPoint = null;
                byte* outCirclePointer = null;
                byte[] buffer = null;
                Action update = () =>
                {
                    byte* updatePointer = outCirclePointer;
                    for (int i = 0; i < width; i++)
                    {
                        *(updatePointer + 2) = buffer[i];
                        updatePointer += 3;
                    }
                };
                if (isPositive)
                {
                    outCirclePointer = san0;
                    for (int i = 0; i < height; i++)
                    {
                        buffer = new byte[width];
                        currentPoint = outCirclePointer;
                        for (int j = 0; j < width; j++)
                        {
                            var tmp = rgbHandle(*(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                            buffer[j] = tmp;
                            currentPoint += 3;
                        }
                        oneRowEvt(rectangle.Y + i, buffer, update);
                        outCirclePointer += stride;
                    }
                }
                else
                {
                    outCirclePointer = san0 + (stride * (height - 1));
                    for (int i = height - 1; i > -1; i--)
                    {
                        buffer = new byte[width];
                        currentPoint = outCirclePointer;
                        for (int j = 0; j < width; j++)
                        {
                            var tmp = rgbHandle(*(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                            buffer[j] = tmp;
                            currentPoint += 3;
                        }
                        oneRowEvt(rectangle.Y + i, buffer, update);
                        outCirclePointer -= stride;
                    }
                }
            }
            bitmap.UnlockBits(bitmapData);
        }
        /// <summary>
        /// 获取一行的像素数据
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="height"></param>
        /// <param name="callback"></param>
        public static void GetRowsBuffer(this Bitmap bitmap, int height, Action<int, byte[], Action> callback)
        {
            bitmap.RowForeach(0, height, bitmap.Width, height + 1, (r, g, b) => (byte)((r + g + b) / 3), callback);
        }

        /// <summary>
        /// 遍历每列的数据
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rgbHandle"></param>
        /// <param name="oneRowEvt">row buffer</param>
        public static void ColumnForeach(this Bitmap bitmap, bool isPositive, Rectangle rectangle, Func<byte, byte, byte, byte> rgbHandle, Action<int, byte[], Action> oneColumnEvt)
        {
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int width = bitmapData.Width;
                int height = bitmapData.Height;
                int stride = bitmapData.Stride;
                byte* san0 = (byte*)bitmapData.Scan0;
                byte* currentPoint = null;
                byte* outCirclePointer = null;
                byte[] buffer = null;
                Action update = () =>
                {
                    byte* updatePointer = outCirclePointer;
                    for (int i = 0; i < height; i++)
                    {
                        *(updatePointer + 2) = buffer[i];
                        updatePointer += stride;
                    }
                };
                if (isPositive)
                {
                    outCirclePointer = san0;
                    for (int j = 0; j < width; j++)
                    {
                        buffer = new byte[height];
                        currentPoint = outCirclePointer;
                        for (int i = 0; i < height; i++)
                        {
                            var tmp = rgbHandle(*(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                            buffer[i] = tmp;
                            currentPoint += stride;
                        }
                        oneColumnEvt(rectangle.X + j, buffer, update);
                        outCirclePointer += 3;
                    }
                }
                else
                {
                    outCirclePointer = san0 + (width - 1) * 3;
                    for (int j = width - 1; j > -1; j--)
                    {
                        buffer = new byte[height];
                        currentPoint = outCirclePointer;
                        for (int i = 0; i < height; i++)
                        {
                            var tmp = rgbHandle(*(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                            buffer[i] = tmp;
                            currentPoint += stride;
                        }
                        oneColumnEvt(rectangle.X + j, buffer, update);
                        outCirclePointer -= 3;
                    }
                }
            }
            bitmap.UnlockBits(bitmapData);
        }
        /// <summary>
        /// 搜索指定圆
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rgbHandle"></param>
        /// <param name="oneRowEvt">row buffer</param>
        public static void CircleLineForeach(this Bitmap bitmap, int centerX, int centerY, int radius,
            Func<byte, byte, byte, byte> colorFunc,
            Action<int, Color, Action<Color>> pixAction, Action<int, byte[]> action)
        {
            int diameter = radius + radius;
            if (diameter > bitmap.Width || diameter > bitmap.Height)
            {
                throw new Exception("圆过大");
            }
            var area = new Rectangle(centerX - radius, centerY - radius, diameter, diameter);
            if (area.X < 0 || area.Y < 0 || area.Right > bitmap.Width || area.Bottom > bitmap.Height)
            {
                throw new Exception("圆区域超过图像区域");
            }
            bitmap.GetSetPix((getPix, setPix) =>
            {
                int size = (int)(0.5 * Math.Sqrt(2) * radius + 0.5);
                byte[] buffer = new byte[size * 8];
                int bufferIndex = 0;
                ShapeCreateTool.CreateCircle(centerX, centerY, radius, 0, Math.PI * 2, (x, y) =>
                 {
                     var color = getPix(x, y);
                     var g = colorFunc(color.R, color.G, color.B);
                     pixAction(bufferIndex, color, color1 =>
                     {
                         setPix(x, y, color1);
                     });
                     buffer[bufferIndex] = g;
                     bufferIndex++;
                 });
                action(radius, buffer);
            });
        }

        /// <summary>
        /// 画线段
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="msg"></param>
        /// <param name="msgSize"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public static void DrawSegment(this Bitmap bitmap,
            string msg, int msgSize,
            int x1, int y1, int x2, int y2, int offsetX = 0, int offsetY = 0)
        {
            int scaleWidth = 5;
            var brush = new SolidBrush(Color.Red);
            var reaPen = new Pen(brush);
            var g = Graphics.FromImage(bitmap);

            int startX = x1;
            int startY = y1;
            var diffX = x2 - x1;
            var diffY = y2 - y1;
            var length = Math.Sqrt(diffX * diffX + diffY * diffY);

            var angle = Math.PI / 2;
            if (diffY < 0)
            {
                angle = -angle;
            }
            if (diffX != 0)
            {
                var diff = diffY / diffX;
                if (diffX < 0)
                {
                    diff = -diff;
                    startX = x2;
                    startY = y2;
                }
                angle = Math.Atan(diff);
            }
            g.TranslateTransform(startX, startY);
            g.RotateTransform((float)(angle / Math.PI * 180));
            g.TranslateTransform(offsetX, 0);
            g.DrawLine(reaPen, 0, scaleWidth + offsetY, (int)length, scaleWidth + offsetY);
            g.DrawLine(reaPen, 0, 0, 0, (scaleWidth << 1) + offsetY);
            g.DrawLine(reaPen, (int)length, 0, (int)length, (scaleWidth << 1) + offsetY);
            float msgStartX = (float)((length * 0.5) - ((msg.Length >> 1) * msgSize));
            float msgStartY = scaleWidth + 3 + offsetY;
            float msgWidth = msg.Length * (msgSize);
            float msgHeight = (msgSize);
            g.FillRectangle(new SolidBrush(Color.White), msgStartX, msgStartY + 3, msgWidth + 2, msgHeight + 5);
            g.DrawString(msg, new Font("微软雅黑", msgSize), brush, msgStartX, msgStartY);

            g.Dispose();
        }
        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static Bitmap DrawLine(this Bitmap bitmap, int x1, int y1, int x2, int y2)
        {
            var g = Graphics.FromImage(bitmap);
            var brush = new SolidBrush(Color.Red);
            var reaPen = new Pen(brush);
            g.DrawLine(reaPen, x1, y1, x2, y2);
            g.Dispose();
            return bitmap;
        }

        /// <summary>
        /// 斜角遍历
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static void ForeachBevel(this Bitmap bitmap, Rectangle rectangle, Func<byte, byte, byte, byte> selectChannel, Action<byte, byte> callback)
        {
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            unsafe
            {
                int width = bitmapData.Width;
                int height = bitmapData.Height;
                int stride = bitmapData.Stride;
                var step = 3 + stride;//斜角
                byte* san0 = (byte*)bitmapData.Scan0;
                byte* currentOutCirclePoint;
                byte* currentPoint;
                int x;
                int y;
                currentOutCirclePoint = san0;
                int count = 0;
                byte currentValue = 0;
                byte nextValue = 0;
                for (int i = 1; i < height; i++)
                {
                    y = i;
                    x = 1;
                    currentPoint = currentOutCirclePoint;
                    currentValue = selectChannel(*(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                    nextValue = 0;
                    currentPoint += step;
                    while (x < width && y < height)
                    {
                        nextValue = selectChannel(*(currentPoint + 2), *(currentPoint + 1), *currentPoint);
                        callback(currentValue, nextValue);
                        currentValue = nextValue;
                        currentPoint += step;
                        y++;
                        x++;
                        count++;
                    }
                    currentOutCirclePoint += stride;
                }
                currentOutCirclePoint = san0 + 3;
                for (int i = 2; i < width; i++)
                {
                    y = 1;
                    x = i;
                    currentPoint = currentOutCirclePoint;
                    currentValue = selectChannel(*(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                    nextValue = 0;
                    currentPoint += step;
                    while (x < width && y < height)
                    {
                        nextValue = selectChannel(*(currentPoint + 2), *(currentPoint + 1), *currentPoint);
                        callback(currentValue, nextValue);
                        currentValue = nextValue;
                        y++;
                        x++;
                        currentPoint += step; count++;
                    }
                    currentOutCirclePoint += 3;
                }
            }
            bitmap.UnlockBits(bitmapData);
        }
    }
}
