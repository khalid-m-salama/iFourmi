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
    public class ABCMiner : AntColony<Edge>,IClassificationAlgorithm
    {
        #region Data Members

        private double[] _dependenciesProbability;
        private BayesianNetworks.Model.BayesianNetworkClassifier _bnclassifier;
        private DataMining.Data.Dataset _trainingSet;        
        private int _currentDependencies;
        private int _bestDependencies;        
        private bool _performLocalSearch;
        private List<VariableTypeAssignment> _variableTypeAssignemts;

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

        #region Constructor


        public ABCMiner(int maxIterations, int colonySize, int convergenceIterations, Problem<Edge> problem, int maxDependencies, Dataset trainingset, bool performLocalSearch)
            :this(maxIterations, colonySize, convergenceIterations,  problem,maxDependencies,performLocalSearch)
        {   
            this._trainingSet = trainingset;         
        }

        public ABCMiner(int maxIterations, int colonySize, int convergenceIterations, Problem<Edge> problem, int maxDependencies, bool performLocalSearch)
            : base(maxIterations, colonySize, convergenceIterations, problem)
        {
            
            this._dependenciesProbability = new double[maxDependencies];
            this._performLocalSearch = performLocalSearch;


        }

        #endregion

        #region Methods

        public override void Initialize()
        {
            List<VariableTypeAssignment> variableTypes = new List<VariableTypeAssignment>();
            foreach (DataMining.Data.Attribute attribute in this._trainingSet.Metadata.Attributes)
                variableTypes.Add(new VariableTypeAssignment(attribute.Index, VariableType.Effect));
            this.SetInputVariableTypes(variableTypes);

            ((BayesianClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).ValidationSet = this._trainingSet;
            ((CMICalculator)this.Problem.HeuristicsCalculator).Dataset = this._trainingSet;
            this._graph = ConstructionGraphBuilder.BuildBNConstructionGraph(_trainingSet.Metadata);
            base.Initialize();
            for (int index = 0; index < this._dependenciesProbability.Length; index++)
                this._dependenciesProbability[index] = 1 / (double)this._dependenciesProbability.Length;


            
        }

        //public void Initialize(List<VariableTypeAssignment> variableTypeAssignments)
        //{
        //    this._variableTypeAssignemts = variableTypeAssignments;
        //    ((BayesianClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).VariableTypeAssignments = variableTypeAssignments;


        //    ((BayesianClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).ValidationSet = this._trainingSet;
        //    ((IHeuristicValueCalculator<Edge>)this.Problem.HeuristicsCalculator).Dataset = this._trainingSet;

        //    this._graph = ConstructionGraphBuilder.BuildBNConstructionGraph(_trainingSet.Metadata, variableTypeAssignments);
        //    base.Initialize();
        //    for (int index = 0; index < this._dependenciesProbability.Length; index++)
        //        this._dependenciesProbability[index] = 1 / (double)this._dependenciesProbability.Length;

        //}


        internal void SetInputVariableTypes(List<VariableTypeAssignment> variableTypeAssignments)
        {
            this._variableTypeAssignemts = variableTypeAssignments;
            ((BayesianClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).VariableTypeAssignments = variableTypeAssignments;
         
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
                if(_performLocalSearch)
                    this.PerformLocalSearch(this._iterationBestAnt);
                this.UpdateBestAnt();               

                if (this.IsConverged())
                    break;

                this.UpdatePheromoneLevels();

                if (OnPostColonyIteration != null)
                    this.OnPostColonyIteration(this, null);
            }

            if(!_performLocalSearch)
                this.PerformLocalSearch(this._bestAnt);

            if(_variableTypeAssignemts!=null)
                this._bnclassifier = new BayesianNetworks.Model.BayesianNetworkClassifier(this._trainingSet.Metadata,this._variableTypeAssignemts,this._bestAnt.Solution.ToList());
            else
                this._bnclassifier = new BayesianNetworks.Model.BayesianNetworkClassifier(this._trainingSet.Metadata,this._bestAnt.Solution.ToList());
            this._bnclassifier.LearnParameters(this._trainingSet);

            
        }

        public override void CreateSolution()
        {
            this._iterationBestAnt = null;

            for (int antIndex = 0; antIndex < this.ColonySize; antIndex++)
            {
                this.ResetConstructionGraphValidation();
                
                this.SetVariableMaxDependencies();

                Ant<Edge> ant = new Ant<Edge>(antIndex, this);
                ant.CreateSoltion();
                this.EvaluateSolutionQuality(ant.Solution);

                if (this._iterationBestAnt == null || ant.Solution.Quality > this._iterationBestAnt.Solution.Quality)
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

        private void ResetConstructionGraphValidation()
        {
            foreach (DecisionComponent<Edge> component in this.ConstructionGraph.Components)
            {
                Edge edge = component.Element;
                VariableType childVariableType = this._variableTypeAssignemts.Find(v => v.VariableIndex == edge.ChildIndex).Type;
                if (childVariableType == VariableType.Effect)
                    component.IsValid = true;
                else
                    component.IsValid = false;

            }
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

        public override void UpdatePheromoneLevels()
        {
            base.UpdatePheromoneLevels();
            this.UpdateVariableMaxDependencies(this._iterationBestAnt);
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
        
        public IClassifier CreateClassifier()
        {
            this.Initialize();
            this.Work();
            this._bnclassifier.Desc = "ABCMiner-" + this._trainingSet.Metadata.DatasetName;
            return this._bnclassifier;
        }

        #endregion






    }
}
