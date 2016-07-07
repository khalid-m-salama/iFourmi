using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.Algorithms
{
    public interface IClassificationAlgorithm
    {
        Data.Dataset Dataset
        {
            get;
            set;
        }

        IClassifier CreateClassifier();
    }
}
