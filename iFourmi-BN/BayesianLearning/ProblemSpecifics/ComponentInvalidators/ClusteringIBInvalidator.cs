using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Model;

namespace iFourmi.BayesianLearning.ProblemSpecifics.ComponentInvalidators
{
    public class ClusteringIBInvalidator : IComponentInvalidator<ClusterExampleAssignment>
    {
        #region Data Members
        private int _clustersNumber;
        #endregion

        public int ClustersNumber
        {
            get { return this._clustersNumber; }
            set { this._clustersNumber = value; }
        }

        public void Invalidate(ACO.DecisionComponent<ClusterExampleAssignment> component, ACO.Solution<ClusterExampleAssignment> solution, ACO.ConstructionGraph<ClusterExampleAssignment> graph)
        {
            int index = component.Element.ExampleID;
            int elementIndex = (index + 1) * this._clustersNumber;
    
            for (int i = elementIndex - this._clustersNumber; i < elementIndex; i++)
                graph.Components[i].IsValid = false;
            

            if (elementIndex < graph.Components.Length)
                for (int i = elementIndex; i < elementIndex + this._clustersNumber; i++)
                    graph.Components[i].IsValid = true;
            
            
        }
    }
}
