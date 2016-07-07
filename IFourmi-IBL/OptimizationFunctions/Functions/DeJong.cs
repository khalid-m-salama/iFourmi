
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class DeJong: IFunction
    {
        int _size;
        string _name;

        public DeJong(int size)
        {
            _size = size;
            _name = "De_Jong_"+_size;
        }

        public double Calculate(double[] variables)
        {
            double ans = 0;


            ans = Math.Pow(variables[0], 2) + Math.Pow(variables[1], 2) + Math.Pow(variables[2], 2);

            return ans;
        }

        public double MaximumVariableValue
        {
            get { return 5.12; }
        }

        public double MinimumVariableValue
        {
            get
            {
                return -5.12;
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
