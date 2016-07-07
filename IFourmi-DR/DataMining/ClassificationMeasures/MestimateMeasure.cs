using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class MestimateMeasure: IClassificationMeasure
    {
        public double MValue
        {
            get;
            set;
        }

        public MestimateMeasure(double m)
        {
            this.MValue = m;
        }

        public double CalculateMeasure(ConfusionMatrix[] list)
        {
            double measure = 0.0;
            foreach (ConfusionMatrix matrix in list)
            {
                double value = ((matrix.TP) + (MValue * (matrix.TP / (double)matrix.Total))) / (double)(matrix.TP + matrix.FP + MValue);
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
            return "m-estimate";
        }
    }
}
