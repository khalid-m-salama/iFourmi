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
    public class ACOClustering_MB: AntColony<int>,IClusteringAlgorithm
    {
        #region Data Members

        protected  DataMining.Data.Dataset _dataset;
        protected bool _performLocalSearch;
        protected int _clustersNumber;
        protected ClusteringSolution _clusteringSolution;
        protected DataMining.ProximityMeasures.ISimilarityMeasure _similarityMeasure;
        
        #endregion

        #region Properties

        public Dataset Dataset
        {
            get { return this._dataset; }
            set
            {
                this._dataset = value;

            }
        }

        public DataMining.ProximityMeasures.ISimilarityMeasure SimilarityMeasure
        {
            get
            {
                return this._similarityMeasure;
            }

            set
            {
                this._similarityMeasure = value;
            }
        }


        public ClusteringSolution ClustringSolution
        {
            get
            {
                return this._clusteringSolution;
            }
        }

        #endregion

        #region Constructors

        public ACOClustering_MB(int maxIterations, int colonySize, int convergenceIterations, Problem<int> problem, int clustersNumber, DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure, Dataset dataset, bool performLocalSearch)
            : base(maxIterations, colonySize, convergenceIterations, problem)
        {

            this._clustersNumber = clustersNumber;
            this._similarityMeasure = similarityMeasure;
            this._performLocalSearch = performLocalSearch;
            this.Dataset = dataset;
            


        }

        public ACOClustering_MB(int maxIterations, int colonySize, int convergenceIterations, Problem<int> problem, int clustersNumber, DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure, bool performLocalSearch)
            : base(maxIterations, colonySize, convergenceIterations, problem)
        {
            this._clustersNumber = clustersNumber;
            this._similarityMeasure = similarityMeasure;
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
            this._clusteringSolution = new ClusteringSolution(this._dataset, this._clustersNumber, this._similarityMeasure);
            this._graph = ConstructionGraphBuilder.BuildMBClusteringConstructionGraph(this.Dataset, this._clustersNumber);

            ((KMeansLocalSearch)this.Problem.LocalSearch).ClustersNumber = this._clustersNumber;            
            ((KMeansLocalSearch)this.Problem.LocalSearch).ProximityMatrix = this._clusteringSolution.ProximityMatrix;
            
            ((ClusteringMBInvalidator)this.Problem.ComponentInvalidator).ClustersNumber = this._clustersNumber;
            ((ClusteringQualityEvaluator)this.Problem.SolutionQualityEvaluator).ClusteringSolution = this._clusteringSolution;            
            ((ClusteringQualityEvaluator)((KMeansLocalSearch)this.Problem.LocalSearch).SolutionQualityEvaluator).ClusteringSolution = this._clusteringSolution;
            

            this.ConstructionGraph.InitializePheromone(1);
            this.ConstructionGraph.SetHeuristicValues(this._problem.HeuristicsCalculator,false);
            this._bestAnt = null;
            this._iterationBestAnt = null;

      
    
        }

        public override void Work()
        {
            
            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {
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
                this.ConstructionGraph.ResetValidation(true);
             

                Ant<int> ant = new Ant<int>(antIndex, this);
                ant.CreateSoltion();
                this.EvaluateSolutionQuality(ant.Solution);

                if (this._iterationBestAnt == null || ant.Solution.Quality > this._iterationBestAnt.Solution.Quality)
                    this._iterationBestAnt = ant;

                if (this.OnPostAntSolutionContruction != null)
                    this.OnPostAntSolutionContruction(ant, null);
            }
        }

        public override void EvaluateSolutionQuality(Solution<int> solution)
        {
            this._problem.SolutionQualityEvaluator.EvaluateSolutionQuality(solution);
        }


        public override void UpdatePheromoneLevels()
        {
            double phi1 = (double)(this._maxIterations - this._currentIteration) / _maxIterations;
            double phi2 = (double)(this._currentIteration) / _maxIterations;

            this._iterationBestAnt.DepositePheromone(phi1);
            this._bestAnt.DepositePheromone(phi2);

            this.ConstructionGraph.EvaporatePheromone(0.01);
        }

        public override void PostProcessing()
        {
            ((KMeansLocalSearch)this.Problem.LocalSearch).MaxIterations = 100;
            this.PerformLocalSearch(this._bestAnt);
            this._clusteringSolution.SetClusterExampleAssignment(this._bestAnt.Solution.ToList());
                    
        }

        public ClusteringSolution CreateClusters()
        {
            this.Initialize();
            this.Work();
            this.PostProcessing();
            return this._clusteringSolution;
        }

        public override string ToString()
        {
            return "ACOClustering_IB:" + "k=" + this._clustersNumber.ToString() + "-" + _dataset.Metadata.DatasetName;
        }

        #endregion

       
    }
}
