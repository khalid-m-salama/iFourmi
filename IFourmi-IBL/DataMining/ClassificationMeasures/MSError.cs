using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class MSError : IClassificationMeasure
    {
        public double CalculateMeasure(ConfusionMatrix[] list)
        {
            throw new Exception("Unsupported Operation");

        }


        public double CalculateMeasure(DataMining.Model.IClassifier classifier, Data.Dataset dataset)
        {
            double MSE = 0;

            Example example = null;
            for (int i = 0; i < dataset.Size; i++)
            {
                example = dataset[i];
                
                int actual = example.Label;

                Prediction prediction = classifier.Classify(example);
                int predicted = prediction.Label;
                double probability = prediction.Probabilities[prediction.Label];

                MSE+= Math.Pow(1-prediction.Probabilities[actual],2);


            }

            MSE = Math.Sqrt(MSE) / dataset.Size;

            return MSE;
        }

        public override string ToString()
        {
            return "MSE";
        }
    }
}
