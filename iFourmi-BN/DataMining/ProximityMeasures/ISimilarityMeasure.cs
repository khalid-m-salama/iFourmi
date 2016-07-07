using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;


namespace iFourmi.DataMining.ProximityMeasures
{
    public interface ISimilarityMeasure
    {
        double CalculateSimilarity(Example example1,Example example2);
    }
}
