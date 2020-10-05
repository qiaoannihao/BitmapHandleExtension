using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class DoubleExtension
    {
        /// <summary>
        /// 保留有效位数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static double Fit(this double value, int count = 2)
        {
            return double.Parse(value.ToString(string.Format("F{0}", count)));
        }

        /// <summary>
        /// 获得整数和小数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Tuple<int, double> GetIntegersAndDecimals(this double value)
        {
            int intValue = (int)value;
            return new Tuple<int, double>(intValue, value - intValue);
        }

        /// <summary>
        /// 获得整数和小数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetIntegersAndDecimals<T>(this double value, Func<int, double, T> callback)
        {
            int intValue = (int)value;
            return callback(intValue, value - intValue);
        }
    }
}
