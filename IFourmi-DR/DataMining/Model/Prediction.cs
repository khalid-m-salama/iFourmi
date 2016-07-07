using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Model
{
    public struct Prediction
    {
        public int Label;
        public double[] Probabilities;

  
        public Prediction(int label, double [] probabilities)
        {
            this.Label = label;
            this.Probabilities = probabilities;
        }
    }
}
