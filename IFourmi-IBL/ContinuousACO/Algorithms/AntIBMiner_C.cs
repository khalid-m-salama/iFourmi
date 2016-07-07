using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ContinuousACO.ProblemSpecifics;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.Model.IBLearning;

namespace iFourmi.ContinuousACO.Algorithms
{
    public class AntIBMiner_C:ACO_C,IClassificationAlgorithm
    {
        #region Data Members

        protected DataMining.Data.Dataset _trainingSet;

        #endregion

        public DataMining.Data.Dataset Dataset
        {
            get
            {
                return this._trainingSet;
            }
            set
            {
                this._trainingSet = value;
            }
        }

        public AntIBMiner_C(int maxIterations, int colonySize, int convergenceIterations, Problem<double> problem, int problemSize, bool useBestSolution, bool usePheromoneInverse, DataMining.Data.Dataset trainingSet)
            :base(maxIterations,colonySize,convergenceIterations,problem,problemSize,useBestSolution,usePheromoneInverse)
        {
            this._trainingSet = trainingSet;
        }

        public override void Work()
        {
            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {
                if (this._currentIteration % 5 == 0)
                    SetLearningAndValidationSets();

                this.CreateSolution();

                if (this.IsConverged())
                    break;

                this.UpdateBestAnt();

                this.UpdatePheromoneLevels();

            }
        }


        private void SetLearningAndValidationSets()
        {
            DataMining.Data.Dataset[] datasets = this._trainingSet.SplitRandomly(0.8);
            ((IBClassificationQualityEvaluator)this.Problem.SolutionQualityEvaluator).LearningSet = datasets[0];
            ((IBClassificationQualityEvaluator)this.Problem.SolutionQualityEvaluator).ValidationSet = datasets[1];

        }

        public DataMining.Model.IClassifier CreateClassifier()
        {
            this.Work();

            double[] weights = this.GlobalBest.ToList().ToArray();

            AbstractLazyClassifier clasifier = ((IBClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).Classifier;
            clasifier.Weights = weights;

            return clasifier;
        }
    }
}
