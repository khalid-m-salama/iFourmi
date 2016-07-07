using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model;

namespace iFourmi.BayesianNetworks.Model
{
    public class BayesianMultinetClassifier : DataMining.Model.IClassifier
    {
        #region Data Members

        private Dictionary<int, BayesianNetwork> _bayesianNetworks;
        private DataMining.Data.Metadata _metadata;        
        private double[] _classProbabilities;

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

        public BayesianNetwork this[int classIndex]
        {
            get
            {
                return this._bayesianNetworks[classIndex];
            }
        }

        public double[] ClassProbabilities
        {
            get { return this._classProbabilities; }
        }

        public int[] EdgesCount
        {
            get
            {
                int[] edges = new int[this._bayesianNetworks.Count];
                for (int i = 0; i < this._bayesianNetworks.Count; i++)
                    edges[i] = this._bayesianNetworks[i].Edges.Length;
                return edges;
            }
        }

        #endregion
        
        #region Constructor
        

        public BayesianMultinetClassifier(DataMining.Data.Metadata metadata)
        {
            this._bayesianNetworks = new Dictionary<int, BayesianNetwork>();
            this._metadata = metadata;
                                   
        }


        

        #endregion

        #region Methods

        public void AddBayesianNetwork(int classIndex, BayesianNetwork network)
        {
            if (!this._bayesianNetworks.Keys.Contains(classIndex))
                this._bayesianNetworks.Add(classIndex, network);
            else
                this._bayesianNetworks[classIndex] = network;
        }
         


        public Prediction Classify(DataMining.Data.Example example)
        {
            example = example.Clone() as DataMining.Data.Example;
            example.Label = -1;

            this._classProbabilities = new double[this._metadata.Target.Values.Length];            
            int resultIndex = 0;

            for(int classIndex=0; classIndex < this._metadata.Target.Values.Length; classIndex++)
            {
                BayesianNetwork bayesianNetwork = this._bayesianNetworks[classIndex];                
                double probability = this._metadata.Target.ValueCounts[classIndex] / (double)this._metadata.Size;
                probability *= bayesianNetwork.GetProbability(example);

                this._classProbabilities[classIndex] = probability;

                if (probability > this._classProbabilities[resultIndex])
                    resultIndex = classIndex;
               
            }

            double sum = this._classProbabilities.Sum();
            for (int i = 0; i < this._classProbabilities.Length; i++)
                this._classProbabilities[i] /= sum;

            return new Prediction(resultIndex,this._classProbabilities[resultIndex]);
        }

        #endregion
    }
}
