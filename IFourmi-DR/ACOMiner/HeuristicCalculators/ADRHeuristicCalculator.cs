using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.ProximityMeasures;
using iFourmi.DataMining.Model.IBLearning;

namespace iFourmi.ACOMiner.HeuristicCalculators
{
    public class ADRHeuristicCalculator: DefaultHeuristicCalculator<DRComponent>
    {
        #region Data Members

        EntropyHeuristicsCalculator _entropyCalculator; 
        IBLHeuristicsCalculator _iblCalculator; 
                
        #endregion

        #region Constructor

        public ADRHeuristicCalculator()
        {
 
        }
 
      
        public void InitilizeHeuristicInformation(Dataset dataset, bool useAttributes, bool useInstances)
        {
            if (useAttributes)
            {
  
                this._entropyCalculator = new EntropyHeuristicsCalculator(dataset);

            }
            
            if(useInstances)
            {
                DefaultDistanceMeasure measure = new DefaultDistanceMeasure(2);
                KNearestNeighbours knn = new KNearestNeighbours(measure, _dataset, false);

                this._iblCalculator = new IBLHeuristicsCalculator(dataset, knn);
            }
        }

        public override void CalculateHeuristics(ACO.DecisionComponent<DRComponent> component)
        {

            if (component.Element.ElementType == DatasetElementType.Attribute)
                this._entropyCalculator.CalculateHeuristics(component);
            else
                this._iblCalculator.CalculateHeuristics(component);
            
        }

        #endregion
    }
}
