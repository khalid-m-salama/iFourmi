using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;


namespace iFourmi.DataMining.ProximityMeasures
{
    public interface IDistanceMeasure
    {
        double CalculateDistance(Instance instance1,Instance instance2);

        double CalculateDistance(Instance instance1, Instance instance2, double[] weights);

        double CalculateDistance(Instance instance1, Instance instance2, double[][] weights);


    }
}
