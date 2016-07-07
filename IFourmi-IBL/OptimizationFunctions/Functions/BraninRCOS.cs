
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class BraninRCOS : IFunction
    {
        int _size;
        string _name;
        public BraninRCOS(int size)
        {
            _size = size;
            _name = "Branin_RCOS_"+_size;
        }

        public double Calculate(double[] variables)
        {
            double ans = 0;
            double first = 0;
            double second = 0;

            first = variables[1] - (5.0 / (4.0 * Math.Pow(Math.PI, 2.0))) * Math.Pow(variables[0],2) + ((5.0 / Math.PI) * variables[0]) - 6;
            first = Math.Pow(first, 2);

            second = 10*(1 - (1.0/(8.0*Math.PI))*Math.Cos(variables[1]));

            ans = first + second + 10;

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
                return 15;
            }
        }

        public double MinimumVariableValue
        {
            get
            {
                return -5;
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
                return 0.397887;
            }
        }
    }
}
