using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class AccuracyMeasure:IClassificationMeasure
    {
        public bool IsError
        {
            get { return false; }
        }

        public double CalculateMeasure(ConfusionMatrix[] list)
        {
            double correct = 0;
         
            foreach (ConfusionMatrix matrix in list)            
                correct += matrix.TP;
                        
            return correct/ list[0].Total;
            
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
