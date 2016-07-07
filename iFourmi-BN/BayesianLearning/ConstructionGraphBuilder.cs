using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.DataMining.Data;
using iFourmi.BayesianNetworks.Model;
using iFourmi.DataMining.Model;
using iFourmi.BayesianLearning.Algorithms.ACO;

namespace iFourmi.BayesianLearning
{
    public static class ConstructionGraphBuilder
    {
        public static ConstructionGraph<Edge> BuildBNConstructionGraph(Metadata metadata)
        {
            int counter = 0;
            List<DecisionComponent<Edge>> decisionComponents = new List<DecisionComponent<Edge>>();
            for (int parentIndex = 0; parentIndex < metadata.Attributes.Length; parentIndex++)
                for (int childIndex = 0; childIndex < metadata.Attributes.Length; childIndex++)
                    if (parentIndex != childIndex)
                        decisionComponents.Add(new DecisionComponent<Edge>(counter++, new Edge(parentIndex, childIndex)));

            return new ConstructionGraph<Edge>(decisionComponents.ToArray());
        }


        //public static ConstructionGraph<Edge> BuildBNConstructionGraph(Metadata metadata, List<VariableTypeAssignment> variableTypeAssignments)
        //{
        //    int counter = 0;
        //    List<DecisionComponent<Edge>> decisionComponents = new List<DecisionComponent<Edge>>();
        //    for (int parentIndex = 0; parentIndex < metadata.Attributes.Length; parentIndex++)
        //        for (int childIndex = 0; childIndex < metadata.Attributes.Length; childIndex++)
        //            if (parentIndex != childIndex)
        //            {                        
        //                VariableType childVariableType = variableTypeAssignments.Find(v => v.VariableIndex == childIndex).Type;
        //                if(childVariableType== VariableType.Effect)
        //                    decisionComponents.Add(new DecisionComponent<Edge>(counter++, new Edge(parentIndex, childIndex)));

        //            }

        //    return new ConstructionGraph<Edge>(decisionComponents.ToArray());
        //}

        public static ConstructionGraph<ClusterExampleAssignment> BuildIBClusteringConstructionGraph(Dataset dataset, int  clustersNumber)
        {
            int counter = 0;
            List<DecisionComponent<ClusterExampleAssignment>> decisionComponents=new List<DecisionComponent<ClusterExampleAssignment>>();
            foreach (Example example in dataset)
            {
                for (int clusterLabel = 0; clusterLabel < clustersNumber; clusterLabel++)
                {
                    DecisionComponent<ClusterExampleAssignment> component = new DecisionComponent<ClusterExampleAssignment>(counter++, new ClusterExampleAssignment(example.Index, clusterLabel));
                    decisionComponents.Add(component);
                }
            }
            return new ConstructionGraph<ClusterExampleAssignment>(decisionComponents.ToArray());
 
        }



        public static ConstructionGraph<int> BuildMBClusteringConstructionGraph(Dataset dataset, int clustersNumber)
        {
            int counter = 0;
            List<DecisionComponent<int>> decisionComponents = new List<DecisionComponent<int>>();
            foreach (Example example in dataset)
            {
               
                    DecisionComponent<int> component = new DecisionComponent<int>(counter++, example.Index);
                    decisionComponents.Add(component);
                
            }
            return new ConstructionGraph<int>(decisionComponents.ToArray());

        }

        public static ConstructionGraph<VariableTypeAssignment> BuildABCConstructionGraph(Metadata metadata)
        {
            int counter = 0;
            List<DecisionComponent<VariableTypeAssignment>> decisionComponents = new List<DecisionComponent<VariableTypeAssignment>>();
            foreach (DataMining.Data.Attribute attribute in metadata.Attributes)
            {
                DecisionComponent<VariableTypeAssignment> component1 = new DecisionComponent<VariableTypeAssignment>(counter++, new VariableTypeAssignment(attribute.Index, VariableType.Cause));
                decisionComponents.Add(component1);
                DecisionComponent<VariableTypeAssignment> component2 = new DecisionComponent<VariableTypeAssignment>(counter++, new VariableTypeAssignment(attribute.Index, VariableType.Effect));
                decisionComponents.Add(component2);
                DecisionComponent<VariableTypeAssignment> component3 = new DecisionComponent<VariableTypeAssignment>(counter++, new VariableTypeAssignment(attribute.Index, VariableType.None));
                decisionComponents.Add(component3);

            }

            return new ConstructionGraph<VariableTypeAssignment>(decisionComponents.ToArray());
        }

     
    }
}
