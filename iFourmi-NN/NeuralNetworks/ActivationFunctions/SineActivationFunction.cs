using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.NeuralNetworks.ActivationFunctions
{
    public class SineActivationFunction:IActivationFunction
    {


        public double Activate(double input)
        {
            return Math.Sin(input);
        }

        public double Dervative(double input)
        {
            return Math.Sqrt(1 - input * input);
        }
    }
}
