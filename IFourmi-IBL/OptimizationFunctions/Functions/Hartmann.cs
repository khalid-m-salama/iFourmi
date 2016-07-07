using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public class Hartmann : IFunction
    {
        int _size;
        double[][] _a;
        double[] _c;
        double[][] _p;
        string _name;

        public Hartmann(int size)
        {
            _size = size;
            if ((_size == 4) || (_size == 6))
                IntializeArray();
            else
                throw new Exception("Invalid Input");
           
           _name = "Hartmann_"+size;
        }

        public double Calculate(double[] variables)
        {
            double ans = 0;
            double middle = 0;
            for (int i = 0; i < 4; i++)
            {
                middle = 0;
                if (_size == 6)
                {
                    for (int j = 0; j < _size; j++)
                    {
                        middle += _a[i][j] * Math.Pow((variables[j] - _p[i][j]), 2);
                    }
                }
                else
                {
                    for (int j = 0; j < 3; j++)
                    {
                        middle += _a[i][j] * Math.Pow((variables[j] - _p[i][j]), 2);
                    }
                }
                ans += _c[i] * Math.Exp(-middle);
            }
            ans = -ans;
           
            return ans;
        }

        public void IntializeArray()
        {
            if (_size == 4)
            {
                _a = new double[_size][];
                _c = new double[_size];
                _p = new double[_size][];
                for (int i = 0; i < _size; i++)
                {
                    _a[i] = new double[3];
                    _p[i] = new double[3];
                }
                _a[0][0] = 3.0;
                _a[0][1] = 10.0;
                _a[0][2] = 30.0;

                _a[1][0] = 0.1;
                _a[1][1] = 10.0;
                _a[1][2] = 35.0;

                _a[2][0] = 3.0;
                _a[2][1] = 10.0;
                _a[2][2] = 30.0;

                _a[3][0] = 0.1;
                _a[3][1] = 10.0;
                _a[3][2] = 35.0;

                _c[0] = 1.0;
                _c[1] = 1.2;
                _c[2] = 3.0;
                _c[3] = 3.2;

                _p[0][0] = 0.3689;
                _p[0][1] = 0.1170;
                _p[0][2] = 0.2673;

                _p[1][0] = 0.4699;
                _p[1][1] = 0.4387;
                _p[1][2] = 0.7470;

                _p[2][0] = 0.1091;
                _p[2][1] = 0.8732;
                _p[2][2] = 0.5547;

                _p[3][0] = 0.0381;
                _p[3][1] = 0.5743;
                _p[3][2] = 0.8828;

            }
            else
            {
                _a = new double[_size][];
                _c = new double[_size];
                _p = new double[_size][];
                for (int i = 0; i < _size; i++)
                {
                    _a[i] = new double[6];
                    _p[i] = new double[6];
                }

                _a[0][0] = 10.0;
                _a[0][1] = 3.0;
                _a[0][2] = 17.0;
                _a[0][3] = 3.50;
                _a[0][4] = 1.50;
                _a[0][5] = 8.00;

                _a[1][0] = 0.05;
                _a[1][1] = 10.0;
                _a[1][2] = 17.0;
                _a[1][3] = 0.10;
                _a[1][4] = 8.00;
                _a[1][5] = 14.00;

                _a[2][0] = 3.00;
                _a[2][1] = 3.50;
                _a[2][2] = 1.70;
                _a[2][3] = 10.00;
                _a[2][4] = 17.00;
                _a[2][5] = 8.00;

                _a[3][0] = 17.00;
                _a[3][1] = 8.00;
                _a[3][2] = 0.05;
                _a[3][3] = 10.00;
                _a[3][4] = 0.10;
                _a[3][5] = 14.00;

                _c[0] = 1.0;
                _c[1] = 1.2;
                _c[2] = 3.0;
                _c[3] = 3.2;

                _p[0][0] = 0.1312;
                _p[0][1] = 0.1696;
                _p[0][2] = 0.5569;
                _p[0][3] = 0.0124;
                _p[0][4] = 0.8283;
                _p[0][5] = 0.5886;

                _p[1][0] = 0.2329;
                _p[1][1] = 0.4135;
                _p[1][2] = 0.8307;
                _p[1][3] = 0.3736;
                _p[1][4] = 0.1004;
                _p[1][5] = 0.9991;

                _p[2][0] = 0.2348;
                _p[2][1] = 0.1451;
                _p[2][2] = 0.3522;
                _p[2][3] = 0.2883;
                _p[2][4] = 0.3047;
                _p[2][5] = 0.6650;

                _p[3][0] = 0.4047;
                _p[3][1] = 0.8828;
                _p[3][2] = 0.8732;
                _p[3][3] = 0.5743;
                _p[3][4] = 0.1091;
                _p[3][5] = 0.0381;
            }

        }

        public double MaximumVariableValue
        {
            get 
            { 
                return 1; 
            }
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
                if(this._size == 6)
                    return -3.32236801141551;
                else
                    return -3.86278214782076;
            }
        }
    }
}
