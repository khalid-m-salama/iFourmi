using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.Utilities
{
    public static class RandomUtility
    {


        private static Random rand; 

        
        public static void Initialize(int seed)
        {
            rand=new Random(seed);
        }

        public static void Initialize()
        {
            rand = new Random((int)DateTime.Now.Ticks);
        }


        public static double RandomDouble
        {
            get { return rand.NextDouble(); }
        }

        public static int GetNextInt(int min, int max)
        {
            return rand.Next(min, max);
        }

        public static double GetNextDouble(double min, double max)
        {
            double range = max - min;
            return min + (range * rand.NextDouble());
        }

        public static double GetNextDoubleFromGaussianFunction(double mean, double stdDev)
        {

            double u1 = RandomDouble;
            double u2 = RandomDouble;
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }
    }
}
