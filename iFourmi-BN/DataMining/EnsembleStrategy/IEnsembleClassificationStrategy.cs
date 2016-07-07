using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.EnsembleStrategy
{
    public interface IEnsembleClassificationStrategy
    {
        int Classify(List<ClassifierInfo> classifiersInfo, Data.Example example);
        int Classify(List<ClassifierInfo> classifiersInfo, List<Data.Example> examples);
    }
}
