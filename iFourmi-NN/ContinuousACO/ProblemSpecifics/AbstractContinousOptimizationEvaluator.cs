using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;

namespace iFourmi.ContinuousACO.ProblemSpecifics
{
    public abstract class AbstractContinousOptimizationEvaluator : ISolutionQualityEvaluator<double>
    {
        protected double _minimumVariableValue;
        protected double _maximumVariableValue;



        public double CurrentFitness
        {
            get;
            protected set;
        }

        public double MinimumVariableValue
        {
            get
            { 
                return this._minimumVariableValue; 
            }

        }

        public double MaximumVariableValue
        {
            get
            { 
                return this._maximumVariableValue;
            }
            
        }

        public AbstractContinousOptimizationEvaluator( double minimumValue,double maximumValue)
        {
            this._minimumVariableValue = minimumValue;
            this._maximumVariableValue = maximumValue;
        }

        public abstract void EvaluateSolutionQuality(ACO.Solution<double> solution);
       
    }
}
