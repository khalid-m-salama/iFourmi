using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class Shekel: IFunction
    {
        int _size;
        int _k;
        double[][] _a;
        double[] _c;
        string _name;

        public Shekel(int k, int size)
        {
            _size = size;
            _k = k;
            IntializeArray();
            _name = "Shekel_" + _size + "_" + _k;
        }

        private void IntializeArray()
        {
            _a = new double[10][];
            _c = new double[10];
            for (int i = 0; i < 10; i++)
            {
                _a[i] = new double[_size];
            }

            _a[0][0] = 4.0;
            _a[0][1] = 4.0;
            _a[0][2] = 4.0;
            _a[0][3] = 4.0;

            _a[1][0] = 1.0;
            _a[1][1] = 1.0;
            _a[1][2] = 1.0;
            _a[1][3] = 1.0;

            _a[2][0] = 8.0;
            _a[2][1] = 8.0;
            _a[2][2] = 8.0;
            _a[2][3] = 8.0;

            _a[3][0] = 6.0;
            _a[3][1] = 6.0;
            _a[3][2] = 6.0;
            _a[3][3] = 6.0;

            _a[4][0] = 3.0;
            _a[4][1] = 7.0;
            _a[4][2] = 3.0;
            _a[4][3] = 7.0;

            _a[5][0] = 2.0;
            _a[5][1] = 9.0;
            _a[5][2] = 2.0;
            _a[5][3] = 9.0;

            _a[6][0] = 5.0;
            _a[6][1] = 5.0;
            _a[6][2] = 3.0;
            _a[6][3] = 3.0;

            _a[7][0] = 5.0;
            _a[7][1] = 5.0;
            _a[7][2] = 3.0;
            _a[7][3] = 3.0;

            _a[8][0] = 8.0;
            _a[8][1] = 1.0;
            _a[8][2] = 8.0;
            _a[8][3] = 1.0;

            _a[9][0] = 7.0;
            _a[9][1] = 3.6;
            _a[9][2] = 7.0;
            _a[9][3] = 3.6;

            _c[0] = 0.1;
            _c[1] = 0.2;
            _c[2] = 0.2;
            _c[3] = 0.4;
            _c[4] = 0.4;
            _c[5] = 0.6;
            _c[6] = 0.3;
            _c[7] = 0.7;
            _c[8] = 0.5;
            _c[9] = 0.5;

        }

        public double Calculate(double[] variables)
        {
            double ans = 0;
            double middle1 = 0;
            double middle2 = 0;
            for (int i = 0; i < _k; i++)
            {
                middle1 = 0;
                for (int j = 0; j < _size; j++)
                {
                    middle2 = variables[j] - _a[i][j];
                    middle1 += Math.Pow(middle2, 2);
                }
                middle1 += _c[i];
            }
            ans = - 1 / middle1;
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
                return 0;
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
                if(_k == 10)
                    return -10.5364;
                if (_k == 7)
                    return -10.4029;
                if (_k == 5)
                    return -10.1532;
                return 0;


            }
        }
    }
}
