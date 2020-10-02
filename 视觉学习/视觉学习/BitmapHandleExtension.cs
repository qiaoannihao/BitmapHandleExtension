using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        public static void ChangeChannelOrder(this Bitmap bitmap, Rectangle rectangle, Action<byte, byte, byte, Action<byte, byte, byte>> orderCallback)
        {
            bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
            {
                orderCallback(r, g, b, setter);
            });
        }
        public static void Grayscale(this Bitmap bitmap, Rectangle rectangle)
        {
            bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
            {
                byte tmp = RgbToGray(r, g, b);
                setter(tmp, tmp, tmp);
            });
        }
        public static void Thresholding(this Bitmap bitmap, Rectangle rectangle, int value)
        {
            bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
            {
                byte tmp = RgbToGray(r, g, b);
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
        public static void LoseLustre(this Bitmap bitmap, Rectangle rectangle)
        {
            bitmap.HandleImage(rectangle, (x, y, r, g, b, setter) =>
             {
                 setter(LoseLustre(r), LoseLustre(g), LoseLustre(b));
             });
        }
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
        public static void SobelVFilter(this Bitmap bitmap, Rectangle rectangle, int windowWidth, int windowHeight)
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
        public static void SobelHFilter(this Bitmap bitmap, Rectangle rectangle, int windowWidth, int windowHeight)
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
        public static void CammaCorrection(this Bitmap bitmap, Rectangle rectangle)
        {

        }
    }
}
