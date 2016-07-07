using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class Sphere: IFunction
    {
        private  int _size;
        private string _name;

        public Sphere(int size)
        {
            _size = size;
            _name = "Sphere_"+_size;
        }

        public int Size
        {
            get { return this._size; }
        }


        public double Calculate(double[] variables)
        {
            double ans = 0;
            for (int i = 0; i < _size; i++)
            {
                ans += Math.Pow(variables[i],2);
            }
            return ans;
        }


        public bool AccuracyReached(double fitness)
        {
            double epioson = Math.Pow(10, -10);
            double fitnessDiff = Math.Abs(fitness - 0);

            if (fitnessDiff < epioson)
                return true;
            else
                return false;
        }

        public double MaximumVariableValue
        {
            get
            {
                return 7;
            }
        }

        public double MinimumVariableValue
        {
            get
            {
                return -3;
            }
        }

        public OptimizationType Type
        {
            get
            {
                return OptimizationType.Minimization;
            }
        }

        public string Description
        {
            get
            {
                return this._name;
            }
        }

        public double BestFittness
        {
            get
            {
                return 0;
            }
        }

    }
}
