using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ProximityMeasures
{
    public class CohesionClusteringMeasure:IClusteringQualityMeasure
    {
        public double CalculateQuality(Model.ClusteringSolution clusteringSolution)
        {
            return clusteringSolution.CalculateCohesion();
        }
    }
}
