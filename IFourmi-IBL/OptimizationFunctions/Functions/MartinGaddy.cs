using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class MartinGaddy: IFunction
    {
        int _size;
        string _name;

        public MartinGaddy(int size)
        {
            _size = size;
            _name = "Martin and Gaddy_"+_size;
        }

        public double Calculate(double[] variables)
        {
            double first = 0;
            double second = 0;
            double ans=0;


            first = variables[0] - variables[1];
            first = Math.Pow(first, 2);

            second = variables[0] + variables[1] - 10;
            second = Math.Pow(second, 2);

            ans = first + second;

            return ans;
        }

        public double MaximumVariableValue
        {
            get
            {
                return 20;
            }
        }

        public double MinimumVariableValue
        {
            get
            {
                return -20;
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
