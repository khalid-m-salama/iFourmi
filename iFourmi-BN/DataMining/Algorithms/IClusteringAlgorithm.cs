using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.ProximityMeasures;

namespace iFourmi.DataMining.Algorithms
{
    public interface IClusteringAlgorithm
    {     

        Data.Dataset Dataset
        {
            get;
            set;
        }

        ISimilarityMeasure SimilarityMeasure
        {
            get;
            set;
        }


        void Initialize();

        ClusteringSolution CreateClusters();
    }
}
