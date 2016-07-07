using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class ReducedErrorMeasure:IClassificationMeasure
    {

        public static double CONFIDENCE_LEVEL = 0.25;
        private static  double [] values = { 0, 0.001, 0.005, 0.01, 0.05, 0.10, 0.20, 0.40, 1.00 };
        private static  double [] deviation = { 4.0, 3.09, 2.58, 2.33, 1.65, 1.28, 0.84, 0.25, 0.00 };

        public double CalculateMeasure(ConfusionMatrix[] list)
        {
           
            double correct=0;
            double error=0;

            foreach (ConfusionMatrix matrix in list)
            {
                correct += matrix.TP;
                error += matrix.FP;

            }

            double size=error+correct;


            error = error + EstimateError(error, size);

            return (size - error) / size;
        }

        public double CalculateMeasure(DataMining.Model.IClassifier classifier, Data.Dataset dataset)
        {
            return this.CalculateMeasure(ConfusionMatrix.ComputeConfusionMatrixes(classifier, dataset));
        }

        public double EstimateError(double error, double total)
        {
            double confidence = CONFIDENCE_LEVEL;
      

            // computes the coefficient value based on C4.5

            int i = 0;

            while (confidence > values[i])
            {
                i++;
            }

            double coefficient =
                deviation[i - 1] + (deviation[i] - deviation[i - 1])
                    * (confidence - values[i - 1]) / (values[i] - values[i - 1]);
            coefficient = coefficient * coefficient;

            if (error < 1E-6)
            {
                return total * (1 - Math.Exp(Math.Log(confidence) / total));
            }
            else if (error < 0.9999)
            {
                double v = total * (1 - Math.Exp(Math.Log(confidence) / total));

                return v + error * (EstimateError(1.0, total) - v);
            }
            else if (error + 0.5 >= total)
            {
                return 0.67 * (total - error);
            }
            else
            {
                double pr =
                    (error + 0.5 + coefficient / 2 + Math
                        .Sqrt(coefficient
                            * ((error + 0.5) * (1 - (error + 0.5) / total) + coefficient / 4)))
                        / (total + coefficient);
                return (total * pr - error);
            }
 
        }

        public override string ToString()
        {
            return "E-Accuracy";
        }
    }
}
