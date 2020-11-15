using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 视觉学习
{
    public class _2DArraryFunction
    {
        public static void Foreach(IntPtr intPtr, int pointerStep, int startX, int endX, int startY, int endY,
            Action<int, int, IntPtr> callback, Func<IntPtr, IntPtr> nextRow = null)
        {
            if (nextRow == null)
            {
                nextRow = s => s;
            }
            while (startY < endY)
            {
                while (startX < endX)
                {
                    callback(startX, startY, intPtr);
                    intPtr += pointerStep;
                    startX++;
                }
                intPtr = nextRow(intPtr);
                startY++;
            }
        }

        public static void ForeachMoveWindow(IntPtr intPtr, int pointerStep,
            int startX, int endX, int startY, int endY,
            int leftWidth, int rightWidth, int topHeight, int bottomHeight,
            Action<int, int, IntPtr, Func<Func<int, int, IntPtr, bool>, bool>> callback,
            int windowPixLocationType = 0, bool outReturnFlag = true,
            Func<IntPtr, IntPtr> nextRow = null,
            Func<IntPtr, IntPtr> nextWindowRow = null)
        {
            if (nextRow == null)
            {
                nextRow = s => s;
            }
            int x = 0, y = 0, dx = 0, dy = 0;
            Func<int> xxGetter = () => x;
            Func<int> yyGetter = () => y;
            if (windowPixLocationType == 1)
            {
                xxGetter = () => x + leftWidth;
                yyGetter = () => y + topHeight;
            }
            else if (windowPixLocationType == 2)
            {
                xxGetter = () => dx;
                yyGetter = () => dy;
            }
            bottomHeight++;
            rightWidth++;
            bool yOutFlag = false;
            IntPtr tmp = IntPtr.Zero;
            Func<Func<int, int, IntPtr, bool>, bool> getter = s =>
            {
                tmp = intPtr;
                for (y = -topHeight; y < bottomHeight; y++)
                {
                    dy = startY + y;
                    if (dy < startY || dy > endY)
                    {
                        yOutFlag = true;
                    }
                    else
                    {
                        yOutFlag = false;
                    }
                    for (x = -leftWidth; x < rightWidth; x++)
                    {
                        dx = startX + x;
                        if (yOutFlag || dx < startX || dx > endX)
                        {
                            if (outReturnFlag)
                            {
                                if (!s(xxGetter(), yyGetter(), IntPtr.Zero))
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            if (!s(xxGetter(), yyGetter(), tmp))
                            {
                                return false;
                            }
                        }
                        tmp += pointerStep;
                    }
                    tmp = nextWindowRow(tmp);
                }
                return true;
            };
            while (startY < endY)
            {
                while (startX < endX)
                {
                    callback(startX, startY, intPtr, getter);
                    intPtr += pointerStep;
                    startX++;
                }
                intPtr = nextRow(intPtr);
                startY++;
            }
        }
    }
}
