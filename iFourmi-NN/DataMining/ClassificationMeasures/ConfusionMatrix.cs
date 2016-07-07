using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Data;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public struct ConfusionMatrix
    {
        internal static ConfusionMatrix[] ComputeConfusionMatrixes(Model.IClassifier classifier, Data.Dataset testset)
        {
            ConfusionMatrix[] list = new ConfusionMatrix[testset.Metadata.Target.Length];

            Example example = null;
            for(int i=0; i< testset.Size;i++)
            {
                example = testset[i];
                bool correct = false;
                int actual = example.Label;

                Prediction prediction = classifier.Classify(example);
                int predicted = prediction.Label;
                double probability = prediction.Probabilities[prediction.Label];

                if (predicted == actual)
                    correct = true;

                for (int classIndex = 0; classIndex < list.Length; classIndex++)
                {
                    if (correct)
                    {
                        if (classIndex == actual)
                        {
                            list[classIndex].TP++;
                            list[classIndex].TP_Prob += probability;
                        }

                        else
                        {
                            list[classIndex].TN++;
                            list[classIndex].TN_Prob += probability;

                        }
                    }
                    else
                    {
                        if (classIndex == actual)
                        {
                            list[classIndex].FN++;
                            list[classIndex].FN_Prob += probability;
                        }
                        else if (classIndex == predicted)
                        {
                            list[classIndex].FP++;
                            list[classIndex].FP_Prob += probability;
                        }
                        else
                        {
                            list[classIndex].TN++;
                            list[classIndex].TN_Prob += probability;
                        }

                    }
                }

                

            }

            return list;
        }

        public int TP;
        public int TN;
        public int FP;
        public int FN;

        public double TP_Prob;
        public double TN_Prob;
        public double FP_Prob;
        public double FN_Prob;


        public double MSE;

        public int Total
        {
            get
            {
                return TP + TN + FP + FN;
            }
        }

        public double TotalProbability
        {
            get
            {
                return TP_Prob + TN_Prob + FP_Prob + FN_Prob;
            }
        }

    }
}
