using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions 
{
    public class Cigar : IFunction
    {
        int _size;
        string _name;

        public Cigar(int size)
        {
            _size = size;
            _name = "Cigar";
        }

        public double Calculate(double[] variables)
        {
            double ans = 0;
            double middle = 0;
            ans = Math.Pow(variables[0], 2);
            for (int i = 1; i < _size; i++)
            {
                middle += Math.Pow(variables[i], 2);
            }
            ans += Math.Pow(10, 4) * middle;
            return ans;
        }

        public OptimizationType Type
        {
            get 
            {
                return OptimizationType.Minimization;
            }
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
