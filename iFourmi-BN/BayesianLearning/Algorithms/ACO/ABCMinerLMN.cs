using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.DataMining.Data;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.BayesianNetworks.Model;
using iFourmi.BayesianLearning.ProblemSpecifics.ComponentInvalidators;
using iFourmi.BayesianLearning.ProblemSpecifics.HeuristicCalculators;
using iFourmi.BayesianLearning.ProblemSpecifics.LocalSearch;
using iFourmi.BayesianLearning.ProblemSpecifics.QualityEvaluators;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Algorithms;

namespace iFourmi.BayesianLearning.Algorithms.ACO
{
    public class ABCMinerLMN : AntColony<Edge>, IClassificationAlgorithm
    {
        #region Data Members

        private int _maxDependencies;
        private double[] _dependenciesProbability;
        private BayesianNetworks.Model.BayesianMultinetClassifier _mnClassifier;
        private DataMining.Data.Dataset _trainingSet;
        private int _currentDependencies;
        private int _bestDependencies;        
        private double _qualityFactor;



        #endregion

        #region Events

        public event EventHandler OnPostAntSolutionContruction;
        public event EventHandler OnPostColonyIteration;

        #endregion

        #region Properties

        public BayesianMultinetClassifier MultinetBayesianClassifier
        {
            get { return this._mnClassifier; }
        }

        public Dataset Dataset
        {
            get { return this._trainingSet; }
            set { this._trainingSet = value; }

        }


        #endregion

        #region Constructor

        public ABCMinerLMN(int maxIterations, int colonySize, int convergenceIterations, Problem<Edge> problem, int maxDependencies, Dataset trainingset)
            : base(maxIterations, colonySize, convergenceIterations, ConstructionGraphBuilder.BuildBNConstructionGraph(trainingset.Metadata), problem)
        {
            this._trainingSet = trainingset;
            this._maxDependencies = maxDependencies;
            this._dependenciesProbability = new double[maxDependencies];
        }

        #endregion

        #region Methods
        
        public override void Initialize()
        {
            base.Initialize();
            for (int index = 0; index < this._dependenciesProbability.Length; index++)
                this._dependenciesProbability[index] = 1 / (double)this._dependenciesProbability.Length;

        }

        public override void Work()
        {
            this._mnClassifier = new BayesianMultinetClassifier(this._trainingSet.Metadata);

            DataMining.Data.Dataset[] datasets = this._trainingSet.Split();

            for (int classIndex = 0; classIndex < this._trainingSet.Metadata.Target.Values.Length; classIndex++)
            {
                ((MICalculator)this._problem.HeuristicsCalculator).Dataset = datasets[classIndex];

                this.Initialize();

                ((likelihoodQualityEvaluator)this._problem.SolutionQualityEvaluator).QualityFactor = 0;
                ((likelihoodQualityEvaluator)this._problem.SolutionQualityEvaluator).TaregtClass = classIndex;
                ((likelihoodQualityEvaluator)this._problem.SolutionQualityEvaluator).TrainingSet = datasets[classIndex];

                BayesianNetwork bayesianNetwork = this.LearnLocalBayesianNetwork(classIndex, datasets[classIndex]);

                this._mnClassifier.AddBayesianNetwork(classIndex, bayesianNetwork);
            }

        }
        
        private BayesianNetwork LearnLocalBayesianNetwork(int classIndex, DataMining.Data.Dataset subDataset)
        {

            this._bestAnt = null;
            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {
                this.CreateSolution();
                this.PerformLocalSearch(this._iterationBestAnt);
                this.UpdateBestAnt();

                if (this.IsConverged())
                    break;

                this.UpdatePheromoneLevels();
                
                if (OnPostColonyIteration != null)
                    this.OnPostColonyIteration(this, null);
            }

            BayesianNetwork bayesianNetwork = new BayesianNetworks.Model.BayesianNetwork(subDataset.Metadata, this._bestAnt.Solution.ToList());
            bayesianNetwork.LearnParameters(subDataset);
            return bayesianNetwork;

        }

        public override void CreateSolution()
        {
            this._iterationBestAnt = null;

            for (int antIndex = 0; antIndex < this.ColonySize; antIndex++)
            {
                this.ConstructionGraph.ResetValidation();
                this.SetVariableMaxDependencies();

                Ant<Edge> ant = new Ant<Edge>(antIndex, this);
                ant.CreateSoltion();
                this.EvaluateSolutionQuality(ant.Solution);

                //ant.DepositePheromone(0.1);

                if (this._iterationBestAnt != null)
                {
                    if (ant.Solution.Quality > this._iterationBestAnt.Solution.Quality)
                    {
                        this._iterationBestAnt = ant;
                        this._bestDependencies = this._currentDependencies;
                    }
                }
                else
                {
                    this._iterationBestAnt = ant;
                    this._bestDependencies = this._currentDependencies;
                }

                if (this.OnPostAntSolutionContruction != null)
                    this.OnPostAntSolutionContruction(ant, null);
            }
        }

        public override void EvaluateSolutionQuality(Solution<Edge> solution)
        {
            this._problem.SolutionQualityEvaluator.EvaluateSolutionQuality(solution);
        }

        private void SetVariableMaxDependencies()
        {
            double randomNumber = DataMining.Utilities.RandomUtility.GetNextDouble(0, 1);
            double sum = 0;

            for (int index = 0; index < this._dependenciesProbability.Length; index++)
            {
                sum += this._dependenciesProbability[index];
                if (sum >= randomNumber)
                {
                    this._currentDependencies = index + 1;
                    break;
                }

            }

            ((CyclicRelationInvalidator)this.Problem.ComponentInvalidator).MaxDependencies = this._currentDependencies;

        }

        private void UpdateVariableMaxDependencies(Ant<Edge> ant)
        {
            int bestIndex = this._bestDependencies - 1;
            double currentProbability = this._dependenciesProbability[bestIndex];
            this._dependenciesProbability[bestIndex] += currentProbability * ant.Solution.Quality * 0.5;

            double sum = 0;
            foreach (double value in this._dependenciesProbability)
                sum += value;
            for (int index = 0; index < this._dependenciesProbability.Length; index++)
                this._dependenciesProbability[index] /= sum;

        }

        public override void UpdatePheromoneLevels()
        {
            this._iterationBestAnt.DepositePheromone(0.5);
            this.UpdateVariableMaxDependencies(this._iterationBestAnt);
            this.ConstructionGraph.EvaporatePheromone();
        }

        public IClassifier CreateClassifier()
        {
            this.Work();
            this._mnClassifier.Desc = "ABCMiner-" + this._trainingSet.Metadata.DatasetName;
            return this._mnClassifier;
        }

        #endregion
    }
}
