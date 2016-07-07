using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACO;
using iFourmi.NeuralNetworks.Model;
using iFourmi.NeuralNetworks.ActivationFunctions;

namespace iFourmi.ACONeuralNets.ProblemSpecifics.QualityEvaluators
{
    public class NNClassificationQualityEvaluator2 : ContinuousACO.ProblemSpecifics.AbstractContinousOptimizationEvaluator
    {
        #region Data Members
        
        private DataMining.Data.Dataset _validationSet;
        private DataMining.Data.Dataset _learningSet;
        private DataMining.ClassificationMeasures.IClassificationMeasure _measure;
        private NeuralNetwork _network;

        #endregion

        #region properties

        public DataMining.Data.Dataset LearningSet
        {
            get { return this._learningSet; }
            set { this._learningSet = value; }
        }

        public DataMining.Data.Dataset ValidationSet
        {
            get { return this._validationSet; }
            set { this._validationSet = value; }
        }

        public NeuralNetwork NeuralNetwork
        {
            get { return this._network; }
            set { this._network = value; }
        }

        public DataMining.ClassificationMeasures.IClassificationMeasure Measure
        {
            get { return this._measure; }
        }

        #endregion

        #region Constructor

        public NNClassificationQualityEvaluator2(double minimumValue,double maximumValue,DataMining.ClassificationMeasures.IClassificationMeasure measure)
            :base(minimumValue,maximumValue)
        {
            this._measure = measure;
         }

        #endregion

        #region Methods

        public override void EvaluateSolutionQuality(Solution<double> solution)
        {
            double quality = 0;

            int counter=0;

            for (int i = 0; i < this._network.NetNeurons.Length; i++)
            {
                if (this._network.NetNeurons[i].Weights != null)
                {
                    for (int j = 0; j < this._network.NetNeurons[i].Weights.Count; j++)
                    {
                        this._network.NetNeurons[i].Weights[j] = solution.Components[counter].Element;
                        counter++;
                    }
                }
            }

            quality = this._measure.CalculateMeasure(this._network, ValidationSet);

            if (this._measure.IsError)
                solution.Quality = 1 - quality;
            else
                solution.Quality = quality;


        }
        
        #endregion
    }

}
