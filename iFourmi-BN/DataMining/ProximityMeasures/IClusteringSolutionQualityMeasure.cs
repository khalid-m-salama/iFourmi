using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ProximityMeasures
{
    public interface IClusteringQualityMeasure
    {
        double CalculateQuality(Model.ClusteringSolution clusteringSolution);
    }
}
