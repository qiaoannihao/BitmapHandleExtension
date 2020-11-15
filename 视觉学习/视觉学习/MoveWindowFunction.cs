using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 视觉学习
{
    public class _2dArrayHandleFunction
    {
        public static void ImageSegmentation(Rectangle rectangle, int poolWidth, int poolHeight, Action<Rectangle> callback)
        {
            ImageSegmentation(rectangle, poolWidth, poolHeight, (x, y, rec) =>
            {
                callback(rec);
            });
        }

        public static void ImageSegmentation(Rectangle rectangle, int poolWidth, int poolHeight, Action<int, int, Rectangle> callback)
        {
            double columnCount = rectangle.Width / poolWidth;
            double rowCount = rectangle.Height / poolHeight;
            int columnCurrentCount = 0;
            int rowCurrentCount = 0;
            int columnValue = rectangle.X;
            int rowValue = rectangle.Y;
            Rectangle[] rectangles;
            rectangles = new Rectangle[(int)(columnCount + 0.5)];
            while (columnCurrentCount < columnCount)
            {
                var current = new Rectangle()
                {
                    X = columnValue,
                    Y = rowValue,
                    Height = poolHeight,
                    Width = poolWidth
                };
                columnValue += poolWidth;
                callback(columnCurrentCount, rowCurrentCount, current);
                rectangles[columnCurrentCount] = current;
                columnCurrentCount++;
            }
            if (rectangle.Width % poolWidth > 0)
            {
                var current = new Rectangle()
                {
                    X = columnValue,
                    Y = rowValue,
                    Height = poolHeight,
                    Width = rectangle.Width - columnValue
                };
                callback(columnCurrentCount, rowCurrentCount, current);
                rectangles[columnCurrentCount] = current;
            }
            rowValue += poolHeight;
            rowCurrentCount++;
            while (rowCurrentCount < rowCount)
            {
                Rectangle item;
                for (int i = 0; i < rectangles.Length; i++)
                {
                    item = rectangles[i];
                    item.Y = rowValue;
                    callback(i, rowCurrentCount, item);
                }
                rowValue += poolHeight;
                rowCurrentCount++;
            }
            if (rectangle.Height % poolHeight > 0)
            {
                Rectangle item;
                int height = rectangle.Height - rowValue;
                for (int i = 0; i < rectangles.Length; i++)
                {
                    item = rectangles[i];
                    item.Y = rowValue;
                    item.Height = height;
                    callback(i, rowCurrentCount, item);
                }
            }
        }
        public static void ImageSegmentation(Rectangle rectangle, int poolWidth, int poolHeight, int columnStep, int rowStep, Action<int, int, Rectangle> callback)
        {
            double columnCount = (rectangle.Width - poolWidth + columnStep) / columnStep;
            double rowCount = (rectangle.Height - poolHeight + rowStep) / rowStep;
            int columnCurrentCount = 0;
            int rowCurrentCount = 0;
            int columnValue = rectangle.X;
            int rowValue = rectangle.Y;
            int xEnd = rectangle.Right;
            int yEnd = rectangle.Bottom;
            Rectangle[] rectangles;
            rectangles = new Rectangle[(int)(columnCount + 0.5)];
            while (columnCurrentCount < columnCount)
            {
                var current = new Rectangle()
                {
                    X = columnValue,
                    Y = rowValue,
                    Height = poolHeight,
                    Width = poolWidth
                };
                columnValue += columnStep;
                callback(columnCurrentCount, rowCurrentCount, current);
                rectangles[columnCurrentCount] = current;
                columnCurrentCount++;
            }
            if (columnCount.GetIntegersAndDecimals((s1, s2) => s2) > 0)
            {
                var current = new Rectangle()
                {
                    X = columnValue,
                    Y = rowValue,
                    Height = poolHeight,
                    Width = rectangle.Width - columnValue
                };
                callback(columnCurrentCount, rowCurrentCount, current);
                rectangles[columnCurrentCount] = current;
            }
            rowValue += rowStep;
            rowCurrentCount++;
            while (rowCurrentCount < rowCount)
            {
                Rectangle item;
                for (int i = 0; i < rectangles.Length; i++)
                {
                    item = rectangles[i];
                    item.Y = rowValue;
                    callback(i, rowCurrentCount, item);
                }
                rowValue += rowStep;
                rowCurrentCount++;
            }
            if (rowCount.GetIntegersAndDecimals((s1, s2) => s2) > 0)
            {
                Rectangle item;
                int height = rectangle.Height - rowValue;
                for (int i = 0; i < rectangles.Length; i++)
                {
                    item = rectangles[i];
                    item.Y = rowValue;
                    item.Height = height;
                    callback(i, rowCurrentCount, item);
                }
            }
        }

        public static void _2dArrayMoveWindowFunction<T>(T[][] array,
            Rectangle rectangle,
            int leftWidth, int rightWidth, int topHeight, int bottomHeight,
            Func<T> fillValueFunc,
            Action<int, int, T, Func<Func<int, int, T, bool>, bool>, Action<T>> handleFunc,
            int windowPixLocationType = 0, bool outReturnFlag = true)
        {
            int arrayWidth = array[0].Length;
            int arrayHeight = array.Length;
            if (rectangle.X < 0)
            {
                rectangle.X = 0;
            }
            if (rectangle.Y < 0)
            {
                rectangle.Y = 0;
            }
            if (rectangle.Right > arrayWidth)
            {
                rectangle.Width = arrayWidth - rectangle.X;
            }
            if (rectangle.Bottom > arrayHeight)
            {
                rectangle.Height = arrayHeight - rectangle.Y;
            }
            var fillValue = fillValueFunc();
            T[] item = null;
            int y = 0, x = 0, yy = 0, xx = 0, dy = 0, dx = 0;

            Action<T> action = (value) =>
            {
                array[y][x] = value;
            };
            Func<int> xxGetter = () =>
            {
                return xx;
            };
            Func<int> yyGetter = () =>
            {
                return yy;
            };
            if (windowPixLocationType == 1)
            {
                xxGetter = () =>
                {
                    return xx + leftWidth;
                };
                yyGetter = () =>
                {
                    return yy + topHeight;
                };
            }
            else if (windowPixLocationType == 2)
            {
                xxGetter = () =>
                {
                    return dx;
                };
                yyGetter = () =>
                {
                    return dy;
                };
            }
            int yEnd = rectangle.Bottom;
            int xEnd = rectangle.Right;
            int xEnd_1 = xEnd - 1, yEnd_1 = yEnd - 1;
            bool yOutRectangleFlag = false;
            Func<Func<int, int, T, bool>, bool> getter = ss =>
               {
                   for (yy = -topHeight; yy < bottomHeight + 1; yy++)
                   {
                       dy = y + yy;
                       if (dy < rectangle.Y || dy > yEnd_1)
                       {
                           yOutRectangleFlag = true;
                       }
                       else
                       {
                           item = array[dy];
                           yOutRectangleFlag = false;
                       }
                       for (xx = -leftWidth; xx < rightWidth + 1; xx++)
                       {
                           dx = x + xx;
                           if (yOutRectangleFlag || dx < rectangle.X || dx > xEnd_1)
                           {
                               if (outReturnFlag)
                               {
                                   if (!ss(xxGetter(), yyGetter(), fillValue))
                                   {
                                       return false;
                                   }
                               }
                           }
                           else
                           {
                               if (!ss(xxGetter(), yyGetter(), item[dx]))
                               {
                                   return false;
                               }
                           }
                       }
                   }
                   return true;
               };

            T[] currentItem;
            for (y = rectangle.Top; y < yEnd; y++)
            {
                currentItem = array[y];
                for (x = rectangle.Left; x < xEnd; x++)
                {
                    handleFunc(x, y, currentItem[x], getter, action);
                }
            }
        }

        public static void _2dArrayMoveWindowFunction<T>(T[][] array,
            Rectangle rectangle,
            Action<int, int, T, Action<int, int, Action<T>>, Action<T>> handleFunc, bool outReturnFlag = false)
        {
            int arrayWidth = array[0].Length;
            int arrayHeight = array.Length;
            if (rectangle.X < 0)
            {
                rectangle.X = 0;
            }
            if (rectangle.Y < 0)
            {
                rectangle.Y = 0;
            }
            if (rectangle.Right > arrayWidth)
            {
                rectangle.Width = arrayWidth - rectangle.X;
            }
            if (rectangle.Bottom > arrayHeight)
            {
                rectangle.Height = arrayHeight - rectangle.Y;
            }
            int y = 0, x = 0, xEnd = rectangle.Right, yEnd = rectangle.Bottom;
            int xEnd_1 = xEnd - 1, yEnd_1 = yEnd - 1;
            Action<T> action = (value) =>
            {
                array[y][x] = value;
            };
            Action<int, int, Action<T>> getter = (offsetX, offsetY, callback) =>
            {
                int xTmp = x + offsetX;
                if (xTmp < rectangle.X || xTmp > xEnd_1)
                {
                    if (outReturnFlag)
                    {
                        return;
                    }
                    else
                    {
                        if (xTmp < rectangle.X)
                        {
                            xTmp = rectangle.X;
                        }
                        else if (xTmp > xEnd_1)
                        {
                            xTmp = xEnd_1;
                        }
                    }
                }
                int yTmp = y + offsetY;
                if (yTmp < rectangle.Y || yTmp > yEnd_1)
                {
                    if (outReturnFlag)
                    {
                        return;
                    }
                    else
                    {
                        if (yTmp < rectangle.Y)
                        {
                            yTmp = rectangle.Y;
                        }
                        else if (yTmp > xEnd_1)
                        {
                            yTmp = yEnd_1;
                        }
                    }
                }
                callback(array[yTmp][xTmp]);
            };
            T[] currentItem;
            for (y = rectangle.Y; y < yEnd; y++)
            {
                currentItem = array[y];
                for (x = rectangle.Left; x < xEnd; x++)
                {
                    handleFunc(x, y, currentItem[x], getter, action);
                }
            }
        }

        public static void _2dArrayForeach<T>(T[][] array,
            Rectangle rectangle,
            Action<int, int, T, Action<T>> callback)
        {
            int arrayWidth = array[0].Length;
            int arrayHeight = array.Length;
            if (rectangle.X < 0)
            {
                rectangle.X = 0;
            }
            if (rectangle.Y < 0)
            {
                rectangle.Y = 0;
            }
            if (rectangle.Right > arrayWidth)
            {
                rectangle.Width = arrayWidth - rectangle.X;
            }
            if (rectangle.Bottom > arrayHeight)
            {
                rectangle.Height = arrayHeight - rectangle.Y;
            }
            int y = 0, x = 0, xEnd = rectangle.Right, yEnd = rectangle.Bottom;
            T[] currentItem = null;
            Action<T> setter = s =>
            {
                currentItem[x] = s;
            };
            for (y = rectangle.Y; y < yEnd; y++)
            {
                currentItem = array[y];
                for (x = rectangle.Left; x < xEnd; x++)
                {
                    callback(x, y, currentItem[x], setter);
                }
            }
        }
    }


}
