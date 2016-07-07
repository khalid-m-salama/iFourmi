using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.BayesianNetworks.Model;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.ProximityMeasures;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.BayesianLearning.ProblemSpecifics.ComponentInvalidators;
using iFourmi.BayesianLearning.ProblemSpecifics.HeuristicCalculators;
using iFourmi.BayesianLearning.ProblemSpecifics.LocalSearch;
using iFourmi.BayesianLearning.ProblemSpecifics.QualityEvaluators;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.Algorithms;

namespace iFourmi.BayesianLearning.Algorithms.GHC
{
    public class ClusterBMN:IClassificationAlgorithm
    {
        #region Data Members

        
        private int _clustersNumber;
        private Dataset _trainingset;        
        private IClassificationQualityMeasure _classificationMeasure;
        private BayesianClusterMultinetClassifier _BMNClassifier;
        private IClusteringAlgorithm _clusteringAlgorithm;
        private IClassificationAlgorithm _classificationAlgorithm;
        private ISimilarityMeasure _similarityMeasure;
        private ClusteringSolution _clusteringSolution;

        #endregion
        
        #region Propoerties
        
        public Dataset Dataset
        {
            get 
            {
                return this._trainingset;

            }

            set
            {
                this._trainingset = value;
            }

        }

        public ClusteringSolution ClusteringSolution
        {
            get
            {
                return this._clusteringSolution;
            }
        }

        public BayesianClusterMultinetClassifier BMNClassifier
        {
            get
            {
                return this._BMNClassifier;
            }
        }

        #endregion

        public ClusterBMN(DataMining.Data.Dataset trainingset, int clustersNumber, ISimilarityMeasure similarityMeasure, IClassificationQualityMeasure classificationMeasure, IClusteringAlgorithm clusteringAlgorithm, IClassificationAlgorithm classificationAlgorithm)
        {
            this._trainingset = trainingset;
            this._similarityMeasure = similarityMeasure;
            this._clustersNumber = clustersNumber;
            this._classificationMeasure = classificationMeasure;
            this._clusteringAlgorithm = clusteringAlgorithm;
            this._classificationAlgorithm = classificationAlgorithm;
        }

        public ClusterBMN(int clustersNumber, ISimilarityMeasure similarityMeasure, IClassificationQualityMeasure classificationMeasure, IClusteringAlgorithm clusteringAlgorithm, IClassificationAlgorithm classificationAlgorithm)
        {
            
            this._similarityMeasure = similarityMeasure;
            this._clustersNumber = clustersNumber;
            this._classificationMeasure = classificationMeasure;
            this._clusteringAlgorithm = clusteringAlgorithm;
            this._classificationAlgorithm = classificationAlgorithm;

        }


        public void Initialize()
        {
            this._clusteringAlgorithm.Dataset = this._trainingset;
            this._clusteringAlgorithm.SimilarityMeasure = this._similarityMeasure;
            this._clusteringAlgorithm.Initialize();
            
        }
             

        public IClassifier CreateClassifier()
        {
            this.Initialize();
            this._clusteringSolution = this._clusteringAlgorithm.CreateClusters();
            this._BMNClassifier = BayesianClusterMultinetClassifier.ConstructClusterBMNClassifier(this._clusteringSolution, this._classificationAlgorithm, this._trainingset);            
            return this._BMNClassifier;
        }


    }
}
