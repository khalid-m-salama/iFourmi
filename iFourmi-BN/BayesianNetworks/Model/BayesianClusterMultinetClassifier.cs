using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Algorithms;

namespace iFourmi.BayesianNetworks.Model
{
    public class BayesianClusterMultinetClassifier : DataMining.Model.IClassifier
    {
        #region Data Members

        private Dictionary<int, BayesianNetworkClassifier> _bayesianNetworkClassfiers;
        private DataMining.Data.Metadata _metadata;
        private ClusteringSolution _clusteringSolution;


        #endregion

        #region Properties

        public DataMining.Data.Metadata Metadata
        {
            get { return this._metadata; }
        }

        public string Desc
        {
            get;
            set;
        }


        public ClusteringSolution ClusteringSolution
        {
            get { return _clusteringSolution; }
            set { _clusteringSolution = value; }
        }


        public BayesianNetworkClassifier this[int index]
        {
            get
            {
                return this._bayesianNetworkClassfiers[index];
            }
        }
 

        #endregion

        #region Constructors

        private BayesianClusterMultinetClassifier(DataMining.Data.Metadata metadata, ClusteringSolution clusteringSolution)
        {
            this._bayesianNetworkClassfiers = new Dictionary<int, BayesianNetworkClassifier>();
            this._metadata = metadata;
            this._clusteringSolution = clusteringSolution;

        }

        #endregion

        #region Methods

        private void AddBayesianNetworkClassifier(int clusterLabel, BayesianNetworkClassifier network)
        {
            if (!this._bayesianNetworkClassfiers.Keys.Contains(clusterLabel))
                this._bayesianNetworkClassfiers.Add(clusterLabel, network);
            else
                this._bayesianNetworkClassfiers[clusterLabel] = network;
        }
        
        public Prediction Classify(DataMining.Data.Example example)
        {
            example = example.Clone() as DataMining.Data.Example;
            example.Label = -1;            
            int clusterIndex = this._clusteringSolution.GetExampleNativeCluster(example);
            Prediction prediction = this._bayesianNetworkClassfiers[clusterIndex].Classify(example);
            return prediction;
        }

        public static BayesianClusterMultinetClassifier ConstructClusterBMNClassifier(ClusteringSolution clusteringSolution, IClassificationAlgorithm BayesianClassificationAlgorithms, DataMining.Data.Dataset trainingSet)
        {
            BayesianClusterMultinetClassifier BMNClassifier=new BayesianClusterMultinetClassifier(trainingSet.Metadata,clusteringSolution);
            foreach(Cluster cluster in clusteringSolution.Clusters)
            {
                BayesianClassificationAlgorithms.Dataset=cluster.ConvertToDataset();
                BayesianNetworkClassifier BNClassifier=BayesianClassificationAlgorithms.CreateClassifier() as BayesianNetworkClassifier;
                BMNClassifier.AddBayesianNetworkClassifier(cluster.Label,BNClassifier);
            }

            return BMNClassifier;
            
        }

        #endregion
    }
}
