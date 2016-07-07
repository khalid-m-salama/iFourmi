using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ContinuousACO.ProblemSpecifics;

namespace iFourmi.ContinuousACO.Algorithms
{
    public class ACO_C : ACO.AntColony<double>
    {
        #region Data members

        int _problemSize;

        double[] _pheromone = null; 
        double[] _pheromoneInverse = null;

        Solution<double> _iterationBestSolution;
        Solution<double> _bestSolution;
        
        double _bestFitness = double.MaxValue;

        double _pheromonVectorQuality=0;
        
        double[] _stdv = null;
                
        bool _useBestSolution = false;
        bool _usePheromoneInverse = false;

        #endregion

        #region Properties

        public double BestFitness
        {
            get
            {
                return this._bestFitness;
 
            }
            
        }

        public Solution<double> GlobalBest
        {
            get
            {
                return this._bestSolution;
            }
        }

        public Solution<double> IterationBest
        {
            get
            {
                return this._iterationBestSolution;
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

        public ACO_C(int maxIterations, int colonySize, int convergenceIterations, Problem<double> problem, int problemSize, bool useBestSolution,bool usePheromoneInverse)
            : base(maxIterations, colonySize, convergenceIterations, problem)
        {
            this._problemSize = problemSize;

            this._pheromone = new double[problemSize];
            this._pheromoneInverse = new double[problemSize];

            this._bestSolution = new Solution<double>();
            

            this._stdv = new double[problemSize];

            this._useBestSolution = useBestSolution;
            this._usePheromoneInverse = usePheromoneInverse;

            this.Initialize();

         
        }

        #endregion

        #region Methods

        public override void Initialize()
        {

            AbstractContinousOptimizationEvaluator evaluator = this._problem.SolutionQualityEvaluator as AbstractContinousOptimizationEvaluator;

            double mean = (evaluator.MaximumVariableValue + evaluator.MinimumVariableValue) / 2;
            double initialStdv = (evaluator.MaximumVariableValue - mean) / 3;

            for (int index = 0; index < this._problemSize; index++)
            {
                this._pheromone[index] = RandomUtility.Random(evaluator.MinimumVariableValue, evaluator.MaximumVariableValue);
                this._pheromoneInverse[index] = this._pheromone[index] * -1;
                             
                this._stdv[index] = initialStdv;
            }
        }

        public override void Work()
        {
            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {               
                this.CreateSolution();

                //if (this._terminate)
                if (this.IsConverged())
                    break;

                this.UpdateBestAnt();

                this.UpdatePheromoneLevels();

                if (OnPostColonyIteration != null)
                    this.OnPostColonyIteration(this, null);
            }
        }

        private bool CheckTerminationCondition()
        {
            bool terminate=false;
            if (this._bestFitness <= Math.Pow(10, -4))
                terminate=true;
            else
                terminate= false;
            return terminate;

        }

        public override void CreateSolution()
        {
            if (this._iterationBestSolution == null)
            {
                Solution<double> solution = Solution<double>.FromList(this._pheromone.ToList());
                this.EvaluateSolutionQuality(solution);
                this._iterationBestSolution = solution;
            }


            if (_usePheromoneInverse)
            {
                Solution<double> solutionInverse = Solution<double>.FromList(this._pheromoneInverse.ToList());
                this.EvaluateSolutionQuality(solutionInverse);

                //if (CheckTerminationCondition())
                   // return;

                if (solutionInverse.Quality < this._iterationBestSolution.Quality)
                    this._iterationBestSolution = solutionInverse;
            }

            for (int antIndex = 0; antIndex < this.ColonySize; antIndex++)
            {
                Solution<double> solution1 = CreateAntSolution(this._pheromone);
                this.EvaluateSolutionQuality(solution1);
                if (this._iterationBestSolution == null || solution1.Quality > this._iterationBestSolution.Quality)
                    this._iterationBestSolution = solution1;

                //if (CheckTerminationCondition())
                  //  break;

                if (_usePheromoneInverse)
                {
                    Solution<double> solution2 = CreateAntSolution(this._pheromoneInverse);
                    this.EvaluateSolutionQuality(solution2);
                    if (this._iterationBestSolution == null || solution2.Quality > this._iterationBestSolution.Quality)
                        this._iterationBestSolution = solution2;

                    //if (CheckTerminationCondition())
                        //break;
                }

                if (_useBestSolution)
                {

                    Solution<double> solution3 = CreateAntSolution(this._bestSolution.ToList().ToArray());
                    this.EvaluateSolutionQuality(solution3);
                    if (this._iterationBestSolution == null || solution3.Quality > this._iterationBestSolution.Quality)
                        this._iterationBestSolution = solution3;

                    //if (CheckTerminationCondition())
                      //  break;
                }
             
                if (this.OnPostAntSolutionContruction != null)
                    this.OnPostAntSolutionContruction(this, null);
            }
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
            if (this._iterationBestSolution.Quality == this._lastQuality)
                this._convergenceCounter++;
            else
            {
                this._convergenceCounter = 0;
                this._lastQuality = this._iterationBestSolution.Quality; ;
            }

            if (_convergenceCounter == this._convergenceIterations)
                IsConverged = true;
            else
                IsConverged = false;

            return IsConverged;
        }

        protected override void UpdateBestAnt()
        {
            if (this._iterationBestSolution.Quality > this._bestSolution.Quality)
                this._bestSolution = _iterationBestSolution.Clone() as Solution<double>;           
        }

        public override void UpdatePheromoneLevels()
        {

            AbstractContinousOptimizationEvaluator evaluator = this._problem.SolutionQualityEvaluator as AbstractContinousOptimizationEvaluator;

            double mean = (evaluator.MaximumVariableValue + evaluator.MinimumVariableValue) / 2;
            double initialStdv = (evaluator.MaximumVariableValue - mean) / 3;
            double finalStdv = 0.0001; //*(function.MaximumVariableValue - function.MinimumVariableValue) / 2;

            List<double> iterationBest = this._iterationBestSolution.ToList();
            List<double> best = this._bestSolution.ToList();


            for (int index = 0; index < this._problemSize; index++)
            {
                double value=this._pheromone[index] ;

                double factor1 = (this._iterationBestSolution.Quality - this._pheromonVectorQuality) / (this._iterationBestSolution.Quality + this._pheromonVectorQuality);
                double factor2 = (this._bestSolution.Quality - this._pheromonVectorQuality) / (this._bestSolution.Quality + this._pheromonVectorQuality);
                
                double step1 = (iterationBest[index] - this._pheromone[index]) * factor1;
                double step2 = (best[index] - this._pheromone[index]) * factor2;


                //this._pheromone[index] = value + step1 + step2;

                //this._pheromoneInverse[index] = value - step1 - step2;

                this._pheromone[index] = iterationBest[index];
                this._pheromoneInverse[index] = -this._pheromone[index];

                double ratio=Math.Pow((finalStdv / initialStdv), (1.0 / (this.MaxIterations)));

                this._stdv[index] *= ratio;
            }

            //???
          //  this._bestSolution.Quality = 0;

        }

        public override void PostProcessing()
        {
            throw new NotImplementedException();
        }

        #endregion
    }


}
