using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Algorithms;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.BayesianLearning.ProblemSpecifics.QualityEvaluators;
using iFourmi.DataMining.ProximityMeasures;

namespace iFourmi.BayesianLearning.ProblemSpecifics.LocalSearch
{
    public class KMeansLocalSearch: ILocalSearch<ClusterExampleAssignment>, ILocalSearch<int>
    {
        #region Data Members

        private int _maxIterations;
        private KMeans _kmeans;
        private double[,] _proximityMatrix;
        private DataMining.Data.Dataset _dataset;
        private ISimilarityMeasure _similarityMeasure;
        
               
        #endregion

        #region Properties

        

        ConstructionGraph<int> ILocalSearch<int>.ConstructionGraph
        {
            get;
            set;
        }

        ConstructionGraph<ClusterExampleAssignment> ILocalSearch<ClusterExampleAssignment>.ConstructionGraph
        {
            get;
            set;
        }

        


        public double [,] ProximityMatrix
        {
            get
            {
                return this._proximityMatrix;
            }

            set
            {
                this._proximityMatrix = value;
                this._kmeans = new KMeans(this._dataset, this.ClustersNumber,this._similarityMeasure,this._proximityMatrix, this._maxIterations, false);
                this._kmeans.Initialize();
            }
        }


        public int ClustersNumber
        {
            get;
            set;
        }

        public int MaxIterations
        {
            get
            {
                return this._maxIterations;
            }
            set
            {
                this._maxIterations = value;
                this._kmeans.MaxIterations = value;
                
            }
        }

        public ClusteringQualityEvaluator SolutionQualityEvaluator
        {
            get;
            set;

        }
 
        


        #endregion

        #region Constructors

        public KMeansLocalSearch(DataMining.Data.Dataset dataset, int maxIterations, ISimilarityMeasure similarityMeasure, ClusteringQualityEvaluator evaluator)
        {
            this._dataset = dataset;
            this._maxIterations = maxIterations;
            this.SolutionQualityEvaluator = evaluator;
            this._similarityMeasure = similarityMeasure;

        }

        #endregion

        #region method

        public void PerformLocalSearch(Ant<ClusterExampleAssignment> ant)
        {
            Solution<ClusterExampleAssignment> originalSolution = ant.Solution;
            _kmeans.SetAssignment(originalSolution.ToList());
            ClusteringSolution clusteringSolution = _kmeans.CreateClusters();            

            Solution<ClusterExampleAssignment> optimizedSolution = new Solution<ClusterExampleAssignment>();
            List<int> optimizedTrail = new List<int>();

            foreach (Cluster cluster in clusteringSolution.Clusters)
            {
                foreach (DataMining.Data.Example example in cluster.Examples)
                {
                    int componentIndex = (example.Index * ClustersNumber) + cluster.Label;
                    optimizedSolution.Components.Add (new DecisionComponent<ClusterExampleAssignment>(componentIndex, new ClusterExampleAssignment(example.Index, cluster.Label)));
                    optimizedTrail.Add(  componentIndex);

                }
            }

            
            this.SolutionQualityEvaluator.EvaluateSolutionQuality(optimizedSolution);


            if (optimizedSolution.Quality > originalSolution.Quality)
            {
                ant.Solution = optimizedSolution;
                ant.Trail = optimizedTrail;
            }

            
        }

        public void PerformLocalSearch(Ant<int> ant)
        {
             Solution<int> originalSolution = ant.Solution;
            _kmeans.SetAssignment(originalSolution.ToList());
            ClusteringSolution clusteringSolution = _kmeans.CreateClusters();            

            Solution<int> optimizedSolution = new Solution<int>();
            List<int> optimizedTrail = new List<int>();


            int[] optimizedMedoids = clusteringSolution.GetMedoids();

            foreach (int exampleIndex in optimizedMedoids)
            {
                optimizedSolution.Components.Add(new DecisionComponent<int>(exampleIndex,exampleIndex));
                optimizedTrail.Add(exampleIndex);
            }

            
            this.SolutionQualityEvaluator.EvaluateSolutionQuality(optimizedSolution);


            if (optimizedSolution.Quality > originalSolution.Quality)
            {
                ant.Solution = optimizedSolution;
                ant.Trail = optimizedTrail;
            }

            
        }
        

        #endregion

       
    }
}
