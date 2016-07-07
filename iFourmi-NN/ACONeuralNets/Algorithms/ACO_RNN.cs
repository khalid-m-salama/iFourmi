using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACONeuralNets;
using iFourmi.ACONeuralNets.Algorithms;
using iFourmi.ACONeuralNets.ProblemSpecifics.ComponentInvalidators;
using iFourmi.ACONeuralNets.ProblemSpecifics.HeuristicCalculators;
using iFourmi.ACONeuralNets.ProblemSpecifics.LocalSearch;
using iFourmi.ACONeuralNets.ProblemSpecifics.QualityEvaluators;
using iFourmi.DataMining.Algorithms;
using iFourmi.NeuralNetworks.Model;



namespace iFourmi.ACONeuralNets.Algorithms
{
    public class ACO_RNN:ContinuousACO.Algorithms.ACO_R,NeuralNetworks.LearningMethods.ILearningMethod,IClassificationAlgorithm
    {
        #region Data Members

        private NeuralNetwork _network;

        protected DataMining.Data.Dataset _trainingSet;

        #endregion

        public new event EventHandler OnPostColonyIteration;

        public DataMining.Data.Dataset Dataset
        {
            get
            {
                return this._trainingSet;
            }
            set
            {
                this._trainingSet = value;

            }
        }

        public ACO_RNN(int maxIterations, int colonySize, int convergenceIterations, Problem<double> problem, int problemSize, int archive, double q, double segma)
            :base(maxIterations, colonySize, convergenceIterations, problem, problemSize, archive, 0, q, segma)
        {
            this._maxIterations = 1000;
            this._colonySize = 1;
            this._convergenceIterations = 100;
        }



        public void TrainNetwork(NeuralNetwork network, Dataset trainingSet)
        {
            this._trainingSet = trainingSet;
            this._network=network;

            this.Work();
            
        }

        public override void Work()
        {
            SetLearningAndValidationSets();
            

            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {
                this.CreateSolution();

                if (this.IsConverged())
                    break;

                this.UpdateBestAnt();

                this.UpdatePheromoneLevels();

                if (OnPostColonyIteration != null)
                    this.OnPostColonyIteration(this, null);

            }
        }


        private void SetLearningAndValidationSets()
        {
            ((NNClassificationQualityEvaluator2)this.Problem.SolutionQualityEvaluator).NeuralNetwork=this._network;
            DataMining.Data.Dataset[] datasets = this._trainingSet.SplitRandomly(0.8);
            ((NNClassificationQualityEvaluator2)this.Problem.SolutionQualityEvaluator).LearningSet = datasets[0];
            ((NNClassificationQualityEvaluator2)this.Problem.SolutionQualityEvaluator).ValidationSet = datasets[1];
        }

        public DataMining.Model.IClassifier CreateClassifier()
        {
            this.Work();

            this._problem.SolutionQualityEvaluator.EvaluateSolutionQuality(this.GlobalBest);
            NeuralNetwork network = ((NNClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).NeuralNetwork;

            return network;
        }

     }
}
