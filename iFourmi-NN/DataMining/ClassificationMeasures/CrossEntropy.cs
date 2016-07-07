using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class CrossEntropy : IClassificationMeasure
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
            double QLE = 0;

            Example example = null;
            for (int i = 0; i < dataset.Size; i++)
            {
                example = dataset[i];
                
                int actual = example.Label;

                Prediction prediction = classifier.Classify(example);
                int predicted = prediction.Label;
                double probability = prediction.Probabilities[prediction.Label];

                QLE += Math.Log(1 + Math.Exp(-probability));


            }

            QLE = QLE / dataset.Size;

            return QLE;
        }

        public override string ToString()
        {
            return "Ent";
        }
    }
}
