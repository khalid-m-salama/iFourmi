using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model.Hierarchical;

namespace iFourmi.DataMining.Algorithms.Hierarchical
{
    public interface IHierarchicalClassificationAlgorithm
    {
        Data.Dataset TrainingSet
        {
            get;
            set;
        }

        IHierarchicalClassifier CreateClassifier();
    }
}
