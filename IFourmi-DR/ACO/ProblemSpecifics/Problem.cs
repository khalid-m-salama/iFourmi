using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.ACO.ProblemSpecifics
{
    public class Problem<T>
    {
        #region Data Members

        IComponentInvalidator<T> _invalidator;

        IHeuristicsCalculator<T> _calculator;

        ISolutionQualityEvaluator<T> _evaluator;

        ILocalSearch<T> _localSearch;

        
        #endregion
        
        #region Properties

        public IComponentInvalidator<T> ComponentInvalidator
        {
            get { return this._invalidator; }
        }

        public IHeuristicsCalculator<T> HeuristicsCalculator
        {
            get { return this._calculator; }
        }

        public ISolutionQualityEvaluator<T> SolutionQualityEvaluator
        {
            get { return this._evaluator; }
        }

        public ILocalSearch<T> LocalSearch
        {
            get { return this._localSearch; }
        }




        #endregion

        #region Constructor

        public Problem(IComponentInvalidator<T> invalidator, IHeuristicsCalculator<T> calculator, ISolutionQualityEvaluator<T> evaluator, ILocalSearch<T> localSearch)
        {
            this._invalidator = invalidator;
            this._calculator = calculator;
            this._evaluator = evaluator;
            this._localSearch = localSearch;
        }

        #endregion

    }
}
