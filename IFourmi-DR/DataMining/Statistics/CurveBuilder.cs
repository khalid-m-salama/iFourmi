using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Statistics
{
    public abstract class CurveBuilder
    {
        protected static int GetClassCount(double[][] values, int classIndex)
        {
            int counter = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i][1] == classIndex)
                {
                    counter++;
                }
            }

            return counter;
        }


        protected static void Sort(double[][] values)
        {
            Array.Sort(values, new Comparer());

        }

        public abstract Curve CreateCurve(double[][] values);
    }

    public class Comparer : IComparer<double[]>
    {

        public int Compare(double[] x, double[] y)
        {
            return y[0].CompareTo(x[0]);
        }
    }


}
