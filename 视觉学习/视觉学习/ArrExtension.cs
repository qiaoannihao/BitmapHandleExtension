using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ArrExtension
    {

        public static T FindMedian<T>(this T[] data, int k, int start, int end, Func<T, T, bool> compare)
        {
            T[] buffer = new T[data.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = data[i];
            }
            return buffer.FindMedianChangeSelf(k, start, end, compare);
        }
        public static T FindMedianChangeSelf<T>(this T[] data, int k, int start, int end, Func<T, T, bool> compare)
        {
            if (start < end)
            {
                var x = data[start];
                var i = start;
                var j = end;
                while (i < j)
                {
                    while (i < j && compare(data[j], x))
                        j--;

                    if (i < j)
                    {
                        data[i] = data[j];
                        i++;
                    }

                    while (i < j && !compare(data[i], x))
                        i++;

                    if (i < j)
                    {
                        data[j] = data[i];
                        j--;
                    }
                }
                if (k == i)
                {
                    return x;
                }

                if (k > i)
                {
                    return data.FindMedianChangeSelf(k, i + 1, end, compare);
                }
                else
                {
                    return data.FindMedianChangeSelf(k, start, i - 1, compare);
                }
            }
            return data[start];
        }
        /// <summary>
        /// 中值处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="compare"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T[] Median<T>(this T[] data, Func<T, T, bool> compare, int count = 5)
        {
            var dataArr = data;
            for (int i = 0; i < dataArr.Length; i++)
            {
                if (i >= count && i < dataArr.Length - count)
                {
                    var tmp = new T[count * 2 + 1];
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        tmp[j] = dataArr[i - count + j];
                    }
                    tmp.Sort(0, tmp.Length - 1, compare);
                    //取中值
                    var mid = tmp[count];
                    dataArr[i] = mid;
                }
            }
            return dataArr;
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static T[] Sort<T>(this T[] data, Func<T, T, bool> compare)
        {
            return data.Sort(0, data.Length - 1, compare);
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static T[] Sort<T>(this T[] data, int start, int end, Func<T, T, bool> compare)
        {
            if (start < end)
            {
                var x = data[start];
                var i = start;
                var j = end;
                while (i < j)
                {
                    while (i < j && compare(data[j], x))
                        j--;

                    if (i < j)
                    {
                        data[i] = data[j];
                        i++;
                    }

                    while (i < j && !compare(data[i], x))
                        i++;

                    if (i < j)
                    {
                        data[j] = data[i];
                        j--;
                    }
                }
                data[i] = x;
                data.Sort(start, i - 1, compare);
                data.Sort(i + 1, end, compare);
            }
            return data;
        }
        /// <summary>
        /// 自动扩展长度至大于当前长度的最小2的幂次方
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T[] FitTwoPowerLength<T>(this T[] data, Func<T> func)
        {
            if (func == null)
            {
                throw new Exception("func为空");
            }
            int length = 2;
            while (length < data.Length)
            {
                length *= 2;
            }
            T[] result = new T[length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = data[i];
            }
            for (int i = data.Length; i < length; i++)
            {
                result[i] = func();
            }
            return result;
        }
        /// <summary>
        /// 扩展长度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T[] FitLength<T>(this T[] data, int length, Func<T> func)
        {
            if (data.Length >= length)
            {
                throw new Exception("数组长度小于填充长度");
            }
            if (func == null)
            {
                throw new Exception("func为空");
            }
            T[] result = new T[length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = data[i];
            }
            for (int i = data.Length; i < length; i++)
            {
                result[i] = func();
            }
            return result;
        }

        /// <summary>
        /// 截取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T[] CutOut<T>(this T[] data, int start, int count)
        {
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = data[i + start];
            }
            return result;
        }
        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="array"></param>
        /// <param name="action"></param>
        public static void ForEach<TValue>(this TValue[] array, Action<int, TValue> action)
        {
            for (int i = 0; i < array.Length; i++)
            {
                action(i, array[i]);
            }
        }
        /// <summary>
        /// 寻找符合条件的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="compareFunc"></param>
        /// <returns></returns>
        public static Tuple<int, T> FindItem<T>(this T[] list, Func<T, T, bool> compareFunc)
        {
            int index = 0;
            T outItem = list[index];
            for (int i = 1; i < list.Length; i++)
            {
                var item = list[i];
                if (compareFunc(outItem, item))
                {
                    outItem = item;
                    index = i;
                }
            }
            return new Tuple<int, T>(index, outItem);
        }
        public static int Indexof<T>(this T[] arr, T value, int start, Func<T, T, bool> compare)
        {
            if (start < arr.Length)
            {
                for (int i = start; i < arr.Length; i++)
                {
                    var item = arr[i];
                    if (compare(item, value))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        public static int Indexof<T>(this T[] arr, T[] value, int start, Func<T, T, bool> compare)
        {
            int end = arr.Length - value.Length + 1;
            int j;
            if (start < arr.Length || start < end)
            {
                for (int i = start; i < end; i++)
                {
                    for (j = 0; j < value.Length; j++)
                    {
                        if (!compare(arr[i + j], value[j]))
                        {
                            break;
                        }
                    }
                    if (j == value.Length)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        public static void Insert<T>(this T[] arr, T value, int index)
        {
            if (index < 0 || index > arr.Length - 1)
            {
                return;
            }
            for (int i = arr.Length - 1; i > index; i++)
            {
                arr[i] = arr[i - 1];
            }
            arr[index] = value;
        }
    }

    public static class IListExtension
    {
        public static T FindMedian<T>(this IList<T> data, int k, int start, int end, Func<T, T, bool> compare)
        {
            T[] buffer = new T[data.Count];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = data[i];
            }
            return buffer.FindMedianChangeSelf(k, start, end, compare);
        }
        public static T FindMedianChangeSelf<T>(this IList<T> data, int k, int start, int end, Func<T, T, bool> compare)
        {
            if (start < end)
            {
                var x = data[start];
                var i = start;
                var j = end;
                while (i < j)
                {
                    while (i < j && compare(data[j], x))
                        j--;

                    if (i < j)
                    {
                        data[i] = data[j];
                        i++;
                    }

                    while (i < j && !compare(data[i], x))
                        i++;

                    if (i < j)
                    {
                        data[j] = data[i];
                        j--;
                    }
                }
                if (k == i)
                {
                    return x;
                }

                if (k > i)
                {
                    return data.FindMedianChangeSelf(k, i + 1, end, compare);
                }
                else
                {
                    return data.FindMedianChangeSelf(k, start, i - 1, compare);
                }
            }
            return data[start];
        }

        /// <summary>
        /// 中值处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="compare"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IList<T> Median<T>(this IList<T> data, Func<T, T, bool> compare, int count = 5)
        {
            var dataArr = data;
            for (int i = 0; i < dataArr.Count; i++)
            {
                if (i >= count && i < dataArr.Count - count)
                {
                    var tmp = new T[count * 2 + 1];
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        tmp[j] = dataArr[i - count + j];
                    }
                    tmp.Sort(0, tmp.Length - 1, compare);
                    //取中值
                    var mid = tmp[count];
                    dataArr[i] = mid;
                }
            }
            return dataArr;
        }
        /// <summary>
        /// 寻找符合条件的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="compareFunc"></param>
        /// <returns></returns>
        public static Tuple<int, T> FindItem<T>(this IList<T> list, Func<T, T, bool> compareFunc)
        {
            int index = 0;
            T outItem = list[index];
            for (int i = 1; i < list.Count; i++)
            {
                var item = list[i];
                if (compareFunc(outItem, item))
                {
                    outItem = item;
                    index = i;
                }
            }
            return new Tuple<int, T>(index, outItem);
        }
        /// <summary>
        /// 遍历
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="array"></param>
        /// <param name="action"></param>
        public static void ForEach<TValue>(this IList<TValue> array, Action<int, TValue> action)
        {
            int index = 0;
            foreach (TValue item in array)
            {
                action(index, item);
                index++;
            }
        }
        /// <summary>
        /// 截取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IList<T> CutOut<T>(this IList<T> data, int start, int count)
        {
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = data[i + start];
            }
            return result;
        }
        /// <summary>
        /// 保留有效数字
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IList<T> FitLength<T>(this IList<T> data, int length, Func<T> func)
        {
            if (data.Count >= length)
            {
                throw new Exception("数组长度小于填充长度");
            }
            if (func == null)
            {
                throw new Exception("func为空");
            }
            T[] result = new T[length];
            for (int i = 0; i < data.Count; i++)
            {
                result[i] = data[i];
            }
            for (int i = data.Count; i < length; i++)
            {
                result[i] = func();
            }
            return result;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static IList<T> Sort<T>(this IList<T> data, Func<T, T, bool> compare)
        {
            return data.Sort(0, data.Count - 1, compare);
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static IList<T> Sort<T>(this IList<T> data, int start, int end, Func<T, T, bool> compare)
        {
            if (start < end)
            {
                var x = data[start];
                var i = start;
                var j = end;
                while (i < j)
                {
                    while (i < j && compare(data[j], x))
                        j--;

                    if (i < j)
                    {
                        data[i] = data[j];
                        i++;
                    }

                    while (i < j && !compare(data[i], x))
                        i++;

                    if (i < j)
                    {
                        data[j] = data[i];
                        j--;
                    }

                }
                data[i] = x;
                data.Sort(start, i - 1, compare);
                data.Sort(i + 1, end, compare);
            }
            return data;
        }
    }

    public static class IDictionaryExtension
    {
        /// <summary>
        /// 遍历
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="array"></param>
        /// <param name="action"></param>
        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> array, Action<int, TKey, TValue> action)
        {
            int index = 0;
            foreach (var item in array)
            {
                action(index, item.Key, item.Value);
                index++;
            }
        }
        /// <summary>
        /// 寻找符合条件的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="compareFunc"></param>
        /// <returns></returns>
        public static Tuple<TKey, TValue> FindItem<TKey, TValue>(this IDictionary<TKey, TValue> array,
            Func<TKey, TValue, TKey, TValue, bool> compareFunc)
        {
            var keys = array.Keys.ToArray();
            var key = keys[0];
            TValue outItem = array[key];
            for (int i = 1; i < keys.Length; i++)
            {
                var item = keys[i];
                var itemValue = array[item];
                if (compareFunc(key, outItem, item, itemValue))
                {
                    key = item;
                    outItem = itemValue;
                }
            }
            return new Tuple<TKey, TValue>(key, outItem);
        }
    }
}
