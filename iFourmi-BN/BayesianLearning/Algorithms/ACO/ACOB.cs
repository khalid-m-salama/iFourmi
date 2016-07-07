using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.BayesianNetworks.Model;
using iFourmi.BayesianLearning.ProblemSpecifics.ComponentInvalidators;
using iFourmi.BayesianLearning.ProblemSpecifics.HeuristicCalculators;
using iFourmi.BayesianLearning.ProblemSpecifics.LocalSearch;
using iFourmi.BayesianLearning.ProblemSpecifics.QualityEvaluators;


namespace iFourmi.BayesianLearning.Algorithms.ACO
{
    public class ACOB : AntColony<Edge>
    {
        #region Data Members

        private int _dependencies;
        private double[] _dependenciesProbability;
        private BayesianNetworks.Model.BayesianNetwork _bayesianNetwork;
        private DataMining.Data.Dataset _trainingSet;
        private int _currentDependencies;
        private int _bestDependencies;
        private double _lastQuality;

        #endregion

        #region Events

        public event EventHandler OnPostAntSolutionContruction;
        public event EventHandler OnPostColonyIteration;

        #endregion

        #region Properties

        public BayesianNetwork MultinetBayesianClassifier
        {
            get { return this._bayesianNetwork; }
        }

        public int CurrentDepenencies
        {
            get { return this._currentDependencies; }
        }

        public int BestDependencies
        {
            get { return this._bestDependencies; }
            set { this._bestDependencies = value; }
        }

        #endregion

        public ACOB(int maxIterations, int colonySize, int convergenceIterations, Problem<Edge> problem, int maxDependencies, Dataset trainingset)
            : base(maxIterations, colonySize, convergenceIterations, ConstructionGraphBuilder.BuildBNConstructionGraph(trainingset.Metadata), problem)
        {
            this._trainingSet = trainingset;
            this._dependencies = maxDependencies;
            this._dependenciesProbability = new double[maxDependencies];
        }

        public override void Initialize()
        {
            base.Initialize();

            for (int index = 0; index < this._dependencies; index++)
                this._dependenciesProbability[index] = 1 / (double)this._dependencies;

        }

        public override void Work()
        {
            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {
                this.CreateSolution();
                this.PerformLocalSearch(this._iterationBestAnt);
                this.UpdateBestAnt();

                if (this.IsConverged())
                    break;

                this.UpdatePheromoneLevels();
                this.UpdateVariableMaxDependencies(this._iterationBestAnt);

                if (OnPostColonyIteration != null)
                    this.OnPostColonyIteration(this, null);
            }

            BayesianNetwork bayesianNetwork = new BayesianNetworks.Model.BayesianNetwork(this._trainingSet.Metadata, this._bestAnt.Solution.ToList());
            bayesianNetwork.LearnParameters(this._trainingSet);
            this._bayesianNetwork = bayesianNetwork;
        }

        public override void CreateSolution()
        {
            this._iterationBestAnt = null;

            for (int antIndex = 0; antIndex < this.ColonySize; antIndex++)
            {
                this.ConstructionGraph.ResetValidation();
                this.SetAntVariableMaxDependencies();

                Ant<Edge> ant = new Ant<Edge>(antIndex, this);
                ant.CreateSoltion();
                this.EvaluateSolutionQuality(ant.Solution);
                
                if (this._iterationBestAnt == null)
                {
                    this._iterationBestAnt = ant;
                    this._bestDependencies = this._currentDependencies;

                }

                else if (ant.Solution.Quality > this._iterationBestAnt.Solution.Quality)
                {
                    this._iterationBestAnt = ant;
                    this._bestDependencies = this._currentDependencies;
                }

                if (this.OnPostAntSolutionContruction != null)
                    this.OnPostAntSolutionContruction(ant, null);
            }
        }
        
        public void SetAntVariableMaxDependencies()
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

        public override void EvaluateSolutionQuality(Solution<Edge> solution)
        {
            this._problem.SolutionQualityEvaluator.EvaluateSolutionQuality(solution);
        }

        public void UpdateVariableMaxDependencies(Ant<Edge> ant)
        {
            int bestIndex = this._bestDependencies - 1;
            double currentProbability = this._dependenciesProbability[bestIndex];
            this._dependenciesProbability[bestIndex] += currentProbability * ant.Solution.Quality * 0.5;
                                   
            double sum = 0;
            for (int index = 0; index < this._dependenciesProbability.Length; index++)            
                sum += this._dependenciesProbability[index];
            
            for (int index = 0; index < this._dependenciesProbability.Length; index++)            
                this._dependenciesProbability[index] /= sum;            
        }

        

     
    }
}
