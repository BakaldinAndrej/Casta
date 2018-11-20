using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Math6
{
    class Program
    {
        const int n = 2;
        const int k = 2;

        static double Q(double x1, double x2, double[] bs, double[] ass, double[,] xjs)
        {
            return -bs[0] / (Math.Pow(ass[0], 2 * n) + Math.Pow(Math.Pow(x2 - xjs[0, 0], 2) + Math.Pow(x2 - xjs[1, 0], 2), n)) - bs[1] / (Math.Pow(ass[1], 2 * n) + Math.Pow(Math.Pow(x2 - xjs[0, 1], 2) + Math.Pow(x2 - xjs[1, 1], 2), n));
        }

        static void Main(string[] args)
        {
            for (int life = 0; life < 20; life++)
            {
                double[,] CX = new double[n, k];
                double[] bs = new double[k];
                double[] ass = new double[k];
                
                CX[0, 0] = 3.0;
                CX[0, 1] = 3.0;
                CX[1, 0] = -5.0;
                CX[1, 1] = -5.0;

                bs[0] = 3.0;
                bs[1] = 4.0;

                ass[0] = 5.0;
                ass[1] = 2.0;

                //число особей
                const int osobi = 1000;
                const int MKS = 50;
                const int pokolen = 10;

                double[] x1_new = new double[osobi];
                double[] x2_new = new double[osobi];

                Random rnd = new Random();
                //генерируем координаты для точек
                for (int i = 0; i < osobi; i++)
                {
                    x1_new[i] = rnd.NextDouble() * 10.0;
                    x2_new[i] = rnd.NextDouble() * 10.0;
                }

                //Цикл по поколениям
                for (int p = 0; p < pokolen; p++)
                {
                    double[] x1 = new double[osobi];
                    double[] x2 = new double[osobi];

                    x1 = x1_new;
                    x2 = x2_new;

                    double Qmax = Q(x1[0], x2[0], bs, ass, CX);
                    double Qmin = Qmax;
                    double deltaQ = 0.0;

                    for (int i = 0; i < osobi; i++)
                    {
                        if (Q(x1[i], x2[i], bs, ass, CX) > Qmax) Qmax = Q(x1[i], x2[i], bs, ass, CX);
                        if (Q(x1[i], x2[i], bs, ass, CX) < Qmin) Qmin = Q(x1[i], x2[i], bs, ass, CX);
                    }

                    deltaQ = Qmax - Qmin;

                    double[] FL = new double[MKS]; //Левая граница
                    double[] FP = new double[MKS]; //Првавая граница
                    int[] MMKS = new int[MKS]; //Число точек в касте
                    int[,] SP = new int[MKS, osobi]; //списки номеров точек с порядковым номером jKS в касте KS

                    double perKS = 1 / (double)MKS;

                    //определяем границы и число точек в кастах
                    for (int i = 0; i < MKS; i++)
                    {
                        FL[i] = i * (perKS);
                        FP[i] = (i + 1) * (perKS);
                        MMKS[i] = 0;
                    }

                    //распределение по кастам
                    for (int i = 0; i < osobi; i++)
                    {
                        double F = (Qmax - Q(x1[i], x2[i], bs, ass, CX)) / deltaQ;

                        for (int j = 0; j < MKS; j++)
                        {
                            if ((F >= FL[j]) && ((F < FP[j]) || ((F == FP[j]) && (j == MKS - 1))))
                            {
                                SP[j, MMKS[j]] = i;
                                MMKS[j]++;
                            }
                        }
                    }

                    //Цикл по числу новых особей
                    for (int t = 0; t < osobi; t++)
                    {
                        int randomKS = rnd.Next(0, MKS); //выбираем случайную касту
                        int rod1 = rnd.Next(0, MMKS[randomKS]); //выбираем в касте родителя

                        int rod2 = 0;
                        double maxDist = 0.0;

                        //выбираем второго родителя
                        for (int i = 0; i < MMKS[randomKS]; i++)
                        {
                            int ind = SP[randomKS, i];

                            double dist = Math.Pow(x1[rod1] - x1[ind], 2) + Math.Pow(x2[rod1] - x2[ind], 2);

                            if (dist > maxDist)
                            {
                                maxDist = dist;
                                rod2 = ind;
                            }
                        }

                        double heta = 0.1; //шаг смещения

                        int size = (int)(1 / heta);

                        int minChildInd = 0;
                        double minChildQ = Q(x1[rod1], x2[rod1], bs, ass, CX);

                        //находим наилучшего потомка
                        for (int i = 0; i < size; i++)
                        {
                            //смещение от 1 родителя
                            double eta = i * heta;

                            //координаты потомка
                            double xx1 = x1[rod1] + eta * (x1[rod2] - x1[rod1]);
                            double xx2 = x2[rod1] + eta * (x2[rod2] - x2[rod1]);

                            if (Q(xx1, xx2, bs, ass, CX) < minChildQ)
                            {
                                minChildQ = Q(xx1, xx2, bs, ass, CX);
                                minChildInd = i;
                            }
                        }

                        //добавляем точку в поколение новых
                        x1_new[t] = x1[rod1] + minChildInd * heta * (x1[rod2] - x1[rod1]);
                        x2_new[t] = x2[rod1] + minChildInd * heta * (x2[rod2] - x2[rod1]);
                    }

                }

                //Выбираем наилучшие результаты и выводим

                double QminFin = Q(x1_new[0], x2_new[0], bs, ass, CX);

                for (int i = 0; i < osobi; i++)
                {
                    if (Q(x1_new[i], x2_new[i], bs, ass, CX) < QminFin) QminFin = Q(x1_new[i], x2_new[i], bs, ass, CX);
                }

                Console.Write("MIN = {0:f5}\n", QminFin);
            }
            Console.ReadKey();
        }
    }
}
