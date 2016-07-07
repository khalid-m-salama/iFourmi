using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ContinuousACO.ProblemSpecifics;

namespace iFourmi.ContinuousACO.Algorithms
{
    public class ACO_R : ACO.AntColony<double>
    {
        #region Data members

        protected int _problemSize;
        protected int _archiveSize;

        protected Solution<double>[] _solutionArchive = null;
        protected double[] _weightProbability = null;

        protected Solution<double> _iterationBest;
        protected Solution<double> _bestSolution;

        protected double _bestFitness=double.MaxValue;

        protected double _q;
        protected double _segma;

        protected double _terminateValue = double.MaxValue;

        protected bool _terminate = false;

        protected bool _terminationCondition2 = false;

        #endregion

        #region Properties

        public double BestFitness
        {
            get
            {
                return this._bestFitness;

            }

        }

        public Solution<double> IterationBest
        {
            get
            {
                return this._iterationBest;
            }
        }

        public Solution<double> GlobalBest
        {
            get
            {
                return this._solutionArchive[0];
            }
        }


        public int EvaluationCounter
        {
            get;
            private set;
        }

        #endregion

        #region Events

        public event EventHandler OnPostAntSolutionContruction;
        public event EventHandler OnPostColonyIteration;

        #endregion

        #region Constructors

        public ACO_R(int maxIterations, int colonySize, int convergenceIterations, Problem<double> problem, int problemSize, int archiveSize, double terminateValue, double q, double segma)
            : base(maxIterations, colonySize, convergenceIterations, problem)
        {
            this._problemSize = problemSize;
            this._archiveSize = archiveSize;

            this._q = q;

            this._segma = segma;

            this._solutionArchive = new Solution<double>[_archiveSize];

            this._weightProbability = new double[_archiveSize];

            this._terminateValue = terminateValue;


            this.Initialize();


        }

        #endregion

        #region Methods

        public override void Initialize()
        {
            AbstractContinousOptimizationEvaluator evaluator = this._problem.SolutionQualityEvaluator as AbstractContinousOptimizationEvaluator;

            double sum = 0;
            double dominator;
            double expo;
            for (int solutionIndex = 0; solutionIndex < this._archiveSize; solutionIndex++)
            {
                _solutionArchive[solutionIndex] = new Solution<double>();
                if (solutionIndex == 0)
                {
                    for (int index = 0; index < this._problemSize; index++)
                        _solutionArchive[solutionIndex].Components.Add(new DecisionComponent<double>(index, 1));
                }
                else
                {

                    for (int index = 0; index < this._problemSize; index++)
                    {
                        double value = RandomUtility.Random(evaluator.MinimumVariableValue, evaluator.MaximumVariableValue);
                        _solutionArchive[solutionIndex].Components.Add(new DecisionComponent<double>(index, value));
                    }
                }

                dominator = 1/((this._q * this._archiveSize) * Math.Sqrt(2 * Math.PI));
                
                expo = Math.Pow(solutionIndex, 2) / (2 * Math.Pow(this._q, 2) * Math.Pow(this._archiveSize, 2));

                _weightProbability[solutionIndex] = (dominator) * Math.Exp(-expo);
                sum += _weightProbability[solutionIndex];

                EvaluateSolutionQuality(_solutionArchive[solutionIndex]);
            }

            this._solutionArchive = this._solutionArchive.OrderByDescending(x => x.Quality).ToArray();

            for (int solutionIndex = 0; solutionIndex < this._archiveSize; solutionIndex++)            
                _weightProbability[solutionIndex] = _weightProbability[solutionIndex] / sum;

        }
        
        public override void Work()
        {
            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {
                this.CreateSolution();

                if (this.IsConverged())
                    break;

                //this.UpdateBestAnt();

                this.UpdatePheromoneLevels();

                if (OnPostColonyIteration != null)
                    this.OnPostColonyIteration(this, null);
            }
        }

        private bool CheckTerminationCondition()
        {
            bool terminate = false;
            if (this._bestFitness <= Math.Pow(10, -4))
                terminate = true;
            else
                terminate = false;
            return terminate;

        }

        public override void CreateSolution()
        {
            AbstractContinousOptimizationEvaluator evaluator = this._problem.SolutionQualityEvaluator as AbstractContinousOptimizationEvaluator;

            this._iterationBest = null;

            for (int i = 0; i < this._colonySize; i++)
            {
                int selectIndex = ProbablisticallySelectSolution();
                double std = 0;
                double value = 0;

                Solution<double> solution = new Solution<double>();

                for (int index = 0; index < this._problemSize; index++)
                {
                    std = 0;

                    for (int solutionIndex = 0; solutionIndex < this._archiveSize; solutionIndex++)                    
                        std += (Math.Abs(_solutionArchive[solutionIndex].Components[index].Element - _solutionArchive[selectIndex].Components[index].Element) / (this._archiveSize - 1));
                    
                    std *= this._segma;
                    value = RandomUtility.Gaussian(_solutionArchive[selectIndex].Components[index].Element, std);

                    if (value < evaluator.MinimumVariableValue)
                        value = evaluator.MinimumVariableValue;

                    else if (value > evaluator.MaximumVariableValue)
                        value = evaluator.MaximumVariableValue;


                    solution.Components.Add(new DecisionComponent<double>(index, value));
                }

                EvaluateSolutionQuality(solution);

                if (this._iterationBest == null || solution.Quality > this._iterationBest.Quality)
                    this._iterationBest = solution;
            }

            if (this.OnPostAntSolutionContruction != null)
                this.OnPostAntSolutionContruction(this, null);
        }

        private int ProbablisticallySelectSolution()
        {         
            double random = RandomUtility.Random(0, 1);

            double cumulativeSum = 0;

            int selectedIndex = -1;

            for (int vectorIndex = 0; vectorIndex < this._archiveSize; vectorIndex++)
            {
                cumulativeSum += this._weightProbability[vectorIndex];
                if (cumulativeSum > random)
                {
                    selectedIndex = vectorIndex;
                    break;
                }
            }

            return selectedIndex;
        }
                
        public override void EvaluateSolutionQuality(ACO.Solution<double> solution)
        {
            if (solution.Components.Count != this._problemSize)
                return;

            AbstractContinousOptimizationEvaluator evaluator = this._problem.SolutionQualityEvaluator as AbstractContinousOptimizationEvaluator;

            evaluator.EvaluateSolutionQuality(solution);
            this.EvaluationCounter++;

            double fitness = evaluator.CurrentFitness;

            if (this._bestFitness > fitness)
                this._bestFitness = fitness;

        }

        protected override bool IsConverged()
        {
            bool IsConverged = false;
            if (this.GlobalBest.Quality == this._lastQuality)
                this._convergenceCounter++;
            else
            {
                this._convergenceCounter = 0;
                this._lastQuality = this.GlobalBest.Quality;
            }

            if (_convergenceCounter == this._convergenceIterations)
                IsConverged = true;
            else
                IsConverged = false;

            return IsConverged;
        }

        protected override void UpdateBestAnt()
        {
            this._bestSolution = this._solutionArchive[0];
        }

        public override void UpdatePheromoneLevels()
        {
            if (this._iterationBest.Quality > this._solutionArchive[this._archiveSize - 1].Quality)            
                this._solutionArchive[this._archiveSize - 1] = this._iterationBest;

            this._solutionArchive = this._solutionArchive.OrderByDescending(x => x.Quality).ToArray();
        }

        public override void PostProcessing()
        {
            throw new NotImplementedException();
        }

        #endregion
    }


}
