using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class MicroAccuracyMeasure:IClassificationMeasure
    {
        public double CalculateMeasure(ConfusionMatrix[] list)
        {
            double measure = 0.0;
            foreach (ConfusionMatrix matrix in list)
                measure += (double)(matrix.TP + matrix.TN) / (double)matrix.Total;
            measure /= (double)list.Length;
            return measure;
            
        }

        public double CalculateMeasure(DataMining.Model.IClassifier classifier, Data.Dataset dataset)
        {
            return this.CalculateMeasure(ConfusionMatrix.ComputeConfusionMatrixes(classifier, dataset));
        }

        public override string ToString()
        {
            return "accuracy";
        }
    }
}
