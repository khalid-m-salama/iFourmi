using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.DataMining.ProximityMeasures;

namespace iFourmi.DataMining.Model.IBLearning
{
    public class GaussianKernelEstimator:AbstractLazyClassifier
    {
        public double KernelParameter
        {
            get;
            set;
        }

        public GaussianKernelEstimator(double kernelParameter, IDistanceMeasure distanceMeasure, Dataset database, double[][] weights)
            : base(distanceMeasure, database, weights)
        {
            this.KernelParameter = kernelParameter;
            
        }

        public GaussianKernelEstimator(double kernelParameter, IDistanceMeasure distanceMeasure, Dataset database, double[] weights)
            :base(distanceMeasure,database,weights)
        {
            this.KernelParameter = kernelParameter;
           
        }

        public GaussianKernelEstimator(double kernelParameter, IDistanceMeasure distanceMeasure, Dataset database)
            : base(distanceMeasure, database)
        {
            this.KernelParameter = kernelParameter;
           
       
        }

        public override Prediction Classify(Example example)
        {
            double[] classValues = new double[this._database.Metadata.Target.Values.Length];
            int max = 0;

            for (int exampleIndex = 0; exampleIndex < this._database.Size; exampleIndex++)
            {
                if (this._database[exampleIndex] == example)
                    continue;

                classValues[this._database[exampleIndex].Label] += this.GaussianKernel(example, this._database[exampleIndex]);
                if (classValues[this._database[exampleIndex].Label] > classValues[max])
                    max = this._database[exampleIndex].Label;
            }

            double sum = classValues.Sum();
            for (int i = 0; i < classValues.Length; i++)
                classValues[i] /= sum;


            Prediction prediction = new Prediction(max, classValues);
            return prediction;      
        }

        private double GaussianKernel(Example example1, Example example2)
        {
            if (KernelParameter == 0)
                KernelParameter = 0.00001;
            
            double distance = this._distanceMeasure.CalculateDistance(example1, example2, this._classBasedWeights);
            double value = (1.0 / (Math.Sqrt(2 * Math.PI))) * Math.Exp(- Math.Pow(distance, 2) / KernelParameter);
            return value;
        }
    }
}
