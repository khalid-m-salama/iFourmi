using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class QLFunction : IClassificationMeasure
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
            double QLF = 0;

            Example example = null;
            for (int i = 0; i < dataset.Size; i++)
            {
                example = dataset[i];
                
                int actual = example.Label;

                Prediction prediction = classifier.Classify(example);
                int predicted = prediction.Label;
                double probability = prediction.Probabilities[prediction.Label];

                double value = 0;
               
                for(int index=0; index<dataset.Metadata.Target.Length;index++)
                    if(index==actual)
                        value-=2*probability;
                    else
                        value+=Math.Pow( prediction.Probabilities[index],2);

                QLF += (2 + value)/3;


            }

            QLF = QLF / dataset.Size;

            return QLF;
        }

        public override string ToString()
        {
            return "QLF";
        }
    }
}
