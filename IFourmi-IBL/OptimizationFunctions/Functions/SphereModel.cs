using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class SphereModel: IFunction
    {
        int _size;
        string _name;
        public SphereModel(int size)
        {
            _size = size;
            _name = "Sphere Model_" + _size;
        }

        public int Size
        {
            get
            {
                return this._size;
            }
        }
        
        public double Calculate(double[] variables)
        {

            double ans = 0;

            for (int i = 0; i < _size; i++)
            {
                ans += Math.Pow(variables[i], 2) ;
            }

            return ans;
        }

        public double MaximumVariableValue
        {
            get { return 7; }
        }

        public double MinimumVariableValue
        {
            get
            {
                return -3;
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
