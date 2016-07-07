using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class Rosenbrock: IFunction
    {
        private int _size;
        private string _name;

        public Rosenbrock(int size)
        {
            _size = size;
            _name = "Rosenbrock_"+_size;
        }

        public int Size
        {
            get { return this._size; }
        }

        public double BestFittness
        {
            get
            {
                return 0;
            }
        }

        public double Calculate(double[] variables)
        {
            double ans = 0;
            double middle = 0;
            double middle1 = 0;
            for (int i = 0; i < _size - 1; i++)
            {
                middle = Math.Pow(variables[i], 2) - variables[i + 1];
                middle = Math.Pow(middle, 2);
                middle *= 100;
                middle1 = Math.Pow(variables[i], 2) - 1;
                middle1 = Math.Pow(middle1, 2);
                ans += middle + middle1;
            }
        
            return ans;
        }

        public double MaximumVariableValue
        {
            get
            {
                return 5;
            }
        }

        public double MinimumVariableValue
        {
            get
            {
                return -5;
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
    }
}
