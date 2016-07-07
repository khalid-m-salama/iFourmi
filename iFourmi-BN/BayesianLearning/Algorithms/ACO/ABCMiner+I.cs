using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.BayesianNetworks.Model;
using iFourmi.BayesianLearning.ProblemSpecifics.ComponentInvalidators;
using iFourmi.BayesianLearning.ProblemSpecifics.HeuristicCalculators;
using iFourmi.BayesianLearning.ProblemSpecifics.LocalSearch;
using iFourmi.BayesianLearning.ProblemSpecifics.QualityEvaluators;
using iFourmi.DataMining.Algorithms;

namespace iFourmi.BayesianLearning.Algorithms.ACO
{
    public class ABCMinerPlusI : IClassificationAlgorithm
    {
        #region Data Members

        private int _maxIterations;
        private int _colonySize;
        private int _convergenceIterations;
        private int _currentIteration;
        private int _convergenceCounter;
        private bool _performLocalSearch;
        private Ant<Edge> _bestAnt1;
        private Ant<VariableTypeAssignment> _bestAnt2;
        protected Ant<Edge> _iterationBestAnt1;
        protected Ant<VariableTypeAssignment> _iterationBestAnt2;
        private double _lastQuality;
        private BayesianNetworks.Model.BayesianNetworkClassifier _bnclassifier;

        private ABC abcAlgorithm;
        private ABCMiner abcminerAlgorithm;
        private DataMining.Data.Dataset _trainingSet;

        #endregion

        #region Propoerties
        public Dataset Dataset
        {
            get
            {
                return this._trainingSet;
            }
            set
            {
                this._trainingSet = value;
                this.abcAlgorithm.Dataset = this._trainingSet;
                this.abcminerAlgorithm.Dataset = this._trainingSet;
            }
        }

        public ABC ABCAlgorithm
        {
            get { return this.abcAlgorithm; }
        }

        public ABCMiner ABCMinerAlgorithm
        {
            get { return this.abcminerAlgorithm; }
        }


        #endregion

        #region Events

        public event EventHandler OnPostAntSolutionContruction;
        public event EventHandler OnPostColonyIteration;

        #endregion

        #region Constructors

        public ABCMinerPlusI(int maxIterations, int colonySize, int localColonySize, int convergenceIterations, Problem<Edge> abcMinerProblem, Problem<VariableTypeAssignment> abcProblem, int maxDependencies, Dataset trainingset, bool performLocalSearch)
            : this(maxIterations, colonySize, localColonySize, convergenceIterations, abcMinerProblem, abcProblem, maxDependencies, performLocalSearch)
        {
            this._trainingSet = trainingset;
            this.abcAlgorithm.Dataset = this._trainingSet;
            this.abcminerAlgorithm.Dataset = this._trainingSet;
        }

        public ABCMinerPlusI(int maxIterations, int colonySize, int localColonySize, int convergenceIterations, Problem<Edge> abcMinerProblem, Problem<VariableTypeAssignment> abcProblem, int maxDependencies, bool performLocalSearch)
        {
            this._colonySize = colonySize;
            this._maxIterations = maxIterations;
            this._convergenceIterations = convergenceIterations;
            this._currentIteration = 0;
            this._performLocalSearch = performLocalSearch;

            this.abcAlgorithm = new ABC(0, localColonySize, 0, abcProblem, false);
            this.abcminerAlgorithm = new ABCMiner(0, localColonySize, 0, abcMinerProblem, maxDependencies, performLocalSearch);

        }

        #endregion

        #region Methods

        public IClassifier CreateClassifier()
        {
            this.Initialize();
            this.Work();

            this._bnclassifier = new BayesianNetworks.Model.BayesianNetworkClassifier(this._trainingSet.Metadata, this._bestAnt2.Solution.ToList(), this._bestAnt1.Solution.ToList());
            this._bnclassifier.LearnParameters(this._trainingSet);

            return this._bnclassifier;

        }



        public void Initialize()
        {
            this.abcAlgorithm.Initialize();
            this.abcminerAlgorithm.Initialize();
        }

        public void Work()
        {
            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {
                if (this._currentIteration % 5 == 0)
                {
                    DataMining.Data.Dataset[] datasets = this._trainingSet.SplitRandomly(0.7);
                    ((BayesianClassificationQualityEvaluator)this.abcAlgorithm.Problem.SolutionQualityEvaluator).LearningSet = datasets[0];
                    ((BayesianClassificationQualityEvaluator)this.abcminerAlgorithm.Problem.SolutionQualityEvaluator).ValidationSet = datasets[1];
                }
                
                this.CreateSolution();

                if (_performLocalSearch)
                    this.PerformLocalSearch(this._iterationBestAnt1, this._iterationBestAnt2);

                this.UpdateBestAnt();

                if (this.IsConverged())
                    break;

                this.UpdatePheromone();

                if (OnPostColonyIteration != null)
                    this.OnPostColonyIteration(this, null);
            }

            if (!_performLocalSearch)
                this.PerformLocalSearch(this._bestAnt1,this._bestAnt2);
        }

        public void CreateSolution()
        {
            this._iterationBestAnt1 = null;
            this._iterationBestAnt2 = null;

            for (int antIndex = 0; antIndex < this._colonySize; antIndex++)
            {
                this.abcAlgorithm.CreateSolution();
                List<VariableTypeAssignment> variableTypeAssignments = this.abcAlgorithm.IterationBestAnt.Solution.ToList();
                this.abcminerAlgorithm.SetInputVariableTypes(variableTypeAssignments);
                this.abcminerAlgorithm.CreateSolution();

                if (this._iterationBestAnt1 == null || this.abcminerAlgorithm.IterationBestAnt.Solution.Quality > this._iterationBestAnt1.Solution.Quality)
                {
                    this._iterationBestAnt1 = this.abcminerAlgorithm.IterationBestAnt;
                    this._iterationBestAnt2 = this.abcAlgorithm.IterationBestAnt;
                    this.abcAlgorithm.IterationBestAnt.Solution.Quality = this._iterationBestAnt1.Solution.Quality;

                }

                if (this.OnPostAntSolutionContruction != null)
                    this.OnPostAntSolutionContruction(this.abcminerAlgorithm.IterationBestAnt, null);
            }
        }

        public void PerformLocalSearch(Ant<Edge> ant1, Ant<VariableTypeAssignment> ant2)
        {
            abcminerAlgorithm.PerformLocalSearch(ant1);
            ((BayesianClassificationQualityEvaluator)abcAlgorithm.Problem.SolutionQualityEvaluator).InputVariableDependencies = ant1.Solution.ToList();
            abcAlgorithm.PerformLocalSearch(ant2);
        }

        public void UpdatePheromone()
        {
            this.abcminerAlgorithm.UpdatePheromoneLevels();
            this.abcAlgorithm.UpdatePheromoneLevels();

        }

        private void UpdateBestAnt()
        {
            if (this._bestAnt1 == null || _iterationBestAnt1.Solution.Quality > this._bestAnt1.Solution.Quality)
            {
                this._bestAnt1 = _iterationBestAnt1.Clone() as Ant<Edge>;
                this._bestAnt2 = _iterationBestAnt2.Clone() as Ant<VariableTypeAssignment>;
            }

        }

        private bool IsConverged()
        {
            bool IsConverged = false;
            if (this._iterationBestAnt1.Solution.Quality == this._lastQuality)
                this._convergenceCounter++;
            else
            {
                this._convergenceCounter = 0;
                this._lastQuality = this._iterationBestAnt1.Solution.Quality; ;
            }

            if (_convergenceCounter == this._convergenceIterations)
                IsConverged = true;
            else
                IsConverged = false;

            return IsConverged;

        }

        #endregion
    }
}
