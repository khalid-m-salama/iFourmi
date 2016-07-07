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
    public class ANNMiner : AntColony<ConnectionDC>, IClassificationAlgorithm
    {

        #region Data Members

        protected int _hiddenUnitCount;
        protected bool _wisdom;
        protected DataMining.Data.Dataset _trainingSet;
        protected bool _performLocalSearch;

        #endregion

        #region Properties

        public Dataset Dataset
        {
            get { return this._trainingSet; }
            set
            {
                this._trainingSet = value;

            }
        }

        public NeuralNetwork NetworkBeforePostProcessing
        {
            get;
            set;
        }

        public NeuralNetwork FinalNetwork
        {
            get;
            set;
        }



        #endregion

        #region Constructors

        public ANNMiner(int maxIterations, int colonySize, int convergenceIterations, Problem<ConnectionDC> problem, int hiddenUnitCount, bool wisdom, bool performLocalSearch, Dataset trainingSet)
            : this(maxIterations, colonySize, convergenceIterations, problem, hiddenUnitCount, wisdom, performLocalSearch)
        {
            this.Dataset = trainingSet;
        }

        public ANNMiner(int maxIterations, int colonySize, int convergenceIterations, Problem<ConnectionDC> problem, int hiddenUnitCount, bool wisdom, bool performLocalSearch)
            : base(maxIterations, colonySize, convergenceIterations, problem)
        {
            this._wisdom = wisdom;
            this._hiddenUnitCount = hiddenUnitCount;
            this._performLocalSearch = performLocalSearch;
        }

        #endregion

        #region Events

        public event EventHandler OnPostAntSolutionContruction;
        public event EventHandler OnPostColonyIteration;

        #endregion

        #region Methods

        public override void Initialize()
        {

            this._graph = ConstructionGraphBuilder.BuildNNConstructionGraph(this.Dataset.Metadata, _hiddenUnitCount);
            this.ConstructionGraph.InitializePheromone(1);
            this.ConstructionGraph.SetHeuristicValues(this._problem.HeuristicsCalculator, false);
            this._bestAnt = null;
            this._iterationBestAnt = null;

        }

        public override void Work()
        {
            SetLearningAndValidationSets();

            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {
                if (this._currentIteration % 5 == 0)
                    SetLearningAndValidationSets();
                
                this.CreateSolution();

                if (this._performLocalSearch)
                    this.PerformLocalSearch(this._iterationBestAnt);

                this.UpdateBestAnt();

                if (this.IsConverged())
                    break;

                this.UpdatePheromoneLevels();

                if (OnPostColonyIteration != null)
                    this.OnPostColonyIteration(this, null);
            }


        }

        public override void CreateSolution()
        {
            this._iterationBestAnt = null;

            for (int antIndex = 0; antIndex < this.ColonySize; antIndex++)
            {
                this.ConstructionGraph.ResetValidation(false);

                for (int i = 0; i < 2; i++)
                    this.ConstructionGraph.Components[i].IsValid = true;

                Ant<ConnectionDC> ant = new Ant<ConnectionDC>(antIndex, this);

                ant.CreateSoltion();

                this.EvaluateSolutionQuality(ant.Solution);

                if (this._iterationBestAnt == null || ant.Solution.Quality > this._iterationBestAnt.Solution.Quality)
                {
                    this._iterationBestAnt = ant;
                }

                if (this.OnPostAntSolutionContruction != null)
                    this.OnPostAntSolutionContruction(ant, null);
            }
        }

        public override void EvaluateSolutionQuality(Solution<ConnectionDC> solution)
        {
            this._problem.SolutionQualityEvaluator.EvaluateSolutionQuality(solution);
        }

        protected override void UpdateBestAnt()
        {
            if (this._bestAnt == null || _iterationBestAnt.Solution.Quality > this._bestAnt.Solution.Quality)
            {
                this._bestAnt = _iterationBestAnt.Clone() as Ant<ConnectionDC>;

                if (_wisdom)
                    this.UpdateWeightsOnConstructionGraph();
            }
        }

        private void UpdateWeightsOnConstructionGraph()
        {
            NeuralNetwork network = ((NNClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).NeuralNetwork;

            int offset1 = (network.InputLayer.Length * network.HiddenLayer.Length * 2) + (network.InputLayer.Length * network.OutputLayer.Length * 2);
            int offset2 = (network.InputLayer.Length * network.HiddenLayer.Length * 2);
            int fromIndex = 0;
            int index = -1;

            for (int to = 0; to < network.OutputLayer.Length; to++)
            {
                for (int from = 0; from < network.OutputLayer[to].From.Count; from++)
                {
                    fromIndex = network.OutputLayer[to].From[from].Index;

                    if (network.OutputLayer[to].From[from].Layer == LayerType.Hidden)
                    {
                        index = offset1 + (fromIndex * network.OutputLayer.Length * 2) + 2 * to;
                        this._graph.Components[index].Element.Connection.Weight = network.OutputLayer[to].Weights[from];
                    }
                    else
                    {

                        index = offset2 + (fromIndex * network.OutputLayer.Length * 2) + 2 * to;
                        this._graph.Components[index].Element.Connection.Weight = network.OutputLayer[to].Weights[from];
                    }

                }
            }


            offset1 = (network.InputLayer.Length * network.HiddenLayer.Length * 2) + (network.InputLayer.Length * network.OutputLayer.Length * 2) + (network.HiddenLayer.Length * network.OutputLayer.Length * 2);
            offset2 = 0;
            int value1 = (network.HiddenLayer.Length - 1) * (network.HiddenLayer.Length);
            int value2 = 0;


            for (int to = 0; to < network.HiddenLayer.Length; to++)
            {
                for (int from = 0; from < network.HiddenLayer[to].From.Count; from++)
                {
                    fromIndex = network.HiddenLayer[to].From[from].Index;

                    if (network.HiddenLayer[to].From[from].Layer == LayerType.Hidden)
                    {


                        value2 = (network.HiddenLayer.Length - fromIndex - 1) * (network.HiddenLayer.Length - fromIndex);

                        index = offset1 + value1 - value2 + to - fromIndex - 1;
                        this._graph.Components[index].Element.Connection.Weight = network.HiddenLayer[to].Weights[from];
                    }
                    else
                    {
                        index = offset2 + (fromIndex * network.HiddenLayer.Length * 2) + 2 * to;
                        this._graph.Components[index].Element.Connection.Weight = network.HiddenLayer[to].Weights[from];
                    }

                }
            }


        }

        public override void UpdatePheromoneLevels()
        {
            this._iterationBestAnt.DepositPheromone(1);
            this.ConstructionGraph.EvaporatePheromone(0.01);
        }

        public override void PostProcessing()
        {
            NNClassificationQualityEvaluator evaluator = this._problem.SolutionQualityEvaluator as NNClassificationQualityEvaluator;
            //NeuralNetwork network = evaluator.NeuralNetwork;
            NeuralNetwork network = evaluator.CreateNeuralNetwork(this._bestAnt.Solution);
            this.NetworkBeforePostProcessing = network;

            //this.PerformLocalSearch(this._bestAnt);            
            NeuralNetworks.LearningMethods.BackPropagation BP = new NeuralNetworks.LearningMethods.BackPropagation(0.01, 1000, 0.9, true);
            this.FinalNetwork = evaluator.CreateNeuralNetwork(this._bestAnt.Solution, BP);

        }

        public void PostProcessing2()
        {
            NNClassificationQualityEvaluator evaluator = this._problem.SolutionQualityEvaluator as NNClassificationQualityEvaluator;
            //NeuralNetwork network = evaluator.NeuralNetwork;
            NeuralNetwork network = evaluator.CreateNeuralNetwork(this._bestAnt.Solution);
            this.NetworkBeforePostProcessing = network;

            //this.PerformLocalSearch(this._bestAnt);            
            int maxIterations = 1000;
            int colonySize = 1;
            int convergence = 100;
            int archive = 25;
            double q = 0.25;
            double segma = 0.85;
            int problemSize = network.Size;           

            NNClassificationQualityEvaluator2 evaluator2 = new NNClassificationQualityEvaluator2(-5, 5, evaluator.Measure);
            evaluator2.LearningSet = _trainingSet;
            evaluator2.ValidationSet = _trainingSet;
            evaluator2.NeuralNetwork = network;

            Problem<double> problem = new Problem<double>(null, null, evaluator2, null);
            
            ACO_RNN acornn = new ACO_RNN(maxIterations, colonySize, convergence, problem, problemSize, archive, q, segma);
            acornn.OnPostColonyIteration += OnPostColonyIteration;

            acornn.TrainNetwork(network, _trainingSet);
            this.FinalNetwork = network;
            
        }

        private void SetLearningAndValidationSets()
        {
            DataMining.Data.Dataset[] datasets = this._trainingSet.SplitRandomly(0.8);
            ((NNClassificationQualityEvaluator)this.Problem.SolutionQualityEvaluator).LearningSet = datasets[0];
            ((NNClassificationQualityEvaluator)this.Problem.SolutionQualityEvaluator).ValidationSet = datasets[1];
 
        }

        public IClassifier CreateClassifier()
        {
            this.Initialize();
            this.Work();
            this.PostProcessing();
            return this.FinalNetwork;
        }

        public IClassifier CreateClassifier2()
        {
            this.Initialize();
            this.Work();
            this.PostProcessing2();
            return this.FinalNetwork;
        }

        public override string ToString()
        {
            return "ANN-Miner -" + _trainingSet.Metadata.DatasetName;
        }

        #endregion


    }
}
