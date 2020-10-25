using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace 视觉学习
{
    public static class BitmapHandleExtension
    {
        public static byte RgbToGray(byte r, byte g, byte b)
        {
            return (byte)(0.2126 * r + 0.7152 * g + 0.0722 * b);
        }
        /// <summary>
        /// 通道调换
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        /// <param name="orderCallback"></param>
        public static void ChangeChannelOrder(this Bitmap bitmap, Rectangle rectangle, Action<byte, byte, byte, Action<byte, byte, byte>> orderCallback)
        {
            bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
            {
                orderCallback(r, g, b, setter);
            });
        }
        /// <summary>
        /// 彩色转灰度
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        public static void Grayscale(this Bitmap bitmap, Rectangle rectangle)
        {
            bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
            {
                byte tmp = RgbToGray(r, g, b);
                setter(tmp, tmp, tmp);
            });
        }
        /// <summary>
        /// 二值化阈值
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        /// <param name="value"></param>
        public static void Thresholding(this Bitmap bitmap, Rectangle rectangle, int value)
        {
            byte tmp;
            bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
            {
                tmp = RgbToGray(r, g, b);
                if (tmp < value)
                {
                    setter(0, 0, 0);
                }
                else
                {
                    setter(255, 255, 255);
                }
            });
        }
        /// <summary>
        /// 大津二值化
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        /// <param name="callback"></param>
        public static void OtsusMethod(this Bitmap bitmap, Rectangle rectangle, Action<int> callback)
        {
            double maxSb = 0;
            int maxIndex = 0;
            double sb = 0;
            double w0 = 0;
            double w1 = 0;
            int m0 = 0;
            int m1 = 0;
            int count = bitmap.Width * bitmap.Height;
            for (int i = 0; i < 256; i++)
            {
                m0 = 0;
                m1 = 0;
                w0 = 0;
                w1 = 0;
                bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
                {
                    byte tmp = RgbToGray(r, g, b);
                    if (tmp < i)
                    {
                        w0++;
                        m0 += tmp;
                    }
                    else
                    {
                        m1 += tmp;
                    }
                });
                w1 = count - w0;
                var diffM = m0 / w0 - m1 / w1;
                w0 = w0 / count;
                w1 = 1 - w0;
                sb = diffM * diffM * w0 * w1;
                if (sb > maxSb)
                {
                    maxSb = sb;
                    maxIndex = i;
                }
            }
            bitmap.Thresholding(rectangle, maxIndex);
            callback(maxIndex);
        }
        /// <summary>
        /// RGB转HSV
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="callback"></param>
        public static void RgbToHSV(byte r, byte g, byte b, Action<double, double, double> callback)
        {
            byte max = r;
            byte min = r;
            if (g > max)
            {
                max = g;
            }
            if (b > max)
            {
                max = b;
            }
            if (g < min)
            {
                min = g;
            }
            if (b < min)
            {
                min = b;
            }
            double _60 = 60.0;
            double h = 0;
            int s = max - min;
            if (min == b)
            {
                h = (g - r) * _60 / s + 60;
            }
            else if (min == r)
            {
                h = (b - g) * _60 / s + 180;
            }
            else if (min == g)
            {
                h = (r - b) * _60 / s + 300;
            }
            callback(h, s * 1.0 / 255, max * 1.0 / 255);
        }
        /// <summary>
        /// HSV转RGB
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <param name="callback"></param>
        public static void HSVToRgb(double h, double s, double v, Action<byte, byte, byte> callback)
        {
            double hh = ((h % 360) / 60);
            double x = s * (1 - Math.Abs(hh % 2 - 1));
            double diffV_C = v - s;
            double r = diffV_C;
            double g = diffV_C;
            double b = diffV_C;
            if (hh >= 0 && hh < 1)
            {
                r += s;
                g += x;
            }
            else if (hh < 2)
            {
                r += x;
                g += s;
            }
            else if (hh < 3)
            {
                g += s;
                b += x;
            }
            else if (hh < 4)
            {
                g += x;
                b += s;
            }
            else if (hh < 5)
            {
                r += x;
                b += s;
            }
            else if (hh < 6)
            {
                r += s;
                b += x;
            }
            callback((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }
        /// <summary>
        /// 反色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        public static void Inverse(this Bitmap bitmap, Rectangle rectangle)
        {
            bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
            {
                RgbToHSV(r, g, b, (h, s, v) =>
                {
                    HSVToRgb(h + 180, s, v, (_r, _g, _b) =>
                    {
                        setter(_r, _g, _b);
                    });
                });
            });
        }
        /// <summary>
        /// 减色处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte LoseLustre(byte value)
        {
            if (value < 64)
            {
                return 32;
            }
            else if (value < 128)
            {
                return 96;
            }
            else if (value < 192)
            {
                return 160;
            }
            else
            {
                return 224;
            }
        }
        /// <summary>
        /// 减色处理
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        public static void LoseLustre(this Bitmap bitmap, Rectangle rectangle)
        {
            bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
             {
                 setter(LoseLustre(r), LoseLustre(g), LoseLustre(b));
             });
        }
        /// <summary>
        /// 指定区域平均池化
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        public static void Average(this Bitmap bitmap, Rectangle rectangle)
        {
            int rSum = 0;
            int gSum = 0;
            int bSum = 0;
            bitmap.PixForeach(rectangle, (x, y, r, g, b) =>
            {
                rSum += r;
                gSum += g;
                bSum += b;
            });
            int totalCount = rectangle.Width * rectangle.Height;
            rSum /= totalCount;
            gSum /= totalCount;
            bSum /= totalCount;
            byte rAverage = (byte)rSum;
            byte gAverage = (byte)gSum;
            byte bAverage = (byte)bSum;
            bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
            {
                setter(rAverage, gAverage, bAverage);
            });
        }
        public static void AveragePooling(this Bitmap bitmap, Rectangle rectangle, int poolSize)
        {
            AveragePooling(bitmap, rectangle, poolSize, poolSize);
        }
        public static void AveragePooling(this Bitmap bitmap, Rectangle rectangle, int poolWidth, int poolHeight)
        {
            MoveWindowFunction.ImageSegmentation(rectangle, poolWidth, poolHeight, area =>
            {
                bitmap.Average(area);
            });
        }
        public static void Max(this Bitmap bitmap, Rectangle rectangle)
        {
            byte rMax = 0;
            byte gMax = 0;
            byte bMax = 0;
            bitmap.PixForeach(rectangle, (x, y, r, g, b) =>
            {
                if (r > rMax)
                {
                    rMax = r;
                }
                if (g > gMax)
                {
                    gMax = g;
                }
                if (b > bMax)
                {
                    bMax = b;
                }
            });
            bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
            {
                setter(rMax, gMax, bMax);
            });
        }
        public static void MaxPooling(this Bitmap bitmap, Rectangle rectangle, int poolWidth, int poolHeight)
        {
            MoveWindowFunction.ImageSegmentation(rectangle, poolWidth, poolHeight, area =>
            {
                bitmap.Max(area);
            });
        }
        public static void GaussianFilter(this Bitmap bitmap, Rectangle rectangle, int windowWidth, int windowHeight, double sigma)
        {
            double rSum = 0;
            double gSum = 0;
            double bSum = 0;
            double value = 0;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var coefficient = MathFunction.GaussianDistribution(windowWidth, windowHeight, sigma);
            newBitmap.HandleImage(rectangle, windowWidth, windowHeight, () => 0, (x, y, getter, setter) =>
            {
                rSum = 0;
                gSum = 0;
                bSum = 0;
                getter((xx, yy, r, g, b) =>
                {
                    value = coefficient[yy][xx];
                    rSum += (value * r);
                    gSum += (value * g);
                    bSum += (value * b);
                });
                bitmap.SetPi(x, y, (byte)rSum, (byte)gSum, (byte)bSum);
            }, 1, false);

        }
        public static void MedianFilter(this Bitmap bitmap, Rectangle rectangle, int windowWidth, int windowHeight)
        {
            int index = 0;
            byte[] rBuffer = new byte[windowHeight * windowWidth];
            byte[] gBuffer = new byte[rBuffer.Length];
            byte[] bBuffer = new byte[rBuffer.Length];
            int half = rBuffer.Length >> 1;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(rectangle, windowWidth, windowHeight, () => 0, (x, y, getter, setter) =>
            {
                index = 0;
                getter((xx, yy, r, g, b) =>
                    {
                        rBuffer[index] = r;
                        gBuffer[index] = g;
                        bBuffer[index] = b;
                        index++;
                    });
                bitmap.SetPi(x, y, rBuffer.FindMedianChangeSelf(half, 0, rBuffer.Length - 1, (s, s1) => s > s1),
                    gBuffer.FindMedianChangeSelf(half, 0, gBuffer.Length - 1, (s, s1) => s > s1),
                    bBuffer.FindMedianChangeSelf(half, 0, bBuffer.Length - 1, (s, s1) => s > s1));
            }, 1);
        }
        public static void MeanFilter(this Bitmap bitmap, Rectangle rectangle, int windowWidth, int windowHeight)
        {
            int count = windowHeight * windowWidth;
            int rSum = 0;
            int gSum = 0;
            int bSum = 0;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(rectangle, windowWidth, windowHeight, () => 0, (x, y, getter, setter) =>
            {
                rSum = 0;
                gSum = 0;
                bSum = 0;
                getter((xx, yy, r, g, b) =>
                {
                    rSum += r;
                    gSum += g;
                    bSum += b;
                });
                bitmap.SetPi(x, y, (byte)(rSum / count), (byte)(gSum / count), (byte)(bSum / count));
            }, 0, false);
        }
        public static void DiagonalFilter(this Bitmap bitmap, Rectangle rectangle, int size)
        {
            int rSum = 0;
            int gSum = 0;
            int bSum = 0;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(rectangle, size, size, () => 0, (x, y, getter, setter) =>
            {
                rSum = 0;
                gSum = 0;
                bSum = 0;
                getter((xx, yy, r, g, b) =>
                {
                    if (xx == yy)
                    {
                        rSum += r;
                        gSum += g;
                        bSum += b;
                    }
                });
                bitmap.SetPi(x, y, (byte)(rSum / size), (byte)(gSum / size), (byte)(bSum / size));
            }, 0);
        }
        public static void MaxMinFilter(this Bitmap bitmap, Rectangle rectangle, int windowWidth, int windowHeight)
        {
            byte max = byte.MinValue; byte min = byte.MaxValue;
            byte gray = 0;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(rectangle, windowWidth, windowHeight, () => 0, (x, y, getter, setter) =>
            {
                max = byte.MinValue; min = byte.MaxValue;
                getter((xx, yy, r, g, b) =>
                {
                    gray = RgbToGray(r, g, b);
                    if (gray > max)
                    {
                        max = gray;
                    }
                    else if (gray < min)
                    {
                        min = gray;
                    }
                });
                gray = (byte)(max - min);
                bitmap.SetPi(x, y, gray, gray, gray);
            }, 0, false);
        }
        public static void DifferentialVFilter(this Bitmap bitmap, Rectangle rectangle, int windowWidth, int windowHeight)
        {
            byte value = 0, value1 = 0;
            int diff = 0;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(rectangle, windowWidth, windowHeight, () => 0, (x, y, getter, setter) =>
            {
                value = 0; value1 = 0;
                getter((xx, yy, r, g, b) =>
                {
                    if (xx == x && yy == y)
                    {
                        value = RgbToGray(r, g, b);
                    }
                    else if (xx == x && yy == y - 1)
                    {
                        value1 = RgbToGray(r, g, b);
                    }
                });
                diff = value - value1;
                if (diff < 0)
                {
                    diff = 0;
                }
                else if (diff > 255)
                {
                    diff = 255;
                }
                bitmap.SetPi(x, y, (byte)diff, (byte)diff, (byte)diff);
            }, 2);
        }
        public static void DifferentialHFilter(this Bitmap bitmap, Rectangle rectangle, int windowWidth, int windowHeight)
        {
            byte value = 0, value1 = 0;
            int diff = 0;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(rectangle, windowWidth, windowHeight, () => 0, (x, y, getter, setter) =>
            {
                value = 0; value1 = 0;
                getter((xx, yy, r, g, b) =>
                {
                    if (xx == x && yy == y)
                    {
                        value = RgbToGray(r, g, b);
                    }
                    else if (xx == x - 1 && yy == y)
                    {
                        value1 = RgbToGray(r, g, b);
                    }
                });
                diff = value - value1;
                if (diff < 0)
                {
                    diff = 0;
                }
                else if (diff > 255)
                {
                    diff = 255;
                }
                bitmap.SetPi(x, y, (byte)diff, (byte)diff, (byte)diff);
            }, 2);
        }
        public static void SobelVFilter(this Bitmap bitmap, Rectangle rectangle)
        {
            int[][] coefficient = new int[][]
            {
                new int[]{1,2,1 },
                new int[]{0,0,0 },
                new int[]{-1,-2,-1 },
            };
            int sum = 0;
            byte value;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(rectangle, 3, 3, () => 0, (x, y, getter, setter) =>
            {
                sum = 0;
                getter((xx, yy, r, g, b) =>
                    {
                        value = RgbToGray(r, g, b);
                        sum += (coefficient[yy][xx] * value);
                    });
                if (sum < 0)
                {
                    sum = 0;
                }
                else if (sum > 255)
                {
                    sum = 255;
                }
                bitmap.SetPi(x, y, (byte)sum, (byte)sum, (byte)sum);
            }, 1, false);
        }
        public static void SobelHFilter(this Bitmap bitmap, Rectangle rectangle)
        {
            int[][] coefficient = new int[][]
           {
                new int[]{1,0,-1 },
                new int[]{2,0,-2 },
                new int[]{1,0,-1 },
           };
            int sum = 0;
            byte value;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(rectangle, 3, 3, () => 0, (x, y, getter, setter) =>
            {
                sum = 0;
                getter((xx, yy, r, g, b) =>
                {
                    value = RgbToGray(r, g, b);
                    sum += (coefficient[yy][xx] * value);
                });
                if (sum < 0)
                {
                    sum = 0;
                }
                else if (sum > 255)
                {
                    sum = 255;
                }
                bitmap.SetPi(x, y, (byte)sum, (byte)sum, (byte)sum);
            }, 1, false);
        }
        public static void PrewittVFilter(this Bitmap bitmap, Rectangle rectangle, int windowWidth, int windowHeight)
        {
            int[][] coefficient = new int[][]
            {
                new int[]{-1,-1,-1 },
                new int[]{0,0,0 },
                new int[]{1,1,1 },
            };
            int sum = 0;
            byte value;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(rectangle, windowWidth, windowHeight, () => 0, (x, y, getter, setter) =>
            {
                sum = 0;
                getter((xx, yy, r, g, b) =>
                {
                    value = RgbToGray(r, g, b);
                    sum += (coefficient[yy][xx] * value);
                });
                if (sum < 0)
                {
                    sum = 0;
                }
                else if (sum > 255)
                {
                    sum = 255;
                }
                bitmap.SetPi(x, y, (byte)sum, (byte)sum, (byte)sum);
            }, 1, false);
        }
        public static void PrewittHFilter(this Bitmap bitmap, Rectangle rectangle, int windowWidth, int windowHeight)
        {
            int[][] coefficient = new int[][]
            {
                new int[]{-1,0,1 },
                new int[]{-1,0,1 },
                new int[]{-1,0,1 },
            };
            int sum = 0;
            byte value;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(rectangle, windowWidth, windowHeight, () => 0, (x, y, getter, setter) =>
            {
                sum = 0;
                getter((xx, yy, r, g, b) =>
                {
                    value = RgbToGray(r, g, b);
                    sum += (coefficient[yy][xx] * value);
                });
                if (sum < 0)
                {
                    sum = 0;
                }
                else if (sum > 255)
                {
                    sum = 255;
                }
                bitmap.SetPi(x, y, (byte)sum, (byte)sum, (byte)sum);
            }, 1, false);
        }
        public static void LaplacicanFilter(this Bitmap bitmap, Rectangle rectangle)
        {
            int[][] coefficient = new int[][]
            {
                new int[]{0,1,0 },
                new int[]{1,-4,1 },
                new int[]{0,1,0 },
            };
            int sum = 0;
            byte value;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(rectangle, 3, 3, () => 0, (x, y, getter, setter) =>
             {
                 sum = 0;
                 getter((xx, yy, r, g, b) =>
                 {
                     value = RgbToGray(r, g, b);
                     sum += (coefficient[yy][xx] * value);
                 });
                 if (sum < 0)
                 {
                     sum = 0;
                 }
                 else if (sum > 255)
                 {
                     sum = 255;
                 }
                 bitmap.SetPi(x, y, (byte)sum, (byte)sum, (byte)sum);
             }, 1, false);
        }
        public static void EmbossFilter(this Bitmap bitmap, Rectangle rectangle)
        {
            int[][] coefficient = new int[][]
            {
                new int[]{-2,-1,0 },
                new int[]{-1,1,1 },
                new int[]{0,1,2 },
            };
            int sum = 0;
            byte value;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(rectangle, 3, 3, () => 0, (x, y, getter, setter) =>
            {
                sum = 0;
                getter((xx, yy, r, g, b) =>
                {
                    value = RgbToGray(r, g, b);
                    sum += (coefficient[yy][xx] * value);
                });
                if (sum < 0)
                {
                    sum = 0;
                }
                else if (sum > 255)
                {
                    sum = 255;
                }
                bitmap.SetPi(x, y, (byte)sum, (byte)sum, (byte)sum);
            }, 1, false);
        }
        public static void LoGFilter(this Bitmap bitmap, Rectangle rectangle, int windowWidth, int windowHeight, double simga)
        {
            double[][] coefficient = MathFunction.LaplaciainOfGaussian(windowWidth, windowHeight, simga);
            double sum = 0;
            byte value;
            Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(rectangle, windowWidth, windowHeight, () => 0, (x, y, getter, setter) =>
            {
                sum = 0;
                getter((xx, yy, r, g, b) =>
                {
                    value = RgbToGray(r, g, b);
                    sum += (coefficient[yy][xx] * value);
                });
                bitmap.SetPi(x, y, (byte)sum, (byte)sum, (byte)sum);
            }, 1, false);
        }
        public static int[] Histogram(this Bitmap bitmap, Rectangle rectangle, Action<Bitmap> callback)
        {
            int[] values = new int[256];
            byte value = 0;
            bitmap.PixForeach(rectangle, (x, y, r, g, b) =>
             {
                 value = RgbToGray(r, g, b);
                 values[value]++;
             });
            if (callback != null)
            {
                int max = 0;
                for (int i = 0; i < values.Length; i++)
                {
                    var item = values[i];
                    if (item > max)
                    {
                        max = item;
                    }
                }
                Bitmap bitmap1 = new Bitmap(256, max);
                var g = Graphics.FromImage(bitmap1);
                Pen pen = new Pen(Color.Red);
                for (int i = 0; i < values.Length; i++)
                {
                    g.DrawLine(pen, i, max - 1, i, max - values[i]);
                }
                pen.Dispose();
                g.Dispose();
                callback(bitmap1);
            }
            return values;
        }
        public static void Normalization(this Bitmap bitmap, Rectangle rectangle, byte a, byte b)
        {
            byte rMax = byte.MinValue, rMin = byte.MaxValue;
            byte gMax = byte.MinValue, gMin = byte.MaxValue;
            byte bMax = byte.MinValue, bMin = byte.MaxValue;
            bitmap.PixForeach(rectangle, (x, y, red, green, blue) =>
            {
                if (red > rMax)
                {
                    rMax = red;
                }
                else if (red < rMin)
                {
                    rMin = red;
                }
                if (green > gMax)
                {
                    gMax = green;
                }
                else if (green < gMin)
                {
                    gMin = green;
                }
                if (blue > bMax)
                {
                    bMax = blue;
                }
                else if (blue < bMin)
                {
                    bMin = blue;
                }
            });
            double rCoefficient = (b - a) * 1.0 / (rMax - rMin);
            double gCoefficient = (b - a) * 1.0 / (gMax - gMin);
            double bCoefficient = (b - a) * 1.0 / (bMax - bMin);
            Func<byte, byte, byte, double, byte> func = (_value, c, d, coefficient) =>
                {
                    if (_value < c)
                    {
                        return a;
                    }
                    else if (_value < d)
                    {
                        return (byte)(coefficient * (_value - c) + a);
                    }
                    else
                    {
                        return b;
                    }
                };
            bitmap.HandleImage(rectangle, (x, y, red, green, blue, setter) =>
            {
                setter(func(red, rMin, rMax, rCoefficient),
                    func(green, rMin, rMax, rCoefficient),
                    func(blue, rMin, rMax, rCoefficient));
            });
        }
        public static int[] NormalizationHistogram(this Bitmap bitmap, Rectangle rectangle, byte a, byte b, Action<Bitmap> callback)
        {
            bitmap.Normalization(rectangle, a, b);
            return bitmap.Histogram(rectangle, callback);
        }
        /// <summary>
        /// 平坦直方图
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        /// <param name="m0">平均值</param>
        /// <param name="s0">标准差</param>
        public static void Flat(this Bitmap bitmap, Rectangle rectangle, double m0, double s0)
        {
            double sum = 0;
            double squared_sum = 0;
            int count = rectangle.Width * rectangle.Height * 3;
            bitmap.PixForeach(rectangle, (x, y, r, g, b) =>
            {
                sum += r;
                sum += g;
                sum += b;
                squared_sum += (r * r);
                squared_sum += (g * g);
                squared_sum += (b * b);
            });
            double m = sum / count;
            double s = Math.Sqrt(squared_sum / count - m * m);
            double s0_s = s0 / s;
            Func<byte, byte> func = value =>
             {
                 return (byte)(s0_s * (value - m) + m0);
             };
            bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
            {
                setter(func(r), func(g), func(b));
            });
        }
        public static void Equalization(this Bitmap bitmap, Rectangle rectangle)
        {
            float zMax = 255;
            int count = rectangle.Width * rectangle.Height;
            int[] hist = bitmap.Histogram(rectangle, null);
            int sum = 0;
            bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
            {
                byte rSet = 0;
                sum = 0;
                for (int i = 0; i < r; i++)
                {
                    sum += hist[i];
                }
                rSet = (byte)(zMax / count * sum);

                byte gSet = 0;
                sum = 0;
                for (int i = 0; i < g; i++)
                {
                    sum += hist[i];
                }
                gSet = (byte)(zMax / count * sum);

                byte bSet = 0;
                sum = 0;
                for (int i = 0; i < b; i++)
                {
                    sum += hist[i];
                }
                bSet = (byte)(zMax / count * sum);
                setter(rSet, gSet, bSet);
            });
        }
        public static void EqualizationHistogram(this Bitmap bitmap, Rectangle rectangle, Action<Bitmap> histogram)
        {
            bitmap.Equalization(rectangle);
            bitmap.Histogram(rectangle, histogram);
        }
        public static void FlatHistogram(this Bitmap bitmap, Rectangle rectangle, double m0, double s0, Action<Bitmap> callback)
        {
            bitmap.Flat(rectangle, m0, s0);
            bitmap.Histogram(rectangle, callback);
        }
        public static void GammaCorrection(this Bitmap bitmap, Rectangle rectangle, double c, double g)
        {
            double cReciprocal = 1 / c;
            double gReciprocal = 1 / g;
            Func<double, double> gammaCalibration = (value) =>
             {
                 return cReciprocal * Math.Pow(value, gReciprocal);
             };
            bitmap.HandleImage(rectangle, (x, y, red, green, blue, setter) =>
            {
                setter(
                    (byte)(gammaCalibration(red * 1.0 / 255) * 255),
                    (byte)(gammaCalibration(green * 1.0 / 255) * 255),
                    (byte)(gammaCalibration(blue * 1.0 / 255) * 255));
            });
        }
        public static Bitmap NearestNeighborInterpolation(this Bitmap bitmap, double zoomWidthRate, double zoomHeightRate)
        {
            Bitmap newBitmap = new Bitmap(
                (int)(bitmap.Width * zoomWidthRate),
                (int)(bitmap.Height * zoomHeightRate),
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.HandleImage(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), (x, y, setter) =>
               {
                   bitmap.GetPi((int)(x / zoomWidthRate), (int)(y / zoomHeightRate), (red, green, blue) =>
                   {
                       setter(red, green, blue);
                   });
               });
            return newBitmap;
        }
        public static Bitmap BilinearInterpolation(this Bitmap bitmap, double zoomWidthRate, double zoomHeightRate)
        {
            Bitmap newBitmap = new Bitmap(
                (int)(bitmap.Width * zoomWidthRate),
                (int)(bitmap.Height * zoomHeightRate),
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            double dx = 0, dy = 0;
            double _1_dx = 0, _1_dy = 0;
            double x0, y0, x0_1, y0_1;
            double rSum = 0;
            double gSum = 0;
            double bSum = 0;
            newBitmap.HandleImage(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), (x, y, setter) =>
                {
                    rSum = 0;
                    gSum = 0;
                    bSum = 0;
                    x0 = x / zoomWidthRate;
                    y0 = y / zoomHeightRate;
                    x0_1 = x0 + 1;
                    y0_1 = y0 + 1;
                    dx = x0 - ((int)x0);
                    dy = y0 - ((int)y0);
                    _1_dx = 1 - dx;
                    _1_dy = 1 - dy;
                    if (x0_1 > bitmap.Width - 1)
                    {
                        x0_1 = x0;
                    }
                    if (y0_1 > bitmap.Height - 1)
                    {
                        y0_1 = y0;
                    }
                    bitmap.GetPi((int)x0, (int)y0, (r, g, b) =>
                    {
                        rSum += _1_dx * _1_dy * r;
                        gSum += _1_dx * _1_dy * g;
                        bSum += _1_dx * _1_dy * b;
                    });
                    bitmap.GetPi((int)x0_1, (int)y0, (r, g, b) =>
                    {
                        rSum += dx * _1_dy * r;
                        gSum += dx * _1_dy * g;
                        bSum += dx * _1_dy * b;
                    });
                    bitmap.GetPi((int)x0, (int)y0_1, (r, g, b) =>
                    {
                        rSum += _1_dx * dy * r;
                        gSum += _1_dx * dy * g;
                        bSum += _1_dx * dy * b;
                    });
                    bitmap.GetPi((int)x0_1, (int)y0_1, (r, g, b) =>
                    {
                        rSum += dx * dy * r;
                        gSum += dx * dy * g;
                        bSum += dx * dy * b;
                    });
                    setter((byte)rSum, (byte)gSum, (byte)bSum);
                });
            return newBitmap;
        }
        public static Bitmap BicubicInterpolation(this Bitmap bitmap, double zoomWidthRate, double zoomHeightRate)
        {
            double a = -1;
            double integer;
            double powerTmp;
            Func<double, double> func = value =>
            {
                integer = value;
                if (integer < 0)
                {
                    integer = -value;
                }
                if (value <= 1 && value >= -1)
                {
                    powerTmp = integer * integer;
                    return (a + 2) * integer * powerTmp - (a + 3) * powerTmp + 1;
                }
                else if (value <= 2 && value >= -2)
                {
                    powerTmp = integer * integer;
                    return a * integer * powerTmp - 5 * a * powerTmp + 8 * a * integer - 4 * a;
                }
                else
                {
                    return 0;
                }
            };
            Bitmap newBitmap = new Bitmap(
                (int)(bitmap.Width * zoomWidthRate),
                (int)(bitmap.Height * zoomHeightRate),
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            double dx, dy = 0;
            double rSum = 0;
            double gSum = 0;
            double bSum = 0;
            int yOutTmp = -1, yTmp = -2;
            double wy = 0, wx, wSum, wMul;
            newBitmap.HandleImage(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height),
                (x, y, setter) =>
                {
                    wSum = 0;
                    rSum = 0;
                    gSum = 0;
                    bSum = 0;
                    if (y != yOutTmp)
                    {
                        dy = y / zoomHeightRate;
                    }
                    yTmp = -2;
                    dx = x / zoomWidthRate;
                    bitmap.PixForeach(new Rectangle((int)dx - 1, (int)dy - 1, 4, 4), (xGetter, yGetter, r, g, b) =>
                    {
                        if (yTmp != yGetter)
                        {
                            wy = func(dy - yGetter);
                        }
                        wx = func(dx - xGetter);
                        wMul = wx * wy;
                        wSum += wMul;
                        rSum += wMul * r;
                        gSum += wMul * g;
                        bSum += wMul * b;
                        yTmp = yGetter;
                    });
                    yOutTmp = y;
                    setter((byte)(rSum / wSum).FitRange(0, 255), (byte)(gSum / wSum).FitRange(0, 255), (byte)(bSum / wSum).FitRange(0, 255));
                });
            return newBitmap;
        }
        public static Bitmap Translation(this Bitmap bitmap, int offsetX, int offsetY)
        {
            return bitmap.Translation(new Rectangle(0, 0, bitmap.Width, bitmap.Height), offsetX, offsetY);
        }
        public static Bitmap Translation(this Bitmap bitmap, Rectangle rectangle, int offsetX, int offsetY)
        {
            return bitmap.AfineTransformations(1, 0, 0, 1, offsetX, offsetY);
            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int targetX, targetY;
            bitmap.PixForeach(rectangle, (x, y, r, g, b) =>
            {
                targetX = x + offsetX;
                targetY = y + offsetY;
                if (rectangle.Contains(targetX, targetY))
                {
                    newBitmap.SetPi(targetX, targetY, r, g, b);
                }
            });
            return newBitmap;
        }
        public static Bitmap Scale(this Bitmap bitmap, double xRate, double yRate)
        {
            return bitmap.NearestNeighborInterpolation(xRate, yRate);
        }
        public static Bitmap Rotate(this Bitmap bitmap, double angle, int offsetX, int offsetY)
        {
            double radian = angle / 180 * Math.PI;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);
            return bitmap.AfineTransformations(cos, -sin, sin, cos, offsetX, offsetY);
        }
        public static Bitmap Incline(this Bitmap bitmap, int tx, int ty)
        {
            Func<int, int, int> xAction, yAction;
            int newBitmapWidth = bitmap.Width, newBitmapHeight = bitmap.Height;
            if (tx > 0)
            {
                newBitmapWidth += tx;
                xAction = (value, offset) => value + offset;
            }
            else
            {
                newBitmapWidth -= tx;
                xAction = (value, offset) => value - tx - offset;
            }
            if (ty > 0)
            {
                newBitmapHeight += ty;
                yAction = (value, offset) => value + offset;
            }
            else
            {
                newBitmapHeight -= ty;
                yAction = (value, offset) => value - tx - offset;
            }
            Bitmap newBitmap = new Bitmap(newBitmapWidth, newBitmapHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newBitmap.CheckAfineTransformations(0, bitmap.Width, 0, bitmap.Height,
                1, tx * 1.0 / bitmap.Height, ty * 1.0 / bitmap.Width, 1, 0, 0, (x, y, setter) =>
            {
                bitmap.GetPi(x, y, (r, g, b) =>
                {
                    setter(r, g, b);
                });
            });
            return newBitmap;
        }
        public static Bitmap AfineTransformations(this Bitmap bitmap, double a, double b, double c, double d, int offsetX, int offsetY)
        {
            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int originX, originY;
            double det = 1 / (a * d - b * c);
            var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            newBitmap.HandleImage(rectangle, (x, y, setter) =>
             {
                 originX = (int)(det * (d * x - b * y) + 0.5) - offsetX;
                 originY = (int)(det * (-c * x + a * y) + 0.5) - offsetY;
                 if (rectangle.Contains(originX, originY))
                 {
                     bitmap.GetPi(originX, originY, (red, green, blue) =>
                      {
                          setter(red, green, blue);
                      });
                 }
             });
            return newBitmap;
        }
        public static void CheckAfineTransformations(this Bitmap bitmap, int startX, int endX, int startY, int endY,
            double a, double b, double c, double d, int offsetX, int offsetY, Action<int, int, Action<byte, byte, byte>> callback)
        {
            int originX, originY;
            double det = 1 / (a * d - b * c);
            var rectangle = new Rectangle(0, 0, endX - startX, endY - startY);
            bitmap.HandleImage(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                (x, y, setter) =>
                {
                    originX = (int)(det * (d * x - b * y) + 0.5) - offsetX;
                    originY = (int)(det * (-c * x + a * y) + 0.5) - offsetY;
                    if (rectangle.Contains(originX, originY))
                    {
                        callback(originX, originY, setter);
                    }
                });
        }
        /// <summary>
        /// 傅立叶变换
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="selectChannel"></param>
        /// <param name="spectrogram"></param>
        /// <returns></returns>
        public static ComplexSelf[][] DFT(this Bitmap bitmap, Func<byte, byte, byte, byte> selectChannel, Action<Bitmap> spectrogram)
        {
            Action<int, int, ComplexSelf> setSpectrogram;
            Bitmap newBitmap = null;
            double count = bitmap.Width * bitmap.Height;
            if (spectrogram == null)
            {
                setSpectrogram = (xx, yy, value) => { };
            }
            else
            {
                newBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                setSpectrogram = (xx, yy, value) =>
                {
                    value.Modules();
                    byte gray = (byte)((value.Modulus / count).FitRange(0, 255));
                    newBitmap.SetPi(xx, yy, gray, gray, gray);
                };
            }
            ComplexSelf[][] complices = new ComplexSelf[bitmap.Height][];
            int y, x;
            byte grayValue;
            double angle = 0;
            double _2Pi = Math.PI + Math.PI;
            for (y = 0; y < complices.Length; y++)
            {
                var item = new ComplexSelf[bitmap.Width];
                complices[y] = item;
                for (x = 0; x < bitmap.Width; x++)
                {
                    ComplexSelf sum = new ComplexSelf();
                    item[x] = sum;
                    bitmap.PixForeach((xGrayImage, yGrayImage, r, g, b) =>
                    {
                        grayValue = selectChannel(r, g, b);
                        angle = -_2Pi * (x * xGrayImage * 1.0 / bitmap.Width + y * yGrayImage * 1.0 / bitmap.Height);
                        sum += (new ComplexSelf(Math.Cos(angle), Math.Sin(angle)) * grayValue);
                    });
                    setSpectrogram(x, y, sum);
                }
            }
            if (spectrogram != null)
            {
                spectrogram(newBitmap);
            }
            return complices;
        }
        /// <summary>
        /// 离散余弦变换DCT
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        public static double[][][] DiscreteCosineTransformation(this Bitmap bitmap, int T)
        {
            double[][][] buffer = new double[bitmap.Height][][];
            double _1__sqrt2 = Math.Sqrt(2) / 2;
            Func<int, double> cFunc = s => s == 0 ? _1__sqrt2 : 1;
            double _pi__2T = Math.PI / (T << 1);
            double _2__T = 2.0 / T;
            double f = 0;
            int y, x, channel, u, v;
            double cv, cu;
            Rectangle location = new Rectangle(0, 0, T, T);
            for (y = 0; y < bitmap.Height; y++)
            {
                var item = new double[bitmap.Width][];
                buffer[y] = item;
                for (x = 0; x < bitmap.Width; x++)
                {
                    item[x] = new double[3];
                }
            }
            for (y = 0; y < bitmap.Height; y += T)
            {
                var item = buffer[y];
                for (x = 0; x < bitmap.Width; x += T)
                {
                    var item1 = item[x];
                    for (channel = 0; channel < 3; channel++)
                    {
                        for (u = 0; u < T; u++)
                        {
                            for (v = 0; v < T; v++)
                            {
                                f = 0;
                                cv = cFunc(v);
                                cu = cFunc(u);
                                location.X = x;
                                location.Y = y;
                                bitmap.PixForeach(channel, location, (_x, _y, value) =>
                                {
                                    f += cv
                                    * cu
                                    * value
                                    * Math.Cos((((_x - x) << 1) + 1) * u * _pi__2T)
                                    * Math.Cos((((_y - y) << 1) + 1) * v * _pi__2T);
                                });
                                buffer[y + v][x + u][channel] = f * _2__T;
                            }
                        }
                    }
                }
            }
            return buffer;
        }
        /// <summary>
        /// 均方误差
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="compareImage"></param>
        /// <param name="rectangle"></param>
        public static double MSE(this Bitmap bitmap, Bitmap compareImage, Rectangle rectangle)
        {
            int sum = 0;
            int diff = 0;
            bitmap.HandleTwoImage(compareImage, rectangle,
                (x, y, r1, g1, b1, r2, g2, b2, setter1, setter2) =>
                {
                    diff = r1 - r2;
                    sum += diff * diff;
                    diff = g1 - g2;
                    sum += diff * diff;
                    diff = b1 - b2;
                    sum += diff * diff;
                });
            return sum * 1.0 / rectangle.Width / rectangle.Height;
        }
        /// <summary>
        /// 峰值信噪比
        /// </summary>
        /// <param name="bitmap"></param>
        public static double PSNR(this Bitmap bitmap, Bitmap compareImage, Rectangle rectangle)
        {
            double vMax = 255;
            return 10 * Math.Log10(vMax * vMax / bitmap.MSE(compareImage, rectangle));
        }
        public static void Canny(this Bitmap bitmap, Rectangle rectangle,
            int windowWidth, int windowHeight, double sigma, int max, int min,
            Action<Bitmap, Bitmap> callback,
            Action<Bitmap> callback1,
            Action<Bitmap> callback2)
        {
            bitmap.GaussianFilter(rectangle, windowWidth, windowHeight, sigma);
            var bitmapH = bitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            bitmapH.SobelHFilter(rectangle);
            var bitmapV = bitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            bitmapV.SobelVFilter(rectangle);

            Func<double, byte> quantificationFunc = value =>
            {
                if (value <= 0.4142 && value > -0.4142)
                {
                    return 0;
                }
                else if (value < 2.4142 && value > 0.4142)
                {
                    return 45;
                }
                else if (value > -2.4142 && value <= -0.4142)
                {
                    return 90;
                }
                return 135;
            };

            var edgeBitmap = new Bitmap(rectangle.Width, rectangle.Height,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var angleBitmap = new Bitmap(rectangle.Width, rectangle.Height,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            byte edgeValue = 0;
            byte angleValue = 0;
            bitmapH.HandleTwoImage(bitmapV, rectangle,
                (x, y, r1, g1, b1, r2, g2, b2, setter1, setter2) =>
                {
                    edgeValue = (byte)(Math.Sqrt(r1 * r1 + r2 * r2).FitRange(0, 255));
                    edgeBitmap.SetPi(x, y, edgeValue, edgeValue, edgeValue);

                    if (r1 == 0)
                    {
                        angleValue = quantificationFunc(Math.Atan(r2 * 1.0 / 0.0000001));
                    }
                    else
                    {
                        angleValue = quantificationFunc(Math.Atan(r2 * 1.0 / r1));
                    }
                    angleBitmap.SetPi(x, y, angleValue, angleValue, angleValue);
                });
            if (callback != null)
            {
                callback(edgeBitmap, angleBitmap);
            }

            byte value1 = 0, value2 = 0;
            var newResultBitmap = edgeBitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            angleBitmap.PixForeach(rectangle, (x, y, r, g, b) =>
            {
                if (r == 0)
                {
                    if (rectangle.Contains(x - 1, y) && rectangle.Contains(x + 1, y))
                    {
                        edgeBitmap.GetPi(x - 1, y, (_r, _g, _b) =>
                        {
                            value1 = _r;
                        });
                        edgeBitmap.GetPi(x + 1, y, (_r, _g, _b) =>
                          {
                              value2 = _r;
                          });
                        edgeBitmap.GetSetPix(x, y, (_r, _g, _b, setter) =>
                        {
                            if (_r < value1 || _r < value2)
                            {
                                newResultBitmap.SetPi(x, y, 0, 0, 0);
                            }
                        });
                    }
                }
                else if (r == 45)
                {
                    if (rectangle.Contains(x - 1, y + 1) && rectangle.Contains(x + 1, y - 1))
                    {
                        edgeBitmap.GetPi(x - 1, y + 1, (_r, _g, _b) =>
                          {
                              value1 = _r;
                          });
                        edgeBitmap.GetPi(x + 1, y - 1, (_r, _g, _b) =>
                            {
                                value2 = _r;
                            });
                        edgeBitmap.GetSetPix(x, y, (_r, _g, _b, setter) =>
                        {
                            if (_r < value1 || _r < value2)
                            {
                                newResultBitmap.SetPi(x, y, 0, 0, 0);
                            }
                        });
                    }
                }
                else if (r == 90)
                {
                    if (rectangle.Contains(x, y - 1) && rectangle.Contains(x, y + 1))
                    {
                        edgeBitmap.GetPi(x, y - 1, (_r, _g, _b) =>
                         {
                             value1 = _r;
                         });
                        edgeBitmap.GetPi(x, y + 1, (_r, _g, _b) =>
                          {
                              value2 = _r;
                          });
                        edgeBitmap.GetSetPix(x, y, (_r, _g, _b, setter) =>
                        {
                            if (_r < value1 || _r < value2)
                            {
                                newResultBitmap.SetPi(x, y, 0, 0, 0);
                            }
                        });
                    }
                }
                else if (r == 135)
                {
                    if (rectangle.Contains(x - 1, y - 1) && rectangle.Contains(x + 1, y + 1))
                    {
                        edgeBitmap.GetPi(x - 1, y - 1, (_r, _g, _b) =>
                           {
                               value1 = _r;
                           });
                        edgeBitmap.GetPi(x + 1, y + 1, (_r, _g, _b) =>
                            {
                                value2 = _r;
                            });
                        edgeBitmap.GetSetPix(x, y, (_r, _g, _b, setter) =>
                        {
                            if (_r < value1 || _r < value2)
                            {
                                newResultBitmap.SetPi(x, y, 0, 0, 0);
                            }
                        });
                    }
                }
            });
            if (callback1 != null)
            {
                callback1(newResultBitmap);
            }
            var newResultBitmap1 = newResultBitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            newResultBitmap.HandleImage(rectangle, 1, 1, 1, 1, () => 0, (x, y, getter, setter) =>
            {
                getter((xx, yy, r, g, b) =>
                {
                    if (xx == x && yy == y)
                    {
                        if (r > max)
                        {
                            newResultBitmap1.SetPi(xx, yy, 255, 255, 255);
                        }
                        else if (r < min)
                        {
                            newResultBitmap1.SetPi(xx, yy, 0, 0, 0);
                        }
                    }
                    if (r > max)
                    {
                        newResultBitmap1.SetPi(xx, yy, 255, 255, 255);
                    }
                });
            }, 2, false);
            if (callback2 != null)
            {
                callback2(newResultBitmap1);
            }
        }

        public class PixInfo
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Count { get; set; }
        }



        public static void HoughTransform(this Bitmap bitmap, Rectangle rectangle,
            int lineCount,
            Action<Bitmap> callback1,
            Action<Bitmap> callback2,
            Action<Bitmap> callback3)
        {
            var result = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var rMax = (int)Math.Sqrt(rectangle.Height * rectangle.Height + rectangle.Width * rectangle.Width);
            var _2rMax = rMax << 1;
            int[][] count = new int[180][];
            int i;
            for (i = 0; i < count.Length; i++)
            {
                var item = new int[_2rMax];
                count[i] = item;
            }
            int rPos = 0;
            int angleIndex = 0;
            double angleStep = Math.PI / 180;
            double angleCurrent = 0;
            bitmap.Canny(rectangle, 5, 5, 1.4, 100, 30, null, null, s =>
            {
                s.PixForeach(0, rectangle, (x, y, value) =>
                {
                    if (value == 255)
                    {
                        angleCurrent = 0;
                        for (angleIndex = 0; angleIndex < 180; angleIndex++)
                        {
                            rPos = (int)(x * Math.Cos(angleCurrent) + y * Math.Sin(angleCurrent));
                            count[angleIndex][rPos + rMax]++;
                            angleCurrent += angleStep;
                        }
                    }
                });
            });
            if (callback1 != null)
            {
                int item = 0;
                var newBitmap = new Bitmap(180, _2rMax, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                newBitmap.HandleImage(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), (x, y, setter) =>
                {
                    item = count[x][y];
                    if (item > 255)
                    {
                        setter(255, 255, 255);
                    }
                    else
                    {
                        setter((byte)item, (byte)item, (byte)item);
                    }
                });
                callback1(newBitmap);
            }
            int[][] countClone = new int[180][];
            for (i = 0; i < count.Length; i++)
            {
                var item = new int[_2rMax];
                countClone[i] = item;
            }
            List<PixInfo> list = new List<PixInfo>();
            int current = 0;
            int ii = 0;
            MoveWindowFunction._2dArrayMoveWindowFunction(
                count, new Rectangle(0, 0, _2rMax, 180),
                1, 1, 1, 1, () => 0, (x, y, value, getter, setter) =>
                {
                    if (getter((xx, yy, _value) => _value <= value))
                    {
                        if (value == 0)
                        {
                            return;
                        }
                        if (list.Count == 0)
                        {
                            list.Add(new PixInfo()
                            {
                                X = x,
                                Y = y,
                                Count = value
                            });
                            current++;
                            return;
                        }
                        for (ii = 0; ii < list.Count; ii++)
                        {
                            var item = list[ii];
                            if (value > item.Count)
                            {
                                current++;
                                list.Insert(ii, new PixInfo()
                                {
                                    X = x,
                                    Y = y,
                                    Count = value
                                });
                                if (current > lineCount)
                                {
                                    int indexx;
                                    int tmp;
                                    do
                                    {
                                        indexx = list.Count - 1;
                                        tmp = list[indexx].Count;
                                        list.RemoveAt(indexx);
                                        indexx--;
                                    } while (tmp == list[indexx].Count);
                                }
                                return;
                            }
                            else if (value == item.Count)
                            {
                                list.Insert(ii, new PixInfo()
                                {
                                    X = x,
                                    Y = y,
                                    Count = value
                                });
                                return;
                            }
                        }
                    }
                }, 0, false);

            if (callback2 != null)
            {
                var newBitmap = new Bitmap(180, _2rMax, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (ii = 0; ii < list.Count; ii++)
                {
                    var item = list[ii];
                    var value = (byte)(item.Count.FitRange(0, 255));
                    newBitmap.SetPi(item.Y, item.X, value, value, value);
                }
                callback2(newBitmap);
            }
            double cosT, sinT;
            int xxx, yyy;
            int xEnd = rectangle.Right, yEnd = rectangle.Bottom, xEnd_1 = xEnd - 1, yEnd_1 = yEnd - 1;
            for (ii = 0; ii < list.Count; ii++)
            {
                var item = list[ii];
                angleCurrent = item.Y * Math.PI / 180;
                cosT = Math.Cos(angleCurrent);
                sinT = Math.Sin(angleCurrent);
                if (sinT == 0 || cosT == 0)
                {
                    continue;
                }

                for (xxx = rectangle.X; xxx < xEnd; xxx++)
                {
                    yyy = (int)((-cosT * xxx + (item.X - rMax)) / sinT);
                    if (yyy < rectangle.Y || yyy > yEnd_1)
                    {
                        continue;
                    }
                    result.SetPi(xxx, yyy, 255, 0, 0);
                }
                for (yyy = rectangle.Y; yyy < yEnd; yyy++)
                {
                    xxx = (int)((-sinT * yyy + (item.X - rMax)) / cosT);
                    if (xxx < rectangle.X || xxx > xEnd_1)
                    {
                        continue;
                    }
                    result.SetPi(xxx, yyy, 255, 0, 0);
                }
            }
            callback3(result);
        }
        /// <summary>
        /// 膨胀
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        /// <param name="size"></param>
        /// <param name="callback"></param>
        public static void Dilate(this Bitmap bitmap, Rectangle rectangle, int size,
            Action<Bitmap> callback)
        {
            bitmap.OtsusMethod(rectangle, s =>
            {

            });
            int current = 0;
            var newBitmap = bitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            while (current < size)
            {
                bitmap.HandleImage(rectangle, 1, 1, 1, 1, () => 0,
                (x, y, r, g, b, getter, setter) =>
                {
                    if (r == 255)
                    {
                        return;
                    }
                    getter((xx, yy, red, green, blue) =>
                    {
                        if (xx == 0)
                        {
                            if (red != 0)
                            {
                                newBitmap.SetPi(x, y, 255, 255, 255);
                                return false;
                            }
                        }
                        else if (yy == 0)
                        {
                            if (red != 0)
                            {
                                newBitmap.SetPi(x, y, 255, 255, 255);
                                return false;
                            }
                        }
                        return true;
                    });
                }, 0, false);
                current++;
            }
            callback(newBitmap);
        }
        /// <summary>
        /// 腐蚀
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        /// <param name="size"></param>
        /// <param name="callback"></param>
        public static void Erode(this Bitmap bitmap, Rectangle rectangle, int size,
            Action<Bitmap> callback)
        {
            bitmap.OtsusMethod(rectangle, s =>
            {

            });
            int current = 0;
            var newBitmap = bitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            while (current < size)
            {
                bitmap.HandleImage(rectangle, 1, 1, 1, 1, () => 0,
                (x, y, r, g, b, getter, setter) =>
                {
                    if (r == 0)
                    {
                        return;
                    }
                    getter((xx, yy, red, green, blue) =>
                    {
                        if (xx == 0)
                        {
                            if (red == 0)
                            {
                                newBitmap.SetPi(x, y, 0, 0, 0);
                                return false;
                            }
                        }
                        else if (yy == 0)
                        {
                            if (red == 0)
                            {
                                newBitmap.SetPi(x, y, 0, 0, 0);
                                return false;
                            }
                        }
                        return true;
                    });
                }, 0, false);
                current++;
            }
            callback(newBitmap);
        }
        /// <summary>
        /// 开运算
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        /// <param name="size"></param>
        /// <param name="callback"></param>
        public static void OpeningOperation(this Bitmap bitmap, Rectangle rectangle, int size,
            Action<Bitmap> callback)
        {
            bitmap.OtsusMethod(rectangle, s =>
            {

            });
            bitmap.Erode(rectangle, size, s =>
            {
                s.Dilate(rectangle, size, ss =>
                {
                    callback(ss);
                });
            });
        }
        /// <summary>
        /// 闭运算
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        /// <param name="size"></param>
        /// <param name="callback"></param>
        public static void ClosingOperation(this Bitmap bitmap, Rectangle rectangle, int size,
            Action<Bitmap> callback)
        {
            bitmap.Canny(rectangle, 5, 5, 1.4, 50, 20, null, null, s =>
            {
                s.Dilate(rectangle, size, ss =>
                {
                    ss.Erode(rectangle, size, sss =>
                    {
                        callback(sss);
                    });
                });
            });
        }

        /// <summary>
        /// 形态学梯度
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        /// <param name="size"></param>
        /// <param name="callback"></param>
        public static void MorphologyGradient(this Bitmap bitmap, Rectangle rectangle, int size,
            Action<Bitmap> callback)
        {
            bitmap.Dilate(rectangle, size, s =>
            {
                bitmap.Erode(rectangle, size, ss =>
                {
                    callback(s.Sub(ss, rectangle, 0));
                });
            });
        }

        /// <summary>
        /// 顶帽
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        public static void TopHat(this Bitmap bitmap, Rectangle rectangle)
        {
            var cloneImage = bitmap.Clone(
                   new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                   System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            bitmap.OtsusMethod(rectangle, s =>
            {

            });
            cloneImage.OpeningOperation(rectangle, 3, s =>
            {
                bitmap.Sub(s, rectangle, 0);
            });
        }
        /// <summary>
        /// 黑帽
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        public static void BlackHat(this Bitmap bitmap, Rectangle rectangle)
        {
            var cloneImage = bitmap.Clone(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            bitmap.OtsusMethod(rectangle, s =>
            {

            });
            cloneImage.ClosingOperation(rectangle, 3, s =>
            {
                s.Sub(bitmap, rectangle, 0);
            });
        }
        /// <summary>
        /// 误差平方和 ssd
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        /// <param name="template"></param>
        /// <param name="callback"></param>
        public static void TemplateMatchBySumOfSquaredDifference(this Bitmap bitmap, Rectangle rectangle, Bitmap template, Action<int, int> callback)
        {
            int min = int.MaxValue, sum, tmp;
            int matchX = -1, matchY = -1;
            bitmap.TemplateMatching(rectangle, template, () => 0,
                (x, y, getter, setter) =>
                {
                    sum = 0;
                    getter((xx, yy, r1, g1, b1, r2, g2, b2) =>
                     {
                         tmp = r1 - r2;
                         sum += tmp * tmp;
                         tmp = g1 - g2;
                         sum += tmp * tmp;
                         tmp = b1 - b2;
                         sum += tmp * tmp;
                     });
                    if (sum < min)
                    {
                        min = sum;
                        matchX = x;
                        matchY = y;
                    }
                }, 1);
            callback(matchX, matchY);
        }
        /// <summary>
        /// 绝对值差和 sad
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        /// <param name="template"></param>
        /// <param name="callback"></param>
        public static void TemplateMatchBySumOfAbsoluteDifference(this Bitmap bitmap, Rectangle rectangle, Bitmap template, Action<int, int> callback)
        {
            int min = int.MaxValue, sum, tmp;
            int matchX = -1, matchY = -1;
            bitmap.TemplateMatching(rectangle, template, () => 0,
                (x, y, getter, setter) =>
                {
                    sum = 0;
                    getter((xx, yy, r1, g1, b1, r2, g2, b2) =>
                    {
                        tmp = r1 - r2;
                        if (tmp < 0)
                        {
                            tmp = -tmp;
                        }
                        sum += tmp;
                        tmp = g1 - g2;
                        if (tmp < 0)
                        {
                            tmp = -tmp;
                        }
                        sum += tmp;
                        tmp = b1 - b2;
                        if (tmp < 0)
                        {
                            tmp = -tmp;
                        }
                        sum += tmp;
                    });
                    if (sum < min)
                    {
                        min = sum;
                        matchX = x;
                        matchY = y;
                    }
                }, 1);
            callback(matchX, matchY);
        }
        /// <summary>
        /// 归一化交叉相关 ncc
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        /// <param name="template"></param>
        /// <param name="callback"></param>
        public static void TemplateMatchByNormalizationCrossCorrelation(this Bitmap bitmap, Rectangle rectangle, Bitmap template, Action<int, int> callback)
        {
            int sum1, sum2, sum3;
            double sum, max = double.MinValue;
            int matchX = -1, matchY = -1;
            bitmap.TemplateMatching(rectangle, template, () => 0,
                (x, y, getter, setter) =>
                {
                    sum = 0;
                    sum1 = 0;
                    sum2 = 0;
                    sum3 = 0;
                    getter((xx, yy, r1, g1, b1, r2, g2, b2) =>
                    {
                        sum1 += r1 * r2;
                        sum2 += r1 * r1;
                        sum3 += r2 * r2;

                        sum1 += g1 * g2;
                        sum2 += g1 * g1;
                        sum3 += g2 * g2;

                        sum1 += b1 * b2;
                        sum2 += b1 * b1;
                        sum3 += b2 * b2;
                    });
                    sum = sum1 * 1.0 / (Math.Sqrt(sum2) * Math.Sqrt(sum3));
                    if (sum > max)
                    {
                        max = sum;
                        matchX = x;
                        matchY = y;
                    }
                }, 1);
            callback(matchX, matchY);
        }
        /// <summary>
        /// 零均值归一化交叉相关
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        /// <param name="template"></param>
        /// <param name="callback"></param>
        public static void TemplateMatchByZeroMeanNormalizationCrossCorrelation(this Bitmap bitmap, Rectangle rectangle, Bitmap template, Action<int, int> callback)
        {
            double bitmapRAvg = 0, bitmapGAvg = 0, bitmapBAvg = 0,
                templateRAvg = 0, templateGAvg = 0, templateBAvg = 0;
            bitmap.PixForeach(rectangle, (x, y, r, g, b) =>
            {
                bitmapRAvg += r;
                bitmapGAvg += g;
                bitmapBAvg += b;
            });
            template.PixForeach((x, y, r, g, b) =>
            {
                templateRAvg += r;
                templateGAvg += g;
                templateBAvg += b;
            });
            int count = rectangle.Width * rectangle.Height * 3;
            bitmapRAvg /= count;
            bitmapGAvg /= count;
            bitmapBAvg /= count;
            count = template.Width * template.Height * 3;
            templateRAvg /= count;
            templateGAvg /= count;
            templateBAvg /= count;
            double sum1, sum2, sum3, tmp1, diffBitmap, diffTemplate;
            double sum, max = double.MinValue;
            int matchX = -1, matchY = -1;
            bitmap.TemplateMatching(rectangle, template, () => 0,
                (x, y, getter, setter) =>
                {
                    sum = 0;
                    sum1 = 0;
                    sum2 = 0;
                    sum3 = 0;
                    getter((xx, yy, r1, g1, b1, r2, g2, b2) =>
                    {
                        diffBitmap = r1 - bitmapRAvg;
                        diffTemplate = r2 - templateRAvg;
                        tmp1 = diffBitmap * diffTemplate;
                        sum2 += diffBitmap * diffBitmap;
                        sum3 += diffTemplate * diffTemplate;
                        if (tmp1 < 0)
                        {
                            tmp1 = -tmp1;
                        }
                        sum1 += tmp1;
                        diffBitmap = g1 - bitmapGAvg;
                        diffTemplate = g2 - templateGAvg;
                        sum2 += diffBitmap * diffBitmap;
                        sum3 += diffTemplate * diffTemplate;
                        tmp1 = diffBitmap * diffTemplate;
                        if (tmp1 < 0)
                        {
                            tmp1 = -tmp1;
                        }
                        sum1 += tmp1;
                        diffBitmap = b1 - bitmapBAvg;
                        diffTemplate = b2 - templateBAvg;
                        sum2 += diffBitmap * diffBitmap;
                        sum3 += diffTemplate * diffTemplate;
                        tmp1 = diffBitmap * diffTemplate;
                        if (tmp1 < 0)
                        {
                            tmp1 = -tmp1;
                        }
                        sum1 += tmp1;
                    });
                    sum = sum1 / (Math.Sqrt(sum2) * Math.Sqrt(sum3));
                    if (sum > max)
                    {
                        max = sum;
                        matchX = x;
                        matchY = y;
                    }
                }, 1);
            callback(matchX, matchY);
        }

        /// <summary>
        /// 4-邻接连通域标记
        /// </summary>
        /// <param name="bitmap"></param>
        public static void _4AdjacentConnectedDomainMarker(this Bitmap bitmap)
        {
            int connectionTotal = 1, tmp, zeroCount;
            int[][] connectionCountArr = new int[bitmap.Height][];
            for (int i = 0; i < connectionCountArr.Length; i++)
            {
                connectionCountArr[i] = new int[bitmap.Width];
            }
            bitmap.HandleImage(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                (int x, int y, byte red, byte green, byte blue, Action<int, int, Action<byte, byte, byte>> getter, Action<byte, byte, byte> setter) =>
                {
                    if (red == 0)
                    {
                        return;
                    }
                    zeroCount = 0;
                    getter(-1, 0, (r, g, b) =>
                    {
                        if (r == 0)
                        {
                            return;
                        }
                        tmp = connectionCountArr[y][x - 1];

                    });
                    getter(0, -1, (r, g, b) =>
                    {
                        if (r == 0)
                        {
                            return;
                        }

                    });
                });
        }
    }
}
