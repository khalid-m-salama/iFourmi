using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions 
{
    public class Tablet : IFunction
    {
        int _size;
        string _name;
        public Tablet(int size)
        {
            _size = size;
            _name = "Tablet_" + _size;
        }

        public double Calculate(double[] variables)
        {
            double ans = 0;
            ans = Math.Pow(10, 4) *  Math.Pow(variables[0], 2);
            for (int i = 1; i < _size; i++)
            {
                ans += Math.Pow(variables[i], 2);
            }
            return ans;
        }

        public int Size
        {
            get
            {
                return this._size;
            }
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

        public double BestFittness
        {
            get
            {
                return 0;
            }
        }

    }
}
