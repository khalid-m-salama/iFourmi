using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.NeuralNetworks.ActivationFunctions
{
    public class SigmoidActivationFunction:IActivationFunction
    {
       
        public double Activate(double value)
        {
            return 1d / (1 + Math.Exp(-value));
        }

        public double Dervative(double value)
        {
            return value * (1 - value);
        }
    }
}
