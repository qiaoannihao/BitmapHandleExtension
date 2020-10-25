using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 视觉学习
{
    public class MoveWindowFunction
    {
        public static void ImageSegmentation(Rectangle rectangle, int poolWidth, int poolHeight, Action<Rectangle> callback)
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
                callback(current);
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
                callback(current);
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
                    callback(item);
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
                    callback(item);
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
    }


}
