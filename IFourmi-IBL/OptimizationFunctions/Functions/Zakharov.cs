using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class Zakharov: IFunction
    {
        int _size;
        string _name;

        public Zakharov(int size)
        {
            _size = size;
            _name = "Zakharov_" + _size;
        }
      
        public double Calculate(double[] variables)
        {
            double ans = 0;
            double first = 0;
            double second = 0;
            double third = 0;

            for (int i = 0; i < _size; i++)
            {
                first += Math.Pow(variables[i], 2);
            }

            for (int i = 1; i < _size + 1; i++)
            {
                second += (i * variables[i - 1]) / 2;
            }
            
            second = Math.Pow(second, 2);

            for (int i = 1; i < _size + 1; i++)
            {
                third += (i * variables[i - 1]) / 2;
            }
            third = Math.Pow(third, 4);

            ans = first + second + third;

            return ans;
        }

        public double MaximumVariableValue
        {
            get { return 10; }
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
