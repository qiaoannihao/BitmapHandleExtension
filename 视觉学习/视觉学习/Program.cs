using Common;
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
            #region 01-10
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
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_21_30\\imori_gamma.jpg");
            //var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            //index = 24;
            //bitmap.GammaCorrection(rectangle, 1, 2.2);
            //25
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_21_30\\imori.jpg");
            //index = 25;
            //bitmap.NearestNeighborInterpolation(1.5, 1.5).Save(string.Format("{0}_zoom.bmp", index));
            //26
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_21_30\\imori.jpg");
            //index = 26;
            //bitmap.BilinearInterpolation(1.5, 1.5).Save(string.Format("{0}_zoom.bmp", index));
            //27
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_21_30\\imori.jpg");
            //index = 27;
            //bitmap.BicubicInterpolation(1.5, 1.5).Save(string.Format("{0}_zoom.bmp", index));
            //28
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_21_30\\imori.jpg");
            //index = 28;
            //bitmap.Translation(new Rectangle(0, 0, bitmap.Width, bitmap.Height), 30, -30).Save(string.Format("{0}_ttranslation.bmp", index));
            //29
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_21_30\\imori.jpg");
            //index = 29;
            //var resultImage = bitmap.Scale(1.3, 4.0 / 5);
            //resultImage.Save(string.Format("{0}_scale.bmp", index));
            //resultImage = resultImage.Translation(30, -30);
            //resultImage.Save(string.Format("{0}_scale_translation.bmp", index));
            //30
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_21_30\\imori.jpg");
            //index = 30;
            //var resultImage = bitmap.Rotate(-30, 0, 30);
            //resultImage.Save(string.Format("{0}_rotate.bmp", index));
            #endregion
            #region 31-40
            //31
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_31_40\\imori.jpg");
            //index = 31;
            //var resultImage = bitmap.Incline(30, 30);
            //resultImage.Save(string.Format("{0}_incline.bmp", index));
            //32
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_31_40\\imori.jpg");
            //index = 32;
            //Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //var result = bitmap.DFT((r, g, b) => r, null);
            //result.IDFT((x, y, value) =>
            //{
            //    newBitmap.SetPiRedChannel(x, y, (byte)value.FitRange(0, 255));
            //});
            //result = bitmap.DFT((r, g, b) => g, null);
            //result.IDFT((x, y, value) =>
            //{
            //    newBitmap.SetPiGreenChannel(x, y, (byte)value.FitRange(0, 255));
            //});
            //result = bitmap.DFT((r, g, b) => b, null);
            //result.IDFT((x, y, value) =>
            //{
            //    newBitmap.SetPiBlueChannel(x, y, (byte)value.FitRange(0, 255));
            //});
            //newBitmap.Save(string.Format("{0}_idft.bmp", index));
            //33
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_31_40\\imori.jpg");
            //index = 33;
            //Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //var result = bitmap.DFT((r, g, b) => r, null);
            //result.LowPassFilter(0.5);
            //result.IDFT((x, y, value) =>
            //{
            //    newBitmap.SetPiRedChannel(x, y, (byte)value.FitRange(0, 255));
            //});
            //result = bitmap.DFT((r, g, b) => g, null);
            //result.LowPassFilter(0.5);
            //result.IDFT((x, y, value) =>
            //{
            //    newBitmap.SetPiGreenChannel(x, y, (byte)value.FitRange(0, 255));
            //});
            //result = bitmap.DFT((r, g, b) => b, null);
            //result.LowPassFilter(0.5);
            //result.IDFT((x, y, value) =>
            //{
            //    newBitmap.SetPiBlueChannel(x, y, (byte)value.FitRange(0, 255));
            //});
            //newBitmap.Save(string.Format("{0}_lowFilter.bmp", index));
            //34
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_31_40\\imori.jpg");
            //index = 34;
            //Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //var result = bitmap.DFT((r, g, b) => r, null);
            //result.HighPassFilter(0.2);
            //result.IDFT((x, y, value) =>
            //{
            //    newBitmap.SetPiRedChannel(x, y, (byte)value.FitRange(0, 255));
            //});
            //result = bitmap.DFT((r, g, b) => g, null);
            //result.HighPassFilter(0.2);
            //result.IDFT((x, y, value) =>
            //{
            //    newBitmap.SetPiGreenChannel(x, y, (byte)value.FitRange(0, 255));
            //});
            //result = bitmap.DFT((r, g, b) => b, null);
            //result.HighPassFilter(0.2);
            //result.IDFT((x, y, value) =>
            //{
            //    newBitmap.SetPiBlueChannel(x, y, (byte)value.FitRange(0, 255));
            //});
            //newBitmap.Save(string.Format("{0}_highFilter.bmp", index));
            //35
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_31_40\\imori.jpg");
            //index = 35;
            //Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //var result = bitmap.DFT((r, g, b) => r, null);
            //result.BandWidthPassFilter(0.1, 0.5);
            //result.IDFT((x, y, value) =>
            //{
            //    newBitmap.SetPiRedChannel(x, y, (byte)value.FitRange(0, 255));
            //});
            //result = bitmap.DFT((r, g, b) => g, null);
            //result.BandWidthPassFilter(0.1, 0.5);
            //result.IDFT((x, y, value) =>
            //{
            //    newBitmap.SetPiGreenChannel(x, y, (byte)value.FitRange(0, 255));
            //});
            //result = bitmap.DFT((r, g, b) => b, null);
            //result.Ban dWidthPassFilter(0.1, 0.5);
            //result.IDFT((x, y, value) =>
            //{
            //    newBitmap.SetPiBlueChannel(x, y, (byte)value.FitRange(0, 255));
            //});
            //newBitmap.Save(string.Format("{0}_bandWidthFilter.bmp", index));
            //36
            //index = 36;
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_31_40\\imori.jpg");
            //Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //bitmap.DiscreteCosineTransformation(8).InverseDiscreteCosineTransformation(4, 8, (x, y, channel, value) =>
            // {
            //     newBitmap.SetPiChannel(x, y, channel, (byte)value.FitRange(0, 255));
            // });
            //newBitmap.Save(string.Format("{0}_dct.bmp", index));
            //37
            //index = 37;
            //Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_31_40\\imori.jpg");
            //Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //bitmap.DiscreteCosineTransformation(8).InverseDiscreteCosineTransformation(4, 8, (x, y, channel, value) =>
            // {
            //     newBitmap.SetPiChannel(x, y, channel, (byte)value.FitRange(0, 255));
            // });
            //var result = bitmap.PSNR(newBitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            //int k = 1;
            //38            
            //39
            //40
            #endregion
            #region 41-50           
            //41 42 43
            index = 41;
            Bitmap bitmap = new Bitmap("..\\..\\..\\..\\ImageProcessing100Wen-master\\Question_41_50\\imori.jpg");
            bitmap.Canny(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                100, 20,
                (edgeBitmap, angleBitmap) =>
            {
                edgeBitmap.Save(string.Format("{0}_edge.bmp", index));
                angleBitmap.Save(string.Format("{0}_angle.bmp", index));
            },
            edgeBitmap =>
            {
                index = 42;
                edgeBitmap.Save(string.Format("{0}_edge1.bmp", index));
            },
            edgeBitmap =>
            {
                index = 43;
                edgeBitmap.Save(string.Format("{0}_edge2.bmp", index));
            });

            #endregion
        }
    }
}
