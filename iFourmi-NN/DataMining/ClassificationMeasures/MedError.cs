using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class MedAError : IClassificationMeasure
    {
        public double CalculateMeasure(ConfusionMatrix[] list)
        {
            throw new Exception("Unsupported Operation");

        }

        public bool IsError
        {
            get { return true; }
        }

        public double CalculateMeasure(DataMining.Model.IClassifier classifier, Data.Dataset dataset)
        {
            Example example = null;
            double[] errors = new double[dataset.Size];

            for (int i = 0; i < dataset.Size; i++)
            {
                example = dataset[i];
                
                int actual = example.Label;

                Prediction prediction = classifier.Classify(example);
                int predicted = prediction.Label;
                double probability = prediction.Probabilities[prediction.Label];

                errors[i]= Math.Abs(1 - prediction.Probabilities[actual]);
                
            }

            Array.Sort(errors);

            double MedAE = errors[errors.Length / 2];

            return MedAE;
        }

        public override string ToString()
        {
            return "MedAE";
        }
    }
}
