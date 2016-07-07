
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class GoldsteinPrice : IFunction
    {
        int _size;
        string _name;

        public GoldsteinPrice(int size)
        {
            _size = size;
            _name = "Goldstein_Price_"+_size;
        }


        public double Calculate(double[] variables)
        {
            double ans = 0;
            double first = 0;
            double second = 0;
            double third = 0;

            first = variables[0] + variables[1] + 1;
            first = Math.Pow(first, 2);

            second = 19 - (14 * variables[0]) + (13 * Math.Pow(variables[0], 2)) - (14 * variables[1]) + (6 * variables[0] * variables[1]) + (3 * Math.Pow(variables[1], 2));

            first = first * second + 1;

            second = (2 * variables[0]) - (3 * variables[1]);
            second = Math.Pow(second, 2);

            third = 18 - (32 * variables[0]) + (12 * Math.Pow(variables[0], 2)) - (48 * variables[1]) + (36 * variables[0] * variables[1]) + (27 * Math.Pow(variables[1], 2));

            second = 30 + (second * third);

            ans = first * second;

            return ans;
        }

        public double MaximumVariableValue
        {
            get { return 2; }
        }

        public double MinimumVariableValue
        {
            get
            {
                return -2;
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
                return 3;
            }
        }
    }
}
