using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class FmeasureMeasure: IClassificationMeasure
    {
        public double Beta
        {
            get;
            set;
        }

        public FmeasureMeasure(double beta)
        {
            this.Beta = beta;
        }

        public double CalculateMeasure(ConfusionMatrix[] list)
        {
            double measure = 0.0;
            foreach (ConfusionMatrix matrix in list)
            {
                double precision = matrix.TP / (double)(matrix.TP + matrix.FP);
                double recall = matrix.TP / (double)(matrix.TP + matrix.FN);
                double value= (1 + Beta) * (((precision) * recall) / ((Beta * precision) + recall));
                if (!double.IsNaN(value))
                    measure += value;
            }

           measure /= (double)list.Length;
           return measure;
        }

        public double CalculateMeasure(DataMining.Model.IClassifier classifier, Data.Dataset dataset)
        {
            return this.CalculateMeasure(ConfusionMatrix.ComputeConfusionMatrixes(classifier, dataset));
        }

        public override string ToString()
        {
            return "fmeasure";
        }
    }
}
