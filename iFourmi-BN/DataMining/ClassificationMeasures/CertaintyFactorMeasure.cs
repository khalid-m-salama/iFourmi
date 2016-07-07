using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class CertaintyFactorMeasure:IClassificationQualityMeasure
    {
        public double CalculateMeasure(ConfusionMatrix[] list)
        {
            double measure = 0.0;
            foreach (ConfusionMatrix matrix in list)
            {
                double value = ((matrix.TP / (double)(matrix.TP + matrix.FP)) - ((matrix.TP + matrix.FN) / (double)matrix.Total)) / (1 - ((matrix.TP + matrix.FN) / (double)matrix.Total));
                if (!double.IsNaN(value))
                    measure += value;
            }
            measure /= (double)list.Length;
            return measure;
            
        }

        public override string ToString()
        {
            return "CertaintyFactor";
        }
    }
}
