using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;

namespace iFourmi.DataMining.Statistics
{
    public static class DatasetExtentionMethods
    {
        public static double CalculateClassEntropy(this Dataset dataset)
        {
            double entropy = 0;

            for (int classIndex = 0; classIndex < dataset.Metadata.Target.Values.Length; classIndex++)
            {
                double classProb = (double)dataset.Metadata.Target.ValueCounts[classIndex] / (double)dataset.Size;
                entropy -= classProb * Math.Log(classProb);
            }

            return entropy;
        }

        public static double CalculateEntropy(this Dataset dataset, int attributeIndex)
        {
            double entopy = 0;

            if (dataset.Metadata.Attributes[attributeIndex] is NominalAttribute)
            {
                
                for (int classIndex = 0; classIndex < dataset.Metadata.Target.Values.Length; classIndex++)
                {
                    double currentEntropy = 0;

                    for (int valueIndex = 0; valueIndex < ((NominalAttribute)dataset.Metadata.Attributes[attributeIndex]).Values.Length; valueIndex++)
                    {

                        int freq = dataset.Filter(attributeIndex, valueIndex, classIndex).Count;
                        if (freq != 0)
                        {
                            double prob = (double)freq / (double)dataset.Size;
                            currentEntropy -= prob * Math.Log(prob);
                        }
                    }

                    double classProb = (double)dataset.Metadata.Target.ValueCounts[classIndex] / (double)dataset.Size;
                    entopy += currentEntropy * classProb;
                }

                
            }
            else
            {
                double stdv = dataset.CalculateStdValue(attributeIndex);

                //ln(sigma*Sqrt(2*Pi*e))

                double differentialEntropy = Math.Log(stdv * Math.Sqrt(2 * Math.PI * Math.E), Math.E);

                entopy = differentialEntropy;
            }

           
            return entopy;

        }

        public static double CalculateMeanValue(this Dataset dataset, int attributeIndex)
        {
            if (!(dataset.Metadata.Attributes[attributeIndex] is NumericAttribute))
                throw new Exception("Invalid Attribute Type");

            double mean = 0;

            int count = 0;

            for (int instanceIndex = 0; instanceIndex < dataset.Size; instanceIndex++)
            {
                mean += dataset[instanceIndex][attributeIndex];
                count++;
            }

            mean /= count;

            return mean;
        }

        public static double CalculateMeanValue(this Dataset dataset, int attributeIndex, int label)
        {
            if (!(dataset.Metadata.Attributes[attributeIndex] is NumericAttribute))
                throw new Exception("Invalid Attribute Type");

            double mean = 0;

            int count = 0;

            for (int instanceIndex = 0; instanceIndex < dataset.Size; instanceIndex++)
            {
                if (dataset[instanceIndex].Label == label)
                {
                    mean += dataset[instanceIndex][attributeIndex];
                    count++;
                }
            }

            mean /= count;

            return mean;
        }

        public static double CalculateStdValue(this Dataset dataset, int attributeIndex)
        {
            if (!(dataset.Metadata.Attributes[attributeIndex] is NumericAttribute))
                throw new Exception("Invalid Attribute Type");

            double stdv = 0;

            double mean = CalculateMeanValue(dataset,attributeIndex);

            int count = 0;
            
            for (int instanceIndex = 0; instanceIndex < dataset.Size; instanceIndex++)
            {
                stdv += Math.Pow( mean-dataset[instanceIndex][attributeIndex],2);
                count++;
            }

            stdv = Math.Sqrt(stdv / count);

            return stdv;
        }

        public static double CalculateStdValue(this Dataset dataset, int attributeIndex, int label)
        {
            if (!(dataset.Metadata.Attributes[attributeIndex] is NumericAttribute))
                throw new Exception("Invalid Attribute Type");

            double stdv = 0;

            double mean = CalculateMeanValue(dataset, attributeIndex,label);

            int count = 0;

            for (int instanceIndex = 0; instanceIndex < dataset.Size; instanceIndex++)
            {
                if (dataset[instanceIndex].Label == label)
                {
                    stdv += Math.Pow(mean - dataset[instanceIndex][attributeIndex], 2);
                    count++;
                }
            }

            stdv = Math.Sqrt(stdv / count);

            return stdv;
        }
    }
}
