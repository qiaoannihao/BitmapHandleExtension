using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class _2DTran
    {
        public static double[][] CreateVector(double x, double y)
        {
            return new double[][]
            {
                new double[]{x},
                new double[]{y},
                new double[]{1},
            };
        }
        public static double[][] Rotate(double angle)
        {
            return new double[][]
            {
                new double[]{Math.Cos(angle),-Math.Sin(angle),0 },
                new double[]{Math.Sin(angle),Math.Cos(angle),0 },
                new double[]{0,0,1},
            };
        }

        public static double[][] GetTransformMatrix(double x, double y)
        {
            return new double[][]
            {
                new double[]{1,0,x},
                new double[]{0,1,y},
                new double[]{0,0,1},
            };
        }
        public static double[][] RotateFollowZero(this double[][] data, double angle)
        {
            return Rotate(angle).Mul(data);
        }
        public static double[][] Transform(this double[][] data, double x, double y)
        {
            return GetTransformMatrix(x, y).Mul(data);
        }
        public static double[][] Transform(this double[][] data, params double[][][] value)
        {
            double[][] res = value[0];
            for (int i = 1; i < value.Length; i++)
            {
                res = res.Mul(value[i]);
            }
            return res.Mul(data);
        }
    }
}
