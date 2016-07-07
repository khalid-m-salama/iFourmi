using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class AccuracyMeasure:IClassificationQualityMeasure
    {
        public double CalculateMeasure(ConfusionMatrix[] list)
        {
            double correct = 0;
            double error = 0;

            foreach (ConfusionMatrix matrix in list)
            {
                correct += matrix.TP;
                error += matrix.FP;

            }

            double size = error + correct;
            return correct/ size;
            
        }

        public override string ToString()
        {
            return "accuracy";
        }
    }
}
