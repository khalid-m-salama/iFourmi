using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class B2 : IFunction
    {
        int _size;
        string _name;

        public B2(int size)
        {
            _size = size;
            _name = "B2_" + _size;
        }

        public double Calculate(double[] variables)
        {
            double ans = 0;

            ans = Math.Pow(variables[0], 2) + (2 * Math.Pow(variables[1], 2)) - ((3.0 / 10.0) * Math.Cos(3 * Math.PI * variables[0])) - ((2.0 / 5.0) * Math.Cos(4 * Math.PI * variables[1])) + (7.0 / 10.0);

            
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
                return 100;
            }
        }

        public double MinimumVariableValue
        {
            get 
            {
                return -100;
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
