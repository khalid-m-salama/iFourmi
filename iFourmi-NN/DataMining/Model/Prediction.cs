using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Model
{
    public struct Prediction
    {
        public int Label;
        public double[] Scores;

        public double [] Probabilities
        {
            get 
            {
                //normalize score
                double[] probabilities = new double[this.Scores.Length];
                double sum = this.Scores.Sum();
                for (int i = 0; i < this.Scores.Length; i++)
                    probabilities[i] = this.Scores[i] / sum;
                return probabilities; 
            }
        }

        public Prediction(int label, double [] probabilities)
        {
            this.Label = label;
            this.Scores = probabilities;
        }
    }
}
