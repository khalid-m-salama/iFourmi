using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model;


namespace iFourmi.DataMining.ClassificationMeasures
{
    public interface IClassificationMeasure
    {
        double CalculateMeasure(ConfusionMatrix[] list);

        double CalculateMeasure(IClassifier classifier, Data.Dataset dataset);

    }
}
