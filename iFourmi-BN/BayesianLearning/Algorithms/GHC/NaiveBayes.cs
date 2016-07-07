using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.BayesianLearning.Algorithms.GHC
{
    public class NaiveBayesAlgorithm:iFourmi.DataMining.Algorithms.IClassificationAlgorithm
    {
        private iFourmi.DataMining.Data.Dataset _trainingset;

        public iFourmi.DataMining.Data.Dataset Dataset
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

        public iFourmi.DataMining.Model.IClassifier CreateClassifier()
        {
            iFourmi.BayesianNetworks.Model.BayesianNetworkClassifier bnclassifier = new iFourmi.BayesianNetworks.Model.BayesianNetworkClassifier(this._trainingset.Metadata);        
            bnclassifier.LearnParameters(this._trainingset);
            return bnclassifier;
        }
    }
}
