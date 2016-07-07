using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Data;


namespace iFourmi.ACONeuralNets.ProblemSpecifics.HeuristicCalculators
{
    public class NNConnectionHeuristicCalculator : IHeuristicsCalculator<ConnectionDC>
    {
        #region Data Members

        private Dataset _dataset;

        private double _ratio;

        #endregion

        #region Properties

        public DataMining.Data.Dataset Dataset
        {
            get { return this._dataset; }
            set { this._dataset = value; }
        }

        #endregion

        #region Constructor

        public NNConnectionHeuristicCalculator(double ratio)
        {
            this._ratio = ratio;
        }

        #endregion

        #region Mehods

        public void CalculateHeuristics(ACO.DecisionComponent<ConnectionDC> component)
        {
            if (component.Element.Connection.FromLayerType == NeuralNetworks.Model.LayerType.Input)
                if (component.Element.Connection.ToLayerType == NeuralNetworks.Model.LayerType.Hidden)
                    if (component.Element.Include)
                        component.Heuristic = this._ratio;
                    else
                        component.Heuristic = 1-_ratio;
                else
                    component.Heuristic = 1;

            else if (component.Element.Connection.FromLayerType == NeuralNetworks.Model.LayerType.Hidden)
                if (component.Element.Connection.ToLayerType == NeuralNetworks.Model.LayerType.Output)
                    if (component.Element.Include)
                        component.Heuristic = this._ratio;
                    else
                        component.Heuristic = 1-this._ratio;
                else
                    component.Heuristic = 1;
            else
                component.Heuristic = 1;
               


        }

        #endregion
    }


}
