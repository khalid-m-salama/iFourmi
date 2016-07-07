
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class Easom : IFunction
    {
        int _size;
        string _name;

        public Easom(int size)
        {
            _size = size;
            _name = "Easom_" + _size;
        }
   
        public double Calculate(double[] variables)
        {
            double ans = 0;
            double first = 0;

            first = -Math.Pow((variables[0] - Math.PI), 2) + Math.Pow((variables[1] - Math.PI), 2);

            ans = -(Math.Cos(variables[0]) * Math.Cos(variables[1]) * (Math.Exp(first)));
           
            return ans;
        }

        public double MaximumVariableValue
        {
            get { return 100; }
        }

        public double MinimumVariableValue
        {
            get
            {
                return -100;
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
                return -1;
            }
        }
    }
    
}
