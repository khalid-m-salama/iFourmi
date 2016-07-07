using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.Tester
{
    public class Result
    {
        public string Algorithm
        {
            get;
            set;
        }

        public string Dataset
        {
            get;
            set;
        }

        public int Fold
        {
            get;
            set;
        }

        public double Quality
        {
            get;
            set;
        }

        public Double Edges
        {
            get;
            set;
        }

        public int Seconds
        {
            get;
            set;
        }

        public int Clusters
        {
            get;
            set;

        }

        public override string ToString()
        {
            //return Algorithm + "  ,  " + Dataset + "  ,  " + Math.Round( Quality*100,2) + "  ,  " + Math.Round(Edges, 2).ToString()+"  ,  " + Seconds.ToString();
            //return Algorithm + "," + Clusters.ToString() + "," + Dataset + "," + Math.Round( Quality*100,2) + "," + Seconds.ToString();
            return Algorithm + "," + Math.Round(Quality * 100, 2) + "," + Math.Round(Edges, 2).ToString();
        }
    }
}
