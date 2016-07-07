using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ContinuousACO.ProblemSpecifics;

namespace iFourmi.ContinuousACO.Algorithms
{
    public class PBACO_C : ACO.AntColony<double>
    {
        #region Data members

        int _problemSize;

        double[][] _pheromone = null;
        double[][] _inverse = null;

        double[] _qualityArray = null;
        
        int _currentVectorIndex;

        Solution<double> _iterationBestSolution;
        Solution<double> _bestSolution;
        
        double _bestFitness = double.MaxValue;
        
        double[] _stdv = null;

        bool _terminate = false;

        #endregion

        #region Properties

        public double BestFitness
        {
            get
            {
                return this._bestFitness;

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

        public PBACO_C(int maxIterations, int colonySize, int convergenceIterations, Problem<double> problem, int problemSize)
            : base(maxIterations, colonySize, convergenceIterations, problem)
        {
            this._problemSize = problemSize;

            this._pheromone = new double[colonySize][];
            this._inverse = new double[colonySize][];

            this._bestSolution = new Solution<double>();
            this._stdv = new double[problemSize];
            this._qualityArray = new double[colonySize];
            this.Initialize();
            
        }

        #endregion

        #region Methods

      
        public override void Initialize()
        {

            AbstractContinousOptimizationEvaluator evaluator = this._problem.SolutionQualityEvaluator as AbstractContinousOptimizationEvaluator;
            double mean = (evaluator.MaximumVariableValue + evaluator.MinimumVariableValue) / 2;
            double initialStdv = (evaluator.MaximumVariableValue - mean) / 3;

            for (int vectorIndex = 0; vectorIndex < this._colonySize; vectorIndex++)
            {
                this._pheromone[vectorIndex] = new double[this._problemSize];
                this._inverse[vectorIndex] = new double[this._problemSize];

                for (int variableIndex = 0; variableIndex < this._problemSize; variableIndex++)
                {
                    this._pheromone[vectorIndex][variableIndex] = RandomUtility.Random(evaluator.MinimumVariableValue, evaluator.MaximumVariableValue);
                    this._inverse[vectorIndex][variableIndex] = -this._pheromone[vectorIndex][variableIndex];
                    this._stdv[variableIndex] = initialStdv;
                }

                Solution<double> solution=Solution<double>.FromList(this._pheromone[vectorIndex].ToList());
                this.EvaluateSolutionQuality(solution);
                this._qualityArray[vectorIndex] = solution.Quality;
            }
        }

        public override void Work()
        {
            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {
                this.CreateSolution();

                if (this._terminate)
                    break;

                this.UpdateBestAnt();

                //-----------------------------------------------------------------------------------------------------------
                Console.WriteLine("Iteration:" + this._currentIteration.ToString() + " - Fitness:" + this._bestFitness.ToString());
                //-----------------------------------------------------------------------------------------------------------

                this.UpdatePheromoneLevels();

                if (OnPostColonyIteration != null)
                    this.OnPostColonyIteration(this, null);
            }
        }

        public override void CreateSolution()
        {
            this._iterationBestSolution = null;
            this._currentVectorIndex = this.ProbablisticallySelectPheromoneVector();

            for (int antIndex = 0; antIndex < this._colonySize; antIndex++)
            {
                Solution<double> current = Solution<double>.FromList(this._pheromone[this._currentVectorIndex].ToList());
                this.EvaluateSolutionQuality(current);
                this._qualityArray[this._currentVectorIndex] = current.Quality;


                if (this._iterationBestSolution == null || current.Quality > this._iterationBestSolution.Quality)
                    this._iterationBestSolution = current;

                Solution<double> solution1 = CreateAntSolution(this._pheromone[this._currentVectorIndex]);
                this.EvaluateSolutionQuality(solution1);

                if (this._iterationBestSolution == null || solution1.Quality > this._iterationBestSolution.Quality)
                    this._iterationBestSolution = solution1;

                Solution<double> solution2 = CreateAntSolution(this._inverse[this._currentVectorIndex]);
                this.EvaluateSolutionQuality(solution2);

                if (this._iterationBestSolution == null || solution2.Quality > this._iterationBestSolution.Quality)
                    this._iterationBestSolution = solution2;

            }
            
        }

        private int ProbablisticallySelectPheromoneVector()
        {
            double sum = 0;
            
            for (int vectorIndex = 0; vectorIndex < this._colonySize; vectorIndex++)
                sum += this._qualityArray[vectorIndex];
            
            double random = RandomUtility.Random(0,sum);

            double cumulativeSum = 0;

            int selectedIndex = -1;

            for (int vectorIndex = 0; vectorIndex < this._colonySize; vectorIndex++)
            {
                cumulativeSum += this._qualityArray[vectorIndex];
                if (cumulativeSum > random)
                {
                    selectedIndex = vectorIndex;
                    break;
                }
            }

            return selectedIndex;
        }

        private Solution<double> CreateAntSolution(double[] vector)
        {
            Solution<double> solution = new Solution<double>();

            if (vector.Length == this._problemSize)
            {
                for (int index = 0; index < this._problemSize; index++)
                {
                    double value = RandomUtility.Gaussian(vector[index], this._stdv[index]);
                    solution.Components.Add(new DecisionComponent<double>(index, value));
                }
            }
            else
            {
                AbstractContinousOptimizationEvaluator evaluator = this._problem.SolutionQualityEvaluator as AbstractContinousOptimizationEvaluator;

                for (int index = 0; index < this._problemSize; index++)
                {
                    double value = RandomUtility.Random(evaluator.MinimumVariableValue, evaluator.MinimumVariableValue);
                    solution.Components.Add(new DecisionComponent<double>(index, value));
                }

            }

            return solution;

        }

        public override void EvaluateSolutionQuality(ACO.Solution<double> solution)
        {
            if (solution.Components.Count != this._problemSize)
                return;


            this._problem.SolutionQualityEvaluator.EvaluateSolutionQuality(solution);
            this.EvaluationCounter++;

            double fitness = ((OptimizationFunctionQualityEvaluator)this._problem.SolutionQualityEvaluator).Function.Calculate(solution.ToList().ToArray());
            this.CheckTerminationCondition(fitness);

        }


        public override void UpdatePheromoneLevels()
        {

            AbstractContinousOptimizationEvaluator evaluator = this._problem.SolutionQualityEvaluator as AbstractContinousOptimizationEvaluator;

            double mean = (evaluator.MaximumVariableValue + evaluator.MinimumVariableValue) / 2;
            double initialStdv = (evaluator.MaximumVariableValue - mean) / 3;
            double finalStdv = 0.0001;// * (function.MaximumVariableValue - function.MinimumVariableValue) / 2;

            List<double> iterationBest = this._iterationBestSolution.ToList();
            List<double> best = this._iterationBestSolution.ToList();


            for (int index = 0; index < this._problemSize; index++)
            {
                double value = this._pheromone[this._currentVectorIndex][index];

                double factor1 = (this._iterationBestSolution.Quality - this._qualityArray[this._currentVectorIndex]) / this._iterationBestSolution.Quality + this._qualityArray[this._currentVectorIndex];
                double factor2 = (this._bestSolution.Quality - this._qualityArray[this._currentVectorIndex]) / this._bestSolution.Quality + this._qualityArray[this._currentVectorIndex];

                double step1 = (iterationBest[index] - this._pheromone[this._currentVectorIndex][index]) * factor1;
                double step2 = (best[index] - this._pheromone[this._currentVectorIndex][index]) * factor2;


                this._pheromone[this._currentVectorIndex][index] = iterationBest[index];//value + step1 + step2;

                this._inverse[this._currentVectorIndex][index] = -this._pheromone[this._currentVectorIndex][index];


                double ratio = Math.Pow((finalStdv / initialStdv), (1.0 / (this.MaxIterations)));

                this._stdv[index] *= ratio;
            }

         
        }

      

        protected override void UpdateBestAnt()
        {
            if (this._iterationBestSolution.Quality > this._bestSolution.Quality)
                this._bestSolution = _iterationBestSolution.Clone() as Solution<double>;
        }

        private void CheckTerminationCondition(double fitness)
        {
            if (fitness < this._bestFitness)
                this._bestFitness = fitness;

            if (this._bestFitness <= Math.Pow(10, -4))
                this._terminate = true;
            else
                this._terminate = false;

        }

        public override void PostProcessing()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

