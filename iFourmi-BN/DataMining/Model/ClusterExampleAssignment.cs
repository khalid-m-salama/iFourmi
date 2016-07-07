using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Model
{
    public struct ClusterExampleAssignment
    {
        public int ExampleID;
        public int ClusterLabel;

        public ClusterExampleAssignment(int exampleID, int clusterLabel)
        {
            this.ExampleID = exampleID;
            this.ClusterLabel = clusterLabel;
        }

        public override string ToString()
        {
            return "(" + this.ExampleID.ToString() + "," + this.ClusterLabel.ToString() + ")";
        }
    }
}
