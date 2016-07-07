using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.OptimizationFunctions;
using iFourmi.ACO.ProblemSpecifics;

namespace iFourmi.ContinuousACO.ProblemSpecifics
{
    public class OptimizationFunctionQualityEvaluator:AbstractContinousOptimizationEvaluator
    {
        private IFunction _function;

        public OptimizationFunctions.IFunction Function
        {
            get
            {
                return this._function;
            }
        }

        public OptimizationFunctionQualityEvaluator(IFunction optimizationFunction)
            :base(optimizationFunction.MinimumVariableValue,optimizationFunction.MaximumVariableValue)
        {
            this._function = optimizationFunction;            
        }


        public override void EvaluateSolutionQuality(ACO.Solution<double> solution)
        {
            double value = this._function.Calculate(solution.ToList().ToArray());
            if(_function.Type==OptimizationType.Maximization)
                solution.Quality = value;
            else
                solution.Quality = 1.0/(1+value);

            this.CurrentFitness = value;
        }
    }
}
