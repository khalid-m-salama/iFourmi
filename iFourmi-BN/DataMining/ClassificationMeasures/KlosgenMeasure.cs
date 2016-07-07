using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class KlosgenMeasure:IClassificationQualityMeasure
    {
        public double CalculateMeasure(ConfusionMatrix[] list)
        {
            double measure = 0.0;
            foreach (ConfusionMatrix matrix in list)
            {
                double v1=Math.Pow((matrix.TP + matrix.FP)/(double)matrix.Total,0.5);
                double v2 = (matrix.TP / (double)(matrix.TP + matrix.FP)) - ((matrix.TP + matrix.FP) / (double)matrix.Total);
                double value = v1 * v2;
                   if (!double.IsNaN(value))
                    measure += value;
                
            }
            measure /= (double)list.Length;
            return measure;
            
        }

        public override string ToString()
        {
            return "Klosgen";
        }
    }
}
