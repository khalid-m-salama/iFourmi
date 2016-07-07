using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.NeuralNetworks.ActivationFunctions
{
    public interface IActivationFunction
    {
        double Activate(double input);

        double Dervative(double input);
    }
}
