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
    public class NNClassificationQualityEvaluator : ISolutionQualityEvaluator<ConnectionDC>
    {
        #region Data Members
        
        private DataMining.Data.Dataset _validationSet;
        private DataMining.Data.Dataset _learningSet;
        private DataMining.ClassificationMeasures.IClassificationMeasure _measure;
        private NeuralNetworks.LearningMethods.ILearningMethod _learningMethod;
        private int _hiddenUnitCount;
        private IActivationFunction _activationFunction;
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
         
        }

        public DataMining.ClassificationMeasures.IClassificationMeasure Measure
        {
            get { return this._measure; }
        }

        #endregion

        #region Constructor

        public NNClassificationQualityEvaluator(DataMining.ClassificationMeasures.IClassificationMeasure measure,NeuralNetworks.LearningMethods.ILearningMethod learningMethod, int hiddenUnitCount, IActivationFunction activationFunction)
        {

            this._measure = measure;
            this._learningMethod=learningMethod;            
            this._hiddenUnitCount = hiddenUnitCount;
            this._activationFunction = activationFunction;
        }

        #endregion

        #region Methods

        public void EvaluateSolutionQuality(Solution<ConnectionDC> solution)
        {
            double quality = 0;

            NeuralNetwork nnClassifier = this.CreateNeuralNetwork(solution,this._learningMethod);

            quality = this._measure.CalculateMeasure(nnClassifier,ValidationSet);

            if (this._measure.IsError)
                solution.Quality = 1 - quality;
            else
                solution.Quality = quality;


        }

  


        public NeuralNetwork CreateNeuralNetwork(Solution<ConnectionDC> solution,NeuralNetworks.LearningMethods.ILearningMethod learningMethod)
        {
            List<Connection> connections = new List<Connection>();
            for(int i=0;i<solution.Components.Count;i++)
                if (solution.Components[i].Element.Include)
                    connections.Add(solution.Components[i].Element.Connection);

            NeuralNetworks.Model.NeuralNetwork network = new NeuralNetwork(this._learningSet.Metadata,  this._hiddenUnitCount, this._activationFunction,connections.ToArray());
            network.TrainNetwork(this._learningSet, learningMethod);
            this._network = network;
            return this._network;

        }

        public NeuralNetwork CreateNeuralNetwork(Solution<ConnectionDC> solution)
        {
            List<Connection> connections = new List<Connection>();
            for (int i = 0; i < solution.Components.Count; i++)
                if (solution.Components[i].Element.Include)
                    connections.Add(solution.Components[i].Element.Connection);

            NeuralNetworks.Model.NeuralNetwork network = new NeuralNetwork(this._learningSet.Metadata, this._hiddenUnitCount, this._activationFunction, connections.ToArray());
            this._network = network;
            return this._network;

        }

        #endregion
    }

}
