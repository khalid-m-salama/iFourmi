using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Model.Hierarchical
{
    public interface IHierarchicalClassifier
    {
      
        int[] Classify(Data.Example example);
    }
}
