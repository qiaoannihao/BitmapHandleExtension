using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class MatrixExtension
    {
        public static double[][] UnitMatrix(int degree)
        {
            double[][] result = new double[degree][];
            for (int y = 0; y < degree; y++)
            {
                var tmp = new double[degree];
                result[y] = tmp;
                tmp[y] = 1;
            }
            return result;
        }
        public static double[][] UnitMatrix(int degree, double rate)
        {
            double[][] result = new double[degree][];
            for (int y = 0; y < degree; y++)
            {
                var tmp = new double[degree];
                result[y] = tmp;
                tmp[y] = rate;
            }
            return result;
        }

        private static double[][] OutputResult(int column, int row, Func<int, int, double> handleFunc)
        {
            double[][] result = new double[row][];
            for (int y = 0; y < row; y++)
            {
                var tmp = new double[column];
                result[y] = tmp;
                for (int x = 0; x < column; x++)
                {
                    tmp[x] = handleFunc(y, x);
                }
            }
            return result;
        }
        public static double[][] Copy(this double[][] data)
        {
            return OutputResult(data[0].Length, data.Length, (y, x) => data[y][x]);
        }
        public static double[][] RowOperation(this double[][] data, int baseRow, int targetRow, Func<double[], double[], double[]> handleFunc)
        {
            double[][] result = data;
            result[targetRow] = handleFunc(result[baseRow], result[targetRow]);
            return result;
        }
        public static double[][] RowOperationForeach(this double[][] data, int baseRow, Func<double[], double[], double[]> handleFunc)
        {
            double[][] result = data;
            var baseRowObject = result[baseRow];
            for (int i = 0; i < data.Length; i++)
            {
                if (i != baseRow)
                {
                    result[i] = handleFunc(baseRowObject, result[i]);
                }
            }
            return result;
        }
        public static double[][] Add(this double[][] data, double[][] value)
        {
            if (!(data.Length == value.Length && data[0].Length == value[0].Length))
            {
                return null;
            }
            return OutputResult(data[0].Length, data.Length, (y, x) => data[y][x] + value[y][x]);
        }
        public static double[][] Dec(this double[][] data, double[][] value)
        {
            if (!(data.Length == value.Length && data[0].Length == value.Length))
            {
                return null;
            }
            return OutputResult(data[0].Length, data.Length, (y, x) => data[y][x] - value[y][x]);
        }
        public static double[][] Mul(this double[][] data, double[][] value)
        {
            if (data[0].Length != value.Length)
            {
                return null;
            }
            var rowMax = data.Length;
            var columnMax = value[0].Length;
            return OutputResult(columnMax, rowMax, (y, x) =>
            {
                var sum = 0.0;
                for (int i = 0; i < value.Length; i++)
                {
                    var tmp = value[i][x];
                    var tmp1 = data[y][i];
                    if (tmp == 0 || tmp1 == 0)
                    {
                        continue;
                    }
                    sum += tmp1 * tmp;
                }
                return sum;
            });
        }
        public static double[][] Mul(this double[][] data, double value)
        {
            return OutputResult(data[0].Length, data.Length, (y, x) => data[y][x] * value);
        }
        public static double[][] Transposition(this double[][] data)
        {
            return OutputResult(data.Length, data[0].Length, (y, x) => data[x][y]);
        }
        public static double[][] Inverse(this double[][] data)
        {
            int column = data[0].Length;
            int row = data.Length;
            if (row != column)
            {
                return null;
            }
            double[][] resultArr = UnitMatrix(row);
            double[][] tmp = data.Copy();
            if (tmp[0][0] == 0)
            {
                for (int i = 1; i < tmp.Length; i++)
                {
                    var arr = tmp[i];
                    if (arr[0] != 0)
                    {
                        tmp[0][0] = arr[0];
                        for (int j = 1; j < arr.Length; j++)
                        {
                            tmp[0][j] += arr[j];
                        }
                        resultArr[0][i] += 1;
                        break;
                    }
                }
            }
            for (int i = 0; i < column; i++)
            {
                var currentRowTmpArr = tmp[i];
                if (currentRowTmpArr[i] != 1)
                {
                    var divisor = currentRowTmpArr[i];
                    if (divisor == 0)
                    {
                        return null;
                    }
                    for (int k = i + 1; k < column; k++)
                    {
                        currentRowTmpArr[k] /= divisor;
                    }
                    var currentRowResultArr = resultArr[i];
                    for (int k = 0; k < column; k++)
                    {
                        currentRowResultArr[k] /= divisor;
                    }
                    currentRowTmpArr[i] = 1;
                }
                for (int j = i + 1; j < row; j++)
                {
                    var currentHandleRowTmpArr = tmp[j];
                    var divisor = currentHandleRowTmpArr[i];
                    for (int k = i + 1; k < column; k++)
                    {
                        currentHandleRowTmpArr[k] -= divisor * currentRowTmpArr[k];
                    }
                    var currentRowResultArr = resultArr[j];
                    var currentRowResultBaseAtt = resultArr[i];
                    for (int k = 0; k < column; k++)
                    {
                        currentRowResultArr[k] -= divisor * currentRowResultBaseAtt[k];
                    }
                    currentHandleRowTmpArr[i] = 0;
                }
            }
            for (int i = 0; i < column; i++)
            {
                if (tmp[i][i] == 0)
                {
                    return null;
                }
            }
            for (int i = column - 1; i > -1; i--)
            {
                for (int j = i - 1; j > -1; j--)
                {
                    var currentHandleRowTmpArr = tmp[j];
                    var divisor = currentHandleRowTmpArr[i];
                    var currentRowResultArr = resultArr[j];
                    var currentRowResultBaseAtt = resultArr[i];
                    for (int k = 0; k < column; k++)
                    {
                        currentRowResultArr[k] -= divisor * currentRowResultBaseAtt[k];
                    }
                }
            }
            return resultArr;
        }
        public static void Show(double[][] da)
        {
            for (int i = 0; i < da.Length; i++)
            {
                for (int j = 0; j < da[0].Length; j++)
                {

                    Console.Write("{0} ", da[i][j].Fit(2).ToString().PadLeft(5));
                }
                Console.WriteLine();
            }
        }
        public static double[][] DivL(this double[][] data, double[][] value)
        {
            var res = value.Inverse();
            if (res == null)
            {
                return null;
            }
            return res.Mul(data);
        }
        public static double[][] DivR(this double[][] data, double[][] value)
        {
            var res = value.Inverse();
            if (res == null)
            {
                return null;
            }
            return data.Mul(res);
        }

    }
}
