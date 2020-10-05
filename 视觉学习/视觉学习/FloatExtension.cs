using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class FloatExtension
    {
        /// <summary>
        /// 保留有效位数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static float Fit(this float value, int count = 2)
        {
            return float.Parse(value.ToString(string.Format("F{0}", count)));
        }
        /// <summary>
        /// 获得整数和小数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Tuple<int, float> GetIntegersAndDecimals(this float value)
        {
            int intValue = (int)value;
            return new Tuple<int, float>(intValue, value - intValue);
        }
        /// <summary>
        /// 获得整数和小数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetIntegersAndDecimals<T>(this float value, Func<int, float, T> callback)
        {
            int intValue = (int)value;
            return callback(intValue, value - intValue);
        }
    }
}
