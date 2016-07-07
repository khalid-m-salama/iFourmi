
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class Ellipsoid: IFunction
    {
        int _size;
        string _name;

        public Ellipsoid(int size)
        {
            _size = size;
            _name = "Ellipsoid_" + _size;
        }

        public double Calculate(double[] variables)
        {
            double ans = 0;
            double middle = 0;
            for (int i = 1; i < _size +1; i++)
            {
                middle = Math.Pow(100, ((i - 1) / (_size - 1)));
                ans += Math.Pow((variables[i - 1] * middle), 2);
            }
            return ans;
        }


        public double MaximumVariableValue
        {
            get { return 7; }
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
                return 0;
            }
        }
    }
}
