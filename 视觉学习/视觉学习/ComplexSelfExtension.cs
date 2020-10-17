using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 视觉学习
{
    public static class ComplexSelfExtension
    {
        /// <summary>
        /// 傅里叶逆变换
        /// </summary>
        /// <param name="complices"></param>
        /// <param name="spectrogram"></param>
        public static void IDFT(this ComplexSelf[][] complices, Action<int, int, double> spectrogram)
        {
            int y, x, yy, xx, width = complices[0].Length, height = complices.Length;
            ComplexSelf sum;
            double count = width * height;
            double angle = 0;
            double _2Pi = Math.PI + Math.PI;
            for (y = 0; y < height; y++)
            {

                for (x = 0; x < width; x++)
                {
                    sum = new ComplexSelf();
                    for (yy = 0; yy < height; yy++)
                    {
                        for (xx = 0; xx < width; xx++)
                        {
                            angle = _2Pi * (x * xx * 1.0 / width + y * yy * 1.0 / height);
                            sum += (new ComplexSelf(Math.Cos(angle), Math.Sin(angle)) * complices[yy][xx]);
                        }
                    }
                    sum.Re /= count;
                    sum.Im /= count;
                    spectrogram(x, y, sum.Modules());
                }
            }
        }
        /// <summary>
        /// 滤波器系数处理
        /// </summary>
        /// <param name="complices"></param>
        /// <param name="condition"></param>
        public static void PassFilter(this ComplexSelf[][] complices, Func<double, bool> condition)
        {
            int height = complices.Length;
            int width = complices[0].Length;
            int halfHeight = height >> 1;
            int halfWidth = width >> 1;
            width--;
            height--;
            ComplexSelf[] item;
            for (int i = 0; i < halfHeight; i++)
            {
                for (int j = 0; j < halfWidth; j++)
                {
                    if (condition(Math.Sqrt(i * i + j * j)))
                    {
                        item = complices[i];
                        item[j].Clear();
                        item[width - j].Clear();
                        item = complices[height - i];
                        item[j].Clear();
                        item[width - j].Clear();
                    }
                }
            }
        }
        /// <summary>
        /// 低通滤波器
        /// </summary>
        /// <param name="complices"></param>
        /// <param name="rate"></param>
        public static void LowPassFilter(this ComplexSelf[][] complices, double rate)
        {
            int filterD = (byte)((complices.Length >> 1) * rate);
            complices.PassFilter(s => s >= filterD);
        }
        /// <summary>
        /// 高通滤波器
        /// </summary>
        /// <param name="complices"></param>
        /// <param name="rate"></param>
        public static void HighPassFilter(this ComplexSelf[][] complices, double rate)
        {
            int filterD = (byte)((complices.Length >> 1) * rate);
            complices.PassFilter(s => s <= filterD);
        }
        /// <summary>
        /// 带通滤波器
        /// </summary>
        /// <param name="complices"></param>
        /// <param name="lowRate"></param>
        /// <param name="highRate"></param>
        public static void BandWidthPassFilter(this ComplexSelf[][] complices, double lowRate, double highRate)
        {
            int LowFilterD = (byte)((complices.Length >> 1) * lowRate);
            int highFilterD = (byte)((complices.Length >> 1) * highRate);
            complices.PassFilter(s => s < LowFilterD || s > highFilterD);
        }
    }
}
