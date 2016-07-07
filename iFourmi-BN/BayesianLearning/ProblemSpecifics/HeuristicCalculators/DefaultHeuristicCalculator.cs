using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Data;
using iFourmi.BayesianNetworks.Model;

namespace iFourmi.BayesianLearning.ProblemSpecifics.HeuristicCalculators
{
    public class DefaultHeuristicCalculator<T> : IHeuristicValueCalculator<T>
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

        public void CalculateHieristicValue(ACO.DecisionComponent<T> Element)
        {
            Element.Heuristic = 1;
        }

        #endregion
    }


}
