using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Statistics;


namespace iFourmi.ACOMiner.HeuristicCalculators
{

    public class EntropyHeuristicsCalculator : DefaultHeuristicCalculator<DRComponent>
    {
        #region Data Members

        private double _maxEntopy;
        private Dictionary<int, double> _entopy;

        #endregion

        #region Constructors

        public EntropyHeuristicsCalculator(Dataset dataset)
        {
            this._dataset = dataset;

            this._entopy = new Dictionary<int, double>();

            foreach (DataMining.Data.Attribute attribute in this._dataset.Metadata.Attributes)
            {

                double entopy = _dataset.CalculateEntropy(attribute.Index);
                this._entopy.Add(attribute.Index, entopy);

                if (entopy > _maxEntopy)
                    _maxEntopy = entopy;
            }
        }

        #endregion

        #region Mehods

        public override void CalculateHeuristics(ACO.DecisionComponent<DRComponent> component)
        {
            
            double entopy = this._entopy[component.Element.ElemetIndex];

            if (component.Element.Include)
                component.Heuristic = 1 - entopy;
               //component.Heuristic = this._maxEntopy - entopy; 
            else
                component.Heuristic = entopy;
                


        }

        #endregion
    }


}
