using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public interface IClassificationQualityMeasure
    {
        double CalculateMeasure(ConfusionMatrix[] list);
    }
}
