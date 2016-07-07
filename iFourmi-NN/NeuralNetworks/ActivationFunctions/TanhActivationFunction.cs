using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.NeuralNetworks.ActivationFunctions
{
    public class TanhActivationFunction:IActivationFunction
    {
        

        public double Activate(double input)
        {
            return Math.Tanh(input);
        }

        public double Dervative(double input)
        {
            return (1 - (input * input));
        }
    }
}
