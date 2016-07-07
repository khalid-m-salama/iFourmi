using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;


namespace iFourmi.DataMining.ProximityMeasures
{
    public interface IDistanceMeasure
    {
        double CalculateDistance(Example example1,Example example2);

        double CalculateDistance(Example example1, Example example2, double[] weights);

        double CalculateDistance(Example example1, Example example2, double[][] weights);


    }
}
