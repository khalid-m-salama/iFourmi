using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Model;

namespace iFourmi.BayesianLearning.ProblemSpecifics.ComponentInvalidators
{
    public class ClusteringMBInvalidator : IComponentInvalidator<int>
    {
        #region Data Members
        private int _clustersNumber;
        #endregion

        public int ClustersNumber
        {
            get { return this._clustersNumber; }
            set { this._clustersNumber = value; }
        }

        public void Invalidate(DecisionComponent<int> component, Solution<int> solution, ConstructionGraph<int> graph)
        {

            graph.Components[component.Index].IsValid = false;
            if (solution.Components.Count == this._clustersNumber)
                foreach (DecisionComponent<int> current in graph.Components)
                    current.IsValid = false;


        }
    }
}
