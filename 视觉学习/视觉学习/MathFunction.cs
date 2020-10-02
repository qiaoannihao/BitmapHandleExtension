using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 视觉学习
{
    public class MathFunction
    {
        public static double[][] GaussianDistribution(int x, int y, double sigma)
        {
            double[][] result = new double[y][];
            double standardDeviationPower = sigma * sigma;
            double standardDeviationPowerDouble = standardDeviationPower + standardDeviationPower;
            double standardDeviationPowerDouble_Pi = standardDeviationPowerDouble * Math.PI;
            double standardDeviationPowerDouble_Pi_reverse = 1 / standardDeviationPowerDouble_Pi;
            double kSum = 0;
            double[] item;
            int halfX = x >> 1;
            int halfY = y >> 1;
            for (int i = 0; i < y; i++)
            {
                item = new double[x];
                for (int j = 0; j < x; j++)
                {
                    var xx = j - halfX;
                    var yy = i - halfY;
                    var value = standardDeviationPowerDouble_Pi_reverse * Math.Pow(Math.E, -(xx * xx + yy * yy) / standardDeviationPowerDouble);
                    kSum += value;
                    item[j] = value;
                }
                result[i] = item;
            }
            for (int i = 0; i < y; i++)
            {
                item = result[i];
                for (int j = 0; j < x; j++)
                {
                    item[j] /= kSum;
                }
            }
            return result;
        }
        public static double[][] LaplaciainOfGaussian(int x, int y, double sigma)
        {
            double[][] result = new double[y][];
            double standardDeviationPower = sigma * sigma;
            double standardDeviationPowerDouble = standardDeviationPower + standardDeviationPower;
            double standardDeviationPowerDouble_Pi = standardDeviationPowerDouble * Math.PI;
            double standardDeviationPowerDouble_Pi_reverse = 1 / (standardDeviationPowerDouble_Pi * standardDeviationPower * standardDeviationPower);
            double tmp = 0;

            double kSum = 0;
            double[] item;
            int halfX = x >> 1;
            int halfY = y >> 1;
            for (int i = 0; i < y; i++)
            {
                item = new double[x];
                for (int j = 0; j < x; j++)
                {
                    var xx = j - halfX;
                    var yy = i - halfY;
                    tmp = xx * xx + yy * yy;
                    var value = (tmp - standardDeviationPower) * standardDeviationPowerDouble_Pi_reverse * Math.Pow(Math.E, -tmp / standardDeviationPowerDouble);
                    kSum += value;
                    item[j] = value;
                }
                result[i] = item;
            }
            for (int i = 0; i < y; i++)
            {
                item = result[i];
                for (int j = 0; j < x; j++)
                {
                    item[j] /= kSum;
                }
            }
            return result;
        }
    }
}
