﻿using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 视觉学习
{
    class Program
    {
        static void Main(string[] args)
        {

            int index = 0;
            #region 1-10
            //Bitmap bitmap = new Bitmap("E:\\ZG180140\\Desktop\\视觉\\ImageProcessing100Wen-master\\Question_01_10\\imori.jpg");
            //var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            //1
            //bitmap.ChangeChannelOrder(rectangle, (r, g, b, setter) => setter(b, g, r));
            //index = 1;
            //2
            //bitmap.Grayscale(rectangle);
            //index = 2;
            //3
            //bitmap.Thresholding(rectangle, 128);
            //index = 3;
            //4
            //bitmap.OtsusMethod(rectangle, s =>
            //{

            //});
            //index = 4;
            //5
            //bitmap.Inverse(rectangle);
            //index = 5;
            //6
            //bitmap.LoseLustre();
            //index = 6;
            //7
            //bitmap.AveragePooling(rectangle, 8, 8);            
            //index = 7;
            //8
            //bitmap.MaxPooling(rectangle, 8, 8);
            //index = 8;
            //9           
            //bitmap = new Bitmap("G:\\desk\\视觉\\ImageProcessing100Wen-master\\Question_01_10\\imori_noise.jpg");
            //bitmap.GaussianFilter(rectangle, 3, 3, 1.3);
            //index = 9;
            //10
            //bitmap = new Bitmap("E:\\ZG180140\\Desktop\\视觉\\ImageProcessing100Wen-master\\Question_01_10\\imori_noise.jpg");
            //bitmap.MedianFilter(rectangle, 3, 3);
            //index = 10;
            #endregion
            #region 11-20
            //11
            //bitmap.MeanFilter(rectangle, 3, 3);
            //index = 11;
            //12
            //bitmap.DiagonalFilter(rectangle, 3);
            //index = 12;
            //13
            //bitmap.MaxMinFilter(rectangle, 3, 3);
            //index = 13;
            //14
            //index = 14;
            //var bitmapClone = bitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //bitmapClone.DifferentialVFilter(rectangle, 3, 3);
            //bitmapClone.Save(string.Format("{0}-V.bmp", index));
            //bitmapClone = bitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //bitmapClone.DifferentialHFilter(rectangle, 3, 3);
            //bitmapClone.Save(string.Format("{0}-H.bmp", index));
            //15
            //index = 15;
            //var bitmapClone = bitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //bitmapClone.SobelVFilter(rectangle, 3, 3);
            //bitmapClone.Save(string.Format("{0}-V.bmp", index));
            //bitmapClone = bitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //bitmapClone.SobelHFilter(rectangle, 3, 3);
            //bitmapClone.Save(string.Format("{0}-H.bmp", index));
            //16
            //index = 16;
            //var bitmapClone = bitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //bitmapClone.PrewittVFilter(rectangle, 3, 3);
            //bitmapClone.Save(string.Format("{0}-V.bmp", index));
            //bitmapClone = bitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //bitmapClone.PrewittHFilter(rectangle, 3, 3);
            //bitmapClone.Save(string.Format("{0}-H.bmp", index));
            //17
            //index = 17;
            //bitmap.LaplacicanFilter(rectangle);
            //18
            //index = 18;
            //bitmap.EmbossFilter(rectangle);
            //19
            //index = 19;
            //bitmap = new Bitmap("E:\\ZG180140\\Desktop\\视觉\\ImageProcessing100Wen-master\\Question_11_20\\imori_noise.jpg");
            //bitmap.LoGFilter(rectangle, 5, 5, 3);
            //20
            //index = 20;
            //bitmap.Histogram(rectangle, s =>
            // {
            //     s.Save(string.Format("{0}_Chart.bmp", index));
            // });

            #endregion
            #region 21-30
            //21
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_21_30\\imori_dark.jpg");
            //var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            //index = 21;
            //bitmap.NormalizationHistogram(rectangle, 0, 255, s =>
            //{
            //    s.Save(string.Format("{0}_chart.bmp", index));
            //});
            //22
            //index = 22;
            //bitmap = new Bitmap("E:\\ZG180140\\Desktop\\视觉\\ImageProcessing100Wen-master\\Question_21_30\\imori_dark.jpg");
            //bitmap.FlatHistogram(rectangle, 128, 52, s =>
            //  {
            //      s.Save(string.Format("{0}_Chart.bmp", index));
            //  });
            //23
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_21_30\\imori.jpg");
            //var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            //index = 23;
            //bitmap.EqualizationHistogram(rectangle, s =>
            //{
            //    s.Save(string.Format("{0}_Chart.bmp", index));
            //});
            //24
            Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_21_30\\imori_dark.jpg");
            var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            index = 21;
            bitmap.NormalizationHistogram(rectangle, 0, 255, s =>
            {
                s.Save(string.Format("{0}_chart.bmp", index));
            });

            #endregion
            bitmap.Save(string.Format("{0}.bmp", index));
        }
    }
}
