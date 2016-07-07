using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class ProbablisticMicroAccuracyMeasure:IClassificationQualityMeasure
    {
        public double CalculateMeasure(ConfusionMatrix[] list)
        {
            double measure = 0.0;
            foreach (ConfusionMatrix matrix in list)
                measure += (double)(matrix.TP_Prob + matrix.TN_Prob) / (double)matrix.TotalProbability;
            measure /= (double)list.Length;
            return measure;
            
        }

        public override string ToString()
        {
            return "probablistic micro-accuracy";
        }
    }
}
