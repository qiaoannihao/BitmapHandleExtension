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
using 视觉学习;

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
        public static void SetPiRedChannel(this Bitmap bitmap, int x, int y, byte value)
        {
            var res = bitmap.LockBits(new Rectangle(x, y, 1, 1), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* first = (byte*)res.Scan0;
                *(first + 2) = value;
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
        public static void SetPiGreenChannel(this Bitmap bitmap, int x, int y, byte value)
        {
            var res = bitmap.LockBits(new Rectangle(x, y, 1, 1), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* first = (byte*)res.Scan0;
                *(first + 1) = value;
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
        public static void SetPiBlueChannel(this Bitmap bitmap, int x, int y, byte value)
        {
            var res = bitmap.LockBits(new Rectangle(x, y, 1, 1), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* first = (byte*)res.Scan0;
                *first = value;
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
        public static void SetPiChannel(this Bitmap bitmap, int x, int y, int channel, byte value)
        {
            channel = 2 - channel;
            var res = bitmap.LockBits(new Rectangle(x, y, 1, 1), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* first = (byte*)(res.Scan0 + channel);
                *first = value;
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
        /// 获取多个像素点的颜色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static void GetPi(this Bitmap bitmap, int x, int y, Action<byte, byte, byte> callback)
        {
            var res = bitmap.LockBits(new Rectangle(x, y, 1, 1), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* first = (byte*)res.Scan0;
                callback(*(first + 2), *(first + 1), *first);
            }
            bitmap.UnlockBits(res);
        }

        /// <summary>
        /// 获取或设置像素点的颜色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static void GetSetPix(this Bitmap bitmap, int x, int y, Action<byte, byte, byte, Action<byte, byte, byte>> action)
        {
            var res = bitmap.LockBits(new Rectangle(x, y, 1, 1), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* first = (byte*)res.Scan0;
                action(*(first + 2), *(first + 1), *first, (r, g, b) =>
                {
                    *(first + 2) = r;
                    *(first + 1) = g;
                    *first = b;
                });
            }
            bitmap.UnlockBits(res);
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
        /// 获取或设置多个像素点的颜色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static void GetSetPix(this Bitmap bitmap, Action<Action<int, int, Action<byte, byte, byte>>, Action<int, int, byte, byte, byte>> action)
        {
            action((x, y, callback) => bitmap.GetPi(x, y, callback), (x, y, r, g, b) => bitmap.SetPi(x, y, r, g, b));
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
            rectangle.CheckOutBitmapRangeAndFit(bitmap);
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int stride = bitmapData.Stride;
                byte* currentPoint;
                byte* outCirclePointer = (byte*)bitmapData.Scan0;
                int y, x, xEnd = rectangle.Right, yEnd = rectangle.Bottom;
                for (y = rectangle.Y; y < yEnd; y++)
                {
                    currentPoint = outCirclePointer;
                    for (x = rectangle.X; x < xEnd; x++)
                    {
                        handleFunc(x, y, *(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                        currentPoint += 3;
                    }
                    outCirclePointer += stride;
                }
            }
            bitmap.UnlockBits(bitmapData);
        }
        /// <summary>
        /// 遍历每个像素
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="handleFunc">row column r g b</param>
        public static void PixForeach(this Bitmap bitmap, Rectangle rectangle, Action<int, int, byte> handleFunc)
        {
            rectangle.CheckOutBitmapRangeAndFit(bitmap);
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int stride = bitmapData.Stride;
                byte* currentPoint;
                byte* outCirclePointer = (byte*)bitmapData.Scan0;
                int y, x, xEnd = rectangle.Right, yEnd = rectangle.Bottom;
                for (y = rectangle.Y; y < yEnd; y++)
                {
                    currentPoint = outCirclePointer;
                    for (x = rectangle.X; x < xEnd; x++)
                    {
                        handleFunc(x, y, *(currentPoint + 2));
                        handleFunc(x, y, *(currentPoint + 1));
                        handleFunc(x, y, *currentPoint);
                        currentPoint += 3;
                    }
                    outCirclePointer += stride;
                }
            }
            bitmap.UnlockBits(bitmapData);
        }
        /// <summary>
        /// 遍历每个像素
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="handleFunc">row column r g b</param>
        public static void PixForeach(this Bitmap bitmap, int channelIndex, Rectangle rectangle, Action<int, int, byte> handleFunc)
        {
            rectangle.CheckOutBitmapRangeAndFit(bitmap);
            channelIndex = 2 - channelIndex;
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int stride = bitmapData.Stride;
                byte* currentPoint;
                byte* outCirclePointer = (byte*)bitmapData.Scan0;
                int y, x, xEnd = rectangle.Right, yEnd = rectangle.Bottom;
                for (y = rectangle.Y; y < yEnd; y++)
                {
                    currentPoint = outCirclePointer;
                    for (x = rectangle.X; x < xEnd; x++)
                    {
                        handleFunc(x, y, *(currentPoint + channelIndex));
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
            rectangle.CheckOutBitmapRangeAndFit(bitmap);
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int stride = bitmapData.Stride;
                byte* outCirclePointer = (byte*)bitmapData.Scan0;
                byte* currentPoint = null;
                Action<byte, byte, byte> action = (r, g, b) =>
                {
                    *(currentPoint + 2) = r;
                    *(currentPoint + 1) = g;
                    *(currentPoint) = b;
                };
                int y, x, xEnd = rectangle.Right, yEnd = rectangle.Bottom;
                for (y = rectangle.Y; y < yEnd; y++)
                {
                    currentPoint = outCirclePointer;
                    for (x = rectangle.X; x < xEnd; x++)
                    {
                        handleFunc(
                            x, y,
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
            Rectangle rectangle,
            int leftWidth, int rightWidth, int topHeight, int bottomHeight,
            Func<byte> fillValueFunc,
            Action<int, int, byte, byte, byte, Func<Func<int, int, byte, byte, byte, bool>, bool>, Action<byte, byte, byte>> handleFunc,
            int windowPixLocationType = 0, bool outReturnFlag = true)
        {
            rectangle.CheckOutBitmapRangeAndFit(bitmap);
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                var fillValue = fillValueFunc();
                int stride = bitmapData.Stride;
                byte* outCirclePointer = (byte*)bitmapData.Scan0;
                byte* currentPoint = null, currentPointTmp;

                Action<byte, byte, byte> action = (r, g, b) =>
                {
                    *(currentPoint + 2) = r;
                    *(currentPoint + 1) = g;
                    *(currentPoint) = b;
                };
                int y = 0, yy = 0, x = 0, xx = 0, dx = 0, dy = 0,
                    windowHlafWidthPointerStep, windowHlafHeightPointerStep,
                    currentWidthPointerOffset, currentHeightPointerOffset;
                windowHlafWidthPointerStep = leftWidth * 3;
                windowHlafHeightPointerStep = topHeight * stride;
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
                        return xx + leftWidth;
                    };
                    yyGetter = () =>
                    {
                        return yy + topHeight;
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
                int yEnd = rectangle.Bottom;
                int xEnd = rectangle.Right;
                int xEnd_1 = xEnd - 1;
                int yEnd_1 = yEnd - 1;
                bool yOutRectangleFlag = false;
                Func<Func<int, int, byte, byte, byte, bool>, bool> getter = ss =>
                  {
                      currentHeightPointerOffset = -windowHlafHeightPointerStep;
                      for (yy = -topHeight; yy < bottomHeight + 1; yy++)
                      {
                          dy = y + yy;
                          yOutRectangleFlag = dy < rectangle.Y || dy > yEnd_1;
                          currentWidthPointerOffset = -windowHlafWidthPointerStep;
                          for (xx = -leftWidth; xx < rightWidth + 1; xx++)
                          {
                              dx = x + xx;
                              if (yOutRectangleFlag || dx < rectangle.X || dx > xEnd_1)
                              {
                                  if (outReturnFlag)
                                  {
                                      if (!ss(xxGetter(), yyGetter(), fillValue, fillValue, fillValue))
                                      {
                                          return false;
                                      }
                                  }
                              }
                              else
                              {
                                  currentPointTmp = currentPoint + currentWidthPointerOffset + currentHeightPointerOffset;
                                  if (!ss(xxGetter(), yyGetter(), *(currentPointTmp + 2), *(currentPointTmp + 1), *(currentPointTmp)))
                                  {
                                      return false;
                                  }
                              }
                              currentWidthPointerOffset += 3;
                          }
                          currentHeightPointerOffset += stride;
                      }
                      return true;
                  };

                for (y = rectangle.Y; y < yEnd; y++)
                {
                    currentPoint = outCirclePointer;
                    for (x = rectangle.X; x < xEnd; x++)
                    {
                        handleFunc(x, y,
                            *(currentPoint + 2),
                            *(currentPoint + 1),
                            *currentPoint, getter, action);
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
            Rectangle rectangle,
            int leftWidth, int rightWidth, int topHeight, int bottomHeight,
            Func<byte> fillValueFunc,
            Action<int, int, Action<Action<int, int, byte, byte, byte>>, Action<byte, byte, byte>> handleFunc,
            int windowPixLocationType = 0, bool outReturnFlag = true)
        {
            rectangle.CheckOutBitmapRangeAndFit(bitmap);
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                var fillValue = fillValueFunc();
                int stride = bitmapData.Stride;
                byte* outCirclePointer = (byte*)bitmapData.Scan0;
                byte* currentPoint = null, currentPointTmp;

                Action<byte, byte, byte> action = (r, g, b) =>
                {
                    *(currentPoint + 2) = r;
                    *(currentPoint + 1) = g;
                    *(currentPoint) = b;
                };
                int y = 0, yy = 0, x = 0, xx = 0, dx = 0, dy = 0,
                    windowHlafWidthPointerStep, windowHlafHeightPointerStep,
                    currentWidthPointerOffset, currentHeightPointerOffset;
                windowHlafWidthPointerStep = leftWidth * 3;
                windowHlafHeightPointerStep = topHeight * stride;
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
                        return xx + leftWidth;
                    };
                    yyGetter = () =>
                    {
                        return yy + topHeight;
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
                int yEnd = rectangle.Bottom;
                int xEnd = rectangle.Right;
                int xEnd_1 = xEnd - 1;
                int yEnd_1 = yEnd - 1;
                bool yOutRectangleFlag = false;
                Action<Action<int, int, byte, byte, byte>> getter = ss =>
                {
                    currentHeightPointerOffset = -windowHlafHeightPointerStep;
                    for (yy = -topHeight; yy < bottomHeight + 1; yy++)
                    {
                        dy = y + yy;
                        yOutRectangleFlag = dy < rectangle.Y || dy > yEnd_1;
                        currentWidthPointerOffset = -windowHlafWidthPointerStep;
                        for (xx = -leftWidth; xx < rightWidth + 1; xx++)
                        {
                            dx = x + xx;
                            if (yOutRectangleFlag || dx < rectangle.X || dx > xEnd_1)
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
                };

                for (y = rectangle.Y; y < yEnd; y++)
                {
                    currentPoint = outCirclePointer;
                    for (x = rectangle.X; x < xEnd; x++)
                    {
                        handleFunc(x, y, getter, action);
                        currentPoint += 3;
                    }
                    outCirclePointer += stride;
                }
            }
            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// 处理每个像素 不超出边缘的移窗
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="handleFunc">column row r g b</param>
        public static void HandleImage(this Bitmap bitmap,
            Rectangle rectangle,
            int windowWidth, int windowHeight, int windowMoveStep,
            Action<int, int, Action<Action<int, int, byte, byte, byte>>> handleFunc,
            int windowPixLocationType = 0)
        {
            rectangle.CheckOutBitmapRangeAndFit(bitmap);
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //unsafe
            //{
            //    int stride = bitmapData.Stride;
            //    byte* outCirclePointer = (byte*)bitmapData.Scan0;
            //    byte* currentPoint = null, currentPointTmp;

                
            //    int y = 0, yy = 0, x = 0, xx = 0, dx = 0, dy = 0,
            //        windowHlafWidthPointerStep, windowHlafHeightPointerStep,
            //        currentWidthPointerOffset, currentHeightPointerOffset;
            //    windowHlafWidthPointerStep = leftWidth * 3;
            //    windowHlafHeightPointerStep = topHeight * stride;
            //    Func<int> xxGetter = () =>
            //    {
            //        return xx;
            //    };
            //    Func<int> yyGetter = () =>
            //    {
            //        return yy;
            //    };
            //    if (windowPixLocationType == 1)
            //    {
            //        xxGetter = () =>
            //        {
            //            return xx + leftWidth;
            //        };
            //        yyGetter = () =>
            //        {
            //            return yy + topHeight;
            //        };
            //    }
            //    else if (windowPixLocationType == 2)
            //    {
            //        xxGetter = () =>
            //        {
            //            return dx;
            //        };
            //        yyGetter = () =>
            //        {
            //            return dy;
            //        };
            //    }
            //    int yEnd = rectangle.Bottom;
            //    int xEnd = rectangle.Right;
            //    int xEnd_1 = xEnd - 1;
            //    int yEnd_1 = yEnd - 1;
            //    bool yOutRectangleFlag = false;
            //    Action<Action<int, int, byte, byte, byte>> getter = ss =>
            //    {
                    
            //    };

            //    for (y = rectangle.Y; y < yEnd; y++)
            //    {
            //        currentPoint = outCirclePointer;
            //        for (x = rectangle.X; x < xEnd; x++)
            //        {
            //            handleFunc(x, y, getter, action);
            //            currentPoint += 3;
            //        }
            //        outCirclePointer += stride;
            //    }
            //}
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
            var remainder = windowWidth & 1;
            int leftWidth, rightWidth, topHeight, bottomHeight;
            if (remainder == 0)
            {
                rightWidth = (windowWidth >> 1) + 1;
                leftWidth = rightWidth - 1;
            }
            else
            {
                leftWidth = windowWidth >> 1;
                rightWidth = leftWidth;
            }
            remainder = windowHeight & 1;
            if (remainder == 0)
            {
                bottomHeight = (windowHeight >> 1);
                topHeight = bottomHeight - 1;
            }
            else
            {
                bottomHeight = (windowHeight >> 1);
                topHeight = bottomHeight;
            }
            bitmap.HandleImage(rectangle,
                leftWidth, rightWidth, topHeight, bottomHeight, fillValueFunc, handleFunc, windowPixLocationType, outReturnFlag);
        }
        /// <summary>
        /// 处理每个像素
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="handleFunc">column row r g b</param>
        public static void HandleImage(this Bitmap bitmap,
            Rectangle rectangle,
            Action<int, int, Action<int, int, Action<byte, byte, byte>>, Action<byte, byte, byte>> handleFunc, int windowPixLocationType = 0, bool outReturnFlag = true)
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
                int y = 0, x = 0, xEnd = rectangle.Right, yEnd = rectangle.Bottom;
                int xEnd_1 = xEnd - 1, yEnd_1 = yEnd - 1;
                Action<int, int, Action<byte, byte, byte>> getter = (offsetX, offsetY, callback) =>
                {
                    int xTmp = x + offsetX;
                    if (xTmp < rectangle.X || xTmp > xEnd_1)
                    {
                        if (outReturnFlag)
                        {
                            return;
                        }
                        else
                        {
                            if (xTmp < rectangle.X)
                            {
                                offsetX = rectangle.X - x;
                            }
                            else if (xTmp > xEnd_1)
                            {
                                offsetX = xEnd - x;
                            }
                        }
                    }
                    int yTmp = y + offsetY;
                    if (yTmp < rectangle.Y || yTmp > yEnd_1)
                    {
                        if (outReturnFlag)
                        {
                            return;
                        }
                        else
                        {
                            if (yTmp < rectangle.Y)
                            {
                                offsetY = rectangle.Y - y;
                            }
                            else if (yTmp > xEnd_1)
                            {
                                offsetY = yEnd - y;
                            }
                        }
                    }
                    bool directionX = offsetX > 0;
                    bool directionY = offsetY > 0;
                    int xPointerOffset = 0;
                    int yPointerOffset = 0;
                    Func<int, int> stepRunX = null;
                    if (directionX)
                    {
                        stepRunX = s => s - 1;
                    }
                    else
                    {
                        stepRunX = s => s + 1;
                    }
                    Func<int, int> stepRunY = null;
                    if (directionY)
                    {
                        stepRunY = s => s - 1;
                    }
                    else
                    {
                        stepRunY = s => s + 1;
                    }
                    while (offsetX != 0)
                    {
                        xPointerOffset += 3;
                        offsetX = stepRunX(offsetX);
                    }
                    while (offsetY != 0)
                    {
                        yPointerOffset += stride;
                        offsetY = stepRunY(offsetY);
                    }
                    if (!directionX)
                    {
                        xPointerOffset = -xPointerOffset;
                    }
                    if (!directionY)
                    {
                        yPointerOffset = -yPointerOffset;
                    }
                    byte* sum = currentPoint + xPointerOffset + yPointerOffset;
                    callback(*(sum + 2), *(sum + 1), *sum);
                };
                for (y = rectangle.Y; y < yEnd; y++)
                {
                    currentPoint = outCirclePointer;
                    for (x = rectangle.X; x < xEnd; x++)
                    {
                        handleFunc(x, y, getter, action);
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
            Rectangle rectangle,
            Action<int, int, byte, byte, byte, Action<int, int, Action<byte, byte, byte>>, Action<byte, byte, byte>> handleFunc, bool outReturnFlag = false)
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
                int y = 0, x = 0, xEnd = rectangle.Right, yEnd = rectangle.Bottom;
                int xEnd_1 = xEnd - 1, yEnd_1 = yEnd - 1;
                Action<int, int, Action<byte, byte, byte>> getter = (offsetX, offsetY, callback) =>
                {
                    int xTmp = x + offsetX;
                    if (xTmp < rectangle.X || xTmp > xEnd_1)
                    {
                        if (outReturnFlag)
                        {
                            return;
                        }
                        else
                        {
                            if (xTmp < rectangle.X)
                            {
                                offsetX = rectangle.X - x;
                            }
                            else if (xTmp > xEnd_1)
                            {
                                offsetX = xEnd - x;
                            }
                        }
                    }
                    int yTmp = y + offsetY;
                    if (yTmp < rectangle.Y || yTmp > yEnd_1)
                    {
                        if (outReturnFlag)
                        {
                            return;
                        }
                        else
                        {
                            if (yTmp < rectangle.Y)
                            {
                                offsetY = rectangle.Y - y;
                            }
                            else if (yTmp > xEnd_1)
                            {
                                offsetY = yEnd - y;
                            }
                        }
                    }
                    bool directionX = offsetX > 0;
                    bool directionY = offsetY > 0;
                    int xPointerOffset = 0;
                    int yPointerOffset = 0;
                    Func<int, int> stepRunX = null;
                    if (directionX)
                    {
                        stepRunX = s => s - 1;
                    }
                    else
                    {
                        stepRunX = s => s + 1;
                    }
                    Func<int, int> stepRunY = null;
                    if (directionY)
                    {
                        stepRunY = s => s - 1;
                    }
                    else
                    {
                        stepRunY = s => s + 1;
                    }
                    while (offsetX != 0)
                    {
                        xPointerOffset += 3;
                        offsetX = stepRunX(offsetX);
                    }
                    while (offsetY != 0)
                    {
                        yPointerOffset += stride;
                        offsetY = stepRunY(offsetY);
                    }
                    if (!directionX)
                    {
                        xPointerOffset = -xPointerOffset;
                    }
                    if (!directionY)
                    {
                        yPointerOffset = -yPointerOffset;
                    }
                    byte* sum = currentPoint + xPointerOffset + yPointerOffset;
                    callback(*(sum + 2), *(sum + 1), *sum);
                };
                for (y = rectangle.Y; y < yEnd; y++)
                {
                    currentPoint = outCirclePointer;
                    for (x = rectangle.X; x < xEnd; x++)
                    {
                        handleFunc(x, y, *(currentPoint + 2), *(currentPoint + 1), *currentPoint, getter, action);
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
            Rectangle rectangle,
            Action<int, int, byte, byte, byte, Action<int, int, Action<byte, byte, byte>>, Action<int, int, byte, byte, byte>> handleFunc, bool outReturnFlag = false)
        {
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int width = bitmapData.Width;
                int height = bitmapData.Height;
                int stride = bitmapData.Stride;
                byte* outCirclePointer = (byte*)bitmapData.Scan0;
                byte* currentPoint = null;

                int y = 0, x = 0, xEnd = rectangle.Right, yEnd = rectangle.Bottom;
                int xEnd_1 = xEnd - 1, yEnd_1 = yEnd - 1;
                Action<int, int, byte, byte, byte> action = (offsetX, offsetY, r, g, b) =>
                  {
                      int xTmp = x + offsetX;
                      if (xTmp < rectangle.X || xTmp > xEnd_1)
                      {
                          return;
                      }
                      int yTmp = y + offsetY;
                      if (yTmp < rectangle.Y || yTmp > yEnd_1)
                      {
                          return;
                      }
                      bool directionX = offsetX > 0;
                      bool directionY = offsetY > 0;
                      int xPointerOffset = 0;
                      int yPointerOffset = 0;
                      Func<int, int> stepRunX = null;
                      if (directionX)
                      {
                          stepRunX = s => s - 1;
                      }
                      else
                      {
                          stepRunX = s => s + 1;
                      }
                      Func<int, int> stepRunY = null;
                      if (directionY)
                      {
                          stepRunY = s => s - 1;
                      }
                      else
                      {
                          stepRunY = s => s + 1;
                      }
                      while (offsetX != 0)
                      {
                          xPointerOffset += 3;
                          offsetX = stepRunX(offsetX);
                      }
                      while (offsetY != 0)
                      {
                          yPointerOffset += stride;
                          offsetY = stepRunY(offsetY);
                      }
                      if (!directionX)
                      {
                          xPointerOffset = -xPointerOffset;
                      }
                      if (!directionY)
                      {
                          yPointerOffset = -yPointerOffset;
                      }
                      *(currentPoint + 2) = r;
                      *(currentPoint + 1) = g;
                      *(currentPoint) = b;
                  };
                Action<int, int, Action<byte, byte, byte>> getter = (offsetX, offsetY, callback) =>
                {
                    int xTmp = x + offsetX;
                    if (xTmp < rectangle.X || xTmp > xEnd_1)
                    {
                        if (outReturnFlag)
                        {
                            return;
                        }
                        if (xTmp < rectangle.X)
                        {
                            offsetX = rectangle.X - x;
                        }
                        else if (xTmp > xEnd_1)
                        {
                            offsetX = xEnd - x;
                        }
                    }
                    int yTmp = y + offsetY;
                    if (yTmp < rectangle.Y || yTmp > yEnd_1)
                    {
                        if (outReturnFlag)
                        {
                            return;
                        }
                        if (yTmp < rectangle.Y)
                        {
                            offsetY = rectangle.Y - y;
                        }
                        else if (yTmp > xEnd_1)
                        {
                            offsetY = yEnd - y;
                        }
                    }
                    bool directionX = offsetX > 0;
                    bool directionY = offsetY > 0;
                    int xPointerOffset = 0;
                    int yPointerOffset = 0;
                    Func<int, int> stepRunX = null;
                    if (directionX)
                    {
                        stepRunX = s => s - 1;
                    }
                    else
                    {
                        stepRunX = s => s + 1;
                    }
                    Func<int, int> stepRunY = null;
                    if (directionY)
                    {
                        stepRunY = s => s - 1;
                    }
                    else
                    {
                        stepRunY = s => s + 1;
                    }
                    while (offsetX != 0)
                    {
                        xPointerOffset += 3;
                        offsetX = stepRunX(offsetX);
                    }
                    while (offsetY != 0)
                    {
                        yPointerOffset += stride;
                        offsetY = stepRunY(offsetY);
                    }
                    if (!directionX)
                    {
                        xPointerOffset = -xPointerOffset;
                    }
                    if (!directionY)
                    {
                        yPointerOffset = -yPointerOffset;
                    }
                    byte* sum = currentPoint + xPointerOffset + yPointerOffset;
                    callback(*(sum + 2), *(sum + 1), *sum);
                };
                for (y = rectangle.Y; y < yEnd; y++)
                {
                    currentPoint = outCirclePointer;
                    for (x = rectangle.X; x < xEnd; x++)
                    {
                        handleFunc(x, y, *(currentPoint + 2), *(currentPoint + 1), *currentPoint, getter, action);
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
            if (rectangle.X < 0)
            {
                rectangle.X = 0;
            }
            if (rectangle.Y < 0)
            {
                rectangle.Y = 0;
            }
            if (rectangle.Right > bitmap.Width)
            {
                rectangle.Width = bitmap.Width - rectangle.X;
            }
            if (rectangle.Bottom > bitmap.Height)
            {
                rectangle.Height = bitmap.Height - rectangle.Y;
            }
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int stride = bitmapData.Stride;
                byte* outCirclePointer = (byte*)bitmapData.Scan0;
                byte* currentPoint = null;
                Action<byte, byte, byte> action = (r, g, b) =>
                {
                    *(currentPoint + 2) = r;
                    *(currentPoint + 1) = g;
                    *(currentPoint) = b;
                };
                int y, x, xEnd = rectangle.Right, yEnd = rectangle.Bottom;
                for (y = rectangle.Y; y < yEnd; y++)
                {
                    currentPoint = outCirclePointer;
                    for (x = rectangle.X; x < xEnd; x++)
                    {
                        handleFunc(
                            x, y,
                            action);
                        currentPoint += 3;
                    }
                    outCirclePointer += stride;
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
        public static void RowForeach<T>(this Bitmap bitmap, bool isPositive, Rectangle rectangle, Func<byte, byte, byte, T> rgbHandle, Func<T, byte> setter, Action<int, T[], Action> oneRowEvt)
        {
            if (rectangle.X < 0)
            {
                rectangle.X = 0;
            }
            if (rectangle.Y < 0)
            {
                rectangle.Y = 0;
            }
            if (rectangle.Right > bitmap.Width)
            {
                rectangle.Width = bitmap.Width - rectangle.X;
            }
            if (rectangle.Bottom > bitmap.Height)
            {
                rectangle.Height = bitmap.Height - rectangle.Y;
            }
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int width = bitmapData.Width;
                int height = bitmapData.Height;
                int stride = bitmapData.Stride;
                byte* san0 = (byte*)bitmapData.Scan0;
                byte* currentPoint = null;
                byte* outCirclePointer = null;
                T[] buffer = null;
                int y, x, yEnd;
                Action update = () =>
                {
                    byte* updatePointer = outCirclePointer;
                    for (int i = 0; i < width; i++)
                    {
                        *(updatePointer + 2) = setter(buffer[i]);
                        updatePointer += 3;
                    }
                };

                if (isPositive)
                {
                    yEnd = rectangle.Bottom;
                    outCirclePointer = san0;
                    for (y = rectangle.Y; y < yEnd; y++)
                    {
                        buffer = new T[width];
                        currentPoint = outCirclePointer;
                        for (x = 0; x < width; x++)
                        {
                            buffer[x] = rgbHandle(*(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                            currentPoint += 3;
                        }
                        oneRowEvt(y, buffer, update);
                        outCirclePointer += stride;
                    }
                }
                else
                {
                    outCirclePointer = san0 + (stride * (height - 1));
                    yEnd = rectangle.Y - 1;
                    for (y = height - 1; y > yEnd; y--)
                    {
                        buffer = new T[width];
                        currentPoint = outCirclePointer;
                        for (x = 0; x < width; x++)
                        {
                            buffer[x] = rgbHandle(*(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                            currentPoint += 3;
                        }
                        oneRowEvt(y, buffer, update);
                        outCirclePointer -= stride;
                    }
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
        public static void RowForeach(this Bitmap bitmap, bool isPositive, Rectangle rectangle, Func<byte, byte, byte, byte> rgbHandle, Action<int, byte[], Action> oneRowEvt)
        {
            if (rectangle.X < 0)
            {
                rectangle.X = 0;
            }
            if (rectangle.Y < 0)
            {
                rectangle.Y = 0;
            }
            if (rectangle.Right > bitmap.Width)
            {
                rectangle.Width = bitmap.Width - rectangle.X;
            }
            if (rectangle.Bottom > bitmap.Height)
            {
                rectangle.Height = bitmap.Height - rectangle.Y;
            }
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
                int y, x, yEnd;
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
                    yEnd = rectangle.Bottom;
                    outCirclePointer = san0;
                    for (y = rectangle.Y; y < yEnd; y++)
                    {
                        buffer = new byte[width];
                        currentPoint = outCirclePointer;
                        for (x = 0; x < width; x++)
                        {
                            buffer[x] = rgbHandle(*(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                            currentPoint += 3;
                        }
                        oneRowEvt(y, buffer, update);
                        outCirclePointer += stride;
                    }
                }
                else
                {
                    outCirclePointer = san0 + (stride * (height - 1));
                    yEnd = rectangle.Y - 1;
                    for (y = height - 1; y > yEnd; y--)
                    {
                        buffer = new byte[width];
                        currentPoint = outCirclePointer;
                        for (x = 0; x < width; x++)
                        {
                            buffer[x] = rgbHandle(*(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                            currentPoint += 3;
                        }
                        oneRowEvt(y, buffer, update);
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
                int y, x, xEnd;
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
                    xEnd = rectangle.Right;
                    outCirclePointer = san0;
                    for (x = rectangle.X; x < xEnd; x++)
                    {
                        buffer = new byte[height];
                        currentPoint = outCirclePointer;
                        for (y = 0; y < height; y++)
                        {
                            buffer[y] = rgbHandle(*(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                            currentPoint += stride;
                        }
                        oneColumnEvt(x, buffer, update);
                        outCirclePointer += 3;
                    }
                }
                else
                {
                    outCirclePointer = san0 + (width - 1) * 3;
                    xEnd = rectangle.X - 1;
                    for (x = width - 1; x > xEnd; x--)
                    {
                        buffer = new byte[height];
                        currentPoint = outCirclePointer;
                        for (y = 0; y < height; y++)
                        {
                            buffer[y] = rgbHandle(*(currentPoint + 2), *(currentPoint + 1), *(currentPoint));
                            currentPoint += stride;
                        }
                        oneColumnEvt(x, buffer, update);
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
            if (rectangle.X < 0)
            {
                rectangle.X = 0;
            }
            if (rectangle.Y < 0)
            {
                rectangle.Y = 0;
            }
            if (rectangle.X > bitmap.Width - 1)
            {
                rectangle.Width = bitmap.Width - rectangle.X;
            }
            if (rectangle.Y > bitmap.Height - 1)
            {
                rectangle.Height = bitmap.Height - rectangle.Y;
            }
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

        /// <summary>
        /// 处理相同大小不同图
        /// </summary>
        /// <param name="bitmap1"></param>
        /// <param name="bitmap2"></param>
        /// <param name="rectangle"></param>
        /// <param name="handleFunc">column row r1 g1 b1 r2 g2 b2 setter1 setter2</param>
        public static void HandleTwoImage(this Bitmap bitmap1, Bitmap bitmap2, Rectangle rectangle,
            Action<int, int, byte, byte, byte, byte, byte, byte, Action<byte, byte, byte>, Action<byte, byte, byte>> handleFunc)
        {
            if (rectangle.X < 0)
            {
                rectangle.X = 0;
            }
            if (rectangle.Y < 0)
            {
                rectangle.Y = 0;
            }
            if (rectangle.Right > bitmap1.Width)
            {
                rectangle.Width = bitmap1.Width - rectangle.X;
            }
            if (rectangle.Bottom > bitmap1.Height)
            {
                rectangle.Height = bitmap1.Height - rectangle.Y;
            }
            BitmapData bitmapData1 = bitmap1.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bitmapData2 = bitmap2.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int stride = bitmapData1.Stride;
                byte* outCirclePointer1 = (byte*)bitmapData1.Scan0;
                byte* currentPoint1 = null;
                byte* outCirclePointer2 = (byte*)bitmapData2.Scan0;
                byte* currentPoint2 = null;
                Action<byte, byte, byte> action1 = (r, g, b) =>
                {
                    *(currentPoint1 + 2) = r;
                    *(currentPoint1 + 1) = g;
                    *(currentPoint1) = b;
                };
                Action<byte, byte, byte> action2 = (r, g, b) =>
                {
                    *(currentPoint2 + 2) = r;
                    *(currentPoint2 + 1) = g;
                    *(currentPoint2) = b;
                };
                int x, y, xEnd = rectangle.Right, yEnd = rectangle.Bottom;
                for (y = rectangle.Y; y < yEnd; y++)
                {
                    currentPoint1 = outCirclePointer1;
                    currentPoint2 = outCirclePointer2;
                    for (x = rectangle.X; x < rectangle.Right; x++)
                    {
                        handleFunc(
                            x, y,
                            *(currentPoint1 + 2), *(currentPoint1 + 1), *(currentPoint1),
                            *(currentPoint2 + 2), *(currentPoint2 + 1), *(currentPoint2),
                            action1, action2);
                        currentPoint1 += 3;
                        currentPoint2 += 3;
                    }
                    outCirclePointer1 += stride;
                    outCirclePointer2 += stride;
                }
            }
            bitmap1.UnlockBits(bitmapData1);
            bitmap2.UnlockBits(bitmapData2);
        }
        /// <summary>
        /// 加
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="bitmap1"></param>
        /// <param name="rectangle"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Bitmap Add(this Bitmap bitmap, Bitmap bitmap1, Rectangle rectangle, int offset)
        {
            bitmap.HandleTwoImage(bitmap1, rectangle, (x, y, r1, g1, b1, r2, g2, b2, setter1, setter2) =>
            {
                setter1((byte)((r1 + r2 + offset).FitRange(0, 255)),
                    (byte)((g1 + g2 + offset).FitRange(0, 255)),
                    (byte)((b1 + b2 + offset).FitRange(0, 255)));
            });
            return bitmap;
        }
        /// <summary>
        /// 减
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="bitmap1"></param>
        /// <param name="rectangle"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Bitmap Sub(this Bitmap bitmap, Bitmap bitmap1, Rectangle rectangle, int offset)
        {
            bitmap.HandleTwoImage(bitmap1, rectangle, (x, y, r1, g1, b1, r2, g2, b2, setter1, setter2) =>
            {
                setter1((byte)((r1 - r2 + offset).FitRange(0, 255)),
                    (byte)((g1 - g2 + offset).FitRange(0, 255)),
                    (byte)((b1 - b2 + offset).FitRange(0, 255)));
            });
            return bitmap;
        }

        /// <summary>
        /// 模板匹配
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="handleFunc">column row r g b</param>
        public static void TemplateMatching(this Bitmap bitmap,
            Rectangle rectangle,
            Bitmap template,
            Func<byte> fillValueFunc,
            Action<int, int, Action<Action<int, int, byte, byte, byte, byte, byte, byte>>, Action<byte, byte, byte>> handleFunc,
            int windowPixLocationType = 0, bool outReturnFlag = true)
        {
            if (rectangle.X < 0)
            {
                rectangle.X = 0;
            }
            if (rectangle.Y < 0)
            {
                rectangle.Y = 0;
            }
            if (rectangle.Right > bitmap.Width)
            {
                rectangle.Width = bitmap.Width - rectangle.X;
            }
            if (rectangle.Bottom > bitmap.Height)
            {
                rectangle.Height = bitmap.Height - rectangle.Y;
            }
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            var templateBitmapData = template.LockBits(new Rectangle(0, 0, template.Width, template.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                var remainder = template.Width & 1;
                int leftWidth, rightWidth, topHeight, bottomHeight;
                if (remainder == 0)
                {
                    rightWidth = (template.Width >> 1);
                    leftWidth = rightWidth - 1;
                }
                else
                {
                    leftWidth = template.Width >> 1;
                    rightWidth = leftWidth;
                }
                remainder = template.Height & 1;
                if (remainder == 0)
                {
                    bottomHeight = (template.Height >> 1);
                    topHeight = bottomHeight - 1;
                }
                else
                {
                    bottomHeight = (template.Height >> 1);
                    topHeight = bottomHeight;
                }
                var fillValue = fillValueFunc();
                int stride1 = bitmapData.Stride;
                int stride2 = templateBitmapData.Stride;
                byte* outCirclePointer1 = (byte*)bitmapData.Scan0;
                byte* currentPoint1 = null, currentPointTmp1;
                byte* outCirclePointer2 = (byte*)templateBitmapData.Scan0;
                byte* currentPoint2 = null;

                Action<byte, byte, byte> action = (r, g, b) =>
                {
                    *(currentPoint1 + 2) = r;
                    *(currentPoint1 + 1) = g;
                    *(currentPoint1) = b;
                };
                int y = 0, yy = 0, x = 0, xx = 0, dx = 0, dy = 0,
                    windowHlafWidthPointerStep, windowHlafHeightPointerStep,
                    currentWidthPointerOffset, currentHeightPointerOffset;
                windowHlafWidthPointerStep = leftWidth * 3;
                windowHlafHeightPointerStep = topHeight * stride1;
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
                        return xx + leftWidth;
                    };
                    yyGetter = () =>
                    {
                        return yy + topHeight;
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
                int yEnd = rectangle.Bottom;
                int xEnd = rectangle.Right;
                int xEnd_1 = xEnd - 1;
                int yEnd_1 = yEnd - 1;
                bool yOutRectangleFlag = false;
                Action<Action<int, int, byte, byte, byte, byte, byte, byte>> getter = ss =>
                {
                    currentHeightPointerOffset = -windowHlafHeightPointerStep;
                    outCirclePointer2 = (byte*)templateBitmapData.Scan0;
                    for (yy = -topHeight; yy < bottomHeight + 1; yy++)
                    {
                        dy = y + yy;
                        yOutRectangleFlag = dy < rectangle.Y || dy > yEnd_1;
                        currentWidthPointerOffset = -windowHlafWidthPointerStep;
                        currentPoint2 = outCirclePointer2;
                        for (xx = -leftWidth; xx < rightWidth + 1; xx++)
                        {
                            dx = x + xx;
                            if (yOutRectangleFlag || dx < rectangle.X || dx > xEnd_1)
                            {
                                if (outReturnFlag)
                                {
                                    ss(xxGetter(), yyGetter(),
                                     fillValue, fillValue, fillValue,
                                     *(currentPoint2 + 2), *(currentPoint2 + 1), *(currentPoint2));
                                }
                            }
                            else
                            {
                                currentPointTmp1 = currentPoint1 + currentWidthPointerOffset + currentHeightPointerOffset;
                                ss(xxGetter(), yyGetter(),
                                    *(currentPointTmp1 + 2), *(currentPointTmp1 + 1), *(currentPointTmp1),
                                    *(currentPoint2 + 2), *(currentPoint2 + 1), *(currentPoint2));
                            }
                            currentPoint2 += 3;
                            currentWidthPointerOffset += 3;
                        }
                        currentHeightPointerOffset += stride1;
                        outCirclePointer2 += stride2;
                    }
                };

                for (y = rectangle.Y; y < yEnd; y++)
                {
                    currentPoint1 = outCirclePointer1;
                    for (x = rectangle.X; x < xEnd; x++)
                    {
                        handleFunc(x, y, getter, action);
                        currentPoint1 += 3;
                    }
                    outCirclePointer1 += stride1;
                }
            }
            bitmap.UnlockBits(bitmapData);
        }


    }

}
