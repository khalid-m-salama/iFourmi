using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Model
{
    [Serializable()]
    public class ClassifierInfo
    {
        public string Desc
        {
            get;
            set;
        }

        public double Quality
        {
            get;
            set;
        }

        public IClassifier Classifier
        {
            get;
            set;
        }
    }
}
