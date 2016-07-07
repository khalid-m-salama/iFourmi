using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Model
{
    public class Prediction
    {
        private int _label;
        private double _probability;

        public int Label
        {
            get { return this._label; }            
        }

        public double Probability
        {
            get { return this._probability; }
        }

        public Prediction(int _label, double probability)
        {
            this._label = _label;
            this._probability = probability;
        }
    }
}
