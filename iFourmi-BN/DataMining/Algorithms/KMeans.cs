using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.ProximityMeasures;

namespace iFourmi.DataMining.Algorithms
{
    public class KMeans:IClusteringAlgorithm
    {
        #region Data Members

        private int _clustersNumber;
        private int _maxIterations;
        private int _currentIteration;        
        private Dataset _dataset;
        private ClusteringSolution _clusteringSolution;
        //private int[] _belongingness;
        private bool _fireEvents;
        private ISimilarityMeasure _similarityMeasure;
        private double[,] _proximityMatrix;
        private bool _initialized;
        #endregion

        #region Propoerties
                
        public int CurrentIteration
        {
            get
            {
                return this._currentIteration;
            }
        }

        public ClusteringSolution ClusteringSolution
        {
            get
            {
                return this._clusteringSolution;
            }
        }

        public int MaxIterations
        {
            get{return this._maxIterations;}
            set { this._maxIterations = value; }
        }

        public Data.Dataset Dataset
        {
            get 
            {
                return this._dataset;
            }
            set
            {
                this._dataset=value;
     
            }
        }

        public ISimilarityMeasure SimilarityMeasure
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


        #endregion
        
        #region Events
        public event EventHandler OnPostIteration;
        #endregion

        #region Constructors

        public void Initialize()
        {
            if(_dataset==null)
                throw new Exception("Uninitialized Algorithm");
            if (_proximityMatrix != null && this._similarityMeasure != null)
                this._clusteringSolution = new ClusteringSolution(this._dataset, this._clustersNumber, this._similarityMeasure, _proximityMatrix);
            else if (this._similarityMeasure != null)
                this._clusteringSolution = new ClusteringSolution(this._dataset, this._clustersNumber, this._similarityMeasure);
            else 
                throw new Exception("Uninitialized Algorithm");

            this._initialized = true;               
            this.InitializeAssignment();
            
        }

        public KMeans(Dataset dataset,int  clustersNumber, ISimilarityMeasure similarityMeasure, int maxtIterations, bool fireEvents)
        {
            this._dataset = dataset;
            this._clustersNumber = clustersNumber;
            this._maxIterations = maxtIterations;
            this._similarityMeasure = similarityMeasure;            
            this._fireEvents = fireEvents;


        }

        public KMeans(Dataset dataset, int clustersNumber, ISimilarityMeasure similarityMeasure,double[,] proximityMatrix, int maxtIterations, bool fireEvents)
        {
            this._dataset = dataset;
            this._clustersNumber = clustersNumber;
            this._maxIterations = maxtIterations;
            this._proximityMatrix = proximityMatrix;
            this._similarityMeasure = similarityMeasure;   
            this._fireEvents = fireEvents;


        }


        #endregion

        #region Methods

        public Model.ClusteringSolution CreateClusters()
        {
            if (!this._initialized)
                throw new Exception("Uninitialized Algorithm");
            
            for (this._currentIteration=0; this._currentIteration < this._maxIterations; _currentIteration++)
            {
                bool same = true;

                foreach (Example example in this._dataset)
                {                       
                    int previousClusterIndex = this._clusteringSolution.Belongingness[example.Index];
                    int nearstClusterIndex = this._clusteringSolution.GetExampleNativeCluster(example);
                    this._clusteringSolution.DeassignExample(example, previousClusterIndex);                
                    this._clusteringSolution.AssignExample(example, nearstClusterIndex);                    
                    same &= (nearstClusterIndex == previousClusterIndex);
                }

                if (_fireEvents && this.OnPostIteration!=null)
                    this.OnPostIteration(this,null);

                if (same)
                    break;
            }

            return this._clusteringSolution;
        }

        public void InitializeAssignment()
        {
            if(!this._initialized)
                throw new Exception("Uninitialized Algorithm");

            foreach (Example example in this._dataset)
            {
                int clusterIndex = Utilities.RandomUtility.GetNextInt(0, this._clustersNumber);
                this._clusteringSolution.AssignExample(example, clusterIndex);
            }
        }

        public void SetAssignment(List<ClusterExampleAssignment> assignments)
        {
            if (!this._initialized)
                throw new Exception("Uninitialized Algorithm");

            this._clusteringSolution.SetClusterExampleAssignment(assignments);
            
        }

        public void SetAssignment(List<int> medoidExampleIndexes)
        {
            if (!this._initialized)
                throw new Exception("Uninitialized Algorithm");

            this._clusteringSolution.SetClusterExampleAssignment(medoidExampleIndexes);
        }
        #endregion
    }
}
