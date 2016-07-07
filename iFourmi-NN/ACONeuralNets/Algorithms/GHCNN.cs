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
    public class GHCNN:AntColony<ConnectionDC>, IClassificationAlgorithm
    {
        #region Data Members

        protected int _maxEvaluations;

        protected int _hiddenUnitCount;
        protected bool _wisdom;
        protected DataMining.Data.Dataset _trainingSet;
        protected bool _performLocalSearch;

 
        private Solution<ConnectionDC> _solution;


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

        #region Events

        public event EventHandler OnPostEvaluation;

        #endregion
       
        #region Constructors

        public GHCNN(int maxIterations, int colonySize, int convergenceIterations, Problem<ConnectionDC> problem, int hiddenUnitCount, bool wisdom, bool performLocalSearch, Dataset trainingSet)
            : this(maxIterations, colonySize, convergenceIterations, problem, hiddenUnitCount, wisdom, performLocalSearch)
        {
            
            this.Dataset = trainingSet;
        }

        public GHCNN(int maxIterations, int colonySize, int convergenceIterations, Problem<ConnectionDC> problem, int hiddenUnitCount, bool wisdom, bool performLocalSearch)
            : base(maxIterations, colonySize, convergenceIterations, problem)
        {
            this._maxEvaluations = this._maxIterations * this._colonySize;
            this._wisdom = wisdom;
            this._hiddenUnitCount = hiddenUnitCount;
            this._performLocalSearch = performLocalSearch;
           
        }

        #endregion

        #region Methods

        public override void Initialize()
        {
            this._iterationBestAnt = new Ant<ConnectionDC>(0, this);
            this._bestAnt = new Ant<ConnectionDC>(0, this);

            SetLearningAndValidationSets();

            this._graph = ConstructionGraphBuilder.BuildNNConstructionGraph(this.Dataset.Metadata, _hiddenUnitCount);

            this._solution = new Solution<ConnectionDC>();
            foreach (DecisionComponent<ConnectionDC> component in this.ConstructionGraph.Components)
                if (component.Element.Include)
                    this._solution.Components.Add(component);

            this.EvaluateSolutionQuality(this._solution);
        }

        public override void Work()
        {
            
            CreateSolution();
        }


        private void SetLearningAndValidationSets()
        {
            DataMining.Data.Dataset[] datasets = this._trainingSet.SplitRandomly(0.8);
            ((NNClassificationQualityEvaluator)this.Problem.SolutionQualityEvaluator).LearningSet = datasets[0];
            ((NNClassificationQualityEvaluator)this.Problem.SolutionQualityEvaluator).ValidationSet = datasets[1];

        }

        public override void CreateSolution()
        {          

            for (int i = 0; i < this._solution.Components.Count; i++)
            {
                if(this._currentIteration==this._maxEvaluations-1)
                    break;

                double qualityBefore = this._solution.Quality;
                this._solution.Components[i].Element.Include = false;

                this.EvaluateSolutionQuality(this._solution);
                double qualityAfter = this._solution.Quality;

                if (qualityBefore > qualityAfter)
                {
                    this._solution.Components[i].Element.Include = true;
                    this._solution.Quality=qualityBefore;
                }

                this._currentIteration++;

          
                this._iterationBestAnt.Solution = this._solution;
                this._bestAnt.Solution = this._solution;

                if (this.OnPostEvaluation != null)
                    this.OnPostEvaluation(this, null);
            }


        }

        public override void EvaluateSolutionQuality(Solution<ConnectionDC> solution)
        {
            this.Problem.SolutionQualityEvaluator.EvaluateSolutionQuality(solution);
        }

        public override void PostProcessing()
        {
            NNClassificationQualityEvaluator evaluator = this._problem.SolutionQualityEvaluator as NNClassificationQualityEvaluator;
            NeuralNetwork network = evaluator.NeuralNetwork;
            this.NetworkBeforePostProcessing = network;

            //this.PerformLocalSearch(this._bestAnt);            

            //DataMining.ClassificationMeasures.MSError error = new DataMining.ClassificationMeasures.MSError();

            NeuralNetworks.LearningMethods.BackPropagation BP = new NeuralNetworks.LearningMethods.BackPropagation(0.01, 1000, 0.9, true);
            this.FinalNetwork = evaluator.CreateNeuralNetwork(this._solution, BP);

        }

        public IClassifier CreateClassifier()
        {
            this.Initialize();
            this.Work();
            this.PostProcessing();
            return this.FinalNetwork;
        }


        #endregion
    }
}
