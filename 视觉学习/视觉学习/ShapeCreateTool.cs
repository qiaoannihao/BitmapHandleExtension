using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ShapeCreateTool
    {
        public static void CreateCircle(int centerX, int centerY, int radius,
            double startAngle, double endAngle,
            Action<int, int> action)
        {
            int size = (int)(0.5 * Math.Sqrt(2) * radius + 0.5);
            double angleStep = 0.25 * Math.PI / size;
            int[] x = new int[size * 2];
            int[] y = new int[x.Length];

            bool direction = endAngle - startAngle > 0;
            double __90 = -Math.PI / 2;
            double _360 = 2 * Math.PI;
            Func<double, double, double> addAction = null;
            Action<int, int, Action<int, int>>[] symbolActions = new Action<int, int, Action<int, int>>[]{
                (xxx, yyy, set) =>set(xxx,yyy),
                (xxx, yyy, set) =>set(xxx,-yyy),
                (xxx, yyy, set) =>set(-xxx,-yyy),
                (xxx, yyy, set) =>set(-xxx,yyy)
            };
            Func<int, int> symbolDirection = null;
            Func<double, double, bool> compare = null;
            Func<double, double, bool> compare1 = null;


            int symbolIndex = 0;
            double currentAngle = __90;
            if (direction)
            {
                while (startAngle < __90)
                {
                    startAngle += _360;
                    endAngle += _360;
                }
                addAction = (a, b) => a + b;
                symbolDirection = (s) => s + 1;
                compare = (a, b) => a >= b;
                compare1 = (a, b) => a > b;
            }
            else
            {
                symbolIndex = 3;
                while (startAngle > __90)
                {
                    startAngle -= _360;
                    endAngle -= _360;
                }
                addAction = (a, b) => a - b;
                symbolDirection = (s) => s - 1;
                compare = (a, b) => a <= b;
                compare1 = (a, b) => a < b;
            }
            Action<int, int, Action<int, int>> symboolAction = symbolActions[symbolIndex];
            //第一象限90-45                
            int p = 3 - 2 * radius;
            int xx = 0;
            int yy = radius;

            while (xx < size)
            {
                x[xx] = xx;
                y[xx] = yy;
                symboolAction(xx, yy, (xx1, yy1) =>
                {
                    if (compare(currentAngle, startAngle))
                    {
                        action(xx1 + centerX, yy1 + centerY);
                    }
                });
                if (compare1(currentAngle, endAngle))
                {
                    break;
                }
                if (p >= 0)
                {
                    p += (4 * (xx - yy) + 10);
                    yy--;
                }
                else
                {
                    p += (4 * xx + 6);
                }
                xx++;
                currentAngle = addAction(currentAngle, angleStep);
            }
            if (compare1(currentAngle, endAngle))
            {
                return;
            }
            //第一象限45-0
            int index;
            int index1;
            for (int i = 0; i < size; i++)
            {
                index = size - 1 - i;
                index1 = i + size;
                yy = x[index];
                xx = y[index];
                symboolAction(xx, yy, (xx1, yy1) =>
                {
                    if (compare(currentAngle, startAngle))
                    {
                        action(xx1 + centerX, yy1 + centerY);
                    }
                });
                if (compare1(currentAngle, endAngle))
                {
                    break;
                }
                x[index1] = xx;
                y[index1] = yy;
                currentAngle = addAction(currentAngle, angleStep);
            }
            if (compare1(currentAngle, endAngle))
            {
                return;
            }

            symbolIndex = symbolDirection(symbolIndex);
            symboolAction = symbolActions[symbolIndex];
            //第二象限0 - -90
            for (int i = x.Length - 1; i > -1; i--)
            {
                symboolAction(x[i], y[i], (xx1, yy1) =>
                {
                    if (compare(currentAngle, startAngle))
                    {
                        action(xx1 + centerX, yy1 + centerY);
                    }
                });
                if (compare1(currentAngle, endAngle))
                {
                    return;
                }
                currentAngle = addAction(currentAngle, angleStep);
            }


            symbolIndex = symbolDirection(symbolIndex);
            symboolAction = symbolActions[symbolIndex];
            //第三象限-90 - -180
            for (int i = 0; i < x.Length; i++)
            {
                symboolAction(x[i], y[i], (xx1, yy1) =>
                {
                    if (compare(currentAngle, startAngle))
                    {
                        action(xx1 + centerX, yy1 + centerY);
                    }
                });
                if (compare1(currentAngle, endAngle))
                {
                    return;
                }
                currentAngle = addAction(currentAngle, angleStep);
            }

            symbolIndex = symbolDirection(symbolIndex);
            symboolAction = symbolActions[symbolIndex];
            //第四象限-180 - -270
            for (int i = x.Length - 1; i > -1; i--)
            {
                symboolAction(x[i], y[i], (xx1, yy1) =>
                {
                    if (compare(currentAngle, startAngle))
                    {
                        action(xx1 + centerX, yy1 + centerY);
                    }
                });
                if (compare1(currentAngle, endAngle))
                {
                    return;
                }
                currentAngle = addAction(currentAngle, angleStep);
            }
            if (compare1(endAngle, addAction(__90, _360)))
            {
                symbolIndex = direction ? 0 : 3;
                for (int iiii = 0; iiii < 2; iiii++)
                {
                    symboolAction = symbolActions[symbolIndex];
                    //第一象限 90 - 0
                    for (int i = 0; i < x.Length; i++)
                    {
                        symboolAction(x[i], y[i], (xx1, yy1) =>
                        {
                            if (compare(currentAngle, startAngle))
                            {
                                action(xx1 + centerX, yy1 + centerY);
                            }
                        });
                        if (compare1(currentAngle, endAngle))
                        {
                            return;
                        }
                        currentAngle = addAction(currentAngle, angleStep);
                    }
                    symbolIndex = symbolDirection(symbolIndex);

                    symboolAction = symbolActions[symbolIndex];
                    //第四象限-180 - -270
                    for (int i = x.Length - 1; i > -1; i--)
                    {
                        symboolAction(x[i], y[i], (xx1, yy1) =>
                        {
                            if (compare(currentAngle, startAngle))
                            {
                                action(xx1 + centerX, yy1 + centerY);
                            }
                        });
                        if (compare1(currentAngle, endAngle))
                        {
                            return;
                        }
                        currentAngle = addAction(currentAngle, angleStep);
                    }
                    symbolIndex = symbolDirection(symbolIndex);
                }
            }
        }

        public static void CreateCircle2(int centerX, int centerY, int r1, int r2, int r3, int r4, bool direction,
           Action<int, int> action)
        {

            if (direction)
            {
                CreateCircle3(r1, r4, (x, y, breakAction) =>
                {
                    action(x + centerX, y + centerY);

                });

                CreateCircle3(r4, r3, (x, y, breakAction) =>
                {
                    action(-y + centerX, x + centerY);

                });

                CreateCircle3(r3, r2, (x, y, breakAction) =>
                {
                    action(-x + centerX, -y + centerY);

                });

                CreateCircle3(r2, r1, (x, y, breakAction) =>
                {
                    action(y + centerX, -x + centerY);

                });
            }
            else
            {
                CreateCircle3(r3, r4, (x, y, breakAction) =>
                {
                    action(-x + centerX, y + centerY);

                });

                CreateCircle3(r2, r3, (x, y, breakAction) =>
                {
                    action(-y + centerX, -x + centerY);

                });

                CreateCircle3(r1, r2, (x, y, breakAction) =>
                {
                    action(x + centerX, -y + centerY);

                });

                CreateCircle3(r4, r1, (x, y, breakAction) =>
                {
                    action(y + centerX, x + centerY);

                });
            }
        }

        public static void CreateCircle3(int r1, int r2,
          Action<int, int, Action> action)
        {
            int a = r1;
            int b = r2;

            int a2 = a * a;
            int b2 = b * b;

            int _2a2 = a2 + a2;
            int _2b2 = b2 + b2;

            int _4a2 = _2a2 + _2a2;
            int _4b2 = _2b2 + _2b2;

            int _6b2 = _2b2 + _4b2;

            int _6b2_4a2 = _6b2 + _4a2;

            int p = _2b2 - _2a2 * b + a2;
            int x = 0;
            int y = r2;

            int exchange = (int)(a2 / Math.Sqrt(a2 + b2));

            for (; x <= exchange; x++)
            {
                action(x, y, () =>
                {
                    x = r1;
                });
                if (p >= 0)
                {
                    p += (_4b2 * x - _4a2 * y + _6b2_4a2);
                    y--;
                }
                else
                {
                    p += (_4b2 * x + _6b2);
                }
            }
            p = b2 * (x * x + x) + a2 * (y * y - y) - a2 * b2;

            int _2b2__a2 = _2b2 - a2;
            while (y >= 0)
            {
                action(x, y, () =>
                {
                    x = r1;
                });
                y--;
                if (p < 0)
                {
                    x++;
                    p = p - _2a2 * y + _2b2 * x + _2b2__a2;
                }
                else
                {
                    p = p - _2a2 * y - a2;
                }
            }
        }

    }
}
