using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class Griewangk: IFunction
    {
        private int _size;
        private string _name;

        public Griewangk(int size)
        {
            _size = size;
            _name = "Griewangk_"+_size;
        }

        public int Size
        {
            get { return this._size; }
        }


        public double Calculate(double[] variables)
        {

            double ans = 0;
            double first = 0;
            double second = 0;
            double middle = 0;

            for (int i = 0; i < _size; i++)
            {
                first += (Math.Pow(variables[i], 2) / 4000);
            }

            for (int i = 1; i < _size +1; i++)
            {
                middle = (variables[i -1] / Math.Sqrt(i));
                second *= Math.Cos(middle);
            }
            
            ans = first - second + 1;

            ans = ans + (1.0 / 10.0);

            ans = Math.Pow(ans, -1);

            return ans;
        }

        public double MaximumVariableValue
        {
            get { return 5.12;}
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

        public double BestFittness
        {
            get
            {
                return 0;
            }
        }
    }
}
