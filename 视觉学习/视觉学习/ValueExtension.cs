using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 视觉学习
{
    public static class ValueExtension
    {
        public static T FitRange<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                return min;
            }
            else if (value.CompareTo(max) > 0)
            {
                return max;
            }
            return value;
        }
    }
}
