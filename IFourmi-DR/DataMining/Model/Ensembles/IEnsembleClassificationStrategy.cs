using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.Model.Ensembles
{
    public interface IEnsembleClassificationStrategy
    {
        Prediction AggregatePredictions(List<KeyValuePair<ClassifierInfo,Prediction>> ensemblePredictions);
    }
}
