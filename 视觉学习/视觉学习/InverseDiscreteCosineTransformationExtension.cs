using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 视觉学习
{
    public static class InverseDiscreteCosineTransformationExtension
    {
        /// <summary>
        /// 离散余弦逆变换
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="K"></param>
        /// <param name="T"></param>
        /// <param name="spectrogram"></param>
        public static void InverseDiscreteCosineTransformation(this double[][][] buffer, int K, int T, Action<int, int, int, double> spectrogram)
        {
            double _1__sqrt2 = Math.Sqrt(2) / 2;
            Func<int, double> cFunc = s => s == 0 ? _1__sqrt2 : 1;
            double _pi__2T = Math.PI / (T << 1);
            double _2__T = 2.0 / K;
            double f = 0;
            int y, x, channel, u, v, yy, xx;
            int height = buffer.Length;
            int width = buffer[0].Length;
            for (y = 0; y < height; y += T)
            {
                for (x = 0; x < width; x += T)
                {
                    for (channel = 0; channel < 3; channel++)
                    {
                        for (yy = 0; yy < T; yy++)
                        {
                            for (xx = 0; xx < T; xx++)
                            {
                                f = 0;
                                for (v = 0; v < K; v++)
                                {
                                    for (u = 0; u < K; u++)
                                    {
                                        f += cFunc(u)
                                            * cFunc(v)
                                            * buffer[y + v][x + u][channel]
                                            * Math.Cos(((xx << 1) + 1) * u * _pi__2T)
                                            * Math.Cos(((yy << 1) + 1) * v * _pi__2T);
                                    }
                                }
                                spectrogram(x + xx, y + yy, channel, f * _2__T);
                            }
                        }
                    }
                }
            }
        }
    }
}
