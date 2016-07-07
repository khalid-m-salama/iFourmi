using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Model
{
    public interface IClassifier
    {
        Data.Metadata Metadata
        {
            get;
        }

        string Desc
        {
            get;
            set;
        }
        
        Prediction Classify(Data.Example example);

    }
}
