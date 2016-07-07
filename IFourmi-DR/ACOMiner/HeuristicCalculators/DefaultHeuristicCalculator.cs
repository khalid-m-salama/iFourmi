using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Data;


namespace iFourmi.ACOMiner.HeuristicCalculators
{
    public class DefaultHeuristicCalculator<T> : IHeuristicsCalculator<T>
    {
        #region Data Members

        protected Dataset _dataset;

        #endregion

        #region Properties

        public DataMining.Data.Dataset Dataset
        {
            get { return this._dataset; }
            set { this._dataset = value; }
        }

        #endregion

        #region Constructor

        public DefaultHeuristicCalculator()
        {
 
        }

        public DefaultHeuristicCalculator(Dataset dataset)
        {
            this._dataset = dataset;
        }


        #endregion

        #region Mehods

        public virtual void CalculateHeuristics(ACO.DecisionComponent<T> component)
        {
            component.Heuristic = 1;
        }

        #endregion
    }


    public class DefaultDRHeuristicCalculator : DefaultHeuristicCalculator<DRComponent>
    {
       
        #region Mehods

        public override void CalculateHeuristics(ACO.DecisionComponent<DRComponent> component)
        {
            if (component.Element.Include)
                component.Heuristic = 0.66;
            else
                component.Heuristic = 0.33;
        }

        #endregion
    }


}
