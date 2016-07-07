using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weka.classifiers.evaluation;
using weka.core.converters;
using weka.filters;
using weka.filters.unsupervised.attribute;
using weka.core;
using weka.classifiers;

namespace iFourmi.WekaNETBridge
{
    public interface IWekaClassificationAlgorithm
    {
        WekaClassifier CreateWekaClassifier();
  
    }
}
