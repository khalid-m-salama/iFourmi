using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.ClassificationMeasures;


namespace iFourmi.DataMining.Model.Ensembles
{
    public class MajorityVoteStrategy : IEnsembleClassificationStrategy
    {
        public Prediction AggregatePredictions(List<KeyValuePair<ClassifierInfo, Prediction>> ensemblePredictions)
        {
            int classCount=ensemblePredictions[0].Value.Probabilities.Length;
            double[] classProbabilities = new double[classCount];

            double sum = 0;
            int max=0;

            foreach (KeyValuePair<ClassifierInfo, Prediction> pair in ensemblePredictions)
            {
                for (int i = 0; i < classCount; i++)
                {
                    classProbabilities[i] += pair.Value.Probabilities[i];
                    if(classProbabilities[i]>classProbabilities[max])
                        max=i;
                    sum += pair.Value.Probabilities[i];
                }
            }

            for (int i = 0; i < classCount; i++)
            {
                classProbabilities[i] /= sum;
            }

            Prediction finalPrediction=new Prediction(max,classProbabilities);
            return finalPrediction;
        }

        public override string ToString()
        {
            return "Majority Vote";
        }
    }
}
