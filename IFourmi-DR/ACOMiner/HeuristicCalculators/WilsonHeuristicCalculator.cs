using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model.IBLearning;
using iFourmi.DataMining.Model;


namespace iFourmi.ACOMiner.HeuristicCalculators
{

    public class IBLHeuristicsCalculator : DefaultHeuristicCalculator<DRComponent>
    {
        #region

        AbstractLazyClassifier _lazyClassifier;
        private Dictionary<int, double> _probabilities;

        #endregion

        #region Constructors


        public IBLHeuristicsCalculator(Dataset dataset, AbstractLazyClassifier lazyClassifier)
        {
            this._dataset = dataset;
            this._lazyClassifier = lazyClassifier;

            this._probabilities= new Dictionary<int, double>();

            for (int i = 0; i < this._dataset.Size; i++)
            {
                Prediction predicition = this._lazyClassifier.Classify(this._dataset[i]);
                this._probabilities.Add(i, predicition.Probabilities[predicition.Label]);
            }
          

        }

        #endregion

        #region Mehods

        public override void CalculateHeuristics(ACO.DecisionComponent<DRComponent> component)
        {

            if (component.Element.Include)
                component.Heuristic = this._probabilities[component.Element.ElemetIndex];
            else
                component.Heuristic = 1 - this._probabilities[component.Element.ElemetIndex];
        }

        #endregion
    }


}
