using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class Plane : IFunction
    {
        int _size;
        string _name;

        public Plane(int size)
        {
            _size = size;
            _name = "Plane_"+_size;
        }

        public double Calculate(double[] variables)
        {
            double ans = variables[0];
            return ans;
        }

        public double MaximumVariableValue
        {
            get
            {
                return 1.5;
            }
        }

        public double MinimumVariableValue
        {
            get
            {
                return 0.5;
            }
        }

        public OptimizationType Type
        {
            get
            {
                return OptimizationType.Maximization;
            }
        }

        public string Description
        {
            get
            {
                return this._name;
            }
        }

        public int Size
        {
            get
            {
                return this._size;
            }
        }

        public double BestFittness
        {
            get
            {
                return Math.Pow(10,10);
            }
        }
    }
}
