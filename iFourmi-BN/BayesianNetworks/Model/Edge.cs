using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.BayesianNetworks.Model
{
    [Serializable()]
    public struct Edge
    {
        public int ParentIndex;
        public int ChildIndex;

        public Edge(int parentIndex, int childIdex)
        {
            this.ParentIndex = parentIndex;
            this.ChildIndex = childIdex;
        }

        public override string ToString()
        {
            return "("+this.ParentIndex.ToString()+","+this.ChildIndex.ToString()+")";
        }

    }
}
