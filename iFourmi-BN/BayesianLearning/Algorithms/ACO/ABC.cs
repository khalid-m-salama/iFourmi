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
   
    public class ABC : AntColony<VariableTypeAssignment>, IClassificationAlgorithm
    {

        #region Data Members

        private BayesianNetworks.Model.BayesianNetworkClassifier _bnclassifier;
        private DataMining.Data.Dataset _trainingSet;
        private bool _performLocalSearch;

        #endregion

        #region Properties

        public BayesianNetworkClassifier BayesianNetworkClassifier
        {
            get { return this._bnclassifier; }
        }

        public Dataset Dataset
        {
            get { return this._trainingSet; }
            set
            {
                this._trainingSet = value;
            }

        }

        #endregion

        #region Events

        public event EventHandler OnPostAntSolutionContruction;
        public event EventHandler OnPostColonyIteration;

        #endregion

        #region Constructors

        public ABC(int maxIterations, int colonySize, int convergenceIterations, Problem<VariableTypeAssignment> problem, Dataset trainingset, bool performLocalSearch)
            : base(maxIterations, colonySize, convergenceIterations,  problem)
        {
                        
            this._performLocalSearch = performLocalSearch;
            this.Dataset = trainingset;
                   

        }

        public ABC(int maxIterations, int colonySize, int convergenceIterations, Problem<VariableTypeAssignment> problem, bool performLocalSearch)
            : base(maxIterations, colonySize, convergenceIterations, problem)
        {
            this._performLocalSearch = performLocalSearch;
        }

        #endregion
        
        #region Methods

        public override void Initialize()
        {
            ((BayesianClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).ValidationSet = this._trainingSet;
            this._graph = ConstructionGraphBuilder.BuildABCConstructionGraph(this._trainingSet.Metadata);
            ((ILocalSearch<VariableTypeAssignment>)this._problem.LocalSearch).ConstructionGraph = this._graph;            
            base.Initialize();            
            this.ConstructionGraph.InitializePheromone(1);
            this.ConstructionGraph.SetHeuristicValues(this._problem.HeuristicsCalculator, false);
            this._bestAnt = null;
            this._iterationBestAnt = null;

        }


        public override void Work()
        {
         
            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {
                if (this._currentIteration % 5 == 0)
                {
                    DataMining.Data.Dataset[] datasets = this._trainingSet.SplitRandomly(0.7);
                    ((BayesianClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).LearningSet = datasets[0];
                    ((BayesianClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).ValidationSet = datasets[1];
                }

                this.CreateSolution();

                if (_performLocalSearch)
                    this.PerformLocalSearch(this._iterationBestAnt);

                this.UpdateBestAnt();
                

                if (this.IsConverged())
                    break;

                this.UpdatePheromoneLevels();

                if (OnPostColonyIteration != null)
                    this.OnPostColonyIteration(this, null);
            }

            if (!_performLocalSearch)
                this.PerformLocalSearch(this._bestAnt);

            this._bnclassifier = new BayesianNetworks.Model.BayesianNetworkClassifier(this._trainingSet.Metadata, this._bestAnt.Solution.ToList());
            this._bnclassifier.LearnParameters(this._trainingSet);
        }


        public override void CreateSolution()
        {
            this._iterationBestAnt = null;

            for (int antIndex = 0; antIndex < this.ColonySize; antIndex++)
            {
                this.ConstructionGraph.ResetValidation(false);
                for (int i = 0; i < 3; i++)
                    this.ConstructionGraph.Components[i].IsValid = true;

                Ant<VariableTypeAssignment> ant = new Ant<VariableTypeAssignment>(antIndex, this);
                ant.CreateSoltion();
                this.EvaluateSolutionQuality(ant.Solution);

                if (this._iterationBestAnt == null || ant.Solution.Quality > this._iterationBestAnt.Solution.Quality)
                    this._iterationBestAnt = ant;

                if (this.OnPostAntSolutionContruction != null)
                    this.OnPostAntSolutionContruction(ant, null);
            }
        }

        public override void EvaluateSolutionQuality(Solution<VariableTypeAssignment> solution)
        {
          
            this._problem.SolutionQualityEvaluator.EvaluateSolutionQuality(solution);
        }

        public override void UpdatePheromoneLevels()
        {
            this._iterationBestAnt.DepositePheromone(0.5);
            this.ConstructionGraph.EvaporatePheromone(0.01);
                        
        }

        public IClassifier CreateClassifier()
        {
            this.Initialize();
            this.Work();
            this._bnclassifier.Desc = "ABC -" + this._trainingSet.Metadata.DatasetName;
            return this._bnclassifier;
        }


        

        #endregion

     
    }
}
