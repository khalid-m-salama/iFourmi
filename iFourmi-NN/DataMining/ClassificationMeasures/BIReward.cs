using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class BIReward : IClassificationMeasure
    {
        public double CalculateMeasure(ConfusionMatrix[] list)
        {
            throw new Exception("Unsupported Operation");            
        }

        public bool IsError
        {
            get { return false; }
        }

        public double CalculateMeasure(DataMining.Model.IClassifier classifier, Data.Dataset dataset)
        {
            double BIReward = 0;

            double[] classProbabilities = new double[dataset.Metadata.Target.Length];

            for (int i = 0; i < classProbabilities.Length; i++)
                classProbabilities[i] = (double)dataset.Filter(i).Count/(double)dataset.Size;

            Example example = null;
            for (int i = 0; i < dataset.Size; i++)
            {
                example = dataset[i];
                
                int actual = example.Label;

                Prediction prediction = classifier.Classify(example);
                int predicted = prediction.Label;
                double probability = prediction.Probabilities[prediction.Label];

                if (predicted == actual)
                    BIReward += Math.Log(probability / classProbabilities[actual]);
                else
                    BIReward+=Math.Log((1-probability)/(1-classProbabilities[actual]));
                


            }

            BIReward = (BIReward / dataset.Size)+0.5;

            return BIReward;
        }

        public override string ToString()
        {
            return "BIR";
        }
    }
}
