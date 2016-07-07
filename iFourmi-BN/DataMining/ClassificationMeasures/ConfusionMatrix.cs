using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public struct ConfusionMatrix
    {
        public static ConfusionMatrix[] GetConfusionMatrixes(Model.IClassifier classifier, Data.Dataset testset)
        {
            ConfusionMatrix[] list = new ConfusionMatrix[testset.Metadata.Target.Values.Length];


            foreach (Data.Example example in testset)
            {
                bool correct = false;
                int actual = example.Label;

                Prediction prediction=classifier.Classify(example);
                int predicted = prediction.Label;
                double probability = prediction.Probability;

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
                            list[classIndex].TN_Prob+=probability;
                            
                        }
                    }
                    else
                    {
                        if (classIndex == actual)
                        {
                            list[classIndex].FN++;
                            list[classIndex].FN_Prob+=probability;
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

        public static ConfusionMatrix[] GetConfusionMatrixes(Model.Hierarchical.IHierarchicalClassifier classifier, Data.Dataset testset)
        {
            ConfusionMatrix[] list = new ConfusionMatrix[testset.Metadata.Target.Values.Length];


            foreach (Data.Example example in testset)
            {
                int[] predicted = classifier.Classify(example);
                int[] actual = example.HierarchicalLabel;


                for (int classIndex = 0; classIndex < list.Length; classIndex++)
                {

                    if (predicted.Contains(classIndex))
                    {
                        if (actual.Contains(classIndex))
                            list[classIndex].TP++;
                        else
                            list[classIndex].FP++;

                    }
                    else
                    {
                        if (actual.Contains(classIndex))
                            list[classIndex].FN++;
                        else
                            list[classIndex].TN++;
                    }

                }


            }

            return list;
        }

        public static ConfusionMatrix[] GetConfusionMatrixes(Model.Hierarchical.IHierarchicalClassifier classifier, List<Data.Dataset> testingSets)
        {
            DataMining.Model.Hierarchical.LocalClassifier localClassifier = classifier as DataMining.Model.Hierarchical.LocalClassifier;

            ConfusionMatrix[] list = new ConfusionMatrix[testingSets[0].Metadata.Target.Values.Length];

            for (int exmapleIndex = 0; exmapleIndex < testingSets[0].Size; exmapleIndex++)
            {
                List<Data.Example> examples = new List<Data.Example>();
                foreach (Data.Dataset testset in testingSets)
                    examples.Add(testset[exmapleIndex]);


                int[] predicted = localClassifier.Classify(examples);
                int[] actual = testingSets[0][exmapleIndex].HierarchicalLabel;


                for (int classIndex = 0; classIndex < list.Length; classIndex++)
                {

                    if (predicted.Contains(classIndex))
                    {
                        if (actual.Contains(classIndex))
                            list[classIndex].TP++;
                        else
                            list[classIndex].FP++;

                    }
                    else
                    {
                        if (actual.Contains(classIndex))
                            list[classIndex].FN++;
                        else
                            list[classIndex].TN++;
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
