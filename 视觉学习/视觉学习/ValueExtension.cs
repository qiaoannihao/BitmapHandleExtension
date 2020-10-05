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
        //public static int Range(this int value, int min, int max)
        //{
        //    if (value < min)
        //    {
        //        return min;
        //    }
        //    else if (value > max)
        //    {
        //        return max;
        //    }
        //    return value;
        //}
        //public static uint Range(this uint value, uint min, uint max)
        //{
        //    if (value < min)
        //    {
        //        return min;
        //    }
        //    else if (value > max)
        //    {
        //        return max;
        //    }
        //    return value;
        //}
        //public static double Range(this double value, double min, double max)
        //{
        //    if (value < min)
        //    {
        //        return min;
        //    }
        //    else if (value > max)
        //    {
        //        return max;
        //    }
        //    return value;
        //}
        //public static float Range(this float value, float min, float max)
        //{
        //    if (value < min)
        //    {
        //        return min;
        //    }
        //    else if (value > max)
        //    {
        //        return max;
        //    }
        //    return value;
        //}
        //public static byte Range(this byte value, byte min, byte max)
        //{
        //    if (value < min)
        //    {
        //        return min;
        //    }
        //    else if (value > max)
        //    {
        //        return max;
        //    }
        //    return value;
        //}
        //public static ushort Range(this ushort value, ushort min, ushort max)
        //{
        //    if (value < min)
        //    {
        //        return min;
        //    }
        //    else if (value > max)
        //    {
        //        return max;
        //    }
        //    return value;
        //}
        //public static short Range(this short value, short min, short max)
        //{
        //    if (value < min)
        //    {
        //        return min;
        //    }
        //    else if (value > max)
        //    {
        //        return max;
        //    }
        //    return value;
        //}
        //public static sbyte Range(this sbyte value, sbyte min, sbyte max)
        //{
        //    if (value < min)
        //    {
        //        return min;
        //    }
        //    else if (value > max)
        //    {
        //        return max;
        //    }
        //    return value;
        //}
        //public static long Range(this long value, long min, long max)
        //{
        //    if (value < min)
        //    {
        //        return min;
        //    }
        //    else if (value > max)
        //    {
        //        return max;
        //    }
        //    return value;
        //}
        //public static ulong Range(this ulong value, ulong min, ulong max)
        //{
        //    if (value < min)
        //    {
        //        return min;
        //    }
        //    else if (value > max)
        //    {
        //        return max;
        //    }
        //    return value;
        //}

    }
}
