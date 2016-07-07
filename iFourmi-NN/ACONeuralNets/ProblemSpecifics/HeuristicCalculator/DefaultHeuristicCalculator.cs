using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Data;


namespace iFourmi.ACONeuralNets.ProblemSpecifics.HeuristicCalculators
{
    public class DefaultHeuristicCalculator<T> : IHeuristicsCalculator<T>
    {
        #region Data Members

        private Dataset _dataset;

        #endregion

        #region Properties

        public DataMining.Data.Dataset Dataset
        {
            get { return this._dataset; }
            set { this._dataset = value; }
        }

        #endregion

        #region Constructor


        #endregion

        #region Mehods

        public void CalculateHeuristics(ACO.DecisionComponent<T> component)
        {
            component.Heuristic = 1;
        }

        #endregion
    }


}
